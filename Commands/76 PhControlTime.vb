<Command("PhControlTime", "TARGET PH |3-9|.|00-99| OPEN TEMP |25-140| ADD TIME |2-60| ", , , "", CommandType.ParallelCommand),
 TranslateCommand("zh-TW", "PH平行-時間", "PH值設定 |3-9|.|00-99| 起始溫度 |25-140| 加酸時間 |2-60|  "),
 Description("CONTROL PH MAX=PH 9.9,MIN=PH 3.0 ; OPEN TEMP. MAX=140C,MIN=25C ; ADD TIME MAX=60M MIN=2M  "),
 TranslateDescription("zh-TW", "PH控制  最高=PH 9.9,最低=PH 3.0 ; 起始溫度 最高=140度,最低=25度 ; 加酸時間 最高=60分 最低=2分 ")>
Public NotInheritable Class Command76
    Inherits MarshalByRefObject
    Implements ACCommand

    Public Enum S76
        Off
        Start
        Finish_Wash
        Pause
        Finish
    End Enum

    Public PhTarget, PhOpenTemp, AddTime As Integer
    Public Wait As New Timer, RunBackWait As New Timer
  Public StateString As String
  Public StepWas As Integer


    Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
        With ControlCode
            '--------------------------------------------------------------------------------------------------------PH用

            .PhControl.Cancel() : .PhWash.Cancel() : .Command73.Cancel() : .Command75.Cancel() : .Command74.Cancel() : .Command77.Cancel() : .Command78.Cancel() : .Command79.Cancel() : .Command80.Cancel()
            '---------------------------------------------------------------------------------------------------------

            PhTarget = Maximum(param(1) * 100 + param(2), 999) '60*60
            PhOpenTemp = MinMax(param(3), 9, 140)
            AddTime = MinMax(param(4), 1, 60)
            State = S76.Start


        End With
    End Function

    Public Function Run() As Boolean Implements ACCommand.Run
        With ControlCode
            Select Case State
                Case S76.Off
                    StateString = ""
                    If .Command75.State = Command75.S75.Off And .Command74.State = Command74.S74.Off And .Command77.State = Command77.S77.Off And .Command78.State = Command78.S78.Off And .Command80.State = Command80.S80.Off Then
                        .PhControlFlag = False
                    End If


        Case S76.Start
          StateString = ""
          StepWas = .Parent.StepNumber
          If .PhControlFlag = False Then
            .PhControlFlag = True
          End If


          If .Parent.IsPaused Or Not .IO.MainPumpFB Or Not .IO.SystemAuto Or .IO.pHTemperature1 > 1050 Then


            'pause the timer
            Wait.Pause()
            State = S76.Pause
          End If

          If Not .Command01.IsOn = True Then
            State = S76.Finish_Wash
          End If

          If .PhControl.State = PhControl_.PhControl.Finished Then
            State = S76.Finish_Wash
          End If

        Case S76.Pause
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
              State = S76.Start
            Else
              State = S76.Off
            End If

          End If

                Case S76.Finish_Wash
                    StateString = ""
                    If .PhControlFlag = True Then
                        .PhControlFlag = False
                    End If
                    '.PhWash.Run()
                    'If .PhWash.State = PhWash_.PhWash.Finish Then
                    State = S76.Finish
                    'End If

                Case S76.Finish

                    StateString = ""

                    State = S76.Off



            End Select

        End With
    End Function

    Public Sub Cancel() Implements ACCommand.Cancel
        State = S76.Off
        Wait.Cancel()

    End Sub
    Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged

        PhTarget = Maximum(param(1) * 100 + param(2), 999) '60*60
        PhOpenTemp = MinMax(param(3), 25, 140)
        AddTime = MinMax(param(4), 2, 60)
    End Sub
#Region "Standard Definitions"
    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
        Get
            Return State <> S76.Off
        End Get
    End Property
    Public ReadOnly Property IsActive() As Boolean
        Get
            Return State > S76.Start
        End Get
    End Property
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S76
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S76

    Public Property State() As S76
        Get
            Return state_
        End Get
        Private Set(ByVal value As S76)
            state_ = value
        End Set
    End Property
    Public ReadOnly Property Istest() As Boolean
        Get
            Return (State = S76.Start)
        End Get
    End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
    Public ReadOnly Command76 As New Command76(Me)
End Class
#End Region
