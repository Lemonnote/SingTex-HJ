<Command("ReachTemperature", "Time |1-999| mins", , , ), _
 TranslateCommand("zh-TW", "溫度到達", "溫度 |1-999|")> _
Public NotInheritable Class Command23
    Inherits MarshalByRefObject
    Implements ACCommand

    Public Enum S23
        Off
    End Enum

    Public Wait As New Timer

    Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
        With ControlCode
            .CallTemperature = param(1) * 10
            .ReachTemperature_Check = True
        End With
    End Function

    Public Function Run() As Boolean Implements ACCommand.Run
        With ControlCode
            Select Case State
                Case S23.Off
                    State = S23.Off
            End Select
        End With
    End Function

    Public Sub Cancel() Implements ACCommand.Cancel
        State = S23.Off
        Wait.Cancel()
    End Sub

    Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged

    End Sub


#Region "Standard Definitions"
    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
        Get
            Return State <> S23.Off
        End Get
    End Property


    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S23
    Public Property State() As S23
        Get
            Return state_
        End Get
        Private Set(ByVal value As S23)
            state_ = value
        End Set
    End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
    Public ReadOnly Command23 As New Command23(Me)
End Class
#End Region
