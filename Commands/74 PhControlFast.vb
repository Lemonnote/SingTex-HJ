<Command("PhControlFast", "TARGET PH |3-9|.|00-99|", , , "", CommandType.ParallelCommand),
 TranslateCommand("zh-TW", "PH平行-快速", "PH值設定 |3-9|.|00-99|"),
 Description("MAX=PH 9.99,MIN=PH 3.00"),
 TranslateDescription("zh-TW", "最高=PH 9.9,最低=PH 3.0")>
Public NotInheritable Class Command74
    Inherits MarshalByRefObject
    Implements ACCommand

    Public Enum S74
        Off
        Start
        Finish_Wash
        Pause
        Finish
    End Enum

    Public PhTarget As Integer
    Public Wait As New Timer, RunBackWait As New Timer
  Public StateString As String
  Public StepWas As Integer


    Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
        With ControlCode
            '--------------------------------------------------------------------------------------------------------PH用

            .PhControl.Cancel() : .PhWash.Cancel() : .Command73.Cancel() : .Command75.Cancel() : .Command76.Cancel() : .Command77.Cancel() : .Command78.Cancel() : .Command79.Cancel() : .Command80.Cancel()
            '---------------------------------------------------------------------------------------------------------

            PhTarget = Maximum(param(1) * 100 + param(2), 999) '60*60
      Wait.TimeRemaining = 1
      State = S74.Start


        End With
    End Function

    Public Function Run() As Boolean Implements ACCommand.Run
        With ControlCode
            Select Case State
                Case S74.Off
                    StateString = ""
                    If .Command75.State = Command75.S75.Off And .Command76.State = Command76.S76.Off And .Command77.State = Command77.S77.Off And .Command78.State = Command78.S78.Off And .Command80.State = Command80.S80.Off Then
                        .PhControlFlag = False
                    End If


                Case S74.Start
          StepWas = .Parent.StepNumber
          StateString = ""
          If Not Wait.Finished Then Exit Select
                    If .PhControlFlag = False Then
                        .PhControlFlag = True
                    End If

          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Or .IO.pHTemperature1 > 1050 Then
            'pause the timer
            Wait.Pause()
            State = S74.Pause
          End If

          '  If Not (.Command01.IsOn Or .Command02.IsOn) Then
          ' State = S74.Finish_Wash
          ' End If

          If .PhControl.State = PhControl_.PhControl.Finished Then
            State = S74.Finish_Wash
          End If
          If .IO.pHValue <= PhTarget Then
            State = S74.Finish_Wash
          End If


        Case S74.Pause
          If Not .IO.MainPumpFB Then
            StateString = If(.Language = LanguageValue.ZhTW, "馬達未啟動！", "Not Running")
          End If
          If .IO.pHTemperature1 > 1050 Then
            StateString = If(.Language = LanguageValue.ZhTW, "PH檢測桶溫度過高！", "Hot temp.")
          End If
          'no longer pause restart the timer and go back to the wait state
          If (Not .Parent.IsPaused) And .IO.MainPumpFB And .IO.SystemAuto And .IO.pHTemperature1 < 1050 Then

            If .Parent.StepNumber = StepWas Then
              Wait.Restart()
              State = S74.Start
            Else
              State = S74.Off
            End If

          End If

        Case S74.Finish_Wash
          StateString = ""
          If .PhControlFlag = True Then
            .PhControlFlag = False
          End If
          '.PhWash.Run()
          'If .PhWash.State = PhWash_.PhWash.Finish Then
          State = S74.Finish
          'End If

        Case S74.Finish

          StateString = ""

          State = S74.Off

      End Select

        End With
    End Function

    Public Sub Cancel() Implements ACCommand.Cancel
        State = S74.Off
        Wait.Cancel()

    End Sub
    Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
        PhTarget = Maximum(param(1) * 100 + param(2), 999) '60*60

    End Sub
#Region "Standard Definitions"
    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
        Get
            Return State <> S74.Off
        End Get
    End Property
    Public ReadOnly Property IsActive() As Boolean
        Get
            Return State > S74.Start
        End Get
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S74
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S74

    Public Property State() As S74
        Get
            Return state_
        End Get
        Private Set(ByVal value As S74)
            state_ = value
        End Set
    End Property
    Public ReadOnly Property Istest() As Boolean
        Get
            Return (State = S74.Start)
        End Get
    End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
    Public ReadOnly Command74 As New Command74(Me)
End Class
#End Region
