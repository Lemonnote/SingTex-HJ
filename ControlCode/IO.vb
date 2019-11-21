Public NotInheritable Class IO
    Inherits MarshalByRefObject

    <IO(IOType.Dinp, 1), Translate("zh-TW", "流量計脈衝訊號")> Public PulseFB As Boolean
    <IO(IOType.Dinp, 2), Translate("zh-TW", "系統自動")> Public SystemAuto As Boolean
    <IO(IOType.Dinp, 3), Translate("zh-TW", "呼叫確認")> Public CallAck As Boolean
    <IO(IOType.Dinp, 4), Translate("zh-TW", "壓力訊號")> Public PressureSw As Boolean
    <IO(IOType.Dinp, 5), Translate("zh-TW", "主馬達訊號")> Public MainPumpFB As Boolean
    <IO(IOType.Dinp, 6), Translate("zh-TW", "纏車1")> Public Entanglement1 As Boolean
    <IO(IOType.Dinp, 7), Translate("zh-TW", "纏車2")> Public Entanglement2 As Boolean
    <IO(IOType.Dinp, 8), Translate("zh-TW", "低水位")> Public LowLevel As Boolean
    <IO(IOType.Dinp, 9), Translate("zh-TW", "中水位")> Public MiddleLevel As Boolean
    <IO(IOType.Dinp, 10), Translate("zh-TW", "高水位")> Public HighLevel As Boolean
    <IO(IOType.Dinp, 11), Translate("zh-TW", "主馬達過載")> Public MainPumpOverload As Boolean
    <IO(IOType.Dinp, 12), Translate("zh-TW", "動力排水")> Public PowerDrainPB As Boolean
    <IO(IOType.Dinp, 13), Translate("zh-TW", "執行確認")> Public RemoteRun As Boolean
    <IO(IOType.Dinp, 14), Translate("zh-TW", "停止週期")> Public RemoteHalt As Boolean
    <IO(IOType.Dinp, 15), Translate("zh-TW", "往上往左")> Public Up As Boolean
    <IO(IOType.Dinp, 16), Translate("zh-TW", "往下往右")> Public Down As Boolean
    <IO(IOType.Dinp, 17), Translate("zh-TW", "進熱水")> Public HotFillPB As Boolean
    <IO(IOType.Dinp, 18), Translate("zh-TW", "排熱水")> Public HotDrainPB As Boolean
    <IO(IOType.Dinp, 19), Translate("zh-TW", "Fan故障")> Public FanError As Boolean
    <IO(IOType.Dinp, 20), Translate("zh-TW", "B備藥完成")> Public BTankReady As Boolean
    <IO(IOType.Dinp, 21), Translate("zh-TW", "B入水")> Public BTankFillColdPB As Boolean
    <IO(IOType.Dinp, 22), Translate("zh-TW", "B入迴水")> Public BTankFillCircPB As Boolean
    <IO(IOType.Dinp, 23), Translate("zh-TW", "B攪拌")> Public BTankMixCirPB As Boolean
    <IO(IOType.Dinp, 24), Translate("zh-TW", "B馬達過載")> Public BPumpOverload As Boolean
    <IO(IOType.Dinp, 25), Translate("zh-TW", "B加藥")> Public BAllIn As Boolean
    <IO(IOType.Dinp, 26), Translate("zh-TW", "B1段加藥")> Public B1Add As Boolean
    <IO(IOType.Dinp, 27), Translate("zh-TW", "B2段加藥")> Public B2Add As Boolean
    <IO(IOType.Dinp, 28), Translate("zh-TW", "B3段加藥")> Public B3Add As Boolean
    <IO(IOType.Dinp, 29), Translate("zh-TW", "B4段加藥")> Public B4Add As Boolean
    <IO(IOType.Dinp, 30), Translate("zh-TW", "B5段加藥")> Public B5Add As Boolean
    <IO(IOType.Dinp, 31), Translate("zh-TW", "B手動加藥停止")> Public BAddStop As Boolean
    <IO(IOType.Dinp, 32), Translate("zh-TW", "動力排熱水")> Public PwoerHotDrainPB As Boolean
    <IO(IOType.Dinp, 33), Translate("zh-TW", "C高水位")> Public CTankHigh As Boolean
  '<IO(IOType.Dinp, 34), Translate("zh-TW", "C中水位")> Public CTankMiddle As Boolean
  <IO(IOType.Dinp, 34), Translate("zh-TW", "B高水位")> Public BTankHigh As Boolean
  <IO(IOType.Dinp, 35), Translate("zh-TW", "C低水位")> Public CTankLow As Boolean
  <IO(IOType.Dinp, 36), Translate("zh-TW", "C備藥完成")> Public CTankReady As Boolean
    <IO(IOType.Dinp, 37), Translate("zh-TW", "C入水")> Public CTankFillColdPB As Boolean
    <IO(IOType.Dinp, 38), Translate("zh-TW", "C入迴水")> Public CTankFillCircPB As Boolean
    <IO(IOType.Dinp, 39), Translate("zh-TW", "C攪拌")> Public CTankMixCirPB As Boolean
    <IO(IOType.Dinp, 40), Translate("zh-TW", "C馬達過載")> Public AddCPumpOverload As Boolean
    <IO(IOType.Dinp, 41), Translate("zh-TW", "C加藥")> Public CAllIn As Boolean
    <IO(IOType.Dinp, 42), Translate("zh-TW", "C1段加藥")> Public C1Add As Boolean
    <IO(IOType.Dinp, 43), Translate("zh-TW", "C2段加藥")> Public C2Add As Boolean
    <IO(IOType.Dinp, 44), Translate("zh-TW", "C3段加藥")> Public C3Add As Boolean
    <IO(IOType.Dinp, 45), Translate("zh-TW", "C4段加藥")> Public C4Add As Boolean
    <IO(IOType.Dinp, 46), Translate("zh-TW", "C5段加藥")> Public C5Add As Boolean
    <IO(IOType.Dinp, 48), Translate("zh-TW", "C手動加藥停止")> Public CAddStop As Boolean
    <IO(IOType.Dinp, 49), Translate("zh-TW", "手動主馬達啟動")> Public MainPumpOnPB As Boolean
    <IO(IOType.Dinp, 50), Translate("zh-TW", "手動主馬達停止")> Public MainPumpOffPB As Boolean
    <IO(IOType.Dinp, 51), Translate("zh-TW", "手動升溫")> Public HeatPB As Boolean
    <IO(IOType.Dinp, 52), Translate("zh-TW", "手動降溫")> Public CoolPB As Boolean
    <IO(IOType.Dinp, 53), Translate("zh-TW", "溫度控制到達")> Public TempControlOn As Boolean
    <IO(IOType.Dinp, 54), Translate("zh-TW", "手動進水")> Public FillPB As Boolean
    <IO(IOType.Dinp, 55), Translate("zh-TW", "手動水位進水")> Public FillLevelControlPB As Boolean
    <IO(IOType.Dinp, 56), Translate("zh-TW", "手動排水")> Public DrainPB As Boolean
    <IO(IOType.Dinp, 57), Translate("zh-TW", "手動溢流")> Public OverFlowPB As Boolean
    <IO(IOType.Dinp, 58), Translate("zh-TW", "手動降溫溢流洗")> Public CoolingOverFlowPB As Boolean
    <IO(IOType.Dinp, 59), Translate("zh-TW", "手動入軟水")> Public SoftFillPB As Boolean
    <IO(IOType.Dinp, 60), Translate("zh-TW", "手動B缸排水")> Public BDrainPB As Boolean
    <IO(IOType.Dinp, 61), Translate("zh-TW", "手動C缸排水")> Public CDrainPB As Boolean
  <IO(IOType.Dinp, 62), Translate("zh-TW", "馬達速度自動")> Public AutoPumpControlSW As Boolean
  <IO(IOType.Dinp, 65), Translate("zh-TW", "手動帶布輪1正轉")> Public Reel1ForwardPB As Boolean
  <IO(IOType.Dinp, 66), Translate("zh-TW", "手動帶布輪1反轉")> Public Reel1ReversePB As Boolean
    <IO(IOType.Dinp, 67), Translate("zh-TW", "手動帶布輪1停止")> Public Reel1StopPB As Boolean
    <IO(IOType.Dinp, 68), Translate("zh-TW", "手動帶布輪2正轉")> Public Reel2ForwardPB As Boolean
    <IO(IOType.Dinp, 69), Translate("zh-TW", "手動帶布輪2反轉")> Public Reel2ReversePB As Boolean
    <IO(IOType.Dinp, 70), Translate("zh-TW", "手動帶布輪2停止")> Public Reel2StopPB As Boolean
    <IO(IOType.Dinp, 71), Translate("zh-TW", "手動帶布輪3正轉")> Public Reel3ForwardPB As Boolean
    <IO(IOType.Dinp, 72), Translate("zh-TW", "手動帶布輪3反轉")> Public Reel3ReversePB As Boolean
    <IO(IOType.Dinp, 73), Translate("zh-TW", "手動帶布輪3停止")> Public Reel3StopPB As Boolean
    <IO(IOType.Dinp, 74), Translate("zh-TW", "手動帶布輪4正轉")> Public Reel4ForwardPB As Boolean
    <IO(IOType.Dinp, 75), Translate("zh-TW", "手動帶布輪4反轉")> Public Reel4ReversePB As Boolean
    <IO(IOType.Dinp, 76), Translate("zh-TW", "手動帶布輪4停止")> Public Reel4StopPB As Boolean
    <IO(IOType.Dinp, 77), Translate("zh-TW", "帶布輪1過載")> Public Reel1OverLoad As Boolean
    <IO(IOType.Dinp, 78), Translate("zh-TW", "帶布輪2過載")> Public Reel2OverLoad As Boolean
    <IO(IOType.Dinp, 79), Translate("zh-TW", "帶布輪3過載")> Public Reel3OverLoad As Boolean
    <IO(IOType.Dinp, 80), Translate("zh-TW", "帶布輪4過載")> Public Reel4OverLoad As Boolean
    <IO(IOType.Dinp, 81), Translate("zh-TW", "布頭訊號1")> Public FabricCycleInput1 As Boolean
    <IO(IOType.Dinp, 82), Translate("zh-TW", "布頭訊號2")> Public FabricCycleInput2 As Boolean
    <IO(IOType.Dinp, 83), Translate("zh-TW", "手動水流閥1關閉")> Public FlowControl1ClosePB As Boolean
    <IO(IOType.Dinp, 84), Translate("zh-TW", "手動水流閥2關閉")> Public FlowControl2ClosePB As Boolean
    <IO(IOType.Dinp, 85), Translate("zh-TW", "手動水流閥3關閉")> Public FlowControl3ClosePB As Boolean
    <IO(IOType.Dinp, 86), Translate("zh-TW", "手動水流閥4關閉")> Public FlowControl4ClosePB As Boolean
  <IO(IOType.Dinp, 87), Translate("zh-TW", "布頭訊號3")> Public FabricCycleInput3 As Boolean
  <IO(IOType.Dinp, 88), Translate("zh-TW", "布頭訊號4")> Public FabricCycleInput4 As Boolean
  <IO(IOType.Dinp, 89), GraphTrace(0, 1, , , "Blue")> Public 布輪1纏車 As Boolean
  <IO(IOType.Dinp, 90), GraphTrace(0, 1, , , "Blue")> Public 布輪2纏車 As Boolean
  <IO(IOType.Dinp, 91), GraphTrace(0, 1, , , "Blue")> Public 主馬達停止 As Boolean
  '***********************PH功能*************************************************************
  <IO(IOType.Dinp, 111), Translate("zh-TW", "PH循環桶高水位")> Public PhMixTankHighLevel As Boolean
  <IO(IOType.Dinp, 112), Translate("zh-TW", "PH循環桶低水位")> Public PhMixTankLowLevel As Boolean
  <IO(IOType.Dinp, 114), Translate("zh-TW", "手動PH清洗")> Public ManualPHWash As Boolean

    <IO(IOType.Aninp, 1, , , "%t%"), _
     GraphTrace(-100, 1200, 2500, 4000, "Green", "%t%"), _
     GraphLabel("B缸水位0%", 0), GraphLabel("B缸水位100%", 1000), _
     Translate("zh-TW", "藥缸B水位")> Public TankBLevel As Short
    <IO(IOType.Aninp, 2, , , "%t%%"), GraphTrace(-100, 1200, 500, 2000, "Blue", "%t%"), _
     GraphLabel("C缸水位0%", 0), GraphLabel("C缸水位100%", 1000), _
     Translate("zh-TW", "藥缸C水位")> Public TankCLevel As Short
    <IO(IOType.Aninp, 3, , "Red", "%2tpH"),
   GraphTrace(300, 1000, 4000, 5500),
   GraphLabel("4pH", 400), GraphLabel("7pH", 700), GraphLabel("10pH", 1000),
   Translate("zh-TW", "PH值")> Public pHValue As Short
  <IO(IOType.Aninp, 4, , , "0RPM"),
     Translate("zh-TW", "主泵速度")> Public MainPumpSpeed As Short
  <IO(IOType.Aninp, 5, , , "0RPM"),
     Translate("zh-TW", "帶布輪1速度")> Public Reel1Speed As Short
  <IO(IOType.Aninp, 6, , , "0RPM"),
     Translate("zh-TW", "帶布輪2速度")> Public Reel2Speed As Short
  <IO(IOType.Aninp, 7, , , "0sec"),
     Translate("zh-TW", "CycleTime1")> Public CycleTime1 As Integer
  <IO(IOType.Aninp, 8, , , "0sec"),
     Translate("zh-TW", "CycleTime2")> Public CycleTime2 As Integer
  <IO(IOType.Aninp, 9, , , "0sec"),
     Translate("zh-TW", "CycleTime3")> Public CycleTime3 As Integer
  <IO(IOType.Aninp, 10, , , "0sec"),
     Translate("zh-TW", "CycleTime4")> Public CycleTime4 As Integer
  <IO(IOType.Aninp, 11, , , "%tBar"),
     Translate("zh-TW", "噴壓")> Public NozzlePressure As Integer
  <IO(IOType.Aninp, 12, , , "%tBar"),
     Translate("zh-TW", "缸壓")> Public MainPressure As Integer
  <IO(IOType.Aninp, 13, , , "%tBar"),
     Translate("zh-TW", "壓差")> Public PressureDifferent As Integer

  <IO(IOType.Temp, 1, , , "%tC"), Translate("zh-TW", "主缸溫度")> Public MainTemperature As Short
  <IO(IOType.Temp, 2, , , "%tC"), Translate("zh-TW", "pH溫度1")> Public pHTemperature1 As Short
  <IO(IOType.Temp, 3, , , "%tC"), Translate("zh-TW", "pH溫度2")> Public pHTemperature2 As Short


    <IO(IOType.Counter, 1)> Public HSCounter1 As Integer
    <IO(IOType.Counter, 2)> Public HSCounter2 As Integer
    <IO(IOType.Counter, 3), GraphTrace(0, 20, 4500, 5500, "Yellow")> Public 纏車次數 As Integer
    <IO(IOType.Counter, 4)> Public 主泵時間分鐘 As Integer
    <IO(IOType.Counter, 5)> Public 排水時間分鐘 As Integer
    <IO(IOType.Counter, 6)> Public 升溫時間分鐘 As Integer
    <IO(IOType.Counter, 7)> Public 降溫時間分鐘 As Integer




    <IO(IOType.Anout, 1), Translate("zh-TW", "比例式昇降溫")> Public TemperatureControl As Short
    <IO(IOType.Anout, 2), Translate("zh-TW", "主泵變頻器")> Public PumpSpeedControl As Short
    <IO(IOType.Anout, 3), Translate("zh-TW", "B變頻控制")> Public BDosingOutput As Short
  <IO(IOType.Anout, 4), Translate("zh-TW", "C變頻控制")> Public CDosingOutput As Short
  <IO(IOType.Anout, 5), Translate("zh-TW", "熱交換器排水延遲時間")> Public HxDelay As Integer
  <IO(IOType.Anout, 6), Translate("zh-TW", "主缸排水延遲時間")> Public DrainDelay As Integer
    <IO(IOType.Anout, 7), Translate("zh-TW", "主缸安全溫度")> Public MainSafetyTemp As Integer
    <IO(IOType.Anout, 8), Translate("zh-TW", "藥缸加藥延遲時間")> Public STankAddDelay As Integer
    <IO(IOType.Anout, 9), Translate("zh-TW", "動力排水延遲時間")> Public PowerDrainDelay As Integer
    <IO(IOType.Anout, 10), Translate("zh-TW", "帶布輪延遲時間")> Public ReelDelay As Integer
    <IO(IOType.Anout, 11), Translate("zh-TW", "熱交換器加壓延遲時間")> Public ExchangerPressureInDelay As Integer
    <IO(IOType.Anout, 12), Translate("zh-TW", "主缸溫度調整")> Public MainTempOffSet As Integer
    <IO(IOType.Anout, 13), Translate("zh-TW", "B缸水位調整")> Public TankBLevelOffSet As Integer
    <IO(IOType.Anout, 14), Translate("zh-TW", "C缸水位調整")> Public TankCLevelOffSet As Integer
  <IO(IOType.Anout, 15), Translate("zh-TW", "主泵速度控制")> Public MainPumpSpeedControl As Integer
  <IO(IOType.Anout, 16), Translate("zh-TW", "帶布輪1速度控制")> Public Reel1SpeedControl As Integer
  <IO(IOType.Anout, 17), Translate("zh-TW", "帶布輪2速度控制")> Public Reel2SpeedControl As Integer

  <IO(IOType.Dout, 1), Translate("zh-TW", "昇溫")> Public Heat As Boolean
    <IO(IOType.Dout, 2), Translate("zh-TW", "降溫")> Public Cool As Boolean
    <IO(IOType.Dout, 3), Translate("zh-TW", "熱交換器排水")> Public HxDrain As Boolean
    <IO(IOType.Dout, 4), Translate("zh-TW", "排冷凝水")> Public CondenserDrain As Boolean
    <IO(IOType.Dout, 5), Translate("zh-TW", "冷卻排水")> Public CoolDrain As Boolean
    <IO(IOType.Dout, 6), Translate("zh-TW", "加壓")> Public PressureIn As Boolean
    <IO(IOType.Dout, 7), Translate("zh-TW", "排壓")> Public PressureOut As Boolean
    <IO(IOType.Dout, 8), Translate("zh-TW", "溢流")> Public Overflow As Boolean
    <IO(IOType.Dout, 9), Translate("zh-TW", "進冷水")> Public ColdFill As Boolean
    <IO(IOType.Dout, 10), Translate("zh-TW", "進熱水")> Public HotFill As Boolean
    <IO(IOType.Dout, 11), Translate("zh-TW", "排熱水")> Public HotDrain As Boolean
    <IO(IOType.Dout, 12), Translate("zh-TW", "排水")> Public Drain As Boolean
    <IO(IOType.Dout, 13), Translate("zh-TW", "主馬達啟動")> Public PumpOn As Boolean
    <IO(IOType.Dout, 14), Translate("zh-TW", "主馬達停止")> Public PumpOff As Boolean
    <IO(IOType.Dout, 15), Translate("zh-TW", "異常燈")> Public ErrorLamp As Boolean
    <IO(IOType.Dout, 16), Translate("zh-TW", "呼叫燈")> Public CallLamp As Boolean
    <IO(IOType.Dout, 17), Translate("zh-TW", "B排水")> Public BTankDrain As Boolean
    <IO(IOType.Dout, 18), Translate("zh-TW", "BDosing")> Public BDosing As Boolean
    <IO(IOType.Dout, 19), Translate("zh-TW", "B加藥馬達")> Public BTankAddPump As Boolean
    <IO(IOType.Dout, 20), Translate("zh-TW", "B循環攪拌")> Public BTankMixCir As Boolean
    <IO(IOType.Dout, 21), Translate("zh-TW", "B進冷水")> Public BTankFillCold As Boolean
    <IO(IOType.Dout, 22), Translate("zh-TW", "B進迴水")> Public BTankFillCirc As Boolean
    <IO(IOType.Dout, 23), Translate("zh-TW", "B大加藥")> Public BTankAddition As Boolean
  <IO(IOType.Dout, 24), Translate("zh-TW", "B備藥完成燈")> Public BTankOkLamp As Boolean  'M295
  <IO(IOType.Dout, 25), Translate("zh-TW", "動力排熱水")> Public PowerHotDrain As Boolean 'M296
  <IO(IOType.Dout, 26), Translate("zh-TW", "1段B加藥")> Public B1AddLamp As Boolean 'M297
  <IO(IOType.Dout, 27), Translate("zh-TW", "2段B加藥")> Public B2AddLamp As Boolean 'M298
  <IO(IOType.Dout, 28), Translate("zh-TW", "3段B加藥")> Public B3AddLamp As Boolean 'M299
    <IO(IOType.Dout, 29), Translate("zh-TW", "4段B加藥")> Public B4AddLamp As Boolean
    <IO(IOType.Dout, 30), Translate("zh-TW", "5段B加藥")> Public B5AddLamp As Boolean
    <IO(IOType.Dout, 31), Translate("zh-TW", "6段B加藥")> Public B6AddLamp As Boolean
    <IO(IOType.Dout, 32), Translate("zh-TW", "動力排水")> Public PowerDrain As Boolean
    <IO(IOType.Dout, 33), Translate("zh-TW", "C排水")> Public CTankDrain As Boolean
    <IO(IOType.Dout, 34), Translate("zh-TW", "CDosing")> Public CDosingBIn As Boolean
    <IO(IOType.Dout, 35), Translate("zh-TW", "C加藥馬達")> Public CTankAddPump As Boolean
    <IO(IOType.Dout, 36), Translate("zh-TW", "C循環攪拌")> Public CTankMixCir As Boolean
    <IO(IOType.Dout, 37), Translate("zh-TW", "C進冷水")> Public CTankFillCold As Boolean
    <IO(IOType.Dout, 38), Translate("zh-TW", "C進迴水")> Public CTankFillCirc As Boolean
    <IO(IOType.Dout, 39), Translate("zh-TW", "C大加藥")> Public CTankAdditionCIn As Boolean
    <IO(IOType.Dout, 40), Translate("zh-TW", "C備藥完成燈")> Public CTankOkLamp As Boolean
    <IO(IOType.Dout, 41), Translate("zh-TW", "熱交換器加壓")> Public ExchangerPressureIn As Boolean
    <IO(IOType.Dout, 42), Translate("zh-TW", "1段C加藥")> Public C1AddLamp As Boolean
    <IO(IOType.Dout, 43), Translate("zh-TW", "2段C加藥")> Public C2AddLamp As Boolean
    <IO(IOType.Dout, 44), Translate("zh-TW", "3段C加藥")> Public C3AddLamp As Boolean
    <IO(IOType.Dout, 45), Translate("zh-TW", "4段C加藥")> Public C4AddLamp As Boolean
    <IO(IOType.Dout, 46), Translate("zh-TW", "5段C加藥")> Public C5AddLamp As Boolean
    <IO(IOType.Dout, 47), Translate("zh-TW", "6段C加藥")> Public C6AddLamp As Boolean
    <IO(IOType.Dout, 48), Translate("zh-TW", "藥缸自動中")> Public SideTankAuto As Boolean
  <IO(IOType.Dout, 58), Translate("zh-TW", "入降溫回收水")> Public FillReCycleWater As Boolean
  <IO(IOType.Dout, 65), Translate("zh-TW", "氣冷")> Public AirCool As Boolean
  <IO(IOType.Dout, 66), Translate("zh-TW", "流量計重置")> Public CounterReset As Boolean
  <IO(IOType.Dout, 67), Translate("zh-TW", "帶布輪停止")> Public ReelStop As Boolean
  <IO(IOType.Dout, 68), Translate("zh-TW", "B不允許排水")> Public BDisableDrain As Boolean
  <IO(IOType.Dout, 69), Translate("zh-TW", "C不允許排水")> Public CDisableDrain As Boolean
  <IO(IOType.Dout, 70), Translate("zh-TW", "pH啟用")> Public EnablePh As Boolean
  <IO(IOType.Dout, 71), Translate("zh-TW", "主泵自動控制")> Public PumpControlOn As Boolean

  '***********************PH功能*************************************************************
  '-----------------------------------------------------------------------------------------------------
  <IO(IOType.Dout, 101), Translate("zh-TW", "PH入迴水")> Public PhFillCirculate As Boolean
  <IO(IOType.Dout, 102), Translate("zh-TW", "PH入染機")> Public PhInToMachine As Boolean
  <IO(IOType.Dout, 103), Translate("zh-TW", "PH入清水")> Public PhFillCold As Boolean
  <IO(IOType.Dout, 104), Translate("zh-TW", "PH冷卻")> Public PhCool As Boolean
  <IO(IOType.Dout, 105), Translate("zh-TW", "PH排水")> Public PhDrain As Boolean
  <IO(IOType.Dout, 106), Translate("zh-TW", "PH加酸閥")> Public PhAddHacOut As Boolean
  <IO(IOType.Dout, 107), Translate("zh-TW", "PH循環泵")> Public PhCirculate As Boolean
  <IO(IOType.Dout, 108), Translate("zh-TW", "PH定量泵")> Public PhAddPump As Boolean
  <IO(IOType.Dout, 111), Translate("zh-TW", "PH高溫燈")> Public PhHitTempLamp As Boolean
  <IO(IOType.Dout, 112), Translate("zh-TW", "異常紅燈")> Public PhErrorLamp As Boolean
  <IO(IOType.Dout, 113), Translate("zh-TW", "PH輸出測試")> Public PhOutputTest As Boolean

  <Translate("zh-TW", "主缸溫度調整+"), Category("Maintenance"),
   TranslateCategory("zh-TW", "維護保養參數"),
  Description("調整主缸溫度，單位為0.1度，例如: 10=+1.0")> Public Parameters_AdjustMainTemperature_Add As Short = 0

  <Translate("zh-TW", "主缸溫度調整-"), Category("Maintenance"),
   TranslateCategory("zh-TW", "維護保養參數"),
  Description("調整主缸溫度，單位為0.1度，例如: 10=-1.0")> Public Parameters_AdjustMainTemperature_Sub As Short = 0

  <Translate("zh-TW", "噴壓 (<實際值 資料不可以寫入>)"), Category("噴壓校正參數"), DefaultValue(1000)> Public Parameters_NozzlePressureRealValue As Short
  <Translate("zh-TW", "噴壓 (取高值)"), Category("噴壓校正參數"), DefaultValue(1000)> Public Parameters_NozzlePressureHighValue As Short = 0
  <Translate("zh-TW", "噴壓 (取低值)"), Category("噴壓校正參數"), DefaultValue(1000)> Public Parameters_NozzlePressureLowValue As Short = 1000
  <Translate("zh-TW", "噴壓 (設高值)"), Category("噴壓校正參數"), DefaultValue(1000)> Public Parameters_SetNozzlePressureHighValue As Short = 0
  <Translate("zh-TW", "噴壓 (設低值)"), Category("噴壓校正參數"), DefaultValue(1000)> Public Parameters_SetNozzlePressureLowValue As Short = 1000

  <Translate("zh-TW", "缸壓 (<實際值 資料不可以寫入>)"), Category("缸壓校正參數"), DefaultValue(1000)> Public Parameters_MainPressureRealValue As Short
  <Translate("zh-TW", "缸壓 (取高值)"), Category("缸壓校正參數"), DefaultValue(1000)> Public Parameters_MainPressureHighValue As Short = 0
  <Translate("zh-TW", "缸壓 (取低值)"), Category("缸壓校正參數"), DefaultValue(1000)> Public Parameters_MainPressureLowValue As Short = 1000
  <Translate("zh-TW", "缸壓 (設高值)"), Category("缸壓校正參數"), DefaultValue(1000)> Public Parameters_SetMainPressureHighValue As Short = 0
  <Translate("zh-TW", "缸壓 (設低值)"), Category("缸壓校正參數"), DefaultValue(1000)> Public Parameters_SetMainPressureLowValue As Short = 1000


  <Translate("zh-TW", "主泵速度 (<實際值 資料不可以寫入>)"), Category("主泵速度校正參數"), DefaultValue(1000)> Public Parameters_MainPumpRealValue As Short
  <Translate("zh-TW", "主泵速度 (取值1)"), Category("主泵速度校正參數"), DefaultValue(1000)> Public Parameters_MainPumpValue1 As Short = 0
  <Translate("zh-TW", "主泵速度 (取值2)"), Category("主泵速度校正參數"), DefaultValue(1000)> Public Parameters_MainPumpValue2 As Short = 500
  <Translate("zh-TW", "主泵速度 (取值3)"), Category("主泵速度校正參數"), DefaultValue(1000)> Public Parameters_MainPumpValue3 As Short = 1000
  <Translate("zh-TW", "主泵速度 (取值4)"), Category("主泵速度校正參數"), DefaultValue(1000)> Public Parameters_MainPumpValue4 As Short = 1400
  <Translate("zh-TW", "主泵速度 (取值5)"), Category("主泵速度校正參數"), DefaultValue(1000)> Public Parameters_MainPumpValue5 As Short = 1800
  <Translate("zh-TW", "主泵速度 (設值1)"), Category("主泵速度校正參數"), DefaultValue(1000)> Public Parameters_SetMainPumpValue1 As Short = 0
  <Translate("zh-TW", "主泵速度 (設值2)"), Category("主泵速度校正參數"), DefaultValue(1000)> Public Parameters_SetMainPumpValue2 As Short = 500
  <Translate("zh-TW", "主泵速度 (設值3)"), Category("主泵速度校正參數"), DefaultValue(1000)> Public Parameters_SetMainPumpValue3 As Short = 1000
  <Translate("zh-TW", "主泵速度 (設值4)"), Category("主泵速度校正參數"), DefaultValue(1000)> Public Parameters_SetMainPumpValue4 As Short = 1400
  <Translate("zh-TW", "主泵速度 (設值5)"), Category("主泵速度校正參數"), DefaultValue(1000)> Public Parameters_SetMainPumpValue5 As Short = 1800

  <Translate("zh-TW", "帶布輪1速度 (<實際值 資料不可以寫入>)"), Category("帶布輪1速度校正參數"), DefaultValue(1000)> Public Parameters_Reel1RealValue As Short
  <Translate("zh-TW", "帶布輪1速度 (取值1)"), Category("帶布輪1速度校正參數"), DefaultValue(1000)> Public Parameters_Reel1Value1 As Short = 0
  <Translate("zh-TW", "帶布輪1速度 (取值2)"), Category("帶布輪1速度校正參數"), DefaultValue(1000)> Public Parameters_Reel1Value2 As Short = 500
  <Translate("zh-TW", "帶布輪1速度 (取值3)"), Category("帶布輪1速度校正參數"), DefaultValue(1000)> Public Parameters_Reel1Value3 As Short = 1000
  <Translate("zh-TW", "帶布輪1速度 (取值4)"), Category("帶布輪1速度校正參數"), DefaultValue(1000)> Public Parameters_Reel1Value4 As Short = 1400
  <Translate("zh-TW", "帶布輪1速度 (取值5)"), Category("帶布輪1速度校正參數"), DefaultValue(1000)> Public Parameters_Reel1Value5 As Short = 1800
  <Translate("zh-TW", "帶布輪1速度 (設值1)"), Category("帶布輪1速度校正參數"), DefaultValue(1000)> Public Parameters_Reel1SetValue1 As Short = 0
  <Translate("zh-TW", "帶布輪1速度 (設值2)"), Category("帶布輪1速度校正參數"), DefaultValue(1000)> Public Parameters_Reel1SetValue2 As Short = 500
  <Translate("zh-TW", "帶布輪1速度 (設值3)"), Category("帶布輪1速度校正參數"), DefaultValue(1000)> Public Parameters_Reel1SetValue3 As Short = 1000
  <Translate("zh-TW", "帶布輪1速度 (設值4)"), Category("帶布輪1速度校正參數"), DefaultValue(1000)> Public Parameters_Reel1SetValue4 As Short = 1400
  <Translate("zh-TW", "帶布輪1速度 (設值5)"), Category("帶布輪1速度校正參數"), DefaultValue(1000)> Public Parameters_Reel1SetValue5 As Short = 1800

  <Translate("zh-TW", "帶布輪2速度 (<實際值 資料不可以寫入>)"), Category("帶布輪2速度校正參數"), DefaultValue(1000)> Public Parameters_Reel2RealValue As Short
  <Translate("zh-TW", "帶布輪2速度 (取值1)"), Category("帶布輪2速度校正參數"), DefaultValue(1000)> Public Parameters_Reel2Value1 As Short = 0
  <Translate("zh-TW", "帶布輪2速度 (取值2)"), Category("帶布輪2速度校正參數"), DefaultValue(1000)> Public Parameters_Reel2Value2 As Short = 500
  <Translate("zh-TW", "帶布輪2速度 (取值3)"), Category("帶布輪2速度校正參數"), DefaultValue(1000)> Public Parameters_Reel2Value3 As Short = 1000
  <Translate("zh-TW", "帶布輪2速度 (取值4)"), Category("帶布輪2速度校正參數"), DefaultValue(1000)> Public Parameters_Reel2Value4 As Short = 1400
  <Translate("zh-TW", "帶布輪2速度 (取值5)"), Category("帶布輪2速度校正參數"), DefaultValue(1000)> Public Parameters_Reel2Value5 As Short = 1800
  <Translate("zh-TW", "帶布輪2速度 (設值1)"), Category("帶布輪2速度校正參數"), DefaultValue(1000)> Public Parameters_Reel2SetValue1 As Short = 0
  <Translate("zh-TW", "帶布輪2速度 (設值2)"), Category("帶布輪2速度校正參數"), DefaultValue(1000)> Public Parameters_Reel2SetValue2 As Short = 500
  <Translate("zh-TW", "帶布輪2速度 (設值3)"), Category("帶布輪2速度校正參數"), DefaultValue(1000)> Public Parameters_Reel2SetValue3 As Short = 1000
  <Translate("zh-TW", "帶布輪2速度 (設值4)"), Category("帶布輪2速度校正參數"), DefaultValue(1000)> Public Parameters_Reel2SetValue4 As Short = 1400
  <Translate("zh-TW", "帶布輪2速度 (設值5)"), Category("帶布輪2速度校正參數"), DefaultValue(1000)> Public Parameters_Reel2SetValue5 As Short = 1800

  <EditorBrowsable(EditorBrowsableState.Advanced)>
  Public Function ReadInputs(ByVal dinp() As Boolean, ByVal aninp() As Short, ByVal temp() As Short) As Boolean
    CheckForSerialPortParametersChanged()
    If Plc.Read(1, "WM400", dinp) = Ports.LA60B.Result.OK Then
      ReadInputs = True
      PlcTimeout.TimeRemaining = 3 ' must have a problem for at least 1 second before triggering the alarm
    End If
    Static ai(14) As UShort
    If Plc.Read(1, "D300", ai) = Ports.LA60B.Result.OK Then
      ' Copy temperatures directly
      If Not FirstScan Then
        FirstScanTimer.TimeRemaining = 60
        FirstScan = True
      End If
      If FirstScan And FirstScanTimer.Finished Then
        FirstSmoothFinish = True
      End If
      If Not FirstSmoothFinish Then
        MainTemperatureSmooth(MainTemperatureSmoothTimes) = ai(1)
        If SmoothTimer.Finished Then
          SmoothTimer.TimeRemainingMs = 200
          If MainTemperatureSmoothTimes >= 19 Then
            MainTemperatureSmoothTimes = 0
          Else
            MainTemperatureSmoothTimes = MainTemperatureSmoothTimes + 1
          End If
        End If
        Dim j As Integer
        Dim MainTemperatureSmoothSum As Integer
        For j = 0 To 19
          MainTemperatureSmoothSum = MainTemperatureSmoothSum + MainTemperatureSmooth(j)
        Next
        temp(1) = CType((MainTemperatureSmoothSum / 20) + Parameters_AdjustMainTemperature_Add - Parameters_AdjustMainTemperature_Sub, Short)
        '開始做smooth
      Else
        MainTemperatureSmooth(MainTemperatureSmoothTimes) = ai(1)
        If SmoothTimer.Finished Then
          SmoothTimer.TimeRemaining = 1
          If MainTemperatureSmoothTimes >= 19 Then
            MainTemperatureSmoothTimes = 0
          Else
            MainTemperatureSmoothTimes = MainTemperatureSmoothTimes + 1
          End If
        End If
        Dim j As Integer
        Dim MainTemperatureSmoothSum As Integer
        For j = 0 To 19
          MainTemperatureSmoothSum = MainTemperatureSmoothSum + MainTemperatureSmooth(j)
        Next
        temp(1) = CType((MainTemperatureSmoothSum / 20) + Parameters_AdjustMainTemperature_Add - Parameters_AdjustMainTemperature_Sub, Short)
      End If
      temp(2) = CType(ai(2), Short)
      temp(3) = CType(ai(3), Short)

      ' Copy aninps  0mA=0, 20mA=4095, so 0% = 4mA = 819 and 100% = 20mA = 4095
      aninp(1) = CType(ai(4), Short)
      aninp(2) = CType(ai(5), Short)
      aninp(3) = CType(ai(6), Short)
      'aninp(4) = CType(ai(7), Short)
      'aninp(5) = CType(ai(8), Short)
      'aninp(6) = CType(ai(9), Short)

      Dim i As Integer
      Dim MainPumpValue(4) As Short
            Dim MainPumpSetValue(4) As Short
            MainPumpValue(0) = Parameters_MainPumpValue1
            MainPumpValue(1) = Parameters_MainPumpValue2
            MainPumpValue(2) = Parameters_MainPumpValue3
            MainPumpValue(3) = Parameters_MainPumpValue4
            MainPumpValue(4) = Parameters_MainPumpValue5
            MainPumpSetValue(0) = Parameters_SetMainPumpValue1
            MainPumpSetValue(1) = Parameters_SetMainPumpValue2
            MainPumpSetValue(2) = Parameters_SetMainPumpValue3
            MainPumpSetValue(3) = Parameters_SetMainPumpValue4
            MainPumpSetValue(4) = Parameters_SetMainPumpValue5
            Parameters_MainPumpRealValue = CType(ai(7), Short)
      Dim Reel1Value(4) As Short
      Dim Reel1SetValue(4) As Short
      Reel1Value(0) = Parameters_Reel1Value1
      Reel1Value(1) = Parameters_Reel1Value2
      Reel1Value(2) = Parameters_Reel1Value3
      Reel1Value(3) = Parameters_Reel1Value4
      Reel1Value(4) = Parameters_Reel1Value5
      Reel1SetValue(0) = Parameters_Reel1SetValue1
      Reel1SetValue(1) = Parameters_Reel1SetValue2
      Reel1SetValue(2) = Parameters_Reel1SetValue3
      Reel1SetValue(3) = Parameters_Reel1SetValue4
      Reel1SetValue(4) = Parameters_Reel1SetValue5
      Parameters_Reel1RealValue = CType(ai(8), Short)
      Dim Reel2Value(4) As Short
      Dim Reel2SetValue(4) As Short
      Reel2Value(0) = Parameters_Reel2Value1
      Reel2Value(1) = Parameters_Reel2Value2
      Reel2Value(2) = Parameters_Reel2Value3
      Reel2Value(3) = Parameters_Reel2Value4
      Reel2Value(4) = Parameters_Reel2Value5
      Reel2SetValue(0) = Parameters_Reel2SetValue1
      Reel2SetValue(1) = Parameters_Reel2SetValue2
      Reel2SetValue(2) = Parameters_Reel2SetValue3
      Reel2SetValue(3) = Parameters_Reel2SetValue4
      Reel2SetValue(4) = Parameters_Reel2SetValue5
      Parameters_Reel2RealValue = CType(ai(9), Short)

      Dim value1, value2, value3 As Integer
      For i = 0 To 4
        If Parameters_MainPumpRealValue <= MainPumpValue(0) Then
          aninp(4) = MainPumpSetValue(0)
        ElseIf Parameters_MainPumpRealValue <= MainPumpValue(i) Then
          value1 = MainPumpValue(i) - MainPumpValue(i - 1)
          value2 = Parameters_MainPumpRealValue - MainPumpValue(i - 1)
          value3 = MainPumpSetValue(i) - MainPumpSetValue(i - 1)
          aninp(4) = CShort(Minimum(MainPumpSetValue(i - 1) + CInt(value2 * value3 / value1), 0))
          Exit For
        ElseIf Parameters_MainPumpRealValue > MainPumpValue(4) Then
          aninp(4) = MainPumpSetValue(4)
        End If
      Next


      For j = 0 To 4
        If Parameters_Reel1RealValue <= Reel1Value(0) Then
          aninp(5) = Reel1SetValue(0)
        ElseIf Parameters_Reel1RealValue <= Reel1Value(i) Then
          value1 = Reel1Value(i) - Reel1Value(i - 1)
          value2 = Parameters_Reel1RealValue - Reel1Value(i - 1)
          value3 = Reel1SetValue(i) - Reel1SetValue(i - 1)
          aninp(5) = CShort(Minimum(Reel1SetValue(i - 1) + CInt(value2 * value3 / value1), 0))
          Exit For
        ElseIf Parameters_Reel1RealValue > Reel1Value(4) Then
          aninp(5) = Reel1SetValue(4)
        End If
      Next

      For k = 0 To 4
        If Parameters_Reel2RealValue <= Reel2Value(0) Then
          aninp(6) = Reel2SetValue(0)
        ElseIf Parameters_Reel2RealValue <= Reel2Value(i) Then
          value1 = Reel2Value(i) - Reel2Value(i - 1)
          value2 = Parameters_Reel2RealValue - Reel2Value(i - 1)
          value3 = Reel2SetValue(i) - Reel2SetValue(i - 1)
          aninp(6) = CShort(Minimum(Reel2SetValue(i - 1) + CInt(value2 * value3 / value1), 0))
          Exit For
        ElseIf Parameters_Reel2RealValue > Reel2Value(4) Then
          aninp(6) = Reel2SetValue(4)
        End If
      Next
      ' Copy counters directly
      HSCounter1 = ai(10)
      HSCounter2 = ai(11)
      Parameters_NozzlePressureRealValue = CType(ai(13), Short)
      If Parameters_NozzlePressureRealValue <= Parameters_NozzlePressureLowValue Then
        aninp(11) = Parameters_SetMainPressureLowValue
      Else
        value1 = Parameters_NozzlePressureHighValue - Parameters_NozzlePressureLowValue
        value2 = Parameters_NozzlePressureRealValue - Parameters_NozzlePressureLowValue
        value3 = Parameters_SetNozzlePressureHighValue - Parameters_SetNozzlePressureLowValue
        aninp(11) = CShort(Minimum(Parameters_SetNozzlePressureLowValue + CInt(value2 * value3 / value1), 0))
      End If

      Parameters_MainPressureRealValue = CType(ai(14), Short)
      If Parameters_MainPressureRealValue <= Parameters_MainPressureLowValue Then
        aninp(12) = Parameters_SetMainPressureLowValue
      Else
        value1 = Parameters_MainPressureHighValue - Parameters_MainPressureLowValue
        value2 = Parameters_MainPressureRealValue - Parameters_MainPressureLowValue
        value3 = Parameters_SetMainPressureHighValue - Parameters_SetMainPressureLowValue
        aninp(12) = CShort(Minimum(Parameters_SetMainPressureLowValue + CInt(value2 * value3 / value1), 0))
      End If

      aninp(13) = CShort(Minimum(aninp(11) - aninp(12), 0))
    End If
  End Function

  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Public Sub WriteOutputs(ByVal dout() As Boolean, ByVal anout() As Short)
    Static watchdogDout(128) As Boolean
    Array.Copy(dout, watchdogDout, dout.Length)
    watchdogDout(128) = (Date.UtcNow.Millisecond < 500)  ' alternate the last output to keep the plc happy
    ' M = Internal Relays, W = access as words
    Plc.Write(1, "WM272", watchdogDout, Ports.WriteMode.Optimised)

    ' Rescale: 100.0% = 255
    For i = 1 To 4
      anout(i) = CType((anout(i) * 255) \ 1000, Short)
    Next i
    For i = 5 To 17
      anout(i) = CType(anout(i), Short)
    Next i
    Plc.Write(1, "D400", anout, Ports.WriteMode.Optimised)

    '    Dim watchdogDout(128) As Boolean
    '    For i = 1 To 65
    ' watchdogDout(i) = dout(i)
    ' Next
    ' watchdogDout(128) = (Date.UtcNow.Millisecond < 500)  ' alternate the last output to keep the plc happy
    ' ' ' M = Internal Relays, W = access as words
    ' Plc.Write(1, "WM272", watchdogDout, Ports.WriteMode.Optimised)

    '    If ControlCode.Parameters.ConnectPhSystem = 1 Then
    ' Dim watchdogDout2(128) As Boolean
    ' For i = 1 To 17
    ' watchdogDout2(i) = dout(i + 200)
    ' Next
    ' watchdogDout2(128) = (Date.UtcNow.Millisecond < 500)  ' alternate the last output to keep the plc happy
    ' ' M = Internal Relays, W = access as words
    ' Plc2.Write(1, "WM272", watchdogDout2, Ports.WriteMode.Optimised)
    ' End If

    ' Rescale: 100.0% = 255
    ' For i = 1 To 4
    ' anout(i) = CType((anout(i) * 255) \ 1000, Short)
    ' Next i
    ' For i = 5 To 14
    ' anout(i) = CType(anout(i), Short)
    ' Next i
    'Plc.Write(1, "D400", anout, Ports.WriteMode.Optimised)
  End Sub

  Private ReadOnly ControlCode As ControlCode
  <EditorBrowsable(EditorBrowsableState.Advanced)> Public Plc, Plc2 As Ports.LA60B
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub

  Private Sub CheckForSerialPortParametersChanged()
    If LastComNumber1 <> ControlCode.Parameters.ComNumber1 OrElse LastComBaudRate1 <> ControlCode.Parameters.ComBaudRate1 Then
      ReOpenSerialPort()
    End If
  End Sub
  Private LastComNumber1, LastComBaudRate1 As Integer
  ' Private LastComNumber2, LastComBaudRate2 As Integer
  Private LastConnectPh As Integer
  Private Sub ReOpenSerialPort()
    If Plc IsNot Nothing Then DirectCast(Plc, IDisposable).Dispose() : Plc = Nothing
    '   If Plc2 IsNot Nothing Then DirectCast(Plc2, IDisposable).Dispose() : Plc2 = Nothing
    LastComNumber1 = ControlCode.Parameters.ComNumber1
    LastComBaudRate1 = ControlCode.Parameters.ComBaudRate1
    Plc = New Ports.LA60B(New Ports.SerialPort("COM" & LastComNumber1.ToString, LastComBaudRate1,
                                               System.IO.Ports.Parity.Even, 7, System.IO.Ports.StopBits.One))

  End Sub


  Private PlcTimeout As New Timer ' if no communications for 1 second, then make the fault below true
    Friend ReadOnly Property PlcFault() As Boolean
        Get
            Return PlcTimeout.Finished
        End Get
    End Property

  Private Plc2Timeout As New Timer ' if no communications for 1 second, then make the fault below true
  Friend ReadOnly Property Plc2Fault() As Boolean
    Get
      Return Plc2Timeout.Finished
    End Get
  End Property

  Private SmoothTimer As New Timer
  Private FirstScan As Boolean
  Private FirstScanTimer As New Timer
  Private FirstSmoothFinish As Boolean
  Public MainTemperatureSmooth(19) As Integer
  Public MainTemperatureSmoothTimes As Integer

End Class


Partial Public Class ControlCode
  Public ReadOnly IO As New IO(Me)
End Class

