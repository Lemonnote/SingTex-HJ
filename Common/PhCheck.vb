Public Class PhCheck_
    Public Enum PhCheck
        off
        DelayCheck1
        DelayCheck2
        Check1
        Check2
        Finish
    End Enum
    Public StateString As String
    Public Wait As New Timer
    Public Wait1 As New Timer
    Public DelayWait As New Timer
    Public S10, S11, S12, X, X2, S13 As Integer
    Private PhCheckBefore(1000) As Integer

    Public Sub Run()
        With ControlCode
            If Wait1.TimeRemaining = 0 Then
                Wait1.TimeRemaining = .Parameters.PhSamplingTime
                PhCheckBefore(X) = .PhControl.ExpectPh
                X = X + 1
            End If

            Select Case State
                Case PhCheck.off
                    StateString = ""
                    State = PhCheck.DelayCheck1
                    '-------------------------------------------------------載入"延遲檢查時間"
                Case PhCheck.DelayCheck1                                '
                    DelayWait.TimeRemaining = .Parameters.DoublePhSample
                    State = PhCheck.DelayCheck2

                    '-------------------------------------------------------等待"延遲檢查時間"結束
                Case PhCheck.DelayCheck2
                    If DelayWait.Finished Then
                        State = PhCheck.Check1
                        Exit Select
                    End If

                    '-------------------------------------------------------載入每段"取樣時間"
                Case PhCheck.Check1
                    Wait.TimeRemaining = .Parameters.PhSamplingTime
                    State = PhCheck.Check2

                    '-------------------------------------------------------等待每段"取樣時間"結束
                Case PhCheck.Check2
                    If Wait.Finished Then
                        '------------------------------------------------------------------------------------------------
                        '如果 PH實際值 高於 PH設定值
                        '------------------------------------------------------------------------------------------------
                        S12 = PhCheckBefore(X2)
                        S13 = .PhControl.ExpectPh
                        'S12 = PHSetpoint
                        If .IO.PhValue > PhCheckBefore(X2) Then



                            S10 = St.GetAmount(.IO.PhValue, PhCheckBefore(X2))
                            'PH補差額  =  ( PH實際值 , PH設定值 )

                            Dim a1, a2, a3 As Double

                            a1 = (S10 * .PhControl.PhFillLevel)

                            a2 = a1 / .PhControl.PhPumpOutRatio

                            a3 = a2 / .PhControl.PhConcentration

                            S10 = CType(((S10 * .PhControl.PhFillLevel) / .PhControl.PhPumpOutRatio) / .PhControl.PhConcentration, Integer)
                            'PH補差額加酸量 （加成後確認） =  ((( 實際PH - 設定PH ) ＊ 進水量比率 ) / PH泵加酸比率 / 濃度比率 )

                            '------------------------------------------20111019

                            S10 = CType(S10 * 0.2, Integer)
                            '------------------------------------------

                            S11 = .PhControl.Wait10Second.TimeRemaining - .PhControl.WaitAddHac.TimeRemaining
                            '還剩多少空餘時間  =  全部時間  - 加酸時間

                            If S11 >= S10 Then          '如果 剩空餘時間  >= PH補差額加酸量  , 把補差額加酸量，納入剩空餘時間

                                .PhControl.WaitAddHac.TimeRemaining = .PhControl.WaitAddHac.TimeRemaining + S10
                                If .PhControl.WaitAddHac.TimeRemaining > 1 Then
                                    .PhControl.OpenHacValve = True
                                ElseIf .PhControl.WaitAddHac.TimeRemaining < 1 Then
                                    .PhControl.OpenHacValve = False
                                End If
                            ElseIf S11 < S10 Then       '如果 剩空餘時間  不足於 PH補差額加酸量 , 將剩下的不足空餘時間全部加酸
                                .PhControl.WaitAddHac.TimeRemaining = .PhControl.WaitAddHac.TimeRemaining + S11
                                If .PhControl.WaitAddHac.TimeRemaining > 1 Then
                                    .PhControl.OpenHacValve = True
                                ElseIf .PhControl.WaitAddHac.TimeRemaining < 1 Then
                                    .PhControl.OpenHacValve = False
                                End If
                            End If
                            X2 = 0
                            X = 0
                            State = PhCheck.DelayCheck1
                            Exit Select

                        Else
                            X2 = X2 + 1
                            State = PhCheck.Check1
                            Exit Select
                        End If

                        '-----------------------------------------------------------------------------------------------

                    End If

            End Select

        End With
 
    End Sub
    Public Sub Cancel()
        State = PhCheck.off

    End Sub
#Region "Standard Definitions"

    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As PhCheck
    Public Property State() As PhCheck
        Get
            Return state_
        End Get
        Private Set(ByVal value As PhCheck)
            state_ = value
        End Set
    End Property
    Public ReadOnly Property IsOn() As Boolean
        Get
            Return (State <> PhCheck.off)
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

    '   <GraphTrace(1, 1200, 500, 3500, "Blue", "%2tpH"), Translate("zh-TW", "PH設定值")> _
    'Public ReadOnly Property PHSetpoint() As Integer
    '       Get
    '           Return S13
    '       End Get
    '   End Property
#End Region
End Class

Partial Class ControlCode
    Public ReadOnly PhCheck As New PhCheck_(Me)
End Class
