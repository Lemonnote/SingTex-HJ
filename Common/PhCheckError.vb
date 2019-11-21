Public Class PhCheckError_
    Public Enum PhCheckError
        off
        CheckStart
        CheckPer5S
        StopAddPH
        StopAddPH2
    End Enum
    Public StateString As String
    Public Wait As New Timer
    Public Wait1 As New Timer
    Public StopAddPH, StopAddPH2 As Boolean
    Public SlaveTime As Integer
    Public PhValueData, PhOverAddTimes As Integer

    Public Sub Run()
        With ControlCode


            Select Case State
                Case PhCheckError.off
                    StateString = ""
                    PhOverAddTimes = 0
                    State = PhCheckError.CheckStart
                    '-------------------------------------------------------
                Case PhCheckError.CheckStart
                    SlaveTime = My.Computer.Clock.TickCount
                    PhValueData = .IO.PhValue

                    If StopAddPH2 = True Then
                        Wait.TimeRemaining = 20
                    Else

                        Wait.TimeRemaining = 10                         '-----------------修改
                    End If

                    State = PhCheckError.CheckPer5S
                    '-------------------------------------------------------
                Case PhCheckError.CheckPer5S                        '設定等待5秒時間,觀察5秒前與5秒後的變化
                    If Not Wait.Finished Then Exit Select
                    If .IO.PhValue > .PhControl.ExpectPh Then       '如果 PH實際值 > 設定值  代表 有可能準備下降PH值
                        If .IO.PhValue < PhValueData And (PhValueData - .IO.PhValue) >= 5 Then           ' 正在PH下降中  準備管制加藥
                            PhOverAddTimes = PhOverAddTimes + 1
                            If PhOverAddTimes > 3 Then              '超過三次確認，代表在PH下降中
                                PhOverAddTimes = 0
                                StopAddPH = True
                            End If
                        ElseIf .IO.PhValue < PhValueData And (PhValueData - .IO.PhValue) >= 20 Then           ' 正在PH下降中  準備管制加藥
                            PhOverAddTimes = PhOverAddTimes + 1
                            If PhOverAddTimes > 1 Then              '超過三次確認，代表在PH下降中
                                PhOverAddTimes = 0
                                StopAddPH2 = True
                            End If
                        Else
                            PhOverAddTimes = 0
                            StopAddPH = False
                            StopAddPH2 = False
                        End If
                    ElseIf .IO.PhValue <= .PhControl.ExpectPh Then

                        PhOverAddTimes = 0
                        StopAddPH = False
                        StopAddPH2 = False
                    End If
                    State = PhCheckError.CheckStart

            End Select

        End With

    End Sub
    Public Sub Cancel()
        State = PhCheckError.off

    End Sub
#Region "Standard Definitions"

    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As PhCheckError
    Public Property State() As PhCheckError
        Get
            Return state_
        End Get
        Private Set(ByVal value As PhCheckError)
            state_ = value
        End Set
    End Property
    Public ReadOnly Property IsOn() As Boolean
        Get
            Return (State <> PhCheckError.off)
        End Get
    End Property
    'Public ReadOnly Property PhFillCirculate() As Boolean
    '    Get
    '        Return (State = PhCheck.Wash1)
    '    End Get
    'End Property
    'Public ReadOnly Property PhDrain() As Boolean
    '    Get
    '        Return (State = PhCheck.Wash2) Or (State = PhCheck.WaitTime1)
    '    End Get
    'End Property
    '   <GraphTrace(-100, 1200, 500, 3500, "Blue", "%2tpH"), Translate("zh-TW", "PH設定值")> _
    'Public ReadOnly Property PHSetpoint() As Integer
    '       Get
    '           Return S13
    '       End Get
    '   End Property
#End Region
End Class

Partial Class ControlCode
    Public ReadOnly PhCheckError As New PhCheckError_(Me)
End Class
