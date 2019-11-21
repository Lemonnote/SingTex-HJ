<Command("B Tank Prepare", "Mixer Time |0-999| Type |0-1| Call off |0-99| Qty% |0-100|", , , "('1/60)+5", CommandType.Standard), _
TranslateCommand("zh-TW", "B藥缸水量備藥", "攪拌時間 |0-999| 水源 |0-1| 呼叫 |0-99| 水量% |0-100|"), _
Description("999=MAX 0=UN MIX   1=COLD 0=CIRCULATE  0-99 Call off 100%=MAX 0%=MIN"), _
TranslateDescription("zh-TW", "999秒=最高 0秒=不攪拌   1=備清水 0=備回水  0-99 呼叫 100%=最高 0%=最小")> _
Public NotInheritable Class Command54
    Inherits MarshalByRefObject                       'Inheritsg是繼承Windows Form的應用程式要繼承System.Windows.Forms.Form，可先參考物件導向程式設計相關書籍
    Implements ACCommand

    Public Enum S54
        Off
        WaitAuto
        WaitNoAddButtons
        FillToLowLevel
        DispenseWaitReady
        ResetCallOff
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

    Public Time, Type, Qty, CallOff, TimeWas As Integer
    Public WaitTimer As New Timer
    Public SprinkleTimer As New Timer
  Public WaitRunning As New Timer

    Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
        With ControlCode
            'cancels for all other forground functions
            .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
            .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
            .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
            .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command10.Cancel()
            .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
            .Command59.Cancel() : .Command61.Cancel() : .Command64.Cancel() : .Command66.Cancel()
            .Command01.Cancel()
            .TemperatureControl.Cancel()
            .TempControlFlag = False
            .SPCConnectError = False
            .DyeCallOff = 0
            .DyeTank = 0
            .DyeState = 101
      WaitRunning.TimeRemaining = .Parameters.WaitOverTime * 60


            Time = Maximum(param(1), 999)
            Type = MinMax(param(2), 0, 1)
            CallOff = param(3)
            Qty = MinMax(param(4) * 10, 0, 1000)

      State = S54.WaitAuto
        End With
    End Function

    Public Function Run() As Boolean Implements ACCommand.Run
        With ControlCode
            Select Case State
                Case S54.Off
                    StateString = ""

                Case S54.WaitAuto
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        StateWas = State
                        State = S54.Pause
                        WaitTimer.Pause()
                    End If
          If Not .IO.SystemAuto Then
            StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "Interlocked not In auto")
            Exit Select
          End If
          If .IO.BDrainPB Then
            StateString = If(.Language = LanguageValue.ZhTW, "請關閉B缸排水開關", "Please Switch Off Tank B Drain")
            Exit Select
          End If

          State = S54.WaitNoAddButtons

        Case S54.WaitNoAddButtons
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        StateWas = State
                        State = S54.Pause
                        WaitTimer.Pause()
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "等待藥缸", "Tank B Interlocked")
                    If .IO.B1Add Or .IO.B2Add Or .IO.B3Add Or .IO.B4Add Or .IO.B5Add Then Exit Select
                    State = S54.FillToLowLevel
                    .MessageSTankFilling = True

                Case S54.FillToLowLevel
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        StateWas = State
                        State = S54.Pause
                        WaitTimer.Pause()
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "B藥缸進水至低水位", "Tank B Filling to  level")
                    If .BTankLowLevel Then
                        .MessageSTankFilling = False
                        .MessageSTankPrepare = True
                        State = S54.Slow
                        If CallOff > 0 And .Parameters.DyeEnable = 1 Then
                            .DyeCallOff = 0   'Starts the handshake with the host / auto dispenser
                            .DyeTank = 0
                            .TankBReady = False
                            State = S54.DispenseWaitReady
                        End If
                    End If

                Case S54.DispenseWaitReady
                    If .Parent.IsPaused Then
                        StateWas = State
                        State = S54.Pause
                        WaitTimer.Pause()
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "染料備藥中", "Prepare Tank B")
                    'TODO  Add timeout code to switch to manual if no response
                    If .DyeState = EDispenseState.Ready Then
                        'Dispenser is ready so set CallOff number and wait for result
                        .DyeCallOff = CallOff
                        .DyeTank = 1
                        State = S54.DispenseWaitResponse
                    End If
                    'Switch to manual if enable parameter is changed or calloff is reset
                    If .Parameters.DyeEnable <> 1 Then State = S54.Slow
                    If CallOff = 0 Then State = S54.Slow

                Case S54.DispenseWaitResponse
                    If .Parent.IsPaused Then
                        StateWas = State
                        State = S54.Pause
                        WaitTimer.Pause()
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "等待染料計量", "Wait for Dye Dispensing")
                    Select Case .DyeState
                        Case EDispenseState.Complete
                            'Everything completed ok so set ready flag and carry on
                            .DyeCallOff = 0
                            .DyeTank = 0
                            State = S54.Slow

                        Case EDispenseState.Manual
                            'Manual dispenses required so call the operator
                            .DyeCallOff = 0
                            .DyeTank = 0
                            State = S54.Slow

                        Case EDispenseState.Error
                            'Dispense error call the operator
                            .DyeCallOff = 0
                            .DyeTank = 0
                            State = S54.Slow
                    End Select
                    'Switch to manual if enable parameter is changed or calloff is reset
                    If .Parameters.DyeEnable <> 1 Then State = S54.Slow
                    If CallOff = 0 Then State = S54.Slow

                Case S54.Slow
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        StateWas = State
                        State = S54.Pause
                        WaitTimer.Pause()
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "備藥完成，請按備藥OK", "Prepare Tank B")
          .DyeCallOff = 0
                    .DyeTank = 0
                    If (CallOff > 0) And (.Parameters.BTankCallAck = 1) Then
                        .TankBReady = True
                    End If
                    If .TankBReady Then
                        If Qty > 5 Then
                            State = S54.FillQty
                        Else
                            State = S54.MixForTime
                        End If
                    End If

                Case S54.FillQty
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        StateWas = State
                        State = S54.Pause
                        WaitTimer.Pause()
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "B藥缸進水 ", "Filling Tank B to ") & Qty / 10 & "%"
                    If .IO.TankBLevel > Qty Then
                        If Time = 0 Or CallOff > 0 Then
                            State = S54.MixForTime
                            WaitTimer.TimeRemaining = Time
                        Else
                            State = S54.WaitMixStart
                            WaitTimer.TimeRemaining = 5
                        End If
                    End If

                Case S54.WaitMixStart
                    StateString = If(.Language = LanguageValue.ZhTW, "等待B藥缸攪拌啟動 ", "Wait Tank B start mix") & TimerString(WaitTimer.TimeRemaining)
                    If WaitTimer.Finished Then
                        State = S54.Sprinkle
                        WaitTimer.TimeRemaining = Time
                        SprinkleTimer.TimeRemaining = .Parameters.BTankSprinkleTimeWhenMixing
                    End If
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        StateWas = State
                        State = S54.Pause
                        WaitTimer.Pause()
                    End If

                Case S54.Sprinkle
                    StateString = If(.Language = LanguageValue.ZhTW, "B藥缸攪拌 ", "Tank B mixing for ") & TimerString(WaitTimer.TimeRemaining)
                    If SprinkleTimer.Finished Then
                        State = S54.MixForTime
                    End If
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        StateWas = State
                        State = S54.Pause
                        WaitTimer.Pause()
                    End If


                Case S54.MixForTime
                    StateString = If(.Language = LanguageValue.ZhTW, "B藥缸攪拌 ", "Tank B mixing for ") & TimerString(WaitTimer.TimeRemaining)
                    If WaitTimer.Finished Then
                        State = S54.WaitMixStop
                        WaitTimer.TimeRemaining = 10
                    End If
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        StateWas = State
                        State = S54.Pause
                        WaitTimer.Pause()
                    End If

                Case S54.WaitMixStop
                    StateString = If(.Language = LanguageValue.ZhTW, "等待B藥缸穩定 ", "Wait Tank B Stable ") & TimerString(WaitTimer.TimeRemaining)
                    If WaitTimer.Finished Then
                        State = S54.Ready
                    End If


                Case S54.Ready
                    State = S54.Off
          WaitRunning.Cancel()

                Case S54.Pause
                    StateString = If(.Language = LanguageValue.ZhTW, "暫停 ", "Paused ") & " " & TimerString(WaitTimer.TimeRemaining)
                    If .Parent.CurrentStep <> .Parent.ChangingStep Then
                        State = S54.Off
                        WaitTimer.Cancel()
            WaitRunning.Cancel()
          End If
                    If Not .Parent.IsPaused And .IO.SystemAuto Then
                        If StateWas = S54.MixForTime Then
                            State = StateWas
                            StateWas = S54.Off
                            If TimeWas = Time Then
                                WaitTimer.Restart()
                            Else
                                WaitTimer.TimeRemaining = Time
                            End If
                        Else
                            State = StateWas
                            StateWas = S54.Off
                            WaitTimer.Restart()
                        End If
                    End If
            End Select

        End With
    End Function

    Public Sub Cancel() Implements ACCommand.Cancel
        State = S54.Off
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
            Return State <> S54.Off
        End Get
    End Property
    Public ReadOnly Property IsTankInterlocked() As Boolean
        Get
            Return (State = S54.WaitNoAddButtons)
        End Get
    End Property
    Public ReadOnly Property IsFillingFresh() As Boolean
        Get
            Return ((Type = 1) And ((State = S54.FillQty) Or (State = S54.FillToLowLevel))) Or (State = S54.Sprinkle)
        End Get
    End Property
    Public ReadOnly Property IsFillingCirc() As Boolean
        Get
            Return (Type = 0) And ((State = S54.FillQty) Or (State = S54.FillToLowLevel))
        End Get
    End Property
    Public ReadOnly Property IsSlow() As Boolean
        Get
            Return (State = S54.Slow)
        End Get
    End Property
    Public ReadOnly Property IsMixingForTime() As Boolean
        Get
            Return (State = S54.MixForTime) Or (State = S54.Sprinkle) Or (State = S54.WaitMixStart)
        End Get
    End Property
    Public ReadOnly Property IsReady() As Boolean
        Get
            Return (State = S54.Ready)
        End Get
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S54
    Public Property State() As S54          'Property	屬性名稱() As 傳回值型別
        Get                                 'Get
            Return state_                   '屬性名稱 = 私有資料成員        '讀取私有資料成員的值
        End Get                             'End Get
        Private Set(ByVal value As S54)     'Set(ByVal Value As傳回值型別)
            state_ = value                  '私有資料成員 = Value          '設定私有資料成員的值
        End Set                             'End Set
    End Property

    <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S54
    Public Property StateWas() As S54
        Get
            Return statewas_
        End Get
        Private Set(ByVal value As S54)
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
  Public ReadOnly Command54 As New Command54(Me)
End Class

#End Region
