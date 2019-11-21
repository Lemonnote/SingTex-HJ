<Command("Set Speed", "Length:|0-999| Speed:|0-9|.|0-9| yard/sec |0-999| Different", , , , CommandType.BatchParameter),
 TranslateCommand("zh-TW", "布速設定", "碼長:|0-999| 布速:|0-9|.|0-9| 碼/秒 |0-999| 速度差"),
   Description("碼長 最大=999碼，布速 最大=9.9 碼/秒")>
Public NotInheritable Class Command34
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S34
    Off
    載入參數
    背景執行
  End Enum
  Public 碼長, 碼長參數, 布速參數1, 布速參數2, 布速, 速度差, TargetCycleTime As Integer
  Public 參數已變更 As Boolean

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      碼長 = param(1)
      碼長參數 = param(1) \ .Parameters.RopeNumber
      布速參數1 = param(2)
      布速參數2 = param(3)
      布速 = param(2) * 10 + param(3)
      速度差 = param(4)

      State = S34.載入參數
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S34.Off
          State = S34.Off

        Case S34.載入參數
          '馬達全速時的秒數
          '碼長參數 \ 主泵速度
          '碼長217，主泵速度1150，帶布輪1速度916，帶布輪2速度916
          '1150/1800=0.64，主泵輸出是64%
          '916/1800=51%
          'PumpSpeedYardPerSec=7.4, Reel1SpeedYardPerSec=9.4
          If 布速 > 0 And 碼長參數 > 0 Then
            TargetCycleTime = 碼長 * 10 \ 布速
            .PumpControl.PumpControl_MainPumpSpeed = MinMax(((碼長參數 * 10) \ .Parameters.PumpSpeedYardPerSec) * 1800 \ ((碼長參數 * 10) \ 布速), 0, 1800)
            If 速度差 > 50 Then
              .PumpControl.PumpControl_Reel1Speed = MinMax((.PumpControl.PumpControl_MainPumpSpeed - 速度差), 0, 1200)
              .PumpControl.PumpControl_Reel2Speed = MinMax((.PumpControl.PumpControl_MainPumpSpeed - 速度差), 0, 1200)
            Else
              .PumpControl.PumpControl_Reel1Speed = MinMax(((碼長參數 * 10) \ .Parameters.Reel1SpeedYardPerSec) * 1800 \ ((碼長參數 * 10) \ 布速), 0, 1800)
              .PumpControl.PumpControl_Reel2Speed = MinMax(((碼長參數 * 10) \ .Parameters.Reel2SpeedYardPerSec) * 1800 \ ((碼長參數 * 10) \ 布速), 0, 1800)
            End If
            .PumpControl.PumpControl_Reel1Speed = MinMax(((碼長參數 * 10) \ .Parameters.Reel1SpeedYardPerSec) * 1800 \ ((碼長參數 * 10) \ 布速), 0, 1800)
            .PumpControl.PumpControl_Reel2Speed = MinMax(((碼長參數 * 10) \ .Parameters.Reel2SpeedYardPerSec) * 1800 \ ((碼長參數 * 10) \ 布速), 0, 1800)
            .PumpControl.PumpControl_TargetCycleTime = MinMax((碼長 * 10 \ 布速), 20, 300)
          End If
          .PumpControl.PumpControl_MainPumpSpeed = 900
          .PumpControl.PumpControl_Reel1Speed = 600
          .PumpControl.PumpControl_Reel2Speed = 600
          參數已變更 = False
          State = S34.背景執行
          Return True

        Case S34.背景執行
          If Not 參數已變更 Then Exit Select
          State = S34.載入參數

      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S34.Off
  End Sub

  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
    碼長參數 = param(1)
    布速參數1 = param(2)
    布速參數2 = param(3)
    布速 = param(2) * 10 + param(3)
    參數已變更 = True
  End Sub


#Region "Standard Definitions"
  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S34.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S34
  Public Property State() As S34
    Get
      Return state_
    End Get
    Private Set(ByVal value As S34)
      state_ = value
    End Set
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command34 As New Command34(Me)
End Class
#End Region
