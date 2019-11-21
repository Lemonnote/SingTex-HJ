Public Class ManualDose
  Public Enum ManualDose
    Off
    Dose
    AddDelay
    Finish
    Pause
  End Enum

  Public Tank, DoseTime, Curve As Integer
  Public Timer As New Timer, LevelTimer As New Timer
  Public AddLevel, StartLevel As Integer
  Public DoseOutput As Integer
  Public AddType As Integer
  Public DoseON As Boolean
  Public StateString As String

  Public Sub Run()
    With ControlCode
      If Tank = 4 Then
        AddLevel = .IO.TankCLevel
      Else
        AddLevel = .IO.TankBLevel
      End If


      Select Case State
        Case ManualDose.Off
          If .IO.B1Add And Not .IO.SystemAuto Then
            Tank = 5
            DoseTime = .Parameters.ManualDoseTime1 * 60
            AddType = 1
          ElseIf .IO.B2Add And Not .IO.SystemAuto Then
            Tank = 5
            DoseTime = .Parameters.ManualDoseTime2 * 60
            AddType = 2
          ElseIf .IO.B3Add And Not .IO.SystemAuto Then
            Tank = 5
            DoseTime = .Parameters.ManualDoseTime3 * 60
            AddType = 3
          ElseIf .IO.B4Add And Not .IO.SystemAuto Then
            Tank = 5
            DoseTime = .Parameters.ManualDoseTime4 * 60
            AddType = 4
          ElseIf .IO.B5Add And Not .IO.SystemAuto Then
            Tank = 5
            DoseTime = .Parameters.ManualDoseTime5 * 60
            AddType = 5

          ElseIf .IO.C1Add And Not .IO.SystemAuto Then
            Tank = 4
            DoseTime = .Parameters.ManualDoseTime1 * 60
            AddType = 1
          ElseIf .IO.C2Add And Not .IO.SystemAuto Then
            Tank = 4
            DoseTime = .Parameters.ManualDoseTime2 * 60
            AddType = 2
          ElseIf .IO.C3Add And Not .IO.SystemAuto Then
            Tank = 4
            DoseTime = .Parameters.ManualDoseTime3 * 60
            AddType = 3
          ElseIf .IO.C4Add And Not .IO.SystemAuto Then
            Tank = 4
            DoseTime = .Parameters.ManualDoseTime4 * 60
            AddType = 4
          ElseIf .IO.C5Add And Not .IO.SystemAuto Then
            Tank = 4
            DoseTime = .Parameters.ManualDoseTime5 * 60
            AddType = 5
          Else
            Exit Select
          End If
          Curve = .Parameters.ManualDoseCurve

          ' Ok, so let's start dosing
          Timer.TimeRemaining = DoseTime
          ' Sample the start level
          If Tank = 4 Then
            StartLevel = .IO.TankCLevel
          Else
            StartLevel = .IO.TankBLevel
          End If
          State = ManualDose.Dose

        Case ManualDose.Dose
          If Tank = 4 Then
            StateString = If(.Language = LanguageValue.ZhTW, "C缸手動Dosing中", "Tank C manual dosing ") & TimerString(Timer.TimeRemaining)
          Else
            StateString = If(.Language = LanguageValue.ZhTW, "B缸手動Dosing中", "Tank B manual dosing ") & TimerString(Timer.TimeRemaining)

          End If
          Static delay7 As New DelayTimer
          DoseOutput = Maximum((((AddLevel - SetPoint()) * 10) + 100), 1000)
          DoseON = delay7.Run((DoseOutput > 0), 2)
          If Timer.Finished Then
            DoseOutput = 1000
            State = ManualDose.AddDelay
            LevelTimer.TimeRemaining = .Parameters.AddFinishDelay

          End If
          If Not .IO.MainPumpFB Then
            Timer.Pause()
            State = ManualDose.Pause

          End If
          If Tank = 4 And ControlCode.CAddStop Then State = ManualDose.Finish
          If Tank = 5 And ControlCode.BAddStop Then State = ManualDose.Finish

        Case ManualDose.AddDelay
          If AddLevel > 50 Then LevelTimer.TimeRemaining = .Parameters.AddFinishDelay
          If LevelTimer.Finished Then
            State = ManualDose.Finish
          End If
        Case ManualDose.Finish
          If Tank = 4 Then
            .TankCReady = False
          Else
            .TankBReady = False
          End If
          DoseOutput = 0
          Tank = 0
          AddType = 0
          DoseON = False
          State = ManualDose.Off
          Cancel()
        Case ManualDose.Pause
          If Tank = 4 Then
            StateString = If(.Language = LanguageValue.ZhTW, "C缸手動Dosing暫停", "Tank C manual dosing paused") & TimerString(Timer.TimeRemaining)
          Else
            StateString = If(.Language = LanguageValue.ZhTW, "B缸手動Dosing暫停", "Tank B manual dosing paused") & TimerString(Timer.TimeRemaining)

          End If
          If .IO.MainPumpFB Then
            Timer.Restart()
            State = ManualDose.Dose
          End If

      End Select
    End With
  End Sub
  Public ReadOnly Property SetPoint() As Integer
    Get
      'If timer has finished, just return 0
      If Timer.Finished Then Return 0

      'Amount we should have transferred so far
      Dim elapsedTime = (DoseTime - Timer.TimeRemaining) / DoseTime
      Dim timeToGo = 1 - elapsedTime
      Dim linearTerm = elapsedTime
      Dim transferAmount = StartLevel * linearTerm

      'Calculate scaling factor (0-1) for progressive and digressive curves
      If Curve > 0 Then
        Dim scalingFactor = (10 - Curve) / 10
        'Calculate term for progressive transfer (0-1) if odd curve
        If (Curve Mod 2) = 1 Then
          Dim maxOddCurve = 1 - Math.Sqrt(1 - (elapsedTime * elapsedTime * elapsedTime))
          Dim oddTerm = (((9 - Curve) * elapsedTime) + ((Curve + 1) * maxOddCurve)) / 10
          transferAmount = StartLevel * oddTerm
        Else
          'Calculate term for digressive transfer (0-1) if even curve
          Dim maxEvenCurve = 1 - Math.Sqrt(1 - (timeToGo * timeToGo * timeToGo))
          Dim evenTerm = (((10 - Curve) * timeToGo) + (Curve * maxEvenCurve)) / 10
          transferAmount = StartLevel * (1 - evenTerm)
        End If
      End If

      'Calculate and limit to 0-1000
      Return Math.Min(Math.Max(0, StartLevel - CType(transferAmount, Integer)), 1000)
    End Get
  End Property

#Region "Standard Definitions"

  Public Sub Cancel()
    With ControlCode
      State = ManualDose.Off
      Tank = 0
      AddType = 0
      LevelTimer.Cancel()

    End With
  End Sub
  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As ManualDose
  Public Property State() As ManualDose
    Get
      Return state_
    End Get
    Private Set(ByVal value As ManualDose)
      state_ = value
    End Set
  End Property
  Public ReadOnly Wait As New Timer
  
  'this is for the dosing valve
  Public ReadOnly Property IsDosing() As Boolean
    Get
      Return ((State = ManualDose.Dose) And DoseON) Or (State = ManualDose.AddDelay)
    End Get
  End Property
  Public ReadOnly Property IsOn() As Boolean
    Get
      Return (State <> ManualDose.Off)
    End Get
  End Property
  Public ReadOnly Property IsTransfer() As Boolean
    Get
      Return (State = ManualDose.AddDelay)
    End Get
  End Property
  Public ReadOnly Property IsTransferPump() As Boolean
    Get
      Return (State = ManualDose.Dose) Or (State = ManualDose.AddDelay)
    End Get
  End Property

#End Region
End Class

Partial Class ControlCode
  Public ReadOnly ManualDose As New ManualDose(Me)
End Class
