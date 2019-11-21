Partial Class ControlCode
  <Translate("zh-TW", "準備入布")> Public MessageLoadFabric As Boolean
  <Translate("zh-TW", "準備出布")> Public MessageUnloadFiber As Boolean
  <Translate("zh-TW", "呼叫操作員")> Public MessageCallOperator As Boolean
  <Translate("zh-TW", "取樣對色")> Public MessageTakeSample As Boolean
  <Translate("zh-TW", "藥缸備水中")> Public MessageSTankFilling As Boolean
  <Translate("zh-TW", "藥缸請備藥")> Public MessageSTankPrepare As Boolean
  <Translate("zh-TW", "藥缸攪拌中")> Public MessageSTankMixing As Boolean
  <Translate("zh-TW", "主缸昇溫中")> Public MessageHeatingNow As Boolean
  <Translate("zh-TW", "主缸持溫中")> Public MessageHoldingNow As Boolean
  <Translate("zh-TW", "主缸降溫中")> Public MessageCoolingNow As Boolean
  <Translate("zh-TW", "藥缸稀釋加藥中")> Public MessageSTankDiluteAddingNow As Boolean
  <Translate("zh-TW", "藥缸dos加藥中")> Public MessageSTankDosingNow As Boolean
  <Translate("zh-TW", "系統暫停")> Public MessageSystemPause As Boolean
  <Translate("zh-TW", "程式結束")> Public MessageProgramFinish As Boolean
  <Translate("zh-TW", "主馬達保養時數到達")> Public MessageMPumpRepair As Boolean
  <Translate("zh-TW", "系統手動操作中")> Public ManualOperation As Boolean
End Class

Public NotInheritable Class Alarms
  Inherits MarshalByRefObject

  <Translate("zh-TW", "主排風故障")> Public MainElectricFanError As Boolean
  <Translate("zh-TW", "溫度過高無法進水")> Public HighTempNoFill As Boolean
  <Translate("zh-TW", "溫度過高無法加藥")> Public HighTempNoAdd As Boolean
  <Translate("zh-TW", "溫度過高無法排水")> Public HighTempNoDrain As Boolean
  <Translate("zh-TW", "纏車")> Public FabricStop As Boolean
  <Translate("zh-TW", "等待B缸備藥OK")> Public BTankWaitReady As Boolean
  <Translate("zh-TW", "等待C缸備藥OK")> Public CTankWaitReady As Boolean
  <Translate("zh-TW", "B缸有水或是手動開關打開")> Public BTankNotEmpty As Boolean
  <Translate("zh-TW", "布輪加藥馬達過載")> Public AddMotorOverload As Boolean
  <Translate("zh-TW", "主馬達異常")> Public MainPumpError As Boolean
  <Translate("zh-TW", "主馬達過載")> Public MainPumpOverload As Boolean
  <Translate("zh-TW", "終端顯示器異常")> Public TerminalError As Boolean
  <Translate("zh-TW", "Plc異常")> Public PlcError As Boolean
  <Translate("zh-TW", "Plc2異常")> Public Plc2Error As Boolean
  <Translate("zh-TW", "蒸氣不足")> Public InsufficientSteam As Boolean
  <Translate("zh-TW", "Pt1斷路")> Public Pt1Open As Boolean
  <Translate("zh-TW", "Pt1短路")> Public Pt1Short As Boolean
  <Translate("zh-TW", "冷卻水不足")> Public CoolingNotEnough As Boolean
  <Translate("zh-TW", "進水完成")> Public FillFinish As Boolean
  <Translate("zh-TW", "排水完成")> Public DrainFinish As Boolean
  <Translate("zh-TW", "手動溫度控制未啟動主馬達")> Public ManualTempControlMainPumpNotOn As Boolean
  <Translate("zh-TW", "手動進水到達水位")> Public ManualFillEnd As Boolean
  <Translate("zh-TW", "手動排水完成")> Public ManualDrainEnd As Boolean
  <Translate("zh-TW", "助劑備藥未完成")> Public ChemicalDispenseError As Boolean
  <Translate("zh-TW", "B缸未備藥")> Public BTankNotReady As Boolean
  <Translate("zh-TW", "C缸未備藥")> Public CTankNotReady As Boolean
  <Translate("zh-TW", "C缸手動加藥中")> Public CTankTransfering As Boolean
  <Translate("zh-TW", "B缸手動加藥中")> Public BTankTransfering As Boolean
  <Translate("zh-TW", "C缸手動攪拌中")> Public CTankMixing As Boolean
  <Translate("zh-TW", "B缸手動攪拌中")> Public BTankMixing As Boolean
  <Translate("zh-TW", "超溫")> Public TemperatureHigh As Boolean
  <Translate("zh-TW", "溫度不足")> Public TemperatureLow As Boolean
    <Translate("zh-TW", "升溫超時")> Public HeatOverTime As Boolean
    <Translate("zh-TW", "溫度到達")> Public ReachTemperature As Boolean
  <Translate("zh-TW", "呼叫操作員超時")> Public CallOperatorOverTime As Boolean
  <Translate("zh-TW", "C缸備藥超時")> Public CTankPrepareOverTime As Boolean
  <Translate("zh-TW", "B缸備藥超時")> Public BTankPrepareOverTime As Boolean
  <Translate("zh-TW", "C缸加藥超時")> Public CTankDosingOverTime As Boolean
  <Translate("zh-TW", "B缸加藥超時")> Public BTankDosingOverTime As Boolean
  <Translate("zh-TW", "取樣超時")> Public SampleOverTime As Boolean
  <Translate("zh-TW", "等待染料超時")> Public WaitLA252OverTime As Boolean
  <Translate("zh-TW", "等待助劑超時")> Public WaitLA302OverTime As Boolean
  <Translate("zh-TW", "主缸進冷水超時")> Public FillColdOverTime As Boolean
  <Translate("zh-TW", "主缸進熱水超時")> Public FillHotOverTime As Boolean
  <Translate("zh-TW", "主缸進冷熱水超時")> Public FillColdHotOverTime As Boolean
  <Translate("zh-TW", "動力排水超時")> Public PowerDrainOverTime As Boolean
  <Translate("zh-TW", "溫度過高無法加酸")> Public HighTempNoAddHac As Boolean
  <Translate("zh-TW", "停止加酸，加酸總量超過目標量")> Public MessageAddHacError As Boolean
  <Translate("zh-TW", "B缸手動排水")> Public BTankManualDrain As Boolean
  <Translate("zh-TW", "C缸手動排水")> Public CTankManualDrain As Boolean
  <Translate("zh-TW", "主缸低水位異常")> Public MainTankLowLevelError As Boolean
  <Translate("zh-TW", "主缸中水位異常")> Public MainTankMiddleLevelError As Boolean
  <Translate("zh-TW", "主缸高水位異常")> Public MainTankHighLevelError As Boolean

End Class


Partial Public Class ControlCode
  Public ReadOnly Alarms As New Alarms
End Class
