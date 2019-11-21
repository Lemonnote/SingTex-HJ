Public Module TickCountModule
  Friend TickCount As UInt32
End Module

Public Class DelayTimer
  Private ReadOnly timer_ As New Timer
  Private lastInput_ As Boolean
  Public Function Run(ByVal input As Boolean, ByVal delay As Integer) As Boolean
    If input Then
      If Not lastInput_ Then timer_.TimeRemaining = delay
      Run = timer_.Finished
    Else
      timer_.Cancel()
    End If
    lastInput_ = input
  End Function
End Class

' Timer - down timer
Public Class Timer
  Private interval_, startTickCount_, pauseTickCount_ As UInt32, paused_, finished_ As Boolean

  Public Property TimeRemaining() As Integer  ' in seconds
    Get
      Return (TimeRemainingMs + 999) \ 1000
    End Get
    Set(ByVal value As Integer)  ' in seconds
      TimeRemainingMs = value * 1000
    End Set
  End Property

  Friend Property TimeRemainingMs() As Integer
    Get
      If Finished Then Return 0
      Dim t As UInt32
      If paused_ Then
        t = pauseTickCount_
      Else
        t = TickCount
      End If
      Return CType(interval_ - (t - startTickCount_), Integer)
    End Get
    Set(ByVal value As Integer)
      interval_ = CType(value, UInt32)
      startTickCount_ = TickCount
      paused_ = False : finished_ = False
    End Set
  End Property

  Friend ReadOnly Property Finished() As Boolean
    Get
      If Not finished_ AndAlso Not paused_ AndAlso TickCount - startTickCount_ >= interval_ Then finished_ = True
      Return finished_
    End Get
  End Property

  Private Sub CheckFinished()
    If Not paused_ AndAlso TickCount - startTickCount_ >= interval_ Then finished_ = True
  End Sub

  Public Sub Pause()
    'If we're already finshed or paused then ignore
    If Finished OrElse paused_ Then Exit Sub
    paused_ = True : pauseTickCount_ = TickCount
  End Sub

  Public Sub Restart()
    'If we're not paused then ignore
    If finished_ OrElse Not paused_ Then Exit Sub
    Dim lostTime As UInt32 = TickCount - pauseTickCount_
    startTickCount_ += lostTime
    paused_ = False
  End Sub

  Public Sub Cancel()
    TimeRemainingMs = 0
  End Sub
End Class


' -------------------------------------------------------------------------
' Timer - up timer
Public Class TimerUp
  Private startTickCount_, pauseInterval_ As UInt32, started_, paused_ As Boolean

  Public Sub Start()
    started_ = True
    startTickCount_ = TickCount
    paused_ = False
  End Sub

  Public Sub [Stop]()
    started_ = False
  End Sub

  Public ReadOnly Property TimeElapsed() As Integer
    Get
      Return TimeElapsedMs \ 1000 ' return in seconds
    End Get
  End Property

  Friend ReadOnly Property TimeElapsedMs() As Integer
    Get
      If Not started_ Then Return 0
      If paused_ Then Return CType(pauseInterval_, Integer)
      Return CType(TickCount - startTickCount_, Integer)
    End Get
  End Property

  Public Sub Pause()
    'If we're already paused then ignore
    If Not paused_ Then paused_ = True : pauseInterval_ = TickCount - startTickCount_
  End Sub

  Public Sub Restart()
    'If we're not paused then ignore
    If Not paused_ Then Exit Sub

    startTickCount_ = TickCount - pauseInterval_
    paused_ = False
  End Sub
End Class

' -------------------------------------------------------------------------
Public Class Flasher
  Private startTime_ As UInt32

  Public Sub Flash(ByRef variable As Boolean, ByVal onMilliSeconds As Integer)
    Flash(variable, onMilliSeconds, onMilliSeconds)
  End Sub

  Public Sub Flash(ByRef variable As Boolean, ByVal onMilliSeconds As Integer, ByVal offMilliSeconds As Integer)
    ' Initialise StartTime
    Dim elapsed As UInt32
    If startTime_ = 0 Then
      startTime_ = TickCount
    Else
      elapsed = TickCount - startTime_
    End If

    ' Generate a number that ranges from 0 to the sum
    Dim x As UInt32 = elapsed Mod CType(onMilliSeconds + offMilliSeconds, UInt32)
    variable = (x < onMilliSeconds)
  End Sub
End Class

' -------------------------------------------------------------------------

Public Class Smoothing
  ' Use a circular buffer of values
  Private values_() As Integer, count_, firstOfs_ As Integer, sum_ As Long

  Public Function Smooth(ByVal value As Integer, ByVal smoothing As Integer) As Integer
    If smoothing < 2 Then Return value ' no need to smooth
    If values_ Is Nothing OrElse values_.Length <> smoothing Then
      values_ = New Integer(smoothing - 1) {}
      count_ = 0 : firstOfs_ = 0
    End If
    ' Keep a correct sum at all times for performance
    sum_ += value
    If count_ < smoothing Then
      values_(count_) = value : count_ += 1
    Else
      sum_ -= values_(firstOfs_) : values_(firstOfs_) = value
      firstOfs_ += 1 : If firstOfs_ = count_ Then firstOfs_ = 0
    End If
    Return CType(sum_ \ count_, Integer)
  End Function

  Public Function Smooth(ByVal value As Short, ByVal smoothing As Integer) As Short
    Return CType(Smooth(CType(value, Integer), smoothing), Short)
  End Function
End Class

' -------------------------------------------------------------------------
' Raises an event if there has been no mouse or keyboard activity for this application for a while
Public Class InactiveTimeout : Implements IMessageFilter, IDisposable
  Public Event Timeout As EventHandler
  Private reLoad_, remaining_ As Integer
  Private timer_ As New Threading.Timer(AddressOf OnTimer, Nothing, 1000, 1000)

  Public Sub New()
    Application.AddMessageFilter(Me)
  End Sub

  Public Sub Dispose() Implements IDisposable.Dispose
    If timer_ IsNot Nothing Then
      timer_.Dispose() : timer_ = Nothing
      Application.RemoveMessageFilter(Me)
    End If
  End Sub

  Public Sub SetTimeout(ByVal seconds As Integer)
    reLoad_ = seconds : remaining_ = seconds
  End Sub

  Private Sub OnTimer(ByVal state As Object)
    Static inside_ As Boolean : If inside_ Then Exit Sub
    inside_ = True
    If remaining_ > 0 Then
      remaining_ -= 1
      If remaining_ = 0 Then RaiseEvent Timeout(Me, EventArgs.Empty)
    End If
    inside_ = False
  End Sub

  Private Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage
    Select Case m.Msg
      Case &H201 To &H20D, &HA0 To &HAD, &H100 To &H109  ' all mouse and keyboard messages (except WM_MOUSEMOVE)
        If remaining_ > 0 Then remaining_ = reLoad_
    End Select
    Return False
  End Function
End Class

' ------------------------------------------------------------------------------
' Support subs and functions
Module Support
  Public Function MulDiv(ByVal value As Integer, ByVal multiply As Integer, ByVal divide As Integer) As Integer
    If divide = 0 Then Return 0 ' no divide by zero error please
    Return CType((CType(value, Long) * multiply) \ divide, Integer)
  End Function
  Public Function MulDiv(ByVal value As Short, ByVal multiply As Integer, ByVal divide As Integer) As Short
    If divide = 0 Then Return 0 ' no divide by zero error please
    Return CType((CType(value, Long) * multiply) \ divide, Short)
  End Function

  Public Function ReScale(ByVal value As Integer, ByVal inMin As Integer, ByVal inMax As Integer, ByVal outMin As Integer, ByVal outMax As Integer) As Integer
    If inMin = inMax Then Return 0 ' avoid division by zero
    If value < inMin Then value = inMin
    If value > inMax Then value = inMax
    Return MulDiv(value - inMin, outMax - outMin, inMax - inMin) + outMin
  End Function

  Public Function ReScale(ByVal value As Short, ByVal inMin As Integer, ByVal inMax As Integer, ByVal outMin As Integer, ByVal outMax As Integer) As Short
    Return CType(ReScale(CType(value, Integer), inMin, inMax, outMin, outMax), Short)
  End Function

  '#If 0 Then
  'Use this to avoid divide by zero. Returns 1 If value is less than one
  Public Function Min1(ByVal value As Integer) As Integer
    If value < 1 Then Return 1
    Return value
  End Function
  'Use this sub to set minimum of two values
  Public Function Minimum(ByVal target As Integer, ByVal minimumValue As Integer) As Integer
    'If target too tiny set to minimum value
    If target < minimumValue Then Return minimumValue
    Return target
  End Function
  'Use this sub to set maximum of two values
  Public Function Maximum(ByVal target As Integer, ByVal maximumValue As Integer) As Integer
    'F54       攪拌時間(      param(1)*60      ,           1800               )
    'param(1)*60 => target      1800 => maximumValue
    If target > maximumValue Then Return maximumValue '如果目標值target大於最大值maximumValue   
    Return target                   '                 'If target too large set to maximum value 
  End Function

  Public Function MinMax(ByVal target As Integer, ByVal minimumValue As Integer, ByVal maximumValue As Integer) As Integer
    'F54      水源選擇(       param(2)        ,        0                     ,          1                   )  
    'F54      水量設定(       param(4)*10     ,        600                   ,          1000                )         
    If target < minimumValue Then Return minimumValue
    If target > maximumValue Then Return maximumValue
    Return target
  End Function
  '#End If

  Public Function TimerString(ByVal secs As Integer) As String
    ' TODO: pre-make many of these for speed ?
    Dim hours As Integer = secs \ 3600, minutes As Integer = (secs Mod 3600) \ 60, _
        seconds As Integer = (secs Mod 60)

    ' Hours and minutes
    If hours > 0 Then Return hours.ToString("00", Globalization.CultureInfo.InvariantCulture) & ":" _
                             & minutes.ToString("00", Globalization.CultureInfo.InvariantCulture) & "m"
    ' Minutes and seconds
    Return minutes.ToString("00", Globalization.CultureInfo.InvariantCulture) & ":" _
             & seconds.ToString("00", Globalization.CultureInfo.InvariantCulture) & "s"
  End Function
End Module