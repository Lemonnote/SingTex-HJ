Imports System.IO

Public Class Mimic
  Public ControlCode As ControlCode
  Public MainPumpSpeedTextBoxEditing As Boolean
  Public Reel1SpeedTextBoxEditing As Boolean
  Public Reel2SpeedTextBoxEditing As Boolean

  Private Sub Mimic_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    With ControlCode
    End With
  End Sub

  Private Sub Timer1_Tick_1(sender As Object, e As EventArgs) Handles Timer1.Tick
    With ControlCode
      If Not MainPumpSpeedTextBoxEditing Then
        TextBox_MainPumpSpeed.Text = .PumpControl.PumpControl_MainPumpSpeed.ToString
      End If
      If Not Reel1SpeedTextBoxEditing Then
        TextBox_Reel1Speed.Text = .PumpControl.PumpControl_Reel1Speed.ToString
      End If
      If Not Reel2SpeedTextBoxEditing Then
        TextBox_Reel2Speed.Text = .PumpControl.PumpControl_Reel2Speed.ToString
      End If

      If TextBox_MainPumpSpeed.Focused Or TextBox_Reel1Speed.Focused Or TextBox_Reel2Speed.Focused Then
        GroupBox_KeyPad.Visible = True
      End If
      If TextBox_MainPumpSpeed.Focused And Not MainPumpSpeedTextBoxEditing Then
        TextBox_MainPumpSpeed.Text = ""
        MainPumpSpeedTextBoxEditing = True
      End If
      If TextBox_Reel1Speed.Focused And Not Reel1SpeedTextBoxEditing Then
        TextBox_Reel1Speed.Text = ""
        Reel1SpeedTextBoxEditing = True
      End If
      If TextBox_Reel2Speed.Focused And Not Reel2SpeedTextBoxEditing Then
        TextBox_Reel2Speed.Text = ""
        Reel2SpeedTextBoxEditing = True
      End If


      ' If IO_Drain.Value = True Then
      ' DrainPipe1.Visible = True
      ' Else
      ' DrainPipe1.Visible = False
      ' End If

      '   If IO_Heat.Value = True Then
      '   HeatPic1.Visible = True
      '   HeatPic2.Visible = True
      '   Else
      '   HeatPic1.Visible = False
      '   HeatPic2.Visible = False
      '   End If

      '  If IO_Cool.Value = True Then
      '  CoolingPic1.Visible = True
      '  CoolingPic2.Visible = True
      '  Else
      '  CoolingPic1.Visible = False
      '  CoolingPic2.Visible = False
      '  End If

      '  If IO_CTankFillCold.Value = True Or IO_CTankFillCirc.Value = True Then
      '  CTankFillPic1.Visible = True
      '  Else
      '  CTankFillPic1.Visible = False
      '  End If

      ' If IO_CTankDrain.Value = True Then
      ' CTankDrainPic1.Visible = True
      ' CTankDrainPipe.Visible = True
      ' Else
      ' CTankDrainPic1.Visible = False
      ' CTankDrainPipe.Visible = False
      ' End If

      ' If IO_BTankFillCirc.Value = True Or IO_BTankFillCold.Value = True Then
      ' BTankFillPic1.Visible = True
      ' Else
      ' BTankFillPic1.Visible = False
      ' End If

      '  If IO_BTankDrain.Value = True Then
      '  BTankDrainPic1.Visible = True
      '  BTankDrainPipe.Visible = True
      '  Else
      '  BTankDrainPic1.Visible = False
      '  BTankDrainPipe.Visible = False
      '  End If

      '   If IO_MainPumpFB.Value = True Then
      '   MainPumpPipe1.Visible = True
      '   MainPumpPipe2.Visible = True
      '   MainPumpPipe3.Visible = True
      '   MainPumpPipe4.Visible = True
      '   MainPumpPipe5.Visible = True
      '   MainPumpPipe6.Visible = True
      '   MainPumpPipe7.Visible = True
      '   Else
      '   MainPumpPipe1.Visible = False
      '   MainPumpPipe2.Visible = False
      '   MainPumpPipe3.Visible = False
      '   MainPumpPipe4.Visible = False
      '   MainPumpPipe5.Visible = False
      '   MainPumpPipe6.Visible = False
      '   MainPumpPipe7.Visible = False
      '   End If

      ' If IO_MainPumpFB.Value = True Or IO_HotDrain.Value = True Then
      ' MainPumpPipe8.Visible = True
      ' Else
      ' MainPumpPipe8.Visible = False
      ' End If

      ' If IO_CoolDrain.Value = True Then
      ' CoolingWaterOutPipe1.Visible = True
      ' CoolingWaterOutPipe2.Visible = True
      ' Else
      ' CoolingWaterOutPipe1.Visible = False
      ' CoolingWaterOutPipe2.Visible = False
      ' End If

      ' If IO_HxDrain.Value = True Then
      ' HXDrainPipe1.Visible = True
      ' HXDrainPipe2.Visible = True
      ' Else
      ' HXDrainPipe1.Visible = False
      ' HXDrainPipe2.Visible = False
      ' End If

      '  If IO_CondenserDrain.Value = True Then
      '  CondenserDrainPipe1.Visible = True
      '  CondenserDrainPipe2.Visible = True
      '  Else
      '  CondenserDrainPipe1.Visible = False
      '  CondenserDrainPipe2.Visible = False
      '  End If


      ' If IO_HotDrain.Value = True Then
      ' DrainHotPipe2.Visible = True
      ' DrainHotPipe3.Visible = True
      ' Else
      ' DrainHotPipe2.Visible = False
      ' DrainHotPipe3.Visible = False
      ' End If

      '  If IO_Overflow.Value = True Then
      '  OverFlowPipe1.Visible = True
      '  OverFlowPipe2.Visible = True
      '  Else
      '  OverFlowPipe1.Visible = False
      '  OverFlowPipe2.Visible = False
      '  End If
    End With
  End Sub

  Private Sub Button_Save_Click(sender As Object, e As EventArgs) Handles Button_Save.Click
    With ControlCode
      GroupBox_KeyPad.Visible = False
      .PumpControl.PumpControl_MainPumpSpeed = CInt(TextBox_MainPumpSpeed.Text)
      .PumpControl.PumpControl_Reel1Speed = CInt(TextBox_Reel1Speed.Text)
      .PumpControl.PumpControl_Reel2Speed = CInt(TextBox_Reel2Speed.Text)
      MainPumpSpeedTextBoxEditing = False
      Reel1SpeedTextBoxEditing = False
      Reel2SpeedTextBoxEditing = False
    End With
  End Sub

  Private Sub Button_Quit_Click(sender As Object, e As EventArgs) Handles Button_Quit.Click
    With ControlCode
      GroupBox_KeyPad.Visible = False
      TextBox_MainPumpSpeed.Text = .PumpControl.PumpControl_MainPumpSpeed.ToString
      TextBox_Reel1Speed.Text = .PumpControl.PumpControl_Reel1Speed.ToString
      TextBox_Reel2Speed.Text = .PumpControl.PumpControl_Reel2Speed.ToString
      MainPumpSpeedTextBoxEditing = False
      Reel1SpeedTextBoxEditing = False
      Reel2SpeedTextBoxEditing = False
    End With
  End Sub

  Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
    With ControlCode
      If .StartUserLogin And Not .ShowCallMessageWas Then
        .ShowCallMessageWas = True
        Dim alertForm = New UserLogin
        alertForm.ShowDialog()   ' showdialog makes you wait until press OK
        .UserLoginOK = True
        .StartUserLogin = False
        .ShowCallMessageWas = False
        Dim readText As String = My.Computer.FileSystem.ReadAllText("LoginUser.txt")
        Dim objStreamReader As StreamReader
        Dim strLine As String
        Dim i As Integer = 0
        'Pass the file path and the file name to the StreamReader constructor.
        objStreamReader = New StreamReader("LoginUser.txt")
        'Read the first line of text.
        strLine = objStreamReader.ReadLine
        .LoadOperator = strLine
      End If
    End With
  End Sub
End Class
