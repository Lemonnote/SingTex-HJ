Public Class PhCirculateRun_
    Public Enum PhCirculateRun
        Off
        Run
        LowLevel
        NotLowLevel
        HighLevel
        Wait

        test
        Run1
        CLowLevel
        NotCLowLevel
        CHighLevel
        Wait2
        Pause
    End Enum

    Public Wait, WaitLevel As New Timer
    Public C37, DoseOutput As Integer

    Public Sub Run()
        With ControlCode
            Select Case State
                '------------------------------------------------------起始動作
                Case PhCirculateRun.Off
                    If .Parameters.PhCirTank = 1 Then
                        State = PhCirculateRun.Run
                    Else
                        State = PhCirculateRun.Run1
                    End If


                    '--------------------------------------------------判斷目前動作
                Case PhCirculateRun.Run

                    If .Parent.IsPaused Or Not .IO.MainPumpFB Then
                        StateWas = State
                        State = PhCirculateRun.Pause
                        Wait.Pause()
                    End If

                    If .IO.PhMixTankHighLevel Then
                        WaitLevel.TimeRemaining = 3
                        State = PhCirculateRun.HighLevel                    '如有B缸高水位，跳 PhCirculateRun.HighLevel 
                    ElseIf .IO.PhMixTankLowLevel And Not .IO.PhMixTankHighLevel Then
                        WaitLevel.TimeRemaining = 5
                        State = PhCirculateRun.LowLevel                     '如有B缸低水位，跳 PhCirculateRun.LowLevel
                    Else
                        State = PhCirculateRun.NotLowLevel                     '如沒B缸低水位，跳 PhCirculateRun.NotLowLevel
                    End If

                    '---------------------------------------------------等待低水位消失（開馬達、下料閥、加藥閥）
                Case PhCirculateRun.HighLevel

                    If .Parent.IsPaused Or Not .IO.MainPumpFB Then
                        StateWas = State
                        State = PhCirculateRun.Pause
                        Wait.Pause()
                    End If

                    If Not WaitLevel.Finished Then Exit Select
                    State = PhCirculateRun.Run


                    '---------------------------------------------------等待低水位消失（開馬達、下料閥、加藥閥、開迴水）
                Case PhCirculateRun.LowLevel

                    If .Parent.IsPaused Or Not .IO.MainPumpFB Then
                        StateWas = State
                        State = PhCirculateRun.Pause
                        Wait.Pause()
                    End If

                    If Not WaitLevel.Finished Then Exit Select
                    If .IO.PhMixTankHighLevel = True Then
                        State = PhCirculateRun.Run
                        Exit Select
                    ElseIf .IO.PhMixTankLowLevel Then
                        WaitLevel.TimeRemaining = 3
                        State = PhCirculateRun.LowLevel
                        Exit Select
                    ElseIf .IO.PhMixTankLowLevel = False Then
                        State = PhCirculateRun.NotLowLevel
                        Exit Select
                    End If

                    '---------------------------------------------------等待低水位到達(開迴水)
                Case PhCirculateRun.NotLowLevel

                    If .Parent.IsPaused Or Not .IO.MainPumpFB Then
                        StateWas = State
                        State = PhCirculateRun.Pause
                        Wait.Pause()
                    End If

                    DoseOutput = 0
                    If .IO.PhMixTankLowLevel Then

                        Wait.TimeRemaining = .Parameters.CirFillDelayTime
                        State = PhCirculateRun.Wait
                    End If

                    '---------------------------------------------------等待低水位到達(開迴水)
                Case PhCirculateRun.Wait

                    If .Parent.IsPaused Or Not .IO.MainPumpFB Then
                        StateWas = State
                        State = PhCirculateRun.Pause
                        Wait.Pause()
                    End If

                    DoseOutput = 0
                    If Wait.Finished Then
                        State = PhCirculateRun.Run
                    End If

                    '==============================沒有回流桶動作=================================================================
                    '--------------------------------------------------判斷目前動作
                Case PhCirculateRun.Run1

                    If .Parent.IsPaused Or Not .IO.MainPumpFB Then
                        StateWas = State
                        State = PhCirculateRun.Pause
                        Wait.Pause()
                    End If

          If .BTankMiddleLevel Then
            WaitLevel.TimeRemaining = 3
            State = PhCirculateRun.CHighLevel                    '如有C缸中水位，跳 PhCirculateRun.HighLevel 
          ElseIf .BTankLowLevel And Not .BTankMiddleLevel Then
            WaitLevel.TimeRemaining = 5
            State = PhCirculateRun.CLowLevel                     '如有C缸低水位，跳 PhCirculateRun.LowLevel
          Else
            State = PhCirculateRun.NotCLowLevel                     '如沒C缸低水位，跳 PhCirculateRun.NotLowLevel
          End If

                    '---------------------------------------------------等待低水位消失（開馬達、下料閥、加藥閥）
                Case PhCirculateRun.CHighLevel

                    If .Parent.IsPaused Or Not .IO.MainPumpFB Then
                        StateWas = State
                        State = PhCirculateRun.Pause
                        Wait.Pause()
                    End If

                    If Not WaitLevel.Finished Then Exit Select
                    State = PhCirculateRun.Run1


                    '---------------------------------------------------等待低水位消失（開馬達、下料閥、加藥閥、開迴水）
        Case PhCirculateRun.CLowLevel

          If .Parent.IsPaused Or Not .IO.MainPumpFB Then
            StateWas = State
            State = PhCirculateRun.Pause
            Wait.Pause()
          End If

          If Not WaitLevel.Finished Then Exit Select
          If .BTankMiddleLevel = True Then
            State = PhCirculateRun.Run1
            Exit Select
          ElseIf .BTankLowLevel Then
            WaitLevel.TimeRemaining = 3
            State = PhCirculateRun.CLowLevel
            Exit Select
          ElseIf .BTankLowLevel = False Then
            State = PhCirculateRun.NotCLowLevel
            Exit Select
          End If

          '---------------------------------------------------等待低水位到達(開迴水)
        Case PhCirculateRun.NotCLowLevel

          If .Parent.IsPaused Or Not .IO.MainPumpFB Then
            StateWas = State
            State = PhCirculateRun.Pause
            Wait.Pause()
          End If

          DoseOutput = 0
          If .BTankLowLevel Then

            Wait.TimeRemaining = .Parameters.CirFillDelayTime
            State = PhCirculateRun.Wait2
          End If

          '---------------------------------------------------等待低水位到達(開迴水)
                Case PhCirculateRun.Wait2

                    If .Parent.IsPaused Or Not .IO.MainPumpFB Then
                        StateWas = State
                        State = PhCirculateRun.Pause
                        Wait.Pause()
                    End If

                    DoseOutput = 0
                    If Wait.Finished Then
                        State = PhCirculateRun.Run1
                    End If

                    '------------------------------------------------------

                Case PhCirculateRun.Pause
                    If (Not .Parent.IsPaused) And .IO.MainPumpFB Then
                        State = StateWas
                        StateWas = PhCirculateRun.Off
                        If State = PhCirculateRun.Run Then
                            Wait.Pause()
                        Else
                            Wait.Restart()
                        End If
                    End If


            End Select


        End With

    End Sub
    Public Sub Cancel()
        State = PhCirculateRun.Off

    End Sub
#Region "Standard Definitions"

    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    Friend ReadOnly Property IsOn() As Boolean
        Get
            Return State <> PhCirculateRun.Off
        End Get
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As PhCirculateRun
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As PhCirculateRun
    Public Property State() As PhCirculateRun
        Get
            Return state_
        End Get
        Private Set(ByVal value As PhCirculateRun)
            state_ = value
        End Set
    End Property
    Public Property StateWas() As PhCirculateRun
        Get
            Return statewas_
        End Get
        Private Set(ByVal value As PhCirculateRun)
            statewas_ = value
        End Set
    End Property

    Public ReadOnly Property PHTankAddPump() As Boolean          'PH循環泵
        Get
            Return (State = PhCirculateRun.LowLevel) Or (State = PhCirculateRun.HighLevel)
        End Get
    End Property

    Public ReadOnly Property PhInToMachine() As Boolean               '入染機
        Get
            Return (State = PhCirculateRun.LowLevel) Or (State = PhCirculateRun.HighLevel)
        End Get
    End Property

    Public ReadOnly Property PhFillCirculate2() As Boolean         'PH迴水
        Get
            Return (State = PhCirculateRun.NotLowLevel) Or (State = PhCirculateRun.Wait) Or (State = PhCirculateRun.LowLevel)
        End Get
    End Property

    Public ReadOnly Property CAddPump() As Boolean          'C加藥馬達
        Get
            Return (State = PhCirculateRun.CLowLevel) Or (State = PhCirculateRun.CHighLevel)
        End Get
    End Property

    Public ReadOnly Property CtankToMachine() As Boolean               '入染機
        Get
            Return (State = PhCirculateRun.CLowLevel) Or (State = PhCirculateRun.CHighLevel)
        End Get
    End Property

    Public ReadOnly Property PhFillCirculate3() As Boolean         'PH迴水
        Get
            Return (State = PhCirculateRun.NotCLowLevel) Or (State = PhCirculateRun.Wait2) Or (State = PhCirculateRun.CLowLevel)
        End Get
    End Property
#End Region
End Class

Partial Class ControlCode
    Public ReadOnly PhCirculateRun As New PhCirculateRun_(Me)
End Class
