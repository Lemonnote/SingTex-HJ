<Command("Power Drain", "Pump Delay |0-60| Drain Type |0-1| Speed |1-100|%", , , "2"), _
 TranslateCommand("zh-TW", "動力排水 ", "啟動延遲 |0-60| 水管選擇 |0-1| 運轉速度 |1-100|%"), _
 Description("1=HOT 0=COLD   MAX=100% ,MIN=1%"), _
 TranslateDescription("zh-TW", "60(秒)=最大,0(秒)=最小   1=熱水 0=冷水   最高=100% ,最小=1%")> _
Public NotInheritable Class Command32
    Inherits MarshalByRefObject
    Implements ACCommand

  Public Enum S32
    Off
    WaitStopReel
    WaitTime
    WaitTempSafe
    WaitTime4
    WaitLowLevel
    WaitTime5
    WaitMainPumpFB
    WaitNotLowLevel
    WaitTime6
    WaitTime7
    WaitNotLowLevel2
    WaitTime8
    Pause
  End Enum

    Public Wait As New Timer

    Public PumpDelayTime As Integer
    Public DrainType As Integer
    Public StateString As String
  Public WaitRunning As New Timer

    Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
        With ControlCode
            .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
            .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
            .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command10.Cancel()
            .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
            .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
            .Command59.Cancel() : .Command61.Cancel() : .TemperatureControl.Cancel()
            .Command17.Cancel() : .Command01.Cancel()
            .TempControlFlag = False
      WaitRunning.TimeRemaining = .Parameters.WaitOverTime * 60
      PumpDelayTime = MinMax(param(1), 0, 60)
            DrainType = MinMax(param(2), 0, 1)
      ControlCode.ReelStopRequest = True
      Wait.TimeRemaining = .Parameters.ReelStopDelayTime
      .SimulateFillWaterLiters = 0
      State = S32.WaitStopReel
    End With
    End Function

    Public Function Run() As Boolean Implements ACCommand.Run
        With ControlCode
            Select Case State
                Case S32.Off
          StateString = ""


        Case S32.WaitStopReel
          StateString = If(.Language = LanguageValue.ZhTW, "停止帶布輪", "Stopping Reel")
          If Not Wait.Finished Then Exit Select
          .ReelStopRequest = False
          .PumpStopRequest = True
          Wait.TimeRemaining = 1
          State = S32.WaitTime


                Case S32.WaitTime
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        Wait.Pause()
                        StateWas = State
                        State = S32.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "停止主泵", "Stopping pump ")

                    If Not Wait.Finished Then Exit Select
                    .PumpStopRequest = False
                    ' .IO.PumpSpeedControl = 0
                    .PumpOn = False
                    State = S32.WaitTempSafe

                Case S32.WaitTempSafe
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        Wait.Pause()
                        StateWas = State
                        State = S32.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "溫度異常", "Interlocked Temperature")
                    If .IO.MainTemperature >= .Parameters.SetSafetyTemp * 10 Then
                        .Alarms.HighTempNoFill = True
                        Exit Select
                    End If

                    .Alarms.HighTempNoDrain = False
                    'If DrainType = 0 Then
                    ' .IO.PowerDrain = True
                    ' Else
                    ' .IO.PowerHotDrain = True
                    ' End If
                    Wait.TimeRemaining = PumpDelayTime
                    State = S32.WaitTime4

                Case S32.WaitTime4
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        Wait.Pause()
                        StateWas = State
                        State = S32.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "停止主泵", "Stopping pump ")
                    If Not Wait.Finished Then Exit Select

                    State = S32.WaitLowLevel

                Case S32.WaitLowLevel
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        Wait.Pause()
                        StateWas = State
                        State = S32.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "低水位", "Level to low ")
          If Not .IO.LowLevel Then Exit Select
          '.IO.PumpSpeedControl = CType(.PumpSpeed * 10, Short)
          If .Parameters.SetPowerDrain = 0 Then
            .PumpStartRequest = True
            Wait.TimeRemaining = PumpDelayTime
            State = S32.WaitTime5
          Else
            .PumpStartRequest = False
            State = S32.WaitNotLowLevel2
          End If

                Case S32.WaitTime5
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        Wait.Pause()
                        StateWas = State
                        State = S32.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "開始主泵", "Starting pump ")
                    If Not Wait.Finished Then Exit Select
                    .PumpStartRequest = False
                    If .Parameters.SetPowerDrain = 0 Then
                        .PumpOn = True
                    Else
                        .PumpOn = False
                    End If

                    State = S32.WaitMainPumpFB

                Case S32.WaitMainPumpFB
                    StateString = If(.Language = LanguageValue.ZhTW, "主泵沒有運行", "Main pump not running")
                    If Not (.IO.MainPumpFB Or .Parameters.SetPowerDrain = 1) Then Exit Select
                    State = S32.WaitNotLowLevel

                Case S32.WaitNotLowLevel
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        Wait.Pause()
                        StateWas = State
                        State = S32.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "主缸排水至低水位", "Draining to low level")
                    If .IO.LowLevel Then Exit Select
                    Wait.TimeRemaining = .Parameters.PowerDrainDelay
                    State = S32.WaitTime6

                Case S32.WaitTime6
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        Wait.Pause()
                        StateWas = State
                        State = S32.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "主缸排水至低水位", "Draining to low level ") & TimerString(Wait.TimeRemaining)
                    If Not Wait.Finished Then Exit Select

                    .PumpStopRequest = True
                    Wait.TimeRemaining = 1
                    State = S32.WaitTime7

                Case S32.WaitTime7
                    StateString = If(.Language = LanguageValue.ZhTW, "停止主泵", "Stopping pump ")
                    If Not Wait.Finished Then Exit Select
                    .PumpStopRequest = False
                    '.IO.PumpSpeedControl = 0
                    .PumpOn = False
                    'If DrainType = 0 Then
                    ' .IO.Drain = True
                    ' Else
                    '.IO.HotDrain = True
                    'End If
                    State = S32.WaitNotLowLevel2

                Case S32.WaitNotLowLevel2
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        Wait.Pause()
                        StateWas = State
                        State = S32.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "主缸排水至低水位", "Draining to low level ") & TimerString(Wait.TimeRemaining)
                    If .IO.LowLevel Then Exit Select
                    Wait.TimeRemaining = .Parameters.DrainDelay
                    State = S32.WaitTime8

                Case S32.WaitTime8
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        Wait.Pause()
                        StateWas = State
                        State = S32.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "主缸排水至低水位", "Draining to low level ") & TimerString(Wait.TimeRemaining)
                    If Not Wait.Finished Then Exit Select
                    'If DrainType = 0 Then
                    ' .IO.PowerDrain = False
                    ' Else
                    ' .IO.PowerHotDrain = False
                    ' End If

                    'If DrainType = 0 Then
                    ' .IO.Drain = False
                    ' Else
                    ' .IO.HotDrain = False
                    ' End If

                    State = S32.Off
          WaitRunning.Cancel()

                Case S32.Pause
                    StateString = If(.Language = LanguageValue.ZhTW, "暫停", "Paused") & " " & TimerString(Wait.TimeRemaining)
                    'no longer pause restart the timer and go back to the wait state
                    If (Not .Parent.IsPaused) And .IO.SystemAuto Then
                        Wait.Restart()
                        State = StateWas
                        StateWas = S32.Off
                    End If

            End Select
        End With
    End Function

    Public Sub Cancel() Implements ACCommand.Cancel
        State = S32.Off
        Wait.Cancel()
    WaitRunning.Cancel()
  End Sub

    Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged

    End Sub


#Region "Standard Definitions"
    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
        Get
            Return State <> S32.Off
        End Get
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S32
    Public Property State() As S32
        Get
            Return state_
        End Get
        Private Set(ByVal value As S32)
            state_ = value
        End Set
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S32
    Public Property StateWas() As S32
        Get
            Return statewas_
        End Get
        Private Set(ByVal value As S32)
            statewas_ = value
        End Set
    End Property


    Public ReadOnly Property IsPowerDrain() As Boolean
        Get
            Return DrainType = 0 And ((State = S32.WaitNotLowLevel) Or (State = S32.WaitTime6) Or _
                                      (State = S32.WaitTime7) Or (State = S32.WaitNotLowLevel2) Or (State = S32.WaitTime8))
        End Get
    End Property
    Public ReadOnly Property IsPowerHotDrain() As Boolean
        Get
            Return DrainType <> 0 And ((State = S32.WaitNotLowLevel) Or (State = S32.WaitTime6) Or (State = S32.WaitTime7) Or (State = S32.WaitNotLowLevel2) Or _
                   (State = S32.WaitTime8))
        End Get
    End Property
    Public ReadOnly Property IsHotDrain() As Boolean
        Get
            Return ((State = S32.WaitNotLowLevel2) Or (State = S32.WaitTime8))
        End Get
    End Property
    Public ReadOnly Property IsDrain() As Boolean
        Get
            Return ((State = S32.WaitNotLowLevel2) Or (State = S32.WaitTime8))
        End Get
    End Property
  Public ReadOnly Property IsOverflow() As Boolean
    Get
      Return State = S32.WaitLowLevel Or State = S32.WaitTime5 Or State = S32.WaitMainPumpFB Or State = S32.WaitNotLowLevel Or State = S32.WaitTime6 Or State = S32.WaitTime7 Or State = S32.WaitNotLowLevel2 Or State = S32.WaitTime8
    End Get
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command32 As New Command32(Me)
End Class
#End Region
