<Command("MainTankDrainByTankC", "Qty% |0-100|", , , "1", CommandType.Standard), _
 TranslateCommand("zh-TW", "主缸經C缸排水", "水量% |0-100|"), _
 Description("100%=MAX 0%=MIN"), _
 TranslateDescription("zh-TW", "100%=最大 0%=最小")> _
Public NotInheritable Class Command61
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S61
    Off
    WaitTempSafe
    WaitSystemAuto
    WaitFill
    Drain
  End Enum

  Public Qty As Integer
  Public Wait As New Timer, RunBackWait As New Timer
  Public StateString As String

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      'cancels for all other forground functions
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command10.Cancel() : .Command01.Cancel()


      Qty = MinMax(param(1) * 10, 0, 1000)

      .Command65.Cancel()
      .Command57.Cancel()
      .Command67.Cancel()
      .Command55.Cancel()

      State = S61.WaitSystemAuto

    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      ' Run this command
      Select Case State
        Case S61.Off
          StateString = ""

          'check that the temp is ok
        Case S61.WaitTempSafe
          StateString = If(.Language = LanguageValue.ZhTW, "溫度異常", "Interlocked Temperature")
          If .IO.MainTemperature >= .Parameters.SetAddSafetyTemp * 10 Then
            .Alarms.HighTempNoAdd = True
            Exit Select
          End If
          .Alarms.HighTempNoAdd = False

          State = S61.WaitSystemAuto

          'check that we are in auto
        Case S61.WaitSystemAuto
          StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "Interlocked not In auto")
          If Not .IO.SystemAuto Then Exit Select
          State = S61.WaitFill

          'fill to set Qty
        Case S61.WaitFill
          StateString = If(.Language = LanguageValue.ZhTW, "C缸迴水中", "Tank C circulating")
          If .IO.TankCLevel > Qty Then
            State = S61.Drain
          End If

        Case S61.Drain
          StateString = If(.Language = LanguageValue.ZhTW, "C缸排水中", "Tank C draining") & TimerString(Wait.TimeRemaining)
          If .CTankLowLevel Then Wait.TimeRemaining = .Parameters.AddTransferDrainTime
          If Wait.Finished Then
            State = S61.Off
          End If

      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S61.Off
    Wait.Cancel()
    RunBackWait.Cancel()
  End Sub

  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged

  End Sub


#Region "Standard Definitions"
  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S61.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S61
  Public Property State() As S61
    Get
      Return state_
    End Get
    Private Set(ByVal value As S61)
      state_ = value
    End Set
  End Property

  Public ReadOnly Property IsFillingCirc() As Boolean
    Get
      Return (State = S61.WaitFill)
    End Get
  End Property
  Public ReadOnly Property IsDrain() As Boolean
    Get
      Return (State = S61.Drain)
    End Get
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command61 As New Command61(Me)
End Class
#End Region
