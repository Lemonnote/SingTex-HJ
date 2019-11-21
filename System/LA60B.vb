Namespace Ports
  Public Class LA60B
    Private Enum State
      Idle
      Tx
      Rx
      Complete
    End Enum

    Public Enum Result
      Busy
      OK
      Fault
      HwFault
    End Enum

    Private ReadOnly stm_ As Stream
    Private rx_() As Byte, state_ As State, asyncResult_ As IAsyncResult, result_ As Result, errorCode_ As Integer
    Private oks_, faults_, hwFaults_ As Integer
    Private Structure WorkData : Dim StartRegister As String, StationNumber As Integer, IsRead As Boolean, CommandCode As Integer : End Structure
    Private work_ As WorkData
    Private ReadOnly writeOptimisation_ As New WriteOptimisation

    Public Sub New(ByVal stm As System.IO.Stream)
      stm.ReadTimeout = 300
      stm_ = New Stream(stm)
    End Sub

    Friend ReadOnly Property BaseStream() As Stream
      Get
        Return stm_
      End Get
    End Property

    Friend ReadOnly Property OKs() As Integer
      Get
        Return oks_
      End Get
    End Property
    Public ReadOnly Property Faults() As Integer
      Get
        Return faults_
      End Get
    End Property
    Public ReadOnly Property HwFaults() As Integer
      Get
        Return hwFaults_
      End Get
    End Property
    Private Sub SetResult(ByVal value As Result)
      result_ = value
      Select Case value
        Case Result.OK : oks_ += 1
        Case Result.Fault : faults_ += 1
        Case Else : hwFaults_ += 1
      End Select
    End Sub

    Private Shared Function ToHex1Byte(ByVal value As Integer) As Byte
      Return Convert.ToByte(If(value < 10, 48 + value, 65 + value - 10))
    End Function
    Private Shared Function ToHex4Bytes(ByVal value As UShort) As Byte()
      Dim ret(4 - 1) As Byte
      For i = 0 To 4 - 1
        ret(4 - 1 - i) = ToHex1Byte(value Mod 16US)
        value \= 16US
      Next i
      Return ret
    End Function
    Private Shared Function FromHex4Bytes(ByVal value() As Byte, ByVal index As Integer) As UShort
      Dim ret As UShort
      For i = 0 To 4 - 1
        ret = ret * 16US + FromHex1Byte(value(index + i))
      Next i
      Return ret
    End Function
    Private Shared Function FromHex1Byte(ByVal value As Byte) As Byte
      If value <= 57 Then Return CType(value - 48, Byte)
      Return CType(value - 65 + 10, Byte)
    End Function

    Private Sub BeginWriteAndRead(ByVal stationNumber As Integer, ByVal commandCode As Integer, ByVal tx() As Byte, ByVal rxCount As Integer)
      ' Make a packet around the given tx bytes
      Dim txLength = tx.Length, pack(txLength + 8 - 1) As Byte, packLength = pack.Length
      pack(0) = 2 ' STX
      pack(1) = ToHex1Byte(stationNumber \ 16)
      pack(2) = ToHex1Byte(stationNumber Mod 16)
      pack(3) = ToHex1Byte(commandCode \ 16)
      pack(4) = ToHex1Byte(commandCode Mod 16)
      For i = 0 To txLength - 1
        pack(5 + i) = tx(i)
      Next i
      ' Calculate simple adding checksum
      Dim cs As Integer
      For i = 0 To packLength - 3 - 1
        cs = (cs + pack(i)) And 255
      Next i
      pack(packLength - 3) = ToHex1Byte(cs \ 16)
      pack(packLength - 2) = ToHex1Byte(cs Mod 16)
      pack(packLength - 1) = 3 ' ETX

      ' Make a suitable rx_
      rx_ = New Byte(rxCount + 9 - 1) {}

      ' AndAlso then begin the write of the bytes
      state_ = State.Tx : stm_.Flush() : asyncResult_ = stm_.BeginWrite(pack, 0, pack.Length, Nothing, Nothing)
    End Sub

    Private Enum ErrorCode
      None
      IllegalValue = 2
      IllegalFormat = 4
      LadderChecksum
      IdMismatch
      LadderSyntax
      UnsupportedFunction = 9
      IllegalAddress
    End Enum

    Private Sub RunStateMachine() ' ByVal stationAddress As Integer, ByVal firstRegister As String, ByVal isRead As Boolean)
      Select Case state_
        Case State.Tx
          ' Wait for the tx to complete
          If Not asyncResult_.IsCompleted Then Exit Sub
          stm_.EndWrite(asyncResult_)  ' tidy up

          ' Start reading, feeding in the rx timeouts each time
          asyncResult_ = stm_.BeginRead(rx_, 0, rx_.Length, Nothing, Nothing)
          state_ = State.Rx : GoTo stateRx ' go straight on to the next state

        Case State.Rx
stateRx:
          If Not asyncResult_.IsCompleted Then Exit Sub ' it'll be completed soon

          Dim rxCount = rx_.Length, red = stm_.EndRead(asyncResult_) : asyncResult_ = Nothing
          If red = -1 Then
            SetResult(Result.HwFault)
          ElseIf red <> rxCount Then        ' not enough bytes ?
            SetResult(Result.Fault)
          Else
            ' Check the incoming checksum
            Dim cs As Integer
            For i = 0 To rxCount - 3 - 1
              cs = (cs + rx_(i)) And 255
            Next i
            If rx_(rxCount - 3) <> ToHex1Byte(cs \ 16) OrElse rx_(rxCount - 2) <> ToHex1Byte(cs Mod 16) Then
              SetResult(Result.Fault)  ' incoming checksum
            Else
              errorCode_ = FromHex1Byte(rx_(5))
              If errorCode_ = 0 Then
                SetResult(Result.OK)
              Else
                SetResult(Result.Fault)  ' some error code - see Enum above
              End If
            End If
          End If
          state_ = State.Complete
      End Select
    End Sub



    ' We always ignore element 0 of the 'values' array - this is to make it easier for the engineer writing
    ' the control-code who calls this.
    Public Function Read(ByVal stationNumber As Integer, ByVal startRegister As String, ByVal values As Array) As Result
      ' Start a completely new task
      If state_ = State.Idle Then
        work_.StationNumber = stationNumber : work_.StartRegister = startRegister : work_.IsRead = True
        Dim isBits = (values.GetType.GetElementType Is GetType(Boolean)), _
            count = values.Length - 1  ' one less because we ignore element 0

        Select Case startRegister.Chars(0)
          Case "X"c, "Y"c, "M"c, "S"c, "T"c, "C"c
            If Not isBits Then Throw New NotSupportedException
            Dim lenRegister = RegisterLength(startRegister)
            Dim tx(2 + lenRegister - 1) As Byte
            tx(0) = ToHex1Byte(count \ 16)
            tx(1) = ToHex1Byte(count Mod 16)
            System.Text.ASCIIEncoding.ASCII.GetBytes(RegisterToString(startRegister)).CopyTo(tx, 2)
            work_.CommandCode = &H44  ' read bit registers
            BeginWriteAndRead(stationNumber, work_.CommandCode, tx, count)

          Case Else
            ' Get the number of 16-bit registers involved
            Dim shortCount As Integer
            If isBits Then
              shortCount = count \ 16 : If count Mod 16 <> 0 Then shortCount += 1
            Else
              shortCount = count
            End If

            ' Make the request
            Dim lenRegister = RegisterLength(startRegister)
            Const lenData As Integer = 4 ' TODO: the 4 here assumes not a 32-bit register
            Dim tx(2 + lenRegister - 1) As Byte
            tx(0) = ToHex1Byte(shortCount \ 16)
            tx(1) = ToHex1Byte(shortCount Mod 16)
            System.Text.ASCIIEncoding.ASCII.GetBytes(RegisterToString(startRegister)).CopyTo(tx, 2)
            work_.CommandCode = &H46  ' read word registers
            BeginWriteAndRead(stationNumber, work_.CommandCode, tx, shortCount * lenData)
        End Select
      End If


      ' See if we're finished
      RunStateMachine()

      If state_ <> State.Complete OrElse work_.StartRegister <> startRegister OrElse _
         work_.IsRead <> True Then Return Result.Busy
      state_ = State.Idle
      If result_ = Result.OK Then
        Dim count = values.Length - 1  ' one less because we ignore element 0

        ' Ok, store these new values 
        Select Case work_.StartRegister.Chars(0)
          Case "X"c, "Y"c, "M"c, "S"c, "T"c, "C"c
            Dim booleanArray = DirectCast(values, Boolean())
            For i = 0 To count - 1
              booleanArray(i + 1) = (rx_(6 + i) = &H31)
            Next i
          Case Else
            Dim isBits = (values.GetType.GetElementType Is GetType(Boolean))
            ' Get the number of 16-bit registers involved
            Dim shortCount As Integer
            If isBits Then
              shortCount = count \ 16 : If count Mod 16 <> 0 Then shortCount += 1
            Else
              shortCount = count
            End If

            ' Convert back from hex
            Dim data(shortCount - 1) As UShort
            For i As Integer = 0 To shortCount - 1
              data(i) = FromHex4Bytes(rx_, 6 + 4 * i)
            Next i

            Dim uShortArray = TryCast(values, UShort())
            If uShortArray IsNot Nothing Then
              For i As Integer = 0 To count - 1
                uShortArray(i + 1) = data(i)
              Next i
            Else
              If isBits Then
                Dim booleanArray = TryCast(values, Boolean())

                For i As Integer = 0 To count - 1
                  booleanArray(i + 1) = (data(i \ 16) And (1 << (i And 15))) <> 0
                Next i
              Else
                Dim shortArray() As Short = DirectCast(values, Short())
                For i As Integer = 0 To count - 1
                  shortArray(i + 1) = System.BitConverter.ToInt16(System.BitConverter.GetBytes(data(i)), 0)
                Next i
              End If
            End If
        End Select
      End If
      Return result_
    End Function

    Public Function Write(ByVal stationNumber As Integer, ByVal startRegister As String, ByVal values As Array, ByVal writeMode As WriteMode) As Result
      If state_ = State.Idle Then

        ' Optionally, do write-optimisation, meaning we usually do not write the same values to the same
        ' registers in the same slave.
        If writeMode = Ports.WriteMode.Optimised AndAlso _
           writeOptimisation_.RecentlyWrote(values, stationNumber, startRegister) Then Return Result.OK

        work_.StationNumber = stationNumber : work_.StartRegister = startRegister : work_.IsRead = False
        Dim isBits = (values.GetType.GetElementType Is GetType(Boolean)), _
            count = values.Length - 1  ' one less because we ignore element 0

        Select Case startRegister.Chars(0)
          Case "X"c, "Y"c, "M"c, "S"c, "T"c, "C"c
            If Not isBits Then Throw New NotSupportedException
            Dim lenRegister = RegisterLength(startRegister)
            Dim tx(2 + lenRegister + count - 1) As Byte
            tx(0) = ToHex1Byte(count \ 16)
            tx(1) = ToHex1Byte(count Mod 16)
            System.Text.ASCIIEncoding.ASCII.GetBytes(RegisterToString(startRegister)).CopyTo(tx, 2)
            Dim boolArray = DirectCast(values, Boolean())
            For i = 0 To count - 1
              Dim byt As Byte
              If boolArray(i + 1) Then
                byt = &H31
              Else
                byt = &H30
              End If
              tx(2 + lenRegister + i) = byt
            Next i
            work_.CommandCode = &H45  ' write bit registers
            BeginWriteAndRead(stationNumber, work_.CommandCode, tx, 0)

            ' Everything else we do with a command 47
          Case Else
            ' Get the number of 16-bit registers involved
            Dim registers As Integer
            If isBits Then
              registers = count \ 16
              If count Mod 16 <> 0 Then registers += 1
            Else
              registers = count
            End If

            ' Make the request
            Dim lenRegister = RegisterLength(startRegister)
            Const lenData As Integer = 4 ' TODO: the 4 here assumes not a 32-bit register
            Dim tx(2 + lenRegister + registers * lenData - 1) As Byte
            tx(0) = ToHex1Byte(registers \ 16)
            tx(1) = ToHex1Byte(registers Mod 16)
            System.Text.ASCIIEncoding.ASCII.GetBytes(RegisterToString(startRegister)).CopyTo(tx, 2)

            Dim data(registers - 1) As UShort
            If isBits Then
              Dim valuesArray = DirectCast(values, Boolean())
              Dim dataIndex As Integer
              Do While dataIndex * 16 < count
                Dim startIndex = dataIndex * 16, _
                    encodeBitCount = Math.Min(count - startIndex, 16)
                data(dataIndex) = ToUShort(valuesArray, startIndex + 1, encodeBitCount)
                dataIndex += 1
              Loop
            Else
              Dim uShortArray = TryCast(values, UShort())
              If uShortArray IsNot Nothing Then
                For i = 0 To count - 1
                  data(i) = uShortArray(i + 1)
                Next i
              Else
                Dim valuesArray = DirectCast(values, Short())
                For i = 0 To count - 1
                  data(i) = System.BitConverter.ToUInt16(System.BitConverter.GetBytes(valuesArray(i + 1)), 0)
                Next i
              End If
            End If

            ' Put in the data
            For i = 0 To registers - 1
              Array.Copy(ToHex4Bytes(data(i)), 0, tx, 2 + lenRegister + i * lenData, lenData)
            Next i

            work_.CommandCode = &H47  ' write word registers
            BeginWriteAndRead(stationNumber, work_.CommandCode, tx, 0)
        End Select
      End If

      ' See if we're finished
      RunStateMachine()
      If state_ <> State.Complete OrElse work_.StartRegister <> startRegister OrElse _
         work_.IsRead <> False Then Return Result.Busy
      state_ = State.Idle
      Return result_
    End Function

    Private Shared Function RegisterLength(ByVal value As String) As Integer
      Select Case value.Chars(0)
        Case "X"c, "Y"c, "M"c, "S"c, "T"c, "C"c : Return 5
        Case Else : Return 6
      End Select
    End Function
    Private Shared Function RegisterToString(ByVal value As String) As String
      Dim regTypeLen = 1
      Do
        If Char.IsDigit(value, regTypeLen) Then Exit Do
        regTypeLen += 1
      Loop
      Return value.Substring(0, regTypeLen) & _
                                     Integer.Parse(value.Substring(regTypeLen)).ToString( _
                                     "0000000".Substring(0, RegisterLength(value) - regTypeLen))
    End Function

    Private Shared Function ToUShort(ByVal value() As Boolean, ByVal startIndex As Integer, ByVal length As Integer) As UShort
      Dim ret As UShort
      For i = length - 1 To 0 Step -1
        ret *= 2US
        If value(startIndex + i) Then ret += 1US
      Next i
      Return ret
    End Function
  End Class
End Namespace
