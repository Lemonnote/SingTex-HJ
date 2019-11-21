' TODO: watch out - there are 2 versions of this !
Namespace Ports
#Const ASYNCREADANDWRITE = Not CF
  Public Class SerialPort : Implements IDisposable
    Private ReadOnly portName_ As String, baudRate_ As Integer, parity_ As System.IO.Ports.Parity, _
                     dataBits_ As Integer, stopBits_ As System.IO.Ports.StopBits, async_ As Boolean
    Private hCom_ As IntPtr = INVALID_HANDLE_VALUE
    Private Shared INVALID_HANDLE_VALUE As New IntPtr(-1) ' TODO: is this really const ?
    Private ReadOnly stream_ As New SerialStream(Me)
    Private readTotalTimeoutConstant_ As Integer = 0, readIntervalTimeout_ As Integer, readTotalTimeoutMultiplier_ As Integer

    Public Sub New(ByVal portName As String, ByVal baudRate As Integer, ByVal parity As System.IO.Ports.Parity, _
                   ByVal dataBits As Integer, ByVal stopBits As System.IO.Ports.StopBits, Optional ByVal async As Boolean = True)
#If CF Then
    If Not portName.EndsWith(":") Then portName &= ":" ' need a trailing colon in CE
#End If

      portName_ = portName : baudRate_ = baudRate : parity_ = parity : dataBits_ = dataBits : stopBits_ = stopBits : async_ = async
    End Sub

    Private Function TryToOpen() As Boolean
      If IsOpen Then Return True ' already open

      ' Open the port, etc
      Const GENERIC_READ As Integer = &H80000000, GENERIC_WRITE As Integer = &H40000000, _
            OPEN_EXISTING As Integer = 3

#If ASYNCREADANDWRITE Then
      ' TODO: actually, it seems that we can do overlapped calls to ReadFile and WriteFile even
      ' if we leave this as 0.  In this case, the I/O operations are serialized (i.e. one has
      ' to complete before the other can start, but that's fine for us anyway).
      ' This means we can also call ReadFile non-overlapped which makes our 
      ' implementation of Read() and Write() cleaner.
      Dim openFlags As Integer
      If async_ Then openFlags = &H40000000 ' FILE_FLAG_OVERLAPPED
#Else
      Const openFlags As Integer = 0
#End If
      hCom_ = NativeMethods.CreateFile(PortName, GENERIC_READ Or GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING, openFlags, IntPtr.Zero)
      If Not IsOpen Then Return False
      '      Throw New System.IO.IOException("The port '" & PortName & "' could not be opened", New System.ComponentModel.Win32Exception)

      ' Set the timeouts
      SetTimeouts()

      ' Also set all the baud-rate stuff via a DCB
      ' The packing alignment needed is not currently possible with the CF, so 
      ' we have to resort to the BitConverter to get the job done
      Dim dcb(100 - 1) As Byte : NativeMethods.GetCommState(hCom_, dcb)
      System.BitConverter.GetBytes(BaudRate).CopyTo(dcb, 4)
      dcb(18) = CType(DataBits, Byte)
      dcb(19) = CType(Parity, Byte)
      Dim bStopBit As Byte
      Select Case StopBits
        Case System.IO.Ports.StopBits.Two : bStopBit = 2
        Case System.IO.Ports.StopBits.OnePointFive : bStopBit = 1
      End Select
      dcb(20) = bStopBit
      NativeMethods.SetCommState(hCom_, dcb)
      Return True
    End Function

    Private ReadOnly Property IsOpen() As Boolean
      Get
        Return Not hCom_.Equals(INVALID_HANDLE_VALUE)
      End Get
    End Property

    Private Sub Close()
      If IsOpen Then
        NativeMethods.CloseHandle(hCom_)
        hCom_ = INVALID_HANDLE_VALUE
      End If
    End Sub

    ' CF does not implement Component in the way we expect, so we do the stuff ourself here
    Protected Overrides Sub Finalize()
      Try
        Dispose(False)
      Finally
        MyBase.Finalize()
      End Try
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
      Dispose(True)
      GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
      If disposing Then Close()
    End Sub

    Public Overrides Function ToString() As String
      Return portName_ & "," & baudRate_.ToString(Globalization.CultureInfo.InvariantCulture) & ","c & parity_.ToString.Substring(0, 1) & ","c & _
             dataBits_.ToString(Globalization.CultureInfo.InvariantCulture) & ","c & CType(stopBits_, Integer).ToString(Globalization.CultureInfo.InvariantCulture)
    End Function

    Public ReadOnly Property PortName() As String
      Get
        Return portName_
      End Get
    End Property
    Public ReadOnly Property BaudRate() As Integer
      Get
        Return baudRate_
      End Get
    End Property
    Public ReadOnly Property Parity() As System.IO.Ports.Parity
      Get
        Return parity_
      End Get
    End Property
    Public ReadOnly Property DataBits() As Integer
      Get
        Return dataBits_
      End Get
    End Property
    Public ReadOnly Property StopBits() As System.IO.Ports.StopBits
      Get
        Return stopBits_
      End Get
    End Property

    Public ReadOnly Property BaseStream() As System.IO.Stream
      Get
        Return stream_
      End Get
    End Property

    ' This is handy so we can use a SerialPort anywhere where we need a Stream
    Public Shared Widening Operator CType(ByVal port As SerialPort) As System.IO.Stream
      Return port.stream_
    End Operator

    Public Property ReadTotalTimeoutConstant() As Integer
      Get
        Return readTotalTimeoutConstant_
      End Get
      Set(ByVal value As Integer)
        readTotalTimeoutConstant_ = value
        SetTimeouts()
      End Set
    End Property
    Public Property ReadIntervalTimeout() As Integer
      Get
        Return readIntervalTimeout_
      End Get
      Set(ByVal value As Integer)
        ' A small value like 2 can give unexpected results, so make this a minimum of 10ms
        ' In our modbus driver, for example, we do not use this at all, as we are content to 
        ' rely on the big overall timeout since we always know what the return packet
        ' length should be.  For other protocols, like Beacon, we do not have this luxury,
        ' and would need to use this again.
        readIntervalTimeout_ = Math.Max(value, 10)
        SetTimeouts()
      End Set
    End Property

    Public Property ReadTotalTimeoutMultiplier() As Integer
      Get
        Return readTotalTimeoutMultiplier_
      End Get
      Set(ByVal value As Integer)
        readTotalTimeoutMultiplier_ = value
        SetTimeouts()
      End Set
    End Property

    Private Sub SetTimeouts()
      If Not IsOpen Then Exit Sub
      Dim ct = New COMMTIMEOUTS
      ct.ReadIntervalTimeout = readIntervalTimeout_
      ct.ReadTotalTimeoutMultiplier = readTotalTimeoutMultiplier_
      ct.ReadTotalTimeoutConstant = readTotalTimeoutConstant_
      NativeMethods.SetCommTimeouts(hCom_, ct)
    End Sub

    <Flags()> _
    Public Enum PurgeActions
      TXAbort = 1   ' Kill the pending/current writes to the comm port.
      RXAbort = 2   ' Kill the pending/current reads to the comm port.
      TXClear = 4   ' Kill the transmit queue if there.
      RXClear = 8   ' Kill the typeahead buffer if there.
      All = TXAbort Or RXAbort Or TXClear Or RXClear
    End Enum

    Public Sub Purge(ByVal action As PurgeActions)
      NativeMethods.PurgeComm(hCom_, action)
    End Sub

    ' The implementation is actually inside the stream class
    Public Function Read(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
      Dim ret As Integer = stream_.Read(buffer, offset, count)
      Return ret
    End Function

    Public Sub Write(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer)
      stream_.Write(buffer, offset, count)
    End Sub

    Public Function GetStatistics() As SerialPortStatistics
      Dim ret As New SerialPortStatistics
      ret.BytesIn = stream_.TotalIn : ret.BytesOut = stream_.TotalOut
      Return ret
    End Function

    ' -----------------------------------------------------------
    Public Class SerialStream : Inherits System.IO.Stream
      Private ReadOnly port_ As SerialPort
      Private totalIn_, totalOut_ As Integer

      Public Sub New(ByVal port As SerialPort)
        port_ = port
      End Sub

      Public ReadOnly Property Port() As SerialPort
        Get
          Return port_
        End Get
      End Property
      Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then port_.Dispose()
        MyBase.Dispose(disposing)
      End Sub

      Public ReadOnly Property TotalIn() As Integer
        Get
          Return totalIn_
        End Get
      End Property
      Public ReadOnly Property TotalOut() As Integer
        Get
          Return totalOut_
        End Get
      End Property

      Public Overrides Function ToString() As String
        Return port_.ToString
      End Function

      Public Overrides Property ReadTimeout() As Integer
        Get
          Return port_.ReadTotalTimeoutConstant
        End Get
        Set(ByVal value As Integer)
          port_.ReadTotalTimeoutConstant = value
        End Set
      End Property

      Public Overrides Sub Flush()
        port_.Purge(PurgeActions.All)
      End Sub

      Public Overrides ReadOnly Property CanRead() As Boolean
        Get
          Return True
        End Get
      End Property
      Public Overrides ReadOnly Property CanWrite() As Boolean
        Get
          Return True
        End Get
      End Property
      Public Overrides ReadOnly Property CanSeek() As Boolean
        Get
          Return False
        End Get
      End Property
      Public Overrides ReadOnly Property Length() As Long
        Get
        End Get
      End Property

      Public Overrides Property Position() As Long
        Get
        End Get
        Set(ByVal value As Long)
        End Set
      End Property
      Public Overrides Function Seek(ByVal offset As Long, ByVal origin As System.IO.SeekOrigin) As Long
      End Function
      Public Overrides Sub SetLength(ByVal value As Long)
      End Sub

#If Not ASYNCREADANDWRITE Then
      ' The sync versions are straightforward
      Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
        Dim red As Integer
        If offset = 0 Then
          NativeMethods.ReadFile(port_.hCom_, buffer, count, red, Nothing)
        Else
          ' This is a nuisance, but we can do it
          Dim buf2(count - 1) As Byte
          NativeMethods.ReadFile(port_.hCom_, buf2, count, red, Nothing)
          Array.Copy(buf2, 0, buffer, offset, red)
        End If
        if red > 0 then totalIn_ += red
        Return red
      End Function
      Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
        Dim wrote As Integer
        If offset = 0 Then
          NativeMethods.WriteFile(port_.hCom_, buffer, count, wrote, Nothing)
        Else
          ' This is a nuisance, but we can do it
          Dim buf2(count - 1) As Byte : Array.Copy(buffer, offset, buf2, 0, count)
          NativeMethods.WriteFile(port_.hCom_, buf2, count, wrote, Nothing)
        End If
        totalOut_ += wrote
      End Sub
#Else
      ' On the desktop, but not on CF, we can provide our own versions of Begin/End/Read/Write, rather than
      ' relying on the delegate versions that IO.Stream will otherwise synthesise.
      ' Our versions are better because we can wait directly on the Win32 file implementation.
      Private ReadOnly asyncSerialPort_ As New AsyncSerialPort

      Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
        Dim red As Integer
        If port_.async_ Then
          red = EndRead(BeginRead(buffer, offset, count, Nothing, Nothing))
        Else
          If offset <> 0 Then Throw New NotSupportedException
          If NativeMethods.ReadFile(port_.hCom_, buffer, count, red, Nothing) = 0 Then
            Throw New System.ComponentModel.Win32Exception
          End If
        End If
        If red > 0 Then totalIn_ += red
        Return red
      End Function

      Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
        If port_.async_ Then
          EndWrite(BeginWrite(buffer, offset, count, Nothing, Nothing))
          totalOut_ += count
        Else
          If offset <> 0 Then Throw New NotSupportedException
          Dim wrote As Integer
          If NativeMethods.WriteFile(port_.hCom_, buffer, count, wrote, Nothing) = 0 Then
            Throw New System.ComponentModel.Win32Exception
          End If
          totalOut_ += wrote
        End If
      End Sub

      Public Overrides Function BeginRead(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer, _
                                          ByVal callback As AsyncCallback, ByVal state As Object) As IAsyncResult
        If Not port_.IsOpen Then Return New QuietAsyncResult(callback, state, Date.MinValue) ' don't even bother trying to open if not already open

        If Not port_.async_ Then Throw New NotImplementedException
        If callback IsNot Nothing Then Throw New NotSupportedException
        asyncSerialPort_.State = state
        Return asyncSerialPort_.BeginRead(port_.hCom_, buffer, offset, count)
      End Function
      Public Overrides Function EndRead(ByVal asyncResult As IAsyncResult) As Integer
        Dim ret As Integer = asyncSerialPort_.EndRead(asyncResult)
        ' If there has been a bad timeout problem, close the port
        If ret = -1 Then port_.Close()
        Return ret
      End Function
      Public Overrides Function BeginWrite(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer, _
                                           ByVal callback As AsyncCallback, ByVal state As Object) As IAsyncResult
        ' If the port is not open, and can not be opened, then just return something useless
        If Not port_.TryToOpen() Then Return New QuietAsyncResult(Nothing, state, Date.UtcNow.AddSeconds(1))

        If Not port_.async_ Then Throw New NotImplementedException
        If callback IsNot Nothing Then Throw New NotSupportedException
        asyncSerialPort_.State = state
        Return asyncSerialPort_.BeginWrite(port_.hCom_, buffer, offset, count)
      End Function
      Public Overrides Sub EndWrite(ByVal asyncResult As IAsyncResult)
        If TypeOf asyncResult Is QuietAsyncResult Then Exit Sub ' quietly do nothing

        ' If there has been a bad timeout problem, close the port
        If asyncSerialPort_.EndWrite(asyncResult) = -1 Then port_.Close()
      End Sub

      ' --------------------------------------------
      Private Class AsyncSerialPort
        Implements IAsyncResult

        Private overlapped_ As NativeMethods.OVERLAPPED, overlappedHandle_ As Runtime.InteropServices.GCHandle
        Private hCom_ As IntPtr, bufferHandle_ As Runtime.InteropServices.GCHandle, bufferIsPinned_ As Boolean
        Private state_ As Object, timeoutTime_ As Date, timedOut_ As Boolean

        ' Interop will pin buffers only during the function calls, so we have to pin the overlapped_ memory ourself
        Public Sub New()
          overlapped_ = New NativeMethods.OVERLAPPED
          overlappedHandle_ = Runtime.InteropServices.GCHandle.Alloc(overlapped_, Runtime.InteropServices.GCHandleType.Pinned)
        End Sub

        Private Const ERROR_IO_PENDING As Integer = 997  ' not an error, just a confirmation that the work is pending

        Public Function BeginRead(ByVal hCom As IntPtr, ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As IAsyncResult
          If offset <> 0 Then Throw New NotSupportedException
          PinBuffer(buffer)
          Dim red As Integer  ' always an overlapped read, so this will not be set
          Static count2 As Integer
          count2 = count
          If NativeMethods.ReadFile(hCom, buffer, count2, red, overlapped_) = 0 AndAlso _
               Runtime.InteropServices.Marshal.GetLastWin32Error <> ERROR_IO_PENDING Then
            UnpinBuffer()
            Return New QuietAsyncResult(Nothing, state_, Date.MinValue)
          End If
          timeoutTime_ = Date.UtcNow.AddSeconds(10) : hCom_ = hCom
          Return Me
        End Function

        Public Function EndRead(ByVal asyncResult As IAsyncResult) As Integer
          If TypeOf asyncResult Is QuietAsyncResult Then Return -1
          Dim bytes As Integer
          If timedOut_ Then
            bytes = -1
          Else
            NativeMethods.GetOverlappedResult(hCom_, overlapped_, bytes, True)  ' only returns when complete
          End If
          UnpinBuffer()
          Return bytes
        End Function

        Public Function BeginWrite(ByVal hCom As IntPtr, ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As IAsyncResult
          If offset <> 0 Then Throw New NotSupportedException

          ' Make the native, overlapped call
          PinBuffer(buffer)
          If NativeMethods.WriteFile(hCom, buffer, count, 0, overlapped_) = 0 AndAlso _
               Runtime.InteropServices.Marshal.GetLastWin32Error <> ERROR_IO_PENDING Then
            UnpinBuffer()
            Return New QuietAsyncResult(Nothing, state_, Date.MinValue)
          End If
          timeoutTime_ = Date.UtcNow.AddSeconds(2)  ' for safety, an overall timeout of our own, in case the Win serial driver fails us
          hCom_ = hCom
          Return Me
        End Function

        Public Function EndWrite(ByVal asyncResult As IAsyncResult) As Integer
          If TypeOf asyncResult Is QuietAsyncResult Then Return -1
          Dim bytes As Integer
          If timedOut_ Then
            bytes = -1
          Else
            NativeMethods.GetOverlappedResult(hCom_, overlapped_, bytes, True) ' only returns when complete
          End If
          UnpinBuffer()
          Return bytes
        End Function

        Private Sub PinBuffer(ByVal buffer() As Byte)
          bufferHandle_ = Runtime.InteropServices.GCHandle.Alloc(buffer, Runtime.InteropServices.GCHandleType.Pinned)
          bufferIsPinned_ = True
        End Sub
        Private Sub UnpinBuffer()
          If bufferIsPinned_ Then
            bufferHandle_.Free()
            bufferIsPinned_ = False
          End If
        End Sub
        Private ReadOnly Property IsCompleted() As Boolean Implements IAsyncResult.IsCompleted
          Get
            If overlapped_.HasIoCompleted Then
              timedOut_ = False : Return True
            End If
            If Date.UtcNow >= timeoutTime_ Then
              timedOut_ = True : Return True
            End If
            Return False
          End Get
        End Property

        ' The rest are not much interest
        Private ReadOnly Property AsyncState() As Object Implements IAsyncResult.AsyncState
          Get
            Return state_
          End Get
        End Property
        Public Property State() As Object
          Get
            Return state_
          End Get
          Set(ByVal value As Object)
            state_ = value
          End Set
        End Property
        Private ReadOnly Property AsyncWaitHandle() As Threading.WaitHandle Implements IAsyncResult.AsyncWaitHandle
          Get
            Throw New NotImplementedException
            '            Return New Threading.WaitHandle(overlapped_.hEvent)  ' TODO: almost
            Return Nothing
          End Get
        End Property
        Private ReadOnly Property CompletedSynchronously() As Boolean Implements IAsyncResult.CompletedSynchronously
          Get
          End Get
        End Property
      End Class
#End If
    End Class

    ' ---------------------------------------------------------
    Private NotInheritable Class NativeMethods
      Private Sub New()
      End Sub
#If CF Then
      Public Declare Function CloseHandle Lib "coredll" (ByVal handle As IntPtr) As <Runtime.InteropServices.MarshalAs( Runtime.InteropServices.UnmanagedType.Bool)>  Boolean
      Public Declare Function CreateEvent Lib "coredll" (ByVal eventAttributes As IntPtr, <Runtime.InteropServices.MarshalAs( Runtime.InteropServices.UnmanagedType.Bool)>  ByVal manualReset As Boolean, <Runtime.InteropServices.MarshalAs( Runtime.InteropServices.UnmanagedType.Bool)>  ByVal initialState As Boolean, ByVal name As String) As IntPtr
      Public Declare Function CreateFile Lib "coredll" ( _
          ByVal lpFileName As String, ByVal dwDesiredAccess As Integer, ByVal dwShareMode As Integer, _
          ByVal lpSecurityAttributes As IntPtr, ByVal dwCreationDisposition As Integer, _
          ByVal dwFlagsAndAttributes As Integer, ByVal hTemplateFile As IntPtr) As IntPtr
      Public Declare Function GetCommState Lib "coredll" (ByVal hCom As IntPtr, ByVal dcb() As Byte) As <Runtime.InteropServices.MarshalAs( Runtime.InteropServices.UnmanagedType.Bool)>  Boolean
      Public Declare Function PurgeComm Lib "coredll" (ByVal hCom As IntPtr, ByVal flags As Integer) As <Runtime.InteropServices.MarshalAs( Runtime.InteropServices.UnmanagedType.Bool)>  Boolean
      Public Declare Function ReadFile Lib "coredll" (ByVal hFile As IntPtr, ByVal buffer As Byte(), ByVal numberOfBytesToRead As Integer, ByRef numberOfBytesRead As Integer, ByVal unsupported As IntPtr) As Integer
      Public Declare Function SetCommState Lib "coredll" (ByVal hCom As IntPtr, ByVal dcb() As Byte) As <Runtime.InteropServices.MarshalAs( Runtime.InteropServices.UnmanagedType.Bool)>  Boolean
      Public Declare Function SetCommTimeouts Lib "coredll" (ByVal hCom As IntPtr, ByVal ct As COMMTIMEOUTS) As  <Runtime.InteropServices.MarshalAs( Runtime.InteropServices.UnmanagedType.Bool)>  Boolean
      Public Declare Function WriteFile Lib "coredll" (ByVal hFile As IntPtr, ByVal buffer As Byte(), ByVal numberOfBytesToWrite As Integer, ByRef numberOfBytesWritten As Integer, ByVal unsupported As IntPtr) As Integer
#Else
      Public Declare Function CloseHandle Lib "kernel32" (ByVal handle As IntPtr) As <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> Boolean
      Public Declare Auto Function CreateFile Lib "kernel32" ( _
          ByVal lpFileName As String, ByVal dwDesiredAccess As Integer, ByVal dwShareMode As Integer, _
          ByVal lpSecurityAttributes As IntPtr, ByVal dwCreationDisposition As Integer, _
          ByVal dwFlagsAndAttributes As Integer, ByVal hTemplateFile As IntPtr) As IntPtr
      Public Declare Function GetCommState Lib "kernel32" (ByVal hCom As IntPtr, ByVal dcb() As Byte) As <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> Boolean
      Public Declare Function PurgeComm Lib "kernel32" (ByVal hCom As IntPtr, ByVal flags As Integer) As <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> Boolean
      Public Declare Function SetCommState Lib "kernel32" (ByVal hCom As IntPtr, ByVal dcb() As Byte) As <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> Boolean
      Public Declare Function SetCommTimeouts Lib "kernel32" (ByVal hCom As IntPtr, ByVal ct As COMMTIMEOUTS) As <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> Boolean
      Public Declare Function ReadFile Lib "kernel32" (ByVal hFile As IntPtr, ByVal buffer As Byte(), ByVal numberOfBytesToRead As Integer, ByRef numberOfBytesRead As Integer, ByVal ol As OVERLAPPED) As Integer
      Public Declare Function WriteFile Lib "kernel32" (ByVal hFile As IntPtr, ByVal buffer As Byte(), ByVal numberOfBytesToWrite As Integer, ByRef numberOfBytesWritten As Integer, ByVal ol As OVERLAPPED) As Integer
      Public Declare Auto Function CreateEvent Lib "kernel32" (ByVal eventAttributes As IntPtr, <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> ByVal manualReset As Boolean, <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> ByVal initialState As Boolean, ByVal name As String) As IntPtr
      Public Declare Function GetOverlappedResult Lib "kernel32" (ByVal hFile As IntPtr, ByVal overlapped As OVERLAPPED, ByRef numberOfBytesTransferred As Integer, <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> ByVal wait As Boolean) As <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> Boolean

      ' ----------------------------------------------------------------
      <Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Sequential)> _
      Public Class OVERLAPPED
        Implements IDisposable

        Public Internal As Integer
        Public InternalHigh As Integer
        Public Offset As Integer
        Public OffsetHigh As Integer
        Public hEvent As IntPtr


        Public Sub New()
          hEvent = NativeMethods.CreateEvent(IntPtr.Zero, True, False, Nothing)  ' must be a manual reset event
        End Sub

        Public Function HasIoCompleted() As Boolean
          Const STATUS_PENDING As Integer = &H103
          Return Internal <> STATUS_PENDING
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
          Dispose(True)
          GC.SuppressFinalize(Me)
        End Sub

        Private Sub Dispose(ByVal disposing As Boolean)
          If Not hEvent.Equals(IntPtr.Zero) Then
            NativeMethods.CloseHandle(hEvent)
            hEvent = IntPtr.Zero
          End If
        End Sub
        Protected Overrides Sub finalize()
          Dispose(False)
        End Sub
      End Class
#End If
    End Class

    ' -----------------------------------------
    <Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Sequential)> _
    Private Class COMMTIMEOUTS
      Public ReadIntervalTimeout As Integer
      Public ReadTotalTimeoutMultiplier As Integer
      Public ReadTotalTimeoutConstant As Integer
      Public WriteTotalTimeoutMultiplier As Integer
      Public WriteTotalTimeoutConstant As Integer
    End Class
  End Class


  ' -----------------------------------------------------------------------
  ' A network, sockets, stream that keeps re-connecting, and doesn't throw exceptions.
  ' Instead of exceptions, it just returns 0 bytes from Read's and quietly skips Write's
  ' Only works on ipv4 - probably not a problem
  Public Class NetworkPort : Inherits System.IO.Stream
    Private ReadOnly computer_ As String, port_ As Integer
    Private socket_ As Net.Sockets.Socket, receiveTimeout_ As Integer

    Public Sub New(ByVal computer As String, ByVal port As Integer)
      computer_ = computer : port_ = port
    End Sub

    Public Overrides Function ToString() As String
      Return computer_ & ":" & port_.ToString(Globalization.CultureInfo.InvariantCulture)
    End Function

    Public Overrides Sub Close()
      If socket_ IsNot Nothing Then socket_.Close() : socket_ = Nothing
    End Sub

    Public Overrides Property ReadTimeout() As Integer
      Get
        Return receiveTimeout_
      End Get
      Set(ByVal value As Integer)
        If receiveTimeout_ = value Then Exit Property
        receiveTimeout_ = value
        If socket_ IsNot Nothing Then socket_.ReceiveTimeout = value
      End Set
    End Property


    Public Overrides Function BeginWrite(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer, ByVal callback As AsyncCallback, ByVal state As Object) As IAsyncResult
      ' If the socket is not currently connected, then ensure that is attempted first
      If socket_ Is Nothing Then Return New ConnectThenSend(Me, buffer, offset, count, callback, state)
      Try
        Return socket_.BeginSend(buffer, offset, count, Net.Sockets.SocketFlags.None, callback, state)
      Catch
        Close()
        Return New QuietAsyncResult(callback, state, Date.MinValue)
      End Try
    End Function

    ' Nothing worth worrying about
    Public Overrides Sub EndWrite(ByVal asyncResult As IAsyncResult)
      If TypeOf asyncResult Is QuietAsyncResult Then Exit Sub ' quietly do nothing
      If Not TypeOf asyncResult Is ConnectThenSend Then
        Try
          socket_.EndSend(asyncResult)
        Catch
          Close()
        End Try
      End If
    End Sub

    Public Overrides Function BeginRead(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer, _
                                        ByVal callback As AsyncCallback, ByVal state As Object) As IAsyncResult
      ' We assume a previous Write will have done its best to get the socket connected
      If socket_ Is Nothing Then Return New QuietAsyncResult(callback, state, Date.MinValue)

      Try
        Return socket_.BeginReceive(buffer, offset, count, Net.Sockets.SocketFlags.None, callback, state)
      Catch
        Close()  ' some socket exception
        Return New QuietAsyncResult(callback, state, Date.MinValue)  ' handly it quietly
      End Try
    End Function

    Public Overrides Function EndRead(ByVal asyncResult As IAsyncResult) As Integer
      If TypeOf asyncResult Is QuietAsyncResult Then Return 0 ' no bytes received
      Try
        Return socket_.EndReceive(asyncResult)
      Catch
        Close()
        Return 0   ' sanitize it
      End Try
    End Function

    ' We define simple not async code as well, as this may be all that our callers need
    Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
      If socket_ Is Nothing Then Return 0 ' should have been opened by a preceding Write
      Try
        Return socket_.Receive(buffer, offset, count, Net.Sockets.SocketFlags.None)
      Catch
        Close()
        Return 0
      End Try
    End Function

    Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
      If socket_ Is Nothing Then
        socket_ = New Net.Sockets.Socket(Net.Sockets.AddressFamily.InterNetwork, Net.Sockets.SocketType.Stream, Net.Sockets.ProtocolType.Tcp)
        socket_.ReceiveTimeout = receiveTimeout_
        Try
          socket_.Connect(computer_, port_)
        Catch
          Close()
        End Try
      End If
      Try
        socket_.Send(buffer, offset, count, Net.Sockets.SocketFlags.None)
      Catch
        Close()
      End Try
    End Sub

    ' These are necessary, but un-interesting
    Public Overrides ReadOnly Property CanRead() As Boolean
      Get
        Return True
      End Get
    End Property
    Public Overrides ReadOnly Property CanSeek() As Boolean
      Get
        Return False
      End Get
    End Property
    Public Overrides ReadOnly Property CanWrite() As Boolean
      Get
        Return True
      End Get
    End Property
    Public Overrides Sub Flush()
    End Sub
    Public Overrides ReadOnly Property Length() As Long
      Get
        Throw New NotSupportedException
      End Get
    End Property
    Public Overrides Property Position() As Long
      Get
        Throw New NotSupportedException
      End Get
      Set(ByVal value As Long)
        Throw New NotSupportedException
      End Set
    End Property
    Public Overrides Function Seek(ByVal offset As Long, ByVal origin As System.IO.SeekOrigin) As Long
      Throw New NotSupportedException
    End Function
    Public Overrides Sub SetLength(ByVal value As Long)
      Throw New NotSupportedException
    End Sub

    ' ------------------------------------------------------------------
    Private Class ConnectThenSend : Implements IAsyncResult
      Private ReadOnly owner_ As NetworkPort, buffer_() As Byte, offset_, count_ As Integer, _
                       callback_ As AsyncCallback, state_ As Object
      Private completed_ As Boolean

      Public Sub New(ByVal owner As NetworkPort, ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer, ByVal callback As AsyncCallback, ByVal state As Object)
        owner_ = owner : buffer_ = buffer : offset_ = offset : count_ = count
        callback_ = callback : state_ = state
        owner_.socket_ = New Net.Sockets.Socket(Net.Sockets.AddressFamily.InterNetwork, Net.Sockets.SocketType.Stream, Net.Sockets.ProtocolType.Tcp)
        owner_.socket_.ReceiveTimeout = owner.receiveTimeout_
        Try
          ' This BeginConnect may sometimes (but not always) throw a SocketException
          owner_.socket_.BeginConnect(owner.computer_, owner.port_, AddressOf OnConnected, Nothing)
        Catch
          owner_.Close()
          completed_ = True  ' give up
          If callback_ IsNot Nothing Then callback_(Me)
        End Try
      End Sub

      Private Sub OnConnected(ByVal ar As IAsyncResult)
        Try
          owner_.socket_.EndConnect(ar)
          owner_.socket_.BeginSend(buffer_, offset_, count_, Net.Sockets.SocketFlags.None, AddressOf OnSent, Nothing)
        Catch
          owner_.Close()
          completed_ = True  ' give up
          If callback_ IsNot Nothing Then callback_(Me)
        End Try
      End Sub

      Private Sub OnSent(ByVal ar As IAsyncResult)
        Try
          owner_.socket_.EndSend(ar)
        Catch
          owner_.Close()
        End Try
        completed_ = True
        If callback_ IsNot Nothing Then callback_(Me)
      End Sub

      Private ReadOnly Property AsyncState() As Object Implements IAsyncResult.AsyncState
        Get
          Return state_
        End Get
      End Property
      Private ReadOnly Property AsyncWaitHandle() As Threading.WaitHandle Implements IAsyncResult.AsyncWaitHandle
        Get
          Throw New NotImplementedException
        End Get
      End Property
      Private ReadOnly Property CompletedSynchronously() As Boolean Implements IAsyncResult.CompletedSynchronously
        Get
          Return False
        End Get
      End Property
      Private ReadOnly Property IsCompleted() As Boolean Implements IAsyncResult.IsCompleted
        Get
          Return completed_
        End Get
      End Property
    End Class
  End Class

  ' ------------------------------------------------------------------
  Friend Class QuietAsyncResult : Implements IAsyncResult
    Private ReadOnly state_ As Object, completeTime_ As Date
    Public Sub New(ByVal callback As AsyncCallback, ByVal state As Object, ByVal completeTime As Date)
      state_ = state : completeTime_ = completeTime
      If callback IsNot Nothing Then callback(Me)
    End Sub
    Private ReadOnly Property AsyncState() As Object Implements IAsyncResult.AsyncState
      Get
        Return state_
      End Get
    End Property
    Private ReadOnly Property AsyncWaitHandle() As Threading.WaitHandle Implements IAsyncResult.AsyncWaitHandle
      Get
        Throw New NotImplementedException
      End Get
    End Property
    Private ReadOnly Property CompletedSynchronously() As Boolean Implements IAsyncResult.CompletedSynchronously
      Get
        Return False
      End Get
    End Property
    Private ReadOnly Property IsCompleted() As Boolean Implements IAsyncResult.IsCompleted
      Get
        Return completeTime_ = Date.MinValue OrElse Date.UtcNow >= completeTime_
      End Get
    End Property
  End Class



  ' -----------------------------------------------------------------------------------
  ' Encapsulates a stream, adding built-in diagnostics that are useful for the control system
  Public Class Stream
    Private ReadOnly stm_ As System.IO.Stream
    Private logComEvents_ As Boolean

    ' Used for diagnostics
    Private latestRx_() As Byte : Private ReadOnly comEvents_ As New ComEventCollection

    Public Sub New(ByVal stm As System.IO.Stream)
      stm_ = stm
    End Sub

    Public Overrides Function ToString() As String
      Return stm_.ToString
    End Function

    Public Property LogComEvents() As Boolean
      Get
        Return logComEvents_
      End Get
      Set(ByVal value As Boolean)
        logComEvents_ = value
      End Set
    End Property

    Public Sub Flush()
      stm_.Flush()
    End Sub

    Public Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
      If logComEvents_ Then AddTxEvents(buffer, offset, count)
      stm_.Write(buffer, offset, count)
    End Sub
    Public Function BeginWrite(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer, _
                               ByVal callback As AsyncCallback, ByVal state As Object) As IAsyncResult
      If logComEvents_ Then AddTxEvents(buffer, offset, count)
      Return stm_.BeginWrite(buffer, offset, count, callback, state)
    End Function
    Public Sub EndWrite(ByVal asyncResult As IAsyncResult)
      stm_.EndWrite(asyncResult)
    End Sub

    Private Sub AddTxEvents(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
      ' Keep a note for diagnostics - unfortunately, each tx'd byte will have the same time on it
      Dim time As Date = Date.UtcNow
      SyncLock comEvents_
        For i As Integer = 0 To count - 1
          comEvents_.Add(time, ComEvent.EventTypeValue.Tx, buffer(offset + i))
        Next i
      End SyncLock
    End Sub

    Public Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
      Dim ret As Integer = stm_.Read(buffer, offset, count)
      If logComEvents_ Then AddRxEvents(buffer, offset, ret)
      Return ret
    End Function

    Public Function BeginRead(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer, _
                              ByVal callback As AsyncCallback, ByVal state As Object) As IAsyncResult
      latestRx_ = buffer
      Return stm_.BeginRead(buffer, offset, count, callback, state)
    End Function

    Public Function EndRead(ByVal asyncResult As IAsyncResult) As Integer
      Dim ret As Integer = stm_.EndRead(asyncResult)
      If logComEvents_ Then AddRxEvents(latestRx_, 0, ret)
      Return ret
    End Function


    Public Sub AddRxEvents(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
      ' Keep a note for diagnostics - unfortunately, each rx'd byte will have the same time on it
      Dim time As Date = Date.UtcNow
      SyncLock comEvents_
        For i As Integer = 0 To count - 1
          comEvents_.Add(time, ComEvent.EventTypeValue.Rx, buffer(offset + i))
        Next i
      End SyncLock
    End Sub

    Friend ReadOnly Property ComEvents() As ComEvent()
      Get
        ' Returns a thread-safe snapshot
        Dim ret() As ComEvent
        SyncLock comEvents_
          Dim count As Integer = comEvents_.Count
          ret = New ComEvent(count - 1) {}
          For i As Integer = 0 To count - 1
            ret(i) = comEvents_(i)
          Next i
        End SyncLock
        Return ret
      End Get
    End Property

    ' ----------------------------
    Public Structure ComEvent
      Private ReadOnly time_ As Date, eventType_ As EventTypeValue, ch_ As Byte

      Public Sub New(ByVal time As Date, ByVal eventType As EventTypeValue, ByVal ch As Byte)
        time_ = time : eventType_ = eventType : ch_ = ch
      End Sub
      Public ReadOnly Property Time() As Date
        Get
          Return time_
        End Get
      End Property
      Public ReadOnly Property EventType() As EventTypeValue
        Get
          Return eventType_
        End Get
      End Property
      Public ReadOnly Property Ch() As Byte
        Get
          Return ch_
        End Get
      End Property
      Public Enum EventTypeValue
        Rx
        Tx
      End Enum
    End Structure
    ' ----------------------------
    Private Class ComEventCollection : Inherits LimitedCollection(Of ComEvent)
      Public Sub New()
        MyBase.New(1000)  ' limiting capacity
      End Sub
      Public Overloads Sub Add(ByVal time As Date, ByVal eventType As ComEvent.EventTypeValue, ByVal ch As Byte)
        Add(New ComEvent(time, eventType, ch))
      End Sub
    End Class

    Private Class LimitedCollection(Of T) : Implements ICollection(Of T)
      Private ReadOnly coll_() As T, capacity_ As Integer
      Private count_ As Integer, firstOfs_ As Integer

      Public Sub New(ByVal capacity As Integer)
        capacity_ = capacity : coll_ = New T(capacity - 1) {}
      End Sub
      Public Sub Add(ByVal value As T) Implements ICollection(Of T).Add
        If count_ < capacity_ Then
          coll_(count_) = value : count_ += 1
        Else
          coll_(firstOfs_) = value
          firstOfs_ += 1 : If firstOfs_ = capacity_ Then firstOfs_ = 0
        End If
      End Sub

      Default Public ReadOnly Property Item(ByVal index As Integer) As T
        Get
          index += firstOfs_ : If index >= capacity_ Then index -= capacity_
          Return coll_(index)
        End Get
      End Property

      Public ReadOnly Property Count() As Integer Implements ICollection(Of T).Count
        Get
          Return count_
        End Get
      End Property

      Public Sub Clear() Implements ICollection(Of T).Clear
        count_ = 0 : firstOfs_ = 0
      End Sub

      Public Function Contains(ByVal item As T) As Boolean Implements ICollection(Of T).Contains
        Dim ofs As Integer = firstOfs_
        For i As Integer = 0 To count_ - 1
          If coll_(ofs).Equals(item) Then Return True
          ofs += 1 : If ofs = capacity_ Then ofs = 0
        Next i
        Return False
      End Function

      Private Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
        Return Nothing
      End Function


      Private ReadOnly Property IsReadOnly() As Boolean Implements ICollection(Of T).IsReadOnly
        Get
          Return False
        End Get
      End Property
      Private Function Remove(ByVal item As T) As Boolean Implements ICollection(Of T).Remove
        Throw New NotSupportedException
      End Function
      Private Sub CopyTo(ByVal array() As T, ByVal arrayIndex As Integer) Implements ICollection(Of T).CopyTo
        Throw New NotSupportedException
      End Sub
      Private Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
        Return GetEnumerator()
      End Function
    End Class
  End Class

  Public Class SerialPortStatistics
    Public BytesIn As Integer
    Public BytesOut As Integer
    Public FramingErrors As Integer
    Public ParityErrors As Integer
    Public HardwareOverrunErrors As Integer
    Public BufferOverrunErrors As Integer
  End Class

  Public Enum WriteMode
    Optimised
    Always
  End Enum

  ' ------------------------------------------------------
  Friend Class WriteOptimisation
    Private ReadOnly recentWrites_ As New List(Of RecentWrite)

    Public Function RecentlyWrote(ByVal values As Array, ByVal ParamArray params() As Object) As Boolean
      ' Make invalid anything that's got too old
      Dim now As Date = Date.UtcNow
      Const writeDelay As Integer = 5
      Dim expiry As Date = now.Subtract(TimeSpan.FromSeconds(writeDelay))

      ' See if we have something the same recently
      Dim freeRw As RecentWrite
      For i As Integer = 0 To recentWrites_.Count - 1
        Dim rw As RecentWrite = recentWrites_.Item(i)
        If rw.Time < expiry Then
          If freeRw Is Nothing Then freeRw = rw
        Else
          Dim oldParams() As Object = rw.Params
          If oldParams.Length = params.Length Then
            Dim same As Boolean = True
            For j As Integer = 0 To oldParams.Length - 1
              If Not oldParams(j).Equals(params(j)) Then same = False : Exit For
            Next j
            If same Then
              ' See if the value arrays are the same
              Dim oldValues As Array = rw.Values
              If oldValues.Length = values.Length Then
                same = True
                For j As Integer = 0 To oldValues.Length - 1
                  If Not oldValues.GetValue(j).Equals(values.GetValue(j)) Then same = False : Exit For
                Next j
                If same Then Return True
                ' If done recently with the same params, but with different values, then re-use the slot
                Array.Copy(values, rw.Values, values.Length)
                rw.Time = now
                Return False ' got to do a write
              End If
            End If
          End If
        End If
      Next i
      ' Need a new (or expired) slot
      If freeRw Is Nothing Then freeRw = New RecentWrite : recentWrites_.Add(freeRw)

      ' Store the values
      freeRw.Time = now
      freeRw.Values = DirectCast(values.Clone, Array)  ' because the given array may well be re-used
      freeRw.Params = params
      Return False
    End Function

    ' ---------------------------------------
    Private Class RecentWrite
      Private time_ As Date, values_ As Array, params_() As Object

      Public Sub ExpireIfOlder(ByVal value As Date)
        If time_ < value Then time_ = Date.MinValue : values_ = Nothing : params_ = Nothing
      End Sub

      Public Property Time() As Date
        Get
          Return time_
        End Get
        Set(ByVal value As Date)
          time_ = value
        End Set
      End Property
      Public Property Values() As Array
        Get
          Return values_
        End Get
        Set(ByVal value As Array)
          values_ = value
        End Set
      End Property
      Public Property Params() As Object()
        Get
          Return params_
        End Get
        Set(ByVal value As Object())
          params_ = value
        End Set
      End Property
    End Class
  End Class

  ' ------------------------------------------------------------
  Public NotInheritable Class BitConverter
    Private Sub New()
    End Sub
    Public Shared Function GetBooleans(ByVal value() As Short, ByVal startIndex As Integer, ByVal count As Integer) As Boolean()
      Dim ret(count * 16) As Boolean  ' for convenience, element 0 is not used
      For i As Integer = 0 To count * 16 - 1
        ret(i + 1) = (value(i \ 16 + startIndex) And (1 << (i And 15))) <> 0
      Next i
      Return ret
    End Function
    Public Shared Function GetBooleans(ByVal value() As Integer, ByVal startIndex As Integer, ByVal count As Integer) As Boolean()
      Dim ret(count * 32) As Boolean  ' for convenience, element 0 is not used
      For i As Integer = 0 To count * 32 - 1
        ret(i + 1) = (value(i \ 32 + startIndex) And (1 << (i And 31))) <> 0
      Next i
      Return ret
    End Function
    Public Shared Function GetInt16s(ByVal value() As Boolean, ByVal startIndex As Integer, ByVal count As Integer) As Short()
      Dim outCount As Integer = count \ 16
      If (count Mod 16) <> 0 Then outCount += 1
      Dim ret(outCount) As Short  ' for convenience, element 0 is not used
      For i As Integer = 0 To count - 1
        If value(startIndex + i) Then
          Dim ofs As Integer = (i \ 16) + 1
          ret(ofs) = ret(ofs) Or (1S << (i And 15))
        End If
      Next i
      Return ret
    End Function
    Public Shared Function GetInt32s(ByVal value() As Boolean, ByVal startIndex As Integer, ByVal count As Integer) As Integer()
      Dim outCount As Integer = count \ 32
      If (count Mod 32) <> 0 Then outCount += 1
      Dim ret(outCount) As Integer  ' for convenience, element 0 is not used
      For i As Integer = 0 To count - 1
        If value(startIndex + i) Then
          Dim ofs As Integer = (i \ 32) + 1
          ret(ofs) = ret(ofs) Or (1 << (i And 31))
        End If
      Next i
      Return ret
    End Function
  End Class
End Namespace