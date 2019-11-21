<Command("Add Dose C", "Time |0-60| Curve |0-9| CirCheck |0-1|", , , "'1+5", CommandType.Standard), _
 TranslateCommand("zh-TW", "C計量加藥", "加藥時間 |0-60| 曲線選擇 |0-9| 迴水檢測|0-1|"), _
Description("60=MAX 0=AllIn, Curve 0-9, 0=No 1=Yes"), _
TranslateDescription("zh-TW", "60分=最高 0=全加, 曲線=0-9, 0=不檢測 1=迴水檢測")> _
Public NotInheritable Class Command57
    Inherits MarshalByRefObject
    Implements ACCommand
    Public StateString As String

    Public Enum S57
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
        Add
        Rinse2
        Drain
        Pause
    End Enum

    Public Timer As New Timer, LevelTimer As New Timer
    Public AddTime, AddCurve As Integer
    Public StartLevel As Integer
    Public DoseOutput As Integer
    Public DoseON As Boolean
    Public EnableReCycleCheck As Boolean
  Public WaitRunning As New Timer
  Public StepWas As Integer
  Public CTankPrepareRunning As Boolean
  Public DosingError As Boolean

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
        With ControlCode
            'cancels for all other forground functions
            .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
            .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
            .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
            .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
            .Command55.Cancel() : .Command56.Cancel() : .Command10.Cancel() : .Command58.Cancel()
            .Command59.Cancel() : .Command61.Cancel() : .Command65.Cancel() : .Command67.Cancel()
            .Command17.Cancel() : .Command18.Cancel() : .Command01.Cancel()
            .TemperatureControl.Cancel()
            .TempControlFlag = False
      ' .ChemicalCallOff = 0
      ' .ChemicalTank = 0

      If param(1) > 0 And param(1) < 61 Then
                AddTime = Maximum(param(1) * 60, 3600)
        WaitRunning.TimeRemaining = (.Parameters.WaitOverTime * 60) + AddTime
      Else
        AddTime = 0
        WaitRunning.TimeRemaining = .Parameters.WaitOverTime * 60
      End If
            AddCurve = param(2)
            EnableReCycleCheck = (param(3) = 1)
      CTankPrepareRunning = False
      DosingError = False
      State = S57.WaitAuto
    End With
    End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode


      Select Case State
        Case S57.Off
          StateString = ""

        Case S57.WaitAuto
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Timer.Pause()
            StateWas = State
            State = S57.Pause
            StepWas = .Parent.CurrentStep
          End If
          If Not .IO.SystemAuto Then
            StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "Interlocked not In auto")
            Exit Select
          End If
          If .RunCTankPrepare Then
            CTankPrepareRunning = True
            StateString = If(.Language = LanguageValue.ZhTW, "等待C缸備藥中", "Waiting for tank C")
            Exit Select
          End If
          State = S57.CheckReady


        Case S57.CheckReady

          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Timer.Pause()
            StateWas = State
            State = S57.Pause
            StepWas = .Parent.CurrentStep
          End If

          If .TankCReady And (Not (.Command55.IsOn Or .Command65.IsOn Or .Command51.IsActive Or
                                             .Command52.IsActive Or .Command56.IsActive Or .RunCTankPrepare)) Then
            .Alarms.CTankNotReady = False
            Timer.TimeRemaining = AddTime
            CTankPrepareRunning = False
            .CTankPrepare.Cancel()
            .ChemicalCallOff = 0
            .ChemicalTank = 0

            .RunCTankPrepare = False
            StartLevel = .IO.TankCLevel
            State = S57.Dose
          End If

          'state string stuff.
          If .Command55.IsOn Or .Command65.IsOn Then
            StateString = If(.Language = LanguageValue.ZhTW, "等待C缸備藥中", "Waiting for tank C")
          ElseIf .Command52.IsActive Then
            StateString = If(.Language = LanguageValue.ZhTW, "等待C缸稀釋加藥中", "Waiting for tank C to dilute")
          ElseIf .Command51.IsActive Or .Command56.IsActive Then
            StateString = If(.Language = LanguageValue.ZhTW, "等待B缸動作", "Waiting for Tank B")
          ElseIf Not .TankCReady Then
            StateString = If(.Language = LanguageValue.ZhTW, "C缸未備藥", "Tank C not prepared")
            .Alarms.CTankNotReady = True
          Else
            StateString = If(.Language = LanguageValue.ZhTW, "按下C缸備藥OK開始加藥", "Press Tank C Ready to Start Dosing")
          End If

        Case S57.Dose
          .Alarms.CTankNotReady = False
          StateString = If(.Language = LanguageValue.ZhTW, "C缸計量加藥中 ", "Tank C dosing ") & TimerString(Timer.TimeRemaining)
          Static delay10 As New DelayTimer
          DoseOutput = CShort(CType(Maximum((((.IO.TankCLevel - SetPoint()) * 10) + 100), 1000), Double) / 50) * 50
          DoseON = delay10.Run((DoseOutput > 0), 2)
          If Timer.Finished Then
            State = S57.WaitAddFinish
          End If
          If .IO.TankCLevel - SetPoint() > 50 And AddTime >= 60 Then
            DosingError = True
            DoseOutput = 0
          End If
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Timer.Pause()
            StateWas = State
            State = S57.Pause
            StepWas = .Parent.CurrentStep
          End If

        Case S57.WaitAddFinish
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Timer.Pause()
            StateWas = State
            State = S57.Pause
            StepWas = .Parent.CurrentStep
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "等待C缸加藥延遲", "Tank C transferring ") & TimerString(Timer.TimeRemaining)
          If .CTankLowLevel Then Timer.TimeRemaining = .Parameters.AddTransferTimeBeforeRinse
          If Timer.Finished Then
            DoseOutput = 0
            .TankCReady = False
            If EnableReCycleCheck Then
              State = S57.CallReCycleCheck
              Timer.TimeRemaining = 3
            Else
              State = S57.Rinse1
              Timer.TimeRemaining = .Parameters.AddTransferRinseTime
            End If
          End If

        Case S57.CallReCycleCheck
          StateString = ""
          If Not Timer.Finished Then Exit Select
          State = S57.ReCycleCheck

        Case S57.ReCycleCheck
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Timer.Pause()
            StateWas = State
            State = S57.Pause
            StepWas = .Parent.CurrentStep
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "迴水檢測中", "Circulating for check")
          If Not .IO.CTankReady Then Exit Select
          State = S57.ReCycleCheckEnd
          Timer.TimeRemaining = .Parameters.AddTransferTimeBeforeRinse

        Case S57.ReCycleCheckEnd
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Timer.Pause()
            StateWas = State
            State = S57.Pause
            StepWas = .Parent.CurrentStep
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "C缸加藥延遲", "Tank C transferring ") & TimerString(Timer.TimeRemaining)
          If .CTankLowLevel Then Timer.TimeRemaining = .Parameters.AddTransferTimeBeforeRinse
          If Timer.Finished Then
            .TankCReady = False
            State = S57.Rinse1
            Timer.TimeRemaining = .Parameters.AddTransferRinseTime
          End If

        Case S57.Rinse1
          StateString = If(.Language = LanguageValue.ZhTW, "C缸洗缸中", "Tank C rinsing ") & TimerString(Timer.TimeRemaining)
          If Timer.Finished Then
            Timer.TimeRemaining = .Parameters.AddTransferTimeAfterRinse
            State = S57.Add
          End If

        Case S57.Add
          StateString = If(.Language = LanguageValue.ZhTW, "C缸加藥中", "Tank C transferring ") & TimerString(Timer.TimeRemaining)
          If .CTankLowLevel Then Timer.TimeRemaining = .Parameters.AddTransferTimeAfterRinse
          If Timer.Finished Then
            Timer.TimeRemaining = .Parameters.AddTransferRinseTime
            State = S57.Rinse2
          End If

        Case S57.Rinse2
          StateString = If(.Language = LanguageValue.ZhTW, "C缸洗缸中", "Tank C rinsing ") & TimerString(Timer.TimeRemaining)
          If Timer.Finished Then
            Timer.TimeRemaining = .Parameters.AddTransferDrainTime
            State = S57.Drain
          End If

        Case S57.Drain
          StateString = If(.Language = LanguageValue.ZhTW, "C缸排水中", "Tank C draining ") & TimerString(Timer.TimeRemaining)
          If .CTankLowLevel Then Timer.TimeRemaining = .Parameters.AddTransferDrainTime
          If Timer.Finished Then
            .TankCReady = False
            State = S57.Off
            WaitRunning.Cancel()
          End If

        Case S57.Pause
          StateString = If(.Language = LanguageValue.ZhTW, "暫停 ", "Paused ") & " " & TimerString(Timer.TimeRemaining)
          'no longer pause restart the timer and go back to the wait state
          If (Not .Parent.IsPaused) And .IO.MainPumpFB And .IO.SystemAuto Then
            If .Parent.CurrentStep <> StepWas Then
              State = S57.Off
              Timer.Cancel()
              WaitRunning.Cancel()
            Else
              State = StateWas
              StateWas = S57.Off
              Timer.Restart()
            End If
          End If

      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
        With ControlCode
            State = S57.Off
            LevelTimer.Cancel()
      WaitRunning.Cancel()
      DosingError = False
      .Alarms.CTankNotReady = False

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
            Return State <> S57.Off
        End Get
    End Property
    Public ReadOnly Property IsActive() As Boolean
        Get
            Return State > S57.CheckReady
        End Get
    End Property
    Public ReadOnly Property IsWaitingForPrepare() As Boolean
        Get
      Return (State = S57.CheckReady And Not CTankPrepareRunning) Or (State = S57.CallReCycleCheck)
    End Get
    End Property
    Public ReadOnly Property IsCallReCycleCheckEnd() As Boolean
        Get
            Return (State = S57.ReCycleCheck)
        End Get
    End Property
    Public ReadOnly Property IsNotReady() As Boolean
        Get
      Return (State = S57.CheckReady) And Not CTankPrepareRunning
    End Get
    End Property
    'this is for the dosing valve
    Public ReadOnly Property IsDosing() As Boolean
        Get
            Return ((State = S57.Dose) And DoseON) Or (State = S57.WaitAddFinish) Or (State = S57.Rinse1) Or (State = S57.Add)
        End Get
    End Property
    Public ReadOnly Property IsTransfer() As Boolean
        Get
      Return ((State = S57.Dose) And DoseON And DosingError) Or (State = S57.WaitAddFinish) Or (State = S57.Rinse1) Or (State = S57.Add) Or (State = S57.ReCycleCheckEnd)
    End Get
    End Property
    Public ReadOnly Property IsTransferPump() As Boolean
        Get
            Return (State = S57.Dose) Or (State = S57.WaitAddFinish) Or (State = S57.Rinse1) Or (State = S57.Add) Or (State = S57.ReCycleCheckEnd)
        End Get
    End Property
    Public ReadOnly Property IsRinsing() As Boolean
        Get
            Return ((State = S57.Rinse1) Or (State = S57.Rinse2))
        End Get
    End Property
    Public ReadOnly Property IsFillCirc() As Boolean
        Get
            Return (State = S57.CallReCycleCheck) Or (State = S57.ReCycleCheck)
        End Get
    End Property
    Public ReadOnly Property IsDraining() As Boolean
        Get
            Return ((State = S57.Rinse2) Or (State = S57.Drain))
        End Get
    End Property
    Public ReadOnly Property IsPaused() As Boolean
        Get
            Return State = S57.Pause
        End Get
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S57
    Public Property State() As S57
        Get
            Return state_
        End Get
        Private Set(ByVal value As S57)
            state_ = value
        End Set
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S57
    Public Property StateWas() As S57
        Get
            Return statewas_
        End Get
        Private Set(ByVal value As S57)
            statewas_ = value
        End Set
    End Property


#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command57 As New Command57(Me)
End Class
#End Region
