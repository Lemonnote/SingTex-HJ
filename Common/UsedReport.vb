Public Class UsedReport
    Public 主泵運行時間計數器 As New TimerUp
    Public 升溫運行計時器 As New TimerUp
    Public 降溫運行計時器 As New TimerUp
    Public 排水運行計時器 As New TimerUp


    Public Sub Run()
        With ControlCode
            If .IO.MainPumpFB And Not .MainPumpStartWas Then
                主泵運行時間計數器.Restart()
            End If
            .MainPumpStartWas = .IO.MainPumpFB

            If Not .IO.MainPumpFB And Not .MainPumpStopWas Then
                主泵運行時間計數器.Pause()
            End If
            .MainPumpStopWas = Not .IO.MainPumpFB

            If (.IO.Drain Or .IO.HotDrain Or .IO.PowerDrain Or .IO.PowerHotDrain) And Not .DrainStartWas Then
                排水運行計時器.Restart()
            End If
            .DrainStartWas = .IO.Drain Or .IO.HotDrain Or .IO.PowerDrain Or .IO.PowerHotDrain

            If Not .IO.Drain And Not .IO.HotDrain And Not .IO.PowerDrain And Not .IO.PowerHotDrain And Not .DrainStopWas Then
                排水運行計時器.Pause()
            End If
            .DrainStartWas = Not .IO.Drain And Not .IO.HotDrain And Not .IO.PowerDrain And Not .IO.PowerHotDrain

            If .IO.Heat And Not .HeatStartWas Then
                升溫運行計時器.Restart()
            End If
            .HeatStartWas = .IO.Heat

            If Not .IO.Heat And Not .HeatStopWas Then
                升溫運行計時器.Pause()
            End If
            .HeatStopWas = Not .IO.Heat

            If .IO.Cool And Not .CoolStartWas Then
                降溫運行計時器.Restart()
            End If
            .CoolStartWas = .IO.Cool

            If Not .IO.Cool And Not .CoolStopWas Then
                降溫運行計時器.Pause()
            End If
            .CoolStopWas = Not .IO.Cool

            .IO.主泵時間分鐘 = 主泵運行時間計數器.TimeElapsed \ 60
            .IO.升溫時間分鐘 = 升溫運行計時器.TimeElapsed \ 60
            .IO.降溫時間分鐘 = 降溫運行計時器.TimeElapsed \ 60
            .IO.排水時間分鐘 = 排水運行計時器.TimeElapsed \ 60

            If .Parent.IsProgramRunning Then
                主泵運行時間計數器.Start()
                升溫運行計時器.Start()
                降溫運行計時器.Start()
                排水運行計時器.Start()
            Else
                主泵運行時間計數器.Stop()
                升溫運行計時器.Stop()
                降溫運行計時器.Stop()
                排水運行計時器.Stop()
            End If
        End With
    End Sub

#Region "Standard Definitions"

    Private ReadOnly ControlCode As ControlCode
    Public Sub New(ByVal controlCode As ControlCode)
        Me.ControlCode = controlCode
    End Sub
    <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As UsedReport
    Public Property State() As UsedReport
        Get
            Return state_
        End Get
        Private Set(ByVal value As UsedReport)
            state_ = value
        End Set
    End Property
    Public ReadOnly Wait As New Timer

    'this is for the dosing valve

#End Region
End Class

Partial Class ControlCode
    Public ReadOnly UsedReport As New UsedReport(Me)
End Class
