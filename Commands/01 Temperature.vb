<Command("Temperature", "SetTemp |0-145|C Gradient |0-99| HoldTime |0-999|", "'2", "'1", "'3"), _
 TranslateCommand("zh-TW", "時間溫度控制", "目標溫度 |0-145|C, 斜率 |0-99|, 持溫時間 |0-999|分鐘")> _
Public NotInheritable Class Command01
    Inherits MarshalByRefObject
    Implements ACCommand

    Public Enum S01
        Off
        Start
        Run
        Hold
        Complete
        Pause
    End Enum

    Public Wait As New Timer
    Public TargetTemp As Integer
    Public Gradient As Integer
    Public StateString As String
    Public HoldTime As Integer
    Public HoldTimeWas As Integer
    Public HeatTargetTime As Integer
    Public HeatTimer As New Timer
  Public WaitTemperature As New Timer

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
        With ControlCode
            'cancels for all other forground functions

      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
            .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
            .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
            .Command33.Cancel() : .Command17.Cancel() : .Command10.Cancel()
            .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
            .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
            .Command59.Cancel() : .Command61.Cancel() : .TemperatureControl.Cancel()

            TargetTemp = Maximum(param(1) * 10, 1500)
            Gradient = param(2)
            HoldTime = 60 * param(3)
            Wait.TimeRemaining = HoldTime
            Wait.Pause()

            'Check Temperature mode - change during TPHold if necessary
            '.TemperatureControl.TempMode = 0
            'If .TemperatureControl.Parameters_HeatCoolModeChange = 1 Then .TemperatureControl.TempMode = 2
            'If .TemperatureControl.Parameters_HeatCoolModeChange = 2 Then .TemperatureControl.TempMode = 2

            State = S01.Start

        End With
    End Function

    Public Function Run() As Boolean Implements ACCommand.Run
        With ControlCode
            Select Case State
                Case S01.Off
                    .HeatOverTime_Alarm = False
                    StateString = ""

                Case S01.Start
                    StateString = ""
          If .Command10.IsOn Then Exit Select
          If .Command66.IsOn And Not .TankBReady Then
            StateString = "等待B缸備藥完成"
            Exit Select
          End If
          If .Command67.IsOn And Not .TankCReady Then
            StateString = "等待C缸備藥完成"
            Exit Select
          End If
          .TempControlFlag = True
          .TemperatureControl.CoolingIntegral = .TemperatureControl.Parameters_CoolIntegral
                    .TemperatureControl.CoolingMaxGradient = .TemperatureControl.Parameters_CoolMaxGradient
                    .TemperatureControl.CoolingPropBand = .TemperatureControl.Parameters_CoolPropBand
                    .TemperatureControl.CoolingStepMargin = .TemperatureControl.Parameters_CoolStepMargin
                    .TemperatureControl.Start(.IO.MainTemperature, TargetTemp, Gradient)
                    If .IO.MainTemperature > TargetTemp Then
                        .CoolNow = True
                        .HeatNow = False
                        .TemperatureControl.TempMode = 4
                    Else
                        .CoolNow = False
                        .HeatNow = True
                        .TemperatureControl.TempMode = 3

                        '升溫到達目標溫度時間
                        If Gradient = 0 Then
                            HeatTargetTime = ((TargetTemp - .IO.MainTemperature) \ 30 + 5) * 60
                        Else
                            HeatTargetTime = (((TargetTemp - .IO.MainTemperature) \ Gradient) + 5) * 60
                        End If
                        HeatTimer.TimeRemaining = HeatTargetTime
                        HeatTimer.Restart()
                    End If
          WaitTemperature.TimeRemaining = 5
          State = S01.Run

        Case S01.Run
                    StateString = "目標:" & TargetTemp / 10 & "度,斜率:" & Gradient & ",持溫:" & HoldTime / 60 & "分"
                    If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
                        Wait.Pause()
                        StateWas = State
                        State = S01.Pause
                    End If

                    '升溫超時警報
                    If .HeatNow Then
                        If HeatTimer.Finished Then
                            .HeatOverTime_Alarm = True
                        End If
                        If .HeatOverTime_Alarm And .IO.CallAck Then
                            .HeatOverTime_Alarm = False
                            HeatTimer.TimeRemaining = 999
                            HeatTimer.Pause()
                        End If
                    End If
          If Not (.IO.MainTemperature < (TargetTemp + 10) And .IO.MainTemperature > (TargetTemp - 10)) Then WaitTemperature.TimeRemaining = 5
          If Not WaitTemperature.Finished Then Exit Select
          HeatTimer.Cancel()
          .HeatOverTime_Alarm = False
                    Wait.Restart()
                    State = S01.Hold

                Case S01.Hold
                    StateString = "持溫時間" & " " & TimerString(Wait.TimeRemaining)
                    If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
                        .TemperatureControl.Cancel()
                        .TemperatureControl.Start(.IO.MainTemperature, TargetTemp, Gradient)
                        Wait.Pause()
                        StateWas = State
                        State = S01.Pause
                        HoldTimeWas = HoldTime
                    End If
                    If Not Wait.Finished Then Exit Select
                    State = S01.Complete

                Case S01.Complete
                    .HeatNow = False
                    .CoolNow = False
                    .TemperatureControlFlag = False
                    State = S01.Off

                Case S01.Pause
                    StateString = If(.Language = LanguageValue.ZhTW, "暫停 ", "Paused ") & " " & TimerString(Wait.TimeRemaining)
                    .HeatOverTime_Alarm = False
                    If (Not .Parent.IsPaused) And .IO.MainPumpFB And .IO.SystemAuto Then
                        .TemperatureControl.Cancel()
                        .TemperatureControl.Start(.IO.MainTemperature, TargetTemp, Gradient)
                        State = StateWas
                        StateWas = S01.Off
                        If State = S01.Run Then
                            Wait.TimeRemaining = HoldTime
                            Wait.Pause()
                        ElseIf State = S01.Hold Then
                            If ((HoldTime - HoldTimeWas) + Wait.TimeRemaining) > 0 Then
                                Wait.TimeRemaining = (HoldTime - HoldTimeWas) + Wait.TimeRemaining
                            Else
                                Wait.Restart()
                            End If
                        End If
                    End If
            End Select
        End With
    End Function


    Public Sub Cancel() Implements ACCommand.Cancel
        State = S01.Complete
        Wait.Cancel()
    End Sub

    Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
        TargetTemp = Maximum(param(1) * 10, 1500)
        Gradient = param(2)
        HoldTime = 60 * param(3)
    End Sub



#Region "Standard Definitions"
    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
        Get
            Return State <> S01.Off
        End Get
    End Property

    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S01
    Public Property State() As S01
        Get
            Return state_
        End Get
        Private Set(ByVal value As S01)
            state_ = value
        End Set
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S01
    Public Property StateWas() As S01
        Get
            Return statewas_
        End Get
        Private Set(ByVal value As S01)
            statewas_ = value
        End Set
    End Property


    Public ReadOnly Property IsRamping() As Boolean
        Get
            Return (State = S01.Run)
        End Get
    End Property
    Public ReadOnly Property IsHolding() As Boolean
        Get
            Return (State = S01.Hold)
        End Get
    End Property
    Public ReadOnly Property IsPaused() As Boolean
        Get
            Return (State = S01.Pause)
        End Get
    End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command01 As New Command01(Me)
End Class
#End Region
