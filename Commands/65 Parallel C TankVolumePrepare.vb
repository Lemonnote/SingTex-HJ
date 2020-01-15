<Command("ParallelC TankPrepare", "Mixer Time |0-999| Type |0-1| Call off |0-99| Qty% |0-100|", , , "('1/60)+5", CommandType.ParallelCommand), _
TranslateCommand("zh-TW", "平行C藥缸備藥", "攪拌時間 |0-999| 水源 |0-1| 呼叫 |0-99| 水量% |0-100|"), _
Description("999=MAX 0=UN MIX   1=COLD 0=CIRCULATE  0-99 Call off 100%=MAX 0%=MIN"), _
TranslateDescription("zh-TW", "999秒=最高 0秒=不攪拌   1=備清水 0=備回水  0-99 呼叫 100%=最高 0%=最小")> _
 Public NotInheritable Class Command65
    Inherits MarshalByRefObject
    Implements ACCommand

    Public Enum S65
        Off
        WaitAuto
        WaitNoAddButtons
        WaitLowLevel
        DispenseWaitReady
        DispenseWaitResponse
        Slow
        FillQty
        MixForTime
        WaitMixStop
        Ready
        Pause
    End Enum
    Public StateString As String

    Public Time, TimeWas, Type, Qty, CallOff As Integer
    Public WaitTimer As New Timer
    Public StepWas As Integer
  Public WaitRunning As New Timer

    Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
        With ControlCode
            Time = Maximum(param(1), 999)
      WaitRunning.TimeRemaining = (.Parameters.WaitOverTime * 60) + Time
      Type = MinMax(param(2), 0, 1)
            CallOff = param(3)
      If .ChemicalStepReady(CallOff) And .Parameters.EnableRepeatCallDispenser = 0 Then
        CallOff = 0
      End If
      Qty = MinMax(param(4) * 10, 0, 1000)
      .SPCConnectError = False
            .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
            .ChemicalTank = 0
            .ChemicalState = 101
            .Command55.Cancel()
            .Command57.Cancel()
            .Command67.Cancel()
            State = S65.WaitAuto
        End With
    End Function

    Public Function Run() As Boolean Implements ACCommand.Run
        With ControlCode
            Select Case State
                Case S65.Off
                    StateString = ""

                Case S65.WaitAuto
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S65.Pause
                        StepWas = .Parent.CurrentStep
                    End If
          If Not .IO.SystemAuto Then
            StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "Interlocked not In auto")
            Exit Select
          End If
          If .IO.CDrainPB Then
            StateString = If(.Language = LanguageValue.ZhTW, "請關閉C缸排水開關", "Please Switch Off Tank C Drain")
            Exit Select
          End If
          State = S65.WaitNoAddButtons

        Case S65.WaitNoAddButtons
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S65.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "等待藥缸", "Tank C Interlocked")
                    If .IO.C1Add Or .IO.C2Add Or .IO.C3Add Or .IO.C4Add Or .IO.C5Add Then Exit Select

                    If CallOff > 0 And .Parameters.ChemicalEnable = 1 Then
                        .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
                        .ChemicalTank = 0
                        State = S65.DispenseWaitReady
                    Else
                        State = S65.WaitLowLevel
                    End If

                Case S65.WaitLowLevel
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S65.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "C缸進水至低水位", "Tank C fill to low level")
                    If Not .CTankLowLevel Then Exit Select
                    State = S65.Slow


                Case S65.DispenseWaitReady
                    If .Parent.IsPaused Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S65.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "助劑備藥中", "Prepare Tank C")
                    'TODO  Add timeout code to switch to manual if no response
                    If .ChemicalState = EDispenseState.Ready Then
                        'Dispenser is ready so set CallOff number and wait for result
                        .ChemicalCallOff = CallOff
                        .ChemicalTank = 1
                        State = S65.DispenseWaitResponse
                    End If
                    'Switch to manual if enable parameter is changed or calloff is reset
                    If .Parameters.ChemicalEnable <> 1 Then State = S65.Slow
                    If CallOff = 0 Then State = S65.Slow

                Case S65.DispenseWaitResponse
                    If .Parent.IsPaused Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S65.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    Select Case .ChemicalState
                        Case EDispenseState.Complete
                            'Everything completed ok so set ready flag and carry on
                            .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
                            .ChemicalTank = 0
                            State = S65.Slow

                        Case EDispenseState.Manual
                            'Manual dispenses required so call the operator
                            .ChemicalCallOff = 0
                            State = S65.Slow

                        Case EDispenseState.Error
                            'Dispense error call the operator
                            .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
                            .ChemicalTank = 0
                            State = S65.Slow
                    End Select
                    'Switch to manual if enable parameter is changed or calloff is reset
                    If .Parameters.ChemicalEnable <> 1 Then State = S65.Slow
                    If CallOff = 0 Then State = S65.Slow

                Case S65.Slow
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S65.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "備藥完成，請按備藥OK", "Prepare Tank C")
                    .ChemicalCallOff = 0
                    .ChemicalTank = 0
                    If (CallOff > 0) And (.Parameters.CTankCallAck = 1) Then
                        .TankCReady = True
                    End If

                    If .TankCReady Then
                        If Qty > 5 Then
                            State = S65.FillQty
                        Else
                            State = S65.MixForTime
                            WaitTimer.TimeRemaining = Time
                        End If
                    End If

                Case S65.FillQty
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S65.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "C藥缸進水至 ", "Filling Tank C to ") & Qty / 10 & "%"
                    If .IO.TankCLevel > Qty Then
                        State = S65.MixForTime
                        WaitTimer.TimeRemaining = Time
                    End If

                Case S65.MixForTime
                    StateString = If(.Language = LanguageValue.ZhTW, "C藥缸攪拌 ", "Tank C mixing for ") & TimerString(WaitTimer.TimeRemaining)
                    If WaitTimer.Finished Then
                        State = S65.WaitMixStop
                        WaitTimer.TimeRemaining = 10
                    End If
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S65.Pause
                        StepWas = .Parent.CurrentStep
                    End If

                Case S65.WaitMixStop
                    StateString = If(.Language = LanguageValue.ZhTW, "等待C藥缸穩定 ", "Wait Tank C Stable ") & TimerString(WaitTimer.TimeRemaining)
                    If WaitTimer.Finished Then
                        State = S65.Ready
                    End If

                Case S65.Ready
                    State = S65.Off
          WaitRunning.Cancel()

                Case S65.Pause
                    StateString = If(.Language = LanguageValue.ZhTW, "暫停 ", "Paused ") & " " & TimerString(WaitTimer.TimeRemaining)
                    If Not .Parent.IsPaused And .IO.SystemAuto Then
                        If StepWas = .Parent.CurrentStep Then
                            If State = S65.MixForTime Then
                                State = StateWas
                                StateWas = S65.Off
                                If TimeWas = Time Then
                                    WaitTimer.Restart()
                                Else
                                    WaitTimer.TimeRemaining = Time
                                End If
                            Else
                                State = StateWas
                                StateWas = S65.Off
                                WaitTimer.Restart()
                            End If
                        Else
                            State = S65.Off
                            WaitTimer.Cancel()
              WaitRunning.Cancel()
              .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
                            .ChemicalTank = 0
                            .ChemicalState = 101
                        End If
                    End If
            End Select

        End With
    End Function

    Public Sub Cancel() Implements ACCommand.Cancel
        State = S65.Off
    WaitRunning.Cancel()
  End Sub

    Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
        TimeWas = Time
        Time = Maximum(param(1), 999)
        Type = MinMax(param(2), 0, 1)
        CallOff = param(3)
        Qty = MinMax(param(4) * 10, 0, 1000)
    End Sub


#Region " Standard Definitions "

    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
        Get
            Return State <> S65.Off
        End Get
    End Property
    Public ReadOnly Property IsTankInterlocked() As Boolean
        Get
            Return (State = S65.WaitNoAddButtons)
        End Get
    End Property
    Public ReadOnly Property IsFillingFresh() As Boolean
        Get
            Return (Type = 1) And ((State = S65.FillQty) Or (State = S65.WaitLowLevel))
        End Get
    End Property
    Public ReadOnly Property IsFillingCirc() As Boolean
        Get
            Return (Type = 0) And ((State = S65.FillQty) Or (State = S65.WaitLowLevel))
        End Get
    End Property
    Public ReadOnly Property IsSlow() As Boolean
        Get
            Return (State = S65.Slow)
        End Get
    End Property
    Public ReadOnly Property IsMixingForTime() As Boolean
        Get
            Return (State = S65.MixForTime)
        End Get
    End Property
    Public ReadOnly Property IsReady() As Boolean
        Get
            Return (State = S65.Ready)
        End Get
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S65
    Public Property State() As S65
        Get
            Return state_
        End Get
        Private Set(ByVal value As S65)
            state_ = value
        End Set
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S65
    Public Property StateWas() As S65
        Get
            Return statewas_
        End Get
        Private Set(ByVal value As S65)
            statewas_ = value
        End Set
    End Property


#End Region

End Class

#Region " Class Instance "

Partial Public Class ControlCode
  Public ReadOnly Command65 As New Command65(Me)
End Class

#End Region
