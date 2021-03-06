Public NotInheritable Class ControlCode
  Inherits MarshalByRefObject
  Implements ACControlCode
  Public Parent As ACParent
  Friend Language As LanguageValue

  Public Enum DelayReason
    正常運作
    呼叫操作員超時
    蒸氣不足
    冷卻水不足
    呼叫染料超時
    呼叫助劑超時
    取樣超時
    主缸進冷熱水超時
    主缸進冷水超時
    主缸進熱水超時
    動力排水超時
    B缸備藥超時
    C缸備藥超時
    B缸加藥超時
    C缸加藥超時
    等待染料超時
  End Enum

  Public Delay As DelayReason
  Public 程式執行小時 As Integer
  Public 程式執行分鐘 As Integer
  Public 預計時間小時 As Integer
  Public 預計時間分鐘 As Integer
  Public 預計時間 As Double
  Public 染色水量統計開始 As Boolean
  Public 染色水量統計結束 As Boolean

  Public StopDelayAlarmTimer As New Timer
  Public DelayAlarmOn As Boolean

  '連接SPC資料庫用的變數
  Public SPCConnectTimer As New Timer
  Public SPCConnectError As Boolean
  Public ComputerName As String

  Public SPCServerName As String = "192.168.3.2"
  Public SPCUserName As String = "sa1"
  Public SPCPassword As String = "sa1"
  'Public SPCServerName As String = "LEMON-PC\SQLEXPRESS"
  'Public SPCUserName As String = "sa"
  'Public SPCPassword As String = "84160206"
  Public StepNumber(30) As String
  Public ProductCode(30) As String
  Public ProductType(30) As String
  Public Grams(30) As String
  Public DispenseGrams(30) As String
  Public DispenseResult(30) As String


  '染料資料
  Public DyeStepNumber(10) As String
  Public DyeCode(10) As String
  Public DyeGrams(10) As String
  Public DyeDispenseGrams(10) As String
  Public DyeDispenseResult(10) As String
  '助劑資料
  Public ChemicalStepNumber(20) As String
  Public ChemicalCode(20) As String
  Public ChemicalGrams(20) As String
  Public ChemicalDispenseGrams(20) As String
  Public ChemicalDispenseResult(20) As String
  '呼叫252跟302用變數
  Public ChemicalCallOff As Integer
  Public ChemicalTank As Integer
  Public ChemicalState As Integer
  Public ChemicalEnabled As Integer
  Public ChemicalProducts As String            'This value is filled in by host / auto dispenser
  Public DyeCallOff As Integer
  Public DyeTank As Integer
  Public DyeState As Integer
  Public DyeEnabled As Integer
  Public DyeProducts As String                 'This value is filled in by host / auto dispenser

  '跟SPC溝通用的變數
  Public CCallOff As String
  Public CTank As String
  Public CState As String
  Public CEnabled As String
  Public DCallOff As String
  Public DTank As String
  Public DState As String
  Public DEnabled As String
  Public ProgramStopCleanDatabase As Boolean

  '自動備藥訊息
  Public DyeStepDispensing(12) As Boolean
  Public ChemicalStepDispensing(12) As Boolean
  Public DyeStepReady(12) As Boolean
  Public ChemicalStepReady(12) As Boolean


  'mimic用的變數
  Public 工單 As String
  Public 重染 As Integer
  Public 原料ID As Integer
  Public 讀取原料資料 As Boolean
  Public SPC連線異常 As Boolean


  Public TemperatureControl As New TemperatureControl
  ' Public Recipe As New Recipe
  Public Setpoint As Integer

  Public 纏車次數計數器 As Integer

  '總浴量計算用變數
  Public TotalWeight As Integer
  Public LiquidRatio As Integer
  Public TotalVolume As Integer
    Public FillWaterLiters As Integer
    Public FillWaterLitersWas As Integer
    Public TotalWaterLiters As Integer
    Public TotalWaterLitersWas As Integer
  Public HighSpeedCounter1 As Integer
  Public HighSpeedCounter1Was As Integer
  Public FillLiters As Integer
  Public FillLitersWas As Integer

  Public BTankHighLevel As Boolean
  Public BTankMiddleLevel As Boolean
  Public BTankLowLevel As Boolean
  Public TankBReady As Boolean
  Public BTankDosing As Boolean
  Public BDosingOn As Boolean
  Public BPumpOn As Boolean
  Public BCirMixOn As Boolean
  Public BInjOn As Boolean
  Public BTankAddDelay As Boolean
  Public CDosingOn As Boolean
  Public CPumpOn As Boolean
  Public CCirMixOn As Boolean
  Public CInjOn As Boolean
  Public CTankAddDelay As Boolean
  Public BAllinOn As Boolean
  Public CAllinOn As Boolean
  Public CTankHighLevel As Boolean
  Public CTankMiddleLevel As Boolean
  Public CTankLowLevel As Boolean
  Public PumpOn As Boolean
  Public TankCReady As Boolean
  Public CoolNow As Boolean
  Public HeatNow As Boolean
  Public Entanglement As Boolean
  Public TemperatureControlFlag As Boolean
  Public RemoteDisplayError As Boolean
  Public TempMore3 As Boolean
  Public FlashFlag As Boolean
  Public PumpSpeed As Integer
  Public Reel1Speed As Integer
  Public Reel2Speed As Integer
  Public PressureInTemp As Integer
  Public PressureOutTemp As Integer
  Public BAddStop As Boolean
  Public CAddStop As Boolean
  Public TempControlFlag As Boolean
  Public PressureInFlag As Boolean
  Public LA252Ready As Boolean
  Public RunCallLA252 As Boolean
  Public SideTankAuto As Boolean
  Public MainTankSafeToAdd As Boolean
  Public MainTankSafe As Boolean
  Public RinseFillbyBtank As Boolean
  Public RinseBTankDrain As Boolean
  Public HeatValve As Boolean
  Public CoolValve As Boolean
  Public TemperatureControlTimer As New Timer
  Public TemperatureControlTime As Integer
  Public CoolValveOpenTimer As New Timer
  Public CoolValveOpenTime As Integer
  Public HeatValveOpenTimer As New Timer
  Public HeatValveOpenTime As Integer
  Public MainPumpRunMins As Integer
  Public MainPumpRunTimer As New Timer
  Public Delay_BTankReady As Boolean
  Public EntangleWas As Boolean
  Public MainPumpStartWas As Boolean
  Public MainPumpStopWas As Boolean
  Public HeatStartWas As Boolean
  Public HeatStopWas As Boolean
  Public CoolStartWas As Boolean
  Public CoolStopWas As Boolean
  Public DrainStartWas As Boolean
  Public DrainStopWas As Boolean
  Public CondenserDrainWas As Boolean
  Public CallTemperature As Integer
  Public ReachTemperature_Check As Boolean
  Public ReachTemperature_Alarm As Boolean
  Public HeatOverTime_Alarm As Boolean
  Public ReachTemperatureAlarmTimer As New Timer
  Public BackgroundMixing As Boolean
  Public BackgroundMixingOn As New Timer
  Public BackgroundMixingOff As New Timer
  Public BackgroundMixingOnWas As Boolean
  Public BackgroundMixingOffWas As Boolean
  Public BTankBackgroundMixOn As Boolean
  Public CTankBackgroundMixOn As Boolean
  Public RunBTankPrepare As Boolean
  Public RunCTankPrepare As Boolean
  Public FlashFlag2 As Boolean
  Public CoolingWaterTimeOut As Boolean
  Public WaterUsed As Integer
  Public PowerUsed As Integer
  Public SteamUsed As Integer
  Public SteamUsedKgs As Integer
  Public PumpsKW As Integer
  Public PowerKWS As Integer
  Public PowerKWHrs As Integer
  Public FinalTemp As Integer
  Public FinalTempWas As Integer
  Public VesTemp As Integer
  Public StartTemp As Integer
  Public TempRise As Integer
  Public SteamNeeded As Integer
  Public VesVolume As Integer
  Public FlowmeterCheckTimer As New Timer
  Public MainPumpFeedBack As Boolean
  Public MainPumpStopTimer As New Timer
    Public ManualFill As Boolean
  Public ReelStopRequest As Boolean
  Public Dyelot_Weight As Double
  Public Dyelot_LiquidRatio As Integer
  Public Dyelot_TotalVolume As Integer
  Public Dyelot_ActualMainPumpSpeed As Integer
  Public Dyelot_ActualReel1Speed As Integer
  Public Dyelot_ActualReel2Speed As Integer
  Public Dyelot_ActualCycleTime1 As Integer
  Public Dyelot_ActualCycleTime2 As Integer
  Public Dyelot_ActualCycleTime3 As Integer
  Public Dyelot_ActualCycleTime4 As Integer
  Public Dyelot_ActualMainPumpSpeedUpdated As Boolean
  Public PumpControlOn As Boolean
  Public SimulateTotalWaterLiters As Integer
  Public SimulateFillWaterLiters As Integer
  Public SimulateFillLitersPerSecond As Integer
  Public SimulateFillCheckTimer As New Timer
  Public StartSimulateFillCheck As Boolean
  Public SimulateFillCheckFinish As Boolean
  Public SimulateFillCheckSeconds As Integer
  Public NormalAlarm As Boolean
  Public EntangleAlarm As Boolean
  Public PrepareAlarm As Boolean
  Public Check868UpdateTimer As New Timer
  Public BatchNumberAfterRinse As Integer
  Public SetSpeed As Boolean
  Public UserLoginOK As Boolean
  Public StartUserLogin As Boolean
  Public LoadOperator As String
  Public UnloadOperator As String
  Public StartLoadOperator As String
  Public StartUnloadOperator As String
  Public 布輪1纏車 As Boolean
  Public 布輪2纏車 As Boolean
  Public 布輪3纏車 As Boolean
  Public 布輪4纏車 As Boolean

  'C缸備藥狀態顯示
  Public Step1Dispensing As Boolean
  Public Step2Dispensing As Boolean
  Public Step4Dispensing As Boolean
  Public Step5Dispensing As Boolean
  Public Step6Dispensing As Boolean
  Public Step1Ready As Boolean
  Public Step2Ready As Boolean
  Public Step4Ready As Boolean
  Public Step5Ready As Boolean
  Public Step6Ready As Boolean


  '布頭訊號用的變數
  Public FabricCycleInput1Times As Integer
  Public FabricCycleInput2Times As Integer
  Public FabricCycleInput3Times As Integer
  Public FabricCycleInput4Times As Integer
  Public FabricCycle1FirstInput As Boolean
  Public FabricCycle2FirstInput As Boolean
  Public FabricCycle3FirstInput As Boolean
  Public FabricCycle4FirstInput As Boolean
  Public FabricCycleInput1Was As Boolean
  Public FabricCycleInput2Was As Boolean
  Public FabricCycleInput3Was As Boolean
  Public FabricCycleInput4Was As Boolean
  Public FabricCycleTime1 As Integer
  Public FabricCycleTime2 As Integer
  Public FabricCycleTime3 As Integer
  Public FabricCycleTime4 As Integer
  Public FabricCycleTimeCount1 As Integer
  Public FabricCycleTimeCount2 As Integer
  Public FabricCycleTimeCount3 As Integer
  Public FabricCycleTimeCount4 As Integer
  Public FabricCycleTimer1 As New TimerUp
  Public FabricCycleTimer2 As New TimerUp
  Public FabricCycleTimer3 As New TimerUp
  Public FabricCycleTimer4 As New TimerUp
  Public StartCycleTimeRecord As Boolean
  Public MaximumCycleTime As Integer
  Public MinimumCycleTime As Integer
  Public AverageCycleTime As Integer
  Public MaximumCycleTime1 As Integer
  Public MinimumCycleTime1 As Integer
  Public AverageCycleTime1 As Integer
  Public MaximumCycleTime2 As Integer
  Public MinimumCycleTime2 As Integer
  Public AverageCycleTime2 As Integer
  Public MaximumCycleTime3 As Integer
  Public MinimumCycleTime3 As Integer
  Public AverageCycleTime3 As Integer
  Public MaximumCycleTime4 As Integer
  Public MinimumCycleTime4 As Integer
  Public AverageCycleTime4 As Integer
  Public 左布輪纏車警報Timer As New Timer
  Public 右布輪纏車警報Timer As New Timer
  Public 主馬達停止警報Timer As New Timer

  '給mimic用的變數
  Public Mimic_MainTemp As Integer
  Public Mimic_BTankLevel As Integer
  Public Mimic_CTankLevel As Integer
  Public Mimic_MainLevel As Integer
  Public Mimic_TempControl As Integer
  Public Mimic_Timer1 As Boolean

  Public FirstScanDone As Boolean
  Public PumpStartRequest As Boolean
  Public PumpStopRequest As Boolean
  Public btankreadywas As Boolean
  Public ctankreadywas As Boolean
  Public slowflash As New Flasher
  Public ShowCallMessageWas As Boolean

  'machine idle time stuf
  Public ProgramStoppedTimer As New TimerUp                       'Program Stopped Timer
  Public ProgramStoppedTime As Integer
  Public CycleTime As Integer

  '停機延遲時間，時間到則停止染程
  Public ProgramIdleTimer As New Timer
  Public ProgramIdleTimerCounting As Boolean
  Public StopAllProgram As Boolean

  'Public DispenseStep As Integer
  'Public DispenseTank As Integer
  Public BTankHighDelay As New Timer
  Public MiddleLevelDelay As New Timer


  'pH控制
  Public PhFillLevel As Integer
  Public phtest As Double
  Public value1 As Double = 0
  Public testdata As Integer
  Public PhControlFlag As Boolean
  Public SetPointShow As Double           '顯示MIMIC 設定溫度
  Public MainTempShow As Double            '顯示MIMIC 實際溫度
  Public MathHacTimes As New TimerUp
  Public MathHacFlag As Boolean
  Public UseHacThisValue As Integer
  Public UseHacAllTotal, UseHacAllTotal2 As Integer
  Public pHWashRun As Boolean
    Public pHManualWashWas As Boolean
    Public pHWashFinish As Boolean

    Public PhToCPump As Boolean 'PH回流專用的加藥泵
  Public PhToAdd As Boolean 'PH回流專用的加藥閥
  Public PhToDrain As Boolean 'PH回流專用的排水閥

  Public test1, test2, test3, test4, test5, test6, test7, test8, test9, test10, test11, test12, _
test13, test14, test15, test17, test18, test19, test20, test21, test22, test23, test26, test27, test28, test31, test32, test33 As Double
  Public 補償狀態 As String
  Public test24, test16 As String
  Public test25, test29, test30, test35 As Boolean
  Public PhShowPic As Boolean
  Public PhShowData As String
  Public P11, P22 As Double
  Private DelayAddTime As New Timer
  Private DelayAddHac As Boolean
  Public PH再檢測 As Integer
  Public PH檢測_短時間內不在檢測 As Boolean
  Public PH再檢測時間 As New Timer
  Public PhCirRun As Boolean
  Public 已經確保120秒檢測完成, 短時間內重新執行, 是否縮短檢測時間 As Boolean
  Public 加酸時間 As New Timer
  Public 加酸次數 As Integer
  Public 補酸狀態分析 As Integer

  Public Sub New(ByVal parent As ACParent)
    Me.Parent = parent
    Select Case parent.CultureName
      Case "zh-TW" : Language = LanguageValue.ZhTW
      Case "zh-CN" : Language = LanguageValue.ZhCN
      Case Else : Language = LanguageValue.English
    End Select
  End Sub

  Public Sub StartUp() Implements ACControlCode.StartUp
    If True Then  ' Set to True to start in debug mode
      ' Parent.Mode = Mode.Debug
      Translations.Load()
      St.Load()
    End If
  End Sub

  Public Sub ShutDown() Implements ACControlCode.ShutDown
  End Sub

  Public Sub Run() Implements ACControlCode.Run

    '如果待機時檢查Update資料夾是否有檔案
    ' Dim myPath As String
    'myPath = Application.StartupPath
    'If Parent.IsProgramRunning Then Check868UpdateTimer.TimeRemaining = 10
    'If Not Parent.IsProgramRunning And Check868UpdateTimer.Finished Then
    ' Check868UpdateTimer.TimeRemaining = 10
    ' If My.Computer.FileSystem.FileExists("C:\LA868Update\HJDOS109.dll") Then
    ' System.Diagnostics.Process.Start(myPath + "\Update868.bat")
    ' End If
    ' End If


    '主缸水位異常警報

    Alarms.MainTankLowLevelError = (IO.HighLevel Or IO.MiddleLevel) And Not IO.LowLevel
    Alarms.MainTankMiddleLevelError = IO.HighLevel And Not IO.MiddleLevel
    Static FillOverTimeDelay As New DelayTimer
    Alarms.MainTankHighLevelError = FillOverTimeDelay.Run(IO.HotFill Or IO.ColdFill, Parameters.FillOverTime)

    '用手自動開關來切換速度控制
    IO.PumpControlOn = IO.SystemAuto

    '馬達控制
    PumpControl.Run()

    If Alarms.InsufficientSteam Then
      Delay = DelayReason.蒸氣不足
    ElseIf Alarms.CoolingNotEnough Then
      Delay = DelayReason.冷卻水不足
    ElseIf Command05.IsOn And Command05.WaitRunning.Finished And Parameters.WaitOverTime > 0 Then
      Delay = DelayReason.呼叫操作員超時
    ElseIf Command20.IsOn And Command20.WaitRunning.Finished And Parameters.WaitOverTime > 0 Then
      Delay = DelayReason.取樣超時
    ElseIf (Parameters.MainTankFillDelayTimeMinute > 0 And ((Command04.IsFillCold And Command04.IsFillHot And Command04.WaitRunning.Finished) Or
                (Command03.IsFillCold And Command03.IsFillHot And Command03.WaitRunning.Finished) Or
                (Command11.IsFillCold And Command11.IsFillHot And Command11.WaitRunning.Finished) Or
                (Command13.IsFillCold And Command13.IsFillHot And Command13.WaitRunning.Finished))) Then
      Delay = DelayReason.主缸進冷熱水超時
    ElseIf (Parameters.MainTankFillDelayTimeMinute > 0 And ((Command04.IsFillCold And Command04.WaitRunning.Finished) Or
                (Command03.IsFillCold And Command03.WaitRunning.Finished) Or
                (Command11.IsFillCold And Command11.WaitRunning.Finished) Or
                (Command13.IsFillCold And Command13.WaitRunning.Finished))) Then
      Delay = DelayReason.主缸進冷水超時
    ElseIf (Parameters.MainTankFillDelayTimeMinute > 0 And ((Command04.IsFillHot And Command04.WaitRunning.Finished) Or
                (Command03.IsFillHot And Command03.WaitRunning.Finished) Or
                (Command11.IsFillHot And Command11.WaitRunning.Finished) Or
                (Command13.IsFillHot And Command13.WaitRunning.Finished))) Then
      Delay = DelayReason.主缸進熱水超時
    ElseIf Command32.IsOn And Command32.WaitRunning.Finished And Parameters.WaitOverTime > 0 Then
      Delay = DelayReason.動力排水超時
    ElseIf ((Command54.IsOn And Command54.WaitRunning.Finished) Or (Command64.IsOn And Command64.WaitRunning.Finished)) And Parameters.WaitOverTime > 0 Then
      Delay = DelayReason.B缸備藥超時
    ElseIf ((Command55.IsOn And Command55.WaitRunning.Finished) Or (Command65.IsOn And Command65.WaitRunning.Finished)) And Parameters.WaitOverTime > 0 Then
      Delay = DelayReason.C缸備藥超時
    ElseIf ((Command56.IsOn And Command56.WaitRunning.Finished) Or (Command66.IsOn And Command66.WaitRunning.Finished)) And Parameters.WaitOverTime > 0 Then
      Delay = DelayReason.B缸加藥超時
    ElseIf ((Command57.IsOn And Command57.WaitRunning.Finished) Or (Command67.IsOn And Command67.WaitRunning.Finished)) And Parameters.WaitOverTime > 0 Then
      Delay = DelayReason.C缸加藥超時
    ElseIf Command59.IsOn And Command59.WaitRunning.Finished And Parameters.WaitOverTime > 0 Then
      Delay = DelayReason.等待染料超時
    Else
      Delay = DelayReason.正常運作
    End If

    '延遲的警報
    Alarms.SampleOverTime = (Delay = DelayReason.取樣超時)
    Alarms.CallOperatorOverTime = (Delay = DelayReason.呼叫操作員超時)
    Alarms.FillColdOverTime = (Delay = DelayReason.主缸進冷水超時)
    Alarms.FillHotOverTime = (Delay = DelayReason.主缸進熱水超時)
    Alarms.FillColdHotOverTime = (Delay = DelayReason.主缸進冷熱水超時)
    Alarms.PowerDrainOverTime = (Delay = DelayReason.動力排水超時)
    Alarms.WaitLA252OverTime = (Delay = DelayReason.等待染料超時)
    Alarms.BTankPrepareOverTime = (Delay = DelayReason.B缸備藥超時)
    Alarms.CTankPrepareOverTime = (Delay = DelayReason.C缸備藥超時)
    Alarms.BTankDosingOverTime = (Delay = DelayReason.B缸加藥超時)
    Alarms.CTankDosingOverTime = (Delay = DelayReason.C缸加藥超時)

    DelayAlarmOn = StopDelayAlarmTimer.Finished And (Alarms.SampleOverTime Or Alarms.CallOperatorOverTime Or Alarms.PowerDrainOverTime Or
                                   Alarms.WaitLA252OverTime Or Alarms.BTankPrepareOverTime Or Alarms.CTankPrepareOverTime Or Alarms.BTankDosingOverTime Or
                                   Alarms.CTankDosingOverTime)

    If DelayAlarmOn And IO.CallAck Then
      StopDelayAlarmTimer.TimeRemaining = Parameters.WaitOverTime * 60
    End If

    '紀錄主泵速度，帶布輪速度
    If HeatNow And Not Dyelot_ActualMainPumpSpeedUpdated And IO.MainTemperature > Parameters.UploadDataTemperature * 10 And IO.MainPumpSpeed > 0 And IO.Reel1Speed > 0 And IO.CycleTime1 > 0 Then
      Dyelot_ActualMainPumpSpeed = IO.MainPumpSpeed
      Dyelot_ActualReel1Speed = IO.Reel1Speed
      Dyelot_ActualReel2Speed = IO.Reel2Speed
      Dyelot_ActualCycleTime1 = FabricCycleTime1
      Dyelot_ActualCycleTime2 = FabricCycleTime2
      Dyelot_ActualCycleTime3 = FabricCycleTime3
      Dyelot_ActualCycleTime4 = FabricCycleTime4
      Dyelot_ActualMainPumpSpeedUpdated = True
    End If

    'machine idle time stuff
    If Not FirstScanDone Then ProgramStoppedTimer.Start()

    'Program running state changes
    Static WasProgramRunning As Boolean
    If Parent.IsProgramRunning Then            'A Program is running
      Static ProgramRunTimer As New TimerUp  ' program run timer
      CycleTime = ProgramRunTimer.TimeElapsed
      If Not WasProgramRunning Then     'A Program has just started
        ProgramStoppedTime = ProgramStoppedTimer.TimeElapsed
        ProgramStoppedTimer.Pause()
        ProgramRunTimer.Start()
      End If
    Else
      If WasProgramRunning Then         'A program has just finished
        ProgramStoppedTimer.Start()
        TemperatureControl.Cancel()
      End If
      CycleTime = 0                     'No Program is running
    End If
    WasProgramRunning = Parent.IsProgramRunning
    程式執行分鐘 = (CycleTime - 程式執行小時 * 3600) \ 60
    程式執行小時 = CycleTime \ 3600

    Dim halt = Parent.IsPaused ' Or IO_EStop_PB Or (Not ReadInputs_Succeeded)
    Dim NHalt = Not halt And IO.SystemAuto
    If TempControlFlag = True Then TemperatureControl.Run(IO.MainTemperature)
    TemperatureControl.CheckErrorsAndMakeAlarms(Me)
    Setpoint = TemperatureControl.TempSetpoint  ' keep a copy to show on graph

    If TempControlFlag = False Then TemperatureControl.Cancel()

    'On/Off昇降溫閥控制
    If (Command10.IsOn Or Command01.IsOn) And NHalt Then
      CoolValve = Not TemperatureControlTimer.Finished And Not CoolValveOpenTimer.Finished And CoolNow And (Parameters.CoolValveType = 0)
      HeatValve = Not TemperatureControlTimer.Finished And Not HeatValveOpenTimer.Finished And HeatNow And (Parameters.HeatValveType = 0)

      If CoolNow And Parameters.CoolValveType = 0 Then
        TemperatureControlTime = 6
        CoolValveOpenTime = Math.Min(Math.Max(((IO.MainTemperature - Setpoint) \ 5), 0), 6)
        If TemperatureControlTimer.Finished Then
          TemperatureControlTimer.TimeRemaining = (TemperatureControlTime - ((CoolValveOpenTime) \ 2))
          If (IO.MainTemperature - Setpoint) > 0 Then
            If CoolValveOpenTimer.Finished Then
              CoolValveOpenTimer.TimeRemainingMs = CoolValveOpenTime * 500
            End If
          End If
        End If
      ElseIf HeatNow And Parameters.HeatValveType = 0 Then
        TemperatureControlTime = 6
        HeatValveOpenTime = Math.Min(Math.Max((TemperatureControl.Output * 6 \ 1000), 0), 6)
        If TemperatureControlTimer.Finished Then
          TemperatureControlTimer.TimeRemaining = (TemperatureControlTime - ((HeatValveOpenTime) \ 2))
          If HeatValveOpenTimer.Finished Then
            HeatValveOpenTimer.TimeRemainingMs = HeatValveOpenTime * 500
          End If
        End If
      End If
    End If

    If Not NHalt Then TemperatureControl.pidPause()

    '升溫到達溫度警報
    If (ReachTemperature_Alarm And (IO.CallAck Or ReachTemperatureAlarmTimer.Finished)) Then
      ReachTemperature_Check = False
      ReachTemperature_Alarm = False
      Alarms.ReachTemperature = False
    End If
    If IO.MainTemperature >= CallTemperature And ReachTemperature_Check And Not ReachTemperature_Alarm Then
      ReachTemperature_Alarm = True
      Alarms.ReachTemperature = True
    End If
    If ReachTemperature_Alarm And ReachTemperatureAlarmTimer.Finished Then
      ReachTemperatureAlarmTimer.TimeRemaining = 60
    End If


    '升溫超時警報
    Alarms.HeatOverTime = HeatOverTime_Alarm

    'set up a flasher
    slowflash.Flash(FlashFlag, 500)

    'set up a flasher2
    slowflash.Flash(FlashFlag2, 1000)


    'run pressure out control when cooling to open up the pressure out when temp becomes ok
    'run the pressure control 
    'set the pressureouttemp to the parameter on boot
    If Not FirstScanDone Then
      'pressure out temp
      If Parameters.SetPressureOutTemp > 0 Then
        PressureOutTemp = Parameters.SetPressureOutTemp * 10
        PressureInTemp = Parameters.SetPressureOutTemp * 10
      Else
        PressureOutTemp = 850
        PressureInTemp = 850
      End If
      If PressureOutTemp >= 850 Then PressureOutTemp = 850
      If PressureInTemp >= 850 Then PressureInTemp = 850
    End If
        If TemperatureControl.IsCooling Or Parameters.PressureOutAlwaysOn = 1 Then
            PressureOut.Run()
        End If

        'Pressure in啟動條件'
        If TemperatureControl.IsHeating And ((IO.MainTemperature >= PressureInTemp) And (IO.MainTemperature < 1200)) Then
      PressureInFlag = True
    ElseIf TemperatureControl.IsCooling Or IO.MainTemperature > 1200 Or TemperatureControl.IsIdle Then
      PressureInFlag = False
    End If

    CallLA252.Run()

    '纏車
    Entanglement = IO.Entanglement1 Or IO.Entanglement2

    '主缸允許加藥
    MainTankSafeToAdd = (IO.MainTemperature < Parameters.SetAddSafetyTemp * 10)

    '主缸高溫保護
    MainTankSafe = (IO.MainTemperature < Parameters.SetSafetyTemp * 10)

    '主馬達運轉時數統計
    '   If IO.MainPumpFB And MainPumpRunTimer.Finished Then
    '   MainPumpRunTimer.TimeRemaining = 60
    '   MainPumpRunMins = MainPumpRunMins + 1
    '   If MainPumpRunMins > 60 Then
    '   MainPumpRunMins = 0
    '   Parameters.MainPumpRunTime = Parameters.MainPumpRunTime + 1
    '   End If
    '   End If
    'CycleTime過大時研判是沒有入布，將CycleTime的Timer取消
    If FabricCycleTimeCount1 > 600 Or Not IO.MainPumpFB Then
      FabricCycleTimeCount1 = 0
      FabricCycleTimer1.Stop()
    End If
    If FabricCycleTimeCount2 > 600 Or Not IO.MainPumpFB Then
      FabricCycleTimeCount2 = 0
      FabricCycleTimer2.Stop()
    End If
    If FabricCycleTimeCount3 > 600 Or Not IO.MainPumpFB Then
      FabricCycleTimeCount3 = 0
      FabricCycleTimer3.Stop()
    End If
    If FabricCycleTimeCount4 > 600 Or Not IO.MainPumpFB Then
      FabricCycleTimeCount4 = 0
      FabricCycleTimer4.Stop()
    End If

    If IO.Reel1ReversePB Then
      左布輪纏車警報Timer.TimeRemaining = 30
    End If
    If IO.Reel2ReversePB Then
      右布輪纏車警報Timer.TimeRemaining = 30
    End If
    If Not IO.MainPumpFB Then
      主馬達停止警報Timer.TimeRemaining = 30
    End If
    If Not 左布輪纏車警報Timer.Finished Then
      布輪1纏車 = True
      布輪2纏車 = True
    End If

    If FabricCycleTimeCount1 > Parameters.MaximumFabricCycleTime Then
      布輪1纏車 = True
    End If
    If FabricCycleTimeCount2 > Parameters.MaximumFabricCycleTime Then
      布輪2纏車 = True
    End If
    If 左布輪纏車警報Timer.Finished Then
      If FabricCycleTimeCount1 < Parameters.MaximumFabricCycleTime Then
        布輪1纏車 = False
      End If
      If FabricCycleTimeCount2 < Parameters.MaximumFabricCycleTime Then
        布輪2纏車 = False
      End If
    End If
    If Not 右布輪纏車警報Timer.Finished Then
      布輪3纏車 = True
      布輪4纏車 = True
    End If
    If FabricCycleTimeCount3 > Parameters.MaximumFabricCycleTime Then
      布輪3纏車 = True
    End If
    If FabricCycleTimeCount4 > Parameters.MaximumFabricCycleTime Then
      布輪4纏車 = True
    End If
    If 右布輪纏車警報Timer.Finished Then
      If FabricCycleTimeCount3 < Parameters.MaximumFabricCycleTime Then
        布輪3纏車 = False
      End If
      If FabricCycleTimeCount4 < Parameters.MaximumFabricCycleTime Then
        布輪4纏車 = False
      End If
    End If
    If Not 主馬達停止警報Timer.Finished Then
      IO.主馬達停止 = True
    Else
      IO.主馬達停止 = False
    End If


    'manual pump pushbuttons
    If IO.MainPumpOnPB Then
      PumpOn = True
    End If
    If IO.MainPumpOffPB Then
      PumpOn = False
    End If

    '藥缸低水位延遲保護
    Static BTankDelay As New DelayTimer
    Static CTankDelay As New DelayTimer
    BTankAddDelay = BTankDelay.Run(Not BTankLowLevel, Parameters.AddFinishDelay)
    CTankAddDelay = CTankDelay.Run(Not CTankLowLevel, Parameters.AddFinishDelay)
    '升溫時開熱交換器排水閥，熱交換器排水延遲時間到達則改開排冷凝水閥
    Static HxDrainDelay As New DelayTimer
    Static HxPressureInDelay As New DelayTimer

    ' Make run and halt(=pause) push-buttons work
    Parent.PressButtons(IO.RemoteRun, IO.RemoteHalt, StopAllProgram, False, False)
    Parent.PressForwardBackward(IO.Down, IO.Up)

    Alarms.TerminalError = RemoteDisplayError
    Alarms.PlcError = IO.PlcFault And Parent.Mode <> Mode.Debug  ' no plc error in debug mode
    'Alarms.Plc2Error = IO.Plc2Fault And Parent.Mode <> Mode.Debug
    '  Alarms.Pt1Open = (io.MainTemperature > Parameters.pt1Highlimit)
    '  Alarms.Pt1Short = (io.MainTemperature < Parameters.pt1lowlimit)
    Alarms.InsufficientSteam = TempMore3 And HeatNow
    Alarms.CoolingNotEnough = TempMore3 And CoolNow
    Static delay4 As New DelayTimer
    Alarms.MainPumpError = (((PumpOn Or HeatNow Or CoolNow) And IO.SystemAuto) Or ((IO.HeatPB Or IO.CoolPB) And Not IO.SystemAuto)) And Not IO.MainPumpFB

    Static delay2 As New DelayTimer
    Alarms.MainPumpOverload = delay2.Run(IO.MainPumpOverload, 3)
    Alarms.AddMotorOverload = IO.AddCPumpOverload Or IO.BPumpOverload
    ManualOperation = Not IO.SystemAuto

    Alarms.MainElectricFanError = IO.FanError
    Alarms.FabricStop = IO.Entanglement1 Or IO.Entanglement2 Or IO.布輪1纏車 Or IO.布輪2纏車 Or IO.Reel1OverLoad Or IO.Reel2OverLoad Or IO.Reel3OverLoad Or IO.Reel4OverLoad Or 布輪1纏車 Or 布輪2纏車 Or 布輪3纏車 Or 布輪4纏車
    Alarms.BTankWaitReady = Command54.IsSlow Or Command64.IsSlow Or Command51.IsWaitingForPrepare
    Alarms.CTankWaitReady = Command55.IsSlow Or Command65.IsSlow Or Command52.IsWaitingForPrepare
    Alarms.BTankNotEmpty = Command58.IsNotReady
    Alarms.ManualTempControlMainPumpNotOn = (Not IO.SystemAuto) And (IO.HeatPB Or IO.CoolPB) And (Not IO.MainPumpFB)
    Alarms.ManualFillEnd = (Not IO.SystemAuto) And IO.FillLevelControlPB And IO.MiddleLevel
    Alarms.ChemicalDispenseError = (IO.SystemAuto And ChemicalState = (EDispenseState.Manual Or EDispenseState.Error) And Not TankCReady)
    Alarms.Pt1Open = IO.MainTemperature < 100
    Alarms.Pt1Short = IO.MainTemperature > 1400
    'Alarms.STankNotReady = NHalt And (Command56.IsNotReady Or Command57.IsNotReady Or Command66.IsNotReady Or Command67.IsNotReady)
    Alarms.TemperatureHigh = NHalt And TemperatureControl.Message_TemperatureHigh And HeatNow
    Alarms.TemperatureLow = NHalt And TemperatureControl.Message_TemperatureLow

    '單藥缸馬達時，BC缸同時手動攪拌或是加藥時要有警報
    Alarms.BTankMixing = IO.BTankMixCir And (IO.CTankMixCirPB Or IO.CAllIn) And Not IO.SystemAuto And IO.BTankAddPump And Parameters.BCTank100TwoPump1OnePump = 1
    Alarms.BTankTransfering = IO.BTankAddition And Not IO.BTankMixCir And (IO.CTankMixCirPB Or IO.CAllIn) And Not IO.SystemAuto And IO.BTankAddPump And Parameters.BCTank100TwoPump1OnePump = 1
    Alarms.CTankMixing = IO.CTankMixCir And (IO.BTankMixCirPB Or IO.BAllIn) And Not IO.SystemAuto And IO.BTankAddPump And Parameters.BCTank100TwoPump1OnePump = 1
    Alarms.CTankTransfering = IO.CTankAdditionCIn And Not IO.CTankMixCir And (IO.BTankMixCirPB Or IO.BAllIn) And Not IO.SystemAuto And IO.BTankAddPump And Parameters.BCTank100TwoPump1OnePump = 1
    BAddStop = IO.BAddStop
    CAddStop = IO.CAddStop

    '藥缸手動排水警報
    Alarms.BTankManualDrain = IO.BDrainPB And (Command54.IsOn Or Command64.IsOn)
    Alarms.CTankManualDrain = IO.CDrainPB And (Command55.IsOn Or Command65.IsOn)

    'calculate side tank level 
    BTankLowLevel = (IO.TankBLevel >= 50)
    BTankMiddleLevel = (IO.TankBLevel >= 500)
    BTankHighLevel = (IO.TankBLevel >= 1000)
    CTankLowLevel = IO.CTankLow Or (IO.TankCLevel >= 50)
    CTankMiddleLevel = IO.TankCLevel >= 500
    CTankHighLevel = IO.CTankHigh Or (IO.TankCLevel >= 1000)

    'tank ready pushbuttons
    If IO.BTankReady And BTankLowLevel Then
      TankBReady = True
    End If
    If Not BTankLowLevel Then
      TankBReady = False
    End If
    If IO.CTankReady And CTankLowLevel Then
      TankCReady = True
    End If
    If Not CTankLowLevel Then
      TankCReady = False
    End If

    'B缸加藥中
    BTankDosing = Command56.IsOn Or Command66.IsOn Or Command51.IsOn

    'B缸高水位延遲
    If RinseFillbyBtank And BTankHighLevel Then
      BTankHighDelay.TimeRemaining = 10
    End If

    '溢流洗中水位延遲
    If (Command16.IsOn Or Command17.IsOn Or Command18.IsOn) And (IO.MiddleLevel Or IO.HighLevel) Then
      MiddleLevelDelay.TimeRemaining = 20
    End If

    '呼叫LA-252
    CallLA252.Run()

    '溢流水洗時由B缸進水
    RinseFillbyBtank = NHalt And (Command11.IsFillbyBtank Or Command17.IsFillbyBtank)
    RinseBTankDrain = NHalt And (Command11.IsDraining Or Command17.IsDraining)

    ManualDose.Run()

    'digital outputs 
    IO.Heat = NHalt And TemperatureControl.IsHeating And IO.MainPumpFB And ((HeatNow And Parameters.HeatValveType = 1) Or HeatValve)
        IO.Cool = NHalt And (TemperatureControl.IsCooling And IO.MainPumpFB And (CoolValve Or (CoolNow And Parameters.CoolValveType = 1))) Or
                                                 Command15.IsAirCoolRinse Or Command24.IsAirCoolRinse Or (Not IO.SystemAuto And IO.CoolingOverFlowPB)

        Static CondenserDrainDelay As New DelayTimer

        If IO.Heat Then
            CondenserDrainWas = True
        ElseIf IO.Cool Or IO.Drain Or IO.HotDrain Or IO.CoolDrain Or CondenserDrainDelay.Run(Not IO.Heat, 300) Then
            CondenserDrainWas = False
        End If
        IO.CondenserDrain = NHalt And HxDrainDelay.Run(CondenserDrainWas, Parameters.Condensation) And Not IO.FillReCycleWater
    IO.HxDrain = Not (IO.CondenserDrain Or IO.CoolDrain Or IO.Cool Or IO.FillReCycleWater)
    IO.CoolDrain = (CoolNow Or (Not IO.FillReCycleWater And IO.Cool))
    IO.ExchangerPressureIn = HeatNow And IO.HxDrain And Not HxPressureInDelay.Run(HeatNow, Parameters.ExchangerPressureInDelay)

    '主缸溫度在120度以下，90度以上開加壓
    Static PressureInDelay As New DelayTimer
    IO.PressureIn = PressureInDelay.Run(NHalt And Not IO.PressureSw And PressureInFlag And Parameters.PressureInWhenHeating = 1, 5)
    IO.PressureOut = ((PressureOut.IsDepressurising Or PressureOut.IsDepressurised) And (TemperatureControl.IsCooling Or Parameters.PressureOutAlwaysOn = 1)) Or ((Command56.IsOn Or Command57.IsOn Or Command51.IsOn Or Command52.IsOn Or Command66.IsOn Or Command67.IsOn) And MainTankSafeToAdd) Or
                     ((Command04.IsOn Or Command03.IsOn Or Command11.IsOn Or Command12.IsOn Or Command13.IsOn Or Command14.IsOn Or Command16.IsOn Or Command32.IsOn Or Command20.IsOn) Or (TemperatureControl.IsHeating And Parameters.PressureOutWhenHeating = 1) And Not PressureInFlag)
    IO.Overflow = (NHalt And (Command11.IsDrainingToLowLevel Or Command12.IsOverFlow Or Command13.IsOverflow Or Command14.IsOverflowDrain Or Command15.IsOverFlow Or Command16.IsOverFlowDrain Or Command17.IsOn Or Command18.IsOverFlowDrain Or Command26.IsDrain Or Command32.IsOverflow Or Command15.IsAirCoolRinse Or Command24.IsAirCoolRinse)) Or
                  (Not IO.SystemAuto And (IO.PowerDrainPB Or IO.PwoerHotDrainPB)) Or (Not IO.SystemAuto And (IO.OverFlowPB Or IO.CoolingOverFlowPB))
    IO.ColdFill = NHalt And (Command03.IsFillCold Or Command04.IsFillCold Or Command11.IsFillCold Or Command12.IsFillCold Or Command13.IsFillCold Or Command15.IsFillCold Or Command26.IsFilling Or CoolingWaterTimeOut Or Alarms.FillColdHotOverTime) _
                            And Not IO.HighLevel
    IO.HotFill = NHalt And (Command03.IsFillHot Or Command04.IsFillHot Or Command11.IsFillHot Or Command12.IsFillHot Or Command13.IsFillHot Or Command15.IsFillHot Or CoolingWaterTimeOut Or Alarms.FillColdOverTime) And Not IO.HighLevel

    IO.FillReCycleWater = Not IO.HighLevel And (NHalt And Command11.IsCoolFill Or Command12.IsCoolFill Or Command13.IsCoolFill Or Command16.IsCoolFill Or Command18.IsCoolFill Or Command15.IsAirCoolRinse Or Command24.IsAirCoolRinse) Or _
                          (Not IO.SystemAuto And ((IO.OverFlowPB And Parameters.ReCycleWater = 1 And IO.CoolPB) Or IO.CoolingOverFlowPB))

    Static CoolingWaterTimeOutTimer As New DelayTimer
    CoolingWaterTimeOut = CoolingWaterTimeOutTimer.Run(IO.FillReCycleWater, Parameters.CoolingWaterTimeOut * 60)


    IO.HotDrain = NHalt And (Command11.IsDrainingToLowLevel Or Command11.IsDrainingEmpty Or Command13.IsDrainToMiddleLevel Or _
                             Command14.IsHotDrain Or Command32.IsHotDrain Or (Parameters.SetPowerDrain = 1 And Command32.IsPowerDrain Or Command32.IsPowerHotDrain))
    IO.Drain = NHalt And (Command11.IsDrainingToLowLevel Or Command11.IsDrainingEmpty Or Command13.IsDrainToMiddleLevel Or
                          Command14.IsColdDrain Or Command26.IsDrain Or Command32.IsDrain Or (Parameters.SetPowerDrain = 1 And (Command32.IsPowerDrain Or Command32.IsPowerHotDrain)))
    IO.PumpOn = PumpStartRequest And IO.SystemAuto
    IO.PumpOff = PumpStopRequest
    IO.ReelStop = ReelStopRequest
    '纏車警報為閃爍，一般警報為連續
    EntangleAlarm = delay4.Run(Alarms.MainPumpError Or IO.Entanglement1 Or IO.Entanglement2, 5) And FlashFlag
    PrepareAlarm = (Command54.IsSlow Or Command55.IsSlow Or Command64.IsSlow Or
                  Command65.IsSlow Or Command59.IsSlow Or Command58.IsNotReady Or Command56.IsWaitingForPrepare Or Command66.IsWaitingForPrepare Or
                  Command57.IsWaitingForPrepare Or Command67.IsWaitingForPrepare) And NHalt And FlashFlag2
    IO.ErrorLamp = EntangleAlarm Or PrepareAlarm
    IO.CallLamp = NHalt And (Command05.IsCall Or Command19.IsCall Or Command20.IsCall Or Command31.IsCall Or Command33.IsCall Or Command51.IsCallOperator Or
                  Command52.IsCallOperator Or Command54.IsSlow Or Command55.IsSlow Or Command64.IsSlow Or
                  Command65.IsSlow Or Command59.IsSlow Or Command58.IsNotReady Or Command56.IsWaitingForPrepare Or Command66.IsWaitingForPrepare Or
                  Command57.IsWaitingForPrepare Or Command67.IsWaitingForPrepare Or ReachTemperature_Alarm Or HeatOverTime_Alarm Or DelayAlarmOn Or
                  Alarms.BTankManualDrain Or Alarms.CTankManualDrain)
    IO.AirCool = NHalt And (Command15.IsAirCoolRinse Or Command24.IsAirCoolRinse)

    IO.BTankDrain = NHalt And (Command56.IsDraining Or Command60.IsDraining Or Command66.IsDraining Or RinseBTankDrain)
    IO.BDosing = (Parameters.BCTank100TwoPump1OnePump = 0 And BDosingOn) Or _
                 (Parameters.BCTank100TwoPump1OnePump = 1 And (BDosingOn Or CDosingOn))
    IO.BTankAddPump = (Parameters.BCTank100TwoPump1OnePump = 0 And BPumpOn) Or _
                          (Parameters.BCTank100TwoPump1OnePump = 1 And (BPumpOn Or CPumpOn))
    IO.BTankMixCir = BCirMixOn Or IO.BTankFillCold Or IO.BTankFillCirc
    IO.BTankFillCold = NHalt And (Command25.IsFilling Or Command54.IsFillingFresh Or Command64.IsFillingFresh Or Command56.IsRinsing Or Command66.IsRinsing Or Command59.IsFillingFresh Or
                       (RinseFillbyBtank And BTankHighDelay.Finished))
    IO.BTankFillCirc = NHalt And (Command54.IsFillingCirc Or Command64.IsFillingCirc Or Command51.IsFillingCirc Or Command59.IsFillingCirc Or
                                 (RinseFillbyBtank And BTankHighDelay.Finished And Parameters.BTankCirWhenRinsefromB = 1) Or (Command51.IsDilute And Not BTankMiddleLevel) Or Command56.IsFillCirc Or Command66.IsFillCirc)
    IO.BTankAddition = (Parameters.BCTank100TwoPump1OnePump = 0 And BAllinOn) Or _
                       (Parameters.BCTank100TwoPump1OnePump = 1 And BInjOn)
    IO.BTankOkLamp = ((Command54.IsSlow And FlashFlag) Or (Command64.IsSlow And FlashFlag) Or (Command51.IsWaitingForPrepare And FlashFlag) _
                      Or (Command59.IsSlow And FlashFlag) Or (Command56.IsWaitingForPrepare And FlashFlag) Or (Command66.IsWaitingForPrepare And FlashFlag) Or _
                      (Command56.IsCallReCycleCheckEnd And FlashFlag) Or (Command66.IsCallReCycleCheckEnd And FlashFlag) Or TankBReady) And BTankLowLevel
    IO.PowerHotDrain = NHalt And Command32.IsPowerHotDrain And Parameters.SetPowerDrain = 0
    IO.B1AddLamp = ManualDose.AddType = 1 And ManualDose.Tank = 5
    IO.B2AddLamp = ManualDose.AddType = 2 And ManualDose.Tank = 5
    IO.B3AddLamp = ManualDose.AddType = 3 And ManualDose.Tank = 5
    IO.B4AddLamp = ManualDose.AddType = 4 And ManualDose.Tank = 5
    IO.B5AddLamp = ManualDose.AddType = 5 And ManualDose.Tank = 5
    'IO.B6AddLamp = NHalt
    IO.PowerDrain = NHalt And Command32.IsPowerDrain And Parameters.SetPowerDrain = 0
    IO.CTankDrain = NHalt And (Command57.IsDraining Or Command60.IsDraining Or Command67.IsDraining Or Command61.IsDrain)
    IO.CDosingBIn = (Parameters.BCTank100TwoPump1OnePump = 0 And CDosingOn) Or _
                    (Parameters.BCTank100TwoPump1OnePump = 1 And (BAllinOn Or CAllinOn))
    IO.CTankAddPump = Parameters.BCTank100TwoPump1OnePump = 0 And CPumpOn
    IO.CTankMixCir = CCirMixOn Or IO.CTankFillCold Or IO.CTankFillCirc
    IO.CTankFillCold = NHalt And (Command55.IsFillingFresh Or Command65.IsFillingFresh Or Command57.IsRinsing Or Command67.IsRinsing Or CTankPrepare.IsFillingFresh)
    IO.CTankFillCirc = NHalt And (Command55.IsFillingCirc Or Command65.IsFillingCirc Or Command52.IsFillingCirc Or Command57.IsFillCirc Or
                                  Command61.IsFillingCirc Or Command67.IsFillCirc Or CTankPrepare.IsFillingCirc Or (Command52.IsDilute And Not CTankMiddleLevel))
    IO.CTankAdditionCIn = (Parameters.BCTank100TwoPump1OnePump = 0 And CAllinOn) Or _
                              (Parameters.BCTank100TwoPump1OnePump = 1 And CInjOn)
    IO.CTankOkLamp = (((Command55.IsSlow And FlashFlag) Or (Command65.IsSlow And FlashFlag) Or (Command52.IsWaitingForPrepare And FlashFlag) _
                     Or (Command57.IsWaitingForPrepare And FlashFlag) Or (Command67.IsWaitingForPrepare And FlashFlag) Or _
                     (Command67.IsCallReCycleCheckEnd And FlashFlag) Or TankCReady) And CTankLowLevel) Or (RunCTankPrepare And FlashFlag)
    IO.C1AddLamp = ManualDose.AddType = 1 And ManualDose.Tank = 4
    IO.C2AddLamp = ManualDose.AddType = 2 And ManualDose.Tank = 4
    IO.C3AddLamp = ManualDose.AddType = 3 And ManualDose.Tank = 4
    IO.C4AddLamp = ManualDose.AddType = 4 And ManualDose.Tank = 4
    IO.C5AddLamp = ManualDose.AddType = 5 And ManualDose.Tank = 4
    'IO.C6AddLamp = NHalt
    ' IO.SoftFill = NHalt
    'B TANK outputs +++++++++++++++++++++++++++++++
    BDosingOn = (NHalt And (Command56.IsDosing Or Command66.IsDosing)) Or (ManualDose.Tank = 5 And ManualDose.IsDosing) Or
                ((Not IO.SideTankAuto And IO.BAllIn And (Parameters.BCTank100TwoPump1OnePump = 0 Or Not CInjOn)) And Not BTankAddDelay)
    BAllinOn = (NHalt And (Command25.IsAdd Or Command51.IsTransfer Or Command56.IsTransfer Or Command66.IsTransfer Or Command51.IsDilute Or RinseFillbyBtank Or Command56.IsFillCirc Or Command66.IsFillCirc)) Or
                      (ManualDose.Tank = 5 And ManualDose.IsTransfer) Or ((Not IO.SideTankAuto And IO.BAllIn And (Parameters.BCTank100TwoPump1OnePump = 0 Or Not CInjOn)) And Not BTankAddDelay)
    BCirMixOn = (NHalt And (Command54.IsMixingForTime Or Command56.IsMixing Or Command66.IsMixing Or Command64.IsMixingForTime Or Command59.IsMixingForTime Or Command51.IsMixing Or CallLA252.IsReady Or (BackgroundMixingOnWas And BTankBackgroundMixOn))) Or _
                (Not IO.SideTankAuto And IO.BTankMixCirPB And (Parameters.BCTank100TwoPump1OnePump = 0 Or Not CInjOn))
    BInjOn = BDosingOn Or BAllinOn Or BCirMixOn And (Parameters.BCTank100TwoPump1OnePump = 0 Or Not CInjOn)
    BPumpOn = (NHalt And (Command25.IsAdd Or Command54.IsMixingForTime Or Command64.IsMixingForTime Or Command51.IsTransfer Or Command59.IsMixingForTime Or
                      Command51.IsMixing Or Command56.IsTransferPump Or Command66.IsTransferPump Or CallLA252.IsReady Or RinseFillbyBtank Or Command56.IsFillCirc Or Command66.IsFillCirc)) Or
                      (ManualDose.Tank = 5 And ManualDose.IsTransferPump) Or (Not IO.SideTankAuto And (IO.BAllIn Or IO.BTankMixCirPB) And Not BTankAddDelay)
    'C TANK outputs +++++++++++++++++++++++++++++++
    CDosingOn = (NHalt And (Command57.IsDosing Or Command67.IsDosing)) Or (ManualDose.Tank = 4 And ManualDose.IsDosing) Or _
                ((Not IO.SideTankAuto And IO.CAllIn And (Parameters.BCTank100TwoPump1OnePump = 0 Or Not BInjOn)) And Not CTankAddDelay)
    CAllinOn = NHalt And (Command52.IsTransfer Or Command57.IsTransfer Or Command67.IsTransfer Or (ManualDose.Tank = 4 And ManualDose.IsTransfer) Or _
                         (Command57.IsDosing And Command57.AddTime < 300) Or Command52.IsDilute Or Command67.IsFillCirc) Or _
                         ((Not IO.SideTankAuto And IO.CAllIn And (Parameters.BCTank100TwoPump1OnePump = 0 Or Not BInjOn)) And Not CTankAddDelay)
    CCirMixOn = NHalt And (Command55.IsMixingForTime Or Command65.IsMixingForTime Or Command52.IsMixing Or CTankPrepare.IsMixingForTime Or (BackgroundMixingOnWas And CTankBackgroundMixOn)) Or
                (Not IO.SideTankAuto And IO.CTankMixCirPB And (Parameters.BCTank100TwoPump1OnePump = 0 Or Not BInjOn))
    CInjOn = CDosingOn Or CAllinOn Or CCirMixOn
    CPumpOn = (NHalt And (Command55.IsMixingForTime Or Command65.IsMixingForTime Or Command52.IsTransfer _
                                 Or Command57.IsTransferPump Or Command67.IsTransferPump Or Command52.IsMixing Or Command67.IsFillCirc Or CTankPrepare.IsMixingForTime)) Or
                                 (ManualDose.Tank = 4 And ManualDose.IsTransferPump) Or (Not IO.SideTankAuto And (IO.CAllIn Or IO.CTankMixCirPB) And Not CTankAddDelay)

    '鎖住藥缸的手動開關
    IO.SideTankAuto = SideTankAuto
    'SideTankAuto = (Parent.IsProgramRunning And IO.SystemAuto)
    SideTankAuto = Parent.IsProgramRunning And IO.SystemAuto And ( _
                                    Command51.IsOn Or _
                                    Command52.IsOn Or _
                                    Command54.IsOn Or _
                                    Command55.IsOn Or _
                                    Command56.IsOn Or _
                                    Command57.IsOn Or _
                                    Command64.IsOn Or _
                                    Command65.IsOn Or _
                                    Command66.IsOn Or _
                                    Command67.IsOn)


    'analog outputs +++++++++++++++++++++++++++++++
    'Analog Heat/Cool
    If NHalt And ((HeatNow And Parameters.HeatValveType = 1) Or (CoolNow And Parameters.CoolValveType = 1)) And IO.MainPumpFB And TempControlFlag Then
      If TemperatureControl.IsMaxGradient And ((HeatNow And TemperatureControl.TempFinalTemp - IO.MainTemperature > 100) Or (CoolNow And IO.MainTemperature - TemperatureControl.TempFinalTemp > 50)) Then
        IO.TemperatureControl = 1000
      ElseIf Command01.IsHolding And ((HeatNow And IO.MainTemperature - TemperatureControl.TempFinalTemp > 20) Or (CoolNow And TemperatureControl.TempFinalTemp - IO.MainTemperature > 20)) Then
        IO.TemperatureControl = 0
      ElseIf TemperatureControlTimer.Finished Then
        TemperatureControlTimer.TimeRemaining = 10
        IO.TemperatureControl = CType(TemperatureControl.Output, Short)
      End If
    ElseIf NHalt And IO.MainPumpFB And (Command03.IsCoolFill Or Command04.IsCoolFill Or Command15.IsAirCoolRinse) Then
      IO.TemperatureControl = 1000
    ElseIf IO.FillReCycleWater And IO.MainTemperature > 700 Then
      IO.TemperatureControl = CShort(MinMax(Parameters.CoolingValveOutput1, 0, 1000))
    ElseIf IO.FillReCycleWater And IO.MainTemperature < 700 Then
      IO.TemperatureControl = CShort(MinMax(Parameters.CoolingValveOutput2, 0, 1000))
    Else
      IO.TemperatureControl = 0
    End If
    'pump speed
    'If PumpOn Or PumpStartRequest Or IO.MainPumpFB Then
    ' IO.PumpSpeedControl = CType((PumpSpeed * 255) / 1800, Short)
    ' IO.MainPumpSpeedControl = CType((PumpSpeed * 255) / 1800, Short)
    ' IO.Reel1SpeedControl = CType((Reel1Speed * 255) / 1800, Short)
    ' IO.Reel2SpeedControl = CType((Reel2Speed * 255) / 1800, Short)
    ' Else
    ' IO.PumpSpeedControl = 0
    ' IO.MainPumpSpeedControl = 0
    ' IO.Reel1SpeedControl = 0
    ' IO.Reel2SpeedControl = 0
    ' End If
    'b dosing pump output
    'if 0 inverter on dosing pump run at full speed. if 1 just an on off pump
    If Parameters.DosingKind0Pump1AO2DO = 0 Then
      If NHalt And (Command56.IsTransfer Or Command66.IsTransfer Or Command54.IsMixingForTime Or _
                    Command64.IsMixingForTime Or Command51.IsMixing Or Command51.IsTransfer Or _
                    Command59.IsMixingForTime Or CallLA252.IsReady Or RinseFillbyBtank Or _
                    Command56.IsMixing Or Command66.IsMixing) Then
        IO.BDosingOutput = CShort(Parameters.SideTankManualPumpSpeed)
      ElseIf NHalt And (Command56.IsFillCirc Or Command66.IsFillCirc) Then
        IO.BDosingOutput = 200
      ElseIf NHalt And Command56.IsOn Then
        IO.BDosingOutput = CShort(Maximum(Command56.DoseOutput, Parameters.SideTankManualPumpSpeed))
      ElseIf NHalt And Command66.IsOn Then
        IO.BDosingOutput = CShort(Maximum(Command66.DoseOutput, Parameters.SideTankManualPumpSpeed))
      ElseIf (ManualDose.Tank = 5 And ManualDose.IsOn) Then
        IO.BDosingOutput = CShort(ManualDose.DoseOutput)
      ElseIf Not IO.SideTankAuto And ((IO.BAllIn Or IO.BTankMixCirPB) Or ((IO.CAllIn Or IO.CTankMixCirPB) And Parameters.BCTank100TwoPump1OnePump = 1)) And Not BTankAddDelay Then
        IO.BDosingOutput = CShort(Parameters.SideTankManualPumpSpeed)
      Else
        IO.BDosingOutput = 0
      End If
    Else
      IO.BDosingOutput = 0
    End If
    'c dosing pump output
    'if 0 inverter on dosing pump run at full speed. if 1 just an on off pump
    If Parameters.DosingKind0Pump1AO2DO = 0 Then
      If Parameters.BCTank100TwoPump1OnePump = 1 Then
        If NHalt And (Command55.IsMixingForTime Or Command65.IsMixingForTime Or Command52.IsMixing Or Command52.IsTransfer Or Command52.IsDilute Or _
                      Command57.IsTransfer Or Command67.IsTransfer) Then
          IO.BDosingOutput = 1000
        ElseIf NHalt And (Command67.IsFillCirc) Then
          IO.BDosingOutput = 200
        ElseIf NHalt And Command57.IsOn Then
          IO.BDosingOutput = CShort(Maximum(Command57.DoseOutput, Parameters.SideTankManualPumpSpeed))
        ElseIf NHalt And Command67.IsOn Then
          IO.BDosingOutput = CShort(Maximum(Command67.DoseOutput, Parameters.SideTankManualPumpSpeed))
        ElseIf (ManualDose.Tank = 4 And ManualDose.IsOn) Then
          IO.BDosingOutput = CShort(Maximum(ManualDose.DoseOutput, Parameters.SideTankManualPumpSpeed))
        ElseIf Not IO.SideTankAuto And ((IO.CAllIn Or IO.CTankMixCirPB) And Parameters.BCTank100TwoPump1OnePump = 1) And Not CTankAddDelay Then
          IO.BDosingOutput = CShort(Parameters.SideTankManualPumpSpeed)
        End If
      Else
        If NHalt And (Command55.IsMixingForTime Or Command65.IsMixingForTime Or Command52.IsMixing Or Command52.IsTransfer Or Command52.IsDilute Or _
              Command57.IsTransfer Or Command67.IsTransfer) Then
          IO.CDosingOutput = 700
        ElseIf NHalt And (Command67.IsFillCirc) Then
          IO.CDosingOutput = 200
        ElseIf NHalt And Command57.IsOn Then
          IO.CDosingOutput = CShort(Maximum(Command57.DoseOutput, Parameters.SideTankManualPumpSpeed))
        ElseIf NHalt And Command67.IsOn Then
          IO.CDosingOutput = CShort(Maximum(Command67.DoseOutput, Parameters.SideTankManualPumpSpeed))
        ElseIf (ManualDose.Tank = 4 And ManualDose.IsOn) Then
          IO.CDosingOutput = CShort(Maximum(ManualDose.DoseOutput, Parameters.SideTankManualPumpSpeed))
        ElseIf Not IO.SideTankAuto And ((IO.CAllIn Or IO.CTankMixCirPB) And Parameters.BCTank100TwoPump1OnePump = 0) And Not CTankAddDelay Then
          IO.CDosingOutput = CShort(Parameters.SideTankManualPumpSpeed)
        Else
          IO.CDosingOutput = 0
        End If
      End If
    Else
      IO.CDosingOutput = 0
    End If


    '參數轉換至IO
    IO.HxDelay = Parameters.Condensation
    IO.DrainDelay = Parameters.DrainDelay
    IO.MainSafetyTemp = Parameters.SetSafetyTemp * 10
    IO.STankAddDelay = Parameters.AddFinishDelay
    IO.PowerDrainDelay = Parameters.PowerDrainDelay
    IO.ReelDelay = Parameters.ReelStartDelayTime
    IO.ExchangerPressureInDelay = Parameters.ExchangerPressureInDelay

    '變數轉換到mimic
    Mimic_MainTemp = IO.MainTemperature \ 10
    Mimic_BTankLevel = IO.TankBLevel \ 10
    Mimic_CTankLevel = IO.TankCLevel \ 10
    If IO.HighLevel Then
      Mimic_MainLevel = 100
    ElseIf IO.MiddleLevel Then
      Mimic_MainLevel = 50
    ElseIf IO.LowLevel Then
      Mimic_MainLevel = 20
    End If
    Mimic_TempControl = IO.TemperatureControl \ 10
    Mimic_Timer1 = IO.Drain

    'DownLoad Parameter to LB60B
    FirstScanDone = True

    '計算總浴量
    TotalVolume = TotalWeight * LiquidRatio

    'C缸備藥狀態
    If ChemicalState = 202 Then
      If ChemicalCallOff = 1 Then
        Step1Dispensing = True
      ElseIf ChemicalCallOff = 2 Then
        Step2Dispensing = True
      ElseIf ChemicalCallOff = 4 Then
        Step4Dispensing = True
      ElseIf ChemicalCallOff = 5 Then
        Step5Dispensing = True
      ElseIf ChemicalCallOff = 6 Then
        Step6Dispensing = True
      End If
    End If
    If ChemicalState = 101 Or ChemicalState = 302 Or ChemicalState = 301 Then
      If Step1Dispensing Then
        Step1Ready = True
        Step1Dispensing = False
      ElseIf Step2Dispensing Then
        Step2Ready = True
        Step2Dispensing = False
      ElseIf Step4Dispensing Then
        Step4Ready = True
        Step4Dispensing = False
      ElseIf Step5Dispensing Then
        Step5Ready = True
        Step5Dispensing = False
      ElseIf Step6Dispensing Then
        Step6Ready = True
        Step6Dispensing = False
      End If
    End If

    '計算布速

    If IO.MainPumpFB Then
      MainPumpFeedBack = True
      MainPumpStopTimer.TimeRemaining = 5
    ElseIf MainPumpStopTimer.Finished Then
      MainPumpFeedBack = False
    End If

    If IO.FabricCycleInput1 And FabricCycleTimeCount1 > 10 Then
      If FabricCycleTimeCount1 <= 999 Then
        FabricCycleTime1 = FabricCycleTimeCount1
      End If
    End If

    If IO.FabricCycleInput2 And FabricCycleTimeCount2 > 10 Then
            FabricCycleTime2 = FabricCycleTimeCount2
        End If
    If IO.FabricCycleInput3 And FabricCycleTimeCount3 > 10 Then
      FabricCycleTime3 = FabricCycleTimeCount3
    End If
    If IO.FabricCycleInput4 And FabricCycleTimeCount4 > 10 Then
      FabricCycleTime4 = FabricCycleTimeCount4
    End If


    If MainPumpFeedBack Then
      If IO.FabricCycleInput1 And Not FabricCycleInput1Was Then
        FabricCycleTimer1.Start()
        FabricCycleInput1Was = Not FabricCycleInput1Was
      End If
      FabricCycleInput1Was = IO.FabricCycleInput1
      If IO.FabricCycleInput2 And Not FabricCycleInput2Was Then
        FabricCycleTimer2.Start()
        FabricCycleInput2Was = Not FabricCycleInput2Was
      End If
      FabricCycleInput2Was = IO.FabricCycleInput2
      If IO.FabricCycleInput3 And Not FabricCycleInput3Was Then
        FabricCycleTimer3.Start()
        FabricCycleInput3Was = Not FabricCycleInput3Was
      End If
      FabricCycleInput3Was = IO.FabricCycleInput3
      If IO.FabricCycleInput4 And Not FabricCycleInput4Was Then
        FabricCycleTimer4.Start()
        FabricCycleInput4Was = Not FabricCycleInput4Was
      End If
      FabricCycleInput4Was = IO.FabricCycleInput4
    Else
      FabricCycleTimer1.Stop()
      FabricCycleTimer2.Stop()
      FabricCycleTimer3.Stop()
      FabricCycleTimer4.Stop()
    End If
    FabricCycleTimeCount1 = FabricCycleTimer1.TimeElapsed
    FabricCycleTimeCount2 = FabricCycleTimer2.TimeElapsed
    FabricCycleTimeCount3 = FabricCycleTimer3.TimeElapsed
    FabricCycleTimeCount4 = FabricCycleTimer4.TimeElapsed

    If Parent.IsProgramRunning And (Command02.IsOn Or Command05.IsOn Or Command20.IsOn Or Command01.IsOn Or Command33.IsOn) And Not IO.MainPumpFB Then
      If Not ProgramIdleTimerCounting Then
        ProgramIdleTimer.TimeRemaining = 1800
        ProgramIdleTimerCounting = True
      ElseIf ProgramIdleTimer.Finished And Not StopAllProgram Then
        StopAllProgram = True
      End If
    Else
      ProgramIdleTimer.TimeRemaining = 1800
      StopAllProgram = False
      ProgramIdleTimerCounting = False
    End If

    '計算最大、最小、以及平均布速
    Static CycleTimeRecordStartTimer As New DelayTimer
    StartCycleTimeRecord = CycleTimeRecordStartTimer.Run(HeatNow, 300)
    If StartCycleTimeRecord Then
      If FabricCycleTime1 > 0 Then
        If MaximumCycleTime1 = 0 Then MaximumCycleTime1 = FabricCycleTime1
        If MinimumCycleTime1 = 0 Then MinimumCycleTime1 = FabricCycleTime1
        If AverageCycleTime1 = 0 Then AverageCycleTime1 = FabricCycleTime1
        If FabricCycleTime1 > MaximumCycleTime1 Then MaximumCycleTime1 = FabricCycleTime1
        If FabricCycleTime1 < MinimumCycleTime1 Then MinimumCycleTime1 = FabricCycleTime1
        If AverageCycleTime1 > 0 And Not FabricCycleTime1 = Parameters.MaximumFabricCycleTime Then
          AverageCycleTime1 = (AverageCycleTime1 + FabricCycleTime1) \ 2
        End If
      End If
      If FabricCycleTime2 > 0 Then
        If MaximumCycleTime2 = 0 Then MaximumCycleTime2 = FabricCycleTime2
        If MinimumCycleTime2 = 0 Then MinimumCycleTime2 = FabricCycleTime2
        If AverageCycleTime2 = 0 Then AverageCycleTime2 = FabricCycleTime2
        If FabricCycleTime2 > MaximumCycleTime2 Then MaximumCycleTime2 = FabricCycleTime2
        If FabricCycleTime2 < MinimumCycleTime2 Then MinimumCycleTime2 = FabricCycleTime2
        If AverageCycleTime2 > 0 And Not FabricCycleTime1 = Parameters.MaximumFabricCycleTime Then
          AverageCycleTime2 = (AverageCycleTime2 + FabricCycleTime2) \ 2
        End If
      End If
      If FabricCycleTime3 > 0 Then
        If MaximumCycleTime3 = 0 Then MaximumCycleTime3 = FabricCycleTime3
        If MinimumCycleTime3 = 0 Then MinimumCycleTime3 = FabricCycleTime3
        If AverageCycleTime3 = 0 Then AverageCycleTime3 = FabricCycleTime3
        If FabricCycleTime3 > MaximumCycleTime3 Then MaximumCycleTime3 = FabricCycleTime3
        If FabricCycleTime3 < MinimumCycleTime3 Then MinimumCycleTime3 = FabricCycleTime3
        If AverageCycleTime3 > 0 And Not FabricCycleTime3 = Parameters.MaximumFabricCycleTime Then
          AverageCycleTime3 = (AverageCycleTime3 + FabricCycleTime3) \ 2
        End If
      End If
      If FabricCycleTime4 > 0 Then
        If MaximumCycleTime4 = 0 Then MaximumCycleTime4 = FabricCycleTime4
        If MinimumCycleTime4 = 0 Then MinimumCycleTime4 = FabricCycleTime4
        If AverageCycleTime4 = 0 Then AverageCycleTime4 = FabricCycleTime4
        If FabricCycleTime4 > MaximumCycleTime4 Then MaximumCycleTime4 = FabricCycleTime4
        If FabricCycleTime4 < MinimumCycleTime4 Then MinimumCycleTime4 = FabricCycleTime4
        If AverageCycleTime4 > 0 And Not FabricCycleTime4 = Parameters.MaximumFabricCycleTime Then
          AverageCycleTime4 = (AverageCycleTime4 + FabricCycleTime4) \ 2
        End If
      End If
      If MaximumCycleTime1 > MaximumCycleTime2 Then
        MaximumCycleTime = MaximumCycleTime1
      Else
        MaximumCycleTime = MaximumCycleTime2
      End If
      If AverageCycleTime1 > 0 And AverageCycleTime2 > 0 Then
        AverageCycleTime = (AverageCycleTime1 + AverageCycleTime2) \ 2
      ElseIf AverageCycleTime1 = 0 Then
        AverageCycleTime = AverageCycleTime2
      ElseIf AverageCycleTime2 = 0 Then
        AverageCycleTime = AverageCycleTime1
      End If
    End If

    IO.CycleTime1 = FabricCycleTime1
    IO.CycleTime2 = FabricCycleTime2
    IO.CycleTime3 = FabricCycleTime3
    IO.CycleTime4 = FabricCycleTime4

    Delay_BTankReady = BTankLowLevel

    '藥缸功能執行時，啟動溫度控制
    If (Command51.IsOn Or Command52.IsOn Or Command54.IsOn Or Command55.IsOn Or Command56.IsOn Or Command57.IsOn Or Command59.IsOn Or _
        Command64.IsOn Or Command65.IsOn Or Command66.IsOn Or Command67.IsOn) And Not Command01.IsOn And TempControlFlag Then
      If TemperatureControl.TempFinalTemp < 590 Then
        CoolNow = True
        HeatNow = False
      Else
        CoolNow = False
        HeatNow = True
      End If
    End If

    '各種計數器和計時器
    If (IO.Entanglement1 Or IO.Entanglement2 Or IO.布輪1纏車 Or IO.布輪2纏車 Or 布輪1纏車 Or 布輪2纏車 Or 布輪3纏車 Or 布輪4纏車) And Not EntangleWas Then
      纏車次數計數器 = 纏車次數計數器 + 1
    End If
    EntangleWas = IO.Entanglement1 Or IO.Entanglement2 Or IO.布輪1纏車 Or IO.布輪2纏車 Or 布輪1纏車 Or 布輪2纏車 Or 布輪3纏車 Or 布輪4纏車
    IO.纏車次數 = 纏車次數計數器


    '釋放資源給其他程式，避免站用100%CPU造成當機，且讓每次迴圈延遲10ms
    'Application.DoEvents()
    'System.Threading.Thread.Sleep(10)

    If Not Parent.IsProgramRunning And ProgramStopCleanDatabase Then
      DispenseState()
    End If

    '背景執行C缸備藥
    CTankPrepare.Run()

    If BackgroundMixing Then
      If BackgroundMixingOff.Finished And Not BackgroundMixingOnWas Then
        BackgroundMixingOn.TimeRemaining = Parameters.BackgroundMixingOnTime
        BackgroundMixingOnWas = True
        BackgroundMixingOffWas = False
      End If
      If BackgroundMixingOn.Finished And Not BackgroundMixingOffWas Then
        BackgroundMixingOff.TimeRemaining = Parameters.BackgroundMixingOffTime
        BackgroundMixingOnWas = False
        BackgroundMixingOffWas = True
      End If

    End If

    '***********************PH功能*************************************************************
    '-----------------------------------------------------
    '如果在十秒內,重複叫PhControl ,將縮短初期PH檢測時間
    '但是條件是第一次PhControl最少要執行過60秒以上,才可以下次重新執行PhControl時,縮短初期檢測時間


    If PhControl.IsOn = True And PhCirculateRun.IsOn = True And PH檢測_短時間內不在檢測 = False Then
      PH再檢測 = My.Computer.Clock.TickCount
      PH檢測_短時間內不在檢測 = True
    End If

    If (My.Computer.Clock.TickCount - PH再檢測) > 60000 And PH檢測_短時間內不在檢測 = True Then
      已經確保120秒檢測完成 = True
      是否縮短檢測時間 = False
      PH再檢測時間.TimeRemaining = 0
      短時間內重新執行 = False
    End If


    If PhControl.IsOn = False And 已經確保120秒檢測完成 = True And 短時間內重新執行 = False Then
      短時間內重新執行 = True
      PH再檢測時間.TimeRemaining = 10
      是否縮短檢測時間 = True
      PH檢測_短時間內不在檢測 = False
    End If

    If 短時間內重新執行 = True And PH再檢測時間.Finished Then
      是否縮短檢測時間 = False
    End If



    '================================================================
    '-----------------------計算使用PH量---------------------------------------
    If Not IO.PhAddHacOut Then
      加酸時間.TimeRemaining = 0
    ElseIf IO.PhAddHacOut And 加酸時間.Finished Then
      加酸時間.TimeRemaining = 1
      加酸次數 = 加酸次數 + 1
    End If
    If Not PhControl.IsOn Then
      加酸次數 = 0
    End If
    '-----------------------顯示mimic---------------------------------------
    test1 = IO.pHValue / 100
    test2 = PhControl.C75Gradient / 100
    test3 = 加酸次數
    test4 = PhControl.Wait1.TimeRemaining
    test5 = PhControl.PhAddError
    test6 = PhControl.ExpectPh / 100
    test7 = PhControl.CountHacVolume
    test8 = PhCheck.Wait.TimeRemaining
    test9 = PhControl.RegisterI(2)
    test10 = CType(PhControl.TotalAddHac2, Integer)
    test11 = PhControl.CalculateTmepRange
    test12 = PhControl.Wait10Second.TimeRemaining
    test13 = PhControl.WaitAddHac.TimeRemaining
    test14 = PhControl.RegisterI(10)
    test15 = PhControl.ExpectPh2

    test19 = PhCheck.DelayWait.TimeRemaining
    test20 = PhCheck.S12 / 100
    test21 = Parameters.PhConcentration
    test22 = PhControl.PhFillLevel
    test23 = Parameters.PhPumpOutRatio
    test25 = PhControl.AddOverError
    test26 = PhControl.AddOverTimes
    test27 = PhCheckError.PhOverAddTimes
    test28 = PhCheckError.Wait.TimeRemaining
    test29 = PhCheckError.StopAddPH
    test30 = PhCheckError.StopAddPH2
    test31 = PhControl.MathNeverOpenValue
    test32 = PhControl.次數
    test32 = PhControl.次數
    test33 = PhControl.微調次數
    補酸狀態分析 = PhControl.減酸比率
    test35 = PhControl.W0微量補酸


    If Parameters.PhShowData = 0 Then
      PhShowData = "PhShowClose"
    Else
      PhShowData = "PhShowOpen"
    End If


    If Parameters.PhCirTank = 0 Then
      PhShowPic = False
    Else
      PhShowPic = True
    End If

    If PhCheck.State = PhCheck_.PhCheck.Check1 Then
      test16 = "Check1"
    ElseIf PhCheck.State = PhCheck_.PhCheck.Check2 Then
      test16 = "Check2"
    ElseIf PhCheck.State = PhCheck_.PhCheck.DelayCheck1 Then
      test16 = "DelayCheck1"
    ElseIf PhCheck.State = PhCheck_.PhCheck.DelayCheck2 Then
      test16 = "DelayCheck2"
    ElseIf PhCheck.State = PhCheck_.PhCheck.Finish Then
      test16 = "Finish"
    ElseIf PhCheck.State = PhCheck_.PhCheck.off Then
      test16 = "off"
    End If

    If PhControl.State = PhControl_.PhControl.AddHacError Then
      test24 = "AddHacError"
    ElseIf PhControl.State = PhControl_.PhControl.Alarm_TmepHigh Then
      test24 = "Alarm_TmepHigh"
    ElseIf PhControl.State = PhControl_.PhControl.AlarmPhHigh Then
      test24 = "AlarmPhHigh"
    ElseIf PhControl.State = PhControl_.PhControl.AllAddHac Then
      test24 = "AllAddHac"
    ElseIf PhControl.State = PhControl_.PhControl.CheckPhValue Then
      test24 = "CheckPhValue"
    ElseIf PhControl.State = PhControl_.PhControl.Divider Then
      test24 = "Divider"
    ElseIf PhControl.State = PhControl_.PhControl.DownloadParameter Then
      test24 = "DownloadParameter"
    ElseIf PhControl.State = PhControl_.PhControl.Finished Then
      test24 = "Finished"
    ElseIf PhControl.State = PhControl_.PhControl.MathAddHac2 Then
      test24 = "MathAddHac2"
    ElseIf PhControl.State = PhControl_.PhControl.MathAddHac3 Then
      test24 = "MathAddHac3"
    ElseIf PhControl.State = PhControl_.PhControl.MathAddHac4 Then
      test24 = "MathAddHac4"
    ElseIf PhControl.State = PhControl_.PhControl.Off Then
      test24 = "Off"
    ElseIf PhControl.State = PhControl_.PhControl.Pause Then
      test24 = "Pause"
    ElseIf PhControl.State = PhControl_.PhControl.WaitKeepTime Then
      test24 = "WaitKeepTime"
    ElseIf PhControl.State = PhControl_.PhControl.WaitTempArrival Then
      test24 = "WaitTempArrival"
    ElseIf PhControl.State = PhControl_.PhControl.WaitTempFinish Then
      test24 = "WaitTempFinish"
    End If
    '-------------------------PH測試-----------------------------------------------
    If PhControlFlag = True Then

      PhControl.Run()                           'PH控制

      PhCirculateRun.Run()                      '跑迴流桶動作

      '-----------------------顯示PH加藥量-----------------------------------
      If IO.PhAddPump And IO.PhAddHacOut And MathHacFlag = False Then
        MathHacTimes.Stop()

        MathHacTimes.Start()
        MathHacFlag = True
      End If

      If MathHacFlag = True And IO.PhAddPump = False And IO.PhAddHacOut = False Then
        UseHacThisValue = MathHacTimes.TimeElapsed
        'MathHacTimes.Stop()
        'UseHacThisValue = MathHacTimes.TimeElapsed
        MathHacFlag = False
        UseHacAllTotal = UseHacAllTotal + UseHacThisValue

        If IO.pHValue < 450 Then
          UseHacAllTotal2 = UseHacAllTotal2 + UseHacThisValue
        End If

      End If
      '-----------------PH控制加酸時，實際低於PH450時，開始計算每次加酸量，當 實際加酸總量 > 目標加酸總量 
      If UseHacAllTotal2 > PhControl.TotalAddHac4 Then                        '20525---9005 1801 2020 404

        If (UseHacAllTotal2 - PhControl.TotalAddHac4) > 1000 Then

          Alarms.MessageAddHacError = True

        End If

      End If
    ElseIf PhControlFlag = False Then
      MathHacTimes.Stop()
    End If
        '------------------------------------------------------------------------------
        If PhControlFlag Then
            PhCirRun = True
        End If
        If PhControlFlag = False Then
            PhControl.Cancel()                      '結束PH控制
            If Command78.State = Command78.S78.KeepTime Then
                PhCirculateRun.Run()                 '結束迴流桶動作
            Else
                If PhCirRun And Parameters.PhCirRuning = 1 And Parent.IsProgramRunning Then
                    PhCirculateRun.Run()                 '結束迴流桶動作
                Else
                    PhCirculateRun.Cancel()                 '結束迴流桶動作
                End If

            End If

            PhControl.Wait.TimeRemaining = 0
            PhControl.ExpectPh = 0
            UseHacAllTotal = 0
            UseHacAllTotal2 = 0
            Alarms.MessageAddHacError = False
            PhControl.Wait1.TimeRemaining = 0
            PhControl.CalculateTmepRange = 0
            PhControl.TotalAddHac2 = 0
        End If



        '-----------------------------------------------------------------------------
        If PhControl.CheckPhRun = True And PhControl.IsOn Then
      PhCheck.Run()
      PhCheckError.Run()
    End If
    '-----------------------------------------------------------------------------
    If PhControl.CheckPhRun = False Or Not PhControl.IsOn Then
      PhCheckError.Cancel()
      PhCheck.Cancel()
      PhCheck.DelayWait.TimeRemaining = 0
      PhCheck.Wait.TimeRemaining = 0
      PhCheck.Wait1.TimeRemaining = 0
      PhCheck.X = 0
      PhCheck.X2 = 0
      PhCheck.S12 = 0
      PhCheckError.PhOverAddTimes = 0
    End If
    '---------------------------------------------------主要是關閉加藥閥時,需要延遲關閉循環馬達,讓加酸打入染機
    If IO.PhAddHacOut = True Then
      DelayAddTime.TimeRemaining = Parameters.DelayCirculatPump       '加酸關閉,循環馬達延遲關閉時間
      DelayAddHac = True
    End If

    If IO.PhAddHacOut = False And DelayAddTime.Finished Then
      DelayAddHac = False
    End If
    '-----------------------------------------------------------------------------

    Alarms.HighTempNoAddHac = PhControl.IsOn And IO.MainPumpFB And IO.pHTemperature1 > (Parameters.SetAddSafetyTemp * 10) And Not PhCheckError.StopAddPH And Not PhCheckError.StopAddPH2
        'PH加酸閥
        IO.PhAddHacOut = NHalt And Alarms.MessageAddHacError = False And (PhControl.PhAddHacOut And IO.MainPumpFB And IO.pHTemperature1 < (Parameters.PH加酸安全溫度 * 10))       'PH加酸閥
        'PH入染機
        IO.PhInToMachine = NHalt And IO.MainPumpFB And (
                                 (PhControl.PhInToMachine And IO.pHTemperature1 < (Parameters.PH加酸安全溫度 * 10)) Or
                                 (PhCirculateRun.PhInToMachine And IO.pHTemperature1 < (Parameters.PH加酸安全溫度 * 10)) Or
                                 DelayAddHac = True
                                 ) _
                              And Parameters.PhCirTank = 1 'PH入染機閥

        'PH入迴水
        IO.PhFillCirculate = ((PhCirculateRun.PhFillCirculate2 Or PhCirculateRun.PhFillCirculate3) And IO.MainPumpFB And (IO.pHTemperature1 < Parameters.PH加酸安全溫度 * 10))        'PH迴水閥

        'PH定量泵
        IO.PhAddPump = NHalt And (Alarms.MessageAddHacError = False And IO.MainPumpFB And (PhControl.PhAddPump And IO.pHTemperature1 < (Parameters.PH加酸安全溫度 * 10)
                                                                   )) And Not PhCheckError.StopAddPH And Not PhCheckError.StopAddPH2     'PH定量馬達

        'PH循環泵
        IO.PhCirculate = ((PhCirculateRun.PHTankAddPump) Or PhWash.PhCirculatePump Or DelayAddHac = True) _
                          And IO.MainPumpFB And IO.pHTemperature1 < (Parameters.PH加酸安全溫度 * 10) _
                          And Parameters.PhCirTank = 1

        'PH冷卻
        IO.PhCool = NHalt And ((PhCirculateRun.IsOn) And (IO.pHTemperature1 > Parameters.PhCoolingTemp * 10))              'PH冷卻

    'PH入清水
    IO.PhFillCold = PhWash.PhWashFill Or PhWash.PhNoCirTank Or PhWash.PhWashFill

        'PH排水
        IO.PhDrain = (PhWash.PhDrain And Not IO.PhFillCirculate And Parameters.PhCirTank = 1)
        '=======================藥缸回流桶控制==========================================================

        PhToAdd = (PhCirculateRun.CtankToMachine And IO.pHTemperature1 < (Parameters.PH加酸安全溫度 * 10)) And Parameters.PhCirTank = 0
    PhToCPump = PhCirculateRun.CAddPump And IO.pHTemperature1 < (Parameters.PH加酸安全溫度 * 10) And Parameters.PhCirTank = 0
    PhToDrain = PhWash.PhNoCirTank And Parameters.PhCirTank = 0
    '-----------------------------------------------------------------------------
    '******************************************************************************************
    If pHWashRun Then
      PhWash.Run()
    End If
    If IO.ManualPHWash And Not pHManualWashWas Then
      PhControl.Cancel() : PhControlFlag = False : PhCirculateRun.Cancel()
      Command77.Cancel() : Command74.Cancel() : Command75.Cancel() : Command76.Cancel() : Command78.Cancel() : Command73.Cancel() : Command80.Cancel()
      pHWashRun = True
      pHManualWashWas = True
    ElseIf Not IO.ManualPHWash Then
      pHManualWashWas = False
    End If

    'pH啟用
    IO.EnablePh = Parameters.ConnectPhSystem = 1


    '染色水量統計
    '程式開始執行後手動進水則開始統計，到B加藥時停止統計
    染色水量統計開始 = True

    HighSpeedCounter1 = IO.HSCounter1
    If 染色水量統計開始 And Not 染色水量統計結束 Then
      If IO.ColdFill Or IO.HotFill Then
        If HighSpeedCounter1 > HighSpeedCounter1Was Then
          TotalWaterLiters = TotalWaterLiters + (HighSpeedCounter1 - HighSpeedCounter1Was) * Parameters.VolumePerCount
          TotalWaterLitersWas = TotalWaterLiters
        ElseIf HighSpeedCounter1Was > HighSpeedCounter1 Then
          TotalWaterLiters = TotalWaterLiters + (HighSpeedCounter1 + 65535 - HighSpeedCounter1Was) * Parameters.VolumePerCount
          TotalWaterLitersWas = TotalWaterLiters
        Else
          TotalWaterLiters = TotalWaterLiters
          TotalWaterLitersWas = TotalWaterLiters
        End If
      End If
      If IO.Drain Or IO.HotDrain Or IO.DrainPB Or IO.HotDrainPB Or IO.PowerDrain Or IO.PowerHotDrain Or IO.PowerDrainPB Or IO.PwoerHotDrainPB Then
        TotalWaterLiters = 0
        TotalWaterLitersWas = 0
      End If
    End If
    If IO.BTankAddition Or IO.BTankAddPump Then
      染色水量統計結束 = True
    End If
    '    If Command03.IsOn Or Command04.IsOn Or Command11.IsOn Or Command12.IsOn Or Command13.IsOn Or Command15.IsOn Or Command16.IsOn Or Command18.IsOn Then
    '    If HighSpeedCounter1 > HighSpeedCounter1Was Then
    '    FillWaterLiters = FillWaterLiters + (HighSpeedCounter1 - HighSpeedCounter1Was) * Parameters.VolumePerCount
    '    FillWaterLitersWas = FillWaterLiters
    '    ElseIf HighSpeedCounter1Was > HighSpeedCounter1 Then
    '    FillWaterLiters = FillWaterLiters + (HighSpeedCounter1 + 65535 - HighSpeedCounter1Was) * Parameters.VolumePerCount
    '    FillWaterLitersWas = FillWaterLiters
    '    Else
    '    FillWaterLiters = FillWaterLiters
    '    FillWaterLitersWas = FillWaterLiters
    '    End If
    '    Else
    '   FillWaterLiters = 0
    '   End If

    HighSpeedCounter1Was = IO.HSCounter1
    CalculateUtilities()
    SimulateFlowmeter()
    WaterUsed = TotalWaterLiters

        If Parameters.ConnectSPCTest = 1 Then
            SPCConnectError = False
        End If

    If 工單 IsNot Nothing Then
      If 工單.Length >= 8 And SPCConnectTimer.Finished And Not SPCConnectError And Parameters.ConnectSPCEnable = 1 Then
        ConnectBDC()
        DispenseState()
        SPCConnectTimer.TimeRemaining = 4
      End If
    End If


  End Sub

  '用電量計算
  Private Sub CalculateUtilities()

    'Utility usage calculations

    Dim KWNow As Integer

    'PowerFactor = (750 * 68) / 100

    KWNow = 0 'Reset power being used now uasge to 0

    Static UtilitiesTimer As New Timer

    If UtilitiesTimer.Finished Then

      PumpsKW = CType(Parameters.PowerWOfPumps / 1000, Integer)

      If IO.MainPumpFB Then KWNow = KWNow + PumpsKW


      'Assume all motors run at about 68% of rated capacity. 750W is equiv to HP.

      'Power factor = 68% of 750 , convert to watts

      'PowerFactor = 510 'watts which is .51kw

      PowerKWS += Convert.ToInt32(KWNow * 0.68)

      UtilitiesTimer.TimeRemaining = 1

    End If

    If PowerKWS >= 3600 Then

      PowerKWHrs += 1

      PowerKWS -= 3600

    End If

    PowerUsed = PowerKWHrs

    'Steam Consumption formula

    '120% * WorkingVolume * deg F of temp rise * weight of water (8.33lb/g) * 0.001 = lbs of steam used

    'Steam factor = 120% * 8.33 * 0.001 = 1/100

    'lbs of Steam used = working volume * Temp rise in F / 100

    FinalTemp = CType((TemperatureControl.TempFinalTemp - 32) * 5 / 9, Integer)

    VesVolume = Parameters.MainTankLiters

    VesTemp = CType((IO.MainTemperature - 32) * 5 / 9, Integer)

    If FinalTemp <> FinalTempWas Then

      If VesTemp > FinalTempWas Then

        StartTemp = VesTemp

      Else

        StartTemp = FinalTempWas

      End If

      If FinalTemp > StartTemp Then

        TempRise = FinalTemp - StartTemp

        SteamNeeded = ((VesVolume * TempRise) \ 1000)

        SteamUsed += SteamNeeded

      End If

      SteamUsedKgs = CType(SteamUsed * 0.45359237, Integer)

      FinalTempWas = FinalTemp

    End If
  End Sub

  Private Sub SimulateFlowmeter()
    If Not IO.LowLevel And Not StartSimulateFillCheck And ((Not IO.SystemAuto And (IO.HotFillPB Or IO.FillPB Or IO.SoftFillPB)) Or IO.HotFill Or IO.ColdFill Or IO.FillReCycleWater) Then
      StartSimulateFillCheck = True
      SimulateFillCheckFinish = False
      SimulateFillCheckSeconds = 0
    End If
    If StartSimulateFillCheck And IO.LowLevel Then
      SimulateFillCheckFinish = True
      If SimulateFillCheckSeconds > 0 Then
        SimulateFillLitersPerSecond = Parameters.MainTankLowLevelLiters \ SimulateFillCheckSeconds
      End If
      FillWaterLiters = Parameters.MainTankLowLevelLiters
      TotalWaterLiters = TotalWaterLiters + Parameters.MainTankLowLevelLiters
      StartSimulateFillCheck = False
    End If
    If StartSimulateFillCheck And Not SimulateFillCheckFinish And SimulateFillCheckTimer.Finished Then
      SimulateFillCheckTimer.TimeRemaining = 1
      SimulateFillCheckSeconds = SimulateFillCheckSeconds + 1
    End If
    If SimulateFillCheckFinish And SimulateFillCheckTimer.Finished And ((Not IO.SystemAuto And (IO.HotFillPB Or IO.FillPB Or IO.SoftFillPB)) Or IO.HotFill Or IO.ColdFill Or IO.FillReCycleWater) Then
      SimulateFillCheckTimer.TimeRemaining = 1
      SimulateFillWaterLiters = SimulateFillWaterLiters + SimulateFillLitersPerSecond
      SimulateTotalWaterLiters = SimulateTotalWaterLiters + SimulateFillLitersPerSecond
    End If
  End Sub

  Public Function ReadInputs(ByVal dinp() As Boolean, ByVal aninp() As Short, ByVal temp() As Short) As Boolean Implements ACControlCode.ReadInputs
    Return IO.ReadInputs(dinp, aninp, temp)
  End Function

  Public Sub WriteOutputs(ByVal dout() As Boolean, ByVal anout() As Short) Implements ACControlCode.WriteOutputs
    IO.WriteOutputs(dout, anout)
  End Sub


  <ScreenButton("主缸", 1, ButtonImage.Vessel), ScreenButton("藥缸訊息", 2, ButtonImage.SideVessel), ScreenButton("染料訊息", 3, ButtonImage.Information), ScreenButton("助劑訊息", 4, ButtonImage.Information), ScreenButton("能源使用", 5, ButtonImage.Information), ScreenButton("測試", 6, ButtonImage.Beam)>
  Public Sub DrawScreen(ByVal screen As Integer, ByVal row() As String) Implements ACControlCode.DrawScreen
    Dim maximumRows As Integer = 24

    Select Case screen
      Case 1
        'Screen 1
        If FlashFlag Then
          row(1) = " "
        ElseIf Parent.IsProgramRunning Then
          row(1) = If(Language = LanguageValue.ZhTW, "程式執行中", "Program is running")
        ElseIf Parent.IsPaused Then
          row(1) = If(Language = LanguageValue.ZhTW, "程式暫停", "Program is paused")
        Else
          row(1) = If(Language = LanguageValue.ZhTW, "待機", "Idle")
        End If
        row(2) = If(Language = LanguageValue.ZhTW, "現在時間", "Current Time") & ":" & CurrentTime
        row(3) = If(Language = LanguageValue.ZhTW, "運行時間", "Run Time") & ":" & 程式執行小時.ToString("#00") & ":" & 程式執行分鐘.ToString("#00") & "/" & 預計時間小時.ToString("#00") & ":" & 預計時間分鐘.ToString("#00")
        row(4) = Translations.Translate("Actual Temp") & ":" & IO.MainTemperature / 10 & "C"
        row(5) = Translations.Translate("Calculate Temp") & ":" & Setpoint / 10 & "C"
        row(6) = Translations.Translate("Target Temp") & ":" & TemperatureControl.TempFinalTemp / 10 & "C"
        If Command10.IsOn Then
          row(7) = Translations.Translate("Holding Time") & TimerString(Command10.Wait.TimeRemaining)
        Else
          row(7) = Translations.Translate("Holding Time") & TimerString(Command01.Wait.TimeRemaining)
        End If
        If IO.HighLevel Then
                    row(8) = Translations.Translate("MainTankLevel") & ":" & Translations.Translate("High") & "/" & FillWaterLiters & "/" & TotalWaterLiters & "L"
                ElseIf IO.MiddleLevel Then
                    row(8) = Translations.Translate("MainTankLevel") & ":" & Translations.Translate("Middle") & "/" & FillWaterLiters & "/" & TotalWaterLiters & "L"
                ElseIf IO.LowLevel Then
          row(8) = Translations.Translate("MainTankLevel") & ":" & Translations.Translate("Low") & "/" & FillWaterLiters & "/" & TotalWaterLiters & "L"
        Else
                    row(8) = Translations.Translate("MainTankLevel") & ":" & Translations.Translate("None") & "/" & FillWaterLiters & "/" & TotalWaterLiters & "L"
                End If
        row(9) = Translations.Translate("馬達速度") & ": " & IO.MainPumpSpeed & "/ " & IO.Reel1Speed & "/ " & IO.Reel2Speed
        row(10) = Translations.Translate("TempControlOutput") & ":" & IO.TemperatureControl / 10 & "%"
        row(11) = "CycleTime" & ": " & FabricCycleTime1 & "/ " & FabricCycleTime2 & "/ " & FabricCycleTime3 & "/ " & FabricCycleTime4 & " sec"
        row(12) = "噴壓:" & (IO.NozzlePressure / 10).ToString("0.0") & "/ " & (IO.MainPressure / 10).ToString("0.0") & "/ " & (IO.PressureDifferent / 10).ToString("0.0") & " bar"
        row(13) = Translations.Translate("Message") & ":"
        If Command10.IsPaused Or (Command10.IsOn And Not IO.SystemAuto) Then
          row(14) = Translations.Translate("Pause")
        ElseIf Command01.IsPaused Or (Command01.IsOn And Not IO.SystemAuto) Then
          row(14) = Translations.Translate("Pause")
        ElseIf Command10.IsHolding Then
          row(14) = Translations.Translate("Holding")
        ElseIf Command01.IsHolding Then
          row(14) = Translations.Translate("Holding")
        ElseIf TemperatureControl.IsHeating And Not Command10.IsHolding Then
          row(14) = Translations.Translate("Heating")
        ElseIf TemperatureControl.IsCooling And Not Command10.IsHolding Then
          row(14) = Translations.Translate("Cooling")
        ElseIf TemperatureControl.IsHeating And Setpoint - IO.MainTemperature > 20 Then
          row(14) = Translations.Translate("SteamNotEnough")
        ElseIf TemperatureControl.IsCooling And IO.MainTemperature - Setpoint > 20 Then
          row(14) = Translations.Translate("CoolingWaterNotEnough")
        End If
        ' If MessageCallOperator Then
        ' row(15) = Translations.Translate("CallOperator")
        ' ElseIf MessageTakeSample Then
        ' row(15) = Translations.Translate("Sample")
        ' ElseIf MessageLoadFabric Then
        ' row(15) = Translations.Translate("Load")
        ' ElseIf MessageUnloadFiber Then
        ' row(15) = Translations.Translate("Unload")
        ' End If
        ' If MessageMPumpRepair Then
        'row(16) = Translations.Translate("MainPumpMaintenance")
        ' End If
        ' If ManualOperation Then
        ' row(17) = Translations.Translate("Manual")
        ' ElseIf IO.SystemAuto Then
        ' row(17) = Translations.Translate("Auto")
        ' End If
        ' If MessageSystemPause Then
        ' row(17) = Translations.Translate("Pause")
        ' End If
        ' row(18) = "纏車次數" & ": " & IO.纏車次數
        ' If Alarms.FabricStop Then
        ' row(19) = Translations.Translate("Entangle")
        ' End If
        ' If Alarms.PlcError Then
        ' row(19) = Translations.Translate("PLC Not Online")
        ' End If
        ' If RunCTankPrepare And FlashFlag Then
        ' row(20) = CTankPrepare.StateString
        ' Else
        ' row(20) = ""
        ' End If


        If PhCirculateRun.IsOn And (Not Parent.IsPaused) And IO.MainPumpFB Then
                    row(15) = If(Language = LanguageValue.ZhTW, "pH回流檢測中", "pH Checking")
                ElseIf PhCirculateRun.IsOn And (Parent.IsPaused Or Not IO.MainPumpFB) Then
                    row(15) = If(Language = LanguageValue.ZhTW, "pH回流檢測停止", "pH Stop")
                End If


        '-------------------------------------------------------------------------------------------------------
        If PhControl.IsOn Or PhCirculateRun.IsOn Then
          If Command77.State = Command77.S77.Start Or Command77.State = Command77.S77.KeepTime Then
            row(16) = If(Language = LanguageValue.ZhTW, "pH 目標值   ", "        Pause") & (Command77.PhTarget / 100).ToString("0.00")
          ElseIf Command74.State = Command74.S74.Start Then
            row(16) = If(Language = LanguageValue.ZhTW, "pH 目標值   ", "        Pause") & (Command74.PhTarget / 100).ToString("0.00")
          ElseIf PhControl.State = PhControl_.PhControl.AllAddHac Or PhControl.State = PhControl_.PhControl.Divider Or PhControl.State = PhControl_.PhControl.AllPhWaitTime Then
            row(16) = If(Language = LanguageValue.ZhTW, "pH 計量加藥中   ", "        Pause")
          Else

            row(16) = If(Language = LanguageValue.ZhTW, "pH 預定設定值   ", "        Pause") & (PhControl.ExpectPh / 100).ToString("0.00")
          End If
        End If

        If PhCirculateRun.IsOn Then
          If PhControl.IsOn Then
            If PhControl.ExpectPh <= 200 Then
              row(17) = If(Language = LanguageValue.ZhTW, "pH 實際值   ", "        Pause") & (IO.pHValue / 100).ToString("0.00")
            Else
              Dim TestPH As Integer
              TestPH = CType((IO.pHValue + PhControl.ExpectPh) / 2, Integer)

              If IO.pHValue > PhControl.ExpectPh Then

                row(17) = If(Language = LanguageValue.ZhTW, "pH 實際值   ", "        Pause") & (IO.pHValue / 100).ToString("0.00")


              Else
                row(17) = If(Language = LanguageValue.ZhTW, "pH 實際值   ", "        Pause") & (TestPH / 100).ToString("0.00")
              End If

            End If
          Else
            row(17) = If(Language = LanguageValue.ZhTW, "pH 實際值   ", "        Pause") & (IO.pHValue / 100).ToString("0.00")
          End If

        Else
          row(17) = If(Language = LanguageValue.ZhTW, "pH 實際值   ", "        Pause") & (IO.pHValue / 100).ToString("0.00")
        End If
        row(18) = "模擬進水量: " & SimulateFillWaterLiters & " L"
        row(19) = "模擬總進水量: " & SimulateTotalWaterLiters & " L"
      '-------------------------------------------------------------------------------------------------------


      Case 2
        'Screen 2 - tank B (dyes)
        row(1) = Translations.Translate("B Tank Message")
        If BTankHighLevel Then
          row(2) = Translations.Translate("B Tank Level") & ":" & IO.TankBLevel / 10 & "% ------->" & Translations.Translate("High Level")
        ElseIf BTankMiddleLevel Then
          row(2) = Translations.Translate("B Tank Level") & ":" & IO.TankBLevel / 10 & "% ------->" & Translations.Translate("Middle Level")
        ElseIf BTankLowLevel Then
          row(2) = Translations.Translate("B Tank Level") & ":" & IO.TankBLevel / 10 & "% ------->" & Translations.Translate("Low Level")
        Else
          row(2) = Translations.Translate("B Tank Level") & ":" & IO.TankBLevel / 10 & "% ------->" & Translations.Translate("None")
        End If
        '加藥目標水位
        If (Command56.IsOn And Command56.State = HJDOS109.Command56.S56.Dose) Then
          row(3) = Translations.Translate("Target Level") & ":" & Command56.SetPoint / 10 & "%"
        ElseIf ManualDose.IsOn And ManualDose.Tank = 5 Then
          row(3) = Translations.Translate("Target Level") & ":" & ManualDose.SetPoint / 10 & "%"
        ElseIf (Command66.IsOn And Command66.State = HJDOS109.Command66.S66.Dose) Then
          row(3) = Translations.Translate("Target Level") & ":" & Command66.SetPoint / 10 & "%"
        Else
          row(3) = Translations.Translate("Target Level") & ":"
        End If      '進水中
        If IO.SystemAuto And IO.BTankFillCold Then
          row(4) = Translations.Translate("Filling")
        ElseIf IO.SystemAuto And IO.BTankFillCirc Then
          row(4) = Translations.Translate("Circulating")
        ElseIf IO.SystemAuto And IO.BTankMixCir Then
          row(4) = Translations.Translate("Mixing")
        Else
          row(4) = ""
        End If
        '顯示攪拌時間
        If Command54.IsOn Then
          row(5) = Command54.StateString
        ElseIf Command64.IsOn Then
          row(5) = Command64.StateString
        ElseIf LA252Ready Then
          row(5) = Translations.Translate("LA-252 Preparing")
        ElseIf Command59.IsOn Then
          row(5) = Command59.StateString
        End If
        '備藥OK
        '加藥中，顯示時間
        If Command56.IsOn Then
          row(6) = Command56.StateString
        ElseIf Command51.IsOn Then
          row(6) = Command51.StateString
        ElseIf Command66.IsOn Then
          row(6) = Command66.StateString
        End If

        row(7) = Translations.Translate("C Tank Message")
        If CTankHighLevel Then
          row(8) = Translations.Translate("C Tank Level") & ":" & IO.TankCLevel / 10 & "% ------->" & Translations.Translate("High Level")
        ElseIf CTankMiddleLevel Then
          row(8) = Translations.Translate("C Tank Level") & ":" & IO.TankCLevel / 10 & "% ------->" & Translations.Translate("Middle Level")
        ElseIf CTankLowLevel Then
          row(8) = Translations.Translate("C Tank Level") & ":" & IO.TankCLevel / 10 & "% ------->" & Translations.Translate("Low Level")
        Else
          row(8) = Translations.Translate("C Tank Level") & ":" & IO.TankCLevel / 10 & "% ------->" & Translations.Translate("None")
        End If
        '加藥目標水位
        If Command57.IsOn And Command57.State = HJDOS109.Command57.S57.Dose Then
          row(9) = Translations.Translate("Target Level") & ":" & Command57.SetPoint / 10 & "%"
        ElseIf ManualDose.IsOn And ManualDose.Tank = 4 Then
          row(9) = Translations.Translate("Target Level") & ":" & ManualDose.SetPoint / 10 & "%"
        ElseIf Command67.IsOn And Command67.State = HJDOS109.Command67.S67.Dose Then
          row(9) = Translations.Translate("Target Level") & ":" & Command67.SetPoint / 10 & "%"
        Else
          row(9) = Translations.Translate("Target Level") & ":"
        End If
        '進水中
        If IO.SystemAuto And IO.CTankFillCold Then '備水顯示程式還沒完成
          row(10) = Translations.Translate("Filling")
        ElseIf IO.SystemAuto And IO.CTankFillCirc Then
          row(10) = Translations.Translate("Circulating")
        ElseIf IO.SystemAuto And IO.CTankMixCir Then
          row(10) = Translations.Translate("Mixing")
        Else
          row(10) = ""
        End If
        '顯示攪拌時間
        If Command55.IsOn Then
          row(11) = Command55.StateString
        ElseIf Command65.IsOn Then
          row(11) = Command65.StateString
        ElseIf Command61.IsOn Then
          row(11) = Command61.StateString
        ElseIf Command57.IsOn Then
          row(11) = Command57.StateString
        ElseIf Command52.IsOn Then
          row(11) = Command52.StateString
        ElseIf Command67.IsOn Then
          row(11) = Command67.StateString
        End If

        '顯示LA-302F狀態
        If ChemicalState = 101 Then
          row(12) = ""
        ElseIf ChemicalState = 202 Then
          row(12) = Translations.Translate("Step") & ChemicalCallOff & " " & Translations.Translate("Chemical is dispensing")
        End If

        If Step1Ready Or Step2Ready Or Step4Ready Or Step5Ready Or Step6Ready Then
          row(13) = Translations.Translate("Step") & If(Step1Ready, "1 ", "") & _
          If(Step2Ready, "2 ", "") & _
          If(Step4Ready, "4 ", "") & _
          If(Step5Ready, "5 ", "") & _
          If(Step6Ready, "6 ", "") & _
          Translations.Translate("chemical dispensing complete")
        Else
          row(13) = ""
        End If

      Case 3  '呼叫染料訊息

        row(1) = "領料單號: " & Parent.Job

        If DyeState = 202 Or DyeState = 201 Then
          row(2) = "步驟" & DyeCallOff & " " & "染料輸送中: " & TimerString(CallLA252.WaitTimer.TimeRemaining)
        Else
          row(2) = ""
        End If

        If DyeStepReady(1) Or _
           DyeStepReady(2) Or _
           DyeStepReady(3) Or _
           DyeStepReady(4) Or _
           DyeStepReady(5) Or _
           DyeStepReady(6) Or _
           DyeStepReady(7) Or _
           DyeStepReady(8) Or _
           DyeStepReady(9) Or _
           DyeStepReady(10) Or _
           DyeStepReady(11) Or _
           DyeStepReady(12) Then
          row(3) = "步驟" & If(DyeStepReady(1), "1 ", "") & _
          If(DyeStepReady(2), "2 ", "") & _
          If(DyeStepReady(3), "3 ", "") & _
          If(DyeStepReady(4), "4 ", "") & _
          If(DyeStepReady(5), "5 ", "") & _
          If(DyeStepReady(6), "6 ", "") & _
          If(DyeStepReady(7), "7 ", "") & _
          If(DyeStepReady(8), "8 ", "") & _
          If(DyeStepReady(9), "9 ", "") & _
          If(DyeStepReady(10), "10 ", "") & _
          If(DyeStepReady(11), "11 ", "") & _
          If(DyeStepReady(12), "12 ", "") & _
          Translations.Translate("染料計量完成")
        Else
          row(3) = ""
        End If

        '        Dim Dyes() As String = GetProductsFromString(DyeProducts)
        '        For i As Integer = 4 To 20 : row(i) = Nothing : Next
        '        If (DyeProducts IsNot Nothing) AndAlso (DyeProducts.Length > 2) Then
        ' For i As Integer = 0 To 15
        ' row(i + 4) = Dyes(i)
        ' Next
        ' End If
        For i As Integer = 0 To 14
          If DyeCode(i) IsNot Nothing Then
            If DyeDispenseResult(i) = "301" Then
              row(i + 4) = DyeStepNumber(i) & ": " & DyeCode(i) & ": " & DyeGrams(i) & "/" & DyeDispenseGrams(i) & " 正常"
            ElseIf DyeDispenseResult(i) = "302" Then
              row(i + 4) = DyeStepNumber(i) & ": " & DyeCode(i) & ": " & DyeGrams(i) & "/" & DyeDispenseGrams(i) & " 手動"
            ElseIf DyeDispenseResult(i) = "309" Then
              row(i + 4) = DyeStepNumber(i) & ": " & DyeCode(i) & ": " & DyeGrams(i) & "/" & DyeDispenseGrams(i) & " 異常"
            Else
              row(i + 4) = DyeStepNumber(i) & ": " & DyeCode(i) & ": " & DyeGrams(i) & "/" & DyeDispenseGrams(i)
            End If
          Else
            row(i + 4) = ""
          End If
        Next


      Case 4  '呼叫助劑訊息

        row(1) = "領料單號: " & Parent.Job & " 操作員:" & LoadOperator

        '顯示LA-302F狀態

        If ChemicalState = 201 Then
          row(2) = "步驟" & ChemicalCallOff & " " & "助劑呼叫中"
        ElseIf ChemicalState = 202 Then
          row(2) = "步驟" & ChemicalCallOff & " " & "助劑輸送中"
        Else
          row(2) = ""
        End If

        If ChemicalStepReady(1) Or _
           ChemicalStepReady(2) Or _
           ChemicalStepReady(3) Or _
           ChemicalStepReady(4) Or _
           ChemicalStepReady(5) Or _
           ChemicalStepReady(6) Or _
           ChemicalStepReady(7) Or _
           ChemicalStepReady(8) Or _
           ChemicalStepReady(9) Or _
           ChemicalStepReady(10) Or _
           ChemicalStepReady(11) Or _
           ChemicalStepReady(12) Then
          row(3) = "步驟" & If(ChemicalStepReady(1), "1 ", "") & _
          If(ChemicalStepReady(2), "2 ", "") & _
          If(ChemicalStepReady(3), "3 ", "") & _
          If(ChemicalStepReady(4), "4 ", "") & _
          If(ChemicalStepReady(5), "5 ", "") & _
          If(ChemicalStepReady(6), "6 ", "") & _
          If(ChemicalStepReady(7), "7 ", "") & _
          If(ChemicalStepReady(8), "8 ", "") & _
          If(ChemicalStepReady(9), "9 ", "") & _
          If(ChemicalStepReady(10), "10 ", "") & _
          If(ChemicalStepReady(11), "11 ", "") & _
          If(ChemicalStepReady(12), "12 ", "") & _
          Translations.Translate("助劑計量完成")
        Else
          row(3) = ""
        End If

        '        Dim Chemicals() As String = GetProductsFromString(ChemicalProducts)
        '        For i As Integer = 4 To 20 : row(i) = Nothing : Next
        '
        '        If (ChemicalProducts IsNot Nothing) AndAlso (ChemicalProducts.Length > 2) Then
        ' For i As Integer = 0 To 15
        ' row(i + 4) = Chemicals(i)
        ' Next
        ' End If
        For i As Integer = 0 To 14
          If ChemicalCode(i) IsNot Nothing Then
            If ChemicalDispenseResult(i) = "301" Then
              row(i + 4) = ChemicalStepNumber(i) & ": " & ChemicalCode(i) & ": " & ChemicalGrams(i) & "/" & ChemicalDispenseGrams(i) & " 正常"
            ElseIf ChemicalDispenseResult(i) = "302" Then
              row(i + 4) = ChemicalStepNumber(i) & ": " & ChemicalCode(i) & ": " & ChemicalGrams(i) & "/" & ChemicalDispenseGrams(i) & " 手動"
            ElseIf ChemicalDispenseResult(i) = "309" Then
              row(i + 4) = ChemicalStepNumber(i) & ": " & ChemicalCode(i) & ": " & ChemicalGrams(i) & "/" & ChemicalDispenseGrams(i) & " 異常"
            Else
              row(i + 4) = ChemicalStepNumber(i) & ": " & ChemicalCode(i) & ": " & ChemicalGrams(i) & "/" & ChemicalDispenseGrams(i)
            End If
          Else
            row(i + 4) = ""
          End If
        Next


      Case 5 '能源使用統計
        row(1) = "用電量 : " & PowerUsed & " KWhrs"
        row(2) = "用蒸汽量 : " & SteamUsed & " Kgs"
        row(3) = "用水量 : " & WaterUsed & " Liters"

    End Select
  End Sub

  Public Sub ProgramStart() Implements ACControlCode.ProgramStart
    StartUserLogin = True
    PumpControl.PumpControl_PumpSpeedAdjustFinish = False
    PumpControl.PumpControl_StartAdjustCycleTime = False
    PumpControl.Cancel()

    Dyelot_ActualMainPumpSpeedUpdated = False
    染色水量統計開始 = False
    染色水量統計結束 = False
    PhCirRun = True
    TotalWaterLiters = 0
    TotalWaterLitersWas = 0
    HighSpeedCounter1 = IO.HSCounter1
    HighSpeedCounter1Was = IO.HSCounter1

    IO.CounterReset = False

    'Reset dispense variables
    ChemicalCallOff = 0
    ChemicalState = 0
    ChemicalTank = 0
    ChemicalProducts = ""
    DyeCallOff = 0
    DyeState = 0
    DyeTank = 0
    DyeProducts = ""
    LA252Ready = False
    SPCConnectError = False
    ProgramStopCleanDatabase = False

    '清除計量狀態的陣列
    Array.Clear(DyeStepDispensing, 0, 12)
    Array.Clear(DyeStepReady, 0, 12)
    Array.Clear(ChemicalStepDispensing, 0, 12)
    Array.Clear(ChemicalStepReady, 0, 12)

    '清除配方的陣列
    Array.Clear(StepNumber, 0, 30)
    Array.Clear(ProductCode, 0, 30)
    Array.Clear(ProductType, 0, 30)
    Array.Clear(Grams, 0, 30)
    Array.Clear(DispenseGrams, 0, 30)
    Array.Clear(DispenseResult, 0, 30)
    '清除染料的陣列
    Array.Clear(DyeStepNumber, 0, 10)
    Array.Clear(DyeCode, 0, 10)
    Array.Clear(DyeGrams, 0, 10)
    Array.Clear(DyeDispenseGrams, 0, 10)
    Array.Clear(DyeDispenseResult, 0, 10)
    '清除助劑的陣列
    Array.Clear(ChemicalStepNumber, 0, 20)
    Array.Clear(ChemicalCode, 0, 20)
    Array.Clear(ChemicalGrams, 0, 20)
    Array.Clear(ChemicalDispenseGrams, 0, 20)
    Array.Clear(ChemicalDispenseResult, 0, 20)

    纏車次數計數器 = 0
    MainPumpStartWas = False
    MainPumpStopWas = False
    HeatStartWas = False
    HeatStopWas = False
    CoolStartWas = False
    CoolStopWas = False
    DrainStartWas = False
    DrainStopWas = False

    Dim dyelot = Parent.Job
    Dim f = Parent.Job.IndexOf("@")
    If f <> -1 Then
      工單 = dyelot.Substring(0, f)
      Integer.TryParse(dyelot.Substring(f + 1), 重染)
    Else
      工單 = dyelot
      重染 = 0
    End If
    DyelotsState()
    PowerKWHrs = 0
    PowerKWS = 0
    SteamUsed = 0
    SteamUsedKgs = 0
    WaterUsed = 0



  End Sub

  Public Sub ProgramStop() Implements ACControlCode.ProgramStop
    PumpControl.Cancel()
    StartUserLogin = False
    UserLoginOK = False
    PumpControl.PumpControl_PumpSpeedAdjustFinish = False
    StartSimulateFillCheck = False
    SimulateFillCheckFinish = False
    Dyelot_ActualMainPumpSpeedUpdated = False
    PhCirRun = False
    TotalWaterLiters = 0
    IO.CounterReset = True
    預計時間 = 0
    預計時間小時 = 0
    預計時間分鐘 = 0
    CTankPrepare.Cancel()
    ManualDose.Cancel()
    CallLA252.Cancel()
    TemperatureControlFlag = False
    HeatNow = False
    CoolNow = False
    ReelStopRequest = False
    PumpStartRequest = False
    PumpStopRequest = False
    LA252Ready = False
    PumpOn = False
    Step1Ready = False : Step2Ready = False : Step4Ready = False
    Step5Ready = False : Step6Ready = False
    Step1Dispensing = False : Step2Dispensing = False : Step4Dispensing = False
    Step5Dispensing = False : Step6Dispensing = False

    'Reset dispense variables
    ChemicalCallOff = 0
    ChemicalState = 101
    ChemicalTank = 0
    ChemicalProducts = ""
    DyeCallOff = 0
    DyeState = 101
    DyeTank = 0
    DyeProducts = ""

    SPCConnectError = False

    '清除計量狀態的陣列
    Array.Clear(DyeStepDispensing, 0, 12)
    Array.Clear(DyeStepReady, 0, 12)
    Array.Clear(ChemicalStepDispensing, 0, 12)
    Array.Clear(ChemicalStepReady, 0, 12)

    '清除配方的陣列
    Array.Clear(StepNumber, 0, 30)
    Array.Clear(ProductCode, 0, 30)
    Array.Clear(ProductType, 0, 30)
    Array.Clear(Grams, 0, 30)
    Array.Clear(DispenseGrams, 0, 30)
    Array.Clear(DispenseResult, 0, 30)
    '清除染料的陣列
    Array.Clear(DyeStepNumber, 0, 10)
    Array.Clear(DyeCode, 0, 10)
    Array.Clear(DyeGrams, 0, 10)
    Array.Clear(DyeDispenseGrams, 0, 10)
    Array.Clear(DyeDispenseResult, 0, 10)
    '清除助劑的陣列
    Array.Clear(ChemicalStepNumber, 0, 20)
    Array.Clear(ChemicalCode, 0, 20)
    Array.Clear(ChemicalGrams, 0, 20)
    Array.Clear(ChemicalDispenseGrams, 0, 20)
    Array.Clear(ChemicalDispenseResult, 0, 20)
    ProgramStopCleanDatabase = True
    WaterUsed = SimulateTotalWaterLiters
    Try
      Dim UpdateDyelotData As String
      UpdateDyelotData = "update dyelots set PowerUsed = '" & PowerUsed &
        "', SteamUsed='" & SteamUsed &
        "', WaterUsed='" & WaterUsed &
        "', ActualMainPumpSpeed='" & Dyelot_ActualMainPumpSpeed &
        "', ActualReel1Speed='" & Dyelot_ActualReel1Speed &
        "', ActualReel2Speed='" & Dyelot_ActualReel2Speed &
        "', ActualCycleTime1='" & Dyelot_ActualCycleTime1 &
        "', ActualCycleTime2='" & Dyelot_ActualCycleTime2 &
        "', ActualCycleTime3='" & Dyelot_ActualCycleTime3 &
        "', ActualCycleTime4='" & Dyelot_ActualCycleTime4 &
        "'   where dyelot = '" & 工單 & "' and redye = '" & 重染 & "'"
      Dim dt As System.Data.DataTable = Parent.DbGetDataTable(UpdateDyelotData)
    Catch ex As Exception
    End Try

    PowerKWHrs = 0
    PowerKWS = 0
    SteamUsed = 0
    SteamUsedKgs = 0
    WaterUsed = 0
    SimulateTotalWaterLiters = 0
    SimulateFillWaterLiters = 0
    'Dim myPath As String
    'myPath = Application.StartupPath
    'If My.Computer.FileSystem.FileExists("C:\LA868Update\HJDOS109.dll") Then
    'System.Diagnostics.Process.Start(myPath + "\Update868.bat")
    'Else
    'End If
  End Sub

  '  <GraphTrace(0, 20000, 6000, 9500, "Yellow", "0公升")>
  '  Public ReadOnly Property 總水量() As Integer
  '  Get
  '  Return TotalWaterLiters   ' return whole degrees
  '  End Get
  '  End Property

  ' <GraphTrace(0, 20000, 6000, 9500, "Green", "0公升")>
  ' Public ReadOnly Property 進水量() As Integer
  ' Get
  ' Return FillWaterLiters   ' return whole degrees
  ' End Get
  ' End Property

  <GraphTrace(0, 20000, 6000, 9500, "Yellow", "0公升")>
  Public ReadOnly Property 模擬總水量() As Integer
    Get
      Return SimulateTotalWaterLiters   ' return whole degrees
    End Get
  End Property

  <GraphTrace(0, 20000, 6000, 9500, "Green", "0公升")>
  Public ReadOnly Property 模擬進水量() As Integer
    Get
      Return SimulateFillWaterLiters   ' return whole degrees
    End Get
  End Property


  <GraphTrace(0, 1)>
    Public ReadOnly Property 警報() As String
        Get
            Return Parent.ActiveAlarms
        End Get
    End Property


  <GraphTrace(0, 1500, 6000, 9500, "Red", "%tC"), GraphLabel("0C", 0), GraphLabel("50C", 500), GraphLabel("100C", 1000), GraphLabel("150C", 1500), Translate("zh-TW", "主缸溫度")>
  Public ReadOnly Property Temperature() As Integer
    Get
      Return (IO.MainTemperature)   ' return whole degrees
    End Get
  End Property

  Public ReadOnly Property CurrentTime() As String
    Get
      Return Date.Now.ToLongTimeString()
    End Get
  End Property

  Public ReadOnly Property Status() As String
    Get
      If Parent.Signal <> "" And FlashFlag Then Return Parent.Signal
      If ManualDose.IsOn Then
        Return ManualDose.StateString
      ElseIf ManualOperation Then
        Return Translations.Translate("System Manual")
      ElseIf Command10.IsOn Then
        Return Command10.StateString
      ElseIf Command26.IsOn Then
        Return Command26.StateString
      ElseIf Command01.IsOn Then
        Return Command01.StateString
      ElseIf (Command74.IsOn Or Command75.IsOn Or Command76.IsOn Or Command77.IsOn) And PhWash.IsOn And FlashFlag Then
        Return PhWash.StateString

      ElseIf Command73.IsOn Then
        If PhWash.IsOn And FlashFlag2 Then
          Return PhWash.StateString
        Else
          Return Command73.StateString
        End If
      ElseIf Command74.IsOn Then
        If PhControl.IsOn And FlashFlag2 Then
          Return PhControl.StateString
        Else
          Return Command74.StateString
        End If

      ElseIf Command75.IsOn Then
        Return Command75.StateString
      ElseIf Command76.IsOn Then
        If PhControl.IsOn And FlashFlag2 Then
          Return PhControl.StateString
        Else
          Return Command76.StateString
        End If
      ElseIf Command77.IsOn Then
        If PhControl.IsOn And FlashFlag2 Then
          Return PhControl.StateString
        Else
          Return Command77.StateString
        End If
      ElseIf Command78.IsOn Then
        If PhControl.IsOn And FlashFlag2 Then
          Return PhControl.StateString
        Else
          Return Command78.StateString
        End If
      ElseIf Command79.IsOn Then
        If PhControl.IsOn And FlashFlag2 Then
          Return PhControl.StateString
        Else
          Return Command79.StateString
        End If
      ElseIf Command80.IsOn Then
        If PhControl.IsOn And FlashFlag2 Then
          Return PhControl.StateString
        Else
          Return Command80.StateString
        End If
      ElseIf Command02.IsOn Then
        Return Command02.StateString
      ElseIf Command03.IsOn Then
        Return Command03.StateString
      ElseIf Command04.IsOn Then
        Return Command04.StateString
      ElseIf Command11.IsOn Then
        Return Command11.StateString
      ElseIf Command12.IsOn Then
        Return Command12.StateString
      ElseIf Command13.IsOn Then
        Return Command13.StateString
      ElseIf Command14.IsOn Then
        Return Command14.StateString
      ElseIf Command15.IsOn Then
        Return Command15.StateString
      ElseIf Command16.IsOn Then
        Return Command16.StateString
      ElseIf Command17.IsOn Then
        Return Command17.StateString
      ElseIf Command24.IsOn Then
        Return Command24.StateString
      ElseIf Command25.IsOn Then
        Return Command25.StateString
      ElseIf Command32.IsOn Then
        Return Command32.StateString
      ElseIf Command54.IsOn Then
        If Command55.IsOn Or Command65.IsOn Or Command52.IsActive Or Command57.IsActive Then
          If Command55.IsOn And FlashFlag Then
            Return Command55.StateString
          ElseIf Command65.IsOn And FlashFlag Then
            Return Command65.StateString
          ElseIf Command52.IsActive And FlashFlag Then
            Return Command52.StateString
          ElseIf Command57.IsActive And FlashFlag Then
            Return Command57.StateString
          Else
            Return Command54.StateString
          End If
        Else
          Return Command54.StateString
        End If
        '   ElseIf CTankPrepare.IsOn Then
        '    If Command54.IsOn Or Command64.IsOn Or Command51.IsActive Or Command56.IsActive Then
        'If Command54.IsOn And FlashFlag Then
        '   Return Command54.StateString
        ' ElseIf Command64.IsOn And FlashFlag Then
        '   Return Command64.StateString
        ' ElseIf Command51.IsActive And FlashFlag Then
        '   Return Command51.StateString
        ' ElseIf Command56.IsActive And FlashFlag Then
        '   Return Command56.StateString
        ' Else
        '   Return CTankPrepare.StateString
        ' End If
        ' Else
        '   Return CTankPrepare.StateString
        ' End If
      ElseIf Command64.IsOn Then
        If Command55.IsOn Or Command65.IsOn Or Command52.IsActive Or Command57.IsActive Then
          If Command55.IsOn And FlashFlag Then
            Return Command55.StateString
          ElseIf Command65.IsOn And FlashFlag Then
            Return Command65.StateString
          ElseIf Command52.IsActive And FlashFlag Then
            Return Command52.StateString
          ElseIf Command57.IsActive And FlashFlag Then
            Return Command57.StateString
          Else
            Return Command64.StateString
          End If
        Else
          Return Command64.StateString
        End If
      ElseIf Command65.IsOn Then
        If Command54.IsOn Or Command64.IsOn Or Command51.IsActive Or Command56.IsActive Then
          If Command54.IsOn And FlashFlag Then
            Return Command54.StateString
          ElseIf Command64.IsOn And FlashFlag Then
            Return Command64.StateString
          ElseIf Command51.IsActive And FlashFlag Then
            Return Command51.StateString
          ElseIf Command56.IsActive And FlashFlag Then
            Return Command56.StateString
          Else
            Return Command65.StateString
          End If
        Else
          Return Command65.StateString
        End If
      ElseIf Command51.IsActive Then
        If Command52.IsOn Or Command56.IsOn Or Command57.IsOn Then
          If Command52.IsOn And FlashFlag Then
            Return Command52.StateString
          ElseIf Command56.IsOn And FlashFlag Then
            Return Command56.StateString
          ElseIf Command57.IsOn And FlashFlag Then
            Return Command57.StateString
          Else
            Return Command51.StateString
          End If
        Else
          Return Command51.StateString
        End If
      ElseIf Command52.IsActive Then
        If Command51.IsOn Or Command56.IsOn Or Command57.IsOn Then
          If Command51.IsOn And FlashFlag Then
            Return Command51.StateString
          ElseIf Command56.IsOn And FlashFlag Then
            Return Command56.StateString
          ElseIf Command57.IsOn And FlashFlag Then
            Return Command57.StateString
          Else
            Return Command52.StateString
          End If
        Else
          Return Command52.StateString
        End If
      ElseIf Command56.IsActive Then
        If Command51.IsOn Or Command52.IsOn Or Command57.IsOn Then
          If Command51.IsOn And FlashFlag Then
            Return Command51.StateString
          ElseIf Command52.IsOn And FlashFlag Then
            Return Command52.StateString
          ElseIf Command57.IsOn And FlashFlag Then
            Return Command57.StateString
          Else
            Return Command56.StateString
          End If
        Else
          Return Command56.StateString
        End If
      ElseIf Command57.IsOn Then
        If Command51.IsOn Or Command56.IsOn Or Command52.IsOn Then
          If Command51.IsOn And FlashFlag Then
            Return Command51.StateString
          ElseIf Command56.IsOn And FlashFlag Then
            Return Command56.StateString
          ElseIf Command52.IsOn And FlashFlag Then
            Return Command52.StateString
          Else
            Return Command57.StateString
          End If
        Else
          Return Command57.StateString
        End If
      ElseIf Command58.IsOn Then
        If Command58.IsNotReady And FlashFlag Then
          Return Command58.StateString
        ElseIf Command58.IsNotEmpty And FlashFlag Then
          Return Command58.StateString
        End If
      ElseIf Not Parent.IsProgramRunning Then
        Return Translations.Translate("Idle") & ":" & TimerString(ProgramStoppedTimer.TimeElapsed)
      End If
      Return ""
    End Get
  End Property

  '  Private Sub GetRecipeData()
  'Get recipe data... data should be in the following format in the column RecipeProducts
  '  step,product1Code,product1Name,grams;step,product2Code,product2Name,grams;step,product3Code,product3Name,grams etc...
  '   Try
  'Get the current (running) dyelot from the local database (state=2) 
  'Dim dt As System.Data.DataTable = Parent.DbGetDataTable("SELECT * FROM Dyelots WHERE State=2")
  '    If dt IsNot Nothing AndAlso dt.Rows.Count = 1 Then
  '     Recipe.Load(dt.Rows(0))
  '   End If
  ' Catch ex As Exception
  '   Parent.LogException(ex)
  ' End Try
  'End Sub

  '  Private Function GetProductsFromString(ByVal productString As String) As String()
  '    Try
  '  Dim products() As String
  '      products = productString.Split("|".ToCharArray)
  '      Return products
  '
  '    Catch ex As Exception
  '      Parent.LogException(ex)
  '    End Try
  '    Return Nothing
  '  End Function

  Public Sub ConnectBDC()
    Try
      SPCConnectError = True
      Using cn_Recipe As New SqlClient.SqlConnection("data source=" & SPCServerName & ";initial catalog=BatchDyeingCentral;user id= " & SPCUserName & ";password=" & SPCPassword & "")
        cn_Recipe.Open()
        Dim qs_Recipe As New SqlClient.SqlCommand("SELECT Dyelot, ReDye, StepNumber, ProductCode, ProductType, Grams, DispenseGrams, DispenseResult FROM DyelotsBulkedRecipe WHERE (Dyelot = '" & 工單 & "') AND (ReDye = '" & 0 & "') AND (ProductCode <> '') ORDER BY StepNumber, ID", cn_Recipe)
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0
        Using RecipeReader As SqlClient.SqlDataReader = qs_Recipe.ExecuteReader
          Do While RecipeReader.Read
            StepNumber(i) = RecipeReader.GetInt32(2).ToString
            ProductCode(i) = RecipeReader.GetString(3)
            ProductType(i) = RecipeReader.GetInt32(4).ToString
            Grams(i) = RecipeReader.GetDouble(5).ToString
            If Not RecipeReader.IsDBNull(6) Then
              DispenseGrams(i) = RecipeReader.GetDouble(6).ToString
              DispenseResult(i) = RecipeReader.GetInt32(7).ToString
            End If
            If ProductType(i) = "1" Then
              DyeStepNumber(j) = StepNumber(i)
              DyeCode(j) = ProductCode(i)
              DyeGrams(j) = Grams(i)
              DyeDispenseGrams(j) = DispenseGrams(i)
              DyeDispenseResult(j) = DispenseResult(i)
              j = j + 1
            Else
              ChemicalStepNumber(k) = StepNumber(i)
                ChemicalCode(k) = ProductCode(i)
                ChemicalGrams(k) = Grams(i)
                ChemicalDispenseGrams(k) = DispenseGrams(i)
                ChemicalDispenseResult(k) = DispenseResult(i)
                k = k + 1
              End If
            i = i + 1
          Loop
        End Using
        SPCConnectError = False
      End Using
    Catch ex As Exception
      'Ignore errors
    End Try
  End Sub

  Public Sub DispenseState()
    Try
      SPCConnectError = True
      ProgramStopCleanDatabase = False
      ComputerName = System.Environment.MachineName

      Using cn_DispenseState As New SqlClient.SqlConnection("data source=" & SPCServerName & ";initial catalog=BatchDyeingCentral;user id= " & SPCUserName & ";password=" & SPCPassword & "")
        cn_DispenseState.Open()
        Dim qs_DispenseState As New SqlClient.SqlCommand("SELECT Name, DispenseDyelot, DispenseReDye, ChemicalCallOff, ChemicalState, ChemicalTank, ChemicalEnabled, DyeCallOff, DyeState, DyeTank, DyeEnabled FROM Machines WHERE (Name = '" & ComputerName & "') ", cn_DispenseState)
        Using DispenseStateReader As SqlClient.SqlDataReader = qs_DispenseState.ExecuteReader
          Do While DispenseStateReader.Read
            CCallOff = DispenseStateReader.GetInt32(3).ToString
            CState = DispenseStateReader.GetInt32(4).ToString
            CTank = DispenseStateReader.GetInt32(5).ToString
            CEnabled = DispenseStateReader.GetInt32(6).ToString
            DCallOff = DispenseStateReader.GetInt32(7).ToString
            DState = DispenseStateReader.GetInt32(8).ToString
            DTank = DispenseStateReader.GetInt32(9).ToString
            DEnabled = DispenseStateReader.GetInt32(10).ToString
          Loop
        End Using
        '助劑呼叫的規則
        If ProgramStopCleanDatabase Then
            Dim cmd_DispenseState As New SqlClient.SqlCommand("UPDATE Machines SET DispenseDyelot=null, DispenseReDye=0 " &
                                                             ", ChemicalCallOff = 0, ChemicalTank=0, ChemicalState='101', DyeCallOff =0, DyeTank=0, DyeState='101'" &
                                                             " WHERE (Name = '" & ComputerName & "')", cn_DispenseState)
            cmd_DispenseState.ExecuteNonQuery()
          Else
            If ChemicalCallOff = 0 And ChemicalTank = 0 Then
              ChemicalState = 101
              Dim cmd_DispenseState As New SqlClient.SqlCommand("UPDATE Machines SET DispenseDyelot='" & 工單 & "', DispenseReDye='" & 0 &
                                                                 "', ChemicalCallOff = " & ChemicalCallOff & ", ChemicalTank=" & ChemicalTank & ", ChemicalState=" & ChemicalState &
                                                                 " WHERE (Name = '" & ComputerName & "')", cn_DispenseState)
              cmd_DispenseState.ExecuteNonQuery()
            ElseIf ChemicalCallOff <> 0 And ChemicalTank <> 0 And CState = "101" And CEnabled = "1" Then
              ChemicalState = 201
              Dim cmd_DispenseState As New SqlClient.SqlCommand("UPDATE Machines SET DispenseDyelot='" & 工單 & "', DispenseReDye='" & 0 &
                                                                 "', ChemicalCallOff = " & ChemicalCallOff & ", ChemicalTank=" & ChemicalTank & ", ChemicalState=" & ChemicalState &
                                                                 " WHERE (Name = '" & ComputerName & "')", cn_DispenseState)
              cmd_DispenseState.ExecuteNonQuery()
            ElseIf ChemicalCallOff <> 0 And ChemicalTank <> 0 And CState = "202" And CEnabled = "1" Then
              ChemicalState = 202
              For i As Integer = 0 To 15
                If ChemicalCode(i) IsNot Nothing Then
                  If ChemicalStepNumber(i) = ChemicalCallOff.ToString Then
                    If ChemicalDispenseResult(i) = "309" Or ChemicalState = 309 Then
                      ChemicalState = 309
                    ElseIf ChemicalDispenseResult(i) = "302" Or ChemicalState = 302 Then
                      ChemicalState = 302
                    ElseIf ChemicalDispenseResult(i) = "301" Or ChemicalState = 301 Then
                      ChemicalState = 301
                    End If
                  End If
                End If
              Next
              If ChemicalState = 301 Or ChemicalState = 302 Or ChemicalState = 309 Then
                Dim cmd_DispenseState As New SqlClient.SqlCommand("UPDATE Machines SET DispenseDyelot='" & 工單 & "', DispenseReDye='" & 0 &
                                                                     "', ChemicalCallOff = " & ChemicalCallOff & ", ChemicalTank=" & ChemicalTank & ", ChemicalState=" & ChemicalState &
                                                                     " WHERE (Name = '" & ComputerName & "')", cn_DispenseState)
                cmd_DispenseState.ExecuteNonQuery()
              End If
            ElseIf ChemicalCallOff <> 0 And ChemicalTank <> 0 And CState = "301" And CEnabled = "1" Then
              ChemicalState = 301
            ElseIf ChemicalCallOff <> 0 And ChemicalTank <> 0 And CState = "302" And CEnabled = "1" Then
              ChemicalState = 302
            ElseIf ChemicalCallOff <> 0 And ChemicalTank <> 0 And CState = "309" And CEnabled = "1" Then
              ChemicalState = 309
            End If

            '染料呼叫的規則
            If DyeCallOff = 0 And DyeTank = 0 And ((DState = "") Or (DState = "101") Or (DState = "301") Or (DState = "302") Or (DState = "309") Or (DyeState = 101)) Then
              DyeState = 101
              Dim cmd_DispenseState As New SqlClient.SqlCommand("UPDATE Machines SET DispenseDyelot='" & 工單 & "', DispenseReDye='" & 0 &
                                                                 "', DyeCallOff = " & DyeCallOff & ", DyeTank=" & DyeTank & ", DyeState=" & DyeState &
                                                                 " WHERE (Name = '" & ComputerName & "')", cn_DispenseState)
              cmd_DispenseState.ExecuteNonQuery()
            ElseIf DyeCallOff <> 0 And DyeTank <> 0 And DState = "101" And DEnabled = "1" Then
              DyeState = 201
              Dim cmd_DispenseState As New SqlClient.SqlCommand("UPDATE Machines SET DispenseDyelot='" & 工單 & "', DispenseReDye='" & 0 &
                                                                 "', DyeCallOff = " & DyeCallOff & ", DyeTank=" & DyeTank & ", DyeState=" & DyeState &
                                                                 " WHERE (Name = '" & ComputerName & "')", cn_DispenseState)
              cmd_DispenseState.ExecuteNonQuery()
            ElseIf DyeCallOff <> 0 And DyeTank <> 0 And DState = "202" And DEnabled = "1" Then
              DyeState = 202
            ElseIf DyeCallOff <> 0 And DyeTank <> 0 And DState = "301" And DEnabled = "1" Then
              DyeState = 301
            ElseIf DyeCallOff <> 0 And DyeTank <> 0 And DState = "302" And DEnabled = "1" Then
              DyeState = 302
            ElseIf DyeCallOff <> 0 And DyeTank <> 0 And DState = "309" And DEnabled = "1" Then
              DyeState = 309
            End If
          End If
          SPCConnectError = False
        End Using
    Catch ex As Exception
      'Ignore errors
    End Try
  End Sub

  Public Sub DyelotsState()
        Try
            SPCConnectError = True
            ComputerName = System.Environment.MachineName

      Using cn_Dyelots As New SqlClient.SqlConnection("data source=" & SPCServerName & ";initial catalog=BatchDyeingCentral;user id= " & SPCUserName & ";password=" & SPCPassword & "")
        cn_Dyelots.Open()
        Dim qs_Dyelots As New SqlClient.SqlCommand("SELECT StandardTime, TotalWeight, LiquidRatio, TotalVolume,BatchNumberAfterRinse  FROM Dyelots WHERE (Dyelot = '" & 工單 & "') AND (ReDye = '" & 重染 & "') ", cn_Dyelots)
        Using DyelotsReader As SqlClient.SqlDataReader = qs_Dyelots.ExecuteReader
          Do While DyelotsReader.Read
            預計時間 = DyelotsReader.GetDouble(0)
            Dyelot_Weight = DyelotsReader.GetDouble(1)
            Dyelot_LiquidRatio = CType(DyelotsReader.GetString(2), Integer)
            Dyelot_TotalVolume = CType(DyelotsReader.GetString(3), Integer)
            BatchNumberAfterRinse = CType(DyelotsReader.GetInt32(4), Integer)
          Loop
        End Using
      End Using
      預計時間小時 = CType(Decimal.Truncate(CType(預計時間 * 24, Decimal)), Integer)
      預計時間分鐘 = CType(預計時間 * 24 * 60 - 預計時間小時 * 60, Integer)
      If 預計時間分鐘 <= 0 Then 預計時間分鐘 = 0
      SPCConnectError = False

    Catch ex As Exception
            'Ignore errors
        End Try
  End Sub

End Class

Public Enum LanguageValue
  English
  ZhTW
  ZhCN
End Enum