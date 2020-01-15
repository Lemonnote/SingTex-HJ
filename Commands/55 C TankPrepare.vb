<Command("C Tank Prepare", "Mixer Time |0-999| Type |0-1| Call off |0-99| Qty% |0-100|", , , "('1/60)+5", CommandType.Standard), _
TranslateCommand("zh-TW", "C藥缸水量備藥", "攪拌時間 |0-999| 水源 |0-1| 呼叫 |0-99| 水量% |0-100|"), _
Description("999=MAX 0=UN MIX   1=COLD 0=CIRCULATE  0-99 Call off 100%=MAX 0%=MIN"), _
TranslateDescription("zh-TW", "999秒=最高 0秒=不攪拌   1=備清水 0=備回水  0-99 呼叫 100%=最高 0%=最小")> _
 Public NotInheritable Class Command55
    Inherits MarshalByRefObject
    Implements ACCommand

    Public Enum S55
        Off
        WaitAuto
        WaitNoAddButtons
        WaitLowLevel
        DispenseWaitReady
        ResetCallOff
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
  Public WaitRunning As New Timer

    Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
        With ControlCode
            'cancels for all other forground functions
            .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
            .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
            .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
            .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
            .Command10.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
            .Command59.Cancel() : .Command61.Cancel() : .Command65.Cancel() : .Command67.Cancel()
            .Command01.Cancel()
            .TemperatureControl.Cancel()
            .TempControlFlag = False
            .SPCConnectError = False
            .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
            .ChemicalTank = 0
            .ChemicalState = 101
      WaitRunning.TimeRemaining = .Parameters.WaitOverTime * 60

            Time = Maximum(param(1), 999)
            Type = MinMax(param(2), 0, 1)
      CallOff = param(3)
      If .ChemicalStepReady(CallOff) And .Parameters.EnableRepeatCallDispenser = 0 Then
        CallOff = 0
      End If
      Qty = MinMax(param(4) * 10, 0, 1000)
        .RunCTankPrepare = True
      WaitTimer.TimeRemaining = 10
      State = S55.WaitAuto
    End With
    End Function

    Public Function Run() As Boolean Implements ACCommand.Run
        With ControlCode
            Select Case State
                Case S55.Off
                    StateString = ""

                Case S55.WaitAuto
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S55.Pause
                    End If
          If Not .IO.SystemAuto Then
            StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "Interlocked not In auto")
            Exit Select
          End If
          If .IO.CDrainPB Then
            StateString = If(.Language = LanguageValue.ZhTW, "請關閉C缸排水開關", "Please Switch Off Tank C Drain")
            Exit Select
          End If
          If Not WaitTimer.Finished Then
            StateString = If(.Language = LanguageValue.ZhTW, "等待C缸備藥", "Wait For Tank C Prepare")
            Exit Select
          End If
          State = S55.Off
        'State = S55.WaitNoAddButtons

        Case S55.WaitNoAddButtons
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S55.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "等待藥缸", "Tank C Interlocked")
                    If .IO.C1Add Or .IO.C2Add Or .IO.C3Add Or .IO.C4Add Or .IO.C5Add Then Exit Select

                    If CallOff > 0 And .Parameters.ChemicalEnable = 1 Then
                        .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
                        .ChemicalTank = 0
                        .TankCReady = False
                        State = S55.DispenseWaitReady
                    Else
                        State = S55.WaitLowLevel
                    End If

                Case S55.WaitLowLevel
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S55.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "C缸進水至低水位", "Tank C fill to low level")
                    If Not .CTankLowLevel Then Exit Select
                    State = S55.Slow

                Case S55.DispenseWaitReady
                    If .Parent.IsPaused Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S55.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "助劑備藥中", "Prepare Tank C")
                    'TODO  Add timeout code to switch to manual if no response
                    If .ChemicalState = EDispenseState.Ready Then
                        'Dispenser is ready so set CallOff number and wait for result
                        .ChemicalCallOff = CallOff
                        .ChemicalTank = 1
                        State = S55.DispenseWaitResponse
                    End If
                    'Switch to manual if enable parameter is changed or calloff is reset
                    If .Parameters.ChemicalEnable <> 1 Then State = S55.Slow
                    If CallOff = 0 Then State = S55.Slow

                Case S55.DispenseWaitResponse
                    If .Parent.IsPaused Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S55.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "等待助劑計量", "Wait for Chemical Dispensing")
                    Select Case .ChemicalState
                        Case EDispenseState.Complete
                            'Everything completed ok so set ready flag and carry on
                            .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
                            .ChemicalTank = 0
                            State = S55.Slow

                        Case EDispenseState.Manual
                            'Manual dispenses required so call the operator
                            .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
                            .ChemicalTank = 0
                            State = S55.Slow

                        Case EDispenseState.Error
                            'Dispense error call the operator
                            .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
                            .ChemicalTank = 0
                            State = S55.Slow
                    End Select
                    'Switch to manual if enable parameter is changed or calloff is reset
                    If .Parameters.ChemicalEnable <> 1 Then State = S55.Slow
                    If CallOff = 0 Then State = S55.Slow

                Case S55.Slow
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S55.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "備藥完成，請按備藥OK", "Prepare Tank C")
          .ChemicalCallOff = 0
                    .ChemicalTank = 0
                    If (CallOff > 0) And (.Parameters.CTankCallAck = 1) Then
                        .TankCReady = True
                    End If
                    If .TankCReady Then
                        If Qty > 5 Then
                            State = S55.FillQty
                        Else
                            State = S55.MixForTime
                            WaitTimer.TimeRemaining = Time
                        End If
                    End If

                Case S55.FillQty
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S55.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "C藥缸進水至 ", "Filling Tank C to ") & Qty / 10 & "%"
                    If .IO.TankCLevel > Qty Then
                        State = S55.MixForTime
                        WaitTimer.TimeRemaining = Time
                    End If

                Case S55.MixForTime
                    StateString = If(.Language = LanguageValue.ZhTW, "C藥缸攪拌 ", "Tank C mixing for ") & TimerString(WaitTimer.TimeRemaining)
                    If WaitTimer.Finished Then
                        State = S55.WaitMixStop
                        WaitTimer.TimeRemaining = 10
                    End If
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S55.Pause
                    End If

                Case S55.WaitMixStop
                    StateString = If(.Language = LanguageValue.ZhTW, "等待C藥缸穩定 ", "Wait Tank C Stable ") & TimerString(WaitTimer.TimeRemaining)
                    If WaitTimer.Finished Then
                        State = S55.Ready
                    End If

                Case S55.Ready
                    State = S55.Off
          WaitRunning.Cancel()

                Case S55.Pause
                    StateString = If(.Language = LanguageValue.ZhTW, "暫停 ", "Paused ") & " " & TimerString(WaitTimer.TimeRemaining)
                    If .Parent.CurrentStep <> .Parent.ChangingStep Then
                        State = S55.Off
                        WaitTimer.Cancel()
            WaitRunning.Cancel()
            .ChemicalCallOff = 0
                        .ChemicalTank = 0
                        .ChemicalState = 101
                    End If
                    If Not .Parent.IsPaused And .IO.SystemAuto Then
                        If StateWas = S55.MixForTime Then
                            State = StateWas
                            StateWas = S55.Off
                            If TimeWas = Time Then
                                WaitTimer.Restart()
                            Else
                                WaitTimer.TimeRemaining = Time
                            End If
                        Else
                            State = StateWas
                            StateWas = S55.Off
                            WaitTimer.Restart()
                        End If
                    End If
            End Select

        End With
    End Function

    Public Sub Cancel() Implements ACCommand.Cancel
        State = S55.Off
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
            Return State <> S55.Off
        End Get
    End Property
    Public ReadOnly Property IsTankInterlocked() As Boolean
        Get
            Return (State = S55.WaitNoAddButtons)
        End Get
    End Property
    Public ReadOnly Property IsFillingFresh() As Boolean
        Get
            Return (Type = 1) And ((State = S55.FillQty) Or (State = S55.WaitLowLevel))
        End Get
    End Property
    Public ReadOnly Property IsFillingCirc() As Boolean
        Get
            Return (Type = 0) And ((State = S55.FillQty) Or (State = S55.WaitLowLevel))
        End Get
    End Property
    Public ReadOnly Property IsSlow() As Boolean
        Get
            Return (State = S55.Slow)
        End Get
    End Property
    Public ReadOnly Property IsMixingForTime() As Boolean
        Get
            Return (State = S55.MixForTime)
        End Get
    End Property
    Public ReadOnly Property IsReady() As Boolean
        Get
            Return (State = S55.Ready)
        End Get
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S55
    Public Property State() As S55
        Get
            Return state_
        End Get
        Private Set(ByVal value As S55)
            state_ = value
        End Set
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S55
    Public Property StateWas() As S55
        Get
            Return statewas_
        End Get
        Private Set(ByVal value As S55)
            statewas_ = value
        End Set
    End Property


#End Region

End Class

#Region " Class Instance "

Partial Public Class ControlCode
  Public ReadOnly Command55 As New Command55(Me)
End Class

#End Region
