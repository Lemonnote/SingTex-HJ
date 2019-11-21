<Command("Set Speed", "Pump:|0-1800| Reel1:|0-1800| Reel2:|0-1800|RPM",,,, CommandType.BatchParameter),
 TranslateCommand("zh-TW", "主泵帶布輪控制", "主泵:|0-1800| 帶布輪1:|0-1800| 帶布輪2:|0-1800|RPM"),
   Description("最大=1800RPM")>
Public NotInheritable Class Command30
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S30
    Off
    載入參數
    背景執行
  End Enum
  Public 主泵速度參數, 帶布輪1速度參數, 帶布輪2速度參數 As Integer
  Public 參數已變更 As Boolean

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      主泵速度參數 = param(1)
      帶布輪1速度參數 = param(2)
      帶布輪2速度參數 = param(3)

      State = S30.載入參數
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S30.Off
          State = S30.Off

        Case S30.載入參數
          .PumpControl.PumpControl_MainPumpSpeed = MinMax(主泵速度參數, 0, 1800)
          .PumpControl.PumpControl_Reel1Speed = MinMax(帶布輪1速度參數, 0, 1800)
          .PumpControl.PumpControl_Reel2Speed = MinMax(帶布輪2速度參數, 0, 1800)
          參數已變更 = False
          State = S30.背景執行
          Return True

        Case S30.背景執行
          If Not 參數已變更 Then Exit Select
          State = S30.載入參數

      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S30.Off
  End Sub

  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
    主泵速度參數 = param(1)
    帶布輪1速度參數 = param(2)
    帶布輪2速度參數 = param(3)
    參數已變更 = True
  End Sub


#Region "Standard Definitions"
  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S30.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S30
  Public Property State() As S30
    Get
      Return state_
    End Get
    Private Set(ByVal value As S30)
      state_ = value
    End Set
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command30 As New Command30(Me)
End Class
#End Region
