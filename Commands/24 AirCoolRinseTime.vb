<Command("Air Cool Rinse", "Time |0-99|", , "60", "'1"), _
 TranslateCommand("zh-TW", "氣冷水洗時間", "水洗時間 |0-99|"), _
 Description("Air Cool Rinse Time=0-99 minutes"), _
 TranslateDescription("zh-TW", "氣冷水洗時間0-99分鐘")> _
Public NotInheritable Class Command24
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S24
    Off
    WaitTempSafe
    WaitLowLevel
    WaitMainPumpFB
    WaitRinseTime
    Pause
  End Enum

  Public Wait As New Timer
  Public RinseTime As New Integer
  Public StateString As String

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command10.Cancel() : .Command13.Cancel() : .Command14.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .TemperatureControl.Cancel()
      .Command17.Cancel() : .Command01.Cancel()
      .TempControlFlag = False

      RinseTime = param(1) * 60
      State = S24.WaitTempSafe
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S24.Off
          StateString = ""

        Case S24.WaitTempSafe
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S24.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "溫度異常", "Interlocked Temperature")
          If .IO.MainTemperature >= .Parameters.SetSafetyTemp * 10 Then
            .Alarms.HighTempNoFill = True
            Exit Select
          End If

          .Alarms.HighTempNoFill = False
          '.IO.Cool = True
          If RinseTime = 0 Then
            State = S24.Off
          Else
            State = S24.WaitLowLevel
          End If

        Case S24.WaitLowLevel
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S24.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "主缸沒水", "Main tank no water ")
          If Not .IO.LowLevel Then Exit Select
          .PumpStartRequest = True
          State = S24.WaitMainPumpFB

        Case S24.WaitMainPumpFB
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S24.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "主泵沒有運行", "Main pump not running")
          If Not .IO.MainPumpFB Then Exit Select
          .PumpStartRequest = False
          .PumpOn = True
          Wait.TimeRemaining = RinseTime
          State = S24.WaitRinseTime

        Case S24.WaitRinseTime
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S24.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "氣冷水洗時間 :", "Air Cool Rinse Time :") & TimerString(Wait.TimeRemaining)
          If Not Wait.Finished Then Exit Select
          State = S24.Off

        Case S24.Pause
          StateString = If(.Language = LanguageValue.ZhTW, "暫停", "Paused") & " " & TimerString(Wait.TimeRemaining)
          'no longer pause restart the timer and go back to the wait state
          If .Parent.CurrentStep <> .Parent.ChangingStep Then
            State = S24.Off
            Wait.Cancel()
          End If
          If (Not .Parent.IsPaused) And .IO.SystemAuto Then
            Wait.Restart()
            State = StateWas
            StateWas = S24.Off
          End If

      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S24.Off
    Wait.Cancel()
  End Sub

  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
    RinseTime = param(1) * 60
  End Sub


#Region "Standard Definitions"
  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S24.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S24
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S24
  Public Property State() As S24
    Get
      Return state_
    End Get
    Private Set(ByVal value As S24)
      state_ = value
    End Set
  End Property
  Public Property StateWas() As S24
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As S24)
      statewas_ = value
    End Set
  End Property
  Public ReadOnly Property IsAirCoolRinse() As Boolean
    Get
      Return (State = S24.WaitRinseTime)
    End Get
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command24 As New Command24(Me)
End Class
#End Region
