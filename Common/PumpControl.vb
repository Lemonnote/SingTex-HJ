
Public Class PumpControl_
  Public Enum PumpControl
    off
    Start
    Wait1
    CheckCycleTime1
    Wait2
    CheckCycleTime2
    GetSpeed
    PumpRun
    CheckCycleTime3
    PumpRunSlow
  End Enum
  Public StateString As String

  Public WaitTimer As New Timer
  Public PumpControl_MainPumpSpeedWas As Integer
  Public PumpControl_Reel1SpeedWas As Integer
  Public PumpControl_Reel2SpeedWas As Integer
  Public PumpControl_MainPumpSpeed As Integer
  Public PumpControl_Reel1Speed As Integer
  Public PumpControl_Reel2Speed As Integer
  Public PumpControl_MainPumpSpeedManual As Integer
  Public PumpControl_Reel1SpeedManual As Integer
  Public PumpControl_Reel2SpeedManual As Integer
  Public PumpControl_TargetCycleTime As Integer
  Public PumpControl_PumpSpeedCheckCycleTimeTimer As New Timer
  Public PumpControl_PumpSpeedCheck As Boolean
  Public PumpControl_PumpSpeedAdjustTimer As New Timer
  Public PumpControl_PumpSpeedAdjustWas As Boolean
  Public PumpControl_PumpSpeedAdjustFinish As Boolean
  Public PumpControl_PumpSpeedAdjustLower As Boolean
  Public PumpControl_Reel1SpeedAdjust As Boolean
  Public PumpControl_Reel1SpeedAdjustWas As Boolean
  Public PumpControl_Reel1SpeedAdjustTimer As New Timer
  Public PumpControl_Reel2SpeedAdjust As Boolean
  Public PumpControl_Reel2SpeedAdjustWas As Boolean
  Public PumpControl_Reel2SpeedAdjustTimer As New Timer
  Public PumpControl_StartAdjustCycleTime As Boolean
  Public PumpControl_CheckCycleTime As Integer
  Public PumpControl_PumpStop As Boolean
  Public PumpControl_Reel1Stop As Boolean
  Public PumpControl_Reel2Stop As Boolean
  Public FirstPumpStart As Boolean
  Public CycleTimeOK_Times As Integer
  Public SeamCheckWas As Boolean
  Public SeamCheckDelay As New DelayTimer
  Public ReelReStartDelay As New DelayTimer
  Public PumpRunReelReStartDelay As New DelayTimer
  Public PumpRunReelAutoStart As Boolean
  Public PumpRunSlowReelReStartDelay As New DelayTimer
  Public PumpRunSlowReelAutoStart As Boolean
  Public HeatForDyeingWas As Boolean

  Public Sub Run()
    With ControlCode
      Select Case State
        Case PumpControl.off
          StateString = ""
          '手動時紀錄手動主泵速度與布輪速度，切換到自動且染程沒有設定速度時以手動速度為準
          If (Not .IO.SystemAuto Or Not .IO.AutoPumpControlSW Or PumpControl_MainPumpSpeed = 0 Or PumpControl_Reel1Speed = 0) And .IO.MainPumpFB And .IO.MainPumpSpeed > 0 And .IO.Reel1Speed > 0 Then
            PumpControl_MainPumpSpeedManual = .IO.MainPumpSpeed
            PumpControl_Reel1SpeedManual = .IO.Reel1Speed
            PumpControl_Reel2SpeedManual = .IO.Reel2Speed
            .IO.MainPumpSpeedControl = MinMax(PumpControl_MainPumpSpeedManual * 100 \ .Parameters.AnalogOutput100ForMainPumpRPM, 0, 255)
            .IO.Reel1SpeedControl = MinMax(PumpControl_Reel1SpeedManual * 100 \ .Parameters.AnalogOutput100ForReel1PumpRPM, 0, 255)
            .IO.Reel2SpeedControl = MinMax(PumpControl_Reel2SpeedManual * 100 \ .Parameters.AnalogOutput100ForReel2PumpRPM, 0, 255)
          End If
          If .IO.PumpControlOn Then
            State = PumpControl.Start
            PumpControl_MainPumpSpeed = 900
            PumpControl_Reel1Speed = 0
            PumpControl_Reel2Speed = 0
            .IO.Reel1SpeedControl = 0
            .IO.Reel2SpeedControl = 0
            PumpControl_Reel1Stop = True
            PumpControl_Reel2Stop = True
            FirstPumpStart = False
          End If

        Case PumpControl.Start
          '馬達速度設定在900，偵測到兩次布頭訊號算是完成CycleTime檢查
          StateString = "Start"
          If ReelReStartDelay.Run(.IO.MainPumpFB, .Parameters.ReelStartDelayTime) And Not FirstPumpStart Then
            PumpControl_Reel1Speed = 600
            PumpControl_Reel2Speed = 600
            PumpControl_Reel1Stop = False
            PumpControl_Reel2Stop = False
            FirstPumpStart = True
          End If
          If .IO.MainPumpFB Then
            PumpControl_MainPumpSpeed = 900
          Else
            PumpControl_MainPumpSpeed = 0
            PumpControl_Reel1Speed = 0
            PumpControl_Reel2Speed = 0
            PumpControl_Reel1Stop = True
            PumpControl_Reel2Stop = True
          End If
          If .IO.Reel1StopPB Or .IO.Entanglement1 Then
            PumpControl_Reel1Speed = 0
            .IO.Reel1SpeedControl = 0
            PumpControl_Reel1Stop = True
          End If
          If .IO.Reel2StopPB Or .IO.Entanglement2 Then
            PumpControl_Reel2Speed = 0
            .IO.Reel2SpeedControl = 0
            PumpControl_Reel2Stop = True
          End If
          If .IO.Reel1ForwardPB Then
            PumpControl_Reel1Speed = 600
            PumpControl_Reel1Stop = False
          End If
          If .IO.Reel2ForwardPB Then
            PumpControl_Reel2Speed = 600
            PumpControl_Reel2Stop = False
          End If
          AdjustSpeed()
          If .IO.Reel1Speed < 500 Then Exit Select
          If Not .IO.FabricCycleInput1 Then Exit Select
          WaitTimer.TimeRemaining = 10
          State = PumpControl.Wait1

        Case PumpControl.Wait1
          StateString = "Wait 1"
          If .IO.MainPumpFB Then
            PumpControl_MainPumpSpeed = 900
          Else
            PumpControl_MainPumpSpeed = 0
            PumpControl_Reel1Speed = 0
            PumpControl_Reel2Speed = 0
          End If
          If .IO.Reel1StopPB Or .IO.Entanglement1 Then
            PumpControl_Reel1Speed = 0
            .IO.Reel1SpeedControl = 0
            PumpControl_Reel1Stop = True
          End If
          If .IO.Reel2StopPB Or .IO.Entanglement2 Then
            PumpControl_Reel2Speed = 0
            .IO.Reel2SpeedControl = 0
            PumpControl_Reel2Stop = True
          End If
          If .IO.Reel1ForwardPB Then
            PumpControl_Reel1Speed = 600
            PumpControl_Reel1Stop = False
          End If
          If .IO.Reel2ForwardPB Then
            PumpControl_Reel2Speed = 600
            PumpControl_Reel2Stop = False
          End If
          AdjustSpeed()
          If .IO.Reel1Speed < 500 Then Exit Select
          If Not WaitTimer.Finished Then Exit Select
          State = PumpControl.CheckCycleTime1

        Case PumpControl.CheckCycleTime1
          StateString = "CheckCycleTime1"
          If .IO.MainPumpFB Then
            PumpControl_MainPumpSpeed = 900
          Else
            PumpControl_MainPumpSpeed = 0
            PumpControl_Reel1Speed = 0
            PumpControl_Reel2Speed = 0
          End If
          If .IO.Reel1StopPB Or .IO.Entanglement1 Then
            PumpControl_Reel1Speed = 0
            .IO.Reel1SpeedControl = 0
            PumpControl_Reel1Stop = True
          End If
          If .IO.Reel2StopPB Or .IO.Entanglement2 Then
            PumpControl_Reel2Speed = 0
            .IO.Reel2SpeedControl = 0
            PumpControl_Reel2Stop = True
          End If
          If .IO.Reel1ForwardPB Then
            PumpControl_Reel1Speed = 600
            PumpControl_Reel1Stop = False
          End If
          If .IO.Reel2ForwardPB Then
            PumpControl_Reel2Speed = 600
            PumpControl_Reel2Stop = False
          End If
          AdjustSpeed()
          If .IO.Reel1Speed < 500 Then Exit Select
          If Not .IO.FabricCycleInput1 Then Exit Select
          WaitTimer.TimeRemaining = 10
          State = PumpControl.Wait2

        Case PumpControl.Wait2
          StateString = "Wait 2"
          If .IO.MainPumpFB Then
            PumpControl_MainPumpSpeed = 900
          Else
            PumpControl_MainPumpSpeed = 0
            PumpControl_Reel1Speed = 0
            PumpControl_Reel2Speed = 0
          End If
          If .IO.Reel1StopPB Or .IO.Entanglement1 Then
            PumpControl_Reel1Speed = 0
            .IO.Reel1SpeedControl = 0
            PumpControl_Reel1Stop = True
          End If
          If .IO.Reel2StopPB Or .IO.Entanglement2 Then
            PumpControl_Reel2Speed = 0
            .IO.Reel2SpeedControl = 0
            PumpControl_Reel2Stop = True
          End If
          If .IO.Reel1ForwardPB Then
            PumpControl_Reel1Speed = 600
            PumpControl_Reel1Stop = False
          End If
          If .IO.Reel2ForwardPB Then
            PumpControl_Reel2Speed = 600
            PumpControl_Reel2Stop = False
          End If
          AdjustSpeed()
          If Not WaitTimer.Finished Then Exit Select
          WaitTimer.TimeRemaining = 60
          State = PumpControl.CheckCycleTime2

        Case PumpControl.CheckCycleTime2
          StateString = "CheckCycleTime2"
          If WaitTimer.Finished Then
            '馬達速度自動控制時每1分鐘檢查一次CycleTime是否正常，平均CycleTime與設定值相同後即停止調整
            If .Parameters.PumpSpeedAdjustCheckCycleNo = 1 Then
              PumpControl_CheckCycleTime = .FabricCycleTime1
            ElseIf .Parameters.PumpSpeedAdjustCheckCycleNo = 2 Then
              PumpControl_CheckCycleTime = .FabricCycleTime2
            ElseIf .Parameters.PumpSpeedAdjustCheckCycleNo = 3 Then
              PumpControl_CheckCycleTime = .FabricCycleTime3
            ElseIf .Parameters.PumpSpeedAdjustCheckCycleNo = 4 Then
              PumpControl_CheckCycleTime = .FabricCycleTime4
            Else
              PumpControl_CheckCycleTime = .AverageCycleTime
            End If
            If PumpControl_TargetCycleTime > 0 And PumpControl_CheckCycleTime < PumpControl_TargetCycleTime * 1.6 And PumpControl_CheckCycleTime > PumpControl_TargetCycleTime * 0.6 Then
              State = PumpControl.GetSpeed
            End If
            WaitTimer.TimeRemaining = 60
          End If

        Case PumpControl.GetSpeed
          StateString = "Get Speed"
          PumpControl_MainPumpSpeed = MinMax(((PumpControl_MainPumpSpeed * PumpControl_CheckCycleTime) \ PumpControl_TargetCycleTime), CInt(PumpControl_MainPumpSpeed * 0.8), CInt(PumpControl_MainPumpSpeed * 1.2))
          If .Command34.速度差 > 50 Then
            PumpControl_Reel1Speed = PumpControl_MainPumpSpeed - .Command34.速度差
            PumpControl_Reel2Speed = PumpControl_MainPumpSpeed - .Command34.速度差
          Else
            PumpControl_Reel1Speed = MinMax(PumpControl_Reel1Speed - 400, 300, 1000)
            PumpControl_Reel2Speed = MinMax(PumpControl_Reel2Speed - 400, 300, 1000)
          End If
          PumpControl_MainPumpSpeedWas = PumpControl_MainPumpSpeed
          PumpControl_Reel1SpeedWas = PumpControl_Reel1Speed
          PumpControl_Reel2SpeedWas = PumpControl_Reel2Speed
          WaitTimer.TimeRemaining = 60
          CycleTimeOK_Times = 0
          HeatForDyeingWas = False
          State = PumpControl.PumpRun

        Case PumpControl.PumpRun
          StateString = "Pump Run"
          AdjustSpeed()
          If .IO.MainPumpFB Then
            PumpControl_MainPumpSpeed = PumpControl_MainPumpSpeedWas
          Else
            PumpControl_MainPumpSpeed = 0
            PumpControl_Reel1Speed = 0
            PumpControl_Reel2Speed = 0
            PumpControl_Reel1Stop = True
            PumpControl_Reel2Stop = True
            PumpRunReelAutoStart = True
          End If
          If PumpRunReelReStartDelay.Run(.IO.MainPumpFB And PumpRunReelAutoStart, .Parameters.ReelStartDelayTime) Then
            PumpRunReelAutoStart = False
            PumpControl_Reel1Stop = False
            PumpControl_Reel2Stop = False
            PumpControl_Reel1Speed = PumpControl_Reel1SpeedWas
            PumpControl_Reel2Speed = PumpControl_Reel2SpeedWas
          End If
          If .IO.Reel1ForwardPB Then
            PumpControl_Reel1Speed = PumpControl_Reel1SpeedWas
            PumpControl_Reel1Stop = False
          End If
          If .IO.Reel2ForwardPB Then
            PumpControl_Reel2Speed = PumpControl_Reel2SpeedWas
            PumpControl_Reel2Stop = False
          End If

          '布速1超過設定值10%，溫度在85度以下，則重新運算一次速度
          If .IO.FabricCycleInput1 Then
            SeamCheckWas = True
          End If

          If SeamCheckDelay.Run(SeamCheckWas, 5) And .IO.MainTemperature <= 850 And CycleTimeOK_Times < 5 Then
            SeamCheckWas = False
            If .Parameters.PumpSpeedAdjustCheckCycleNo = 1 Then
              PumpControl_CheckCycleTime = .FabricCycleTime1
            ElseIf .Parameters.PumpSpeedAdjustCheckCycleNo = 2 Then
              PumpControl_CheckCycleTime = .FabricCycleTime2
            ElseIf .Parameters.PumpSpeedAdjustCheckCycleNo = 3 Then
              PumpControl_CheckCycleTime = .FabricCycleTime3
            ElseIf .Parameters.PumpSpeedAdjustCheckCycleNo = 4 Then
              PumpControl_CheckCycleTime = .FabricCycleTime4
            Else
              PumpControl_CheckCycleTime = .AverageCycleTime
            End If
            If PumpControl_TargetCycleTime > 0 And PumpControl_CheckCycleTime < PumpControl_TargetCycleTime * 1.6 And (PumpControl_CheckCycleTime > PumpControl_TargetCycleTime * 1.1 Or PumpControl_CheckCycleTime < PumpControl_TargetCycleTime * 0.9) Then
              State = PumpControl.GetSpeed
              CycleTimeOK_Times = 0
            End If
            CycleTimeOK_Times = CycleTimeOK_Times + 1
            WaitTimer.TimeRemaining = PumpControl_CheckCycleTime
          End If
          If .HeatNow And .IO.MainTemperature > 950 Then
            HeatForDyeingWas = True
          End If

          '降溫到120度以下時就用慢速來運行
          If .CoolNow And .IO.MainTemperature < 1200 And HeatForDyeingWas Then
            PumpControl_MainPumpSpeed = CInt(PumpControl_MainPumpSpeed * 0.9)
            PumpControl_Reel1Speed = CInt(PumpControl_Reel1Speed * 0.9)
            PumpControl_Reel2Speed = CInt(PumpControl_Reel1Speed * 0.9)
            PumpControl_MainPumpSpeedWas = PumpControl_MainPumpSpeed
            PumpControl_Reel1SpeedWas = PumpControl_Reel1Speed
            PumpControl_Reel2SpeedWas = PumpControl_Reel1Speed
            State = PumpControl.PumpRunSlow
          End If


        Case PumpControl.PumpRunSlow
          StateString = "Pump Run Slow"
          AdjustSpeed()
          If .IO.MainPumpFB Then
            PumpControl_MainPumpSpeed = PumpControl_MainPumpSpeedWas
          Else
            PumpControl_MainPumpSpeed = 0
            PumpControl_Reel1Speed = 0
            PumpControl_Reel2Speed = 0
            PumpControl_Reel1Stop = True
            PumpControl_Reel2Stop = True
            PumpRunSlowReelAutoStart = True
          End If
          If PumpRunSlowReelReStartDelay.Run(.IO.MainPumpFB And PumpRunSlowReelAutoStart, .Parameters.ReelStartDelayTime) Then
            PumpRunSlowReelAutoStart = False
            PumpControl_Reel1Stop = False
            PumpControl_Reel2Stop = False
            PumpControl_Reel1Speed = PumpControl_Reel1SpeedWas
            PumpControl_Reel2Speed = PumpControl_Reel1SpeedWas
          End If
          If .IO.Reel1ForwardPB Then
            PumpControl_Reel1Speed = PumpControl_Reel1SpeedWas
            PumpControl_Reel1Stop = False
          End If
          If .IO.Reel2ForwardPB Then
            PumpControl_Reel2Speed = PumpControl_Reel2SpeedWas
            PumpControl_Reel2Stop = False
          End If
          State = PumpControl.PumpRunSlow

      End Select

    End With
  End Sub

  Public Sub AdjustSpeed()
    With ControlCode
      '自動時自動調整主泵速度與帶布輪速度
      If PumpControl_PumpSpeedAdjustTimer.Finished Then
        '      PumpSpeedAdjustWas = False
        PumpControl_PumpSpeedAdjustWas = False
      End If
      If Not .IO.MainPumpFB Then
        .IO.MainPumpSpeedControl = 0
        .IO.Reel1SpeedControl = 0
        .IO.Reel2SpeedControl = 0
      End If
      Static Reel1StartDelay As New DelayTimer
      If Reel1StartDelay.Run(.IO.MainPumpFB, .Parameters.ReelStartDelayTime + 5) And Not PumpControl_Reel1Stop Then
        PumpControl_Reel1SpeedAdjust = True
      Else
        PumpControl_Reel1SpeedAdjust = False
      End If

      Static Reel2StartDelay As New DelayTimer
      If Reel2StartDelay.Run(.IO.MainPumpFB, .Parameters.ReelStartDelayTime + 5) And Not PumpControl_Reel2Stop Then
        PumpControl_Reel2SpeedAdjust = True
      Else
        PumpControl_Reel2SpeedAdjust = False
      End If

      If .IO.SystemAuto And .IO.AutoPumpControlSW And .IO.MainPumpFB And PumpControl_PumpSpeedAdjustTimer.Finished And PumpControl_MainPumpSpeed > 0 And Not PumpControl_PumpSpeedAdjustWas Then
        PumpControl_PumpSpeedAdjustTimer.TimeRemainingMs = 1000
        PumpControl_PumpSpeedAdjustWas = True
        '主泵速度調整
        If .IO.MainPumpSpeed + 20 < PumpControl_MainPumpSpeed Then
          '加速
          If .IO.MainPumpSpeed + 200 < PumpControl_MainPumpSpeed Then
            .IO.MainPumpSpeedControl = CShort(MinMax(.IO.MainPumpSpeedControl + 5, 0, 255))
          ElseIf .IO.MainPumpSpeed + 50 < PumpControl_MainPumpSpeed Then
            .IO.MainPumpSpeedControl = CShort(MinMax(.IO.MainPumpSpeedControl + 2, 0, 255))
          ElseIf .IO.MainPumpSpeed + 20 < PumpControl_MainPumpSpeed Then
            .IO.MainPumpSpeedControl = CShort(MinMax(.IO.MainPumpSpeedControl + 1, 0, 255))
          End If
        Else
          '減速
          If .IO.MainPumpSpeed - 20 > PumpControl_MainPumpSpeed Then
            .IO.MainPumpSpeedControl = MinMax(.IO.MainPumpSpeedControl - 1, 0, 255)
          ElseIf .IO.MainPumpSpeed - 50 > PumpControl_MainPumpSpeed Then
            .IO.MainPumpSpeedControl = MinMax(.IO.MainPumpSpeedControl - 2, 0, 255)
          ElseIf .IO.MainPumpSpeed - 200 > PumpControl_MainPumpSpeed Then
            .IO.MainPumpSpeedControl = MinMax(.IO.MainPumpSpeedControl - 5, 0, 255)
          End If
        End If

        '布輪1速度調整
        If PumpControl_Reel1Stop And .IO.Reel1ForwardPB Then
          PumpControl_Reel1Stop = False
        End If
        If .IO.Reel1StopPB Or .IO.Entanglement1 Or PumpControl_Reel1Stop Then
          .IO.Reel1SpeedControl = 0
          PumpControl_Reel1Speed = 0
          PumpControl_Reel1Stop = True
        End If
        If PumpControl_Reel1SpeedAdjust Then
          '加速
          If .IO.Reel1Speed + 20 < PumpControl_Reel1Speed Then
            If .IO.Reel1Speed + 200 < PumpControl_Reel1Speed Then
              .IO.Reel1SpeedControl = CShort(MinMax(.IO.Reel1SpeedControl + 5, 0, 255))
            ElseIf .IO.Reel1Speed + 50 < PumpControl_Reel1Speed Then
              .IO.Reel1SpeedControl = CShort(MinMax(.IO.Reel1SpeedControl + 2, 0, 255))
            ElseIf .IO.Reel1Speed + 20 < PumpControl_Reel1Speed Then
              .IO.Reel1SpeedControl = CShort(MinMax(.IO.Reel1SpeedControl + 1, 0, 255))
            End If
          Else
            '減速
            If .IO.Reel1Speed - 20 > PumpControl_Reel1Speed Then
              .IO.Reel1SpeedControl = MinMax(.IO.Reel1SpeedControl - 1, 0, 255)
            ElseIf .IO.Reel1Speed - 50 > PumpControl_Reel1Speed Then
              .IO.Reel1SpeedControl = MinMax(.IO.Reel1SpeedControl - 2, 0, 255)
            ElseIf .IO.Reel1Speed - 200 > PumpControl_Reel1Speed Then
              .IO.Reel1SpeedControl = MinMax(.IO.Reel1SpeedControl - 5, 0, 255)
            End If
          End If
        End If

        '布輪2速度調整
        If PumpControl_Reel2Stop And .IO.Reel2ForwardPB Then
          PumpControl_Reel2Stop = False
        End If
        If .IO.Reel2StopPB Or .IO.Entanglement2 Or PumpControl_Reel2Stop Then
          .IO.Reel2SpeedControl = 0
          PumpControl_Reel2Speed = 0
          PumpControl_Reel2Stop = True
        End If
        If PumpControl_Reel2SpeedAdjust Then
          If .IO.Reel2Speed + 20 < PumpControl_Reel2Speed Then
            '加速
            If .IO.Reel2Speed + 200 < PumpControl_Reel2Speed Then
              .IO.Reel2SpeedControl = CShort(MinMax(.IO.Reel2SpeedControl + 5, 0, 255))
            ElseIf .IO.Reel2Speed + 50 < PumpControl_Reel2Speed Then
              .IO.Reel2SpeedControl = CShort(MinMax(.IO.Reel2SpeedControl + 2, 0, 255))
            ElseIf .IO.Reel2Speed + 20 < PumpControl_Reel2Speed Then
              .IO.Reel2SpeedControl = CShort(MinMax(.IO.Reel2SpeedControl + 1, 0, 255))
            End If
          Else
            '減速
            If .IO.Reel2Speed - 20 > PumpControl_Reel2Speed Then
              .IO.Reel2SpeedControl = MinMax(.IO.Reel2SpeedControl - 1, 0, 255)
            ElseIf .IO.Reel2Speed - 50 > PumpControl_Reel2Speed Then
              .IO.Reel2SpeedControl = MinMax(.IO.Reel2SpeedControl - 2, 0, 255)
            ElseIf .IO.Reel2Speed - 200 > PumpControl_Reel2Speed Then
              .IO.Reel2SpeedControl = MinMax(.IO.Reel2SpeedControl - 5, 0, 255)
            End If
          End If
        End If
      End If
    End With
  End Sub

#Region " Standard Definitions "
  Public Sub Cancel()
    With ControlCode
      HeatForDyeingWas = False
      State = PumpControl.off
    End With
  End Sub

  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As PumpControl
  Public Property State() As PumpControl
    Get
      Return state_
    End Get
    Private Set(ByVal value As PumpControl)
      state_ = value
    End Set
  End Property
  Public ReadOnly Property IsOn() As Boolean
    Get
      Return (State <> PumpControl.off)
    End Get
  End Property

#End Region

End Class

Partial Class ControlCode
  Public ReadOnly PumpControl As New PumpControl_(Me)
End Class
