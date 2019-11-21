Public Class PhControl_

    Public Enum PhControl
        Off
        DownloadParameter
        Alarm_TmepHigh
        AlarmPhHigh
        Divider
        AddHacError
        MathAddHac2
        MathAddHac3
        MathAddHac4
        AllAddHac
        CheckPhValue
        Pause
        WaitTempFinish
        Finished
        WaitTempArrival
        WaitKeepTime
        AllPhWaitTime
    End Enum

    Public Wait As New Timer
    Public Wait1, WaitPhAllTime, WaitAllAdd3min As New Timer
    Public WaitPhSample As New Timer
    Public WaitCheckPH As New Timer
    Public Wait10Second As New Timer
    Public WaitAddHac As New Timer
    Public TmepTime As New Timer
    Public WaitCheckTime As New Timer
    Public C37 As Integer
    Public StateString As String
    Public CalculateTmep As Integer                     '溫度差
    Public TotalAddHac As Integer                        'HAC總量
    Public TotalAddHac2 As Integer                        'HAC總量
    Public TotalAddHac3 As Integer                        'HAC總量
    Public TotalAddHac4 As Integer                        'HAC總量
    Public C01TargetTemp As Integer                     '到達溫度
    Public C01Gradient As Integer                       '斜率
    Public C75Gradient As Integer                       'PH目標值
    Public C76AddTime As Integer                        '加的時間
    Public C76OpenTemp As Integer                       '起始溫度
    Public C77KeepTime As Integer
    Public PhConcentration As Double                  '濃度
    Public PhPumpOutRatio As Double                    'PH泵加酸比
    Public PhFillLevel As Double                        '進水量
    Public PhFillLevel2 As Integer                         '進水量
    Public PhRangeValue, PhRangeValue2, PhRangeValue3, PhRangeValue4, PhRangeValue5, PhRangeValue6, ExpectPh, ExpectPh2 As Integer
    Public PhSamplingTime As Integer                    'PH取樣時間
    Public FirstPhValue, FirstTemp As Integer                      '初始讀取PH實際值
    Public Register(10) As Integer                      '暫存器
    Public RegisterD(12) As Double                       '暫存器
    Public RegisterI(21) As Integer                     '暫存器
    Public PhAddError As Integer                        '
    Public CalculateTmepRange As Integer                '加酸錯誤演算溫度差的總數
    Public OpenHacValve As Boolean                      '開加藥閥
    Public CountHacVolume As Integer                    '計算HAC加的總量
    Public PauseCode As Integer                             '
    'Public X As Integer
    Private PerPhValue(1000) As Integer
    Private FreeTime(1000) As Integer
    Private TotalValue(1000) As Integer
    Public X, MathNeverOpenValue As Integer
    Public CheckPhRun, OpenKeepTime As Boolean
    Public AddOverTimes As Integer
    Public AddOverError As Boolean
    Public WaitforSecond As New Timer
    Public 開啟PH控制旗標 As Boolean
    Public 錯誤紀錄 As Integer
    Public 未達到演算值, 未達到演算值1, 次數 As Integer
    Public PH減酸Flag2, PH減酸Flag, W0微量補酸 As Boolean
    Public 減酸比率, 微調次數, 微調比率減少, 補酸次數 As Integer




    Public Sub Run()
    Try
      With ControlCode
        Select Case State
          Case PhControl.Off
            次數 = 4
            減酸比率 = 0
            微調次數 = 0
            補酸次數 = 0
            微調比率減少 = 0
            PH減酸Flag = False
            PH減酸Flag2 = False
            Wait.TimeRemaining = 0
            開啟PH控制旗標 = False
            If .是否縮短檢測時間 = True Then
              WaitCheckTime.TimeRemaining = 10
            Else
              WaitCheckTime.TimeRemaining = .Parameters.StartCheckPh
            End If
            State = PhControl.DownloadParameter
            '======================================================================================載入參數
          Case PhControl.DownloadParameter
            CheckPhRun = False
            StateString = If(.Language = LanguageValue.ZhTW, "確認目前PH值，檢查時間剩餘 " & WaitCheckTime.TimeRemaining & " 秒", "Check PH " & WaitCheckTime.TimeRemaining & " S")
            '----------------------------------------------------------'等待PH確定值
            If Not WaitCheckTime.Finished Then Exit Select
            '-----------------------------------------------------------'等待藥桶低水位
            StateString = If(.Language = LanguageValue.ZhTW, "等待PH循環桶低水位", "Waiting for B tank low level")
            If .Parameters.PhCirTank = 0 Then

            Else
              If Not .IO.PhMixTankLowLevel Then Exit Select
            End If

            '------------------------------------------------------------------
            StateString = ""
            For i = 0 To 10
              Register(i) = 0
            Next
            For i = 0 To 12
              RegisterD(i) = 0
            Next
            For i = 0 To 21
              RegisterI(i) = 0
            Next

            For i = 0 To 1000
              PerPhValue(i) = 0
              FreeTime(i) = 0
              TotalValue(i) = 0
            Next

            .UseHacAllTotal = 0
            CountHacVolume = 0                                              '統計加HAC總量 歸零
            ExpectPh = 0
            ExpectPh2 = 0
            PhAddError = 0

            '---------------------------------------------------------------------------------------------------------------------------------------
            If .Command75.State = Command75.S75.Start Then

              If Not .Setpoint > 10 Then Exit Select

              C01TargetTemp = .Command01.TargetTemp                                   '到達溫度
              C01Gradient = .Command01.Gradient                                       '斜率
              C75Gradient = .Command75.PhTarget                                       'PH目標值
              '-----------------------------------20120103Vincent
              C76OpenTemp = 100
              '----------------------------------
              If .IO.MainTemperature > C01TargetTemp Then
                '------------------------------實際溫度 > 設定溫度
                StateString = If(.Language = LanguageValue.ZhTW, "實際溫度高於設定溫度", "MainTemperature is high than TargetTemperature")
                State = PhControl.DownloadParameter

                Exit Select

              Else
                '------------------------------實際溫度 < 設定溫度
                StateString = ""
                If .IO.pHValue < C75Gradient Then
                  State = PhControl.AlarmPhHigh
                Else

                  State = PhControl.Divider
                End If

              End If



              '---------------------------------------------------------------------------------------------------------------------------------------
            ElseIf .Command74.State = Command74.S74.Start Then

              C01Gradient = 0

              C75Gradient = .Command74.PhTarget                                       'PH目標值
              '-----------------------------------20120103Vincent
              C76OpenTemp = 100
              '----------------------------------
              State = PhControl.Divider
              '---------------------------------------------------------------------------------------------------------------------------------------
            ElseIf .Command76.State = Command76.S76.Start Then

              C75Gradient = .Command76.PhTarget                                       'PH目標值
              C76AddTime = .Command76.AddTime
              C76OpenTemp = .Command76.PhOpenTemp * 10

              State = PhControl.WaitTempArrival
              '---------------------------------------------------------------------------------------------------------------------------------------
            ElseIf .Command80.State = Command80.S80.Start Then

              C75Gradient = .Command80.PhTarget                                       'PH目標值
              C76AddTime = .Command80.AddTime
              C76OpenTemp = 150

              State = PhControl.WaitTempArrival
              '---------------------------------------------------------------------------------------------------------------------------------------
            ElseIf .Command77.State = Command77.S77.Start Or .Command77.State = Command77.S77.KeepTime Then

              C75Gradient = .Command77.PhTarget                                       'PH目標值
              '-----------------------------------20120103Vincent
              C76OpenTemp = 100
              '----------------------------------
              OpenKeepTime = False

              State = PhControl.WaitTempArrival

              '---------------------------------------------------------------------------------------------------------------------------------------
            Else

              Exit Select
              '---------------------------------------------------------------------------------------------------------------------------------------
            End If

            '====================================================================================
          Case PhControl.WaitTempArrival

            If .IO.MainTemperature <= C76OpenTemp Then
              StateString = If(.Language = LanguageValue.ZhTW, "等待溫度" & (C76OpenTemp / 10) & "度，才執行PH控制", "Wait for" & (C76OpenTemp / 10) & "C，then PH control")
              State = PhControl.WaitTempArrival
              Exit Select
            Else
              State = PhControl.Divider
            End If



            '====================================================================================超過偈實際值低於PH設定值會提示警告
          Case PhControl.AlarmPhHigh
            StateString = If(.Language = LanguageValue.ZhTW, "PH實際值低於PH設定值", "PH Value Error")
            If .IO.pHValue > (C75Gradient + .Parameters.PhApproach) Then
              State = PhControl.Divider
            End If
            '====================================================================================區分"配合斜率"，計量加HAC  或  直加HAC
          Case PhControl.Divider



            開啟PH控制旗標 = True

            '-----------------------------------------------------------------------載入參數(濃度、PH泵加酸比、進水量)
            PhConcentration = CType((.Parameters.PhConcentration / 100), Double)   '濃度

            '***************************************************************************************2011.9.20   改成X2
            PhPumpOutRatio = CType((.Parameters.PhPumpOutRatio * 3) / 60, Double) 'PH泵加酸比
            '***************************************************************************************

            '---------------------------此部份如果 進水量 濃度 PH泵加酸比 不得為0,如果為0將修改為1
            If .PhFillLevel = 0 Then

              PhFillLevel = CType(.Parameters.MainTankFillLevel / 2000, Double)      '進水量
            Else
              PhFillLevel = CType(.PhFillLevel / 2000, Double)                       '進水量
            End If

            If PhPumpOutRatio = 0 Then
              PhPumpOutRatio = 1
            End If
            If PhConcentration = 0 Then
              PhConcentration = 1
            End If

            WaitAllAdd3min.TimeRemaining = 0
            '-----------------------------------------------------------------------如果 實際PH值 已經低於 PH目標值 時，跳到PH確認部分 PhControl.CheckPhValue

            If .IO.pHValue <= (C75Gradient + .Parameters.PhApproach) Then           'PH實際值 < = PH設定值

              WaitCheckPH.TimeRemaining = .Parameters.PhSamplingTime              'PH取樣時間
              State = PhControl.CheckPhValue
              Exit Select
            End If
            '------------------------------------
            StateString = ""

            TotalAddHac = St.GetAmount(.IO.pHValue, C75Gradient)               '加酸所需要總量 =（ 現在PH值 , PH目標值 ）

            FirstPhValue = .IO.pHValue

            TotalAddHac2 = CType(((TotalAddHac * PhFillLevel) / PhPumpOutRatio) / PhConcentration, Integer)
            '加酸總量 （加成後確認） =  ((( 實際PH - 設定PH ) ＊ 進水量比率 ) / PH泵加酸比率 / 濃度比率 )
            If C75Gradient < 450 Then
              TotalAddHac3 = St.GetAmount(450, C75Gradient)               '加酸所需要總量 =（ 現在PH值 , PH目標值 ）

              TotalAddHac4 = CType(((TotalAddHac3 * PhFillLevel) / PhPumpOutRatio) / PhConcentration, Integer)
            End If



            Wait.TimeRemaining = TotalAddHac2


            '------------------------------------------------------區分 (1)有斜率 就計量加藥 (2)沒有斜率 就直接加藥

            If (((.Command01.Gradient <> 0 Or .Command76.State = Command76.S76.Start) And .Command01.IsOn = True) Or .Command80.State = Command80.S80.Start) Then
              '================================================================================================ 'PH控制模式 = 0

              CheckPhRun = False
              '-----------------------------------------------------------------.Parameters.PhControlMode = 1---F01溫度控制,F75跟溫度跑PH
              If (.Command75.State = Command75.S75.Start) Then
                CalculateTmep = C01TargetTemp - .IO.MainTemperature
                '演算溫度200  = 設定溫度1000  -  實際溫度800

                RegisterD(0) = CType(CalculateTmep / C01Gradient, Double)
                '升溫部分所需要的總時間 = ( 設定溫度-目前溫度 ) / 斜率
                CalculateTmepRange = CType(RegisterD(0) * 60, Integer)

              ElseIf (.Command76.State = Command76.S76.Start) Then
                RegisterD(0) = C76AddTime
                CalculateTmepRange = CType(RegisterD(0) * 60, Integer)

              ElseIf (.Command80.State = Command80.S80.Start) Then

                CalculateTmepRange = .Command80.Wait.TimeRemaining

              ElseIf (.Command74.State = Command74.S74.Start) Then


              ElseIf (.Command77.State = Command77.S77.Start) Then

                If C76AddTime = 0 Then
                  State = PhControl.AllAddHac
                  Exit Select

                Else
                  RegisterD(0) = C76AddTime
                  CalculateTmepRange = CType(RegisterD(0) * 60, Integer)

                End If



              End If



              '升溫部分所需要的總時間 秒

              '-------------------------------------------------------------------升溫部分所需要的總時間(秒)  <   加酸總量 
              If CalculateTmepRange < (TotalAddHac2 + 30) Then

                Register(3) = TotalAddHac2 - .Parameters.PhSamplingTime
                If .IO.pHValue > 800 Then
                  WaitPhAllTime.TimeRemaining = 8
                ElseIf .IO.pHValue > 700 Then
                  WaitPhAllTime.TimeRemaining = 8
                ElseIf .IO.pHValue > 600 Then
                  WaitPhAllTime.TimeRemaining = 7
                ElseIf .IO.pHValue > 500 Then
                  WaitPhAllTime.TimeRemaining = 7
                Else
                  WaitPhAllTime.TimeRemaining = 5
                End If


                State = PhControl.AllPhWaitTime

                Exit Select
              End If
              '-------------------------------------------------------------------
              Dim Y, Y2, Z As Integer



              X = FirstPhValue - C75Gradient

              Z = FirstPhValue

              For i = 1 To X

                Y = St.GetAmount(Z, Z - 1)

                Y2 = CType(((Y * PhFillLevel) / PhPumpOutRatio) / PhConcentration, Integer)

                PerPhValue(i) = Y2                          'PerPhValue(i) 每0.01pH 的 使用量

                Z = Z - 1

              Next

              '%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
              Dim G1, G2, G3, G4, 每段總時間, G6, G7, G8 As Integer
              Dim X2 As Integer

              G1 = St.GetAmount(FirstPhValue, C75Gradient)

              G2 = CType(((G1 * PhFillLevel) / PhPumpOutRatio) / PhConcentration, Integer)
              '加酸總量
              '%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
              G3 = 0
              For i = 1 To X

                G3 = G3 + PerPhValue(i)                 'G3 = 加酸總量

              Next

              G4 = CalculateTmepRange - G3            '空餘秒數 = 升溫部分所需要的總時間 秒 - 加酸總量
              If G4 < 0 Then
                G4 = 0
              End If

              G7 = CalculateTmepRange                 '升溫總時間

              X2 = X
              '==========================================================分配量((KeyPoint))
123:
              每段總時間 = CType(G7 / X2, Integer)                '每段總時間 = 總時間 / 加酸總段數

              For i = 1 To X2

                FreeTime(i) = 每段總時間 - PerPhValue(i)

                If FreeTime(i) < 0 Then
                  FreeTime(i) = 0
                End If

                G6 = G6 + FreeTime(i)
              Next
              '===========================================將超出的時間,扣除
              Dim 初算加總時間, 總時間差, 找出最大值 As Integer
              初算加總時間 = 每段總時間 * X
              If 初算加總時間 > CalculateTmepRange Then
                總時間差 = 初算加總時間 - CalculateTmepRange

                For i = 1 To X2
                  If PerPhValue(i) = 3 Then
                    找出最大值 = i
                    i = X2
                  End If
                Next
                If 找出最大值 < 總時間差 Then
                  總時間差 = 找出最大值
                End If


                For i = 1 To 總時間差
                  FreeTime(i) = FreeTime(i) - 1
                Next



              End If
              '===========================================

              ' X (加酸總段數) =4    ,   G5 (每段時間)=10
              '----------------------------------------------
              '          空時間    加酸
              ' X(1)      4S        6S
              ' X(2)      3S        7S
              ' X(3)      2S        8S
              ' X(4)      1S        9S
              '----------------------------------------------
              '           10        30


              '----------------------------------------------
              '          空時間    加酸
              ' X(1)      4S        6S
              ' X(2)      2S        8S
              ' X(3)      0S        10S
              ' X(4)      0S        11S
              '----------------------------------------------
              '           6        35
              '  G6(6) > G4(5)  X-1變成X1~X3   =>  跳到 GoTo 123
              If G6 > G4 Then

                X2 = X2 - 1

                G7 = G7 - PerPhValue(X)

                G6 = 0

                GoTo 123

              End If
              '==========================================================
              For i = 1 To X

                TotalValue(i) = PerPhValue(i) + FreeTime(i)
                G8 = G8 + TotalValue(i)

              Next


              RegisterI(1) = 1        '每次減依次PH值

              Wait1.TimeRemaining = G8

              MathNeverOpenValue = 0

              State = PhControl.MathAddHac2




              '-------------------------------------------------------------------------------------------------------------------------
            Else



              Register(3) = TotalAddHac2 - .Parameters.PhSamplingTime
              If Not WaitforSecond.Finished Then Exit Select
              If C75Gradient >= 800 Then
                If .IO.pHValue - C75Gradient < 50 Then      '----------PH值接近目標值0.5
                  WaitAllAdd3min.TimeRemaining = 1
                  WaitforSecond.TimeRemaining = 12
                Else    '---------------------------------PH大於目標值很多
                  WaitAllAdd3min.TimeRemaining = 1
                  WaitforSecond.TimeRemaining = 8
                End If

              ElseIf C75Gradient >= 700 Then
                If .IO.pHValue - C75Gradient < 50 Then      '----------PH值接近目標值0.5
                  WaitAllAdd3min.TimeRemaining = 2
                  WaitforSecond.TimeRemaining = 12
                Else    '---------------------------------PH大於目標值很多
                  WaitAllAdd3min.TimeRemaining = 2
                  WaitforSecond.TimeRemaining = 8
                End If
              ElseIf C75Gradient >= 600 Then
                If .IO.pHValue - C75Gradient < 50 Then      '----------PH值接近目標值0.5
                  WaitAllAdd3min.TimeRemaining = 3
                  WaitforSecond.TimeRemaining = 12
                Else    '---------------------------------PH大於目標值很多
                  WaitAllAdd3min.TimeRemaining = 3
                  WaitforSecond.TimeRemaining = 8
                End If
              ElseIf C75Gradient >= 500 Then
                If .IO.pHValue - C75Gradient < 50 Then      '----------PH值接近目標值0.5
                  WaitAllAdd3min.TimeRemaining = 4
                  WaitforSecond.TimeRemaining = 12
                Else    '---------------------------------PH大於目標值很多
                  WaitAllAdd3min.TimeRemaining = 3
                  WaitforSecond.TimeRemaining = 8
                End If

              Else

                WaitAllAdd3min.TimeRemaining = 5
                WaitforSecond.TimeRemaining = 10
              End If


              State = PhControl.AllAddHac
            End If

            '************************************************************************************************************************計量加HAC 2
          Case PhControl.MathAddHac2



            ExpectPh = FirstPhValue - RegisterI(1)
            'PH設定值 = 最初讀取PH值 - 每次減依次PH值
            '6.99     =     7.00     -  0.01

            CheckPhRun = True

            If RegisterI(1) >= X Then
              State = PhControl.CheckPhValue
              C76AddTime = 0
              Exit Select
            End If
            '***********************************************************2011.9.20
            If AddOverTimes >= 1 Then

              Dim PhValve As Double

              If PerPhValue(RegisterI(1)) > 1 Then

                PhValve = PerPhValue(RegisterI(1)) / 2

                PerPhValue(RegisterI(1)) = CType(PhValve, Integer)

                FreeTime(RegisterI(1)) = FreeTime(RegisterI(1)) + CType(PhValve, Integer)


              End If


            Else


            End If

            '**********************************************************
            WaitAddHac.TimeRemaining = PerPhValue(RegisterI(1))
            '每階段加酸時間      =   每階段加酸量(每次減依次PH值)
            '==================================================================================================20120629起
            If .IO.pHValue < ExpectPh - .Parameters.PhApproach Or .IO.pHValue < C75Gradient Or WaitAddHac.TimeRemaining <= 0 Then
              OpenHacValve = False

            Else
              OpenHacValve = True

            End If
            '==================================================================================================20120629底

            Wait10Second.TimeRemaining = PerPhValue(RegisterI(1)) + FreeTime(RegisterI(1))
            '每階段PH設定值時間       =   每階段加酸量(每次減依次PH值) +  空於時間(每次減依次PH值)



            If WaitAddHac.TimeRemaining = 0 And PH減酸Flag2 = False Then '-----------此部分是如果三次都沒開閥,則開一秒鐘的閥---------------------------------
              MathNeverOpenValue += 1
              If MathNeverOpenValue >= 次數 Then
                MathNeverOpenValue = 0
                WaitAddHac.TimeRemaining = 1
                '=============================PH值 7.0 > 設定值6.9 的狀況 (需要加快補酸)
                Dim 預定範圍值 As Integer
                If .IO.pHValue > 700 Then
                  預定範圍值 = (ExpectPh + 20)
                ElseIf .IO.pHValue > 600 Then
                  預定範圍值 = (ExpectPh + 15)
                Else
                  預定範圍值 = (ExpectPh + 10)
                End If

                If .IO.pHValue > 預定範圍值 Then
                  未達到演算值 = 未達到演算值 + 1
                  未達到演算值1 = 0
                  If 未達到演算值 >= 2 Then
                    次數 = 次數 - 1
                    If 次數 <= 1 Then
                      次數 = 1
                    End If
                    '(((((((((((((((((((((((((((((((((((((主要是次數=1,PH降不下去,要再補酸
                    If W0微量補酸 = True Then

                      Dim 總量, 加酸量, 單次加酸量 As Integer

                      總量 = FreeTime(RegisterI(1)) + PerPhValue(RegisterI(1))
                      加酸量 = PerPhValue(RegisterI(1))
                      If 總量 = 加酸量 Then
                      ElseIf 總量 > 加酸量 Then
                        If (.IO.pHValue - ExpectPh) > 30 Then
                          單次加酸量 = CType(總量, Integer)
                          WaitAddHac.TimeRemaining = 單次加酸量
                        ElseIf (.IO.pHValue - ExpectPh) > 20 Then
                          單次加酸量 = CType(總量 * 0.8, Integer)
                          WaitAddHac.TimeRemaining = 單次加酸量
                        ElseIf (.IO.pHValue - ExpectPh) > 10 Then
                          單次加酸量 = CType(總量 * 0.5, Integer)
                          WaitAddHac.TimeRemaining = 單次加酸量
                        Else
                          單次加酸量 = CType(總量 * 0.3, Integer)
                          WaitAddHac.TimeRemaining = 單次加酸量

                        End If
                      End If

                    End If
                    If 次數 = 1 Then
                      W0微量補酸 = True
                    End If

                    '))))))))))))))))))))))))))))))))))))
                    未達到演算值 = 0
                  End If
                Else
                  W0微量補酸 = False
                End If

                '============================PH值 7.0 < 設定值7.5 的狀況 (需要減少補酸)


                If .IO.pHValue > 700 Then
                  預定範圍值 = (ExpectPh - 20)
                ElseIf .IO.pHValue > 600 Then
                  預定範圍值 = (ExpectPh - 15)
                Else
                  預定範圍值 = (ExpectPh - 10)
                End If

                If .IO.pHValue < 預定範圍值 Then
                  未達到演算值1 = 未達到演算值1 + 1
                  未達到演算值 = 0
                  If 未達到演算值1 >= 2 Then
                    次數 = 次數 + 1
                    If 次數 >= 4 Then
                      次數 = 4
                    End If
                    未達到演算值1 = 0
                  End If
                End If
              End If

              '==========================================
            Else '------------------------------------------------------加酸的補償值---------------------------------
              ' Wait10Second.TimeRemaining = PerPhValue(RegisterI(1)) + FreeTime(RegisterI(1))

              If RegisterI(1) <= X Then
                微調酸控量()
              End If


              If PerPhValue(RegisterI(1)) < 0 Then
                WaitAddHac.TimeRemaining = 0
              Else
                WaitAddHac.TimeRemaining = PerPhValue(RegisterI(1))
              End If
              Wait10Second.TimeRemaining = PerPhValue(RegisterI(1)) + FreeTime(RegisterI(1))

            End If

            State = PhControl.MathAddHac3

            Exit Select
            '==============================================================================================================詳細計量加HAC 2
          Case PhControl.MathAddHac3
            StateString = ""

            If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Or .IO.pHTemperature1 > 1050 Then
              State = PhControl.Pause
              Wait1.Pause()
              Exit Select
            End If


            If WaitAddHac.TimeRemaining < 0 Then
              WaitAddHac.TimeRemaining = 0
            End If
            If Not WaitAddHac.Finished Then
              If .IO.pHValue - 5 < ExpectPh Or .IO.pHValue < C75Gradient Then        ' 800<797
                OpenHacValve = False

              Else
                OpenHacValve = True
                '***********************************************************2011.9.20

                AddOverError = True

                '***********************************************************
              End If
            End If


            State = PhControl.MathAddHac4
            Exit Select
            '==============================================================================================================詳細計量加HAC 2
          Case PhControl.MathAddHac4
            If WaitAddHac.TimeRemaining < 0 Then
              WaitAddHac.TimeRemaining = 0
            End If
            If WaitAddHac.Finished Then                         '開閥 HAC 比方說7秒

              OpenHacValve = False                            '關閉 HAC

              If Wait10Second.Finished Then                   '等待10秒時間

                State = PhControl.MathAddHac2

                RegisterI(1) = RegisterI(1) + 1

                '***********************************************************2011.9.20
                If AddOverError = False Then
                  AddOverTimes = AddOverTimes + 1
                Else
                  AddOverTimes = 0
                  AddOverError = False
                End If

                '***********************************************************
                Exit Select

              End If
              State = PhControl.MathAddHac3
              Exit Select
            End If
            State = PhControl.MathAddHac3
            Exit Select

            '====================================================
          Case PhControl.AllPhWaitTime
            If Not WaitPhAllTime.Finished Then Exit Select
            If .IO.pHValue > 800 Then
              WaitAllAdd3min.TimeRemaining = 2
            ElseIf .IO.pHValue > 700 Then
              WaitAllAdd3min.TimeRemaining = 3
            ElseIf .IO.pHValue > 600 Then
              WaitAllAdd3min.TimeRemaining = 4
            ElseIf .IO.pHValue > 500 Then
              WaitAllAdd3min.TimeRemaining = 5
            Else
              WaitAllAdd3min.TimeRemaining = 6
            End If

            State = PhControl.AllAddHac

            '===================================================================================================================================直加HAC
          Case PhControl.AllAddHac
            StateString = ""

            If WaitAllAdd3min.Finished Then
              State = PhControl.Divider
              Exit Select
            End If

            PauseCode = 2

            If .IO.pHValue > (C75Gradient + .Parameters.PhApproach) Then                   ''PH實際值 < = PH設定值
              '--------------------------------------------------------------------------升溫時，系統暫停-----------------------------
              If .Parent.IsPaused Or .Command01.State = Command01.S01.Pause Or Not .IO.MainPumpFB Then
                Wait.Pause()
                'State = PhControl.Pause
                Exit Select
              End If
              '------------------------------------------------------------------------------------------------------------------------
              StateString = If(.Language = LanguageValue.ZhTW, "計量加酸中", "ADD HAC")
              '----------------------------------------------------此部分，是在運算加酸後，大概PH值會落在多少值，預估PH直


              'PhRangeValue = TotalAddHac2 - Wait.TimeRemaining


              'PhRangeValue2 = CType(((PhRangeValue * PhConcentration) * PhPumpOutRatio) / PhFillLevel, Integer)
              ''預設值使用量  =        目前所加酸的量    濃度               PH泵加酸比         水量

              'PhRangeValue3 = St.GetAmount(1000, FirstPhValue)

              'PhRangeValue4 = PhRangeValue3 + PhRangeValue2
              ''預計猜測到達PH值  = 基準點PH值 +  加酸的量

              'PhRangeValue5 = PhRangeValue4 - .Parameters.DoublePhSample
              ''預計確定到達PH直  = 預計猜測到達PH值 - 延遲等待PH穩定值

              'If .Command77.State = Command77.S77.Start Or .Command77.State = Command77.S77.KeepTime Then
              'Else

              '    ExpectPh = St.GetAmount1(PhRangeValue5)
              'End If



              'ExpectPh = St.GetAmount1(PhRangeValue5)
              '此部分是所加的酸，目前應該到達的PH值


              '--------------------------------------------------------------------------

              If Wait.TimeRemaining = Register(3) Then


                Register(3) = Register(3) - .Parameters.PhSamplingTime


                If .IO.pHValue < ExpectPh And FirstPhValue > .IO.pHValue Then
                  PhAddError = 0


                Else
                  If .Parameters.PhAddError = PhAddError Then


                    State = PhControl.AddHacError


                  Else
                    PhAddError = PhAddError + 1

                    State = PhControl.Divider
                  End If


                End If

              End If


              If Not Wait.Finished Then Exit Select
              State = PhControl.CheckPhValue
              Wait.TimeRemaining = 0
              WaitPhSample.TimeRemaining = 0
              WaitCheckPH.TimeRemaining = .Parameters.PhSamplingTime


            Else
              State = PhControl.CheckPhValue
              Wait.TimeRemaining = 0
              WaitPhSample.TimeRemaining = 0
              WaitCheckPH.TimeRemaining = .Parameters.PhSamplingTime

            End If

            '==================================================================================================================================檢測PH值
          Case PhControl.CheckPhValue

            PauseCode = 0

            StateString = If(.Language = LanguageValue.ZhTW, "檢測PH值 ,等待", "ChECK PH VALVE") & WaitCheckPH.TimeRemaining & "秒"

            If .IO.pHValue > (C75Gradient + .Parameters.PhApproach) Then

              WaitCheckPH.Restart()
              State = PhControl.Divider
            End If

            If Not WaitCheckPH.Finished Then Exit Select
            If .IO.pHValue <= (C75Gradient + .Parameters.PhApproach) Then
              StateString = If(.Language = LanguageValue.ZhTW, "PH值控制 - 目前 ", "PH CONTROL  ") & (.IO.pHValue / 100) & " PH "
              WaitCheckPH.TimeRemaining = .Parameters.PhSamplingTime
              State = PhControl.WaitTempFinish

            Else
              State = PhControl.Divider

            End If
            '=====================================================================================================================================
          Case PhControl.WaitTempFinish
            If .Command01.State <> Command01.S01.Off Then

              StateString = If(.Language = LanguageValue.ZhTW, "PH值控制 - 目前 ", "PH CONTROL  ") & (.IO.pHValue / 100) & " PH "

              If Not WaitCheckPH.Finished Then Exit Select

              If .IO.pHValue <= (C75Gradient + .Parameters.PhApproach) Then


                State = PhControl.WaitTempFinish

              Else
                State = PhControl.Divider

              End If
            ElseIf .Command77.State = Command77.S77.Start Or .Command77.State = Command77.S77.KeepTime Then
              OpenKeepTime = True
              State = PhControl.Divider
              'ElseIf .Command01.State = Command01.S01.Off Then
              '    State = PhControl.Finished
            End If

            '=====================================================================================================================================
          Case PhControl.Finished

            '=====================================================================================================================================
          Case PhControl.AddHacError
            StateString = If(.Language = LanguageValue.ZhTW, "加酸動作異常，請檢查設備！！", "ADD HAC ERROR")
            If .IO.CallAck Then
              State = PhControl.Divider
            End If

            '=====================================================================================================================================
          Case PhControl.Pause

            StateString = If(.Language = LanguageValue.ZhTW, "PH加酸控制暫停", "Paused")
            'no longer pause restart the timer and go back to the wait state
            If (Not .Parent.IsPaused) And .IO.MainPumpFB And .IO.SystemAuto And .IO.pHTemperature1 < 1050 Then
              Wait1.Restart()
              State = PhControl.MathAddHac3


              'If PauseCode = 1 Then
              '    State = PhControl.Divider

              'ElseIf PauseCode = 2 Then
              '    Wait.Restart()
              '    State = PhControl.AllAddHac

              'End If
            End If
            '=====================================================================================================================================



        End Select


      End With

    Catch ex As Exception
      錯誤紀錄 = 錯誤紀錄 + 1
      'Ignore errors
    End Try
    End Sub
    Public Sub 微調酸控量()
        With ControlCode

            If .IO.PhValue < ExpectPh And Not PH減酸Flag Then

                Select Case ExpectPh - .IO.PhValue
                    Case Is > 20
                        減酸比率 = 50
                        PH減酸Flag = True
                    Case Is > 10
                        減酸比率 = 25
                        PH減酸Flag = True
                    Case Else
                        PH減酸Flag = False

                End Select



            End If
            '====================================================PH都不降,重新計算
            If .IO.PhValue > ExpectPh Then
                If 減酸比率 = 50 Then
                    補酸次數 = 0
                    If (.IO.PhValue - ExpectPh) > 80 Then
                        PH減酸Flag = False
                        PH減酸Flag2 = False
                        未達到演算值1 = 0
                        減酸比率 = 0
                    End If

                    If (.IO.PhValue - ExpectPh) > 20 Then '$$$$$$$$$$$$$$$$$$$$$起 加酸加不進去 ,降 減酸比率
                        微調比率減少 = 微調比率減少 + 1
                        If 微調比率減少 > 3 Then
                            減酸比率 = 25
                            微調比率減少 = 0
                            微調次數 = 0
                            PH減酸Flag = True
                            PH減酸Flag2 = False
                        End If
                    End If '$$$$$$$$$$$$$$$$$$$$$底

                ElseIf 減酸比率 = 25 Then
                    補酸次數 = 0
                    If (.IO.PhValue - ExpectPh) > 50 Then
                        PH減酸Flag = False
                        PH減酸Flag2 = False
                        未達到演算值1 = 0
                        減酸比率 = 0
                    End If

                    If (.IO.PhValue - ExpectPh) > 20 Then '$$$$$$$$$$$$$$$$$$$$$起 加酸加不進去 ,降 減酸比率
                        微調比率減少 = 微調比率減少 + 1
                        If 微調比率減少 > 3 Then
                            減酸比率 = 0
                            微調次數 = 0
                            微調比率減少 = 0
                            PH減酸Flag = True
                            PH減酸Flag2 = False
                        End If
                    End If '$$$$$$$$$$$$$$$$$$$$$底


                ElseIf 減酸比率 = 0 Then '(((((((((((((((((((((((((((((((((((((起    補酸量計算

                    If (.IO.PhValue - ExpectPh) > 15 Then

                        減酸比率 = -50
                        微調次數 = 0
                        微調比率減少 = 0
                        PH減酸Flag = True
                        PH減酸Flag2 = False
                    End If '(((((((((((((((((((((((((((((((((((((底

                ElseIf 減酸比率 = -50 Then
                    If (.IO.PhValue - ExpectPh) > 10 Then
                        微調次數 = 0
                        微調比率減少 = 0
                        PH減酸Flag = True
                        PH減酸Flag2 = False

                    End If

                End If
            Else
                If 減酸比率 = -50 Then
                    減酸比率 = 0
                End If
            End If
            '=============================================================================================
            If PH減酸Flag = True And PH減酸Flag2 = False Then



                Select Case 減酸比率
                    Case 50 '---------------------------------------------------------------------------
                        Dim 總量 As Integer
                        總量 = 0

                        If RegisterI(1) + 4 <= X Then

                            總量 = (FreeTime(RegisterI(1) + 1)) + (PerPhValue(RegisterI(1) + 1))
                            FreeTime(RegisterI(1) + 1) = 總量
                            PerPhValue(RegisterI(1) + 1) = 0

                            總量 = (FreeTime(RegisterI(1) + 3)) + (PerPhValue(RegisterI(1) + 3))
                            FreeTime(RegisterI(1) + 3) = 總量
                            PerPhValue(RegisterI(1) + 3) = 0

                        End If

                    Case 25 '---------------------------------------------------------------------------
                        Dim 總量 As Integer
                        總量 = 0

                        If RegisterI(1) + 4 <= X Then

                            總量 = (FreeTime(RegisterI(1) + 3)) + (PerPhValue(RegisterI(1) + 3))
                            FreeTime(RegisterI(1) + 3) = 總量
                            PerPhValue(RegisterI(1) + 3) = 0

                        End If

                    Case -50 '---------------------------------------------------------------------------
                        '   If (RegisterI(1)) Mod 2 = 1 Then '-------------------------------------

                        Dim 總量, 加酸量, 計算總量, 單次加酸量, 單次總量 As Integer
                        總量 = FreeTime(RegisterI(1)) + PerPhValue(RegisterI(1))
                        加酸量 = PerPhValue(RegisterI(1))
                        If 總量 = 加酸量 Then
                        ElseIf 總量 > 加酸量 Then
                            If 總量 = 1 Then
                                PerPhValue(RegisterI(1)) = 1
                                FreeTime(RegisterI(1)) = 0

                            ElseIf 總量 = 2 Then
                                PerPhValue(RegisterI(1)) = 2
                                FreeTime(RegisterI(1)) = 0

                            ElseIf 總量 = 3 Then
                                PerPhValue(RegisterI(1)) = 3
                                FreeTime(RegisterI(1)) = 0

                            ElseIf 總量 > 3 Then

                                計算總量 = 加酸量 * 2
                                If 計算總量 > 總量 Then
                                    計算總量 = 總量
                                End If
                                PerPhValue(RegisterI(1)) = 計算總量
                                FreeTime(RegisterI(1)) = 總量 - 計算總量
                                If FreeTime(RegisterI(1)) < 0 Then
                                    FreeTime(RegisterI(1)) = 0
                                End If
                                單次加酸量 = PerPhValue(RegisterI(1))
                                單次總量 = FreeTime(RegisterI(1))
                                單次總量 = 單次總量
                            End If
                            'End If


                        End If '---------------------------------------


                End Select
                PH減酸Flag2 = True
            End If


            If PH減酸Flag = True And PH減酸Flag2 = True Then  '========================================================
                If 減酸比率 = -50 Then '&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
                    PH減酸Flag2 = True
                    微調次數 = 0
                Else

                    微調次數 = 微調次數 + 1

                    '$$$$$$$$$$$$$$$$$$$$$起
                    If (.IO.PhValue - ExpectPh) <= 20 Then
                        微調比率減少 = 0
                    End If
                    '$$$$$$$$$$$$$$$$$$$$$底

                    If 微調次數 >= 4 Then
                        PH減酸Flag2 = False
                        微調次數 = 0
                        If .IO.PhValue < ExpectPh Then

                            Select Case ExpectPh - .IO.PhValue  '+++++++++++++++++++++++++++++++++++++
                                Case Is > 50
                                    減酸比率 = 50
                                Case Is > 20                        'ph 5.0  < 設定值 5.22
                                    If 減酸比率 = 50 Then
                                        減酸比率 = 50
                                    ElseIf 減酸比率 = 25 Then
                                        減酸比率 = 50
                                    End If
                                Case Is > 10                       'ph 5.0  < 設定值 5.12
                                    If 減酸比率 = 50 Then
                                        減酸比率 = 50
                                    ElseIf 減酸比率 = 25 Then
                                        減酸比率 = 25
                                    End If
                                Case Else                       'ph 5.0  < 設定值 5.6
                                    If 減酸比率 = 50 Then
                                        減酸比率 = 25
                                    ElseIf 減酸比率 = 25 Then
                                        減酸比率 = 25

                                    End If

                            End Select '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        End If

                    End If
                End If '&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            End If '====================================================================================================

        End With
       
    End Sub
    Public Sub Cancel()
        State = PhControl.Off

    End Sub
#Region "Standard Definitions"

    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    Friend ReadOnly Property IsOn() As Boolean
        Get
            Return State <> PhControl.Off
        End Get
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As PhControl
    Public Property State() As PhControl
        Get
            Return state_
        End Get
        Private Set(ByVal value As PhControl)
            state_ = value
        End Set
    End Property

    Public ReadOnly Property PhInToMachine() As Boolean     'PH入染機閥
        Get
            Return (State = PhControl.AllAddHac) Or ((State = PhControl.MathAddHac3 Or State = PhControl.MathAddHac4) And OpenHacValve = True)
        End Get
    End Property

    Public ReadOnly Property PhAddHacOut() As Boolean       '加酸閥
        Get
            Return (State = PhControl.AllAddHac) Or ((State = PhControl.MathAddHac3 Or State = PhControl.MathAddHac4) And OpenHacValve = True)
        End Get
    End Property

    Public ReadOnly Property PhAddPump() As Boolean     '加酸定量馬達
        Get
            Return (State = PhControl.AllAddHac) Or ((State = PhControl.MathAddHac3 Or State = PhControl.MathAddHac4) And OpenHacValve = True)
        End Get
    End Property
 
#End Region
End Class

Partial Class ControlCode
    Public ReadOnly PhControl As New PhControl_(Me)
End Class
