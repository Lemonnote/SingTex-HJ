<Command("Dilute Add C", "Time |0-60|", , , "'1", CommandType.Standard), _
 TranslateCommand("zh-TW", "C缸稀釋加藥", "加藥時間 |0-60|"), _
 Description("MAX=60,0=OPERTOR CONTROL"), _
 TranslateDescription("zh-TW", "最高=60分,0=操作員控制")> _
Public NotInheritable Class Command52
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S52
    Off
    WaitNoAddButtons
    WaitTempSafe
    WaitSystemAuto
    WaitMiddleLevel
    WaitCallOperator
    WaitTankReady
    WaitAddTimeHold
    WaitNotTankLowLevel
    Pause
  End Enum

  Public Tank, DiluteAddTime, DiluteTimeWas As Integer
  Public Wait As New Timer, RunBackWait As New Timer
  Public StateString As String

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      'cancels for all other forground functions
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .Command51.Cancel() : .Command10.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .Command01.Cancel()

      If param(1) > 0 And param(1) < 61 Then
        DiluteAddTime = param(1) * 60
      Else
        DiluteAddTime = 0
      End If

      State = S52.WaitSystemAuto

    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      ' Run this command
      Select Case State
        Case S52.Off
          StateString = ""

          'make sure we are not using the tank or the other tank
        Case S52.WaitNoAddButtons
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S52.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "等待藥缸", "Tank C Interlocked")
          'need to look at this
          If .IO.B1Add Or .IO.B2Add Or .IO.B3Add Or .IO.B4Add Or .IO.B5Add Or _
             .IO.C1Add Or .IO.C2Add Or .IO.C3Add Or .IO.C4Add Or .IO.C5Add Or .Command51.IsActive Or _
                .Command56.IsActive Or .Command57.IsActive Then Exit Select
          State = S52.WaitTempSafe

          'check that the temp is ok
        Case S52.WaitTempSafe
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S52.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "溫度異常", "Interlocked Temperature")
          If .IO.MainTemperature >= .Parameters.SetAddSafetyTemp * 10 Then
            .Alarms.HighTempNoAdd = True
            Exit Select
          End If
          .Alarms.HighTempNoAdd = False

          State = S52.WaitSystemAuto

          'check that we are in auto
        Case S52.WaitSystemAuto
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S52.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "Interlocked not In auto")
          If Not .IO.SystemAuto Then Exit Select
          .MessageSTankDiluteAddingNow = True
          State = S52.WaitMiddleLevel

          'fill to hi level
        Case S52.WaitMiddleLevel
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S52.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "C藥缸至中水位", "Filling Tank C to middle level")
          If Not .CTankMiddleLevel Then Exit Select
          Wait.TimeRemaining = 3
          State = S52.WaitCallOperator

          'mix the tank and wait for ready
        Case S52.WaitCallOperator
          If .Parent.IsPaused Then
            Wait.Pause()
            StateWas = State
            State = S52.Pause
          End If
          If Not Wait.Finished Then Exit Select
          State = S52.WaitTankReady

        Case S52.WaitTankReady
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S52.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "準備備藥", "Prepare Tank C")
          If .TankCReady Then
            If DiluteAddTime > 0 Then
              Wait.TimeRemaining = DiluteAddTime
              State = S52.WaitAddTimeHold
            Else
              State = S52.WaitNotTankLowLevel
            End If
          End If

          'if there is a time recirculate the tank to the machine for that time
        Case S52.WaitAddTimeHold
          StateString = If(.Language = LanguageValue.ZhTW, "C缸稀釋加藥中", "Diluting C ") & TimerString(Wait.TimeRemaining)
          'check to see if we have circulated the tank for the total mix time
          If Wait.Finished Then
            State = S52.WaitNotTankLowLevel
          End If

          'pause if halted or pump not runnng
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S52.Pause
          End If


          'ok recirculate time is done transfer it to the machine
        Case S52.WaitNotTankLowLevel
          StateString = If(.Language = LanguageValue.ZhTW, "C缸加藥中 ", "Transferring C ") & TimerString(Wait.TimeRemaining)
          If .CTankLowLevel Then Wait.TimeRemaining = .Parameters.AddTransferTimeBeforeRinse
          If Wait.Finished Then
            .MessageSTankDiluteAddingNow = False
            State = S52.Off
            .TankCReady = False 'set the ready to false
          End If

        Case S52.Pause
          StateString = If(.Language = LanguageValue.ZhTW, "暫停 ", "Paused ") & " " & TimerString(Wait.TimeRemaining)
          'no longer pause restart the timer and go back to the wait state
          If .Parent.CurrentStep <> .Parent.ChangingStep Then
            State = S52.Off
            Wait.Cancel()
          End If
          If (Not .Parent.IsPaused) And .IO.MainPumpFB And .IO.SystemAuto Then
            If StateWas = S52.WaitAddTimeHold Then
              If DiluteTimeWas = DiluteAddTime Then
                Wait.Restart()
              Else
                Wait.TimeRemaining = DiluteAddTime
              End If
            Else
              State = StateWas
              StateWas = S52.Off
              Wait.Restart()
            End If
          End If

      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S52.Off
    Wait.Cancel()
    RunBackWait.Cancel()
  End Sub

  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
    DiluteTimeWas = DiluteAddTime
    If param(1) > 0 And param(1) < 61 Then
      DiluteAddTime = param(1) * 60
    Else
      DiluteAddTime = 0
    End If
  End Sub


#Region "Standard Definitions"
  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S52.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S52
  Public Property State() As S52
    Get
      Return state_
    End Get
    Private Set(ByVal value As S52)
      state_ = value
    End Set
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S52
  Public Property StateWas() As S52
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As S52)
      statewas_ = value
    End Set
  End Property

  Public ReadOnly Property IsTankInterlocked() As Boolean
    Get
      Return ((State = S52.WaitNoAddButtons) Or (State = S52.WaitTempSafe) Or (State = S52.WaitSystemAuto))
    End Get
  End Property
  Public ReadOnly Property IsActive() As Boolean
    Get
      Return State > S52.WaitNoAddButtons
    End Get
  End Property
  Public ReadOnly Property IsFillingCirc() As Boolean
    Get
      Return (State = S52.WaitMiddleLevel)
    End Get
  End Property
  Public ReadOnly Property IsCallOperator() As Boolean
    Get
      Return ((State = S52.WaitCallOperator And DiluteAddTime = 0) Or (State = S52.WaitTankReady And DiluteAddTime > 0))
    End Get
  End Property
  Public ReadOnly Property IsWaitingForPrepare() As Boolean
    Get
      Return (State = S52.WaitTankReady)
    End Get
  End Property

  Public ReadOnly Property IsDilute() As Boolean
    Get
      Return ((State = S52.WaitTankReady) Or (State = S52.WaitAddTimeHold))
    End Get
  End Property
  Public ReadOnly Property IsTransfer() As Boolean
    Get
      Return ((State = S52.WaitAddTimeHold) Or (State = S52.WaitNotTankLowLevel))
    End Get
  End Property
  Public ReadOnly Property IsMixing() As Boolean
    Get
      Return ((State = S52.WaitTankReady) Or (State = S52.WaitAddTimeHold))
    End Get
  End Property
  Public ReadOnly Property IsPaused() As Boolean
    Get
      Return State = S52.Pause
    End Get
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command52 As New Command52(Me)
End Class
#End Region
