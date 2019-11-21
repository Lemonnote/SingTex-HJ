<Command("PH PARAMETER", "FillLevel |100-15000| CleanPipe |0-1 "), _
 TranslateCommand("zh-TW", "PH設定", "進水量 |100-15000| 是否清洗管路 |0-1|"), _
 Description("15000=MAX 100=MIN  0=NO 1=YES"), _
 TranslateDescription("zh-TW", "15000L=最高 100L=最少 ,0=否 1=是 ")> _
Public NotInheritable Class Command73
    Inherits MarshalByRefObject
    Implements ACCommand

    Public Enum S73
        Off
        Wash
        Wash1
        WashFinish
        Pause
    End Enum

    Public Wait As New Timer
    Public StateString As String
  Public CleanPipe As Double


    Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
        With ControlCode
      '--------------------------------------------------------------------------------------------------------PH用
            .PhControl.Cancel() : .PhWash.Cancel() : .PhControlFlag = False : .PhCirculateRun.Cancel() : .Command79.Cancel()
            .Command77.Cancel() : .Command74.Cancel() : .Command75.Cancel() : .Command76.Cancel() : .Command78.Cancel() : .Command80.Cancel()
            '---------------------------------------------------------------------------------------------------------
            .PhFillLevel = param(1)             '進水量
            .PhCirRun = False

            CleanPipe = MinMax(param(2), 0, 1)
            State = S73.Wash
        End With
    End Function

    Public Function Run() As Boolean Implements ACCommand.Run
        With ControlCode
            Select Case State

                Case S73.Wash
                    If CleanPipe = 1 Then
                        State = S73.Wash1
                        .PhCirRun = False
                        .pHWashRun = True
                    Else
                        State = S73.WashFinish
                    End If

                Case S73.Wash1
                    .PhCirRun = False
                    .PhWash.Run()
                    If .pHWashFinish Then
                        State = S73.WashFinish
                    End If

                Case S73.WashFinish
                    If .Parameters.PhCirRuning = 1 Then
                        .PhCirRun = True
                    End If
                    .PhCirRun = True
                    .PhWash.Cancel()
                    .pHWashRun = False
                    .pHWashFinish = False
                    State = S73.Off





            End Select

        End With
    End Function

    Public Sub Cancel() Implements ACCommand.Cancel
        State = S73.Off
        Wait.Cancel()
    End Sub

    'Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged

    'End Sub
    Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged

        ControlCode.PhFillLevel = param(1)             '進水量
    End Sub
#Region "Standard Definitions"
    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
        Get
            Return State <> S73.Off
        End Get
    End Property

    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S73
    Public Property State() As S73
        Get
            Return state_
        End Get
        Private Set(ByVal value As S73)
            state_ = value
        End Set
    End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
    Public ReadOnly Command73 As New Command73(Me)
End Class
#End Region
