Public Class TemperatureControl

  'Temperature Control class module
  '
  'TODO: ?
  '   Add temperature mode state (HeatOnly, CoolOnly, HeatOrCool) so we can limit
  '   temperature states accordingly.

  Public Message_TemperatureHigh As Boolean
  Public Message_TemperatureLow As Boolean
  Public Alarms_CrashCooling As Boolean
  Public Alarms_CrashCoolingDone As Boolean
  Public IgnoreErrors As Boolean

  'PID Heating Parameters - pass to PID while temperature state = heating
  <Translate("zh-TW", "TC:升溫比例參數"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(45)> _
    Public Parameters_HeatPropBand As Integer
  <Translate("zh-TW", "TC:升溫積分參數"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(250)> _
    Public Parameters_HeatIntegral As Integer
  <Translate("zh-TW", "TC:升溫最大斜率"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(60)> _
    Public Parameters_HeatMaxGradient As Integer
  <Translate("zh-TW", "TC:升溫到達溫度"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(10)> _
    Public Parameters_HeatStepMargin As Integer

  'PID Cooling Parameters - pass to PID while temperature state = cooling
  <Translate("zh-TW", "TC:降溫比例參數"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(10)> _
    Public Parameters_CoolPropBand As Integer
  <Translate("zh-TW", "TC:降溫積分參數"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(250)> _
    Public Parameters_CoolIntegral As Integer
  <Translate("zh-TW", "TC:降溫最大斜率"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(50)> _
    Public Parameters_CoolMaxGradient As Integer
  <Translate("zh-TW", "TC:降溫到達溫度"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(10)> _
    Public Parameters_CoolStepMargin As Integer

#If 0 Then
  'PID Cool Rinse Parameters - pass to PID while temperature state = coolrinse
  <Category("Temperature Control"), DefaultValue(40)> _
    Public Parameters_CoolRinsePropBand As Integer
  <Category("Temperature Control"), DefaultValue(250)> _
    Public Parameters_CoolRinseIntegral As Integer
  <Category("Temperature Control"), DefaultValue(70)> _
    Public Parameters_CoolRinseMaxGradient As Integer
  <Category("Temperature Control"), DefaultValue(10)> _
    Public Parameters_CoolRinseStepMargin As Integer
#End If

  ' TODO: This next one is read by other commands, but not by us - this is a bit odd
  <Translate("zh-TW", "TC:升降溫模式"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(0)> _
    Public Parameters_HeatCoolModeChange As Integer

  <Translate("zh-TW", "TC:警報溫度"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(50), Unit(Unit.TemperatureTenths)> _
    Public Parameters_TemperatureAlarmBand As Integer
  <Translate("zh-TW", "TC:警報時間"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(120), Unit(Unit.Seconds)> _
    Public Parameters_TemperatureAlarmDelay As Integer

  <Translate("zh-TW", "TC:PID停止運算"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(120)> _
    Public Parameters_TemperaturePidPause As Integer
  <Translate("zh-TW", "TC:PID重新運算"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(20)> _
    Public Parameters_TemperaturePidRestart As Integer
  <Translate("zh-TW", "TC:PID重設"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(150)> _
    Public Parameters_TemperaturePidReset As Integer
  <Translate("zh-TW", "TC:升溫等待時間"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(10), Unit(Unit.Seconds)> _
    Public Parameters_HeatVentTime As Integer
  <Translate("zh-TW", "TC:降溫等待時間"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(10), Unit(Unit.Seconds)> _
    Public Parameters_CoolVentTime As Integer

  <Translate("zh-TW", "TC:急速降溫到達溫度"), Category("Temperature Control"), TranslateCategory("zh-TW", "溫度控制"), DefaultValue(800), Unit(Unit.TemperatureTenths)> _
    Public Parameters_CrashCoolTemperature As Integer

  Private Enum ModeValue
    HeatAndCool
    HeatOnly
    CoolOnly
    ChangeDisabled
  End Enum
  Private mode_ As ModeValue

  Private Enum StateValue
    Off
    Start
    Pause
    PreHeatVent
    Heat
    PostHeatVent
    PreCoolVent
    Cool
    PostCoolVent
    CrashCoolStart
    CrashCoolPause
    CrashCoolVent
    CrashCool
    CrashCoolDone
    CrashCoolRestart
  End Enum
  Private State As StateValue
  Private PreviousState As StateValue
  Private pid_ As New PidControl
  Private idleTimer_ As New Timer
  Private stateTimer_ As New Timer
  Private enableTimer_ As New Timer
  Private heatDelayTimer_ As New Timer
  Private coolDelayTimer_ As New Timer
  Private modeChangeTimer_ As New Timer

  'PID setpoints - these are useful for restarting temp control after crash cooling
  Private pidStartTemp_ As Integer
  Private pidFinalTemp_ As Integer
  Private pidGradient_ As Integer

  'Temperature control parameters
  'Temp enabled must be made for this time before we can heat or cool
  'Can be zero (default = 10)
  Private enableDelay_ As Integer
  'If we we're cooling and now we want to heat - delay heating for this time (and vice versa)
  'This can be used to ensure the heat exchanger drain valve has been open long enough to
  'fully drain the heat exchanger prior to heating or cooling.
  'Can be zero (default = 10)
  Private tempHeatCoolDelay_ As Integer
  'Usual mode change delay - can be zero (default = 120)
  Private heatCoolModeChangeDelay_ As Integer
  'Time to vent prior to heating - can be zero (default = 10)
  Private tempPreHeatVentTime_ As Integer
  'Time to vent after heating - can be zero (default = 10)
  Private tempPostHeatVentTime_ As Integer
  'Time to vent prior to cooling - can be zero (default = 10)
  Private tempPreCoolVentTime_ As Integer
  'Time to vent after cooling - can be zero (default = 10)
  Private tempPostCoolVentTime_ As Integer

  'Crash Cool setpoint
  Private tempCrashCoolTemp_ As Integer

  Private firstTempStart_ As Boolean

  Private coolPropBand_ As Integer
  Private coolIntegral_ As Integer
  Private coolMaxGradient_ As Integer
  Private coolStepMargin_ As Integer


  Public Sub New()
    ' Default values for temperature control
    mode_ = ModeValue.HeatAndCool
    State = StateValue.Off
    stateTimer_.TimeRemaining = 10
    enableTimer_.TimeRemaining = 10

    ' Set parameters to defaults
    enableDelay_ = 10
    tempHeatCoolDelay_ = 30
    heatCoolModeChangeDelay_ = 60
    tempPreHeatVentTime_ = 10
    tempPostHeatVentTime_ = 10
    tempPreCoolVentTime_ = 10
    tempPostCoolVentTime_ = 10

    tempCrashCoolTemp_ = 800
    firstTempStart_ = True
  End Sub

  Public Sub Start(ByVal vesTemp As Integer, ByVal finalTempInTenths As Integer, ByVal gradientInTenthsPerMinute As Integer)
    ' Set start temperature, target temperature and gradient
    pidStartTemp_ = vesTemp
    pidFinalTemp_ = finalTempInTenths
    pidGradient_ = gradientInTenthsPerMinute

    ' Set pid parameters to heat by default
    pid_.PropBand = Parameters_HeatPropBand
    pid_.Integral = Parameters_HeatIntegral
    pid_.MaxGradient = Parameters_HeatMaxGradient
    pid_.HoldMargin = Parameters_HeatStepMargin

    firstTempStart_ = True
    'If crash cooling exit - PID will start when crash cool done
    If IsCrashCoolOn Then Exit Sub

    'Start the PID
    pid_.Start(pidStartTemp_, pidFinalTemp_, pidGradient_)

    'Decide whether we should heat, cool or wait and see
    'Decision is deliberately biased in favour of heating - cos that's more likely

    'If we're already heating (or about to heat) and the current temp is lower than
    'the Final Temp (and a bit) then keep going i.e. don't change state
    If IsHeating Or IsPreHeatVent Then
      If vesTemp <= (finalTempInTenths + Parameters_HeatPropBand) Then Exit Sub
    End If

    'If we're already cooling (or about to cool) and the current temp is higher than
    'the Final Temp (by a bit) then keep going i.e. don't change state
    If IsCooling Or IsPreCoolVent Then
      If vesTemp > (finalTempInTenths + Parameters_CoolPropBand) Then Exit Sub
    End If

    'If we're venting after heat or cool then let vent finish and allow start state to
    'decide which way to go
    If IsPostHeatVent Or IsPostCoolVent Then Exit Sub

    'Set state to start
    State = StateValue.Start
    stateTimer_.TimeRemaining = 5
  End Sub

  Public Sub Run(ByVal vesTemp As Integer)
    Select Case State
      Case StateValue.Off
        'Set previous state variable
        PreviousState = StateValue.Start
        'Set state timer
        stateTimer_.TimeRemaining = 5

      Case StateValue.Start
        'Set previous state variable
        PreviousState = StateValue.Start
        'If no start temp or final temp then set state to off
        If pidStartTemp_ = 0 Or pidFinalTemp_ = 0 Then State = StateValue.Off
        'Wait for state timer
        If Not stateTimer_.Finished Then Exit Sub
        'Wait for temperature enable
        If Not enableTimer_.Finished Then Exit Sub
        'Reset PID to clear out any "funnies"
        'Comment this out to see if it cures cool on gradient bug
        'PID.Reset VesTemp
        'Run PID to decide whether to heat or cool
        pid_.Run(vesTemp)
        'Added to allow us to go straight to cooling on a controlled gradient
        'If we need to cool, don't think about heating yet
        If firstTempStart_ Then
          firstTempStart_ = False
          If vesTemp > (pidFinalTemp_ + Parameters_CoolPropBand) Then
            State = StateValue.PreCoolVent
            stateTimer_.TimeRemaining = tempPreCoolVentTime_
            Exit Sub
          End If
        End If
        'If PID Output is greater than zero then heat
        If pid_.Output > 0 Then
          'Are we allowed to heat ?
          If mode_ = ModeValue.CoolOnly Then Exit Sub
          'Have we waited long enough before switching to heating
          If Not heatDelayTimer_.Finished Then Exit Sub
          State = StateValue.PreHeatVent
          stateTimer_.TimeRemaining = tempPreHeatVentTime_
        End If
        'If PID Output is less than zero then cool
        If (pid_.Output < 0) Then
          'Are we allowed to cool ?
          If mode_ = ModeValue.HeatOnly Then Exit Sub
          'Have we waited long enough before switching to cooling
          If Not coolDelayTimer_.Finished Then Exit Sub
          State = StateValue.PreCoolVent
          stateTimer_.TimeRemaining = tempPreCoolVentTime_
        End If

      Case StateValue.Pause
        'Pause PID
        pid_.Pause()
        'Wait for state timer
        If Not stateTimer_.Finished Then Exit Sub
        'Wait for temperature enable
        If Not enableTimer_.Finished Then Exit Sub
        'Switch back to previous state
        State = PreviousState
        'Restart PID
        pid_.Restart()
        'If venting set timer to parameter value
        If State = StateValue.PreHeatVent Then stateTimer_.TimeRemaining = tempPreHeatVentTime_
        If State = StateValue.PostHeatVent Then stateTimer_.TimeRemaining = tempPostHeatVentTime_
        If State = StateValue.PreCoolVent Then stateTimer_.TimeRemaining = tempPreCoolVentTime_
        If State = StateValue.PostCoolVent Then stateTimer_.TimeRemaining = tempPostCoolVentTime_

        'Heating

      Case StateValue.PreHeatVent
        'Set previous state variable
        PreviousState = StateValue.PreHeatVent
        'Reset cool delay timer
        coolDelayTimer_.TimeRemaining = tempHeatCoolDelay_
        'If enable timer set switch to pause state
        If Not enableTimer_.Finished Then State = StateValue.Pause
        'Wait for parameter time then switch to heating
        If Not stateTimer_.Finished Then Exit Sub
        State = StateValue.Heat
        'Reset mode change timer
        modeChangeTimer_.TimeRemaining = heatCoolModeChangeDelay_
        'Reset PID to start from current temp
        pid_.Reset(vesTemp)

      Case StateValue.Heat
        'Set previous state variable
        PreviousState = StateValue.Heat
        'Reset cool delay timer
        coolDelayTimer_.TimeRemaining = tempHeatCoolDelay_
        'If enable timer set switch to pause state
        If Not enableTimer_.Finished Then State = StateValue.Pause
        'Set PID parameters here so that changes apply immediately
        pid_.PropBand = Parameters_HeatPropBand
        pid_.Integral = Parameters_HeatIntegral
        pid_.MaxGradient = Parameters_HeatMaxGradient
        pid_.HoldMargin = Parameters_HeatStepMargin
        'Run PID Control
        pid_.Run(vesTemp)
        'Reset mode change timer if mode change disabled (Note parameter could be zero)
        If mode_ = ModeValue.ChangeDisabled Then modeChangeTimer_.TimeRemaining = heatCoolModeChangeDelay_ + 1
        'Reset mode change timer while we're still calling for heating
        If pid_.Output >= 0 Then modeChangeTimer_.TimeRemaining = heatCoolModeChangeDelay_
        If modeChangeTimer_.Finished Then
          State = StateValue.PostHeatVent
          stateTimer_.TimeRemaining = tempPostHeatVentTime_
        End If

      Case StateValue.PostHeatVent
        'Set previous state variable
        PreviousState = StateValue.PostHeatVent
        'Reset cool delay timer
        coolDelayTimer_.TimeRemaining = tempHeatCoolDelay_
        'If enable timer set switch to pause state
        If Not enableTimer_.Finished Then State = StateValue.Pause
        'Wait for parameter time then switch to idle state
        If Not stateTimer_.Finished Then Exit Sub
        State = StateValue.Start
        stateTimer_.TimeRemaining = 5

        'Cooling

      Case StateValue.PreCoolVent
        'Set previous state variable
        PreviousState = StateValue.PreCoolVent
        'Reset heat delay timer
        heatDelayTimer_.TimeRemaining = tempHeatCoolDelay_
        'If enable timer set switch to pause state
        If Not enableTimer_.Finished Then State = StateValue.Pause
        'Wait for parameter time then switch to cooling
        If Not stateTimer_.Finished Then Exit Sub
        State = StateValue.Cool
        'Reset mode change timer
        modeChangeTimer_.TimeRemaining = heatCoolModeChangeDelay_
        'Reset PID to start from current temp
        pid_.Reset(vesTemp)

      Case StateValue.Cool
        'Set previous state variable
        PreviousState = StateValue.Cool
        'Reset heat delay timer
        heatDelayTimer_.TimeRemaining = tempHeatCoolDelay_
        'If enable timer set switch to pause state
        If Not enableTimer_.Finished Then State = StateValue.Pause
        'Set PID parameters here so that changes apply immediately
        pid_.PropBand = coolPropBand_
        pid_.Integral = coolIntegral_
        pid_.MaxGradient = coolMaxGradient_
        pid_.HoldMargin = coolStepMargin_
        'Run PID Control
        pid_.Run(vesTemp)
        'Reset mode change timer if mode change disabled (Note parameter could be zero)
        If mode_ = ModeValue.ChangeDisabled Then modeChangeTimer_.TimeRemaining = heatCoolModeChangeDelay_ + 1
        'Reset mode change timer while we're still calling for cooling
        If pid_.Output <= 0 Then modeChangeTimer_.TimeRemaining = heatCoolModeChangeDelay_
        If modeChangeTimer_.Finished Then
          State = StateValue.PostCoolVent
          stateTimer_.TimeRemaining = tempPostCoolVentTime_
        End If

      Case StateValue.PostCoolVent
        'Set previous state variable
        PreviousState = StateValue.PostCoolVent
        'Reset heat delay timer
        heatDelayTimer_.TimeRemaining = tempHeatCoolDelay_
        'If enable timer set switch to pause state
        If Not enableTimer_.Finished Then State = StateValue.Pause
        'Wait for parameter time then switch to idle state
        If Not stateTimer_.Finished Then Exit Sub
        'Switch to cooling state
        State = StateValue.Start
        stateTimer_.TimeRemaining = 5

        'Crash cooling

      Case StateValue.CrashCoolStart
        'Wait for parameter time
        If Not stateTimer_.Finished Then Exit Sub
        'Wait for cool delay time
        If Not coolDelayTimer_.Finished Then Exit Sub
        'Switch to crash cool vent
        State = StateValue.CrashCoolVent
        stateTimer_.TimeRemaining = tempPreCoolVentTime_
        'Start PID
        pid_.Start(vesTemp, (tempCrashCoolTemp_ - 20), 0)

      Case StateValue.CrashCoolPause
        If Not stateTimer_.Finished Then Exit Sub
        If Not enableTimer_.Finished Then Exit Sub
        'Everythings okay so go back to what we were doing before
        State = PreviousState
        'Reset PID
        pid_.Reset(vesTemp)

      Case StateValue.CrashCoolVent
        'Set previous state variable
        PreviousState = StateValue.CrashCoolVent
        'Reset heat delay timer
        heatDelayTimer_.TimeRemaining = tempHeatCoolDelay_
        'If enable timer set switch to pause state
        If Not enableTimer_.Finished Then State = StateValue.CrashCoolPause
        'Wait for parameter time
        If Not stateTimer_.Finished Then Exit Sub
        'Switch to Crash Cool
        State = StateValue.CrashCool
        'Reset PID to start from current temp
        pid_.Reset(vesTemp)

      Case StateValue.CrashCool
        'Set previous state variable
        PreviousState = StateValue.CrashCool
        'Reset heat delay timer
        heatDelayTimer_.TimeRemaining = tempHeatCoolDelay_
        'If pump not running switch to pause
        If Not enableTimer_.Finished Then State = StateValue.CrashCoolPause
        'Run the PID
        pid_.Run(vesTemp)
        'If we're at or below crash cool temperature start holding
        If vesTemp <= tempCrashCoolTemp_ Then State = StateValue.CrashCoolDone

      Case StateValue.CrashCoolDone
        'Set previous state variable
        PreviousState = StateValue.CrashCoolDone
        'Reset heat delay timer
        heatDelayTimer_.TimeRemaining = tempHeatCoolDelay_
        'If pump not running switch to vent
        If Not enableTimer_.Finished Then State = StateValue.CrashCoolPause
        'Run the PID
        pid_.Run(vesTemp)

      Case StateValue.CrashCoolRestart
        'TO DO: Check hold time / gradient and decide wether to use original gradient, max
        'gradient or cancel and go to next step ?
        'Go to Post cool vent
        State = StateValue.PostCoolVent
        stateTimer_.TimeRemaining = tempPostCoolVentTime_
        'Check to see if we were cooling - if so just maintain current temp
        If (pidStartTemp_ > pidFinalTemp_) And (pidFinalTemp_ < Parameters_CrashCoolTemperature) Then
          pid_.Start(vesTemp, pidFinalTemp_, 0)
          Exit Sub
        End If
        'If temperature control not active then clear PID (?) and carry on
        If pidStartTemp_ = 0 Or pidFinalTemp_ = 0 Then
          pid_.Cancel()
          Exit Sub
        End If
        'Restart pid with original final temp and gradient
        pid_.Start(vesTemp, pidFinalTemp_, pidGradient_)
    End Select
  End Sub

  Public Sub CheckErrorsAndMakeAlarms(ByVal controlCode As ControlCode)
    With controlCode
      'Check pid pause and reset
      Dim TempError As Integer
      TempError = 0
      If IsHeating Then TempError = TempSetpoint - .IO.MainTemperature
      If IsCooling Then TempError = .IO.MainTemperature - TempSetpoint
      IgnoreErrors = (IsCrashCoolOn Or (IsMaxGradient And _
        (.Command10.IsRamping Or .Command01.IsRamping)))
      If Not IgnoreErrors Then
        'If TempError > Parameters_TemperaturePidReset Then pidReset(.IO.MainTemperature)
        If TempError > Parameters_TemperaturePidPause Then pidPause()
        If IsPidPaused Then
          If TempError < Parameters_TemperaturePidRestart Then pidRestart()
        End If
      End If
      MakeAlarms(.IO.MainTemperature, IgnoreErrors)
    End With
  End Sub

  Public Sub Cancel()
    pid_.Cancel()
    State = StateValue.Off
    pidStartTemp_ = 0
    pidFinalTemp_ = 0
    pidGradient_ = 0
  End Sub

  Public Sub CrashCoolStart()
    If Not IsCrashCoolOn Then State = StateValue.CrashCoolStart
  End Sub

  Public Sub CrashCoolStop()
    If IsCrashCooling Then State = StateValue.CrashCoolRestart
  End Sub

  Public Sub pidPause()
    pid_.Pause()
  End Sub

  ' TODO: don't really need this return value
  Public Function pidRestart() As Boolean
    pid_.Restart()
    pidRestart = True
  End Function

  Public Sub pidReset(ByVal vesTemp As Integer)
    pid_.Reset(vesTemp)
  End Sub

  Public Sub resetEnableTimer()
    enableTimer_.TimeRemaining = enableDelay_
  End Sub

  Public Property BalanceTemperature() As Integer
    Get
      Return pid_.BalanceTemp
    End Get
    Set(ByVal value As Integer)
      pid_.BalanceTemp = value
    End Set
  End Property

  Public WriteOnly Property BalancePercent() As Integer
    Set(ByVal value As Integer)
      pid_.BalancePercent = value
    End Set
  End Property

  Public WriteOnly Property EnableDelay() As Integer
    Set(ByVal value As Integer)
      enableDelay_ = Math.Min(Math.Max(value, 0), 30)
    End Set
  End Property

  Public WriteOnly Property TempMode() As Integer
    Set(ByVal value As Integer)
      mode_ = ModeValue.HeatAndCool
      If value = 2 Then mode_ = ModeValue.ChangeDisabled
      If value = 3 Then mode_ = ModeValue.HeatOnly
      If value = 4 Then mode_ = ModeValue.CoolOnly
    End Set
  End Property

  Public WriteOnly Property CoolingPropBand() As Integer
    Set(ByVal value As Integer)
      coolPropBand_ = value
    End Set
  End Property

  Public WriteOnly Property CoolingIntegral() As Integer
    Set(ByVal value As Integer)
      coolIntegral_ = value
    End Set
  End Property

  Public WriteOnly Property CoolingMaxGradient() As Integer
    Set(ByVal value As Integer)
      coolMaxGradient_ = value
    End Set
  End Property

  Public WriteOnly Property CoolingStepMargin() As Integer
    Set(ByVal value As Integer)
      coolStepMargin_ = value
    End Set
  End Property

  Public Property HeatCoolModeChangeDelay() As Integer
    Get
      Return heatCoolModeChangeDelay_
    End Get
    Set(ByVal value As Integer)
      heatCoolModeChangeDelay_ = Math.Min(Math.Max(value, 0), 600)
    End Set
  End Property

  Public WriteOnly Property PreHeatVentTime() As Integer
    Set(ByVal value As Integer)
      tempPreHeatVentTime_ = Math.Min(Math.Max(value, 0), 60)
    End Set
  End Property

  Public WriteOnly Property PostHeatVentTime() As Integer
    Set(ByVal value As Integer)
      tempPostHeatVentTime_ = Math.Min(Math.Max(value, 0), 60)
    End Set
  End Property

  Public WriteOnly Property PreCoolVentTime() As Integer
    Set(ByVal value As Integer)
      tempPreCoolVentTime_ = Math.Min(Math.Max(value, 0), 60)
    End Set
  End Property

  Public WriteOnly Property PostCoolVentTime() As Integer
    Set(ByVal value As Integer)
      tempPostCoolVentTime_ = Math.Min(Math.Max(value, 0), 60)
    End Set
  End Property

  Public WriteOnly Property CrashCoolTemp() As Integer
    Set(ByVal value As Integer)
      tempCrashCoolTemp_ = Math.Min(Math.Max(value, 0), 60)
    End Set
  End Property

  Public ReadOnly Property IsEnabled() As Boolean
    Get
      Return enableTimer_.Finished
    End Get
  End Property

  Public ReadOnly Property IsOn() As Boolean
    Get
      Return (State <> StateValue.Off)
    End Get
  End Property

  Public ReadOnly Property IsIdle() As Boolean
    Get
      Return (State = StateValue.Start) Or (State = StateValue.Off)
    End Get
  End Property

  Public ReadOnly Property IsPaused() As Boolean
    Get
      Return (State = StateValue.Pause)
    End Get
  End Property

  Public ReadOnly Property IsHeating() As Boolean
    Get
      Return (State = StateValue.Heat)
    End Get
  End Property

  Public ReadOnly Property IsCooling() As Boolean
    Get
      Return (State = StateValue.Cool) Or IsCrashCooling
    End Get
  End Property

  Public ReadOnly Property IsMaxGradient() As Boolean
    Get
      'Set max gradient only if we are ramping up/down (it's used to disable alarms)
      ' return (Not PID.IsHolding) And PID.IsMaxGradient
      Return pid_.IsMaxGradient
    End Get
  End Property

  Public ReadOnly Property IsHolding() As Boolean
    Get
      'Make sure IsHolding returns false during Crashcooling
      Return (Not IsCrashCoolOn) And pid_.IsHolding
    End Get
  End Property

  Public ReadOnly Property IsPreHeatVent() As Boolean
    Get
      Return (State = StateValue.PreHeatVent)
    End Get
  End Property

  Public ReadOnly Property IsPostHeatVent() As Boolean
    Get
      Return (State = StateValue.PostHeatVent)
    End Get
  End Property

  Public ReadOnly Property IsPreCoolVent() As Boolean
    Get
      Return (State = StateValue.PreCoolVent) Or IsCrashCoolVent
    End Get
  End Property

  Public ReadOnly Property IsPostCoolVent() As Boolean
    Get
      Return (State = StateValue.PostCoolVent)
    End Get
  End Property

  Public ReadOnly Property Output() As Integer
    Get
      'Analog Output
      If State = StateValue.Heat Then Output = pid_.Output
      If State = StateValue.Cool OrElse State = StateValue.CrashCool Then Output = -pid_.Output

      'Limit output
      If Output < 0 Then Output = 0
      If Output > 1000 Then Output = 1000
    End Get
  End Property

  Public ReadOnly Property IsCrashCoolOn() As Boolean
    Get
      Return (State = StateValue.CrashCoolStart) Or (State = StateValue.CrashCoolPause) Or _
             IsCrashCoolVent Or IsCrashCooling Or IsCrashCoolDone
    End Get
  End Property

  Public ReadOnly Property IsCrashCoolDone() As Boolean
    Get
      Return (State = StateValue.CrashCoolDone)
    End Get
  End Property

  Private ReadOnly Property IsCrashCoolVent() As Boolean
    Get
      Return (State = StateValue.CrashCoolVent)
    End Get
  End Property

  Private ReadOnly Property IsCrashCooling() As Boolean
    Get
      Return (State = StateValue.CrashCool) Or IsCrashCoolDone
    End Get
  End Property

  Public ReadOnly Property IsPidPaused() As Boolean
    Get
      Return pid_.IsPaused
    End Get
  End Property

  'Added for display purposes
  Public ReadOnly Property TempGradient() As Integer
    Get
      Return pid_.Gradient
    End Get
  End Property

  Public ReadOnly Property TempFinalTemp() As Integer
    Get
      Return pid_.FinalTemp
    End Get
  End Property

  <GraphTrace(1, 1500, 6000, 9500, "Blue", "%tC"), Translate("zh-TW", "設定溫度")> _
  Public ReadOnly Property TempSetpoint() As Integer
    Get
      Return pid_.Setpoint
    End Get
  End Property

  Public ReadOnly Property pidPropTerm() As Integer
    Get
      Return pid_.PropTerm
    End Get
  End Property

  Public ReadOnly Property pidIntegralTerm() As Integer
    Get
      Return pid_.IntegralTerm
    End Get
  End Property

  Public ReadOnly Property pidHeatLossTerm() As Integer
    Get
      Return pid_.HeatLossTerm
    End Get
  End Property

  Public ReadOnly Property pidGradientTerm() As Integer
    Get
      Return pid_.GradientTerm
    End Get
  End Property

  ' Make alarms for temperature control
  Private Sub MakeAlarms(ByVal vesTemp As Integer, ByVal ignoreErrors As Boolean)
    'Temperature low/high s
    Static TempLoAlarmTimer As New Timer
    Static TempHiAlarmTimer As New Timer
    If ignoreErrors Or (Not (IsHeating Or IsCooling)) Then
      TempLoAlarmTimer.TimeRemaining = Parameters_TemperatureAlarmDelay
      TempHiAlarmTimer.TimeRemaining = Parameters_TemperatureAlarmDelay
    End If
    If vesTemp > (TempSetpoint - Parameters_TemperatureAlarmBand) Then
      TempLoAlarmTimer.TimeRemaining = Parameters_TemperatureAlarmDelay
    End If
    If vesTemp < (TempSetpoint + Parameters_TemperatureAlarmBand) Then
      TempHiAlarmTimer.TimeRemaining = Parameters_TemperatureAlarmDelay
    End If
    Message_TemperatureLow = TempLoAlarmTimer.Finished And ((pid_.GradientTerm > 0) Or pid_.IsHolding)
    Message_TemperatureHigh = TempHiAlarmTimer.Finished And ((pid_.GradientTerm > 0) Or pid_.IsHolding)

    'Crash Cools
    'Alarms_CrashCooling = IsCrashCoolOn And (Not IsCrashCoolDone)
    'Alarms_CrashCoolingDone = IsCrashCoolDone
  End Sub
End Class


' --------------------------------------------------------------------------
Public Class PidControl
  'PID Control - NOTE: Max Gradient = 0 rather than 99
  Private Enum StateValue                               ' Are we heating, cooling or holding temp
    RampUp
    RampDown
    Hold
  End Enum
  Private state_ As StateValue

  Private startTemp_ As Integer                ' Start temperature for PID
  Private finalTemp_ As Integer                ' Final temperature for PID
  Private gradient_ As Integer                 ' Gradient in degrees per minute
  Private gradientTimer_ As New TimerUp        ' Timer for gradient control
  Private setPoint_ As Integer                 ' Calculated setpoint
  Private output_ As Integer                   ' pid output in tenths of percent

  Private propTerm_ As Integer                 ' Declared as global so we can see from
  Private integralTerm_ As Integer             ' the outside
  Private heatLossTerm_ As Integer             '
  Private gradientTerm_ As Integer             '

  Private errorSum_ As Integer                 ' Declared as global so we can easily reset
  Private errorSumCounter_ As Integer          '     "
  Private errorSumTimer_ As New Timer          '     "

  Private propBand_ As Integer                 ' PID control parameters
  Private integral_ As Integer                 '     "
  Private maxGradient_ As Integer              '     "
  Private balanceTemp_ As Integer              '     "
  Private balancePercent_ As Integer           '     "
  Private holdMargin_ As Integer               ' Go to hold if within this margin of setpoint

  Private paused_ As Boolean                   ' PID paused ?
  Private cancelled_ As Boolean                ' PID cancelled ?


  Public Sub New()
    ' TODO: make in centigrade instead via a Sub New (isFahrenheit as boolean)
    propBand_ = 22              ' Default PID parameters (assumes Farenheit)
    integral_ = 250             '     "
    maxGradient_ = 30           '     "
    balanceTemp_ = 800          '     "
    balancePercent_ = 100        '     "
    holdMargin_ = 22            '     "
    cancelled_ = True           '
  End Sub

  Public Sub Start(ByVal startTempInTenths As Integer, ByVal finalTempInTenths As Integer, _
                   ByVal gradientInTenthsPerMinute As Integer)
    'Set parameters
    startTemp_ = startTempInTenths
    finalTemp_ = finalTempInTenths
    gradient_ = gradientInTenthsPerMinute
    gradientTimer_.Start()

    'Reset control
    paused_ = False
    cancelled_ = False
    ResetIntegral()

    'Are we heating or cooling ?
    state_ = StateValue.RampUp
    If finalTemp_ < startTemp_ Then state_ = StateValue.RampDown
  End Sub

  Public Sub Run(ByVal currentTempInTenths As Integer)
    'Check to see if PID cancelled
    If cancelled_ Then Exit Sub

    'Calculate Setpoint
    Dim RampFinished As Boolean
    If (state_ = StateValue.Hold) Then RampFinished = True
    If (state_ = StateValue.RampUp) And (setPoint_ >= finalTemp_) Then RampFinished = True
    If (state_ = StateValue.RampDown) And (setPoint_ <= finalTemp_) And Not setPoint_ = 0 Then RampFinished = True

    If ((currentTempInTenths > (finalTemp_ - holdMargin_)) And _
       (currentTempInTenths < (finalTemp_ + holdMargin_))) Or IsMaxGradient Then
      state_ = StateValue.Hold
      setPoint_ = finalTemp_
      RampFinished = True
    End If

    If Not (IsMaxGradient Or RampFinished) Then
      If (state_ = StateValue.RampUp) Then setPoint_ = startTemp_ + ((gradientTimer_.TimeElapsed * gradient_) \ 60)
      If (state_ = StateValue.RampDown) Then setPoint_ = startTemp_ - ((gradientTimer_.TimeElapsed * gradient_) \ 60)
    Else
      setPoint_ = finalTemp_
      If IsRampUp And (currentTempInTenths > (finalTemp_ - holdMargin_)) Then
        state_ = StateValue.Hold
      End If
      If IsRampDown And (currentTempInTenths < (finalTemp_ + holdMargin_)) Then
        state_ = StateValue.Hold
      End If
    End If

    'Calculate error
    Dim tempError = setPoint_ - currentTempInTenths

    'Calculate proportional Term
    propTerm_ = (tempError * 1000) \ Math.Max(propBand_, 1) ' avoid division by 0

    'If PID output is maxxed out stop Integral action.
    'This should prevent Integral saturation i.e. Error Sum/Integral term getting huge!
    Dim stopIntegral = (output_ = 1000 And tempError > 0) Or (output_ = -1000 And tempError < 0)
    'Calculate Error Sum for integral term - add once a second if allowed
    If (Not stopIntegral) And errorSumTimer_.Finished Then
      errorSumTimer_.TimeRemaining = 2
      errorSum_ += tempError
    End If

    'Calculate Integral Term - limit to +/- 100%
    integralTerm_ = Math.Min(Math.Max((errorSum_ * integral_) \ 600, -1000), 1000)

    'Calculate heat loss term - limit to +/- 100%

    heatLossTerm_ = Math.Min(Math.Max(((currentTempInTenths - balanceTemp_) * balancePercent_) \ 1000, -1000), 1000)
    'Calculate gradient term - limit to +/- 100%
    If IsHolding Then
      gradientTerm_ = 0
    Else
      gradientTerm_ = Math.Min(Math.Max((1000 * gradient_) \ Math.Max(maxGradient_, 1), -1000), 1000)
      If state_ = StateValue.RampDown Then gradientTerm_ = -gradientTerm_
    End If

    'Calculate output  - limit to +/- 100%
    output_ = Math.Min(Math.Max(propTerm_ + integralTerm_ + heatLossTerm_ + gradientTerm_, -1000), 1000)
  End Sub

  Public Sub Pause()
    paused_ = True
    gradientTimer_.Pause()
  End Sub

  Public Sub Restart()
    paused_ = False
    gradientTimer_.Restart()
  End Sub

  Public Sub Reset(ByVal currentTempInTenths As Integer)
    paused_ = False
    startTemp_ = currentTempInTenths
    gradientTimer_.Start()
    ResetIntegral()
  End Sub

  Public Sub Cancel()
    cancelled_ = True
    ResetIntegral()
    startTemp_ = 0
    finalTemp_ = 0
    gradient_ = 0
    setPoint_ = 0
    output_ = 0
  End Sub

  Private Sub ResetIntegral()
    'Reset integral
    errorSum_ = 0
    errorSumTimer_.TimeRemaining = 10
    errorSumCounter_ = 0
  End Sub

  Public WriteOnly Property PropBand() As Integer
    Set(ByVal value As Integer)
      propBand_ = Math.Min(Math.Max(value, 0), 1000)
    End Set
  End Property

  Public WriteOnly Property Integral() As Integer
    Set(ByVal value As Integer)
      integral_ = Math.Min(Math.Max(value, 0), 1000)
    End Set
  End Property

  Public WriteOnly Property MaxGradient() As Integer
    Set(ByVal value As Integer)
      maxGradient_ = Math.Min(Math.Max(value, 0), 1000)
    End Set
  End Property

  Public Property BalanceTemp() As Integer
    Get
      Return balanceTemp_
    End Get
    Set(ByVal value As Integer)
      balanceTemp_ = Math.Min(Math.Max(value, 0), 1000)
    End Set
  End Property

  Public Property BalancePercent() As Integer
    Get
      Return balanceTemp_
    End Get
    Set(ByVal value As Integer)
      balancePercent_ = Math.Min(Math.Max(value, 0), 1000)
    End Set
  End Property

  Public WriteOnly Property HoldMargin() As Integer
    Set(ByVal value As Integer)
      holdMargin_ = Math.Min(Math.Max(value, 10), 100)
    End Set
  End Property

  Public ReadOnly Property Output() As Integer
    Get
      Output = output_
      If cancelled_ Then Output = 0
    End Get
  End Property

  Public ReadOnly Property Gradient() As Integer
    Get
      Return gradient_
    End Get
  End Property

  Public ReadOnly Property FinalTemp() As Integer
    Get
      Return finalTemp_
    End Get
  End Property

  Public ReadOnly Property Setpoint() As Integer
    Get
      Return setPoint_
    End Get
  End Property

  Public ReadOnly Property IsRampUp() As Boolean
    Get
      Return state_ = StateValue.RampUp
    End Get
  End Property

  Public ReadOnly Property IsRampDown() As Boolean
    Get
      Return state_ = StateValue.RampDown
    End Get
  End Property

  Public ReadOnly Property IsHolding() As Boolean
    Get
      Return state_ = StateValue.Hold
    End Get
  End Property

  Public ReadOnly Property IsRamping() As Boolean
    Get
      Return Not (IsMaxGradient Or IsHolding)
    End Get
  End Property

  Public ReadOnly Property IsMaxGradient() As Boolean
    Get
      Return (gradient_ = 0)
    End Get
  End Property

  Public ReadOnly Property IsPaused() As Boolean
    Get
      Return paused_
    End Get
  End Property

  Public ReadOnly Property PropTerm() As Integer
    Get
      Return propTerm_
    End Get
  End Property

  Public ReadOnly Property IntegralTerm() As Integer
    Get
      Return integralTerm_
    End Get
  End Property

  Public ReadOnly Property HeatLossTerm() As Integer
    Get
      Return heatLossTerm_
    End Get
  End Property

  Public ReadOnly Property GradientTerm() As Integer
    Get
      Return gradientTerm_
    End Get
  End Property
End Class
