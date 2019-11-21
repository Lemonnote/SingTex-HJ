<Command("Parallel Add Dose B", "Time |0-60| Curve |0-9| Mix|0-1|", , , "'1+5", CommandType.ParallelCommand), _
 TranslateCommand("zh-TW", "平行B計量加藥", "加藥時間 |0-60| 曲線選擇 |0-9| 迴水檢測|0-1|"), _
Description("60=MAX 0=AllIn, Curve 0-9, 0=No 1=Yes"), _
TranslateDescription("zh-TW", "60分=最高 0=全加, 曲線=0-9, 0=不檢測 1=迴水檢測")> _
Public NotInheritable Class Command66
    Inherits MarshalByRefObject
    Implements ACCommand
    Public StateString As String

    Public Enum S66
        Off
        WaitAuto
        CheckSafetyTemp
        CheckReady
        Dose
        WaitAddFinish
        CallReCycleCheck
        ReCycleCheck
        ReCycleCheckEnd
        Rinse1
        Circulate
        Add
        Rinse2
        Drain
        Pause
    End Enum

    Public Timer As New Timer, LevelTimer As New Timer
    Public AddTime, AddCurve As Integer
    Public StartLevel As Integer
    Public DoseOutput As Integer
    Public RinseTimes As Integer
    Public DoseON As Boolean
    Public EnableReCycleCheck As Boolean
    Public StepWas As Integer
  Public WaitRunning As New Timer
  Public DosingError As Boolean

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
        With ControlCode
            If param(1) > 0 And param(1) < 61 Then
                AddTime = Maximum(param(1) * 60, 3600)
        WaitRunning.TimeRemaining = (.Parameters.WaitOverTime * 60) + AddTime
      Else
        AddTime = 0
        WaitRunning.TimeRemaining = .Parameters.WaitOverTime * 60
      End If
            AddCurve = param(2)
            EnableReCycleCheck = (param(3) = 1)
            RinseTimes = 2
            .DyeCallOff = 0
            .DyeTank = 0
            .Command56.Cancel()
            .Command54.Cancel()
            .Command64.Cancel()
            State = S66.WaitAuto
        End With
    End Function

    Public Function Run() As Boolean Implements ACCommand.Run
        With ControlCode


            Select Case State
                Case S66.Off
          DosingError = False
          StateString = ""

        Case S66.WaitAuto
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        Timer.Pause()
                        StateWas = State
                        State = S66.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "Interlocked not In auto")
                    If Not .IO.SystemAuto Then Exit Select
                    State = S66.CheckReady

                Case S66.CheckReady

                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        Timer.Pause()
                        StateWas = State
                        State = S66.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    If Not .TankBReady Then             '如果沒有備藥OK 將Alarms.STankNotReady ，有備藥OK跳下一個
            .Alarms.BTankNotReady = True
          End If
          If .TankBReady And (Not (.Command54.IsOn Or .Command64.IsOn Or .Command51.IsActive Or
                                             .Command52.IsActive Or .Command57.IsActive)) Then     '如果有備藥OK及其他沒用到藥缸的
            .Alarms.BTankNotReady = False                   '關閉 呼叫未備藥
            Timer.TimeRemaining = AddTime                   '將（加藥時間AddTime） 放到 （Timer.TimeRemaining）
            StartLevel = .IO.TankBLevel                     '將（藥缸水位.IO.TankBLevel） 放到 （StartLevel）
            State = S66.Dose
          End If

          'state string stuff.
          If Not .TankBReady Then                             '如果沒備藥OK，將顯示"Tank B not prepared"，有備藥跳步驟
                        StateString = If(.Language = LanguageValue.ZhTW, "B缸未備藥", "Tank B not prepared")
                    ElseIf .Command54.IsOn Or .Command64.IsOn Then      '如果B缸備藥有使用，顯示"Waiting for Tank B"，不然跳步驟
                        StateString = If(.Language = LanguageValue.ZhTW, "等待B缸備藥中", "Waiting for Tank B")
                    ElseIf .Command51.IsActive Then                     '如果B稀釋加藥有使用，顯示"Waiting for tank B to dilute"，不然就跳步驟
                        StateString = If(.Language = LanguageValue.ZhTW, "等待B缸稀釋加藥中", "Waiting for tank B to dilute")
                    ElseIf .Command52.IsActive Or .Command57.IsActive Then ''如果C稀釋加藥有使用，顯示"Waiting for tank B to dilute"，不然就跳步驟
                        StateString = If(.Language = LanguageValue.ZhTW, "等待C缸動作", "Waiting for Tank C")
                    Else
            StateString = If(.Language = LanguageValue.ZhTW, "按下B缸備藥OK開始加藥", "Press Tank B Ready to Start Dosing")
          End If

                Case S66.Dose
          .Alarms.BTankNotReady = False
          StateString = If(.Language = LanguageValue.ZhTW, "B藥缸計量加藥 ", "Tank B dosing ") & TimerString(Timer.TimeRemaining)
          Static delay10 As New DelayTimer
          DoseOutput = CShort(CType(Maximum((((.IO.TankBLevel - SetPoint()) * 10) + 100), 1000), Double) / 50) * 50
          DoseON = delay10.Run((DoseOutput > 0), 2)
          If Timer.Finished Then
            State = S66.WaitAddFinish
          End If
          If .IO.TankBLevel - SetPoint() > 200 And AddTime >= 60 Then
            DosingError = True
            DoseOutput = 0
          End If
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Timer.Pause()
            StateWas = State
            State = S66.Pause
            StepWas = .Parent.CurrentStep
          End If

        Case S66.WaitAddFinish
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        Timer.Pause()
                        StateWas = State
                        State = S66.Pause
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "B缸加藥延遲", "Tank B transferring ") & TimerString(Timer.TimeRemaining)
                    If .BTankLowLevel Then Timer.TimeRemaining = .Parameters.AddTransferTimeBeforeRinse
                    If Timer.Finished Then
                        DoseOutput = 0
                        .TankBReady = False
                        If EnableReCycleCheck Then
                            State = S66.CallReCycleCheck
                            Timer.TimeRemaining = 3
                        Else
                            State = S66.Rinse1
                            Timer.TimeRemaining = .Parameters.AddTransferRinseTime
                        End If
                    End If

                Case S66.CallReCycleCheck
                    StateString = ""
                    If Not Timer.Finished Then Exit Select
                    State = S66.ReCycleCheck

                Case S66.ReCycleCheck
                    If .Parent.IsPaused Or Not .IO.SystemAuto Then
                        Timer.Pause()
                        StateWas = State
                        State = S66.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "迴水檢測中", "Circulating for check")
                    If Not .IO.BTankReady Then Exit Select
                    State = S66.ReCycleCheckEnd
                    Timer.TimeRemaining = .Parameters.AddTransferTimeBeforeRinse

                Case S66.ReCycleCheckEnd
                    If .Parent.IsPaused Then
                        Timer.Pause()
                        StateWas = State
                        State = S66.Pause
                        StepWas = .Parent.CurrentStep
                    End If
                    StateString = If(.Language = LanguageValue.ZhTW, "B缸加藥延遲", "Tank B transferring ") & TimerString(Timer.TimeRemaining)
                    If .BTankLowLevel Then Timer.TimeRemaining = .Parameters.AddTransferTimeBeforeRinse
                    If Timer.Finished Then
                        .TankBReady = False
                        State = S66.Rinse1
                        Timer.TimeRemaining = .Parameters.AddTransferRinseTime
                    End If

                Case S66.Rinse1
                    StateString = If(.Language = LanguageValue.ZhTW, "B缸洗缸中", "Tank B rinsing ") & TimerString(Timer.TimeRemaining)
                    If Timer.Finished Then
                        State = S66.Add
                    End If

                Case S66.Add
                    StateString = If(.Language = LanguageValue.ZhTW, "B缸加藥中", "Tank B transferring ") & TimerString(Timer.TimeRemaining)
                    If .BTankLowLevel Then Timer.TimeRemaining = .Parameters.AddTransferTimeAfterRinse
                    If Timer.Finished Then
                        Timer.TimeRemaining = .Parameters.MixCirculateRinseTime
                        State = S66.Rinse2
                    End If

                Case S66.Rinse2
                    StateString = If(.Language = LanguageValue.ZhTW, "B缸洗缸中", "Tank B rinsing ") & TimerString(Timer.TimeRemaining)
                    If Timer.Finished Then
                        RinseTimes = RinseTimes - 1
                        If RinseTimes = 1 Then
                            Timer.TimeRemaining = .Parameters.MixCirculateTimeAfterRinse
                            State = S66.Circulate
                        Else
                            State = S66.Drain
                        End If
                    End If

                Case S66.Circulate
                    StateString = If(.Language = LanguageValue.ZhTW, "B缸循環中", "Tank B circulating ") & TimerString(Timer.TimeRemaining)
                    If Timer.Finished Then
                        State = S66.Add
                    End If

                Case S66.Drain
                    StateString = If(.Language = LanguageValue.ZhTW, "B缸排水", "Tank B draining ") & TimerString(Timer.TimeRemaining)
                    If .BTankLowLevel Then Timer.TimeRemaining = .Parameters.AddTransferDrainTime
                    If Timer.Finished Then
                        .TankBReady = False
                        State = S66.Off
            WaitRunning.Cancel()
          End If

                Case S66.Pause
          If DosingError Then
            StateString = If(.Language = LanguageValue.ZhTW, "Dosing異常，暫停 ", "Dosing error, Paused ")
          Else
            StateString = If(.Language = LanguageValue.ZhTW, "暫停 ", "Paused ")
          End If

          'no longer pause restart the timer and go back to the wait state
          If DosingError And Not .IO.CallAck Then Exit Select
          If (Not .Parent.IsPaused) And .IO.MainPumpFB And .IO.SystemAuto Then
            If DosingError Then
              State = S66.CheckReady
              StateWas = S66.Off
              Timer.Cancel()
              DosingError = False
            ElseIf StepWas = .Parent.CurrentStep Then
              State = StateWas
              StateWas = S66.Off
              Timer.Restart()
            Else
              State = S66.Off
              Timer.Cancel()
              WaitRunning.Cancel()
            End If
          End If

      End Select
        End With
    End Function

    Public Sub Cancel() Implements ACCommand.Cancel
        With ControlCode
            State = S66.Off
            LevelTimer.Cancel()
      WaitRunning.Cancel()
      DosingError = False
      .Alarms.BTankNotReady = False

    End With
    End Sub

    Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged

    End Sub



    '=100*(1-(SQRT(1-(A5*A5*A5))))
    '<GraphTrace(0, 1000, 0, 10000, , )> _
    Public ReadOnly Property SetPoint() As Integer
        Get
            'If timer has finished, just return 0
            If Timer.Finished Then Return 0

            'Amount we should have transferred so far
            Dim elapsedTime = (AddTime - Timer.TimeRemaining) / AddTime
            Dim timeToGo = 1 - elapsedTime
            Dim linearTerm = elapsedTime
            Dim transferAmount = StartLevel * linearTerm

            'Calculate scaling factor (0-1) for progressive and digressive curves
            If AddCurve > 0 Then
                Dim scalingFactor = (10 - AddCurve) / 10
                'Calculate term for progressive transfer (0-1) if odd curve
                If (AddCurve Mod 2) = 1 Then
                    Dim maxOddCurve = 1 - Math.Sqrt(1 - (elapsedTime * elapsedTime * elapsedTime))
                    Dim oddTerm = (((9 - AddCurve) * elapsedTime) + ((AddCurve + 1) * maxOddCurve)) / 10
                    transferAmount = StartLevel * oddTerm
                Else
                    'Calculate term for digressive transfer (0-1) if even curve
                    Dim maxEvenCurve = 1 - Math.Sqrt(1 - (timeToGo * timeToGo * timeToGo))
                    Dim evenTerm = (((10 - AddCurve) * timeToGo) + (AddCurve * maxEvenCurve)) / 10
                    transferAmount = StartLevel * (1 - evenTerm)
                End If
            End If

            'Calculate and limit to 0-1000
            Return Math.Min(Math.Max(0, StartLevel - CType(transferAmount, Integer)), 1000)
        End Get
    End Property

#Region "Standard Definitions"
    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
        Get
            Return State <> S66.Off
        End Get
    End Property
    Public ReadOnly Property IsActive() As Boolean
        Get
            Return State > S66.CheckReady
        End Get
    End Property
    Public ReadOnly Property IsWaitingForPrepare() As Boolean
        Get
            Return (State = S66.CheckReady) Or (State = S66.CallReCycleCheck)
        End Get
    End Property
    Public ReadOnly Property IsCallReCycleCheckEnd() As Boolean
        Get
            Return (State = S66.ReCycleCheck)
        End Get
    End Property
    Public ReadOnly Property IsNotReady() As Boolean
        Get
            Return (State = S66.CheckReady)
        End Get
    End Property
    'this is for the dosing valve
    Public ReadOnly Property IsDosing() As Boolean
        Get
            Return ((State = S66.Dose) And DoseON) Or (State = S66.WaitAddFinish) Or (State = S66.Rinse1) Or (State = S66.Add)
        End Get
    End Property
    Public ReadOnly Property IsTransfer() As Boolean
        Get
            Return (State = S66.WaitAddFinish) Or (State = S66.Rinse1) Or (State = S66.Add) Or (State = S66.ReCycleCheckEnd)
        End Get
    End Property
    Public ReadOnly Property IsTransferPump() As Boolean
        Get
            Return (State = S66.Dose) Or (State = S66.WaitAddFinish) Or (State = S66.Rinse1) Or (State = S66.Add) Or _
            (State = S66.Circulate) Or (State = S66.ReCycleCheckEnd)
        End Get
    End Property
    Public ReadOnly Property IsRinsing() As Boolean
        Get
            Return ((State = S66.Rinse1) Or (State = S66.Rinse2))
        End Get
    End Property
    Public ReadOnly Property IsFillCirc() As Boolean
        Get
            Return (State = S66.CallReCycleCheck) Or (State = S66.ReCycleCheck)
        End Get
    End Property
    Public ReadOnly Property IsMixing() As Boolean
        Get
            Return (State = S66.Circulate)
        End Get
    End Property
    Public ReadOnly Property IsDraining() As Boolean
        Get
            Return (State = S66.Drain)
        End Get
    End Property
    Public ReadOnly Property IsPaused() As Boolean
        Get
            Return State = S66.Pause
        End Get
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S66
    Public Property State() As S66
        Get
            Return state_
        End Get
        Private Set(ByVal value As S66)
            state_ = value
        End Set
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S66
    Public Property StateWas() As S66
        Get
            Return statewas_
        End Get
        Private Set(ByVal value As S66)
            statewas_ = value
        End Set
    End Property


#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command66 As New Command66(Me)
End Class
#End Region
