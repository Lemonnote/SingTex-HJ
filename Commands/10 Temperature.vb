<Command("Temperature", "SetTemp |0-145|C Gradient |0-99| HoldRound |0-999|", "'2", "'1", "'3"), _
 TranslateCommand("zh-TW", "圈數溫度控制", "目標溫度 |0-145|C 斜率 |0-99| 持溫圈數 |0-999|")> _
Public NotInheritable Class Command10
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S10
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
  Public FabricCycleTime As Integer
  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      'cancels for all other forground functions
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .Command17.Cancel() : .Command01.Cancel()
      .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .TemperatureControl.Cancel()

      TargetTemp = Maximum(param(1) * 10, 1500)
      Gradient = param(2)
      If .FabricCycleTime1 > 0 And .FabricCycleTime2 > 0 Then
        If .FabricCycleTime1 > .FabricCycleTime2 Then
          FabricCycleTime = .FabricCycleTime1
        Else
          FabricCycleTime = .FabricCycleTime2
        End If
      Else
        FabricCycleTime = .Parameters.MaximumFabricCycleTime
      End If
      HoldTime = param(3) * FabricCycleTime
      Wait.TimeRemaining = HoldTime
      Wait.Pause()

      'Check Temperature mode - change during TPHold if necessary
      '.TemperatureControl.TempMode = 0
      'If .TemperatureControl.Parameters_HeatCoolModeChange = 1 Then .TemperatureControl.TempMode = 2
      'If .TemperatureControl.Parameters_HeatCoolModeChange = 2 Then .TemperatureControl.TempMode = 2

      State = S10.Start

    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S10.Off
          StateString = ""

        Case S10.Start
          StateString = ""
          If .Command01.IsOn Then Exit Select
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
          End If
          State = S10.Run

        Case S10.Run
          StateString = ""
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S10.Pause
          End If
          If Not (.IO.MainTemperature < (TargetTemp + 10) And .IO.MainTemperature > (TargetTemp - 10)) Then Exit Select
          Wait.Restart()
          State = S10.Hold

        Case S10.Hold
          StateString = "持溫時間" & " " & TimerString(Wait.TimeRemaining)
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            .TemperatureControl.Cancel()
            .TemperatureControl.Start(.IO.MainTemperature, TargetTemp, Gradient)
            Wait.Pause()
            StateWas = State
            State = S10.Pause
            HoldTimeWas = HoldTime
          End If
          If Not Wait.Finished Then Exit Select
          State = S10.Complete

        Case S10.Complete
          .HeatNow = False
          .CoolNow = False
          .TemperatureControlFlag = False
          State = S10.Off

        Case S10.Pause
          StateString = If(.Language = LanguageValue.ZhTW, "暫停 ", "Paused ") & " " & TimerString(Wait.TimeRemaining)
          If (Not .Parent.IsPaused) And .IO.MainPumpFB And .IO.SystemAuto Then
            .TemperatureControl.Cancel()
            .TemperatureControl.Start(.IO.MainTemperature, TargetTemp, Gradient)
            State = StateWas
            StateWas = S10.Off
            If State = S10.Run Then
              Wait.TimeRemaining = HoldTime
              Wait.Pause()
            ElseIf State = S10.Hold Then
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
    State = S10.Complete
    Wait.Cancel()
  End Sub

  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
    TargetTemp = Maximum(param(1) * 10, 1500)
    Gradient = param(2)
    HoldTime = param(3) * FabricCycleTime
  End Sub



#Region "Standard Definitions"
  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S10.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S10
  Public Property State() As S10
    Get
      Return state_
    End Get
    Private Set(ByVal value As S10)
      state_ = value
    End Set
  End Property
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S10
  Public Property StateWas() As S10
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As S10)
      statewas_ = value
    End Set
  End Property


  Public ReadOnly Property IsRamping() As Boolean
    Get
      Return (State = S10.Run)
    End Get
  End Property
  Public ReadOnly Property IsHolding() As Boolean
    Get
      Return (State = S10.Hold)
    End Get
  End Property
  Public ReadOnly Property IsPaused() As Boolean
    Get
      Return (State = S10.Pause)
    End Get
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command10 As New Command10(Me)
End Class
#End Region
