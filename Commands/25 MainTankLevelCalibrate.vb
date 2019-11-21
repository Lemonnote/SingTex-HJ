<Command("Main Level Cal.", "Qty|0-100| Time|0-99| Volume|0-999| Start|0-9999| End|0-9999|", , , "('4/60)+2"),
TranslateCommand("zh-TW", "主缸水位校正", "水量|0-100|% 穩定時間|0-99| 容量|0-999| 開始|0-9999| 結束|0-9999|"),
Description("水量=0-100%, 容量=0-999L"),
TranslateDescription("zh-TW", "水量=0-100%, 容量=0-999L")>
Public NotInheritable Class Command25
  Inherits MarshalByRefObject                       'Inheritsg是繼承Windows Form的應用程式要繼承System.Windows.Forms.Form，可先參考物件導向程式設計相關書籍
  Implements ACCommand

  Public Enum S25
    Off
    CheckReady
    FillQty
    Add
    WaitStable
    Update
    Pause
  End Enum
  Public StateString As String

  Public Time, Qty, Number, StartVolume, EndVolume, Volume, TotalVolume As Integer
  Public WaitTimer As New Timer
  Public CurrentStepNumber As Integer
  Public LowLevelUpdated, MiddleLevelUpdated, HighLevelUpdated, OverflowLevelUpdated As Boolean

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
      Qty = MinMax(param(1) * 10, 0, 1000)
      Time = Maximum(param(2), 99)
      Volume = MinMax(param(3), 0, 999)
      StartVolume = MinMax(param(4), 0, 9999)
      EndVolume = MinMax(param(5), 0, 9999)
      Number = 0
      LowLevelUpdated = False
      MiddleLevelUpdated = False
      HighLevelUpdated = False
      OverflowLevelUpdated = False
      State = S25.CheckReady
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S25.Off
          StateString = ""

        Case S25.CheckReady
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = S25.Pause
            CurrentStepNumber = .Parent.CurrentStep
          End If
          If Not .IO.SystemAuto Then Exit Select
          Number = Number + 1
          State = S25.FillQty

        Case S25.FillQty
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = S25.Pause
            CurrentStepNumber = .Parent.CurrentStep
          End If
          StateString = "B缸進水" & " " & Qty / 10 & "%"
          If .IO.TankBLevel >= Qty Then
            State = S25.Add
            WaitTimer.TimeRemaining = .Parameters.AddTransferTimeAfterRinse
          End If

        Case S25.Add
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = S25.Pause
            CurrentStepNumber = .Parent.CurrentStep
          End If
          StateString = "加藥延遲" & " " & TimerString(WaitTimer.TimeRemaining)
          If .IO.TankBLevel >= 50 Then WaitTimer.TimeRemaining = .Parameters.AddTransferTimeAfterRinse
          If Not WaitTimer.Finished Then Exit Select
          State = S25.WaitStable
          WaitTimer.TimeRemaining = Time

        Case S25.WaitStable
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = S25.Pause
            CurrentStepNumber = .Parent.CurrentStep
          End If
          StateString = "等待穩定" & " " & TimerString(WaitTimer.TimeRemaining)
          If Not WaitTimer.Finished Then Exit Select
          State = S25.Update

        Case S25.Update
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = S25.Pause
            CurrentStepNumber = .Parent.CurrentStep
          End If
          If Not LowLevelUpdated And .IO.LowLevel Then
            .Parameters.LowLevelVolume = TotalVolume
            LowLevelUpdated = True
          End If
          If Not MiddleLevelUpdated And .IO.MiddleLevel Then
            .Parameters.MiddleLevelVolume = TotalVolume
            MiddleLevelUpdated = True
          End If
          If Not HighLevelUpdated And .IO.LowLevel Then
            .Parameters.HighLevelVolume = TotalVolume
            HighLevelUpdated = True
          End If
          TotalVolume = TotalVolume + Volume
          StateString = "目前水量" & " " & TotalVolume & "L, 按呼叫確認執行藥缸進水"
          If Not .IO.CallAck Then Exit Select
          If TotalVolume >= EndVolume Then
            State = S25.Off
          Else
            State = S25.CheckReady
          End If

        Case S25.Pause
          StateString = Translations.Translate("Paused") & " " & TimerString(WaitTimer.TimeRemaining)
          If Not .Parent.IsPaused Then
            If CurrentStepNumber = .Parent.CurrentStep Then
              State = StateWas
              StateWas = S25.Off
              WaitTimer.Restart()
            Else
              State = S25.Off
              WaitTimer.Cancel()
            End If
          End If

      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S25.Off
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
      Return State <> S25.Off
    End Get
  End Property
  Public ReadOnly Property IsFilling() As Boolean
    Get
      Return State = S25.FillQty
    End Get
  End Property
  Public ReadOnly Property IsAdd() As Boolean
    Get
      Return State = S25.Add
    End Get
  End Property
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S25
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S25
  Public Property State() As S25
    Get
      Return state_
    End Get
    Private Set(ByVal value As S25)
      state_ = value
    End Set
  End Property
  Public Property StateWas() As S25
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As S25)
      statewas_ = value
    End Set
  End Property
  'End Property

#End Region

End Class

#Region " Class Instance "

Partial Public Class ControlCode
  Public ReadOnly Command25 As New Command25(Me)
End Class

#End Region
