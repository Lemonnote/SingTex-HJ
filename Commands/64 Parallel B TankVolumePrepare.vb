<Command("ParallelB TankPrepare", "Mixer Time |0-999| Type |0-1| Call off |0-99| Qty% |0-100|", , , "('1/60)+5", CommandType.ParallelCommand),
TranslateCommand("zh-TW", "平行B藥缸備藥", "攪拌時間 |0-999| 水源 |0-1| 呼叫 |0-99| 水量% |0-100|"),
Description("999=MAX 0=UN MIX   1=COLD 0=CIRCULATE  0-99 Call off 100%=MAX 0%=MIN"),
TranslateDescription("zh-TW", "999秒=最高 0秒=不攪拌   1=備清水 0=備回水  0-99 呼叫 100%=最高 0%=最小")>
Public NotInheritable Class Command64
    Inherits MarshalByRefObject                       'Inheritsg是繼承Windows Form的應用程式要繼承System.Windows.Forms.Form，可先參考物件導向程式設計相關書籍
    Implements ACCommand

    Public Enum S64
        Off
        WaitAuto
        WaitNoAddButtons
        FillToLowLevel
        DispenseWaitReady
        DispenseWaitResponse
        Slow
        FillQty
        WaitMixStart
        Sprinkle
        MixForTime
        WaitMixStop
        Ready
        Pause
    End Enum
    Public StateString As String

    Public Time, TimeWas, Type, Qty, CallOff As Integer
    Public WaitTimer As New Timer
    Public SprinkleTimer As New Timer
    Public StepWas As Integer
  Public WaitRunning As New Timer

    Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
        With ControlCode
            Time = Maximum(param(1), 999)
      WaitRunning.TimeRemaining = (.Parameters.WaitOverTime * 60) + Time
      Type = MinMax(param(2), 0, 1)
            CallOff = param(3)
            Qty = MinMax(param(4) * 10, 0, 1000)
            .SPCConnectError = False
            .DyeCallOff = 0   'Starts the handshake with the host / auto dispenser
            .DyeTank = 0
            .DyeState = 101
            .Command54.Cancel()
            .Command56.Cancel()
            .Command66.Cancel()
            State = S64.WaitAuto
        End With
    End Function

    Public Function Run() As Boolean Implements ACCommand.Run
        With ControlCode
            Select Case State
                Case S64.Off
                    StateString = ""

                Case S64.WaitAuto
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S64.Pause
                        StepWas = .Parent.CurrentStep
                    End If
          If Not .IO.SystemAuto Then
            StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "Interlocked not In auto")
            Exit Select
          End If
          If .IO.BDrainPB Then
            StateString = If(.Language = LanguageValue.ZhTW, "請關閉B缸排水開關", "Please Switch Off Tank B Drain")
            Exit Select
          End If
          State = S64.WaitNoAddButtons

        Case S64.WaitNoAddButtons
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S64.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "等待藥缸", "Tank B Interlocked")
                    If .IO.B1Add Or .IO.B2Add Or .IO.B3Add Or .IO.B4Add Or .IO.B5Add Then Exit Select
                    State = S64.FillToLowLevel
                    .MessageSTankFilling = True

                Case S64.FillToLowLevel
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S64.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "B藥缸進水至低水位", "Tank B Filling to  level")
                    If .BTankLowLevel Then
                        .MessageSTankFilling = False
                        .MessageSTankPrepare = True
                        State = S64.Slow
                        If CallOff > 0 And .Parameters.DyeEnable = 1 Then
                            .DyeCallOff = 0   'Starts the handshake with the host / auto dispenser
                            .DyeTank = 0
                            State = S64.DispenseWaitReady
                        End If
                    End If

                Case S64.DispenseWaitReady
                    If .Parent.IsPaused Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S64.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "染料備藥中", "Prepare Tank B")
                    'TODO  Add timeout code to switch to manual if no response
                    If .DyeState = EDispenseState.Ready Then
                        'Dispenser is ready so set CallOff number and wait for result
                        .DyeCallOff = CallOff
                        .DyeTank = 1
                        State = S64.DispenseWaitResponse
                    End If
                    'Switch to manual if enable parameter is changed or calloff is reset
                    If .Parameters.DyeEnable <> 1 Then State = S64.Slow
                    If CallOff = 0 Then State = S64.Slow

                Case S64.DispenseWaitResponse
                    If .Parent.IsPaused Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S64.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    Select Case .DyeState
                        Case EDispenseState.Complete
                            'Everything completed ok so set ready flag and carry on
                            .DyeCallOff = 0   'Starts the handshake with the host / auto dispenser
                            .DyeTank = 0
                            State = S64.Slow

                        Case EDispenseState.Manual
                            'Manual dispenses required so call the operator
                            .DyeCallOff = 0   'Starts the handshake with the host / auto dispenser
                            .DyeTank = 0
                            State = S64.Slow

                        Case EDispenseState.Error
                            'Dispense error call the operator
                            .DyeCallOff = 0   'Starts the handshake with the host / auto dispenser
                            .DyeTank = 0
                            State = S64.Slow
                    End Select
                    'Switch to manual if enable parameter is changed or calloff is reset
                    If .Parameters.DyeEnable <> 1 Then State = S64.Slow
                    If CallOff = 0 Then State = S64.Slow

                Case S64.Slow
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S64.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "備藥完成，請按備藥OK", "Prepare Tank B")
                    .DyeCallOff = 0
                    .DyeTank = 0
                    If (CallOff > 0) And (.Parameters.BTankCallAck = 1) Then
                        .TankBReady = True
                    End If

                    If .TankBReady Then
                        If Qty > 5 Then
                            State = S64.FillQty
                        Else
                            State = S64.MixForTime
                        End If
                    End If

                Case S64.FillQty
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S64.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "B藥缸進水 ", "Filling Tank B to ") & Qty / 10 & "%"
                    If .IO.TankBLevel > Qty Then
                        If Time = 0 Or CallOff > 0 Then
                            State = S64.MixForTime
                            WaitTimer.TimeRemaining = Time
                        Else
                            State = S64.Sprinkle
                            WaitTimer.TimeRemaining = 5
                        End If
                    End If

                Case S64.WaitMixStart
                    StateString = If(.Language = LanguageValue.ZhTW, "等待B藥缸攪拌啟動 ", "Wait Tank B start mix") & TimerString(WaitTimer.TimeRemaining)
                    If WaitTimer.Finished Then
                        State = S64.Sprinkle
                        WaitTimer.TimeRemaining = Time
                        SprinkleTimer.TimeRemaining = .Parameters.BTankSprinkleTimeWhenMixing
                    End If
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        StateWas = State
                        State = S64.Pause
                        WaitTimer.Pause()
                        StepWas = .Parent.CurrentStep
                    End If

                Case S64.Sprinkle
                    StateString = If(.Language = LanguageValue.ZhTW, "B藥缸攪拌 ", "Tank B mixing for ") & TimerString(WaitTimer.TimeRemaining)
                    If SprinkleTimer.Finished Then
                        State = S64.MixForTime
                    End If
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        StateWas = State
                        State = S64.Pause
                        WaitTimer.Pause()
                        StepWas = .Parent.CurrentStep
                    End If

                Case S64.MixForTime
                    StateString = If(.Language = LanguageValue.ZhTW, "B藥缸攪拌 ", "Tank B mixing for ") & TimerString(WaitTimer.TimeRemaining)
                    If WaitTimer.Finished Then
                        State = S64.WaitMixStop
                        WaitTimer.TimeRemaining = 10
                    End If
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        WaitTimer.Pause()
                        StateWas = State
                        State = S64.Pause
                        StepWas = .Parent.CurrentStep
                    End If

                Case S64.WaitMixStop
                    StateString = If(.Language = LanguageValue.ZhTW, "等待B藥缸穩定 ", "Wait Tank B Stable ") & TimerString(WaitTimer.TimeRemaining)
                    If WaitTimer.Finished Then
                        State = S64.Ready
                    End If


                Case S64.Ready
                    State = S64.Off
          WaitRunning.Cancel()


                Case S64.Pause
                    StateString = If(.Language = LanguageValue.ZhTW, "暫停 ", "Paused ") & " " & TimerString(WaitTimer.TimeRemaining)
                    If Not .Parent.IsPaused And .IO.SystemAuto Then
                        If StepWas = .Parent.CurrentStep Then
                            If State = S64.MixForTime Then
                                State = StateWas
                                StateWas = S64.Off
                                If TimeWas = Time Then
                                    WaitTimer.Restart()
                                Else
                                    WaitTimer.TimeRemaining = Time
                                End If
                            Else
                                State = StateWas
                                StateWas = S64.Off
                                WaitTimer.Restart()
                            End If
                        Else
                            State = S64.Off
                            WaitTimer.Cancel()
              WaitRunning.Cancel()
            End If
                    End If
            End Select

        End With
    End Function

    Public Sub Cancel() Implements ACCommand.Cancel
        State = S64.Off
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
            Return State <> S64.Off
        End Get
    End Property
    Public ReadOnly Property IsTankInterlocked() As Boolean
        Get
            Return (State = S64.WaitNoAddButtons)
        End Get
    End Property
    Public ReadOnly Property IsFillingFresh() As Boolean
        Get
            Return ((Type = 1) And ((State = S64.FillQty) Or (State = S64.FillToLowLevel))) Or (State = S64.Sprinkle)
        End Get
    End Property
    Public ReadOnly Property IsFillingCirc() As Boolean
        Get
            Return (Type = 0) And ((State = S64.FillQty) Or (State = S64.FillToLowLevel))
        End Get
    End Property
    Public ReadOnly Property IsSlow() As Boolean
        Get
            Return (State = S64.Slow)
        End Get
    End Property
    Public ReadOnly Property IsMixingForTime() As Boolean
        Get
            Return (State = S64.MixForTime) Or (State = S64.Sprinkle) Or (State = S64.WaitMixStart)
        End Get
    End Property
    Public ReadOnly Property IsReady() As Boolean
        Get
            Return (State = S64.Ready)
        End Get
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S64
    Public Property State() As S64          'Property	屬性名稱() As 傳回值型別
        Get                                 'Get
            Return state_                   '屬性名稱 = 私有資料成員        '讀取私有資料成員的值
        End Get                             'End Get
        Private Set(ByVal value As S64)     'Set(ByVal Value As傳回值型別)
            state_ = value                  '私有資料成員 = Value          '設定私有資料成員的值
        End Set                             'End Set
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S64
    Public Property StateWas() As S64
        Get
            Return statewas_
        End Get
        Private Set(ByVal value As S64)
            statewas_ = value
        End Set
    End Property

    'Property score() As Integer
    '    Get
    '        score = a           '讀取私有資料成員a的值
    '    End Get
    '
    '    Set(ByVal Value As Integer)
    '        a = Value           '設定私有資料成員a的值
    '    End Set
    'End Property

#End Region

End Class

#Region " Class Instance "

Partial Public Class ControlCode
  Public ReadOnly Command64 As New Command64(Me)
End Class

#End Region
