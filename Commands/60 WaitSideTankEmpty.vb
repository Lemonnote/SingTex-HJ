<Command("Wait Side Tank Empty", , , , "1", CommandType.Standard), _
TranslateCommand("zh-TW", "等待藥缸空缸", )> _
Public NotInheritable Class Command60
  Inherits MarshalByRefObject                       'Inheritsg是繼承Windows Form的應用程式要繼承System.Windows.Forms.Form，可先參考物件導向程式設計相關書籍
  Implements ACCommand

  Public Enum S60
    Off
    WaitAuto
    WaitSideTankEmpty
  End Enum
  Public StateString As String

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode

      .Command54.Cancel() : .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel()

      State = S60.WaitAuto
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S60.Off
          StateString = ""

        Case S60.WaitAuto
          StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "Interlocked not In auto")
          If Not .IO.SystemAuto Then Exit Select
          State = S60.WaitSideTankEmpty

        Case S60.WaitSideTankEmpty
          StateString = If(.Language = LanguageValue.ZhTW, "等待藥缸空缸", "Wait Side Tank Empty")
          If .BTankLowLevel Or .CTankLowLevel Then Exit Select
          State = S60.Off

      End Select

    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S60.Off
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
      Return State <> S60.Off
    End Get
  End Property
  Public ReadOnly Property IsDraining() As Boolean
    Get
      Return (State = S60.WaitSideTankEmpty)
    End Get
  End Property
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S60
  Public Property State() As S60          'Property	屬性名稱() As 傳回值型別
    Get                                 'Get
      Return state_                   '屬性名稱 = 私有資料成員        '讀取私有資料成員的值
    End Get                             'End Get
    Private Set(ByVal value As S60)     'Set(ByVal Value As傳回值型別)
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
  Public ReadOnly Command60 As New Command60(Me)
End Class

#End Region
