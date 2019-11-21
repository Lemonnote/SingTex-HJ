Public Class CTankPrepare_
  Public Enum CTankPrepare
    Off
    WaitAuto
    WaitNoAddButtons
    WaitLowLevel
    DispenseWaitReady
    ResetCallOff
    DispenseWaitResponse
    Slow
    FillQty
    MixForTime
    WaitMixStop
    Ready
    Pause
  End Enum
  Public StateString As String

  Public Time, TimeWas, Type, Qty, CallOff As Integer
  Public WaitTimer As New Timer
  Public WaitDispenser As Boolean


  Public Sub Run()
    With ControlCode
      Select Case State
        Case CTankPrepare.Off
          StateString = ""
          If .RunCTankPrepare Then
            Time = .Command55.Time
            Type = .Command55.Type
            CallOff = .Command55.CallOff
            Qty = .Command55.Qty
          Else
            Exit Select
          End If
          State = CTankPrepare.WaitAuto

        Case CTankPrepare.WaitAuto
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            WaitTimer.Pause()
            StateWas = State
            State = CTankPrepare.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "系統手動中", "Interlocked not In auto")
          If Not .IO.SystemAuto Then Exit Select
          WaitDispenser = False
          State = CTankPrepare.WaitNoAddButtons

        Case CTankPrepare.WaitNoAddButtons
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            WaitTimer.Pause()
            StateWas = State
            State = CTankPrepare.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "等待藥缸", "Tank C Interlocked")
          If .IO.C1Add Or .IO.C2Add Or .IO.C3Add Or .IO.C4Add Or .IO.C5Add Then Exit Select

          If CallOff > 0 And .Parameters.ChemicalEnable = 1 Then
            .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
            .ChemicalTank = 0
            .TankCReady = False
            State = CTankPrepare.DispenseWaitReady
          Else
            State = CTankPrepare.WaitLowLevel
          End If

        Case CTankPrepare.WaitLowLevel
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            WaitTimer.Pause()
            StateWas = State
            State = CTankPrepare.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "C缸進水至低水位", "Tank C fill to low level")
          If Not .CTankLowLevel Then Exit Select
          State = CTankPrepare.Slow

        Case CTankPrepare.DispenseWaitReady
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = CTankPrepare.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "助劑備藥中", "Prepare Tank C")
          'TODO  Add timeout code to switch to manual if no response

          If .ChemicalState = EDispenseState.Ready Then
            'Dispenser is ready so set CallOff number and wait for result
            .ChemicalCallOff = CallOff
            .ChemicalTank = 1
            State = CTankPrepare.DispenseWaitResponse
          End If
          'Switch to manual if enable parameter is changed or calloff is reset
          If .Parameters.ChemicalEnable <> 1 Then State = CTankPrepare.Slow
          If CallOff = 0 Then State = CTankPrepare.Slow

        Case CTankPrepare.DispenseWaitResponse
          If .Parent.IsPaused Then
            WaitTimer.Pause()
            StateWas = State
            State = CTankPrepare.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "等待助劑計量", "Wait for Chemical Dispensing")
          If .CTankLowLevel And WaitTimer.Finished And WaitDispenser Then
            .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
            .ChemicalTank = 0
            State = CTankPrepare.Slow
            WaitDispenser = False
          End If
          If .CTankLowLevel And Not WaitDispenser Then
            WaitTimer.TimeRemaining = 30
            WaitDispenser = True
          End If
          If Not .CTankLowLevel Then Exit Select
          Select Case .ChemicalState
            Case EDispenseState.Complete
              'Everything completed ok so set ready flag and carry on
              .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
              .ChemicalTank = 0
              State = CTankPrepare.Slow

            Case EDispenseState.Manual
              'Manual dispenses required so call the operator
              .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
              .ChemicalTank = 0
              State = CTankPrepare.Slow

            Case EDispenseState.Error
              'Dispense error call the operator
              .ChemicalCallOff = 0   'Starts the handshake with the host / auto dispenser
              .ChemicalTank = 0
              State = CTankPrepare.Slow
          End Select
          'Switch to manual if enable parameter is changed or calloff is reset
          If .Parameters.ChemicalEnable <> 1 Then State = CTankPrepare.Slow
          If CallOff = 0 Then State = CTankPrepare.Slow

        Case CTankPrepare.Slow
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            WaitTimer.Pause()
            StateWas = State
            State = CTankPrepare.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "備藥完成，請按備藥OK", "Prepare Tank C")
          .ChemicalCallOff = 0
          .ChemicalTank = 0
          If (CallOff > 0) And (.Parameters.CTankCallAck = 1) Then
            .TankCReady = True
          End If
          If .TankCReady Then
            If Qty > 5 Then
              State = CTankPrepare.FillQty
            Else
              State = CTankPrepare.MixForTime
              WaitTimer.TimeRemaining = Time
            End If
          End If

        Case CTankPrepare.FillQty
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            WaitTimer.Pause()
            StateWas = State
            State = CTankPrepare.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "C藥缸進水至 ", "Filling Tank C to ") & Qty / 10 & "%"
          If .IO.TankCLevel > Qty Then
            State = CTankPrepare.MixForTime
            WaitTimer.TimeRemaining = Time
          End If

        Case CTankPrepare.MixForTime
          StateString = If(.Language = LanguageValue.ZhTW, "C藥缸攪拌 ", "Tank C mixing for ") & TimerString(WaitTimer.TimeRemaining)
          If WaitTimer.Finished Then
            State = CTankPrepare.WaitMixStop
            WaitTimer.TimeRemaining = 10
          End If
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            WaitTimer.Pause()
            StateWas = State
            State = CTankPrepare.Pause
          End If

        Case CTankPrepare.WaitMixStop
          StateString = If(.Language = LanguageValue.ZhTW, "等待C藥缸穩定 ", "Wait Tank C Stable ") & TimerString(WaitTimer.TimeRemaining)
          If WaitTimer.Finished Then
            State = CTankPrepare.Ready
          End If

        Case CTankPrepare.Ready
          .RunCTankPrepare = False
          State = CTankPrepare.Off

        Case CTankPrepare.Pause
          StateString = If(.Language = LanguageValue.ZhTW, "暫停 ", "Paused ") & " " & TimerString(WaitTimer.TimeRemaining)
          If .Parent.CurrentStep <> .Parent.ChangingStep Then
            .RunCTankPrepare = False
            State = CTankPrepare.Off
            WaitTimer.Cancel()
            .ChemicalCallOff = 0
            .ChemicalTank = 0
            .ChemicalState = 101
          End If
          If Not .Parent.IsPaused And .IO.SystemAuto Then
            If StateWas = CTankPrepare.MixForTime Then
              State = StateWas
              StateWas = CTankPrepare.Off
              If TimeWas = Time Then
                WaitTimer.Restart()
              Else
                WaitTimer.TimeRemaining = Time
              End If
            Else
              State = StateWas
              StateWas = CTankPrepare.Off
              WaitTimer.Restart()
            End If
          End If
      End Select

    End With
  End Sub

#Region " Standard Definitions "
  Public Sub Cancel()
    With ControlCode
      State = CTankPrepare.Off
      .RunCTankPrepare = False
      WaitTimer.Cancel()
    End With
  End Sub

  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean
    Get
      Return State <> CTankPrepare.Off
    End Get
  End Property
  Public ReadOnly Property IsTankInterlocked() As Boolean
    Get
      Return (State = CTankPrepare.WaitNoAddButtons)
    End Get
  End Property
  Public ReadOnly Property IsFillingFresh() As Boolean
    Get
      Return (Type = 1) And ((State = CTankPrepare.FillQty) Or (State = CTankPrepare.WaitLowLevel))
    End Get
  End Property
  Public ReadOnly Property IsFillingCirc() As Boolean
    Get
      Return (Type = 0) And ((State = CTankPrepare.FillQty) Or (State = CTankPrepare.WaitLowLevel))
    End Get
  End Property
  Public ReadOnly Property IsSlow() As Boolean
    Get
      Return (State = CTankPrepare.Slow)
    End Get
  End Property
  Public ReadOnly Property IsMixingForTime() As Boolean
    Get
      Return (State = CTankPrepare.MixForTime)
    End Get
  End Property
  Public ReadOnly Property IsReady() As Boolean
    Get
      Return (State = CTankPrepare.Ready)
    End Get
  End Property
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As CTankPrepare
  Public Property State() As CTankPrepare
    Get
      Return state_
    End Get
    Private Set(ByVal value As CTankPrepare)
      state_ = value
    End Set
  End Property
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As CTankPrepare
  Public Property StateWas() As CTankPrepare
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As CTankPrepare)
      statewas_ = value
    End Set
  End Property


#End Region

End Class

#Region " Class Instance "

Partial Public Class ControlCode
  Public ReadOnly CTankPrepare As New CTankPrepare_(Me)
End Class

#End Region
