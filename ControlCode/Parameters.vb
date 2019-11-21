Public NotInheritable Class Parameters
  Inherits MarshalByRefObject

  '流量計校正參數
  <Translate("zh-TW", "FC:低水位進水量"), Category("Main Tank"),
   TranslateCategory("zh-TW", "流量計校正參數"),
  Description("執行流量計校正後的低水位進水量")> Public LowLevelLiters As Integer

  <Translate("zh-TW", "FC:中水位進水量"), Category("Main Tank"),
   TranslateCategory("zh-TW", "流量計校正參數"),
  Description("執行流量計校正後的中水位進水量")> Public MiddleLevelLiters As Integer

  <Translate("zh-TW", "FC:高水位進水量"), Category("Main Tank"),
   TranslateCategory("zh-TW", "流量計校正參數"),
  Description("執行流量計校正後的高水位進水量")> Public HighLevelLiters As Integer

  '主缸參數

  <Translate("zh-TW", "MT:漂後缸數 紅色"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數"),
  Description("如果超過漂後缸數則發出警報")> Public RedBatchNumberAfterRinse As Integer = 6

  <Translate("zh-TW", "MT:漂後缸數 藍色"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數"),
  Description("如果超過漂後缸數則發出警報")> Public BlueBatchNumberAfterRinse As Integer = 6

  <Translate("zh-TW", "MT:漂後缸數 其他顏色"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數"),
  Description("如果超過漂後缸數則發出警報")> Public OthersBatchNumberAfterRinse As Integer = 6

  <Translate("zh-TW", "MT:主缸進水高水位異常時間"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數"),
  Description("主缸進水時，如果超過設定時間但是沒有偵測到高水位則發出警報，單位秒")> Public FillOverTime As Integer = 300

  <Translate("zh-TW", "MT:溢流水洗後等待溢流排水時間"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數"),
  Description("設定溢流水洗後，等待溢流排水的時間，單位秒")> Public OverflowTimeAfterRinse As Integer = 10

  <Translate("zh-TW", "MT:溢流水洗補水時間"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數"),
  Description("溢流水洗時，進水到中水位即停止進水，等待中水位消失且補水時間到達後開始進水")> Public OverflowWaitLevelTime As Integer = 10

  <Translate("zh-TW", "MT:主缸進水超時時間"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數"),
  Description("設定主缸進水超時時間，進水時間超過設定值即發出警報，單位分鐘")> Public MainTankFillDelayTimeMinute As Integer = 5

  <Translate("zh-TW", "MT:主缸流量進水低水位水量"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數"),
  Description("設定主缸低水位水量，用來計算每秒進水量用")> Public MainTankLowLevelLiters As Integer = 1000


  <Translate("zh-TW", "MT:主缸進水最低水量"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數"),
  Description("設定主缸流量進水的最低水量")> Public MainTankFillMinVolume As Integer

  <Translate("zh-TW", "MT:流量進水扣除藥缸水量"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數"),
  Description("設定藥缸加藥的水量，用來作為流量進水時的預扣量")> Public AdditionVolume As Integer

  <Translate("zh-TW", "MT:主缸排水延遲時間秒"), Category("Main Tank"), _
   TranslateCategory("zh-TW", "主缸參數"), _
   Description("When system runs main tank drain function, it will start counting delay time as main tank low level is off. The system will not stop drain until delay time up."), _
   TranslateDescription("zh-TW", "當主缸執行排水功能，若主缸低水位訊號Low，則開始計算延遲時間，延遲時間到達即結束排水")> Public DrainDelay As Integer

  <Translate("zh-TW", "MT:主缸熱交換器排水時間秒"), Category("Main Tank"), _
   TranslateCategory("zh-TW", "主缸參數"), _
  Description("主缸升溫時會先開啟熱交換器排水閥，待設定時間到達則將熱交換器排水閥關閉，開啟排冷凝水閥")> Public Condensation As Integer

  <Translate("zh-TW", "MT:主缸排壓安全溫度度"), Category("Main Tank"), _
   TranslateCategory("zh-TW", "主缸參數"), _
  Description("降溫時，當主缸溫度低於設定值，則開始進行點放排壓動作")> Public SetPressureOutTemp As Integer

  <Translate("zh-TW", "MT:主缸安全溫度度"), Category("Main Tank"), _
   TranslateCategory("zh-TW", "主缸參數"), _
  Description("當主缸溫度高於設定值，則不允許進水、排水、溢流動作")> Public SetSafetyTemp As Integer

  <Translate("zh-TW", "MT:主缸加藥安全溫度度"), Category("Main Tank"), _
   TranslateCategory("zh-TW", "主缸參數"), _
  Description("當主缸溫度高於設定值，則不允許加藥動作")> Public SetAddSafetyTemp As Integer

  <Translate("zh-TW", "MT:是否有動力排水閥0是1否"), Category("Main Tank"), _
   TranslateCategory("zh-TW", "主缸參數"), _
  Description("設定是否有動力排水閥，如果設定為否，則當動力排水時，不啟動主馬達及動力排水閥，只啟動排水閥")> Public SetPowerDrain As Integer

  <Translate("zh-TW", "MT:帶布輪啟動延遲時間秒"), Category("Main Tank"), _
   TranslateCategory("zh-TW", "主缸參數"), _
  Description("當主馬達啟動後，即開始計算延遲時間，時間到達時即啟動帶布輪正轉")> Public ReelStartDelayTime As Integer

  <Translate("zh-TW", "MT:帶布輪停止延遲時間秒"), Category("Main Tank"), _
 TranslateCategory("zh-TW", "主缸參數"), _
Description("當主馬達停止時，先停止帶布輪，然後開始計算延遲時間，時間到達時停止主馬達")> Public ReelStopDelayTime As Integer

  <Translate("zh-TW", "MT:主泵自動控制檢查帶布輪號碼"), Category("Main Tank"),
 TranslateCategory("zh-TW", "主缸參數"),
Description("當主泵自動控制速度時，檢查第幾管的CycleTime")> Public PumpSpeedAdjustCheckCycleNo As Integer

  <Translate("zh-TW", "MT:入降溫回收水超時時間"), Category("Main Tank"), _
 TranslateCategory("zh-TW", "主缸參數"), _
Description("當入降溫回收水時，如果在設定的時間內沒有到達水量，則改成開入冷水和入熱水，單位為分鐘")> Public CoolingWaterTimeOut As Integer = 5

  <Translate("zh-TW", "MT:Liter Per Counter"), Category("Main Tank"), _
   TranslateCategory("zh-TW", "主缸參數")> Public VolumePerCount As Integer

  <Translate("zh-TW", "MT:熱交排壓設定Allno0C1P2Allyes3"), Category("Main Tank"), _
   TranslateCategory("zh-TW", "主缸參數")> Public ConPresOutAllno0C1P2Allyes3 As Integer

  <Translate("zh-TW", "MT:動力排水延遲時間秒"), Category("Main Tank"), _
     TranslateCategory("zh-TW", "主缸參數")> Public PowerDrainDelay As Integer

  <Translate("zh-TW", "MT:降溫回收進水0否1是"), Category("Main Tank"), _
   TranslateCategory("zh-TW", "主缸參數")> Public ReCycleWater As Integer

  <Translate("zh-TW", "MT:昇溫閥種類，0數位，1類比"), Category("Main Tank"), _
     TranslateCategory("zh-TW", "主缸參數")> Public HeatValveType As Integer

  <Translate("zh-TW", "MT:降溫閥種類，0數位，1類比"), Category("Main Tank"), _
     TranslateCategory("zh-TW", "主缸參數")> Public CoolValveType As Integer

  <Translate("zh-TW", "MT:升溫中是否開排壓,0否,1是"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數")> Public PressureOutWhenHeating As Integer = 0

  <Translate("zh-TW", "MT:升溫中是否開加壓,0否,1是"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數")> Public PressureInWhenHeating As Integer = 1

  <Translate("zh-TW", "MT:熱交換器加壓延遲"), Category("Main Tank"), _
     TranslateCategory("zh-TW", "主缸參數")> Public ExchangerPressureInDelay As Integer

  <Translate("zh-TW", "MT:降溫溢流洗開度，主缸70度以上"), Category("Main Tank"), _
     TranslateCategory("zh-TW", "主缸參數"), _
  Description("設定主缸於70度以上執行降溫水洗時，降溫閥的開度，最小為0，最大為1000")> Public CoolingValveOutput1 As Integer

  <Translate("zh-TW", "MT:降溫溢流洗開度，主缸70度以下"), Category("Main Tank"), _
     TranslateCategory("zh-TW", "主缸參數"), _
  Description("設定主缸於70度以下執行降溫水洗時，降溫閥的開度，最小為0，最大為1000")> Public CoolingValveOutput2 As Integer

  <Translate("zh-TW", "MT:最大循環時間"), Category("Main Tank"), _
   TranslateCategory("zh-TW", "主缸參數")> Public MaximumFabricCycleTime As Integer

    <Translate("zh-TW", "MT:排壓閥常開"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數")> Public PressureOutAlwaysOn As Integer

    <Translate("zh-TW", "MT:使用氣冷"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數")> Public AirCooling As Integer

  <Translate("zh-TW", "MT:主缸高水位水量"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數")> Public HighLevelVolume As Integer

  <Translate("zh-TW", "MT:主缸中水位水量"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數")> Public MiddleLevelVolume As Integer

  <Translate("zh-TW", "MT:主缸低水位水量"), Category("Main Tank"),
   TranslateCategory("zh-TW", "主缸參數")> Public LowLevelVolume As Integer

  '手動Dosing

  <Translate("zh-TW", "MD:加藥第一段時間"), Category("Manual Dosing"), _
   TranslateCategory("zh-TW", "手動加藥")> Public ManualDoseTime1 As Integer

  <Translate("zh-TW", "MD:加藥第二段時間"), Category("Manual Dosing"), _
   TranslateCategory("zh-TW", "手動加藥")> Public ManualDoseTime2 As Integer

  <Translate("zh-TW", "MD:加藥第三段時間"), Category("Manual Dosing"), _
   TranslateCategory("zh-TW", "手動加藥")> Public ManualDoseTime3 As Integer

  <Translate("zh-TW", "MD:加藥第四段時間"), Category("Manual Dosing"), _
   TranslateCategory("zh-TW", "手動加藥")> Public ManualDoseTime4 As Integer

  <Translate("zh-TW", "MD:加藥第五段時間"), Category("Manual Dosing"), _
   TranslateCategory("zh-TW", "手動加藥")> Public ManualDoseTime5 As Integer

  <Translate("zh-TW", "MD:加藥第六段時間"), Category("Manual Dosing"), _
   TranslateCategory("zh-TW", "手動加藥")> Public ManualDoseTime6 As Integer

  <Translate("zh-TW", "MD:手動加藥曲線"), Category("Manual Dosing"), _
   TranslateCategory("zh-TW", "手動加藥")> Public ManualDoseCurve As Integer

  '<Translate("zh-TW", "纏車密碼")> Public RollerPassword As Integer
  '<Translate("zh-TW", "反轉時間秒")> Public RollerCcwTime As Integer
  '<Translate("zh-TW", "反轉次數次")> Public RollerCcwTimes As Integer

  '自動計量系統

  <Translate("zh-TW", "AD:連接助劑自動計量系統"), Category("Auto Dispenser"), _
   TranslateCategory("zh-TW", "自動計量系統"), _
  Description("是否連接助劑自動計量系統")> Public ChemicalEnable As Integer

  <Translate("zh-TW", "AD:連接染料自動輸送系統"), Category("Auto Dispenser"), _
   TranslateCategory("zh-TW", "自動計量系統"), _
  Description("是否連接染料自動輸送系統")> Public DyeEnable As Integer

  <Translate("zh-TW", "AD:染料自動輸送延遲時間"), Category("Auto Dispenser"), _
 TranslateCategory("zh-TW", "自動計量系統"), _
Description("輸送染料到染機時，當藥缸檢測到低水位開始計時，計時結束則判定輸送完成。單位是秒")> Public DyeTransferDelayTime As Integer = 300

    <Translate("zh-TW", "連結LASPC數據庫重新連結"), Category("Auto Dispenser"),
TranslateCategory("zh-TW", "LASPC連結"),
Description("如果沒有連線,請按1會重新連線.如果有連線後會自動歸0")> Public ConnectSPCTest As Integer

  <Translate("zh-TW", "連結LASPC數據庫"), Category("Auto Dispenser"),
TranslateCategory("zh-TW", "LASPC連結"),
Description("設定1為連接SPC數據庫")> Public ConnectSPCEnable As Integer = 1

  '藥缸參數

  <Translate("zh-TW", "ST:藥缸加完延遲時間秒"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
  Description("藥缸加藥時，若藥缸低水位Low，則開始計算延遲時間，延遲時間到達則結束加藥")> Public AddFinishDelay As Integer

  <Translate("zh-TW", "ST:Dosing種類0變頻1比例2氣動"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數")> Public DosingKind0Pump1AO2DO As Integer

  <Translate("zh-TW", "ST:BC藥缸0雙馬達1單馬達"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
  Description("設定雙藥缸加藥馬達數量，設定0是雙馬達，設定1是單馬達")> Public BCTank100TwoPump1OnePump As Integer

  <Translate("zh-TW", "ST:BC藥缸0B藥缸1BC藥缸"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
  Description("設定藥缸數量，設定0是單藥缸，設定1是雙藥缸")> Public BCTank100BTank1BCTank As Integer

  <Translate("zh-TW", "ST:藥缸加藥延遲時間"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
  Description("設定洗缸前加藥延遲時間")> Public AddTransferTimeBeforeRinse As Integer

  <Translate("zh-TW", "ST:藥缸加藥洗缸時間"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
  Description("設定加藥完後，洗缸進水的時間")> Public AddTransferRinseTime As Integer

  <Translate("zh-TW", "ST:藥缸循環前洗缸時間"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
  Description("設定循環前，洗缸進水的時間")> Public MixCirculateRinseTime As Integer

  <Translate("zh-TW", "ST:藥缸洗缸循環時間"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
  Description("設定洗缸完後，循環攪拌的時間")> Public MixCirculateTimeAfterRinse As Integer

  <Translate("zh-TW", "ST:藥缸洗缸加藥時間"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
  Description("設定洗缸完後，加藥的延遲時間")> Public AddTransferTimeAfterRinse As Integer

  <Translate("zh-TW", "ST:藥缸排水延遲時間"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
  Description("設定藥缸排水的延遲時間")> Public AddTransferDrainTime As Integer

  <Translate("zh-TW", "ST:藥缸加藥結束最小時間"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數")> Public AddDoseOffMinTime As Integer

  <Translate("zh-TW", "ST:藥缸加藥啟動最大時間"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數")> Public AddDoseOnMaxTime As Integer

  <Translate("zh-TW", "ST:B缸呼叫確認0是1否"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
   Description("After B tank dispense finish, if set 1 then system will go to next step without press confirm button."), _
   TranslateDescription("zh-TW", "當B缸計量完成，如果參數設定1，則系統不需等待確認訊號即進行下一步驟")> Public BTankCallAck As Integer

  <Translate("zh-TW", "ST:C缸呼叫確認0是1否"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
   Description("After C tank dispense finish, if set 1 then system will go to next step without press confirm button."), _
   TranslateDescription("zh-TW", "當C缸計量完成，如果參數設定1，則系統不需等待確認訊號即進行下一步驟")> Public CTankCallAck As Integer

  <Translate("zh-TW", "ST:B缸手動備藥灑水時間"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
   Description("When B tank is preparing by manual, the system will fill cold water during mixing to avoid power product float in the air."), _
   TranslateDescription("zh-TW", "當B缸手動備藥，進行至攪拌時，系統會依照設定時間進水，防止粉體原料飛散")> Public BTankSprinkleTimeWhenMixing As Integer

  <Translate("zh-TW", "ST:B缸降溫洗時是否迴水"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
   Description("When B tank is preparing by manual, the system will fill cold water during mixing to avoid power product float in the air."), _
   TranslateDescription("zh-TW", "當執行B缸降溫溢流洗，是否要打開迴水閥，設定0則否，設定1則是")> Public BTankCirWhenRinsefromB As Integer

  <Translate("zh-TW", "ST:藥缸手動大加藥的速度"), Category("Side Tank"), _
   TranslateCategory("zh-TW", "藥缸參數"), _
   Description("When side tank is manual addition, the add pump speed run by this parameter. 1000 is maximum, 0 is minimnum."), _
   TranslateDescription("zh-TW", "當執行手動大加藥時，加藥馬達速度依照設定執行，1000最大，0最小")> Public SideTankManualPumpSpeed As Integer

  <Translate("zh-TW", "ST:背景攪拌On時間"), Category("Side Tank"), _
 TranslateCategory("zh-TW", "藥缸參數")> Public BackgroundMixingOnTime As Integer

  <Translate("zh-TW", "ST:背景攪拌Off時間"), Category("Side Tank"), _
TranslateCategory("zh-TW", "藥缸參數")> Public BackgroundMixingOffTime As Integer


  '其他參數


  '  <Translate("zh-TW", "主馬達保養設定時數"), Category("Maintenance"), _
  '   TranslateCategory("zh-TW", "維護保養參數"), _
  '  Description("當主馬達運行時數超過設定值，則顯示主馬達需保養的警報")> Public MainPumpMaintainTime As Integer

  '  <Translate("zh-TW", "主馬達運轉時數"), Category("Maintenance"), _
  '   TranslateCategory("zh-TW", "維護保養參數"), _
  '  Description("當主馬達實際運行時數，當此數值超過設定值，則顯示主馬達需保養的警報")> Public MainPumpRunTime As Integer

  <Translate("zh-TW", "管數"), Category("Maintenance"),
 TranslateCategory("zh-TW", "維護保養參數"),
  Description("設定機台管數，用來計算每一管布的碼長")> Public RopeNumber As Integer = 4

  <Translate("zh-TW", "馬達功率"), Category("Maintenance"), _
 TranslateCategory("zh-TW", "維護保養參數"), _
  Description("設定馬達功率，用來統計機台的用電量，單位為W")> Public PowerWOfPumps As Integer

  <Translate("zh-TW", "主缸水量"), Category("Maintenance"), _
TranslateCategory("zh-TW", "維護保養參數"), _
Description("設定主缸水量，用來統計用蒸氣量，單位為公升")> Public MainTankLiters As Integer


  <Translate("zh-TW", "主缸溫度調整"), Category("Maintenance"), _
   TranslateCategory("zh-TW", "維護保養參數"), _
   Description("補償主缸溫度，單位是0.1度")> Public MainTempOffSet As Integer

  <Translate("zh-TW", "B缸水位調整"), Category("Maintenance"), _
   TranslateCategory("zh-TW", "維護保養參數"), _
   Description("補償B缸水位，單位是0.1%")> Public TankBLevelOffSet As Integer

  <Translate("zh-TW", "C缸水位調整"), Category("Maintenance"), _
   TranslateCategory("zh-TW", "維護保養參數"), _
   Description("補償C缸水位，單位是0.1%")> Public TankCLevelOffSet As Integer

  <Translate("zh-TW", "警報時間"), Category("Maintenance"), _
   TranslateCategory("zh-TW", "維護保養參數")> Public WaitOverTime As Integer

  <Translate("zh-TW", "上傳實際染色資料溫度"), Category("Maintenance"),
   TranslateCategory("zh-TW", "維護保養參數")> Public UploadDataTemperature As Integer = 60

  <Translate("zh-TW", "主泵類比輸出100的RPM"), Category("Maintenance"),
   TranslateCategory("zh-TW", "維護保養參數")> Public AnalogOutput100ForMainPumpRPM As Integer = 800

  <Translate("zh-TW", "帶布輪1類比輸出100的RPM"), Category("Maintenance"),
   TranslateCategory("zh-TW", "維護保養參數")> Public AnalogOutput100ForReel1PumpRPM As Integer = 800

  <Translate("zh-TW", "帶布輪2類比輸出100的RPM"), Category("Maintenance"),
   TranslateCategory("zh-TW", "維護保養參數")> Public AnalogOutput100ForReel2PumpRPM As Integer = 800

  <Translate("zh-TW", "主泵速度 每秒多少碼"), Category("Maintenance"),
   TranslateCategory("zh-TW", "維護保養參數"),
   Description("主泵速度，單位是碼/秒")> Public PumpSpeedYardPerSec As Integer

  <Translate("zh-TW", "帶布輪1速度 每秒多少碼"), Category("Maintenance"),
   TranslateCategory("zh-TW", "維護保養參數"),
   Description("帶布輪1速度，單位是碼/秒")> Public Reel1SpeedYardPerSec As Integer

  <Translate("zh-TW", "帶布輪2速度 每秒多少碼"), Category("Maintenance"),
   TranslateCategory("zh-TW", "維護保養參數"),
   Description("帶布輪2速度，單位是碼/秒")> Public Reel2SpeedYardPerSec As Integer


  Public PhShowData As Integer

  <Translate("zh-TW", "PH:是否有回流桶"), Category("PH Setup"), _
   TranslateCategory("zh-TW", "PH參數"), _
   Description("是=1 否=0")> Public PhCirTank As Integer = 1

  <Translate("zh-TW", "PH:HAC酸度%"), Category("PH Setup"), _
   TranslateCategory("zh-TW", "PH參數"), _
  Description("酸的濃度，以%計算")> Public PhConcentration As Integer = 100

  <Translate("zh-TW", "PH:調整時檢測時間(秒)"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("每段時間檢測PH值")> Public PhAdjustCheckTime As Integer = 20

  <Translate("zh-TW", "PH:清洗時間(秒)"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("PH清洗管路時間")> Public PhWashTime As Integer = 20

  <Translate("zh-TW", "PH:迴流桶排水延遲時間(秒)"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("迴流桶排水的延遲時間")> Public CirDrainDelayTime As Integer = 20

  <Translate("zh-TW", "PH:迴流桶迴水延遲關閉時間(秒)"), Category("PH Setup"), _
   TranslateCategory("zh-TW", "PH參數"), _
   Description("迴流桶迴水至低水位時，入迴水延遲關閉時間")> Public CirFillDelayTime As Integer = 10

  <Translate("zh-TW", "PH:啟動冷卻系統溫度(度)"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("PH啟動冷卻系統溫度(度)")> Public PhCoolingTemp As Integer = 40


  <Translate("zh-TW", "PH:加酸安全溫度度"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("當PH溫度高於設定值，則不允許加酸動作 60C~110C")> Public PH加酸安全溫度 As Integer = 85

  <Translate("zh-TW", "PH:取樣時間(秒)"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("PH取樣時間(最少60秒)")> Public PhSamplingTime As Integer = 60

  <Translate("zh-TW", "PH:取樣後，延遲等待穩定值(0-60秒)"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("每PH取樣時間後，將等待設定秒確認PH值，管路越遠時間要越長")> Public DoublePhSample As Integer = 30

  <Translate("zh-TW", "PH:偏差時警報(1:是,0:不)"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("PH偏差時警報(1:是,0:不)")> Public PhErrorAlarm As Integer = 0

  <Translate("zh-TW", "PH:加酸動作異常次數"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("加酸時超過多少次為異常")> Public PhAddError As Integer = 50

  <Translate("zh-TW", "PH:到達範圍"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("PH到達範圍,1= 0.01PH,10=0.1PH")> Public PhApproach As Integer = 2

  <Translate("zh-TW", "PH:泵加酸比(60-600)"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("pH泵加酸比,1代表1分鐘=1C.C")> Public PhPumpOutRatio As Integer = 300

  <Translate("zh-TW", "PH:主缸進水量(L)"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("主缸進水量(L)")> Public MainTankFillLevel As Integer = 2000

  <Translate("zh-TW", "PH:起始檢查PH時間"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("一開始時，檢查PH值")> Public StartCheckPh As Integer = 10

  '<Translate("zh-TW", "PH:控制模式"), Category("PH Setup"), _
  'TranslateCategory("zh-TW", "PH參數"), _
  'Description("時間等分演算 = 0 , PH等分演算 = 1")> Public PhControlMode As Integer = 1

  <Translate("zh-TW", "PH:迴水%,開大閥入染機"), Category("PH Setup"), _
  TranslateCategory("zh-TW", "PH參數"), _
  Description("迴水超過多少%，開大閥加藥")> Public CirculateOpenAdd As Integer = 10

  <Translate("zh-TW", "PH:加酸關閉,循環馬達延遲關閉時間"), Category("PH Setup"), _
   TranslateCategory("zh-TW", "PH參數"), _
   Description("加酸閥關閉時,為了讓加酸確實入染機,延遲關閉循環馬達時間")> Public DelayCirculatPump As Integer = 4


  <Translate("zh-TW", "PH:是否啟用持續回流偵測"), Category("PH Setup"), _
TranslateCategory("zh-TW", "PH參數"), _
Description("F73代表開始,F79洗管代表結束")> Public PhCirRuning As Integer = 0

  <Translate("zh-TW", "PH:是否已連接PH系統"), Category("PH Setup"), _
 TranslateCategory("zh-TW", "PH參數"), _
 Description("是=1 否=0")> Public ConnectPhSystem As Integer


  '通訊設定
  <Translate("zh-TW", "通訊埠1"), Category("Communication"), _
 TranslateCategory("zh-TW", "通訊設定")> Public ComNumber1 As Integer = 1 ' default in case no-one changes it
  <Translate("zh-TW", "連線速度1"), Category("Communication"), _
   TranslateCategory("zh-TW", "通訊設定")> Public ComBaudRate1 As Integer = 57600 ' default
  '  <Translate("zh-TW", "通訊埠2"), Category("Communication"), _
  ' TranslateCategory("zh-TW", "通訊設定")> Public ComNumber2 As Integer = 2 ' default in case no-one changes it
  '  <Translate("zh-TW", "連線速度2"), Category("Communication"), _
  '   TranslateCategory("zh-TW", "通訊設定")> Public ComBaudRate2 As Integer = 57600 ' default




End Class


Partial Public Class ControlCode
  Public ReadOnly Parameters As New Parameters
End Class

