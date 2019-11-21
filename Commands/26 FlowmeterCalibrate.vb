<Command("Flowmeter Cal.", , , , "10"),
TranslateCommand("zh-TW", "流量計校正"),
Description("依照主缸低中高水位的水量來檢查流量計是否準確"),
TranslateDescription("zh-TW", "依照主缸低中高水位的水量來檢查流量計是否準確")>
Public NotInheritable Class Command26
  Inherits MarshalByRefObject                       'Inheritsg是繼承Windows Form的應用程式要繼承System.Windows.Forms.Form，可先參考物件導向程式設計相關書籍
  Implements ACCommand

  Public Enum S26
    Off
    WaitTempSafe
    DrainDelay
    ResetCounter
    FillLow1
    WaitLow1
    FillMiddle1
    WaitMiddle1
    FillHigh1
    WaitHigh1
    Pause
  End Enum
  Public StateString As String

  Public WaitTimer As New Timer
  Public HighSpeedCounter1 As Integer
  Public HighSpeedCounter1Was As Integer



  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command10.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .TemperatureControl.Cancel()
      .Command17.Cancel() : .Command01.Cancel()
      .TempControlFlag = False
      WaitTimer.TimeRemaining = .Parameters.DrainDelay
      State = S26.DrainDelay
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S26.Off
          StateString = ""


        Case S26.WaitTempSafe
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            StateWas = State
            State = S26.Pause
            WaitTimer.Pause()
          End If
          If .IO.MainTemperature >= .Parameters.SetSafetyTemp * 10 Then
            StateString = If(.Language = LanguageValue.ZhTW, "溫度異常", "Interlocked Temperature")
            .Alarms.HighTempNoFill = True
            Exit Select
          End If
          .Alarms.HighTempNoFill = False
          State = S26.DrainDelay

        Case S26.DrainDelay
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = S26.Pause
          End If
          If Not .IO.SystemAuto Then
            StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "System Manual")
            Exit Select
          End If
          If .IO.LowLevel Then WaitTimer.TimeRemaining = .Parameters.DrainDelay
          If WaitTimer.Finished Then
            .FillLiters = 0
            HighSpeedCounter1Was = .IO.HSCounter1
            HighSpeedCounter1 = .IO.HSCounter1
            State = S26.FillLow1
          Else
            StateString = If(.Language = LanguageValue.ZhTW, "排水", "Draining ") & TimerString(WaitTimer.TimeRemaining)
            Exit Select
          End If

        Case S26.FillLow1
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = S26.Pause
          End If

          HighSpeedCounter1 = .IO.HSCounter1
          If HighSpeedCounter1 > HighSpeedCounter1Was Then
            .FillLiters = .FillLitersWas + (HighSpeedCounter1 - HighSpeedCounter1Was) * .Parameters.VolumePerCount
            .FillLitersWas = .FillLiters
          ElseIf HighSpeedCounter1Was > HighSpeedCounter1 Then
            .FillLiters = .FillLitersWas + (HighSpeedCounter1 + 65535 - HighSpeedCounter1Was) * .Parameters.VolumePerCount
            .FillLitersWas = .FillLiters
          End If
          HighSpeedCounter1Was = .IO.HSCounter1

          StateString = If(.Language = LanguageValue.ZhTW, "入水到低水位", "Fill To Low Level") & " " & .FillLiters & " L"
          If Not .IO.LowLevel Then Exit Select
          WaitTimer.TimeRemaining = 5
          State = S26.WaitLow1

        Case S26.WaitLow1
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = S26.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "等待水位穩定", "Wait Stable") & " " & TimerString(WaitTimer.TimeRemaining)
          If Not WaitTimer.Finished Then Exit Select
          If .IO.LowLevel Then
            .Parameters.LowLevelLiters = .FillLiters
            State = S26.FillMiddle1
          Else
            State = S26.FillLow1
          End If

        Case S26.FillMiddle1
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = S26.Pause
          End If
          HighSpeedCounter1 = .IO.HSCounter1
          If HighSpeedCounter1 > HighSpeedCounter1Was Then
            .FillLiters = .FillLitersWas + (HighSpeedCounter1 - HighSpeedCounter1Was) * .Parameters.VolumePerCount
            .FillLitersWas = .FillLiters
          ElseIf HighSpeedCounter1Was > HighSpeedCounter1 Then
            .FillLiters = .FillLitersWas + (HighSpeedCounter1 + 65535 - HighSpeedCounter1Was) * .Parameters.VolumePerCount
            .FillLitersWas = .FillLiters
          End If
          HighSpeedCounter1Was = .IO.HSCounter1
          StateString = If(.Language = LanguageValue.ZhTW, "入水到中水位", "Fill To Middle Level") & " " & .FillLiters & " L"
          If Not .IO.MiddleLevel Then Exit Select
          WaitTimer.TimeRemaining = 5
          State = S26.WaitMiddle1


        Case S26.WaitMiddle1
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = S26.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "等待水位穩定", "Wait Stable") & " " & TimerString(WaitTimer.TimeRemaining)
          If Not WaitTimer.Finished Then Exit Select
          If .IO.MiddleLevel Then
            .Parameters.MiddleLevelLiters = .FillLiters
            State = S26.FillHigh1
          Else
            State = S26.FillMiddle1
          End If

        Case S26.FillHigh1
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = S26.Pause
          End If
          HighSpeedCounter1 = .IO.HSCounter1
          If HighSpeedCounter1 > HighSpeedCounter1Was Then
            .FillLiters = .FillLitersWas + (HighSpeedCounter1 - HighSpeedCounter1Was) * .Parameters.VolumePerCount
            .FillLitersWas = .FillLiters
          ElseIf HighSpeedCounter1Was > HighSpeedCounter1 Then
            .FillLiters = .FillLitersWas + (HighSpeedCounter1 + 65535 - HighSpeedCounter1Was) * .Parameters.VolumePerCount
            .FillLitersWas = .FillLiters
          End If
          HighSpeedCounter1Was = .IO.HSCounter1
          StateString = If(.Language = LanguageValue.ZhTW, "入水到高水位", "Fill To High Level") & " " & .FillLiters & " L"
          If Not .IO.HighLevel Then Exit Select
          WaitTimer.TimeRemaining = 5
          State = S26.WaitHigh1

        Case S26.WaitHigh1
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = S26.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "等待水位穩定", "Wait Stable") & " " & TimerString(WaitTimer.TimeRemaining)
          If Not WaitTimer.Finished Then Exit Select
          If .IO.HighLevel Then
            .Parameters.HighLevelLiters = .FillLiters
            State = S26.Off
          Else
            State = S26.FillHigh1
          End If

        Case S26.Pause
          StateString = Translations.Translate("Paused") & " " & TimerString(WaitTimer.TimeRemaining)
          If Not .Parent.IsPaused Then
            State = StateWas
            StateWas = S26.Off
            WaitTimer.Restart()
          End If

      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S26.Off
    WaitTimer.Cancel()
  End Sub
  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
  End Sub

#Region " Standard Definitions "

  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S26.Off
    End Get
  End Property
  Public ReadOnly Property IsFilling() As Boolean
    Get
      Return State = S26.FillLow1 Or State = S26.FillMiddle1 Or State = S26.FillHigh1
    End Get
  End Property
  Public ReadOnly Property IsDrain() As Boolean
    Get
      Return State = S26.DrainDelay
    End Get
  End Property
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S26
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S26
  Public Property State() As S26
    Get
      Return state_
    End Get
    Private Set(ByVal value As S26)
      state_ = value
    End Set
  End Property
  Public Property StateWas() As S26
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As S26)
      statewas_ = value
    End Set
  End Property
  'End Property

#End Region

End Class

#Region " Class Instance "

Partial Public Class ControlCode
  Public ReadOnly Command26 As New Command26(Me)
End Class

#End Region
