
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
  Public PumpControl_ReelSpeedAdjust As Boolean
  Public PumpControl_ReelSpeedAdjustWas As Boolean
  Public PumpControl_ReelSpeedAdjustTimer As New Timer
  Public PumpControl_StartAdjustCycleTime As Boolean
  Public PumpControl_CheckCycleTime As Integer
  Public PumpControl_PumpStop As Boolean
  Public PumpControl_Reel1Stop As Boolean
  Public PumpControl_Reel2Stop As Boolean

  Public Sub Run()
    With ControlCode
      Select Case State
        Case PumpControl.off
          StateString = ""
          '��ʮɬ�����ʥD���t�׻P�����t�סA������۰ʥB�V�{�S���]�w�t�׮ɥH��ʳt�׬���
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
          End If

        Case PumpControl.Start
          '���F�t�׳]�w�b900�A������⦸���Y�T����O����CycleTime�ˬd
          StateString = "Start"
          Static ReelReStartDelay As New DelayTimer
          If ReelReStartDelay.Run(.IO.MainPumpFB, .Parameters.ReelStartDelayTime) Then
            PumpControl_Reel1Speed = 600
            PumpControl_Reel2Speed = 600
          End If
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
          End If
          If .IO.Reel2StopPB Or .IO.Entanglement2 Then
            PumpControl_Reel1Speed = 0
            .IO.Reel2SpeedControl = 0
          End If
          If .IO.Reel1ForwardPB Then
            PumpControl_Reel1Speed = 600
          End If
          If .IO.Reel2ForwardPB Then
            PumpControl_Reel2Speed = 600
          End If
          AdjustSpeed()
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
          End If
          If .IO.Reel2StopPB Or .IO.Entanglement2 Then
            PumpControl_Reel1Speed = 0
            .IO.Reel2SpeedControl = 0
          End If
          If .IO.Reel1ForwardPB Then
            PumpControl_Reel1Speed = 600
          End If
          If .IO.Reel2ForwardPB Then
            PumpControl_Reel2Speed = 600
          End If
          AdjustSpeed()
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
          End If
          If .IO.Reel2StopPB Or .IO.Entanglement2 Then
            PumpControl_Reel1Speed = 0
            .IO.Reel2SpeedControl = 0
          End If
          If .IO.Reel1ForwardPB Then
            PumpControl_Reel1Speed = 600
          End If
          If .IO.Reel2ForwardPB Then
            PumpControl_Reel2Speed = 600
          End If
          AdjustSpeed()
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
          End If
          If .IO.Reel2StopPB Or .IO.Entanglement2 Then
            PumpControl_Reel1Speed = 0
            .IO.Reel2SpeedControl = 0
          End If
          If .IO.Reel1ForwardPB Then
            PumpControl_Reel1Speed = 600
          End If
          If .IO.Reel2ForwardPB Then
            PumpControl_Reel2Speed = 600
          End If
          AdjustSpeed()
          If Not WaitTimer.Finished Then Exit Select
          State = PumpControl.CheckCycleTime2

        Case PumpControl.CheckCycleTime2
          StateString = "CheckCycleTime2"
          '���F�t�צ۰ʱ���ɨC1�����ˬd�@��CycleTime�O�_���`�A����CycleTime�P�]�w�ȬۦP��Y����վ�
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
          Dim CycleTimeMax As Integer
          CycleTimeMax = MinMax(PumpControl_CheckCycleTime + 20, 0, 300)
          If PumpControl_TargetCycleTime > 0 And PumpControl_CheckCycleTime < PumpControl_TargetCycleTime * 1.6 And PumpControl_CheckCycleTime > PumpControl_TargetCycleTime * 0.6 Then
            State = PumpControl.GetSpeed
          Else
            State = PumpControl.Start
          End If

        Case PumpControl.GetSpeed
          StateString = "Get Speed"
          PumpControl_MainPumpSpeed = MinMax(((PumpControl_MainPumpSpeed * PumpControl_CheckCycleTime) \ PumpControl_TargetCycleTime), CInt(PumpControl_MainPumpSpeed * 0.8), CInt(PumpControl_MainPumpSpeed * 1.2))
          If .Command34.�t�׮t > 50 Then
            PumpControl_Reel1Speed = PumpControl_MainPumpSpeed - .Command34.�t�׮t
            PumpControl_Reel2Speed = PumpControl_MainPumpSpeed - .Command34.�t�׮t
          Else
            PumpControl_Reel1Speed = MinMax(PumpControl_Reel1Speed - 400, 300, 1000)
            PumpControl_Reel2Speed = MinMax(PumpControl_Reel2Speed - 400, 300, 1000)
          End If
          PumpControl_MainPumpSpeedWas = PumpControl_MainPumpSpeed
          PumpControl_Reel1SpeedWas = PumpControl_Reel1Speed
          PumpControl_Reel2SpeedWas = PumpControl_Reel2Speed
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
          End If
          If .IO.Reel1StopPB Or .IO.Entanglement1 Then
            PumpControl_Reel1Speed = 0
            .IO.Reel1SpeedControl = 0
          End If
          If .IO.Reel2StopPB Or .IO.Entanglement2 Then
            PumpControl_Reel1Speed = 0
            .IO.Reel2SpeedControl = 0
          End If
          If .IO.Reel1ForwardPB Then
            PumpControl_Reel1Speed = PumpControl_Reel1SpeedWas
          End If
          If .IO.Reel2ForwardPB Then
            PumpControl_Reel2Speed = PumpControl_Reel1SpeedWas
          End If


          If .CoolNow And .IO.MainTemperature < 1200 Then
            PumpControl_MainPumpSpeed = PumpControl_MainPumpSpeed - 200
            PumpControl_Reel1Speed = PumpControl_Reel1Speed - 200
            PumpControl_Reel2Speed = PumpControl_Reel1Speed - 200
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
          End If
          If .IO.Reel1StopPB Or .IO.Entanglement1 Then
            PumpControl_Reel1Speed = 0
            .IO.Reel1SpeedControl = 0
          End If
          If .IO.Reel2StopPB Or .IO.Entanglement2 Then
            PumpControl_Reel1Speed = 0
            .IO.Reel2SpeedControl = 0
          End If
          If .IO.Reel1ForwardPB Then
            PumpControl_Reel1Speed = PumpControl_Reel1SpeedWas
          End If
          If .IO.Reel2ForwardPB Then
            PumpControl_Reel2Speed = PumpControl_Reel1SpeedWas
          End If
          State = PumpControl.PumpRunSlow

          '���t1�b�]�w��+-5�H���Y�������F�t�׽վ�Ϊ̬O�D���ūר�F90��
          'If (PumpControl_CheckCycleTime > PumpControl_TargetCycleTime - 3 And PumpControl_CheckCycleTime < PumpControl_TargetCycleTime + 3 And PumpControl_TargetCycleTime > 30 And PumpControl_CheckCycleTime > 30 And PumpControl_PumpSpeedCheck) Or .IO.MainTemperature > 900 Then
          'PumpControl_PumpSpeedAdjustFinish = True
          'End If

          '���Ũ�120�׭p�N�N�D���P�a�����t��-200




      End Select

    End With
  End Sub

  Public Sub AdjustSpeed()
    With ControlCode
      '�۰ʮɦ۰ʽվ�D���t�׻P�a�����t��
      If PumpControl_PumpSpeedAdjustTimer.Finished Then
        '      PumpSpeedAdjustWas = False
        PumpControl_PumpSpeedAdjustWas = False
      End If
      If Not .IO.MainPumpFB Then
        .IO.MainPumpSpeedControl = 0
        .IO.Reel1SpeedControl = 0
        .IO.Reel2SpeedControl = 0
      End If
      Static ReelStartDelay As New DelayTimer
      If ReelStartDelay.Run(.IO.MainPumpFB, .Parameters.ReelStartDelayTime + 5) Then
        PumpControl_ReelSpeedAdjust = True
      Else
        PumpControl_ReelSpeedAdjust = False
      End If

      If .IO.SystemAuto And .IO.AutoPumpControlSW And .IO.MainPumpFB And PumpControl_PumpSpeedAdjustTimer.Finished And PumpControl_MainPumpSpeed > 0 And Not PumpControl_PumpSpeedAdjustWas Then
        PumpControl_PumpSpeedAdjustTimer.TimeRemainingMs = 1000
        PumpControl_PumpSpeedAdjustWas = True
        '�D���t�׽վ�
        If .IO.MainPumpSpeed + 20 < PumpControl_MainPumpSpeed Then
          '�[�t
          If .IO.MainPumpSpeed + 200 < PumpControl_MainPumpSpeed Then
            .IO.MainPumpSpeedControl = CShort(MinMax(.IO.MainPumpSpeedControl + 5, 0, 255))
          ElseIf .IO.MainPumpSpeed + 50 < PumpControl_MainPumpSpeed Then
            .IO.MainPumpSpeedControl = CShort(MinMax(.IO.MainPumpSpeedControl + 2, 0, 255))
          ElseIf .IO.MainPumpSpeed + 20 < PumpControl_MainPumpSpeed Then
            .IO.MainPumpSpeedControl = CShort(MinMax(.IO.MainPumpSpeedControl + 1, 0, 255))
          End If
        Else
          '��t
          If .IO.MainPumpSpeed - 20 > PumpControl_MainPumpSpeed Then
            .IO.MainPumpSpeedControl = MinMax(.IO.MainPumpSpeedControl - 1, 0, 255)
          ElseIf .IO.MainPumpSpeed - 50 > PumpControl_MainPumpSpeed Then
            .IO.MainPumpSpeedControl = MinMax(.IO.MainPumpSpeedControl - 2, 0, 255)
          ElseIf .IO.MainPumpSpeed - 200 > PumpControl_MainPumpSpeed Then
            .IO.MainPumpSpeedControl = MinMax(.IO.MainPumpSpeedControl - 5, 0, 255)
          End If
        End If

        Static Reel1StopDelay As New DelayTimer
        Dim Reel1Stop As Boolean
        Reel1Stop = Reel1StopDelay.Run(.IO.Reel1Speed < 50, 2)
        '����1�t�׽վ�
        If PumpControl_ReelSpeedAdjust Then
          If .IO.Reel1StopPB Or .IO.Entanglement1 Then
            .IO.Reel1SpeedControl = 0
            PumpControl_Reel1Speed = 0
          Else
            '�[�t
            If .IO.Reel1Speed + 20 < PumpControl_Reel1Speed Then
              If .IO.Reel1Speed + 200 < PumpControl_Reel1Speed Then
                .IO.Reel1SpeedControl = CShort(MinMax(.IO.Reel1SpeedControl + 5, 0, 255))
              ElseIf .IO.Reel1Speed + 50 < PumpControl_Reel1Speed Then
                .IO.Reel1SpeedControl = CShort(MinMax(.IO.Reel1SpeedControl + 2, 0, 255))
              ElseIf .IO.Reel1Speed + 20 < PumpControl_Reel1Speed Then
                .IO.Reel1SpeedControl = CShort(MinMax(.IO.Reel1SpeedControl + 1, 0, 255))
              End If
            Else
              '��t
              If .IO.Reel1Speed - 20 > PumpControl_Reel1Speed Then
                .IO.Reel1SpeedControl = MinMax(.IO.Reel1SpeedControl - 1, 0, 255)
              ElseIf .IO.Reel1Speed - 50 > PumpControl_Reel1Speed Then
                .IO.Reel1SpeedControl = MinMax(.IO.Reel1SpeedControl - 2, 0, 255)
              ElseIf .IO.Reel1Speed - 200 > PumpControl_Reel1Speed Then
                .IO.Reel1SpeedControl = MinMax(.IO.Reel1SpeedControl - 5, 0, 255)
              End If
            End If
          End If

          Static Reel2StopDelay As New DelayTimer
          Dim Reel2Stop As Boolean
          Reel2Stop = Reel1StopDelay.Run(.IO.Reel2Speed < 50, 2)
          '����2�t�׽վ�
          If .IO.Reel2StopPB Or .IO.Entanglement2 Then
            .IO.Reel2SpeedControl = 0
            PumpControl_Reel2Speed = 0
          Else
            If .IO.Reel2Speed + 20 < PumpControl_Reel2Speed Then
              '�[�t
              If .IO.Reel2Speed + 200 < PumpControl_Reel2Speed Then
                .IO.Reel2SpeedControl = CShort(MinMax(.IO.Reel2SpeedControl + 5, 0, 255))
              ElseIf .IO.Reel2Speed + 50 < PumpControl_Reel2Speed Then
                .IO.Reel2SpeedControl = CShort(MinMax(.IO.Reel2SpeedControl + 2, 0, 255))
              ElseIf .IO.Reel2Speed + 20 < PumpControl_Reel2Speed Then
                .IO.Reel2SpeedControl = CShort(MinMax(.IO.Reel2SpeedControl + 1, 0, 255))
              End If
            Else
              '��t
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
      End If
    End With
  End Sub

#Region " Standard Definitions "
  Public Sub Cancel()
    With ControlCode
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
