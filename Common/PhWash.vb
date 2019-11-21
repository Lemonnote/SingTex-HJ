Public Class PhWash_
    Public Enum PhWash
        off
        Wash
        Wash1
        Wash2
        Wash3
        Wash4
        Wash5
        Finish
    End Enum
    Public StateString As String
    Public Wait, Wait1 As New Timer
    Public C37 As Integer
    Public Sub Run()
        With ControlCode
            'Usual pressure State control this will by default start at pressurized


            Select Case State
                Case PhWash.off
                    StateString = ""
                    If .PhCirRun Then
                        .pHWashRun = False
                        State = PhWash.off
                    End If
                    If .pHWashRun Or .IO.ManualPHWash Then
                        State = PhWash.Wash
                    End If

                Case PhWash.Wash
                    '-------------------------------------------------------------------------------------------
                    '開清洗管路 .IO.PhWashFill
                    If .PhCirRun Then
                        .pHWashRun = False
                        State = PhWash.off
                    End If
                    If .Parameters.PhCirTank = 1 Then
                        StateString = If(.Language = LanguageValue.ZhTW, "進清水到迴流桶高水位!!", "Cir to highlevel！！")
                        If .IO.PhMixTankHighLevel Then
                            Wait.TimeRemaining = 0
                            'Wait.TimeRemaining = .Parameters.PhWashTime
                            StateString = If(.Language = LanguageValue.ZhTW, "PH管路清洗中!!", "PH WASH FILLING！！")
                            State = PhWash.Wash1
                        End If
                    Else
                        Wait.TimeRemaining = 0
            StateString = If(.Language = LanguageValue.ZhTW, "PH管路清洗中!!", "PH WASH FILLING！！")
            State = PhWash.Wash5
          End If

                Case PhWash.Wash1
                    StateString = If(.Language = LanguageValue.ZhTw, "PH管路清洗中!!", "PH WASH FILLING！！")
                    If .PhCirRun Then
                        .pHWashRun = False
                        State = PhWash.off
                    End If
                    If Not .IO.PhMixTankLowLevel Then
                        Wait.TimeRemaining = 20
                        State = PhWash.Wash2
                    End If
                Case PhWash.Wash2
                    StateString = If(.Language = LanguageValue.ZhTw, "PH管路清洗中!!", "PH WASH FILLING！！")
                    If .PhCirRun Then
                        .pHWashRun = False
                        State = PhWash.off
                    End If
                    If Wait.Finished Then
                        State = PhWash.Wash5
                    End If
                Case PhWash.Wash5
          StateString = If(.Language = LanguageValue.ZhTW, "PH管路清洗中!!", "PH WASH FILLING！！")
          If Wait.Finished Then
            State = PhWash.Finish
            .pHWashRun = False
          End If

                Case PhWash.Finish
                    .pHWashFinish = True
                    State = PhWash.off

      End Select

        End With

    End Sub
    Public Sub Cancel()
        State = PhWash.off

    End Sub
#Region "Standard Definitions"

    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As PhWash
    Public Property State() As PhWash
        Get
            Return state_
        End Get
        Private Set(ByVal value As PhWash)
            state_ = value
        End Set
    End Property
    Public ReadOnly Property IsOn() As Boolean
        Get
            Return (State <> PhWash.off)
        End Get
    End Property
    Public ReadOnly Property PhWashFill() As Boolean            'PH入清水
        Get
      Return (State = PhWash.Wash) Or (State = PhWash.Wash2)
        End Get
    End Property
    Public ReadOnly Property PhCirculatePump() As Boolean        'PH循環泵
        Get
            Return (State = PhWash.Wash1) Or (State = PhWash.Wash2)
        End Get
    End Property
    Public ReadOnly Property PhDrain() As Boolean           'PH排水     
        Get
            Return (State = PhWash.Wash1) Or (State = PhWash.Wash2)
        End Get
    End Property

    Public ReadOnly Property PhNoCirTank() As Boolean           'PH排水     
        Get
            Return (State = PhWash.Wash5)
        End Get
    End Property

#End Region
End Class

Partial Class ControlCode
    Public ReadOnly PhWash As New PhWash_(Me)
End Class
