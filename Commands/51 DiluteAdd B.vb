<Command("Dilute Add B", "Time |0-60|", , , "'2", CommandType.Standard), _
 TranslateCommand("zh-TW", "B缸稀釋加藥", "加藥時間 |0-60|"), _
 Description("MAX=60,0=OPERTOR CONTROL"), _
 TranslateDescription("zh-TW", "最高=60分,0=操作員控制")> _
Public NotInheritable Class Command51
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S51
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
      .Command33.Cancel() : .Command10.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .Command01.Cancel()
      If param(1) > 0 And param(1) < 61 Then
        DiluteAddTime = param(1) * 60
      Else
        DiluteAddTime = 0
      End If

      State = S51.WaitNoAddButtons

    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      ' Run this command
      Select Case State
        Case S51.Off
          StateString = ""

          'make sure we are not using the tank or the other tank
        Case S51.WaitNoAddButtons
          StateString = If(.Language = LanguageValue.ZhTW, "等待藥缸", "Tank B Interlocked")
          'need to look at this
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S51.Pause
          End If
          If .IO.B1Add Or .IO.B2Add Or .IO.B3Add Or .IO.B4Add Or .IO.B5Add Or _
             .IO.C1Add Or .IO.C2Add Or .IO.C3Add Or .IO.C4Add Or .IO.C5Add Or _
             .Command52.IsActive Or .Command56.IsActive Or .Command57.IsActive Then Exit Select
          State = S51.WaitTempSafe

          'check that the temp is ok
        Case S51.WaitTempSafe
          StateString = If(.Language = LanguageValue.ZhTW, "溫度異常", "Interlocked Temperature")
          If .IO.MainTemperature >= .Parameters.SetAddSafetyTemp * 10 Then
            .Alarms.HighTempNoAdd = True
            Exit Select
          End If
          .Alarms.HighTempNoAdd = False

          State = S51.WaitSystemAuto

          'check that we are in auto
        Case S51.WaitSystemAuto
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S51.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "Interlocked not In auto")
          If Not .IO.SystemAuto Then Exit Select

          .MessageSTankDiluteAddingNow = True
          State = S51.WaitMiddleLevel

          'fill to middle level
        Case S51.WaitMiddleLevel
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S51.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "等待藥缸中水位", "Filling Tank B to middle level")
          If Not .BTankMiddleLevel Then Exit Select
          Wait.TimeRemaining = 3
          State = S51.WaitCallOperator

        Case S51.WaitCallOperator
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S51.Pause
          End If
          If Not Wait.Finished Then Exit Select
          State = S51.WaitTankReady


          'mix the tank and wait for ready
        Case S51.WaitTankReady
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S51.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "準備備藥", "Prepare Tank B")
          If .TankBReady Then
            If DiluteAddTime > 0 Then
              Wait.TimeRemaining = DiluteAddTime
              State = S51.WaitAddTimeHold
            Else
              State = S51.WaitNotTankLowLevel
            End If
          End If

          'if there is a time recirculate the tank to the machine for that time
        Case S51.WaitAddTimeHold
          StateString = If(.Language = LanguageValue.ZhTW, "B缸稀釋加藥中", "Diluting B ") & TimerString(Wait.TimeRemaining)
          'check to see if we have circulated the tank for the total mix time
          If Wait.Finished Then
            State = S51.WaitNotTankLowLevel
          End If

          'pause if halted or pump not runnng
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S51.Pause
          End If


          'ok recirculate time is done transfer it to the machine
        Case S51.WaitNotTankLowLevel
          StateString = If(.Language = LanguageValue.ZhTW, "B缸加藥中", "Transferring B ") & TimerString(Wait.TimeRemaining)
          If .BTankLowLevel Then Wait.TimeRemaining = .Parameters.AddTransferTimeBeforeRinse
          If Wait.Finished Then
            .MessageSTankDiluteAddingNow = False
            State = S51.Off
            .TankBReady = False 'set the ready to false
          End If
          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S51.Pause
          End If


        Case S51.Pause
          StateString = If(.Language = LanguageValue.ZhTW, "暫停 ", "Paused ") & " " & TimerString(Wait.TimeRemaining)
          'no longer pause restart the timer and go back to the wait state
          If .Parent.CurrentStep <> .Parent.ChangingStep Then
            Wait.Cancel()
            State = S51.Off
          End If
          If (Not .Parent.IsPaused) And .IO.MainPumpFB And .IO.SystemAuto Then
            If StateWas = S51.WaitAddTimeHold Then
              State = StateWas
              StateWas = S51.Off
              If DiluteTimeWas = DiluteAddTime Then
                Wait.Restart()
              Else
                Wait.TimeRemaining = DiluteAddTime
              End If
            Else
              State = StateWas
              StateWas = S51.Off
              Wait.Restart()
            End If
          End If


      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S51.Off
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
      Return State <> S51.Off
    End Get
  End Property
  Public ReadOnly Property IsActive() As Boolean
    Get
      Return State > S51.WaitNoAddButtons
    End Get
  End Property
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S51
  Public Property State() As S51
    Get
      Return state_
    End Get
    Private Set(ByVal value As S51)
      state_ = value
    End Set
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S51
  Public Property StateWas() As S51
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As S51)
      statewas_ = value
    End Set
  End Property

  Public ReadOnly Property IsTankInterlocked() As Boolean
    Get
      Return ((State = S51.WaitNoAddButtons) Or (State = S51.WaitTempSafe) Or (State = S51.WaitSystemAuto))
    End Get
  End Property
  Public ReadOnly Property IsFillingCirc() As Boolean
    Get
      Return (State = S51.WaitMiddleLevel)
    End Get
  End Property
  Public ReadOnly Property IsCallOperator() As Boolean
    Get
      Return ((State = S51.WaitCallOperator And DiluteAddTime = 0) Or (State = S51.WaitTankReady And DiluteAddTime > 0))
    End Get
  End Property

  Public ReadOnly Property IsWaitingForPrepare() As Boolean
    Get
      Return (State = S51.WaitTankReady)
    End Get
  End Property
  Public ReadOnly Property IsDilute() As Boolean
    Get
      Return ((State = S51.WaitTankReady) Or (State = S51.WaitAddTimeHold))
    End Get
  End Property
  Public ReadOnly Property IsTransfer() As Boolean
    Get
      Return ((State = S51.WaitAddTimeHold) Or (State = S51.WaitNotTankLowLevel))
    End Get
  End Property
  Public ReadOnly Property IsMixing() As Boolean
    Get
      Return ((State = S51.WaitTankReady) Or (State = S51.WaitAddTimeHold))
    End Get
  End Property
  Public ReadOnly Property IsPaused() As Boolean
    Get
      Return State = S51.Pause
    End Get
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command51 As New Command51(Me)
End Class
#End Region
