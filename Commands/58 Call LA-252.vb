<Command("Call LA-252", "Call off |0-99|", , , "1", CommandType.Standard), _
TranslateCommand("zh-TW", "呼叫染料備藥", "呼叫染料備藥 |0-99|"), _
Description("0-99 Call off"), _
TranslateDescription("zh-TW", "0-99 呼叫染料備藥")> _
Public NotInheritable Class Command58
  Inherits MarshalByRefObject                       'Inheritsg是繼承Windows Form的應用程式要繼承System.Windows.Forms.Form，可先參考物件導向程式設計相關書籍
  Implements ACCommand

  Public Enum S58
    Off
    CheckLevel
    CheckManualButton
    WaitAuto
    Ready
  End Enum

  Public StateString As String
  Public CallOff As Integer
  Public WaitTimer As New Timer

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      'cancels for all other forground functions
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command10.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .Command01.Cancel()

      CallOff = param(1)
      .Command64.Cancel()
      .Command54.Cancel()
      .Command66.Cancel()
      .Command56.Cancel()
      .SPCConnectError = False
      State = S58.CheckLevel
    End With
  End Function


  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S58.Off
          StateString = ""

        Case S58.CheckLevel
          StateString = If(.Language = LanguageValue.ZhTW, "B缸有水", "B Tank not empty")
          If .BTankLowLevel Then Exit Select
          State = S58.CheckManualButton

        Case S58.CheckManualButton
          StateString = If(.Language = LanguageValue.ZhTW, "請關閉B缸手動開關", "Please close B tank manual button")
          If .IO.BDrainPB Or .IO.BTankFillColdPB Or .IO.BTankFillCircPB Or .IO.BAllIn Then Exit Select
          State = S58.WaitAuto

        Case S58.WaitAuto
          StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "Interlocked not In auto")
          If Not .IO.SystemAuto Then Exit Select
          State = S58.Ready

        Case S58.Ready
          .DyeCallOff = 0   'Starts the handshake with the host / auto dispenser
          .DyeTank = 1
          .RunCallLA252 = True
          State = S58.Off


      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S58.Off
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
      Return State <> S58.Off
    End Get
  End Property
  Public ReadOnly Property IsNotReady() As Boolean
    Get
      Return (State = S58.CheckManualButton) Or (State = S58.CheckLevel)
    End Get
  End Property
  Public ReadOnly Property IsNotEmpty() As Boolean
    Get
      Return (State = S58.CheckLevel)
    End Get
  End Property
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S58
  Public Property State() As S58          'Property	屬性名稱() As 傳回值型別
    Get                                 'Get
      Return state_                   '屬性名稱 = 私有資料成員        '讀取私有資料成員的值
    End Get                             'End Get
    Private Set(ByVal value As S58)     'Set(ByVal Value As傳回值型別)
      state_ = value                  '私有資料成員 = Value          '設定私有資料成員的值
    End Set                             'End Set
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
  Public ReadOnly Command58 As New Command58(Me)
End Class

#End Region
