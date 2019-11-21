<Command("Parallel Wait LA-252", "Mixer Time |60-999| Type |0-1| Qty% |0-100|", , , "15+('1/60)", CommandType.ParallelCommand), _
TranslateCommand("zh-TW", "平行等待染料", "攪拌時間 |60-999| 水源 |0-1| 水量% |0-100|"), _
Description("999=MAX 60=MIN,1=COLD 0=CIRCULATE,100%=MAX 0%=MIN"), _
TranslateDescription("zh-TW", "999秒=最高 60秒=最低,1=備清水 0=備回水,100%=最高 0%=最小")> _
Public NotInheritable Class Command69
  Inherits MarshalByRefObject                       'Inheritsg是繼承Windows Form的應用程式要繼承System.Windows.Forms.Form，可先參考物件導向程式設計相關書籍
  Implements ACCommand

  Public Enum S59
    Off
    WaitAuto
    WaitLA252
    DispenseWaitResponse
    Slow
    FillLevel
    FillQty
    MixForTime
    WaitMixStop
    Ready
  End Enum
  Public StateString As String

  Public Time, Type, Qty, CallOff As Integer
  Public WaitTimer As New Timer
  Public WaitRunning As New Timer

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      'cancels for all other forground functions
      Time = Maximum(param(1), 999)
      Type = MinMax(param(2), 0, 1)
      Qty = MinMax(param(3) * 10, 0, 1000)
      WaitRunning.TimeRemaining = (.Parameters.WaitOverTime * 60) + Time

      .Command64.Cancel()

      State = S59.WaitAuto
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S59.Off
          StateString = ""

        Case S59.WaitAuto
          StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "Interlocked not In auto")
          If Not .IO.SystemAuto Then Exit Select
          State = S59.WaitLA252

        Case S59.WaitLA252
          StateString = If(.Language = LanguageValue.ZhTW, "等待LA-252備藥中", "Wait LA-252 Ready")
          If Not .LA252Ready Then Exit Select
          State = S59.DispenseWaitResponse


        Case S59.DispenseWaitResponse
          StateString = ""
          If Not .IO.TankBLevel > 100 Then Exit Select
          State = S59.Slow

        Case S59.Slow
          .LA252Ready = False
          StateString = If(.Language = LanguageValue.ZhTW, "備藥完成，請按備藥OK", "Prepare Tank B")
          If (.Parameters.BTankCallAck = 1) Then
            .TankBReady = True
          End If

          If .TankBReady Then
            If Qty > 5 Then
              State = S59.FillQty
            Else
              State = S59.MixForTime
              WaitTimer.TimeRemaining = Time
            End If
          End If

        Case S59.FillQty
          StateString = If(.Language = LanguageValue.ZhTW, "B藥缸進水 ", "Filling Tank B to ") & Qty / 10 & "%"
          If .IO.TankBLevel > Qty Then
            State = S59.MixForTime
            WaitTimer.TimeRemaining = Time
          End If

        Case S59.MixForTime
          StateString = If(.Language = LanguageValue.ZhTW, "B藥缸攪拌 ", "Tank B mixing for ") & TimerString(WaitTimer.TimeRemaining)
          If WaitTimer.Finished Then
            State = S59.WaitMixStop
            WaitTimer.TimeRemaining = 10
          End If

        Case S59.WaitMixStop
          If WaitTimer.Finished Then
            State = S59.Ready
          End If


        Case S59.Ready
          State = S59.Off
          WaitRunning.Cancel()

      End Select

    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S59.Off
    WaitRunning.Cancel()
  End Sub

  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged

  End Sub


#Region " Standard Definitions "

  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S59.Off
    End Get
  End Property
  Public ReadOnly Property IsFillingFresh() As Boolean
    Get
      Return (Type = 1) And (State = S59.FillQty)
    End Get
  End Property
  Public ReadOnly Property IsFillingCirc() As Boolean
    Get
      Return (Type = 0) And (State = S59.FillQty)
    End Get
  End Property
  Public ReadOnly Property IsSlow() As Boolean
    Get
      Return (State = S59.Slow)
    End Get
  End Property
  Public ReadOnly Property IsMixingForTime() As Boolean
    Get
      Return (State = S59.MixForTime) Or (State = S59.Slow)
    End Get
  End Property
  Public ReadOnly Property IsReady() As Boolean
    Get
      Return (State = S59.Ready)
    End Get
  End Property
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S59
  Public Property State() As S59          'Property	屬性名稱() As 傳回值型別
    Get                                 'Get
      Return state_                   '屬性名稱 = 私有資料成員        '讀取私有資料成員的值
    End Get                             'End Get
    Private Set(ByVal value As S59)     'Set(ByVal Value As傳回值型別)
      state_ = value                  '私有資料成員 = Value          '設定私有資料成員的值
    End Set                             'End Set
  End Property
  'Property score() As Integer
  '    Get
  '        score = a           '讀取私有資料成員a的值
  '    End Get
  '
  '    Set(ByVal Value As Integer)
  '        a = Value           '設定私有資料成員a的值
  '    End Set
  'End Property

#End Region

End Class

#Region " Class Instance "

Partial Public Class ControlCode
  Public ReadOnly Command69 As New Command69(Me)
End Class

#End Region
