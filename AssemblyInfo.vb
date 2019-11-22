Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

'Assembly info / properties
<Assembly: ComVisibleAttribute(False)> 

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: GuidAttribute("c7a7269f-e825-4aa2-aa88-1460d9e4435f")> 

<Assembly: AssemblyTitleAttribute("HJDOS109")> 
<Assembly: AssemblyDescriptionAttribute("")> 
<Assembly: AssemblyCompanyAttribute("Adaptive Control")> 
<Assembly: AssemblyProductAttribute("HJDOS109")> 
<Assembly: AssemblyCopyrightAttribute("Copyright ©  2008")> 
<Assembly: AssemblyTrademarkAttribute("")>

<Assembly: AssemblyVersionAttribute("8.05.*")>
<Assembly: AssemblyFileVersionAttribute("8.05")>

'Version 8.05 2019-11-22 George Lin
'修正馬達自動控制的邏輯

'Version 8.01 2019-09-25 George Lin
'F34新增參數主泵與帶布輪速度差，將主泵調整到固定速度時，帶布輪速度為主泵速度-速度差

'Version 7.9 2019-05-27 George Lin
'新增功能F34 設定布速

'Version 7.8 2019-04-17 George Lin
'加入噴壓、缸壓

'Version 7.6 2018-11-22 George Lin
'C缸計量加藥速度太慢時開大加藥

'Version 7.5 2018-11-19 George Lin
'增加F26流量計校正功能

'Version 7.4 2018-11-11 George Lin
'備藥警報改為1秒，纏車警報改為0.5秒
'to do
'增加功能，流量計校正
'增加參數漂後缸數紅、漂後缸數藍、漂後缸數其他。超過設定時，開始工單會警報

'Version 7.3 2018-10-22 George Lin
'主泵速度、布輪1速度與布輪2速度的校正改為5段

'Version 7.2 2018-10-17 George Lin
'增加主泵速度、布輪1速度與布輪2速度的校正

'Version 7.0 2018-09-10 George Lin
'增加F25 主缸水位校正

'Version 6.8 2018-05-29 George Lin
'取樣太久沒有跳下一步則重新發出警報
'增加進水異常的警報，如果有溢流水位，但是沒有高或中水位，則發出警報並且暫停

'Version 6.7 2018-04-07 George Lin
'新增升溫是否開加壓的參數

'Version 6.6 2018-02-03 George Lin
'新增溢流水洗後等待溢流排水時間，F12、F13、F15、F16、F18在水洗後會依照參數設定打開溢流閥

'Version 6.5 2018-01-08 George Lin
'纏車警報包含CycleTime超時

'Version 5.9 2017-08-14 George Lin
'如果升溫與加藥一起執行時，等待加藥開始才開始升溫，避免溫度過高無法加藥

'Version 5.8 2017-08-11 George Lin
'新增模擬入水量功能

'Version 5.7 2017-04-07 George Lin
'新增F30 主泵帶布輪控制

'Version 5.6 2017-03-31 George Lin
'修改讀取SQL資料的方式
'mimic的Cycle Time顯示變成4個


'Version 5.5 2017-03-23 George Lin
'讀取主泵速度、帶布輪速度

'Version 5.4 2016-12-05 George Lin
'入水量統計只有統計染色時的進水，也就是B缸加藥前的入水量

'Version 5.3 2016-10-22 George Lin
'Dosing時，如果藥缸實際水位與目標水位差異大時則發出異常訊息並且暫停加藥
'循環時間改為最大四個

'Version 5.2 2016-08-09 George Lin
'設定連接pH時會輸出DO70來連接pH的PLC

'Version 5.1 2016-07-30 George Lin
'進水超時警報

'Version 5.0 2016-07-15 George Lin
'增加流量進水功能，依照ERP的總浴量來進水，並且依照設定的比例扣掉藥缸加藥的水量

'Version 4.9 2016-06-13 George Lin
'修正F56 B dosing與F57 C dosing時如果沒有馬達訊號會自動跳段


'Version 4.6 2016-04-04 George Lin
'新增輸出點B不允許手動排水, C不允許手動排水

'Version 4.5 2015-12-25 George Lin
'降溫排壓後,排壓閥常開,到升溫時關閉
'pH設定值最小值改成3.00
'

'Version 4.4 2015-11-13 George Lin
'系統沒有升溫或是降溫時，則停機排水常開
'修正PLC未連線的異常訊息
'Cycle Time最小10秒鐘

'Version 4.3 2015-08-15 George Lin
'新增DO67, 帶布輪停止
'新增參數，帶布輪停止延遲時間

'Version 4.0 2015-02-01 George Lin
'修改加藥功能，按下備藥OK後才能開始加藥
'入降溫水超過參數設定的時間則改成入冷水+熱水

'Version 4.0 2014-09-09 George Lin
'新增功能，呼叫染料警報，呼叫助劑警報
'新增功能，取樣通知超時警報

'Version 3.9 2014-06-24 George Lin
'新增功能，氣冷溢流洗
'F55 C缸備藥改成背景功能

'Version 3.8 2014-02-21 George Lin
'新增參數，輸送染料延遲時間，LA-252輸送到染機時，當偵測到藥缸低水位開始計時，延遲時間到達即判定計量完成
'染程結束時會更新資料庫中的Machine Table，將資料都重置

'Version 3.7 2013-01-29 George Lin
'新增F23溫度到達呼叫以及升溫超時警報

'Version 3.6 2012-05-03 George Lin
'修正自動呼叫的異常，在備藥以及結束染程時將ChemicalState=101, DyeState=101

'Version 3.5 2012-04-11 George Lin
'增加SQL資料庫連線功能，取代AutoDispenser功能
'增加升溫中開排壓的參數


'Version 3.4 2012-03-04 George Lin
'增加纏車次數的統計
'修正等待備藥時，HeatNow跟CoolNow兩個變數會一直交替啟動的問題，造成冷卻排水一直開 

'Version 3.3 2011-03-22 George Lin
'修正C缸加藥迴水檢測時沒有迴水的問題 
'mimic增加布速的計數，方便使用者檢視布速的計算是否正常
'修改F54,F55，備藥時如果有呼叫LA-252或是LA-302，等到呼叫的State變成202，則將Call Off跟Tank都變成0，避免因為訊息延遲造成重複叫料

'Version 3.2 2010-08-06 George Lin
'修正計量結果為302時，Call off跟Tank不會歸零 

'Version 3.1 2010-05-24 George Lin
'增加溫度補償參數，使用者可以依照實際狀況修改溫度 

'Version 3.0 2009-11-23 George Lin
'增加布速、主馬達轉速、布輪轉速的紀錄 

'Version 2.9 2009-10-22 George Lin
'修正B C缸備藥時，溫度控制程式沒有停掉的問題 

'Version 2.8 2009-09-07 George Lin
'修正2.7版語言問題
'特殊版，降溫水洗時先關馬達，開降溫水20秒後再開馬達

'Version 2.7 2009-05-27 George Lin
'新增功能，增加外部按鈕控制染程跳步

'Version 2.6 2009-04-29 George Lin
'新增功能，如果程式閒置過久則自動停止程式


'Version 2.5 2009-04-14 George Lin
'新增IO點，布頭訊號
'新增功能，command10 溫度控制，持溫時間改用圈數計算


'Version 2.4 2009-03-28 George Lin
'降溫水洗增加水源參數選擇


'Version 2.3 2009-03-23 George Lin
'修正降溫的類比控制方式
'修正修改持溫時間後，自動計算目前的持溫時間
'新增兩個參數，控制降溫水洗時的降溫閥開度

'Version 2.2 2009-03-16 George Lin
'昇級為 BC 3.2.44
'修正類比溫控閥開度，固定10段
'新增F17，副缸溫度水洗

'Version 2.1 2009-02-24 George Lin
'修正BC 3.2.41，PLC通訊問題

'Version 2.0 2009-02-21 George Lin
'由於V1.9使用BC 3.2.41，PLC通訊有問題 ，故改回BC 3.2.26

'Version 1.9 2009-02-13 George Lin
'修改藥缸IO的邏輯，加上BC藥缸參數的變化
'使用BC 3.2.41，但是PLC通訊有問題 

'Version 1.8 2009-01-21 George Lin
'變更mimic

'Version 1.7 2009-01-02 George Lin
'修正DO升溫控制運算

'Version 1.6 2008-11-19 George Lin
'  呼叫染料備藥改成平行功能
'  增加OnOff閥昇溫控制

'Version 1.5 2008-11-18 George Lin
'  修改F59 等待備藥，由等待Dstate 301改成等待水位到達20%

'Version 1.4 2008-11-14 George Lin
'  F64 與 F65 改成平行備藥
'  F66 與 F67 改成平行加藥


'Version 1.3 2008-10-15 George Lin
'  新增F58 呼叫染料備藥 與 F59 等待染料備藥

'Version 1.2 2008-08-23 ML
'  Added screen 4 and 5 to display dye products and chemical products
'  Re-organized the code files into folders
'  Fixed CallOff bug - set CallOff=0 after response received

'Version 1.1 2008-08-22 ML
'  Put auto dispense code in prepare commands
'  Chemical & Dye CallOff, Tank, State & Products variables are all defined in ControlCode - for simplicity

'Version 1.0 2008-07-28 MW
'  First cut of control code