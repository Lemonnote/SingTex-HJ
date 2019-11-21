Imports System.Drawing.Drawing2D
Imports System.Drawing.Design
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Windows.Forms.Design
Imports System.Globalization
Imports System.Security.Permissions
Imports System.ComponentModel.Design
Imports System.ComponentModel
Imports System.Globalization.CultureInfo


Namespace MimicControls
  ' ----------------------------------------------------
  ' Instrumentation Widgets
  <TypeConverter(GetType(Border.BorderConverter))> _
  Public Class Border
    Private innerBorder_, outerBorder_ As Boolean, style_ As BorderStyle

    Public Sub New(ByVal style As BorderStyle, ByVal outerBorder As Boolean, ByVal innerBorder As Boolean)
      style_ = style : innerBorder_ = innerBorder : outerBorder_ = outerBorder
    End Sub

    Public Sub Draw(ByVal g As Graphics, ByVal r As Rectangle)
      Draw(g, r, innerBorder_, outerBorder_, style_)
    End Sub

    Public Shared Sub Draw(ByVal g As Graphics, ByVal r As Rectangle, _
             ByVal innerBorder As Boolean, ByVal outerBorder As Boolean, ByVal style As BorderStyle)
      If style <> BorderStyle.None Then
        Dim location As Point = r.Location
        Dim point2 As Point = (r.Location + r.Size)
        If outerBorder Then
          g.DrawRectangle(Pens.Black, r)
          location.X += 1
          location.Y += 1
          point2.X -= 1
          point2.Y -= 1
        End If
        Dim controlLightLight As Pen = SystemPens.ControlLightLight
        Dim controlDark As Pen = SystemPens.ControlDark
        Dim pen As Pen = Pens.Black
        Dim black As Pen = Pens.Black
        Select Case style
          Case BorderStyle.Flat
            pen = controlDark
            black = controlDark
            Exit Select
          Case BorderStyle.Single
            pen = Pens.Black
            black = Pens.Black
            Exit Select
          Case BorderStyle.Double
            pen = Pens.Black
            black = Pens.Black
            Exit Select
          Case BorderStyle.Raised
            pen = controlLightLight
            black = controlDark
            Exit Select
          Case BorderStyle.Lowered
            pen = controlDark
            black = controlLightLight
            Exit Select
          Case BorderStyle.DoubleRaised
            pen = controlLightLight
            black = controlDark
            Exit Select
          Case BorderStyle.DoubleLowered
            pen = controlDark
            black = controlLightLight
            Exit Select
          Case BorderStyle.FrameRaised
            pen = controlLightLight
            black = controlDark
            Exit Select
          Case BorderStyle.FrameLowered
            pen = controlDark
            black = controlLightLight
            Exit Select
        End Select
        g.DrawLine(pen, location.X, location.Y, location.X, point2.Y)
        g.DrawLine(pen, location.X, location.Y, point2.X, location.Y)
        g.DrawLine(black, location.X, point2.Y, point2.X, point2.Y)
        g.DrawLine(black, point2.X, location.Y, point2.X, point2.Y)
        If (((style = BorderStyle.Double) OrElse (style = BorderStyle.DoubleRaised)) OrElse (((style = BorderStyle.DoubleLowered) OrElse (style = BorderStyle.FrameLowered)) OrElse (style = BorderStyle.FrameRaised))) Then
          If ((style = BorderStyle.FrameLowered) OrElse (style = BorderStyle.FrameRaised)) Then
            Dim pen5 As Pen = pen
            pen = black
            black = pen5
          End If
          location.X += 1
          location.Y += 1
          point2.X -= 1
          point2.Y -= 1
          g.DrawLine(pen, location.X, location.Y, location.X, point2.Y)
          g.DrawLine(pen, location.X, location.Y, point2.X, location.Y)
          g.DrawLine(black, location.X, point2.Y, point2.X, point2.Y)
          g.DrawLine(black, point2.X, location.Y, point2.X, point2.Y)
        End If
        If innerBorder Then
          Dim rect As New Rectangle((location.X + 1), (location.Y + 1), ((point2.X - location.X) - 2), ((point2.Y - location.Y) - 2))
          g.DrawRectangle(Pens.Black, rect)
        End If
      End If
    End Sub

    Public Function GetMargin() As Integer
      Dim num As Integer
      Select Case style_
        Case BorderStyle.None
          num = 0
        Case BorderStyle.Flat, BorderStyle.Single, BorderStyle.Raised, BorderStyle.Lowered
          num = 1
        Case Else
          num = 2
      End Select
      If outerBorder_ Then num += 1
      If innerBorder_ Then num += 1
      Return num
    End Function


    ' Properties
    <DefaultValue(False), NotifyParentProperty(True), Category("Appearance"), Description("Indicate whether the inner border should be drawn")> _
    Public Property InnerBorder() As Boolean
      Get
        Return innerBorder_
      End Get
      Set(ByVal value As Boolean)
        innerBorder_ = value
      End Set
    End Property

    <DefaultValue(False), Category("Appearance"), NotifyParentProperty(True), Description("Indicate whether the outer border should be drawn")> _
    Public Property OuterBorder() As Boolean
      Get
        Return outerBorder_
      End Get
      Set(ByVal value As Boolean)
        outerBorder_ = value
      End Set
    End Property

    <NotifyParentProperty(True), Category("Appearance"), Description("Style of control's border")> _
    Public Property Style() As BorderStyle
      Get
        Return style_
      End Get
      Set(ByVal value As BorderStyle)
        style_ = value
      End Set
    End Property

    ' ----------------------------------------
    Public Class BorderConverter
      Inherits TypeConverter
      ' Methods
      Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
        If (destinationType Is GetType(String)) Then
          Dim border As Border = DirectCast(value, Border)
          With New System.Text.StringBuilder
            .Append(border.style_.ToString)
            If border.outerBorder_ Then .Append("+OuterBorder")
            If border.innerBorder_ Then .Append("+InnerBorder")
            Return .ToString
          End With
        End If
        Return MyBase.ConvertTo(context, culture, value, destinationType)
      End Function

      Public Overrides Function CreateInstance(ByVal context As ITypeDescriptorContext, ByVal propertyValues As IDictionary) As Object
        Dim sty As BorderStyle
        Dim outerBorder, innerBorder As Boolean
        If propertyValues.Contains("Style") Then sty = DirectCast(propertyValues.Item("Style"), BorderStyle)
        If propertyValues.Contains("OuterBorder") Then outerBorder = CBool(propertyValues.Item("OuterBorder"))
        If propertyValues.Contains("InnerBorder") Then innerBorder = CBool(propertyValues.Item("InnerBorder"))
        Return New Border(sty, outerBorder, innerBorder)
      End Function

      Public Overrides Function GetCreateInstanceSupported(ByVal context As ITypeDescriptorContext) As Boolean
        Return True
      End Function

      Public Overrides Function GetProperties(ByVal context As ITypeDescriptorContext, ByVal value As Object, ByVal attributes As Attribute()) As PropertyDescriptorCollection
        Return TypeDescriptor.GetProperties(GetType(Border), attributes)
      End Function

      Public Overrides Function GetPropertiesSupported(ByVal context As ITypeDescriptorContext) As Boolean
        Return True
      End Function
    End Class
  End Class

  Public Enum BorderStyle
    None
    Flat
    [Single]
    [Double]
    Raised
    Lowered
    DoubleRaised
    DoubleLowered
    FrameRaised
    FrameLowered
  End Enum

  Public Class Panel : Inherits ContainerControl
    Private border_ As New Border(BorderStyle.None, False, False), _
            transparentBackColor_ As Boolean = True

    Public Sub New()
      SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
    End Sub

    <DefaultValue(GetType(Color), "Transparent")> _
    Public Overrides Property BackColor() As System.Drawing.Color
      Get
        If transparentBackColor_ Then Return Color.Transparent
        Return MyBase.BackColor
      End Get
      Set(ByVal value As Color)
        If BackColor = value Then Exit Property ' no change
        If value = Color.Transparent Then
          transparentBackColor_ = True
          DoubleBuffered = False
          RecreateHandle()
        Else
          MyBase.BackColor = value
          If transparentBackColor_ Then
            transparentBackColor_ = False
            DoubleBuffered = True
            RecreateHandle()
          End If
        End If
      End Set
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property BackgroundImage() As Image
      Get
        Return MyBase.BackgroundImage
      End Get
      Set(ByVal value As Image)
        MyBase.BackgroundImage = value
      End Set
    End Property
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property BackgroundImageLayout() As ImageLayout
      Get
        Return MyBase.BackgroundImageLayout
      End Get
      Set(ByVal value As ImageLayout)
        MyBase.BackgroundImageLayout = value
      End Set
    End Property

    Protected Overrides ReadOnly Property CreateParams() As CreateParams
      Get
        Dim cp As CreateParams = MyBase.CreateParams
        If transparentBackColor_ Then
          Const WS_EX_TRANSPARENT As Int32 = &H20
          cp.ExStyle = cp.ExStyle Or WS_EX_TRANSPARENT
        End If
        Return cp
      End Get
    End Property

    Protected Overrides Sub OnPaintBackground(ByVal pevent As PaintEventArgs)
    End Sub
    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
      If e.ClipRectangle.IsEmpty Then Exit Sub
      If Not transparentBackColor_ Then
        Using backBrush As New SolidBrush(BackColor)
          e.Graphics.FillRectangle(backBrush, e.ClipRectangle)
        End Using
      End If
      ' Draw the border
      Dim drc As Rectangle = ClientRectangle
      border_.Draw(e.Graphics, New Rectangle(drc.Left, drc.Top, drc.Width - 1, drc.Height - 1))
    End Sub


    <Category("Appearance"), Description("Control's Border"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
    Public Property Border() As Border
      Get
        Return border_
      End Get
      Set(ByVal value As Border)
        border_ = value : Invalidate()
      End Set
    End Property

    Private Function ShouldSerializeBorder() As Boolean
      Return border_.Style <> BorderStyle.None
    End Function

    ' The DisplayRectangle has any border removed
    Public Overrides ReadOnly Property DisplayRectangle() As Rectangle
      Get
        Dim ret As Rectangle = MyBase.DisplayRectangle, margin As Integer = border_.GetMargin
        ret.Inflate(-margin, -margin)
        Return ret
      End Get
    End Property

    Public Overloads Sub Invalidate()
      Invalidate(ClientRectangle)
    End Sub

    Public Overloads Sub Invalidate(ByVal rc As Rectangle)
      If transparentBackColor_ Then
        ' We must invalidate part of our parent as well
        Dim par As Control = Parent
        If par IsNot Nothing Then
          Dim pan As Panel = TryCast(par, Panel)
          If pan IsNot Nothing Then
            pan.Invalidate(Bounds)
          Else
            par.Invalidate(Bounds, True) ' TODO: include rc in the calc
          End If
        End If
      End If
      MyBase.Invalidate(rc)
    End Sub

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property ForeColor() As Color
      Get
        Return MyBase.ForeColor
      End Get
      Set(ByVal value As Color)
        MyBase.ForeColor = value
      End Set
    End Property
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property Font() As Font
      Get
        Return MyBase.Font
      End Get
      Set(ByVal value As Font)
        MyBase.Font = value
      End Set
    End Property
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property Text() As String
      Get
        Return MyBase.Text
      End Get
      Set(ByVal value As String)
        MyBase.Text = value
      End Set
    End Property
  End Class

  <DefaultBindingProperty("Checked")> _
  Public MustInherit Class ButtonBase : Inherits BorderTransparentControl
    Implements IButtonControl

    Private behaviour_ As ButtonBehaviour, dialogResult_ As DialogResult, _
            checked_, highlighted_, pressed_ As Boolean

    Public Enum ButtonBehaviour
      Normal
      CheckBox
      RadioButton
    End Enum


    ' Events
    <Description("Occurs when the Behaviour property changed"), Category("Property Changed")> _
    Public Event BehaviourChanged As EventHandler
    <Description("Occurs when the Checked property changed"), Category("Property Changed")> _
    Public Event CheckedChanged As EventHandler
    <Category("Property Changed"), Description("Occurs when the DialogResult property changed")> _
    Public Event DialogResultChanged As EventHandler

#If 0 Then
    Protected Sub DrawSelect(ByVal g As Graphics)
      If Focused Then
        Dim rect As Rectangle = DisplayRectangle
        rect.Width -= 1
        rect.Height -= 1
        Using pen As New Pen(Color.Black) : pen.DashStyle = DashStyle.Dot
          g.DrawRectangle(pen, rect)
        End Using
      End If
    End Sub
#End If

    Protected Overrides Function IsInputKey(ByVal keyData As Keys) As Boolean
      If Enabled Then Return keyData = Keys.Space
      Return False
    End Function

    Protected Overridable Sub OnBehaviourChanged(ByVal e As EventArgs)
      RaiseEvent BehaviourChanged(Me, e)
    End Sub

    Protected Overridable Sub OnCheckedChanged(ByVal e As EventArgs)
      RaiseEvent CheckedChanged(Me, e)
    End Sub

    Protected Overrides Sub OnClick(ByVal e As EventArgs)
      If Enabled Then
        Select Case behaviour_
          Case ButtonBehaviour.CheckBox
            checked_ = Not checked_
          Case ButtonBehaviour.RadioButton
            checked_ = True
            If Not Parent Is Nothing Then
              For Each control As Control In Parent.Controls
                If Not control Is Me AndAlso TypeOf control Is ButtonBase Then
                  DirectCast(control, ButtonBase).Checked = False
                End If
              Next control
            End If
        End Select
        Try
          MyBase.OnClick(e)
        Catch exception As Exception
          pressed_ = False
          Throw exception
        End Try
      End If
    End Sub

    Protected Overridable Sub OnDialogResultChanged(ByVal e As EventArgs)
      RaiseEvent DialogResultChanged(Me, e)
    End Sub

    Protected Overrides Sub OnEnter(ByVal e As EventArgs)
      Invalidate()
      MyBase.OnEnter(e)
    End Sub

    Protected Overrides Sub OnKeyDown(ByVal e As KeyEventArgs)
      If (Enabled AndAlso (e.KeyData = Keys.Space)) Then
        pressed_ = True
      End If
      MyBase.OnKeyDown(e)
    End Sub

    Protected Overrides Sub OnKeyUp(ByVal e As KeyEventArgs)
      If Enabled AndAlso e.KeyData = Keys.Space Then
        pressed_ = False
        Me.OnClick(EventArgs.Empty)
      End If
      MyBase.OnKeyDown(e)
    End Sub

    Protected Overrides Sub OnLeave(ByVal e As EventArgs)
      Invalidate()
      MyBase.OnLeave(e)
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal mevent As MouseEventArgs)
      If TabStop AndAlso Enabled Then Focus()
      If mevent.Button = MouseButtons.Left Then pressed_ = True
      MyBase.OnMouseDown(mevent)
    End Sub

    Protected Overrides Sub OnMouseEnter(ByVal e As EventArgs)
      highlighted_ = True
      MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(ByVal e As EventArgs)
      highlighted_ = False
      MyBase.OnMouseLeave(e)
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal mevent As MouseEventArgs)
      If mevent.Button = MouseButtons.Left Then pressed_ = False
      MyBase.OnMouseUp(mevent)
    End Sub

    Protected Overrides Sub OnTextChanged(ByVal e As EventArgs)
      Invalidate()
      MyBase.OnTextChanged(e)
    End Sub

    Public Sub PerformClick() Implements IButtonControl.PerformClick
      OnClick(EventArgs.Empty)
    End Sub

    Private Sub NotifyDefault(ByVal value As Boolean) Implements IButtonControl.NotifyDefault
    End Sub

    ' Properties
    <Description("Button's behaviour type"), Category("Behavior"), DefaultValue(GetType(ButtonBehaviour), "CheckBox")> _
    Public Overridable Property Behaviour() As ButtonBehaviour
      Get
        Return behaviour_
      End Get
      Set(ByVal value As ButtonBehaviour)
        If behaviour_ <> value Then
          behaviour_ = value
          OnBehaviourChanged(EventArgs.Empty)
        End If
      End Set
    End Property

    <DefaultValue(False), Category("Behavior"), Description("Indicate if the button is checked(pressed)")> _
    Public Property Checked() As Boolean
      Get
        Return checked_
      End Get
      Set(ByVal value As Boolean)
        If checked_ <> value Then
          checked_ = value
          Invalidate()
          OnCheckedChanged(EventArgs.Empty)
        End If
      End Set
    End Property

    <Category("Behavior"), Description("Modal Dialog Result"), DefaultValue(GetType(DialogResult), "None")> _
    Public Property DialogResult() As DialogResult Implements IButtonControl.DialogResult
      Get
        Return dialogResult_
      End Get
      Set(ByVal value As DialogResult)
        If dialogResult_ <> value Then
          dialogResult_ = value
          OnDialogResultChanged(EventArgs.Empty)
        End If
      End Set
    End Property

    Protected Property Highlighted() As Boolean
      Get
        If Enabled Then Return highlighted_
        Return False
      End Get
      Set(ByVal value As Boolean)
        If highlighted_ <> value Then highlighted_ = value : Invalidate()
      End Set
    End Property

    Protected Property Pressed() As Boolean
      Get
        If Not pressed_ Then Return checked_
        Return True
      End Get
      Set(ByVal value As Boolean)
        If pressed_ <> value Then pressed_ = value : Invalidate()
      End Set
    End Property
  End Class




  Public Class CircularGauge : Inherits BoundedValueControl
    Private dangerColor_ As Color = Color.Red, dangerValue_ As Integer = 70
    Private dialColor_ As Color = Color.White
    Private largeTickFrequency_ As Integer = 10, smallTickFrequency_ As Integer = 2, _
            numberFrequency_ As Integer = 10
    Private pointerColor_ As Color = Color.Red
    Private showGradient_ As Boolean, showNumbers_ As Boolean = True
    Private totalAngle_ As Integer = 270

    ' Want this back
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
      Public Overrides Property Text() As String
      Get
        Return MyBase.Text
      End Get
      Set(ByVal value As String)
        MyBase.Text = value
      End Set
    End Property

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      Dim brush1 As Brush
      Dim num5 As Integer
      Dim single3 As Single
      Dim single4 As Single
      Dim num6 As Double
      Dim num7 As Double
      Dim num8 As Double
      Dim num9 As Integer
      Dim num10 As Double
      Dim num11 As Double
      Dim text1 As String
      Dim ef1 As SizeF
      Dim num12 As Double
      Dim num13 As Double
      Dim tf1 As PointF
      Dim tf2 As PointF
      Dim ef2 As SizeF
      MyBase.OnDraw(e)
      Dim g As Graphics = e.Graphics
      Dim rectangle1 As Rectangle = DisplayRectangle
      Dim pen1 As Pen = New Pen(ForeColor)
      Dim pen2 As Pen = New Pen(dangerColor_)
      If showGradient_ Then
        brush1 = New LinearGradientBrush(rectangle1, GraphicsUtils.ScaleColor(dialColor_, 0.5!), dialColor_, LinearGradientMode.Vertical)
      Else
        brush1 = New SolidBrush(dialColor_)
      End If
      Dim brush2 As Brush = New SolidBrush(ForeColor)
      g.FillEllipse(brush1, rectangle1)
      Dim point1 As Point = Center
      Dim num1 As Integer = CType((CType(Radius, Single) * 0.8!), Integer)
      Dim single1 As Single = (CType((Value - Minimum), Single) / (CType(Maximum, Single) - CType(Minimum, Single)))
      Dim single2 As Single = (single1 * CType(totalAngle_, Single))
      If (Not Text Is "") Then
        ef2 = g.MeasureString(Text, Font)
        g.DrawString(Text, Font, brush2, Point.op_Implicit(New Point(CType((CType(point1.X, Single) - (ef2.Width / 2.0!)), Integer), ((point1.Y - 15) - (Font.Height \ 2)))))
      End If
      DrawPointer(g, point1, single2, (CType(num1, Single) * 0.8!))
      Dim num2 As Integer = CType((CType(num1, Single) * 0.7!), Integer)
      Dim num3 As Integer = (num2 + 3)
      Dim num4 As Integer = (num3 + 6)
      num5 = Minimum
      Do While (num5 <= Maximum)
        single1 = (CType((num5 - Minimum), Single) / (CType(Maximum, Single) - CType(Minimum, Single)))
        single3 = (single1 * CType(totalAngle_, Single))
        single4 = ((CType(totalAngle_, Single) / 2.0!) - single3)
        num6 = (CType(point1.X, Double) - (Math.Sin((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(num2, Double)))
        num7 = (CType(point1.Y, Double) - (Math.Cos((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(num2, Double)))
        num8 = CType(num3, Double)
        If ((num5 Mod largeTickFrequency_) = 0) Then
          num8 = CType(num4, Double)
        End If
        If (showNumbers_ AndAlso ((num5 Mod numberFrequency_) = 0)) Then
          num9 = (num4 + Font.Height)
          num10 = (CType(point1.X, Double) - (Math.Sin((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(num9, Double)))
          num11 = (CType(point1.Y, Double) - (Math.Cos((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(num9, Double)))
          text1 = ScaleAndFormatValue(num5)
          ef1 = g.MeasureString(text1, Font)
          num10 = (num10 - CType((ef1.Width / 2.0!), Double))
          num11 = (num11 - CType((ef1.Height / 2.0!), Double))
          g.DrawString(text1, Font, brush2, Point.op_Implicit(New Point(CType(num10, Integer), CType(num11, Integer))))
        End If
        num12 = (CType(point1.X, Double) - (Math.Sin((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * num8))
        num13 = (CType(point1.Y, Double) - (Math.Cos((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * num8))
        tf1 = New PointF(CType(num6, Single), CType(num7, Single))
        tf2 = New PointF(CType(num12, Single), CType(num13, Single))
        If (num5 < dangerValue_) Then
          g.DrawLine(pen1, tf1, tf2)
        Else
          g.DrawLine(pen2, tf1, tf2)
        End If
        num5 = (num5 + smallTickFrequency_)
      Loop
    End Sub

    Private Sub DrawPointer(ByVal g As Graphics, ByVal p As Point, ByVal angle As Single, ByVal rad As Single)
      Dim point1 As Point
      Dim single1 As Single = ((CType(totalAngle_, Single) / 2.0!) - angle)
      Dim num1 As Double = (CType(p.X, Double) - (Math.Sin((CType((single1 / 180.0!), Double) * 3.1415926535897931)) * CType(rad, Double)))
      Dim num2 As Double = (CType(p.Y, Double) - (Math.Cos((CType((single1 / 180.0!), Double) * 3.1415926535897931)) * CType(rad, Double)))
      point1 = New Point(CType(num1, Integer), CType(num2, Integer))
      g.DrawLine(New Pen(pointerColor_), p, point1)
    End Sub

    Protected Overrides Sub OnTextChanged(ByVal e As EventArgs)
      Invalidate()
      MyBase.OnTextChanged(e)
    End Sub

    Private ReadOnly Property Center() As Point
      Get
        Dim rectangle1 As Rectangle = DisplayRectangle
        Return New Point(((rectangle1.Left + rectangle1.Right) \ 2), ((rectangle1.Bottom + rectangle1.Top) \ 2))
      End Get
    End Property

    <Category("Appearance"), Description("Danger color"), DefaultValue(GetType(Color), "Red")> _
    Public Property DangerColor() As Color
      Get
        Return dangerColor_
      End Get
      Set(ByVal value As Color)
        If Not dangerColor_.Equals(value) Then dangerColor_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(70), Category("Behavior"), Description("Danger Value")> _
    Public Property DangerValue() As Integer
      Get
        Return dangerValue_
      End Get
      Set(ByVal value As Integer)
        If dangerValue_ <> value Then dangerValue_ = value : Invalidate()
      End Set
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(168, 168)
      End Get
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "White"), Description("Color of the dial")> _
    Public Property DialColor() As Color
      Get
        Return dialColor_
      End Get
      Set(ByVal value As Color)
        If Not dialColor_.Equals(value) Then dialColor_ = value : Invalidate()
      End Set
    End Property

    <Description("Define frequency of large ticks"), DefaultValue(10), Category("Appearance")> _
    Public Property LargeTickFrequency() As Integer
      Get
        Return largeTickFrequency_
      End Get
      Set(ByVal value As Integer)
        If largeTickFrequency_ <> value Then largeTickFrequency_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(10), Description("Define frequency of numbers"), Category("Appearance")> _
    Public Property NumberFrequency() As Integer
      Get
        Return numberFrequency_
      End Get
      Set(ByVal value As Integer)
        If numberFrequency_ <> value Then numberFrequency_ = value : Invalidate()
      End Set
    End Property

    <Description("Color of the pointer"), DefaultValue(GetType(Color), "Red"), Category("Appearance")> _
    Public Property PointerColor() As Color
      Get
        Return pointerColor_
      End Get
      Set(ByVal value As Color)
        If Not pointerColor_.Equals(value) Then pointerColor_ = value : Invalidate()
      End Set
    End Property

    Private ReadOnly Property Radius() As Integer
      Get
        Dim rectangle1 As Rectangle = DisplayRectangle
        Return Math.Min((rectangle1.Height \ 2), (rectangle1.Width \ 2))
      End Get
    End Property

    <Description("Gets or sets whether control should use gradient background"), Category("Appearance"), DefaultValue(False)> _
    Public Property ShowGradient() As Boolean
      Get
        Return showGradient_
      End Get
      Set(ByVal value As Boolean)
        If showGradient_ <> value Then showGradient_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(True), Description("Indicate if the control paints scale numbers")> _
    Public Property ShowNumbers() As Boolean
      Get
        Return showNumbers_
      End Get
      Set(ByVal value As Boolean)
        If showNumbers_ <> value Then showNumbers_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(2), Description("Define frequency of small ticks"), Category("Appearance")> _
    Public Property SmallTickFrequency() As Integer
      Get
        Return smallTickFrequency_
      End Get
      Set(ByVal value As Integer)
        If smallTickFrequency_ <> value Then smallTickFrequency_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(270), Category("Appearance"), Description("Angle behind minimum and maximum positions")> _
    Public Property TotalAngle() As Integer
      Get
        Return totalAngle_
      End Get
      Set(ByVal value As Integer)
        If totalAngle_ <> value Then totalAngle_ = value : Invalidate()
      End Set
    End Property
  End Class

  Public Class Dial : Inherits BoundedValueControl
    Private dialColor_ As Color = Color.White, dialRadius_ As Integer = 30, _
            largeTickFrequency_ As Integer = 10, pointerColor_ As Color = Color.Red, _
            pointerRadius_ As Integer = 20, showNumbers_ As Boolean = True, showTicks_ As Boolean = True, _
            smallTickFrequency_ As Integer = 2, totalAngle_ As Integer = 270

    Public Sub New()
      SetStyle(ControlStyles.Selectable, True)
    End Sub

    ' Want this back
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
      Public Overrides Property Text() As String
      Get
        Return MyBase.Text
      End Get
      Set(ByVal value As String)
        MyBase.Text = value
      End Set
    End Property

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      Dim g As Graphics = e.Graphics
      Using pen As New Pen(ForeColor)
        Using brush As New SolidBrush(ForeColor)
          Dim p As Point = Center
          Dim num As Single = (CSng((MyBase.Value - Minimum)) / (Maximum - Minimum))
          Dim angle As Single = (num * totalAngle_)
          DrawDial(g, p, angle)
          Dim num3 As Integer = (dialRadius_ + 2)
          Dim num4 As Integer = (num3 + 3)
          Dim num5 As Integer = (num4 + 5)
          If showTicks_ Then
            Dim i As Integer = Minimum
            Do While (i <= Maximum)
              num = (CSng((i - Minimum)) / (Maximum - Minimum))
              Dim num7 As Single = (num * totalAngle_)
              Dim num8 As Single = ((CSng(totalAngle_) / 2.0!) - num7)
              Dim num9 As Double = (p.X - (Math.Sin(((num8 / 180.0!) * Math.PI)) * num3))
              Dim num10 As Double = (p.Y - (Math.Cos(((num8 / 180.0!) * Math.PI)) * num3))
              Dim num11 As Double = num4
              If ((i Mod largeTickFrequency_) = 0) Then
                num11 = num5
                If showNumbers_ Then
                  Dim num12 As Integer = (num5 + Font.Height)
                  Dim num13 As Double = (p.X - (Math.Sin(((num8 / 180.0!) * Math.PI)) * num12))
                  Dim num14 As Double = (p.Y - (Math.Cos(((num8 / 180.0!) * Math.PI)) * num12))
                  Dim text As String = MyBase.ScaleAndFormatValue(i)
                  Dim ef As SizeF = g.MeasureString([text], Font)
                  num13 = (num13 - (ef.Width / 2.0!))
                  num14 = (num14 - (ef.Height / 2.0!))
                  g.DrawString([text], Font, brush, CInt(num13), CInt(num14))
                End If
              End If
              Dim num15 As Double = (p.X - (Math.Sin(((num8 / 180.0!) * Math.PI)) * num11))
              Dim num16 As Double = (p.Y - (Math.Cos(((num8 / 180.0!) * Math.PI)) * num11))
              g.DrawLine(pen, CInt(num9), CInt(num10), CInt(num15), CInt(num16))
              i = (i + smallTickFrequency_)
            Loop
          End If
          g.DrawString(Text, Font, brush, p.X - CInt(g.MeasureString(Text, Font).Width) \ 2, p.Y + num5 + Font.Height \ 2)
        End Using
      End Using
    End Sub

    Private Sub DrawDial(ByVal g As Graphics, ByVal p As Point, ByVal angle As Single)
      Dim rect As New Rectangle((p.X - pointerRadius_), (p.Y - pointerRadius_), (pointerRadius_ * 2), (pointerRadius_ * 2))
      Dim rectangle2 As New Rectangle((p.X - dialRadius_), (p.Y - dialRadius_), (dialRadius_ * 2), (dialRadius_ * 2))
      Dim num As Single = (p.X - (dialRadius_ * 0.6!))
      Dim num2 As Single = (p.Y - (dialRadius_ * 0.6!))
      Dim num3 As Single = (dialRadius_ * 1.85!)
      Using path As New Drawing2D.GraphicsPath
        path.AddEllipse(CSng((num - num3)), CSng((num2 - num3)), CSng((num3 * 2.0!)), CSng((num3 * 2.0!)))
        Using brush As New PathGradientBrush(path)
          brush.CenterColor = dialColor_
          Dim colorArray2 As Color() = New Color() {GraphicsUtils.ScaleColor(dialColor_, 0.25!)}
          Dim colorArray As Color() = colorArray2
          brush.SurroundColors = colorArray
          num = (p.X + (pointerRadius_ * 0.6!))
          num2 = (p.Y + (pointerRadius_ * 0.6!))
          num3 = (pointerRadius_ * 1.85!)
          Using path2 As New GraphicsPath
            path2.AddEllipse(CSng((num - num3)), CSng((num2 - num3)), CSng((num3 * 2.0!)), CSng((num3 * 2.0!)))
            Using brush2 As New PathGradientBrush(path2)
              brush2.CenterColor = dialColor_
              colorArray2 = New Color() {GraphicsUtils.ScaleColor(dialColor_, 0.5!)}
              colorArray = colorArray2
              brush2.SurroundColors = colorArray
              Dim container As GraphicsContainer = g.BeginContainer
              g.SmoothingMode = SmoothingMode.AntiAlias
              g.FillEllipse(brush, rectangle2)
              g.FillEllipse(brush2, rect)
              Dim num4 As Single = ((CSng(totalAngle_) / 2.0!) - angle)
              Dim num5 As Double = (p.X - (Math.Sin(((num4 / 180.0!) * Math.PI)) * pointerRadius_))
              Dim num6 As Double = (p.Y - (Math.Cos(((num4 / 180.0!) * Math.PI)) * pointerRadius_))
              Dim tf As New PointF(CSng(num5), CSng(num6))
              Using pen As New Pen(pointerColor_, 2.0!)
                g.DrawLine(pen, p, tf)
              End Using
              Dim num7 As Integer = 9
              Dim i As Integer
              For i = 0 To num7 - 1
                Dim num9 As Single = (((360.0! / CSng(num7)) * i) + num4)
                num5 = (p.X - (Math.Sin(((num9 / 180.0!) * Math.PI)) * pointerRadius_))
                num6 = (p.Y - (Math.Cos(((num9 / 180.0!) * Math.PI)) * pointerRadius_))
                Dim tf2 As New PointF(CSng(num5), CSng(num6))
                num5 = (p.X - (Math.Sin(((num9 / 180.0!) * Math.PI)) * dialRadius_))
                num6 = (p.Y - (Math.Cos(((num9 / 180.0!) * Math.PI)) * dialRadius_))
                Dim tf3 As New PointF(CSng(num5), CSng(num6))
                g.DrawLine(Pens.Gray, tf2, tf3)
              Next i
              g.EndContainer(container)
            End Using
          End Using
        End Using
      End Using
    End Sub

    ' If the Text changes, then a re-draw will be needed
    Protected Overrides Sub OnTextChanged(ByVal e As EventArgs)
      Invalidate() : MyBase.OnTextChanged(e)
    End Sub

    Protected Overrides Function PointToValue(ByVal p As Point) As Double
      Dim center As Point = Me.Center, xDif As Integer = center.X - p.X, yDif As Integer = center.Y - p.Y

      Dim angle As Double
      If yDif = 0 Then
        If xDif > 0 Then
          angle = 90
        Else
          angle = -90
        End If
      Else
        Dim num4 As Double = (Math.Atan(xDif / yDif) * 180) / Math.PI
        If yDif > 0 Then
          angle = num4
        Else
          If xDif > 0 Then
            angle = num4 + 180
          Else
            angle = num4 - 180
          End If
        End If
      End If

      Dim fraction As Double = ((totalAngle_ / 2) - angle) / totalAngle_
      If fraction <= 0 Then Return Minimum
      If fraction >= 1 Then Return Maximum
      Return Minimum + CInt((Maximum - Minimum) * fraction)
    End Function


    ' Properties
    Private ReadOnly Property Center() As Point
      Get
        With DisplayRectangle
          Return New Point((.Left + .Right) \ 2, (.Top + .Bottom) \ 2)
        End With
      End Get
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(128, 128)
      End Get
    End Property

    <Description("Color of the dial"), DefaultValue(GetType(Color), "White"), Category("Appearance")> _
    Public Property DialColor() As Color
      Get
        Return dialColor_
      End Get
      Set(ByVal value As Color)
        If dialColor_ <> value Then dialColor_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(30), Description("Radius of dal")> _
    Public Property DialRadius() As Integer
      Get
        Return dialRadius_
      End Get
      Set(ByVal value As Integer)
        If dialRadius_ <> value Then dialRadius_ = value : Invalidate()
      End Set
    End Property

    Protected Overrides ReadOnly Property Editable() As Boolean
      Get
        Return True
      End Get
    End Property

    <Description("Define frequency of large ticks"), DefaultValue(10), Category("Appearance")> _
    Public Property LargeTickFrequency() As Integer
      Get
        Return largeTickFrequency_
      End Get
      Set(ByVal value As Integer)
        If largeTickFrequency_ <> value Then largeTickFrequency_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(GetType(Color), "Red"), Category("Appearance"), Description("Color of the pointer")> _
    Public Property PointerColor() As Color
      Get
        Return pointerColor_
      End Get
      Set(ByVal value As Color)
        If pointerColor_ <> value Then pointerColor_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(20), Description("Radius of slider")> _
    Public Property PointerRadius() As Integer
      Get
        Return pointerRadius_
      End Get
      Set(ByVal value As Integer)
        If pointerRadius_ <> value Then pointerRadius_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Indicate if the control paints dial numbers"), DefaultValue(True)> _
    Public Property ShowNumbers() As Boolean
      Get
        Return showNumbers_
      End Get
      Set(ByVal value As Boolean)
        If showNumbers_ <> value Then showNumbers_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(True), Description("Indicate if the control paints dial ticks"), Category("Appearance")> _
    Public Property ShowTicks() As Boolean
      Get
        Return showTicks_
      End Get
      Set(ByVal value As Boolean)
        If showTicks_ <> value Then showTicks_ = value : Invalidate()
      End Set
    End Property

    <Description("Define frequency of small ticks"), DefaultValue(2), Category("Appearance")> _
    Public Property SmallTickFrequency() As Integer
      Get
        Return smallTickFrequency_
      End Get
      Set(ByVal value As Integer)
        If smallTickFrequency_ <> value Then smallTickFrequency_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(270), Category("Appearance"), Description("Angle behind minimum and maximum positions")> _
    Public Property TotalAngle() As Integer
      Get
        Return totalAngle_
      End Get
      Set(ByVal value As Integer)
        If totalAngle_ <> value Then totalAngle_ = value : Invalidate()
      End Set
    End Property
  End Class


  <DefaultBindingProperty("Text")> _
  Public Class DigitalLED : Inherits BorderTransparentControl
    Private digits_ As New DigitsDisplay

    Public Sub New()
      BackColor = Color.Black
      Text = "0123456789"
    End Sub

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      Dim rectangle1 As Rectangle = DisplayRectangle
      MyBase.OnDraw(e)
      Dim point1 As New Point(rectangle1.Left + 5, _
          rectangle1.Top + rectangle1.Height \ 2 - digits_.DigitHeight \ 2)
      digits_.PaintString(e.Graphics, Point.op_Implicit(point1), Text)
    End Sub

    Protected Overrides Sub OnTextChanged(ByVal e As EventArgs)
      MyBase.OnTextChanged(e)
      Invalidate()
    End Sub

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(168, 48)
      End Get
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance"), Description("Segment digits")> _
    Public Property Digits() As DigitsDisplay
      Get
        Return digits_
      End Get
      Set(ByVal value As DigitsDisplay)
        If Not digits_ Is value Then digits_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(GetType(Color), "Black")> _
    Public Overrides Property BackColor() As Color
      Get
        Return MyBase.BackColor
      End Get
      Set(ByVal value As Color)
        MyBase.BackColor = value
      End Set
    End Property
  End Class

  Public Class DigitsDisplay
    Private activeColor_ As Color = Color.Lime, inactiveColor_ As Color = Color.DarkGreen
    Private antiAlias_ As Boolean
    Private Shared charSegments_ As New Hashtable
    Private digitHeight_ As Integer = 24, digitSpace_ As Integer = 3, digitWidth_ As Integer = 12
    Private segmentPaths_ As GraphicsPath()
    Private segmentSpace_ As Integer = 1, segmentThickness_ As Integer = 2

    Shared Sub New()
      With charSegments_
        .Add("0"c, 63)
        .Add("1"c, 40)
        .Add("2"c, 91)
        .Add("3"c, 107)
        .Add("4"c, 108)
        .Add("5"c, 103)
        .Add("6"c, 119)
        .Add("7"c, 41)
        .Add("8"c, 127)
        .Add("9"c, 111)
        .Add(" "c, 0)
        .Add("-"c, 64)
      End With
    End Sub

    Private Sub ClearRegions()
      segmentPaths_ = Nothing
    End Sub

    Public Sub PaintString(ByVal g As Graphics, ByVal pos As PointF, ByVal [text] As String)
      CalculateRegions()
      Using brush1 As New SolidBrush(activeColor_)
        Dim container1 As GraphicsContainer = g.BeginContainer
        g.TranslateTransform(pos.X, pos.Y)
        If antiAlias_ Then g.SmoothingMode = SmoothingMode.HighQuality
        For Each ch1 As Char In text
          If DigitsDisplay.charSegments_.ContainsKey(ch1) Then
            PaintDigit(g, pos, CType(DigitsDisplay.charSegments_.Item(ch1), Segments))
            g.TranslateTransform(CType((digitSpace_ + digitWidth_), Single), 0.0!)
            Continue For
          End If
          If ch1 = ":"c Then
            g.FillRectangle(brush1, CType(0.0!, Single), CType((CType(digitHeight_, Single) * 0.3!), Single), CType(segmentThickness_, Single), CType(segmentThickness_, Single))
            g.FillRectangle(brush1, CType(0.0!, Single), CType((CType(digitHeight_, Single) * 0.7!), Single), CType(segmentThickness_, Single), CType(segmentThickness_, Single))
            g.TranslateTransform(CType((digitSpace_ + segmentThickness_), Single), 0.0!)
            Continue For
          End If
          If ch1 = "."c OrElse ch1 = ","c Then
            g.FillRectangle(brush1, CType(0, Integer), digitHeight_, segmentThickness_, segmentThickness_)
            g.TranslateTransform(CType((digitSpace_ + segmentThickness_), Single), 0.0!)
          End If
        Next ch1
        g.EndContainer(container1)
      End Using
    End Sub

    Private Sub CalculateRegions()
      Dim single1 As Single
      Dim single2 As Single
      Dim tfArray1 As PointF()
      If (segmentPaths_ Is Nothing) Then
        segmentPaths_ = New GraphicsPath(8 - 1) {}
        single1 = CType((digitHeight_ / 2), Single)
        single2 = CType((segmentThickness_ / 2), Single)
        tfArray1 = New PointF() { _
                     New PointF(CType(segmentSpace_, Single), 0.0!), _
                     New PointF(CType((digitWidth_ - segmentSpace_), Single), 0.0!), _
                     New PointF(CType(((digitWidth_ - segmentSpace_) - segmentThickness_), Single), CType(segmentThickness_, Single)), _
                     New PointF(CType((segmentSpace_ + segmentThickness_), Single), CType(segmentThickness_, Single))}
        AddSegmentRegion(0, tfArray1)

        tfArray1 = New PointF() { _
                     New PointF(CType(segmentSpace_, Single), CType(digitHeight_, Single)), _
                     New PointF(CType((digitWidth_ - segmentSpace_), Single), CType(digitHeight_, Single)), _
                     New PointF(CType(((digitWidth_ - segmentSpace_) - segmentThickness_), Single), CType((digitHeight_ - segmentThickness_), Single)), _
                     New PointF(CType((segmentSpace_ + segmentThickness_), Single), CType((digitHeight_ - segmentThickness_), Single))}
        AddSegmentRegion(1, tfArray1)

        tfArray1 = New PointF() { _
                     New PointF(0.0!, CType(segmentSpace_, Single)), _
                     New PointF(0.0!, (single1 - CType(segmentSpace_, Single))), _
                     New PointF(CType(segmentThickness_, Single), ((single1 - CType(segmentSpace_, Single)) - CType(segmentThickness_, Single))), _
                     New PointF(CType(segmentThickness_, Single), CType((segmentSpace_ + segmentThickness_), Single))}
        AddSegmentRegion(2, tfArray1)

        tfArray1 = New PointF() { _
                     New PointF(CType(digitWidth_, Single), CType(segmentSpace_, Single)), _
                     New PointF(CType(digitWidth_, Single), (single1 - CType(segmentSpace_, Single))), _
                     New PointF(CType((digitWidth_ - segmentThickness_), Single), ((single1 - CType(segmentSpace_, Single)) - CType(segmentThickness_, Single))), _
                     New PointF(CType((digitWidth_ - segmentThickness_), Single), CType((segmentSpace_ + segmentThickness_), Single))}
        AddSegmentRegion(3, tfArray1)

        tfArray1 = New PointF() { _
                     New PointF(0.0!, CType((digitHeight_ - segmentSpace_), Single)), _
                     New PointF(0.0!, ((CType(digitHeight_, Single) - single1) + CType(segmentSpace_, Single))), _
                     New PointF(CType(segmentThickness_, Single), (((CType(digitHeight_, Single) - single1) + CType(segmentSpace_, Single)) + CType(segmentThickness_, Single))), _
                     New PointF(CType(segmentThickness_, Single), CType(((digitHeight_ - segmentSpace_) - segmentThickness_), Single))}
        AddSegmentRegion(4, tfArray1)

        tfArray1 = New PointF() { _
                     New PointF(CType(digitWidth_, Single), CType((digitHeight_ - segmentSpace_), Single)), _
                     New PointF(CType(digitWidth_, Single), ((CType(digitHeight_, Single) - single1) + CType(segmentSpace_, Single))), _
                     New PointF(CType((digitWidth_ - segmentThickness_), Single), (((CType(digitHeight_, Single) - single1) + CType(segmentSpace_, Single)) + CType(segmentThickness_, Single))), _
                     New PointF(CType((digitWidth_ - segmentThickness_), Single), CType(((digitHeight_ - segmentSpace_) - segmentThickness_), Single))}
        AddSegmentRegion(5, tfArray1)

        tfArray1 = New PointF() { _
                     New PointF(CType(segmentSpace_, Single), single1), _
                     New PointF((CType(segmentSpace_, Single) + single2), (single1 - single2)), _
                     New PointF((CType((digitWidth_ - segmentSpace_), Single) - single2), (single1 - single2)), _
                     New PointF(CType((digitWidth_ - segmentSpace_), Single), single1), _
                     New PointF((CType((digitWidth_ - segmentSpace_), Single) - single2), (single1 + single2)), _
                     New PointF((CType(segmentSpace_, Single) + single2), (single1 + single2))}
        AddSegmentRegion(6, tfArray1)
      End If
    End Sub

    Private Sub AddSegmentRegion(ByVal num As Integer, ByVal path As PointF())
      Dim path1 As New GraphicsPath
      path1.StartFigure()
      path1.AddLines(path)
      path1.CloseFigure()
      segmentPaths_(num) = path1
    End Sub



    Private Sub PaintDigit(ByVal g As Graphics, ByVal pos As PointF, ByVal digit As Segments)
      Dim brush3 As Brush
      Using brush1 As New SolidBrush(activeColor_), brush2 As New SolidBrush(inactiveColor_)
        Dim num1 As Integer = CType(digit, Integer)
        Dim num2 As Integer
        Do While num2 < 7
          If segmentPaths_(num2) IsNot Nothing Then
            If (num1 And 1) = 1 Then
              brush3 = brush1
            Else
              brush3 = brush2
            End If
            g.FillPath(brush3, segmentPaths_(num2))
            num1 >>= 1
          End If
          num2 += 1
        Loop
      End Using
    End Sub


    <Description("Active led color"), DefaultValue(GetType(Color), "Lime"), Category("Appearance")> _
    Public Property ActiveColor() As Color
      Get
        Return activeColor_
      End Get
      Set(ByVal value As Color)
        If Not activeColor_.Equals(value) Then activeColor_ = value
      End Set
    End Property

    <Description("Switch antialiasing mode for drawing digits"), DefaultValue(False), Category("Appearance")> _
    Public Property AntiAlias() As Boolean
      Get
        Return antiAlias_
      End Get
      Set(ByVal value As Boolean)
        If antiAlias_ <> value Then antiAlias_ = value
      End Set
    End Property

    <DefaultValue(24), Description("Height of partial digit"), Category("Appearance")> _
    Public Property DigitHeight() As Integer
      Get
        Return digitHeight_
      End Get
      Set(ByVal value As Integer)
        If digitHeight_ <> value Then digitHeight_ = value : ClearRegions()
      End Set
    End Property

    <DefaultValue(3), Category("Appearance"), Description("Space behind neightbours digits")> _
    Public Property DigitSpace() As Integer
      Get
        Return digitSpace_
      End Get
      Set(ByVal value As Integer)
        If digitSpace_ <> value Then digitSpace_ = value
      End Set
    End Property

    <Category("Appearance"), DefaultValue(12), Description("Width of partial digit")> _
    Public Property DigitWidth() As Integer
      Get
        Return digitWidth_
      End Get
      Set(ByVal value As Integer)
        If digitWidth_ <> value Then digitWidth_ = value : ClearRegions()
      End Set
    End Property

    <DefaultValue(GetType(Color), "DarkGreen"), Category("Appearance"), Description("Ininactive led color")> _
    Public Property InactiveColor() As Color
      Get
        Return inactiveColor_
      End Get
      Set(ByVal value As Color)
        If Not inactiveColor_.Equals(value) Then inactiveColor_ = value
      End Set
    End Property

    <DefaultValue(1), Category("Appearance"), Description("Space behind led segments")> _
    Public Property SegmentSpace() As Integer
      Get
        Return segmentSpace_
      End Get
      Set(ByVal value As Integer)
        If segmentSpace_ <> value Then segmentSpace_ = value : ClearRegions()
      End Set
    End Property

    <DefaultValue(2), Category("Appearance"), Description("Led segment thickness")> _
    Public Property SegmentThickness() As Integer
      Get
        Return segmentThickness_
      End Get
      Set(ByVal value As Integer)
        If segmentThickness_ <> value Then segmentThickness_ = value : ClearRegions()
      End Set
    End Property

    Private Enum Segments
      None = 0
      Top = 1
      Bottom = 2
      LeftTop = 4
      RightTop = 8
      LeftBottom = 16
      RightBottom = 32
      Middle = 64
      All = 127
    End Enum
  End Class


  Friend NotInheritable Class GraphicsUtils
    Private Sub New()
    End Sub

    Public Shared Function TransparentColor(ByVal sourceColor As Color, ByVal alpha As Single) As Color
      Return Color.FromArgb(CInt(alpha * 255), sourceColor.R, sourceColor.G, sourceColor.B)
    End Function

    Public Shared Function ScaleColor(ByVal sourceColor As Color, ByVal scale As Single) As Color
      Dim red As Integer = CType((CType(sourceColor.R, Single) * scale), Integer)
      Dim green As Integer = CType((CType(sourceColor.G, Single) * scale), Integer)
      Dim blue As Integer = CType((CType(sourceColor.B, Single) * scale), Integer)
      If red > 255 Then red = 255
      If green > 255 Then green = 255
      If blue > 255 Then blue = 255
      Return Color.FromArgb(red, green, blue)
    End Function
  End Class


  Public MustInherit Class BorderTransparentControl : Inherits Control
    Private border_ As New Border(BorderStyle.None, False, False), _
            transparentBackColor_ As Boolean = True, _
            requestedSize_ As Size
    Protected inNew_ As Integer

    Protected Sub New()
      inNew_ += 1
      SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, True)
      SetStyle(ControlStyles.Selectable, False)
      ForeColor = Color.Black
      inNew_ -= 1
    End Sub

    ' Don't normally show this - but can be overridden again
    <Bindable(False), Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
      Public Overrides Property Text() As String
      Get
        Return MyBase.Text
      End Get
      Set(ByVal value As String)
        MyBase.Text = value
      End Set
    End Property

    Protected Overridable Sub OnDraw(ByVal e As PaintEventArgs)
      ' Draw the border
      Dim drc As Rectangle = ClientRectangle
      border_.Draw(e.Graphics, New Rectangle(drc.Left, drc.Top, drc.Width - 1, drc.Height - 1))
    End Sub

    Protected Overridable Sub OnDrawFocus(ByVal e As PaintEventArgs)
      Dim margin As Integer = border_.GetMargin, rc As Rectangle = ClientRectangle
      rc.Width -= 1 : rc.Height -= 1
      ControlPaint.DrawFocusRectangle(e.Graphics, rc)
      Exit Sub
      If Margin = 0 Then
        Using pen As New Pen(Color.Black)
          pen.DashStyle = DashStyle.Dot
          e.Graphics.DrawRectangle(pen, rc.Left, rc.Top, rc.Width - 20, rc.Height - 20)
        End Using
      Else
        e.Graphics.DrawRectangle(Pens.LightGreen, rc.Left - 1, rc.Top - 1, rc.Width + 1, rc.Height + 1)
      End If
    End Sub

    Protected Overrides Sub OnGotFocus(ByVal e As EventArgs)
      MyBase.OnGotFocus(e)
      Invalidate()
    End Sub
    Protected Overrides Sub OnLostFocus(ByVal e As System.EventArgs)
      MyBase.OnLostFocus(e)
      Invalidate()
    End Sub

    <DefaultValue(GetType(Color), "Transparent")> _
    Public Overrides Property BackColor() As Color
      Get
        If transparentBackColor_ Then Return Color.Transparent
        Return MyBase.BackColor
      End Get
      Set(ByVal value As Color)
        If BackColor = value Then Exit Property ' no change
        If value = Color.Transparent Then
          transparentBackColor_ = True
          DoubleBuffered = False
          RecreateHandle()
        Else
          MyBase.BackColor = value
          If transparentBackColor_ Then
            transparentBackColor_ = False
            DoubleBuffered = True
            RecreateHandle()
          End If
        End If
      End Set
    End Property

#If 0 Then
    <DefaultValue(GetType(Color), "Black")> _
    Public Overrides Property ForeColor() As Color
      Get
        Return MyBase.ForeColor
      End Get
      Set(ByVal value As Color)
        MyBase.ForeColor = value
      End Set
    End Property
#End If

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property BackgroundImage() As Image
      Get
        Return MyBase.BackgroundImage
      End Get
      Set(ByVal value As Image)
        MyBase.BackgroundImage = value
      End Set
    End Property
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property BackgroundImageLayout() As ImageLayout
      Get
        Return MyBase.BackgroundImageLayout
      End Get
      Set(ByVal value As ImageLayout)
        MyBase.BackgroundImageLayout = value
      End Set
    End Property

    Protected Overrides ReadOnly Property CreateParams() As CreateParams
      Get
        Dim cp As CreateParams = MyBase.CreateParams
        If transparentBackColor_ Then
          Const WS_EX_TRANSPARENT As Int32 = &H20
          cp.ExStyle = cp.ExStyle Or WS_EX_TRANSPARENT
        End If
        Return cp
      End Get
    End Property

    Protected Overrides Sub OnPaintBackground(ByVal pevent As PaintEventArgs)
    End Sub
    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
      If e.ClipRectangle.IsEmpty Then Exit Sub
      If Not transparentBackColor_ Then
        Using backBrush As New SolidBrush(BackColor)
          e.Graphics.FillRectangle(backBrush, e.ClipRectangle)
        End Using
      End If
      OnDraw(e)
      If Focused Then OnDrawFocus(e)
    End Sub


    <Category("Appearance"), Description("Control's Border"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
    Public Property Border() As Border
      Get
        Return border_
      End Get
      Set(ByVal value As Border)
        border_ = value : Invalidate()
      End Set
    End Property

    Private Function ShouldSerializeBorder() As Boolean
      Return border_.Style <> BorderStyle.None
    End Function

    ' The DisplayRectangle has any border removed
    Public Overrides ReadOnly Property DisplayRectangle() As Rectangle
      Get
        Dim ret As Rectangle = MyBase.DisplayRectangle, margin As Integer = border_.GetMargin
        ret.Inflate(-margin, -margin)
        Return ret
      End Get
    End Property

    Public Overloads Sub Invalidate()
      If transparentBackColor_ Then
        ' We must invalidate part of our parent as well
        Dim par As Control = Parent
        If par IsNot Nothing Then
          Dim pan As Panel = TryCast(par, Panel)
          If pan IsNot Nothing Then
            pan.Invalidate(Bounds)
          Else
            par.Invalidate(Bounds, True)
          End If
        End If
      End If
      MyBase.Invalidate()
    End Sub

    ' Ensure all these go through our changed Invalidate() above
    Protected Overrides Sub OnFontChanged(ByVal e As EventArgs)
      Invalidate()
      AdjustSize()
      MyBase.OnFontChanged(e)
    End Sub
    Protected Overrides Sub OnForeColorChanged(ByVal e As EventArgs)
      If inNew_ <> 0 Then Exit Sub
      Invalidate()
      MyBase.OnForeColorChanged(e)
    End Sub
    Protected Overrides Sub OnTextChanged(ByVal e As EventArgs)
      Invalidate()
      MyBase.OnTextChanged(e)
    End Sub

    Protected Sub AdjustSize()
      If (AutoSize OrElse (((Anchor And (AnchorStyles.Right Or AnchorStyles.Left)) <> (AnchorStyles.Right Or AnchorStyles.Left)) AndAlso _
         ((Anchor And (AnchorStyles.Bottom Or AnchorStyles.Top)) <> (AnchorStyles.Bottom Or AnchorStyles.Top)))) Then
        Dim height As Integer = requestedSize_.Height
        Dim width As Integer = requestedSize_.Width
        Try
          Dim size As Size
          If AutoSize Then
            size = PreferredSize
          Else
            size = New Size(width, height)
          End If
          If width <> 0 AndAlso height <> 0 Then MyBase.Size = size
        Finally
          requestedSize_.Height = height
          requestedSize_.Width = width
        End Try
      End If
    End Sub

    Protected Overrides Sub SetBoundsCore(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal specified As BoundsSpecified)
      If (specified And BoundsSpecified.Height) <> 0 Then requestedSize_.Height = height
      If (specified And BoundsSpecified.Width) <> 0 Then requestedSize_.Width = width
      If AutoSize Then With MyBase.PreferredSize : width = .Width : height = .Height : End With
      MyBase.SetBoundsCore(x, y, width, height, specified)
    End Sub

    Protected Sub DrawAlignedText(ByVal graphics As Graphics, ByVal str As String, ByVal textAlign As ContentAlignment)
      Dim flags As TextFormatFlags
      If (textAlign And (ContentAlignment.BottomCenter Or ContentAlignment.BottomLeft Or ContentAlignment.BottomRight)) <> 0 Then
        flags = TextFormatFlags.Bottom
      ElseIf (textAlign And (ContentAlignment.MiddleCenter Or ContentAlignment.MiddleLeft Or ContentAlignment.MiddleRight)) <> 0 Then
        flags = TextFormatFlags.VerticalCenter
      End If
      If (textAlign And (ContentAlignment.BottomRight Or ContentAlignment.MiddleRight Or ContentAlignment.TopRight)) <> 0 Then
        flags = flags Or TextFormatFlags.Right
      ElseIf (textAlign And (ContentAlignment.BottomCenter Or ContentAlignment.MiddleCenter Or ContentAlignment.TopCenter)) <> 0 Then
        flags = flags Or TextFormatFlags.HorizontalCenter
      End If
      flags = flags Or TextFormatFlags.NoPrefix Or TextFormatFlags.NoPadding

      TextRenderer.DrawText(graphics, str, Font, DisplayRectangle, ForeColor, flags)
    End Sub

    Protected Function GetPreferredTextSize(ByVal str As String, ByVal proposedSize As Size) As Size
      If str.Length = 0 Then str = "0"
      Return TextRenderer.MeasureText(str, Font, proposedSize, TextFormatFlags.NoPrefix Or TextFormatFlags.NoPadding)
    End Function
  End Class

  Public Class Knob : Inherits BoundedValueControl
    Private activeLedColor_ As Color = Color.LightGreen, dialColor_ As Color = Color.White, _
            dialRadius_ As Integer = 30, inactiveLedColor_ As Color = Color.Green, _
            largeTickFrequency_ As Integer = 10, showNumbers_ As Boolean = True, _
            showTicks_ As Boolean = True, showValue_ As Boolean, smallTickFrequency_ As Integer = 2, _
            totalAngle_ As Integer = 270

    Public Sub New()
      SetStyle(ControlStyles.Selectable, True)
    End Sub

    ' Want this back
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
      Public Overrides Property Text() As String
      Get
        Return MyBase.Text
      End Get
      Set(ByVal value As String)
        MyBase.Text = value
      End Set
    End Property

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      Dim g As Graphics = e.Graphics
      Using pen As New Pen(ForeColor), brush As New SolidBrush(ForeColor)
        Dim center As Point = Me.Center
        Dim num As Single = (CSng((Value - Minimum)) / (Maximum - Minimum))
        Dim angle As Single = (num * totalAngle_)
        DrawDial(g, center, angle)
        Dim num3 As Integer = (dialRadius_ + 2)
        Dim num4 As Integer = (num3 + 3)
        Dim num5 As Integer = (num4 + 5)
        If showTicks_ Then
          Dim i As Integer = Minimum
          Do While (i <= Maximum)
            num = (CSng((i - Minimum)) / (Maximum - Minimum))
            Dim num7 As Single = (num * totalAngle_)
            Dim num8 As Single = ((CSng(totalAngle_) / 2.0!) - num7)
            Dim num9 As Double = (center.X - (Math.Sin(((num8 / 180.0!) * 3.1415926535897931)) * num3))
            Dim num10 As Double = (center.Y - (Math.Cos(((num8 / 180.0!) * 3.1415926535897931)) * num3))
            Dim num11 As Double = num4
            If ((i Mod largeTickFrequency_) = 0) Then
              num11 = num5
              If showNumbers_ Then
                Dim num12 As Integer = (num5 + Font.Height)
                Dim num13 As Double = (center.X - (Math.Sin(((num8 / 180.0!) * 3.1415926535897931)) * num12))
                Dim num14 As Double = (center.Y - (Math.Cos(((num8 / 180.0!) * 3.1415926535897931)) * num12))
                Dim text As String = ScaleAndFormatValue(i)
                Dim ef As SizeF = g.MeasureString([text], Font)
                num13 = (num13 - (ef.Width / 2.0!))
                num14 = (num14 - (ef.Height / 2.0!))
                g.DrawString([text], Font, brush, CInt(num13), CInt(num14))
              End If
            End If
            Dim num15 As Double = (center.X - (Math.Sin(((num8 / 180.0!) * 3.1415926535897931)) * num11))
            Dim num16 As Double = (center.Y - (Math.Cos(((num8 / 180.0!) * 3.1415926535897931)) * num11))
            g.DrawLine(pen, CInt(num9), CInt(num10), CInt(num15), CInt(num16))
            i = (i + smallTickFrequency_)
          Loop
        End If
        g.DrawString(Text, Font, brush, center.X - CInt(g.MeasureString(Text, Font).Width) \ 2, center.Y + num5 + Font.Height \ 2)
      End Using
    End Sub

    Private Sub DrawDial(ByVal g As Graphics, ByVal p As Point, ByVal angle As Single)
      Dim rect As New Rectangle((p.X - dialRadius_), (p.Y - dialRadius_), (dialRadius_ * 2), (dialRadius_ * 2))
      Dim num As Single = (p.X - (dialRadius_ * 0.6!))
      Dim num2 As Single = (p.Y - (dialRadius_ * 0.6!))
      Dim num3 As Single = (dialRadius_ * 1.85!)
      Using path As New GraphicsPath
        path.AddEllipse(CSng((num - num3)), CSng((num2 - num3)), CSng((num3 * 2.0!)), CSng((num3 * 2.0!)))
        Using brush As New PathGradientBrush(path)
          brush.CenterColor = dialColor_
          Dim colorArray As Color() = New Color() {GraphicsUtils.ScaleColor(dialColor_, 0.4!)}
          brush.SurroundColors = colorArray
          Dim container As GraphicsContainer = g.BeginContainer
          g.SmoothingMode = SmoothingMode.AntiAlias
          g.FillEllipse(brush, rect)
          DrawPointer(g, p, angle)
          If showValue_ Then
            Dim format As New StringFormat
            format.Alignment = StringAlignment.Center
            format.LineAlignment = StringAlignment.Center
            Using foreBrush As New SolidBrush(ForeColor)
              g.DrawString(ScaleAndFormatValue(Value), Font, foreBrush, rect, format)
            End Using
          End If
          g.EndContainer(container)
        End Using
      End Using
    End Sub

    Private Sub DrawPointer(ByVal g As Graphics, ByVal p As Point, ByVal angle As Single)
      Dim num As Single = ((CSng(totalAngle_) / 2.0!) - angle)
      Dim num2 As Integer = (dialRadius_ - 6)
      Dim num3 As Double = (p.X - (Math.Sin(((num / 180.0!) * 3.1415926535897931)) * num2))
      Dim num4 As Double = (p.Y - (Math.Cos(((num / 180.0!) * 3.1415926535897931)) * num2))
      Dim rect As New Rectangle((CInt(num3) - 2), (CInt(num4) - 2), 5, 5)

      Dim brush As Brush
      If Capture Then
        brush = New SolidBrush(activeLedColor_)
      Else
        brush = New SolidBrush(inactiveLedColor_)
      End If
      g.FillEllipse(brush, rect)
      brush.Dispose()
    End Sub

    Protected Overrides Sub OnTextChanged(ByVal e As EventArgs)
      Invalidate()
      MyBase.OnTextChanged(e)
    End Sub

    Protected Overrides Function PointToValue(ByVal p As Point) As Double
      Dim center As Point = Me.Center, xDif As Integer = center.X - p.X, yDif As Integer = center.Y - p.Y

      Dim angle As Double
      If yDif = 0 Then
        If xDif > 0 Then
          angle = 90
        Else
          angle = -90
        End If
      Else
        Dim num4 As Double = (Math.Atan(xDif / yDif) * 180) / Math.PI
        If yDif > 0 Then
          angle = num4
        Else
          If xDif > 0 Then
            angle = num4 + 180
          Else
            angle = num4 - 180
          End If
        End If
      End If

      Dim fraction As Double = ((totalAngle_ / 2) - angle) / totalAngle_
      If fraction <= 0 Then Return Minimum
      If fraction >= 1 Then Return Maximum
      Return Minimum + CInt((Maximum - Minimum) * fraction)
    End Function



    ' Properties
    <Category("Appearance"), Description("Active led color"), DefaultValue(GetType(Color), "LightGreen")> _
    Public Property ActiveLedColor() As Color
      Get
        Return activeLedColor_
      End Get
      Set(ByVal value As Color)
        If value <> activeLedColor_ Then activeLedColor_ = value : Invalidate()
      End Set
    End Property

    Private ReadOnly Property Center() As Point
      Get
        With DisplayRectangle
          Return New Point((.Left + .Right) \ 2, (.Top + .Bottom) \ 2)
        End With
      End Get
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(128, 128)
      End Get
    End Property

    <DefaultValue(GetType(Color), "White"), Description("Color of the dial"), Category("Appearance")> _
    Public Property DialColor() As Color
      Get
        Return dialColor_
      End Get
      Set(ByVal value As Color)
        If dialColor_ <> value Then dialColor_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(30), Category("Appearance"), Description("Radius of dial")> _
    Public Property DialRadius() As Integer
      Get
        Return dialRadius_
      End Get
      Set(ByVal value As Integer)
        If dialRadius_ <> value Then dialRadius_ = value : Invalidate()
      End Set
    End Property

    Protected Overrides ReadOnly Property Editable() As Boolean
      Get
        Return True
      End Get
    End Property

    <DefaultValue(GetType(Color), "Green"), Description("Inactive led color"), Category("Appearance")> _
    Public Property InactiveLedColor() As Color
      Get
        Return inactiveLedColor_
      End Get
      Set(ByVal value As Color)
        If value <> inactiveLedColor_ Then inactiveLedColor_ = value : Invalidate()
      End Set
    End Property

    <Description("Define frequency of large ticks"), Category("Appearance"), DefaultValue(10)> _
    Public Property LargeTickFrequency() As Integer
      Get
        Return largeTickFrequency_
      End Get
      Set(ByVal value As Integer)
        If largeTickFrequency_ <> value Then largeTickFrequency_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Indicate if the control paints dial numbers"), DefaultValue(True)> _
    Public Property ShowNumbers() As Boolean
      Get
        Return showNumbers_
      End Get
      Set(ByVal value As Boolean)
        If showNumbers_ <> value Then showNumbers_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(True), Category("Appearance"), Description("Indicate if the control paints dial ticks")> _
    Public Property ShowTicks() As Boolean
      Get
        Return showTicks_
      End Get
      Set(ByVal value As Boolean)
        If showTicks_ <> value Then showTicks_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(False), Category("Appearance"), Description("Indicate if the control paints current value")> _
    Public Property ShowValue() As Boolean
      Get
        Return showValue_
      End Get
      Set(ByVal value As Boolean)
        If showValue_ <> value Then showValue_ = value : Invalidate()
      End Set
    End Property

    <Description("Define frequency of small ticks"), Category("Appearance"), DefaultValue(2)> _
    Public Property SmallTickFrequency() As Integer
      Get
        Return smallTickFrequency_
      End Get
      Set(ByVal value As Integer)
        If smallTickFrequency_ <> value Then smallTickFrequency_ = value : Invalidate()
      End Set
    End Property

    <Description("Angle behind minimum and maximum positions"), Category("Appearance"), DefaultValue(270)> _
    Public Property TotalAngle() As Integer
      Get
        Return totalAngle_
      End Get
      Set(ByVal value As Integer)
        If totalAngle_ <> value Then totalAngle_ = value : Invalidate()
      End Set
    End Property
  End Class

  Public Class Level : Inherits SliderBase
    Private endColor_ As Color = Color.Red, middleColor_ As Color = Color.Yellow, _
            startColor_ As Color = Color.Lime

    Private Function CreateColorsGradient(ByVal gradRect As Rectangle, ByVal k As Single) As LinearGradientBrush
      Dim color As Color = GraphicsUtils.ScaleColor(startColor_, k)
      Dim color2 As Color = GraphicsUtils.ScaleColor(middleColor_, k)
      Dim color3 As Color = GraphicsUtils.ScaleColor(endColor_, k)
      Dim num As Integer

      If Orientation = Orientation.Horizontal Then
        num = 180
      Else
        num = 90
      End If
      Dim ret As New LinearGradientBrush(gradRect, color3, color, CSng(num))
      Dim blend As New ColorBlend(6)
      blend.Positions(0) = 0.0!
      blend.Positions(1) = 0.1!
      blend.Positions(2) = 0.3!
      blend.Positions(3) = 0.45!
      blend.Positions(4) = 0.8!
      blend.Positions(5) = 1.0!
      blend.Colors(0) = color3
      blend.Colors(1) = color3
      blend.Colors(2) = color2
      blend.Colors(3) = color2
      blend.Colors(4) = color
      blend.Colors(5) = color
      ret.InterpolationColors = blend
      Return ret
    End Function

    Private Function CreatePath(ByVal ir As Rectangle) As GraphicsPath
      Dim ret As New GraphicsPath
      If Orientation = Orientation.Horizontal Then
        ret.StartFigure()
        ret.AddLine(ir.X, ir.Y, (ir.X + ir.Width), ir.Y)
        ret.AddArc(((ir.X + ir.Width) - (ir.Height \ 2)), ir.Y, ir.Height, ir.Height, -90.0!, 180.0!)
        ret.AddLine((ir.X + ir.Width), (ir.Y + ir.Height), ir.X, (ir.Y + ir.Height))
        ret.AddArc((ir.X - (ir.Height \ 2)), ir.Y, ir.Height, ir.Height, 90.0!, 180.0!)
        ret.CloseFigure()
        Return ret
      End If
      ret.StartFigure()
      ret.AddLine(ir.X, ir.Y, ir.X, (ir.Y + ir.Height))
      ret.AddArc(ir.X, ((ir.Y - (ir.Width \ 2)) + ir.Height), ir.Width, ir.Width, 0.0!, 180.0!)
      ret.AddLine((ir.X + ir.Width), (ir.Y + ir.Height), (ir.X + ir.Width), ir.Y)
      ret.AddArc(ir.X, (ir.Y - (ir.Width \ 2)), ir.Width, ir.Width, 180.0!, 180.0!)
      ret.CloseFigure()
      Return ret
    End Function

    Private Function CreateTransparencyGradient(ByVal gradRect As Rectangle) As LinearGradientBrush
      Dim color As Color = color.FromArgb(&H80, 0, 0, 0)
      color.FromArgb(&H80, &HFF, &HFF, &HFF)
      color.FromArgb(0, 0, 0, 0)
      Dim color2 As Color = color.FromArgb(100, 0, 0, 0)
      Dim num As Integer : If Orientation = Orientation.Horizontal Then num = 90
      Dim brush As New LinearGradientBrush(gradRect, color2, color, CSng(num))
      Dim blend As New ColorBlend(4)
      blend.Positions(0) = 0.0!
      blend.Positions(1) = 0.25!
      blend.Positions(2) = 0.5!
      blend.Positions(3) = 1.0!
      blend.Colors(0) = color.FromArgb(&H80, 0, 0, 0)
      blend.Colors(1) = color.FromArgb(&H80, &HFF, &HFF, &HFF)
      blend.Colors(2) = color.FromArgb(0, 0, 0, 0)
      blend.Colors(3) = color.FromArgb(100, 0, 0, 0)
      brush.InterpolationColors = blend
      Return brush
    End Function

    Protected Overrides Sub DrawBar(ByVal g As Graphics)
      g.SmoothingMode = SmoothingMode.AntiAlias
      Dim ir As Rectangle = BarRectangle
      Using path As GraphicsPath = CreatePath(ir)
        Dim gradRect As Rectangle = ir
        If Orientation = Orientation.Horizontal Then
          gradRect.Inflate(ir.Height \ 2, 0)
        Else
          gradRect.Inflate(0, ir.Width \ 2)
        End If
        Using brush As LinearGradientBrush = CreateColorsGradient(gradRect, 0.5!)
          g.FillPath(brush, path)
        End Using
        If Value > Minimum Then
          Dim rectangle3 As Rectangle = ir
          If Orientation = Orientation.Horizontal Then
            rectangle3.Width = (ValueToPoint(Value).X - rectangle3.X)
          Else
            rectangle3.Y = ValueToPoint(Value).Y
            rectangle3.Height = ((ir.Y + ir.Height) - rectangle3.Y)
          End If
          Using path2 As GraphicsPath = CreatePath(rectangle3)
            Using brush2 As LinearGradientBrush = CreateColorsGradient(gradRect, 1.0!)
              g.FillPath(brush2, path2)
            End Using
          End Using
        End If
        Using brush3 As LinearGradientBrush = CreateTransparencyGradient(gradRect)
          g.FillPath(brush3, path)
        End Using
        g.SmoothingMode = SmoothingMode.Default
      End Using
    End Sub


    ' Properties
    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(200, 100)
      End Get
    End Property

    <Description("End gauge color"), Category("Appearance"), DefaultValue(GetType(Color), "Red")> _
    Public Property EndColor() As Color
      Get
        Return endColor_
      End Get
      Set(ByVal value As Color)
        If endColor_ <> value Then endColor_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Middle gauge color"), DefaultValue(GetType(Color), "Yellow")> _
    Public Property MiddleColor() As Color
      Get
        Return middleColor_
      End Get
      Set(ByVal value As Color)
        If middleColor_ <> value Then middleColor_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(GetType(Color), "Lime"), Category("Appearance"), Description("Start gauge color")> _
    Public Property StartColor() As Color
      Get
        Return startColor_
      End Get
      Set(ByVal value As Color)
        If startColor_ <> value Then startColor_ = value : Invalidate()
      End Set
    End Property
  End Class

  Public Class LevelBar : Inherits BoundedValueControl
    Private endColor_ As Color = Color.Red, ledHeight_ As Integer = 3, _
            middleColor_ As Color = Color.Yellow, startColor_ As Color = Color.Lime, _
            orientation_ As Orientation = Orientation.Vertical, smooth_ As Boolean = False

    Private Function CreateBlend() As ColorBlend
      Dim blend As New ColorBlend(6)
      blend.Positions(0) = 0.0!
      blend.Positions(1) = 0.1!
      blend.Positions(2) = 0.3!
      blend.Positions(3) = 0.45!
      blend.Positions(4) = 0.8!
      blend.Positions(5) = 1.0!
      Return blend
    End Function

    Private Function Darking(ByVal value As Color) As Color
      Return Color.FromArgb(value.R \ 2, value.G \ 2, value.B \ 2)
    End Function

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e) : Dim crc As Rectangle = DisplayRectangle
      Dim g As Graphics = e.Graphics

      Dim num2, num3 As Integer
      Dim num As Double = (Value - Minimum) / (Maximum - Minimum)
      If orientation_ = Orientation.Vertical Then
        num2 = CInt((num * crc.Height))
        num3 = crc.Height - num2
      Else
        num2 = CInt((num * crc.Width))
        num3 = crc.Width - num2
      End If
      Dim brush, brush2 As LinearGradientBrush
      Try
        Dim rectangle2 As Rectangle = crc : rectangle2.Inflate(1, 1)
        If orientation_ = Orientation.Vertical Then
          brush = New LinearGradientBrush(rectangle2, endColor_, startColor_, LinearGradientMode.Vertical)
        Else
          brush = New LinearGradientBrush(rectangle2, startColor_, endColor_, 180)
        End If
        Dim blend As ColorBlend = CreateBlend()
        blend.Colors(0) = endColor_
        blend.Colors(1) = endColor_
        blend.Colors(2) = middleColor_
        blend.Colors(3) = middleColor_
        blend.Colors(4) = startColor_
        blend.Colors(5) = startColor_
        brush.InterpolationColors = blend
        If orientation_ = Orientation.Vertical Then
          brush2 = New LinearGradientBrush(rectangle2, endColor_, startColor_, LinearGradientMode.Vertical)
        Else
          brush2 = New LinearGradientBrush(rectangle2, startColor_, endColor_, 180)
        End If
        blend = CreateBlend()
        blend.Colors(0) = Darking(endColor_)
        blend.Colors(1) = Darking(endColor_)
        blend.Colors(2) = Darking(middleColor_)
        blend.Colors(3) = Darking(middleColor_)
        blend.Colors(4) = Darking(startColor_)
        blend.Colors(5) = Darking(startColor_)
        brush2.InterpolationColors = blend

        If smooth_ Then
          If Value > Minimum Then
            Dim rc As Rectangle = crc
            If orientation_ = Orientation.Vertical Then
              rc.Y += num3
              rc.Height -= num3
            Else
              rc.Width -= num3
            End If
            g.FillRectangle(brush, rc)
          End If
          If Value < Maximum Then
            Dim rc As Rectangle = crc
            If orientation_ = Orientation.Vertical Then
              rc.Height -= num2
            Else
              rc.Width -= num2
              rc.X += num2
            End If
            g.FillRectangle(brush2, rc)
          End If
        Else
          g.FillRectangle(Brushes.Black, crc)
          If orientation_ = Orientation.Vertical Then
            Dim y As Integer
            Do While y < crc.Height
              Dim brush3 As Brush
              Dim height As Integer = y + ledHeight_
              If height > crc.Height Then height = crc.Height
              If (y + height + 1) \ 2 >= num2 Then
                brush3 = brush2
              Else
                brush3 = brush
              End If
              g.FillRectangle(brush3, crc.Left, crc.Bottom - height, crc.Width, height - y)
              y += ledHeight_ + 1
            Loop
          Else
            Dim x As Integer
            Do While x < crc.Width
              Dim width As Integer = x + ledHeight_
              If width > crc.Width Then width = crc.Width

              Dim fillBrush As Brush
              If (x + width + 1) \ 2 <= crc.Width - num2 Then
                fillBrush = brush2
              Else
                fillBrush = brush
              End If
              g.FillRectangle(fillBrush, crc.Right - width, crc.Top, width - x, crc.Height)
              x += ledHeight_ + 1
            Loop
          End If
        End If
      Finally
        If brush IsNot Nothing Then brush.Dispose()
        If brush2 IsNot Nothing Then brush2.Dispose()
      End Try
    End Sub


    ' Properties
    <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
    Public Overrides Property BackColor() As Color
      Get
        Return MyBase.BackColor
      End Get
      Set(ByVal value As Color)
        MyBase.BackColor = value
      End Set
    End Property

    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Overrides Property BackgroundImage() As Image
      Get
        Return MyBase.BackgroundImage
      End Get
      Set(ByVal value As Image)
        MyBase.BackgroundImage = value
      End Set
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(24, 128)
      End Get
    End Property

    <DefaultValue(GetType(Color), "Red"), Category("Appearance"), Description("End gauge color")> _
    Public Property EndColor() As Color
      Get
        Return endColor_
      End Get
      Set(ByVal value As Color)
        endColor_ = value
        Invalidate()
      End Set
    End Property

    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Overrides Property Font() As Font
      Get
        Return MyBase.Font
      End Get
      Set(ByVal value As Font)
        MyBase.Font = value
      End Set
    End Property

    <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
    Public Overrides Property ForeColor() As Color
      Get
        Return MyBase.ForeColor
      End Get
      Set(ByVal value As Color)
        MyBase.ForeColor = value
      End Set
    End Property

    <Description("Height of leds"), Category("Appearance"), DefaultValue(3)> _
    Public Property LedHeight() As Integer
      Get
        Return ledHeight_
      End Get
      Set(ByVal value As Integer)
        ledHeight_ = value
        Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "Yellow"), Description("Middle gauge color")> _
    Public Property MiddleColor() As Color
      Get
        Return middleColor_
      End Get
      Set(ByVal value As Color)
        middleColor_ = value
        Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Control's orientation"), DefaultValue(GetType(Orientation), "Vertical")> _
    Public Property Orientation() As Orientation
      Get
        Return orientation_
      End Get
      Set(ByVal value As Orientation)
        orientation_ = value
        Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(False), Description("Smooth indicator visualization style")> _
    Public Property Smooth() As Boolean
      Get
        Return smooth_
      End Get
      Set(ByVal value As Boolean)
        smooth_ = value
        Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Start gauge color"), DefaultValue(GetType(Color), "Lime")> _
    Public Property StartColor() As Color
      Get
        Return startColor_
      End Get
      Set(ByVal value As Color)
        startColor_ = value
        Invalidate()
      End Set
    End Property
  End Class


  Public Class LevelSlider : Inherits LevelBar
    Private activeLedColor_ As Color = Color.LightGreen, inactiveLedColor_ As Color = Color.Green
    Private Const sliderHalfHeight As Integer = 5

    Public Sub New()
      SetStyle(ControlStyles.Selectable, True)
    End Sub

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      Dim p As Point
      MyBase.OnDraw(e)
      Dim trc As Rectangle = TrackRectangle
      Dim num As Single = (CSng((Value - Minimum)) / (Maximum - Minimum))
      If (Orientation = Orientation.Vertical) Then
        Dim num2 As Integer = CInt((num * trc.Height))
        p = New Point((trc.X + (trc.Width \ 2)), ((trc.Y + trc.Height) - num2))
      Else
        Dim num3 As Integer = CInt((num * trc.Width))
        p = New Point((trc.X + num3), (trc.Y + (trc.Height \ 2)))
      End If
      DrawSlider(e.Graphics, p)
    End Sub

    Private Sub DrawSlider(ByVal g As Graphics, ByVal p As Point)
      Dim rect As Rectangle

      Dim wrc As Rectangle = DisplayRectangle
      If Orientation = Orientation.Vertical Then
        rect = New Rectangle(wrc.X, (p.Y - 5), wrc.Width, 10)
      Else
        rect = New Rectangle((p.X - 5), wrc.Y, 10, wrc.Height)
      End If
      Dim location As Point = rect.Location
      Dim point2 As Point = (rect.Location + rect.Size)
      Dim controlLightLight As Pen = SystemPens.ControlLightLight
      Dim pen As Pen = SystemPens.ControlDark
      Using brush As New LinearGradientBrush(rect, Color.White, Color.Gray, LinearGradientMode.Horizontal)
        g.FillRectangle(brush, rect)
      End Using
      g.DrawLine(controlLightLight, location.X, location.Y, location.X, point2.Y)
      g.DrawLine(controlLightLight, location.X, location.Y, point2.X, location.Y)
      g.DrawLine(pen, location.X, point2.Y, point2.X, point2.Y)
      g.DrawLine(pen, point2.X, location.Y, point2.X, point2.Y)

      Dim brush2 As Brush
      If Capture Then
        brush2 = New SolidBrush(activeLedColor_)
      Else
        brush2 = New SolidBrush(inactiveLedColor_)
      End If
      Dim rectangle4 As Rectangle = rect
      rectangle4.Inflate(-3, -3)
      g.FillRectangle(brush2, rectangle4)
      brush2.Dispose()
    End Sub

    Protected Overrides Function PointToValue(ByVal p As Point) As Double
      Dim num As Double
      Dim trc As Rectangle = TrackRectangle
      If Orientation = Orientation.Vertical Then
        If p.Y >= trc.Bottom Then Return Minimum
        If p.Y <= trc.Top Then Return Maximum
        num = (CDbl(((trc.Y + trc.Height) - p.Y)) / CDbl(trc.Height))
      Else
        If (p.X <= trc.Left) Then Return Minimum
        If (p.X >= trc.Right) Then Return Maximum
        num = (CSng((p.X - trc.X)) / CSng(trc.Width))
      End If
      Return (CInt((num * (Maximum - Minimum))) + Minimum)
    End Function


    ' Properties
    <DefaultValue(GetType(Color), "LightGreen"), Category("Appearance"), Description("Active led color")> _
    Public Property ActiveLedColor() As Color
      Get
        Return activeLedColor_
      End Get
      Set(ByVal value As Color)
        If activeLedColor_ <> value Then activeLedColor_ = value : Invalidate()
      End Set
    End Property

    Protected Overrides ReadOnly Property Editable() As Boolean
      Get
        Return True
      End Get
    End Property

    <DefaultValue(GetType(Color), "Green"), Description("Inactive led color"), Category("Appearance")> _
    Public Property InactiveLedColor() As Color
      Get
        Return inactiveLedColor_
      End Get
      Set(ByVal value As Color)
        If inactiveLedColor_ <> value Then inactiveLedColor_ = value : Invalidate()
      End Set
    End Property

    Private ReadOnly Property TrackRectangle() As Rectangle
      Get
        Dim ret As Rectangle = DisplayRectangle
        If Orientation = Orientation.Vertical Then ret.Y += 5 : ret.Height -= 10 : Return ret
        ret.X += 5 : ret.Width -= 10 : Return ret
      End Get
    End Property
  End Class


  Public Class Light : Inherits BoundedValueControl
    Private style_ As LightStyle

    Public Enum LightStyle
      Square
      Round
    End Enum

    Public Sub New()
      BackColor = Color.Black
      ForeColor = Color.Lime
    End Sub

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      Dim rect As Rectangle = DisplayRectangle
      Using path As New GraphicsPath
        If style_ = LightStyle.Round Then
          path.AddEllipse(rect)
        Else
          path.AddRectangle(rect)
        End If
        Using brush As New PathGradientBrush(path)
          Dim num As Single = (0.5! + (0.5! * (CSng((Value - Minimum)) / (Maximum - Minimum))))
          brush.CenterColor = Color.FromArgb(255, CInt((ForeColor.R * num)), CInt((ForeColor.G * num)), CInt((ForeColor.B * num)))
          brush.SurroundColors = New Color() {BackColor}
          e.Graphics.FillEllipse(brush, rect)
        End Using
      End Using
    End Sub


    ' Properties
    <DefaultValue(GetType(Color), "Black")> _
    Public Overrides Property BackColor() As Color
      Get
        Return MyBase.BackColor
      End Get
      Set(ByVal value As Color)
        MyBase.BackColor = value
      End Set
    End Property

    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Overrides Property BackgroundImage() As Image
      Get
        Return MyBase.BackgroundImage
      End Get
      Set(ByVal value As Image)
        MyBase.BackgroundImage = value
      End Set
    End Property

    Private ReadOnly Property Center() As Point
      Get
        With DisplayRectangle
          Return New Point((.Left + .Right) \ 2, (.Top + .Bottom) \ 2)
        End With
      End Get
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(24, 24)
      End Get
    End Property

    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Overrides Property Font() As Font
      Get
        Return MyBase.Font
      End Get
      Set(ByVal value As Font)
        MyBase.Font = value
      End Set
    End Property

    <DefaultValue(GetType(Color), "Lime")> _
    Public Overrides Property ForeColor() As Color
      Get
        Return MyBase.ForeColor
      End Get
      Set(ByVal value As Color)
        MyBase.ForeColor = value
      End Set
    End Property

    Private ReadOnly Property Radius() As Integer
      Get
        Dim wrc As Rectangle = DisplayRectangle
        If wrc.Width < wrc.Height Then Return wrc.Width \ 2
        Return wrc.Height \ 2
      End Get
    End Property

    <DefaultValue(GetType(LightStyle), "Square"), Category("Appearance"), Description("Style of light appearance")> _
    Public Property Style() As LightStyle
      Get
        Return style_
      End Get
      Set(ByVal value As LightStyle)
        If style_ <> value Then style_ = value : Invalidate()
      End Set
    End Property

    <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
    Public Overrides Property [Text]() As String
      Get
        Return MyBase.Text
      End Get
      Set(ByVal value As String)
        MyBase.Text = value
      End Set
    End Property
  End Class



  Public MustInherit Class LinearControlBase : Inherits BoundedValueControl
    Private dangerColor_ As Color = Color.Red, dangerValue_ As Integer = 80, _
            indicatorOnly_ As Boolean = True, labels_ As String() = New String(0 - 1) {}, _
            labelsCount_ As Integer = 5, labelsPosition_ As SliderElementsPosition = SliderElementsPosition.Both, _
            orientation_ As Orientation, scaleColor_ As Color = Color.Black, _
            scaleColorMode_ As ScaleColorMode = ScaleColorMode.Sections, _
            ticksCount_ As Integer = 10, ticksLength_ As Integer = 8, _
            ticksPosition_ As SliderElementsPosition = SliderElementsPosition.Both, _
            ticksSubDivisionsCount_ As Integer = 5, _
            warningColor_ As Color = Color.SaddleBrown, warningValue_ As Integer = 50

#If 0 Then  ' TODO: not, just for now
    Protected Sub New()
      SetStyle(ControlStyles.Selectable, True)
    End Sub
#End If

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      DrawBar(e.Graphics)
      DrawScale(e.Graphics)
      DrawSlider(e.Graphics)
    End Sub

    Protected Overridable Sub DrawBar(ByVal g As Graphics)
    End Sub

    Protected Overridable Sub DrawScale(ByVal g As Graphics)
      Dim irc As Rectangle = IndicatorRectangle
      If orientation_ = Orientation.Horizontal Then
        For i As Integer = 0 To ticksCount_
          Dim num2 As Double = (Minimum + ((Maximum - Minimum) * (CDbl(i) / CDbl(ticksCount_))))
          Dim num3 As Integer = (irc.X + CInt(((CDbl(i) / CDbl(ticksCount_)) * irc.Width)))
          Using penValue As New Pen(GetValueColor(CInt(num2)))
            If ((ticksPosition_ = SliderElementsPosition.TopLeft) OrElse (ticksPosition_ = SliderElementsPosition.Both)) Then
              g.DrawLine(penValue, num3, irc.Y, num3, (irc.Y - ticksLength_))
            End If
            If ((ticksPosition_ = SliderElementsPosition.BottomRight) OrElse (ticksPosition_ = SliderElementsPosition.Both)) Then
              g.DrawLine(penValue, num3, (irc.Y + irc.Height), num3, ((irc.Y + irc.Height) + ticksLength_))
            End If
            If (ticksPosition_ = SliderElementsPosition.Internal) Then
              g.DrawLine(penValue, num3, (irc.Y + irc.Height), num3, irc.Y)
            End If
          End Using

          ' Not on the last one
          If i <> ticksCount_ Then
            For j As Integer = 1 To ticksSubDivisionsCount_ - 1
              Dim num5 As Double = (num2 + ((Maximum - Minimum) * ((CDbl(j) / CDbl(ticksCount_)) / CDbl(ticksSubDivisionsCount_))))
              Dim num6 As Integer = (num3 + CInt((((j * irc.Width) / CDbl(ticksCount_)) / CDbl(ticksSubDivisionsCount_))))
              Using penValue As New Pen(GetValueColor(CInt(num5)))
                If ((ticksPosition_ = SliderElementsPosition.TopLeft) OrElse (ticksPosition_ = SliderElementsPosition.Both)) Then
                  g.DrawLine(penValue, num6, irc.Y, num6, (irc.Y - (ticksLength_ \ 2)))
                End If
                If ((ticksPosition_ = SliderElementsPosition.BottomRight) OrElse (ticksPosition_ = SliderElementsPosition.Both)) Then
                  g.DrawLine(penValue, num6, (irc.Y + irc.Height), num6, ((irc.Y + irc.Height) + (ticksLength_ \ 2)))
                End If
                If (ticksPosition_ = SliderElementsPosition.Internal) Then
                  g.DrawLine(penValue, num6, (irc.Y + (irc.Height \ 4)), num6, ((irc.Y + irc.Height) - (irc.Height \ 4)))
                End If
              End Using
            Next j
          End If
        Next i

        If (labelsPosition_ <> SliderElementsPosition.None) Then
          Dim flag As Boolean = ((Not labels_ Is Nothing) AndAlso (labels_.Length <> 0))
          Dim num7 As Integer
          If flag Then
            num7 = labels_.Length - 1
          Else
            num7 = labelsCount_
          End If

          Dim num8 As Integer = irc.Y
          If ticksPosition_ = SliderElementsPosition.TopLeft OrElse ticksPosition_ = SliderElementsPosition.Both Then
            num8 -= ticksLength_
          Else
            num8 += 2
          End If

          Dim num9 As Integer = irc.Y + irc.Height
          If ticksPosition_ = SliderElementsPosition.BottomRight OrElse ticksPosition_ = SliderElementsPosition.Both Then
            num9 += ticksLength_
          Else
            num9 += 2
          End If

          Dim k As Integer = 0
          Do While (k <= num7)
            Dim v As Integer = (Minimum + CInt(((Maximum - Minimum) * (CDbl(k) / CDbl(num7)))))
            Dim num12 As Integer = (irc.X + CInt(((CDbl(k) / CDbl(num7)) * irc.Width)))
            Using brush As New SolidBrush(GetValueColor(v))
              Dim text As String = ScaleAndFormatValue(v)
              If flag Then
                [text] = labels_(k)
              End If
              Dim size As Size = g.MeasureString([text], Font).ToSize
              If ((labelsPosition_ = SliderElementsPosition.TopLeft) OrElse (labelsPosition_ = SliderElementsPosition.Both)) Then
                g.DrawString([text], Font, brush, CSng((num12 - (size.Width / 2))), CSng((num8 - size.Height)))
              End If
              If ((labelsPosition_ = SliderElementsPosition.BottomRight) OrElse (labelsPosition_ = SliderElementsPosition.Both)) Then
                g.DrawString([text], Font, brush, CSng((num12 - (size.Width / 2))), CSng(num9))
              End If
              If (labelsPosition_ = SliderElementsPosition.Internal) Then
                g.DrawString([text], Font, brush, CSng((num12 - (size.Width / 2))), CSng((CenterLine - (size.Height / 2))))
              End If
            End Using
            k += 1
          Loop
        End If
      Else
        Dim m As Integer = 0
        Do While (m <= ticksCount_)
          Dim num14 As Double = (Minimum + ((Maximum - Minimum) * (CDbl(m) / CDbl(ticksCount_))))
          Dim num15 As Integer = ((irc.Height + irc.Y) - CInt(((CDbl(m) / CDbl(ticksCount_)) * irc.Height)))
          Using penColor As New Pen(GetValueColor(CInt(num14)))
            If ((ticksPosition_ = SliderElementsPosition.TopLeft) OrElse (ticksPosition_ = SliderElementsPosition.Both)) Then
              g.DrawLine(penColor, irc.X, num15, (irc.X - ticksLength_), num15)
            End If
            If ((ticksPosition_ = SliderElementsPosition.BottomRight) OrElse (ticksPosition_ = SliderElementsPosition.Both)) Then
              g.DrawLine(penColor, (irc.X + irc.Width), num15, ((irc.X + irc.Width) + ticksLength_), num15)
            End If
            If (ticksPosition_ = SliderElementsPosition.Internal) Then
              g.DrawLine(penColor, (irc.X + irc.Width), num15, irc.X, num15)
            End If
          End Using
          If (m <> ticksCount_) Then
            Dim n As Integer
            For n = 1 To ticksSubDivisionsCount_ - 1
              Dim num17 As Double = (num14 + ((Maximum - Minimum) * ((CDbl(n) / CDbl(ticksCount_)) / CDbl(ticksSubDivisionsCount_))))
              Dim num18 As Integer = (num15 - CInt((((n * irc.Height) / CDbl(ticksCount_)) / CDbl(ticksSubDivisionsCount_))))
              Using penColor As New Pen(GetValueColor(CInt(num17)))
                If ((ticksPosition_ = SliderElementsPosition.TopLeft) OrElse (ticksPosition_ = SliderElementsPosition.Both)) Then
                  g.DrawLine(penColor, irc.X, num18, (irc.X - (ticksLength_ \ 2)), num18)
                End If
                If ((ticksPosition_ = SliderElementsPosition.BottomRight) OrElse (ticksPosition_ = SliderElementsPosition.Both)) Then
                  g.DrawLine(penColor, (irc.X + irc.Width), num18, ((irc.X + irc.Width) + (ticksLength_ \ 2)), num18)
                End If
                If (ticksPosition_ = SliderElementsPosition.Internal) Then
                  g.DrawLine(penColor, (irc.X + (irc.Width \ 4)), num18, ((irc.X + irc.Width) - (irc.Width \ 4)), num18)
                End If
              End Using
            Next n
          End If
          m += 1
        Loop
        If (labelsPosition_ <> SliderElementsPosition.None) Then
          Dim flag2 As Boolean = ((Not labels_ Is Nothing) AndAlso (labels_.Length <> 0))
          Dim num19 As Integer
          If flag2 Then
            num19 = labels_.Length - 1
          Else
            num19 = labelsCount_
          End If

          Dim num20 As Integer = irc.X

          If ticksPosition_ = SliderElementsPosition.TopLeft OrElse ticksPosition_ = SliderElementsPosition.Both Then
            num20 -= ticksLength_
          Else
            num20 += 2
          End If

          Dim num21 As Integer = irc.X + irc.Width
          If ticksPosition_ = SliderElementsPosition.BottomRight OrElse ticksPosition_ = SliderElementsPosition.Both Then
            num21 += ticksLength_
          Else
            num21 += 2
          End If

          Dim index As Integer = 0
          Do While (index <= num19)
            Dim num23 As Integer = (Minimum + CInt(((Maximum - Minimum) * (CDbl(index) / CDbl(num19)))))
            Dim num24 As Integer = ((irc.Y + irc.Height) - CInt(((CDbl(index) / CDbl(num19)) * irc.Height)))
            Dim text2 As String = ScaleAndFormatValue(num23)
            If flag2 Then
              text2 = labels_(index)
            End If
            Dim size2 As Size = g.MeasureString(text2, Font).ToSize
            Using br As New SolidBrush(GetValueColor(num23))
              If ((labelsPosition_ = SliderElementsPosition.TopLeft) OrElse (labelsPosition_ = SliderElementsPosition.Both)) Then
                g.DrawString(text2, Font, br, CSng((num20 - size2.Width)), CSng((num24 - (size2.Height / 2))))
              End If
              If ((labelsPosition_ = SliderElementsPosition.BottomRight) OrElse (labelsPosition_ = SliderElementsPosition.Both)) Then
                g.DrawString(text2, Font, br, CSng(num21), CSng((num24 - (size2.Height / 2))))
              End If
              If (labelsPosition_ = SliderElementsPosition.Internal) Then
                g.DrawString(text2, Font, br, CSng(((irc.X + (irc.Width / 2)) - (size2.Width / 2))), CSng((num24 - (size2.Height / 2))))
              End If
            End Using
            index += 1
          Loop
        End If
      End If
    End Sub

    Protected Overridable Sub DrawSlider(ByVal g As Graphics)
    End Sub

    Protected Function GetPercent(ByVal value As Double) As Double
      If (Maximum = Minimum) Then
        Return 0
      End If
      Return ((value - Minimum) / CDbl((Maximum - Minimum)))
    End Function

    Protected Function GetValueColor(ByVal v As Integer) As Color
      Select Case scaleColorMode_
        Case ScaleColorMode.SingleColor
          Return scaleColor_
        Case ScaleColorMode.Sections
          If (v >= dangerValue_) Then
            Return dangerColor_
          End If
          If (v >= warningValue_) Then
            Return warningColor_
          End If
          Return scaleColor_
      End Select
      Return scaleColor_
    End Function

    Protected Overrides Function PointToValue(ByVal p As Point) As Double
      Dim num As Double
      Dim irc As Rectangle = IndicatorRectangle
      If (orientation_ = Orientation.Vertical) Then
        If (p.Y >= irc.Bottom) Then
          Return Minimum
        End If
        If (p.Y <= irc.Top) Then
          Return Maximum
        End If
        num = (CDbl(((irc.Y + irc.Height) - p.Y)) / CDbl(irc.Height))
      Else
        If (p.X <= irc.Left) Then
          Return Minimum
        End If
        If (p.X >= irc.Right) Then
          Return Maximum
        End If
        num = (CSng((p.X - irc.X)) / CSng(irc.Width))
      End If
      Return (CInt((num * (Maximum - Minimum))) + Minimum)
    End Function

    Protected Overridable Function ValueToPoint(ByVal value As Double) As Point
      Dim irc As Rectangle = IndicatorRectangle
      If (orientation_ = Orientation.Vertical) Then
        Dim num As Integer = CInt((GetPercent(value) * irc.Height))
        Return New Point((irc.X + (irc.Width \ 2)), ((irc.Y + irc.Height) - num))
      End If
      Dim num2 As Integer = CInt((GetPercent(value) * irc.Width))
      Return New Point((irc.X + num2), (irc.Y + (irc.Height \ 2)))
    End Function


    ' Properties
    Protected MustOverride ReadOnly Property CenterLine() As Integer

    <Description("Color of the scale ticks and numbers in the danger section"), DefaultValue(GetType(Color), "Red"), Category("Appearance")> _
    Public Property DangerColor() As Color
      Get
        Return dangerColor_
      End Get
      Set(ByVal value As Color)
        If dangerColor_ <> value Then dangerColor_ = value : Invalidate()
      End Set
    End Property

    <Description("Start value of the danger section"), Category("Appearance"), DefaultValue(80)> _
    Public Property DangerValue() As Integer
      Get
        Return dangerValue_
      End Get
      Set(ByVal value As Integer)
        If dangerValue_ <> value Then dangerValue_ = value : Invalidate()
      End Set
    End Property

    Protected Overrides ReadOnly Property Editable() As Boolean
      Get
        Return Not indicatorOnly_
      End Get
    End Property

    <Category("Behavior"), DefaultValue(True)> _
    Public Property IndicatorOnly() As Boolean
      Get
        Return indicatorOnly_
      End Get
      Set(ByVal value As Boolean)
        If indicatorOnly_ <> value Then
          indicatorOnly_ = value
          SetStyle(ControlStyles.Selectable, value)
        End If
      End Set
    End Property

    Protected MustOverride ReadOnly Property IndicatorRectangle() As Rectangle

    <Description("Labels text"), Category("Appearance")> _
    Public Property Labels() As String()
      Get
        Return labels_
      End Get
      Set(ByVal value As String())
        labels_ = value
        Invalidate()
      End Set
    End Property

    <Description("Number of the text  labels on the scale"), DefaultValue(5), Category("Appearance")> _
    Public Property LabelsCount() As Integer
      Get
        Return labelsCount_
      End Get
      Set(ByVal value As Integer)
        If value <> labelsCount_ Then labelsCount_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(GetType(SliderElementsPosition), "Both"), Description("Position of the text labels on the scale"), Category("Appearance")> _
    Public Property LabelsPosition() As SliderElementsPosition
      Get
        Return labelsPosition_
      End Get
      Set(ByVal value As SliderElementsPosition)
        If labelsPosition_ <> value Then labelsPosition_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(GetType(Orientation), "Horizontal"), Description("Control orientation"), Category("Appearance")> _
    Public Overridable Property Orientation() As Orientation
      Get
        Return orientation_
      End Get
      Set(ByVal value As Orientation)
        If orientation_ <> value Then orientation_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Color of the scale ticks and numbers"), DefaultValue(GetType(Color), "Black")> _
    Public Property ScaleColor() As Color
      Get
        Return scaleColor_
      End Get
      Set(ByVal value As Color)
        If scaleColor_ <> value Then scaleColor_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(GetType(ScaleColorMode), "Sections"), Description("Scale appearance mode"), _
     Category("Appearance")> _
    Public Property ScaleColorMode() As ScaleColorMode
      Get
        Return scaleColorMode_
      End Get
      Set(ByVal value As ScaleColorMode)
        If scaleColorMode_ <> value Then scaleColorMode_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(10), Category("Appearance"), Description("Number of divisions on the scale")> _
    Public Property TicksCount() As Integer
      Get
        Return ticksCount_
      End Get
      Set(ByVal value As Integer)
        If ticksCount_ <> value Then ticksCount_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(8), Description("Length of the scale ticks")> _
    Public Property TicksLength() As Integer
      Get
        Return ticksLength_
      End Get
      Set(ByVal value As Integer)
        If ticksLength_ <> value Then ticksLength_ = value : Invalidate()
      End Set
    End Property

    <Description("Position of the ticks on the scale"), DefaultValue(GetType(SliderElementsPosition), "Both"), _
     Category("Appearance")> _
    Public Property TicksPosition() As SliderElementsPosition
      Get
        Return ticksPosition_
      End Get
      Set(ByVal value As SliderElementsPosition)
        If ticksPosition_ <> value Then ticksPosition_ = value : Invalidate()
      End Set
    End Property

    <Description("Number of subdivisions in each division on the scale"), Category("Appearance"), DefaultValue(5)> _
    Public Property TicksSubDivisionsCount() As Integer
      Get
        Return ticksSubDivisionsCount_
      End Get
      Set(ByVal value As Integer)
        If ticksSubDivisionsCount_ <> value Then ticksSubDivisionsCount_ = value : Invalidate()
      End Set
    End Property

    <Description("Color of the scale ticks and numbers in the warning section"), Category("Appearance"), DefaultValue(GetType(Color), "SaddleBrown")> _
    Public Property WarningColor() As Color
      Get
        Return warningColor_
      End Get
      Set(ByVal value As Color)
        If warningColor_ <> value Then warningColor_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(50), Description("Start value of the warning section")> _
    Public Property WarningValue() As Integer
      Get
        Return warningValue_
      End Get
      Set(ByVal value As Integer)
        If warningValue_ <> value Then warningValue_ = value : Invalidate()
      End Set
    End Property

  End Class


  Public Enum SliderElementsPosition
    None
    TopLeft
    BottomRight
    Both
    Internal
  End Enum

  Public Enum ScaleColorMode
    Sections = 1
    SingleColor = 0
  End Enum



  Public Class Matrix : Inherits Control ' InstrumentationControl
    Private scrollInterval_ As Integer = 100, scrollAmount_ As Integer = 1, _
            scrollStyle_ As TextScrollStyle, scrollText_ As Boolean
    Private textDx_, textMove_, textLength_, textYOfs_ As Integer
    Private WithEvents timer_ As Windows.Forms.Timer

    Public Sub New()
      BackColor = Color.Black : ForeColor = Color.Lime
      DoubleBuffered = True
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
      If disposing AndAlso Not timer_ Is Nothing Then timer_.Dispose() : timer_ = Nothing
      MyBase.Dispose(disposing)
    End Sub

    Const factor As Integer = 2
    Private Sub DoScroll()
      textDx_ += textMove_
      Invalidate()
      If textDx_ < -textLength_ - 1 OrElse textDx_ > 1 + DisplayRectangle.Width \ (factor * scrollAmount_) Then
        Select Case scrollStyle_
          Case TextScrollStyle.RightToLeft
            textDx_ = DisplayRectangle.Width \ (factor * scrollAmount_)
          Case TextScrollStyle.LeftToRight
            textDx_ = -textLength_
          Case TextScrollStyle.Reverse
            textMove_ = textMove_ * -1
        End Select
      End If
    End Sub


    Protected Overrides Sub OnPaint(ByVal pe As PaintEventArgs)
      If pe.ClipRectangle.IsEmpty Then Exit Sub
      Draw(pe.Graphics, pe.ClipRectangle)
    End Sub
    Protected Overrides Sub OnPaintBackground(ByVal pe As PaintEventArgs)
    End Sub

    Protected Overrides Sub OnResize(ByVal e As EventArgs)
      MyBase.OnResize(e)
      ResetScroll()
    End Sub

    Protected Overrides Sub OnFontChanged(ByVal e As EventArgs)
      MyBase.OnFontChanged(e)
      If IsHandleCreated Then Diddly()
    End Sub

    Private Shared Sub Diddly()

    End Sub

    Public Property TextYOfs() As Integer
      Get
        Return textYOfs_
      End Get
      Set(ByVal value As Integer)
        textYOfs_ = value
      End Set
    End Property

    Private bmGrid_, bmTextOnGrid_ As Bitmap

    Protected Overridable Sub Draw(ByVal g As Graphics, ByVal r As Rectangle)
      If textLength_ = 0 AndAlso scrollText_ Then InitScroll()

      If bmGrid_ Is Nothing Then
        Dim rc As Rectangle = ClientRectangle
        bmGrid_ = New Bitmap(rc.Width, rc.Height)
        Using gr As Graphics = Graphics.FromImage(bmGrid_)
          Using br As New SolidBrush(BackColor)
            gr.FillRectangle(br, 0, 0, rc.Width, rc.Height)
          End Using
          Using brush2 As New SolidBrush(Color.FromArgb(85, ForeColor))
            Dim w As Integer = (rc.Width + factor - 1) \ factor, _
                h As Integer = (rc.Height + factor - 1) \ factor
            For x As Integer = 0 To w - 1
              For y As Integer = 0 To h - 1
                gr.FillRectangle(brush2, x * factor, 1 + y * factor, factor - 1, factor - 1)  ' the 1 + y gives some background at the top
              Next y
            Next x
          End Using
        End Using
      End If

      If bmTextOnGrid_ Is Nothing Then
        Dim w, h As Integer : With TextRenderer.MeasureText(Text, Font) : w = .Width : h = .Height : End With
        If w <> 0 AndAlso h <> 0 Then
          ' Draw the text as white text onto a black background in a smaller bitmap
          Using bm As New Bitmap(w, h)
            Using gr As Graphics = Graphics.FromImage(bm)
              gr.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit ' Drawing.Text.TextRenderingHint.AntiAliasGridFit  ' no clear type please
              gr.FillRectangle(Brushes.Black, 0, 0, w, h)
              gr.DrawString(Text, Font, Brushes.White, 0, textYOfs_)
            End Using
            bmTextOnGrid_ = New Bitmap(w * factor, h * factor)
            Using gr As Graphics = Graphics.FromImage(bmTextOnGrid_)
              Using br As New SolidBrush(BackColor)
                gr.FillRectangle(br, 0, 0, bmTextOnGrid_.Width, bmTextOnGrid_.Height)
              End Using
              Using brush1 As New SolidBrush(ForeColor), brush2 As New SolidBrush(Color.FromArgb(85, ForeColor))
                For x As Integer = 0 To w - 1
                  For y As Integer = 0 To h - 1
                    Dim brush3 As Brush
                    If bm.GetPixel(x, y).R <> 0 Then
                      brush3 = brush1
                    Else
                      brush3 = brush2
                    End If
                    gr.FillRectangle(brush3, x * factor, 1 + y * factor, factor - 1, factor - 1)
                  Next y
                Next x
              End Using
            End Using
          End Using
        End If
      End If

      g.DrawImage(bmGrid_, 0, 0)
      If bmTextOnGrid_ IsNot Nothing Then g.DrawImage(bmTextOnGrid_, textDx_ * factor * scrollAmount_, 0)

    End Sub



    Private Sub InitScroll()
      textLength_ = TextRenderer.MeasureText(Text, Font).Width
      Select Case scrollStyle_
        Case TextScrollStyle.RightToLeft
          textDx_ = DisplayRectangle.Width \ (factor * scrollAmount_)
          textMove_ = -1
        Case TextScrollStyle.LeftToRight
          textDx_ = -textLength_
          textMove_ = 1
        Case TextScrollStyle.Reverse
          textDx_ = DisplayRectangle.Width \ (factor * scrollAmount_)
          textMove_ = -1
      End Select

    End Sub



    Protected Overrides Sub OnTextChanged(ByVal e As EventArgs)
      ResetScroll()
      Invalidate()
      MyBase.OnTextChanged(e)
    End Sub


    Private Sub ResetScroll()
      textLength_ = 0
      textDx_ = 0
    End Sub

    Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles timer_.Tick
      DoScroll()
    End Sub


    Private Sub UpdateTimerSettings()
      If timer_ IsNot Nothing Then With timer_ : .Interval = scrollInterval_ : .Enabled = scrollText_ : End With
    End Sub


    <DefaultValue(GetType(Color), "Black")> _
    Public Overrides Property BackColor() As Color
      Get
        Return MyBase.BackColor
      End Get
      Set(ByVal value As Color)
        MyBase.BackColor = value
      End Set
    End Property


    <DefaultValue(GetType(Color), "Lime")> _
    Public Overrides Property ForeColor() As Color
      Get
        Return MyBase.ForeColor
      End Get
      Set(ByVal value As Color)
        MyBase.ForeColor = value
      End Set
    End Property


    Public Property ScrollAmount() As Integer
      Get
        Return scrollAmount_
      End Get
      Set(ByVal value As Integer)
        If scrollAmount_ <> value Then scrollAmount_ = value
      End Set
    End Property

    <Description("Scrolling ticks interval"), Category("Appearance"), DefaultValue(100)> _
    Public Property ScrollInterval() As Integer
      Get
        Return scrollInterval_
      End Get
      Set(ByVal value As Integer)
        If value <> scrollInterval_ Then scrollInterval_ = value : UpdateTimerSettings()
      End Set
    End Property


    <Category("Appearance"), DefaultValue(0), Description("Text scrolling direction")> _
    Public Property ScrollStyle() As TextScrollStyle
      Get
        Return scrollStyle_
      End Get
      Set(ByVal value As TextScrollStyle)
        If scrollStyle_ <> value Then scrollStyle_ = value : ResetScroll()
      End Set
    End Property

    <DefaultValue(False), Category("Appearance"), Description("Indicate whether text should be scrolled")> _
    Public Property ScrollText() As Boolean
      Get
        Return scrollText_
      End Get
      Set(ByVal value As Boolean)
        If value <> scrollText_ Then
          ResetScroll()
          scrollText_ = value
          If scrollText_ AndAlso timer_ Is Nothing Then timer_ = New Windows.Forms.Timer
          If timer_ IsNot Nothing Then UpdateTimerSettings()
          Invalidate()
        End If
      End Set
    End Property
  End Class

  Public Enum TextScrollStyle
    RightToLeft
    LeftToRight
    Reverse
  End Enum




  Public Class Meter : Inherits BoundedValueControl
    Private dangerColor_ As Color = Color.Red
    Private dangerValue_ As Integer = 70
    Private largeTickFrequency_ As Integer = 20
    Private numberFrequency_ As Integer = 25
    Private pointerColor_ As Color = Color.Red
    Private showGradient_ As Boolean
    Private showNumbers_ As Boolean = True
    Private smallTickFrequency_ As Integer = 5
    Private totalAngle_ As Integer = 130

    ' Want this back
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
      Public Overrides Property Text() As String
      Get
        Return MyBase.Text
      End Get
      Set(ByVal value As String)
        MyBase.Text = value
      End Set
    End Property

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      Dim brush1 As Brush
      Dim num5 As Integer
      Dim single3 As Single
      Dim single4 As Single
      Dim num6 As Double
      Dim num7 As Double
      Dim num8 As Double
      Dim num9 As Integer
      Dim num10 As Double
      Dim num11 As Double
      Dim text1 As String
      Dim ef1 As SizeF
      Dim num12 As Double
      Dim num13 As Double
      Dim tf1 As PointF
      Dim tf2 As PointF
      Dim ef2 As SizeF
      MyBase.OnDraw(e)
      Dim g As Graphics = e.Graphics
      Dim rectangle1 As Rectangle = Me.DisplayRectangle
      Dim pen1 As Pen = New Pen(Me.ForeColor)
      Dim pen2 As Pen = New Pen(dangerColor_)
      If showGradient_ Then
        brush1 = New LinearGradientBrush(rectangle1, GraphicsUtils.ScaleColor(Me.BackColor, 0.5!), Me.BackColor, LinearGradientMode.Vertical)
        g.FillRectangle(brush1, rectangle1)
      End If
      Dim brush2 As Brush = New SolidBrush(Me.ForeColor)
      Dim point1 As Point = Me.Center
      Dim num1 As Integer = CType((CType(Me.Radius, Single) * 0.8!), Integer)
      Dim single1 As Single = (CType((MyBase.Value - Me.Minimum), Single) / (CType(Me.Maximum, Single) - CType(Me.Minimum, Single)))
      Dim single2 As Single = (single1 * CType(totalAngle_, Single))
      If (Not Me.Text Is "") Then
        ef2 = g.MeasureString(Me.Text, Me.Font)
        g.DrawString(Me.Text, Me.Font, brush2, Point.op_Implicit(New Point(CType((CType(point1.X, Single) - (ef2.Width / 2.0!)), Integer), ((point1.Y - 10) - (Me.Font.Height \ 2)))))
      End If
      Me.DrawPointer(g, point1, single2, (CType(num1, Single) * 0.8!))
      Dim num2 As Integer = CType((CType(num1, Single) * 0.7!), Integer)
      Dim num3 As Integer = (num2 + 3)
      Dim num4 As Integer = (num3 + 6)
      num5 = Me.Minimum
      Do While (num5 <= Me.Maximum)
        single1 = (CType((num5 - Me.Minimum), Single) / (CType(Me.Maximum, Single) - CType(Me.Minimum, Single)))
        single3 = (single1 * CType(totalAngle_, Single))
        single4 = ((CType(totalAngle_, Single) / 2.0!) - single3)
        num6 = (CType(point1.X, Double) - (Math.Sin((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(num2, Double)))
        num7 = (CType(point1.Y, Double) - (Math.Cos((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(num2, Double)))
        num8 = CType(num3, Double)
        If ((num5 Mod largeTickFrequency_) = 0) Then
          num8 = CType(num4, Double)
        End If
        If (showNumbers_ AndAlso ((num5 Mod numberFrequency_) = 0)) Then
          num9 = (num4 + Me.Font.Height)
          num10 = (CType(point1.X, Double) - (Math.Sin((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(num9, Double)))
          num11 = (CType(point1.Y, Double) - (Math.Cos((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(num9, Double)))
          text1 = MyBase.ScaleAndFormatValue(num5)
          ef1 = g.MeasureString(text1, Me.Font)
          num10 = (num10 - CType((ef1.Width / 2.0!), Double))
          num11 = (num11 - CType((ef1.Height / 2.0!), Double))
          g.DrawString(text1, Me.Font, brush2, Point.op_Implicit(New Point(CType(num10, Integer), CType(num11, Integer))))
        End If
        num12 = (CType(point1.X, Double) - (Math.Sin((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * num8))
        num13 = (CType(point1.Y, Double) - (Math.Cos((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * num8))
        tf1 = New PointF(CType(num6, Single), CType(num7, Single))
        tf2 = New PointF(CType(num12, Single), CType(num13, Single))
        If (num5 < dangerValue_) Then
          g.DrawLine(pen1, tf1, tf2)
        Else
          g.DrawLine(pen2, tf1, tf2)
        End If
        ef2 = g.MeasureString(Me.Text, Me.Font)
        g.DrawString(Me.Text, Me.Font, brush2, Point.op_Implicit(New Point(CType((CType(point1.X, Single) - (ef2.Width / 2.0!)), Integer), ((point1.Y + num4) + (Me.Font.Height \ 2)))))
        num5 = (num5 + smallTickFrequency_)
      Loop
    End Sub

    Private Sub DrawPointer(ByVal g As Graphics, ByVal p As Point, ByVal angle As Single, ByVal rad As Single)
      Dim point1 As Point
      Dim single1 As Single = ((CType(totalAngle_, Single) / 2.0!) - angle)
      Dim num1 As Double = (CType(p.X, Double) - (Math.Sin((CType((single1 / 180.0!), Double) * 3.1415926535897931)) * CType(rad, Double)))
      Dim num2 As Double = (CType(p.Y, Double) - (Math.Cos((CType((single1 / 180.0!), Double) * 3.1415926535897931)) * CType(rad, Double)))
      point1 = New Point(CType(num1, Integer), CType(num2, Integer))
      g.DrawLine(New Pen(pointerColor_), p, point1)
    End Sub

    Protected Overrides Sub OnTextChanged(ByVal e As EventArgs)
      Invalidate()
      MyBase.OnTextChanged(e)
    End Sub

    Private ReadOnly Property Center() As Point
      Get
        Dim rectangle1 As Rectangle = Me.DisplayRectangle
        Return New Point(((rectangle1.Left + rectangle1.Right) \ 2), rectangle1.Bottom)
      End Get
    End Property

    <DefaultValue(GetType(Color), "Red"), Description("Danger color."), Category("Appearance")> _
    Public Property DangerColor() As Color
      Get
        Return dangerColor_
      End Get
      Set(ByVal value As Color)
        If Not dangerColor_.Equals(value) Then
          dangerColor_ = value
          Invalidate()
        End If
      End Set
    End Property

    <Description("Danger Value"), DefaultValue(70), Category("Behavior")> _
    Public Property DangerValue() As Integer
      Get
        Return dangerValue_
      End Get
      Set(ByVal value As Integer)
        If (dangerValue_ <> value) Then
          dangerValue_ = value
          Invalidate()
        End If
      End Set
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(168, 96)
      End Get
    End Property

    <Category("Appearance"), DefaultValue(20), Description("Define frequency of large ticks")> _
    Public Property LargeTickFrequency() As Integer
      Get
        Return largeTickFrequency_
      End Get
      Set(ByVal value As Integer)
        If (largeTickFrequency_ <> value) Then
          largeTickFrequency_ = value
          Invalidate()
        End If
      End Set
    End Property

    <Category("Appearance"), DefaultValue(25), Description("Define frequency of numbers")> _
    Public Property NumberFrequency() As Integer
      Get
        Return numberFrequency_
      End Get
      Set(ByVal value As Integer)
        If (numberFrequency_ <> value) Then
          numberFrequency_ = value
          Invalidate()
        End If
      End Set
    End Property

    <DefaultValue(GetType(Color), "Red"), Category("Appearance"), Description("Color of the  pointer")> _
    Public Property PointerColor() As Color
      Get
        Return pointerColor_
      End Get
      Set(ByVal value As Color)
        If Not pointerColor_.Equals(value) Then
          pointerColor_ = value
          Invalidate()
        End If
      End Set
    End Property

    Private ReadOnly Property Radius() As Integer
      Get
        Dim rectangle1 As Rectangle = Me.DisplayRectangle
        If (rectangle1.Height >= (rectangle1.Width \ 2)) Then
          Return (rectangle1.Width \ 2)
        End If
        Return rectangle1.Height
      End Get
    End Property

    <Description("Gets or sets whether control should use gradient background"), Category("Appearance"), DefaultValue(False)> _
    Public Property ShowGradient() As Boolean
      Get
        Return showGradient_
      End Get
      Set(ByVal value As Boolean)
        If (showGradient_ <> value) Then
          showGradient_ = value
          Invalidate()
        End If
      End Set
    End Property

    <Category("Appearance"), DefaultValue(True), Description("Indicate if the control paints scale numbers")> _
    Public Property ShowNumbers() As Boolean
      Get
        Return showNumbers_
      End Get
      Set(ByVal value As Boolean)
        If (showNumbers_ <> value) Then
          showNumbers_ = value
          Invalidate()
        End If
      End Set
    End Property

    <DefaultValue(5), Category("Appearance"), Description("Define frequency of small ticks")> _
    Public Property SmallTickFrequency() As Integer
      Get
        Return smallTickFrequency_
      End Get
      Set(ByVal value As Integer)
        If (smallTickFrequency_ <> value) Then
          smallTickFrequency_ = value
          Invalidate()
        End If
      End Set
    End Property

    <Description("Angle behind minimum and maximum positions"), DefaultValue(130), Category("Behavior")> _
    Public Property TotalAngle() As Integer
      Get
        Return totalAngle_
      End Get
      Set(ByVal value As Integer)
        If (totalAngle_ <> value) Then
          totalAngle_ = value
          Invalidate()
        End If
      End Set
    End Property
  End Class




  Public Class MultiPositionSwitch : Inherits BoundedValueControl
    Private cases_ As String() = New String() {}
    Private dialColor_ As Color = Color.White
    Private dialRadius_ As Integer = 30
    Private pointerColor_ As Color = Color.Black
    Private pointerRadius_ As Integer = 20
    Private totalAngle_ As Integer = 270

    Public Sub New()
      SetStyle(ControlStyles.Selectable, True)
      Maximum = 5 : Increment = 1
    End Sub

    ' Want this back
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
      Public Overrides Property Text() As String
      Get
        Return MyBase.Text
      End Get
      Set(ByVal value As String)
        MyBase.Text = value
      End Set
    End Property

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      Dim num4 As Integer
      Dim single3 As Single
      Dim single4 As Single
      Dim num5 As Double
      Dim num6 As Double
      Dim num7 As Double
      Dim num8 As Double
      Dim num9 As Double
      Dim num10 As Double
      Dim text1 As String
      Dim num11 As Integer
      Dim ef1 As SizeF
      Dim point2 As Point
      Dim font1 As Font
      MyBase.OnDraw(e)
      Dim g As Graphics = e.Graphics
      Dim pen1 As Pen = New Pen(Me.ForeColor)
      Dim brush1 As Brush = New SolidBrush(Me.ForeColor)
      Dim point1 As Point = Me.Center
      Dim single1 As Single = (CType((MyBase.Value - Me.Minimum), Single) / (CType(Me.Maximum, Single) - CType(Me.Minimum, Single)))
      Dim single2 As Single = (single1 * CType(totalAngle_, Single))
      Me.DrawDial(g, point1, single2)
      Dim num1 As Integer = (dialRadius_ + 2)
      Dim num2 As Integer = (num1 + 6)
      Dim num3 As Integer = 7
      num4 = Me.Minimum
      Do While (num4 <= Me.Maximum)
        single1 = (CType((num4 - Me.Minimum), Single) / (CType(Me.Maximum, Single) - CType(Me.Minimum, Single)))
        single3 = (single1 * CType(totalAngle_, Single))
        single4 = ((CType(totalAngle_, Single) / 2.0!) - single3)
        num5 = (CType(point1.X, Double) - (Math.Sin((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(num1, Double)))
        num6 = (CType(point1.Y, Double) - (Math.Cos((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(num1, Double)))
        num7 = (CType(point1.X, Double) - (Math.Sin((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(num2, Double)))
        num8 = (CType(point1.Y, Double) - (Math.Cos((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(num2, Double)))
        g.DrawLine(pen1, CType(num5, Integer), CType(num6, Integer), CType(num7, Integer), CType(num8, Integer))
        num11 = (num4 - Me.Minimum)
        If (cases_.Length > num11) Then
          text1 = cases_(num11)
        Else
          text1 = String.Concat("case ", num4.ToString)
        End If
        ef1 = g.MeasureString(text1, Me.Font)
        If ((num4 - Me.Minimum) < ((Me.Maximum - Me.Minimum) / 2)) Then
          num9 = (num7 - CType(num3, Double))
          num10 = num8
          point2 = New Point(((CType(num9, Integer) - CType(ef1.Width, Integer)) - 2), (CType(num10, Integer) - CType(ef1.Height, Integer)))
        Else
          If ((num4 - Me.Minimum) > ((Me.Maximum - Me.Minimum) / 2)) Then
            num9 = (num7 + CType(num3, Double))
            num10 = num8
            point2 = New Point((CType(num9, Integer) + 2), (CType(num10, Integer) - CType(ef1.Height, Integer)))
          Else
            num9 = num7
            num10 = num8
            point2 = New Point((CType(num9, Integer) - CType((ef1.Width / 2.0!), Integer)), ((CType(num10, Integer) - CType(ef1.Height, Integer)) - 3))
          End If
        End If
        g.DrawLine(pen1, CType(num9, Integer), CType(num10, Integer), CType(num7, Integer), CType(num8, Integer))
        font1 = Me.Font
        g.DrawString(text1, font1, brush1, Point.op_Implicit(point2))
        num4 = (num4 + 1)
      Loop
      Dim ef2 As SizeF = g.MeasureString(Me.Text, Me.Font)
      g.DrawString(Me.Text, Me.Font, brush1, Point.op_Implicit(New Point(CType((CType(point1.X, Single) - (ef2.Width / 2.0!)), Integer), ((point1.Y + num2) + (Me.Font.Height \ 2)))))
    End Sub

    Private Sub DrawDial(ByVal g As Graphics, ByVal p As Point, ByVal angle As Single)
      Dim rectangle1 As Rectangle
      Dim rectangle2 As Rectangle
      Dim num8 As Integer
      Dim single5 As Single
      Dim tf1 As PointF
      Dim tf2 As PointF
      rectangle1 = New Rectangle((p.X - pointerRadius_), (p.Y - pointerRadius_), (pointerRadius_ * 2), (pointerRadius_ * 2))
      rectangle2 = New Rectangle((p.X - dialRadius_), (p.Y - dialRadius_), (dialRadius_ * 2), (dialRadius_ * 2))
      Dim single1 As Single = (CType(p.X, Single) - (CType(dialRadius_, Single) * 0.6!))
      Dim single2 As Single = (CType(p.Y, Single) - (CType(dialRadius_, Single) * 0.6!))
      Dim single3 As Single = (CType(dialRadius_, Single) * 1.85!)
      Dim path1 As GraphicsPath = New GraphicsPath
      path1.AddEllipse((single1 - single3), (single2 - single3), (single3 * 2.0!), (single3 * 2.0!))
      Dim brush1 As PathGradientBrush = New PathGradientBrush(path1)
      brush1.CenterColor = dialColor_
      Dim colorArray2 As Color() = New Color() {GraphicsUtils.ScaleColor(dialColor_, 0.25!)}
      brush1.SurroundColors = colorArray2
      Dim brush2 As SolidBrush = New SolidBrush(GraphicsUtils.ScaleColor(dialColor_, 0.75!))
      Dim container1 As GraphicsContainer = g.BeginContainer
      g.SmoothingMode = SmoothingMode.AntiAlias
      g.FillEllipse(brush1, rectangle2)
      g.FillEllipse(brush2, rectangle1)
      Dim single4 As Single = ((CType(totalAngle_, Single) / 2.0!) - angle)
      Dim num1 As Double = (CType(p.X, Double) - (Math.Sin((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(pointerRadius_, Double)))
      Dim num2 As Double = (CType(p.Y, Double) - (Math.Cos((CType((single4 / 180.0!), Double) * 3.1415926535897931)) * CType(pointerRadius_, Double)))
      '   New PointF(CType(num1,Single), CType(num2,Single))
      Dim num3 As Double = (CType(p.X, Double) - (Math.Sin((CType(((90.0! + single4) / 180.0!), Double) * 3.1415926535897931)) * 3))
      Dim num4 As Double = (CType(p.Y, Double) - (Math.Cos((CType(((90.0! + single4) / 180.0!), Double) * 3.1415926535897931)) * 3))
      Dim num5 As Double = (CType(p.X, Double) - (Math.Sin((CType(((-90.0! + single4) / 180.0!), Double) * 3.1415926535897931)) * 3))
      Dim num6 As Double = (CType(p.Y, Double) - (Math.Cos((CType(((-90.0! + single4) / 180.0!), Double) * 3.1415926535897931)) * 3))
      Dim tfArray1 As PointF() = New PointF() { _
                    New PointF(CType(num1, Single), CType(num2, Single)), _
                    New PointF(CType(num3, Single), CType(num4, Single)), _
                    New PointF(CType(num5, Single), CType(num6, Single))}
      g.FillPolygon(New SolidBrush(pointerColor_), tfArray1)
      Dim num7 As Integer = 9
      num8 = 0
      Do While (num8 < num7)
        single5 = (((360.0! / CType(num7, Single)) * CType(num8, Single)) + single4)
        num1 = (CType(p.X, Double) - (Math.Sin((CType((single5 / 180.0!), Double) * 3.1415926535897931)) * CType(pointerRadius_, Double)))
        num2 = (CType(p.Y, Double) - (Math.Cos((CType((single5 / 180.0!), Double) * 3.1415926535897931)) * CType(pointerRadius_, Double)))
        tf1 = New PointF(CType(num1, Single), CType(num2, Single))
        num1 = (CType(p.X, Double) - (Math.Sin((CType((single5 / 180.0!), Double) * 3.1415926535897931)) * CType(dialRadius_, Double)))
        num2 = (CType(p.Y, Double) - (Math.Cos((CType((single5 / 180.0!), Double) * 3.1415926535897931)) * CType(dialRadius_, Double)))
        tf2 = New PointF(CType(num1, Single), CType(num2, Single))
        g.DrawLine(Pens.Gray, tf1, tf2)
        num8 = (num8 + 1)
      Loop
      g.EndContainer(container1)
    End Sub

    Private Sub DrawPointer(ByVal g As Graphics, ByVal p As Point, ByVal angle As Single)
      Dim point1 As Point
      Dim single1 As Single = ((CType(totalAngle_, Single) / 2.0!) - angle)
      Dim num1 As Double = (CType(p.X, Double) - (Math.Sin((CType((single1 / 180.0!), Double) * 3.1415926535897931)) * CType(pointerRadius_, Double)))
      Dim num2 As Double = (CType(p.Y, Double) - (Math.Cos((CType((single1 / 180.0!), Double) * 3.1415926535897931)) * CType(pointerRadius_, Double)))
      point1 = New Point(CType(num1, Integer), CType(num2, Integer))
      g.DrawLine(New Pen(pointerColor_), p, point1)
    End Sub

    Protected Overrides Sub OnTextChanged(ByVal e As EventArgs)
      Invalidate()
      MyBase.OnTextChanged(e)
    End Sub

    Protected Overrides Function PointToValue(ByVal p As Point) As Double
      Dim num1 As Double
      Dim num2 As Double
      Dim point1 As Point = Me.Center
      Dim single1 As Single = CType((point1.X - p.X), Single)
      Dim single2 As Single = CType((point1.Y - p.Y), Single)
      If single2 = 0.0! Then
        If single1 > 0.0! Then
          num1 = 90
        Else
          num1 = -90
        End If
      Else
        num2 = ((Math.Atan(CType((single1 / single2), Double)) * 180) / 3.1415926535897931)
        If single2 > 0.0! Then
          num1 = num2
        Else
          If single1 > 0.0! Then
            num1 = num2 + 180
          Else
            num1 = num2 - 180
          End If
        End If
      End If
      Dim num3 As Double = (CType((totalAngle_ / 2), Double) - num1)
      Dim num4 As Double = (num3 / CType(totalAngle_, Double))
      If (num4 <= 0) Then
        Return Me.Minimum
      End If
      If (num4 >= 1) Then
        Return Me.Maximum
      End If
      Return (Me.Minimum + CType(((CType(Me.Maximum, Double) - CType(Me.Minimum, Double)) * num4), Integer))
    End Function

    <Description("Cases text"), Category("Appearance")> _
    Public Property Cases() As String()
      Get
        Return cases_
      End Get
      Set(ByVal value As String())
        cases_ = value
        Invalidate()
      End Set
    End Property

    Private ReadOnly Property Center() As Point
      Get
        Dim rectangle1 As Rectangle = Me.DisplayRectangle
        Return New Point(((rectangle1.Left + rectangle1.Right) \ 2), ((rectangle1.Top + rectangle1.Bottom) \ 2))
      End Get
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(180, 170)
      End Get
    End Property

    <Description("Color of the dial"), DefaultValue(GetType(Color), "White"), Category("Appearance")> _
    Public Property DialColor() As Color
      Get
        Return dialColor_
      End Get
      Set(ByVal value As Color)
        If Not dialColor_.Equals(value) Then dialColor_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(30), Category("Appearance"), Description("Radius of dial")> _
    Public Property DialRadius() As Integer
      Get
        Return dialRadius_
      End Get
      Set(ByVal value As Integer)
        If dialRadius_ <> value Then dialRadius_ = value : Invalidate()
      End Set
    End Property

    Protected Overrides ReadOnly Property Editable() As Boolean
      Get
        Return True
      End Get
    End Property

    <DefaultValue(1)> _
    Public Overrides Property Increment() As Integer
      Get
        Return MyBase.Increment
      End Get
      Set(ByVal value As Integer)
        MyBase.Increment = value
      End Set
    End Property

    <DefaultValue(5)> _
    Public Overrides Property Maximum() As Integer
      Get
        Return MyBase.Maximum
      End Get
      Set(ByVal value As Integer)
        MyBase.Maximum = value
      End Set
    End Property

    <DefaultValue(GetType(Color), "Black"), Category("Appearance"), Description("Color of the pointer")> _
    Public Property PointerColor() As Color
      Get
        Return pointerColor_
      End Get
      Set(ByVal value As Color)
        If Not pointerColor_.Equals(value) Then pointerColor_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(20), Description("Radius of slider"), Category("Appearance")> _
    Public Property PointerRadius() As Integer
      Get
        Return pointerRadius_
      End Get
      Set(ByVal value As Integer)
        If pointerRadius_ <> value Then pointerRadius_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Angle behind minimum and maximum positions"), DefaultValue(270)> _
    Public Property TotalAngle() As Integer
      Get
        Return totalAngle_
      End Get
      Set(ByVal value As Integer)
        If totalAngle_ <> value Then totalAngle_ = value : Invalidate()
      End Set
    End Property
  End Class

  <DefaultBindingProperty("Value")> _
  Public Class Odometer : Inherits BorderTransparentControl
    Private firstDigitBackColor_ As Color = Color.Red, firstDigitForeColor_ As Color = Color.White, _
            showGradient_ As Boolean = True, style_ As OdometerStyle, value_ As Single

    Public Enum OdometerStyle
      Mechanic
      Modern
    End Enum

    <Category("Action"), Description("Occurs when Value of the control changed")> _
    Public Event ValueChanged As EventHandler

    ' Methods
    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      Dim g As Graphics = e.Graphics
      DrawNumber(g)
      If showGradient_ Then
        Dim rect As Rectangle = DisplayRectangle
        Using brush As New LinearGradientBrush(rect, Color.FromArgb(128, 0, 0, 0), Color.FromArgb(128, 0, 0, 0), LinearGradientMode.Vertical)
          Dim blend As New ColorBlend(3)
          blend.Positions(0) = 0.0!
          blend.Positions(1) = 0.4!
          blend.Positions(2) = 1.0!
          blend.Colors(0) = Color.FromArgb(128, 0, 0, 0)
          blend.Colors(1) = Color.Transparent
          blend.Colors(2) = Color.FromArgb(128, 0, 0, 0)
          brush.InterpolationColors = blend
          g.FillRectangle(brush, rect)
        End Using
      End If
    End Sub

    Private Sub DrawDigit(ByVal g As Graphics, ByVal rect As Rectangle, ByVal color As Color, ByVal digitColor As Color, ByVal digit As Integer, ByVal delta As Single)
      Dim container As GraphicsContainer = g.BeginContainer
      g.IntersectClip(rect)
      Using br As New SolidBrush(color)
        g.FillRectangle(br, rect)
      End Using
      Dim format As New StringFormat
      format.Alignment = StringAlignment.Center
      format.LineAlignment = StringAlignment.Center
      Dim num As Integer : If digit <> 9 Then num = digit + 1
      Dim height As Single = g.MeasureString("8", Font).Height
      Dim layoutRectangle As Rectangle = rect
      layoutRectangle.Y = (layoutRectangle.Y + (CInt(height) - CInt((height * delta))))
      If style_ = OdometerStyle.Mechanic Then
        rect.Y = (rect.Y - CInt((height * delta)))
        Using digitBrush As New SolidBrush(digitColor)
          g.DrawString(digit.ToString, Font, digitBrush, rect, format)
          If delta <> 0 Then
            g.DrawString(num.ToString, Font, digitBrush, layoutRectangle, format)
          End If
        End Using
      Else
        Dim color2 As Color = digitColor
        If delta >= 0.6 Then
          color2 = GraphicsUtils.TransparentColor(digitColor, (1.0! - ((delta - 0.6!) * 2.0!)))
        End If
        Using br As New SolidBrush(color2)
          g.DrawString(digit.ToString, Font, br, rect, format)
        End Using
        If delta <> 0 Then
          Dim color3 As Color = GraphicsUtils.TransparentColor(digitColor, delta)
          Using br As New SolidBrush(color3)
            g.DrawString(num.ToString, Font, br, layoutRectangle, format)
          End Using
        End If
      End If
      g.EndContainer(container)
    End Sub

    Private Sub DrawNumber(ByVal g As Graphics)
      Dim wrc As Rectangle = DisplayRectangle
      Dim size As Size = size.Ceiling(g.MeasureString("8", Font))
      Dim num As Integer = (wrc.Width \ (size.Width + 2))
      Dim numArray As Integer() = New Integer(num - 1) {}
      Dim format As String = String.Empty
      For i As Integer = 0 To num - 1
        format &= "0"
      Next i
      Dim text2 As String = Math.Floor(CDbl(value_)).ToString(format)
      For j As Integer = 0 To num - 1
        numArray(j) = Short.Parse(text2.Chars(j).ToString)
      Next j
      For k As Integer = 0 To num - 1
        Dim index As Integer = num - k - 1
        Dim digit As Integer = numArray(index)
        Dim rect As New Rectangle(((wrc.X + wrc.Width) - ((k + 1) * (size.Width + 2))), wrc.Y, size.Width, wrc.Height)
        Dim delta As Single = 0.0!
        If k = 0 Then
          delta = value_ - CSng(Math.Floor(CDbl(value_)))
        Else
          Dim flag As Boolean = True
          For m As Integer = (index + 1) To num - 1
            If numArray(m) <> 9 Then flag = False
          Next m
          If flag Then delta = (value_ - CSng(Math.Floor(CDbl(value_))))
        End If
        Dim color As Color
        If k = 0 Then
          color = firstDigitBackColor_
        Else
          color = BackColor
        End If
        Dim digitColor As Color
        If k = 0 Then
          digitColor = firstDigitForeColor_
        Else
          digitColor = ForeColor
        End If
        DrawDigit(g, rect, color, digitColor, digit, delta)
      Next k
    End Sub

    Protected Overridable Sub OnValueChanged(ByVal e As EventArgs)
      RaiseEvent ValueChanged(Me, e)
    End Sub

    ' Properties
    <DefaultValue(GetType(Color), "Red"), Description("Back Color of the first digit"), Category("Appearance")> _
    Public Property FirstDigitBackColor() As Color
      Get
        Return firstDigitBackColor_
      End Get
      Set(ByVal value As Color)
        If firstDigitBackColor_ <> value Then firstDigitBackColor_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Fore Color of the first digit"), DefaultValue(GetType(Color), "White")> _
    Public Property FirstDigitForeColor() As Color
      Get
        Return firstDigitForeColor_
      End Get
      Set(ByVal value As Color)
        If firstDigitForeColor_ <> value Then firstDigitForeColor_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Gets or sets whether control should use gradient background"), DefaultValue(False)> _
    Public Property ShowGradient() As Boolean
      Get
        Return showGradient_
      End Get
      Set(ByVal value As Boolean)
        If showGradient_ <> value Then showGradient_ = value : Invalidate()
      End Set
    End Property

    <Description("The odometer appearance style"), DefaultValue(GetType(OdometerStyle), "Mechanic"), _
     Category("Appearance")> _
    Public Property Style() As OdometerStyle
      Get
        Return style_
      End Get
      Set(ByVal value As OdometerStyle)
        If style_ <> value Then style_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(CSng(0.0!)), Category("Behavior"), Description("The position of the marker")> _
    Public Property Value() As Single
      Get
        Return value_
      End Get
      Set(ByVal value As Single)
        If value_ <> value Then
          value_ = value
          OnValueChanged(EventArgs.Empty)
          Invalidate()
        End If
      End Set
    End Property
  End Class

  Public Class PowerSwitch : Inherits ButtonBase
    Private activeLedColor_ As Color = Color.LightGreen, inactiveLedColor_ As Color = Color.Green

    Public Sub New()
      Behaviour = ButtonBehaviour.CheckBox
    End Sub

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      DrawToggle(e.Graphics)
    End Sub

    Private Sub DrawToggle(ByVal g As Graphics)
      Dim y As Integer

      g.SmoothingMode = SmoothingMode.AntiAlias
      Dim workRectangle As Rectangle = Me.DisplayRectangle
      Dim point As New Point((workRectangle.X + (workRectangle.Width \ 2)), (workRectangle.Y + (workRectangle.Height \ 2)))
      Dim num As Integer = Math.Min(CInt(((workRectangle.Width / 2) - 2)), CInt(((workRectangle.Height / 6) - 2)))
      Dim num2 As Integer = CInt((CSng(num) / 1.5!))
      Dim num3 As Integer = CInt((CSng(num) / 2.3!))
      If MyBase.Pressed Then
        y = (point.Y - (num * 2))
      Else
        y = (point.Y + (num * 2))
      End If
      Dim rect As New Rectangle((point.X - num2), (point.Y - num2), (num2 * 2), (num2 * 2))
      Using brush As New LinearGradientBrush(rect, Color.Gainsboro, Color.Gray, 45)
        g.FillEllipse(brush, rect)
      End Using


      Dim brPoly As Brush
      If MyBase.Pressed Then
        brPoly = New SolidBrush(Color.DarkGray)
      Else
        brPoly = New SolidBrush(Color.Gainsboro)
      End If
      Dim points As Point() = New Point() {New Point((point.X - num3), point.Y), New Point((point.X + num3), point.Y), New Point((point.X + num), y), New Point((point.X - num), y)}
      Dim rectangle3 As New Rectangle((point.X - num3), (point.Y - num3), (num3 * 2), (num3 * 2))
      g.FillEllipse(brPoly, rectangle3)
      g.FillPolygon(brPoly, points)
      brPoly.Dispose()

      Dim brEllipse As Brush
      If Not MyBase.Pressed Then
        brEllipse = New SolidBrush(Color.DarkGray)
      Else
        brEllipse = New SolidBrush(Color.Gainsboro)
      End If
      g.FillEllipse(brEllipse, (point.X - num), (y - num), (num * 2), (num * 2))
      brEllipse.Dispose()
      g.SmoothingMode = SmoothingMode.Default
    End Sub


    ' Properties
    <Category("Appearance"), DefaultValue(GetType(Color), "LightGreen"), Description("Active led color")> _
    Public Property ActiveLedColor() As Color
      Get
        Return activeLedColor_
      End Get
      Set(ByVal value As Color)
        If activeLedColor_ <> value Then activeLedColor_ = value : Invalidate()
      End Set
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(32, 48)
      End Get
    End Property

    <DefaultValue(GetType(Color), "Green"), Category("Appearance"), Description("Inactive led color")> _
    Public Property InactiveLedColor() As Color
      Get
        Return inactiveLedColor_
      End Get
      Set(ByVal value As Color)
        If inactiveLedColor_ <> value Then inactiveLedColor_ = value : Invalidate()
      End Set
    End Property
  End Class

  Public Class ProgressBar : Inherits BoundedValueControl
    Private startColor_ As Color = Color.DarkBlue, endColor_ As Color = Color.LightBlue, _
            smooth_ As Boolean

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      Dim g As Graphics = e.Graphics
      Dim wrc As Rectangle = Me.DisplayRectangle
      Dim num As Single = CSng((Value - Minimum) / (Maximum - Minimum))
      Dim num2 As Integer = CInt(num * wrc.Width)
      Dim width As Integer = wrc.Width
      g.FillRectangle(Brushes.Black, wrc)
      Dim workRectangle As Rectangle = Me.DisplayRectangle
      workRectangle.Inflate(1, 1)
      Using brush As New LinearGradientBrush(workRectangle, startColor_, endColor_, LinearGradientMode.Horizontal)
        If smooth_ Then
          If Value > Minimum Then
            g.FillRectangle(brush, wrc.Left, wrc.Top, num2, wrc.Height)
          End If
        Else
          Dim num3 As Integer = (wrc.Height - 1)
          Dim i As Integer = 0
          Do While (i + ((num3 + 1) / 2)) <= num2
            g.FillRectangle(brush, wrc.Left + i, wrc.Top, num3, wrc.Height)
            i += num3 + 1
          Loop
        End If
      End Using
    End Sub

    <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
    Public Overrides Property BackColor() As Color
      Get
        Return MyBase.BackColor
      End Get
      Set(ByVal value As Color)
        MyBase.BackColor = value
      End Set
    End Property

    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Overrides Property BackgroundImage() As Image
      Get
        Return MyBase.BackgroundImage
      End Get
      Set(ByVal value As Image)
        MyBase.BackgroundImage = value
      End Set
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(&H80, &H10)
      End Get
    End Property

    <Category("Appearance"), Description("End gradient color"), DefaultValue(GetType(Color), "LightBlue")> _
    Public Property EndColor() As Color
      Get
        Return endColor_
      End Get
      Set(ByVal value As Color)
        If endColor_ <> value Then endColor_ = value : Invalidate()
      End Set
    End Property

    <EditorBrowsable(EditorBrowsableState.Never), Browsable(False)> _
    Public Overrides Property Font() As Font
      Get
        Return MyBase.Font
      End Get
      Set(ByVal value As Font)
        MyBase.Font = value
      End Set
    End Property

    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Overrides Property ForeColor() As Color
      Get
        Return MyBase.ForeColor
      End Get
      Set(ByVal value As Color)
        MyBase.ForeColor = value
      End Set
    End Property

    <Description("Smooth indicator visualization style."), Category("Appearance"), DefaultValue(False)> _
    Public Property Smooth() As Boolean
      Get
        Return smooth_
      End Get
      Set(ByVal value As Boolean)
        If smooth_ <> value Then smooth_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "DarkBlue"), Description("Start gradient color")> _
    Public Property StartColor() As Color
      Get
        Return startColor_
      End Get
      Set(ByVal value As Color)
        If startColor_ <> value Then startColor_ = value : Invalidate()
      End Set
    End Property

    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Overrides Property [Text]() As String
      Get
        Return MyBase.Text
      End Get
      Set(ByVal value As String)
        MyBase.Text = value
      End Set
    End Property
  End Class


  Public Class RotarySlider : Inherits BoundedValueControl
    Private Const pix_ As Integer = 20
    Private mainInterval_ As Integer = 10, pointerColor_ As Color = Color.Red, _
            showGradient_ As Boolean, startVal_ As Double, startY_ As Integer, _
            subDivisions_ As Integer = 10

    Public Sub New()
      SetStyle(ControlStyles.Selectable, True)
    End Sub

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      DrawGauge(e.Graphics)
    End Sub

    Private Sub DrawGauge(ByVal g As Graphics)
      Dim rect As Rectangle = Me.DisplayRectangle
      Dim container As GraphicsContainer = g.BeginContainer
      g.IntersectClip(rect)
      Dim color As Color = GraphicsUtils.ScaleColor(BackColor, 0.5!)
      Dim rectangle2 As New Rectangle(rect.X, rect.Y, rect.Width, rect.Height \ 2)
      Using brush As New LinearGradientBrush(rectangle2, color, BackColor, LinearGradientMode.Vertical)
        g.FillRectangle(brush, rectangle2)
      End Using
      rectangle2 = New Rectangle(rect.X, (rect.Top + rect.Bottom) \ 2, rect.Width, rect.Height \ 2)
      Using brush As New LinearGradientBrush(rectangle2, BackColor, color, LinearGradientMode.Vertical)
        g.FillRectangle(brush, rectangle2)
      End Using
      Dim y As Integer = (rect.Top + rect.Bottom) \ 2
      Dim num2 As Integer = 20
      Dim num3 As Integer = rect.Height \ num2 + 1
      If ((num3 Mod 2) <> 0) Then num3 += 1
      Dim num4 As Integer = CInt(Value / mainInterval_)
      Dim num5 As Integer = (num4 * mainInterval_)
      Dim num6 As Integer = (num5 - (mainInterval_ * (num3 \ 2)))
      Dim num7 As Integer = (num6 + (mainInterval_ * num3))
      Using pen As New Pen(color.DarkGray), brush2 As New SolidBrush(ForeColor)
        Dim i As Integer = num6
        Do While (i <= num7)
          Dim num9 As Integer = (y + CInt(((CDbl((Value - i)) / CDbl(mainInterval_)) * num2)))
          g.DrawLine(pen, (rect.Left + 9), num9, (rect.Left + 12), num9)
          g.DrawLine(pen, (rect.Right - 9), num9, (rect.Right - 12), num9)
          For j As Integer = 1 To subDivisions_ - 1
            Dim num11 As Integer = (num9 - CInt(((CSng(num2) / CSng(subDivisions_)) * j)))
            g.DrawLine(pen, (rect.Left + 11), num11, (rect.Left + 12), num11)
            g.DrawLine(pen, (rect.Right - 11), num11, (rect.Right - 12), num11)
          Next j
          Dim num12 As Integer = (num9 - 1)
          Dim controlLightLight As Pen = SystemPens.ControlLightLight
          Dim controlDark As Pen = SystemPens.ControlDark
          g.DrawLine(controlLightLight, (rect.X + 1), num12, (rect.X + 5), num12)
          g.DrawLine(controlDark, rect.X + 1, num12 + 1, rect.X + 5, num12 + 1)
          g.DrawLine(controlLightLight, ((rect.X + rect.Width) - 1), num12, ((rect.X + rect.Width) - 5), num12)
          g.DrawLine(controlDark, rect.X + rect.Width - 1, num12 + 1, rect.X + rect.Width - 5, num12 + 1)
          num12 = ((num9 + (num2 \ 2)) - 1)
          g.DrawLine(controlLightLight, (rect.X + 1), num12, (rect.X + 5), num12)
          g.DrawLine(controlDark, rect.X + 1, num12 + 1, rect.X + 5, num12 + 1)
          g.DrawLine(controlLightLight, ((rect.X + rect.Width) - 1), num12, ((rect.X + rect.Width) - 5), num12)
          g.DrawLine(controlDark, rect.X + rect.Width - 1, num12 + 1, rect.X + rect.Width - 5, num12 + 1)
          Dim num13 As Single = (rect.Left + rect.Right) \ 2
          Dim num14 As Single = num9
          Dim text As String = ScaleAndFormatValue(i)
          Dim ef As SizeF = g.MeasureString([text], Font)
          num13 -= ef.Width / 2
          num14 -= ef.Height / 2
          g.DrawString([text], Font, brush2, CInt(num13), CInt(num14))
          i = (i + mainInterval_)
        Loop
      End Using
      Using brush3 As New SolidBrush(pointerColor_)
        Dim points As Point() = New Point() {New Point((rect.X + 5), y), New Point(rect.X, (y - 3)), New Point(rect.X, (y + 3))}
        g.FillPolygon(brush3, points)
        points = New Point() {New Point(((rect.X + rect.Width) - 5), y), New Point((rect.X + rect.Width), (y - 3)), New Point((rect.X + rect.Width), (y + 3))}
        g.FillPolygon(brush3, points)
      End Using
      g.EndContainer(container)
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
      If e.Button = MouseButtons.Left Then startY_ = e.Y : startVal_ = Value
      MyBase.OnMouseDown(e)
    End Sub

    Protected Overrides Function PointToValue(ByVal p As Point) As Double
      Dim num As Double = (startVal_ + CInt(((CDbl((-startY_ + p.Y)) / 20) * mainInterval_)))
      If num <= Minimum Then Return Minimum
      If num >= Maximum Then Return Maximum
      Return num
    End Function


    ' Properties
    Protected Overrides ReadOnly Property Editable() As Boolean
      Get
        Return True
      End Get
    End Property

    <Description("Color of the  pointer"), DefaultValue(GetType(Color), "Red"), Category("Appearance")> _
    Public Property PointerColor() As Color
      Get
        Return pointerColor_
      End Get
      Set(ByVal value As Color)
        If pointerColor_ <> value Then pointerColor_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Gets or sets whether control should use gradient background"), DefaultValue(False)> _
    Public Property ShowGradient() As Boolean
      Get
        Return showGradient_
      End Get
      Set(ByVal value As Boolean)
        If showGradient_ <> value Then showGradient_ = value : Invalidate()
      End Set
    End Property
  End Class




  Public Class Slider : Inherits SliderBase
    Private activeBarColor_ As Color = Color.Red
    Private activeLedColor_ As Color = Color.LightGreen
    Private barBorder_ As BorderStyle = BorderStyle.None
    Private inactiveBarColor_ As Color = Color.Gainsboro
    Private inactiveLedColor_ As Color = Color.Green
    Private sliderHotImage_ As Image
    Private sliderImage_ As Image
    Private sliderSize_ As New Size(20, 20)
    Private sliderStyle_ As SliderStyle = SliderStyle.Led

    Protected Function CreateColorsGradient(ByVal gradRect As Rectangle, ByVal k As Single) As LinearGradientBrush
      Dim color1 As Color = GraphicsUtils.ScaleColor(Color.Lime, k)
      Dim color2 As Color = GraphicsUtils.ScaleColor(Color.Yellow, k)
      Dim color3 As Color = GraphicsUtils.ScaleColor(Color.Red, k)
      Dim num1 As Integer
      If Orientation = Orientation.Horizontal Then
        num1 = 180
      Else
        num1 = 90
      End If

      Dim brush1 As LinearGradientBrush = New LinearGradientBrush(gradRect, color3, color1, CType(num1, Single))
      Dim blend1 As ColorBlend = New ColorBlend(6)
      blend1.Positions(0) = 0.0!
      blend1.Positions(1) = 0.1!
      blend1.Positions(2) = 0.3!
      blend1.Positions(3) = 0.45!
      blend1.Positions(4) = 0.8!
      blend1.Positions(5) = 1.0!
      blend1.Colors(0) = color3
      blend1.Colors(1) = color3
      blend1.Colors(2) = color2
      blend1.Colors(3) = color2
      blend1.Colors(4) = color1
      blend1.Colors(5) = color1
      brush1.InterpolationColors = blend1
      Return brush1
    End Function


    Protected Function CreatePath(ByVal ir As Rectangle) As GraphicsPath
      Dim ret As GraphicsPath = New GraphicsPath
      If (Me.Orientation = Orientation.Horizontal) Then
        ret.StartFigure()
        ret.AddLine(ir.X, ir.Y, (ir.X + ir.Width), ir.Y)
        ret.AddLine((ir.X + ir.Width), (ir.Y + ir.Height), ir.X, (ir.Y + ir.Height))
        ret.CloseFigure()
        Return ret
      End If
      ret.StartFigure()
      ret.AddLine(ir.X, ir.Y, ir.X, (ir.Y + ir.Height))
      ret.AddLine((ir.X + ir.Width), (ir.Y + ir.Height), (ir.X + ir.Width), ir.Y)
      ret.CloseFigure()
      Return ret
    End Function


    Protected Function CreateTransparencyGradient(ByVal gradRect As Rectangle) As LinearGradientBrush
      Dim color1 As Color = Color.FromArgb(128, 0, 0, 0)
      Color.FromArgb(128, 255, 255, 255)
      Color.FromArgb(0, 0, 0, 0)
      Dim color2 As Color = Color.FromArgb(100, 0, 0, 0)
      Dim num1 As Integer : If Orientation = Orientation.Horizontal Then num1 = 90

      Dim ret As LinearGradientBrush = New LinearGradientBrush(gradRect, color2, color1, CType(num1, Single))
      Dim blend1 As ColorBlend = New ColorBlend(4)
      blend1.Positions(0) = 0.0!
      blend1.Positions(1) = 0.25!
      blend1.Positions(2) = 0.5!
      blend1.Positions(3) = 1.0!
      blend1.Colors(0) = Color.FromArgb(128, 0, 0, 0)
      blend1.Colors(1) = Color.FromArgb(128, 255, 255, 255)
      blend1.Colors(2) = Color.FromArgb(0, 0, 0, 0)
      blend1.Colors(3) = Color.FromArgb(100, 0, 0, 0)
      ret.InterpolationColors = blend1
      Return ret
    End Function


    Protected Overrides Sub DrawBar(ByVal g As Graphics)
      Dim rectangle3 As Rectangle
      Dim path2 As GraphicsPath
      Dim point1 As Point
      g.SmoothingMode = SmoothingMode.AntiAlias
      Dim rectangle1 As Rectangle = MyBase.BarRectangle
      Dim path1 As GraphicsPath = Me.CreatePath(rectangle1)
      Dim rectangle2 As Rectangle = rectangle1
      If (Me.Orientation = Orientation.Horizontal) Then
        rectangle2.Inflate((rectangle1.Height \ 2), 0)
      Else
        rectangle2.Inflate(0, (rectangle1.Width \ 2))
      End If
      Dim brush1 As Brush = New SolidBrush(inactiveBarColor_)
      g.FillPath(brush1, path1)
      If (MyBase.Value > Me.Minimum) Then
        rectangle3 = rectangle1
        If (Me.Orientation = Orientation.Horizontal) Then
          point1 = ValueToPoint(MyBase.Value)
          rectangle3.Width = (point1.X - rectangle3.X)
        Else
          point1 = ValueToPoint(MyBase.Value)
          rectangle3.Y = point1.Y
          rectangle3.Height = ((rectangle1.Y + rectangle1.Height) - rectangle3.Y)
        End If
        path2 = CreatePath(rectangle3)
        brush1 = New SolidBrush(activeBarColor_)
        g.FillPath(brush1, path2)
      End If
      brush1 = CreateTransparencyGradient(rectangle2)
      g.FillPath(brush1, path1)
      g.SmoothingMode = SmoothingMode.Default
      Border.Draw(g, MyBase.BarRectangle, False, False, barBorder_)
    End Sub



    Protected Overrides Sub DrawSlider(ByVal g As Graphics)
      Dim rectangle1 As Rectangle
      Dim image1 As Image
      Dim brush1 As Brush
      Dim rectangle2 As Rectangle
      Dim format1 As StringFormat
      Dim size1 As Size
      Dim point1 As Point = ValueToPoint(MyBase.Value)
      rectangle1 = New Rectangle(New Point((point1.X - (sliderSize_.Width \ 2)), (point1.Y - (sliderSize_.Height \ 2))), sliderSize_)
      If (sliderStyle_ = SliderStyle.Image) Then
        If (sliderImage_ Is Nothing) Then
          Return
        End If
        image1 = sliderImage_
        If (MyBase.Capture AndAlso (Not sliderHotImage_ Is Nothing)) Then
          image1 = sliderHotImage_
        End If
        size1 = image1.Size
        size1 = image1.Size
        g.DrawImageUnscaled(image1, (point1.X - (size1.Width \ 2)), (point1.Y - (size1.Height \ 2)))
        Return
      End If
      Dim angle As Single = 45
      If Capture AndAlso sliderStyle_ = SliderStyle.Value Then angle += 180
      g.FillRectangle(New LinearGradientBrush(rectangle1, Color.White, Color.Gray, angle), rectangle1)
      Border.Draw(g, rectangle1, False, False, BorderStyle.Raised)
      If (sliderStyle_ = SliderStyle.Led) Then
        If MyBase.Capture Then
          brush1 = New SolidBrush(activeLedColor_)
        Else
          brush1 = New SolidBrush(inactiveLedColor_)
        End If
        rectangle2 = rectangle1
        rectangle2.Inflate(-3, -3)
        g.FillRectangle(brush1, rectangle2)
      End If
      If (sliderStyle_ = SliderStyle.Value) Then
        format1 = New StringFormat
        format1.LineAlignment = StringAlignment.Center
        format1.Alignment = StringAlignment.Center
        g.DrawString(MyBase.ScaleAndFormatValue(MyBase.Value), Font, New SolidBrush(ForeColor), RectangleF.op_Implicit(rectangle1), format1)
      End If
    End Sub


    <Category("Appearance"), Description("Color of the active part of bar"), DefaultValue(GetType(Color), "Red")> _
    Public Property ActiveBarColor() As Color
      Get
        Return activeBarColor_
      End Get
      Set(ByVal value As Color)
        If Not value.Equals(activeBarColor_) Then activeBarColor_ = value : Invalidate()
      End Set
    End Property


    <Category("Appearance"), Description("Led color in active state"), DefaultValue(GetType(Color), "LightGreen")> _
    Public Property ActiveLedColor() As Color
      Get
        Return activeLedColor_
      End Get
      Set(ByVal value As Color)
        If Not value.Equals(activeLedColor_) Then activeLedColor_ = value : Invalidate()
      End Set
    End Property


    <DefaultValue(GetType(BorderStyle), "None"), Category("Appearance"), Description("Style of slider bar border")> _
    Public Property BarBorder() As BorderStyle
      Get
        Return barBorder_
      End Get
      Set(ByVal value As BorderStyle)
        If value <> barBorder_ Then barBorder_ = value : Invalidate()
      End Set
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(192, 100)
      End Get
    End Property

    <Category("Appearance"), Description("Color of the inactive part of bar"), DefaultValue(GetType(Color), "Gainsboro")> _
    Public Property InactiveBarColor() As Color
      Get
        Return inactiveBarColor_
      End Get
      Set(ByVal value As Color)
        If Not value.Equals(inactiveBarColor_) Then inactiveBarColor_ = value : Invalidate()
      End Set
    End Property

    <Description("Led color in inactive state"), Category("Appearance"), DefaultValue(GetType(Color), "Green")> _
    Public Property InactiveLedColor() As Color
      Get
        Return inactiveLedColor_
      End Get
      Set(ByVal value As Color)
        If Not value.Equals(inactiveLedColor_) Then inactiveLedColor_ = value : Invalidate()
      End Set
    End Property


    <Description("Image for slider in hot state"), Category("Appearance")> _
    Public Property SliderHotImage() As Image
      Get
        Return sliderHotImage_
      End Get
      Set(ByVal value As Image)
        If Not value Is sliderHotImage_ Then sliderHotImage_ = value : Invalidate()
      End Set
    End Property

    Private Function ShouldSerializeSliderHotImage() As Boolean
      Return sliderHotImage_ IsNot Nothing
    End Function

    <Description("Image for slider"), Category("Appearance")> _
    Public Property SliderImage() As Image
      Get
        Return sliderImage_
      End Get
      Set(ByVal value As Image)
        If Not value Is sliderImage_ Then sliderImage_ = value : Invalidate()
      End Set
    End Property

    Private Function ShouldSerializeSliderImage() As Boolean
      Return sliderImage_ IsNot Nothing
    End Function

    <Description("Size of the slider"), Category("Appearance")> _
    Public Property SliderSize() As Size
      Get
        Return sliderSize_
      End Get
      Set(ByVal value As Size)
        If Not value.Equals(sliderSize_) Then sliderSize_ = value : Invalidate()
      End Set
    End Property


    <Description("Style of slider "), Category("Appearance"), DefaultValue(GetType(SliderStyle), "Led")> _
    Public Property SliderStyle() As SliderStyle
      Get
        Return sliderStyle_
      End Get
      Set(ByVal value As SliderStyle)
        If value <> sliderStyle_ Then sliderStyle_ = value : Invalidate()
      End Set
    End Property
  End Class

  Public Enum SliderStyle
    Image = 2
    Led = 0
    Value = 1
  End Enum




  Public MustInherit Class SliderBase : Inherits LinearControlBase
    Private barIndent_ As Integer = 10, barWidth_ As Integer = 10, indicatorWidth_ As Integer = 20

    Protected Sub New()
    End Sub

    <Category("Appearance"), DefaultValue(10), Description("Left and right indents")> _
    Public Property BarIndent() As Integer
      Get
        Return barIndent_
      End Get
      Set(ByVal value As Integer)
        If barIndent_ <> value Then barIndent_ = value : Invalidate()
      End Set
    End Property


    Protected ReadOnly Property BarRectangle() As Rectangle
      Get
        Dim rectangle1 As Rectangle = Me.DisplayRectangle
        If Orientation = Orientation.Horizontal Then
          Return New Rectangle((rectangle1.X + barIndent_), (Me.CenterLine - (barWidth_ \ 2)), (rectangle1.Width - (barIndent_ * 2)), barWidth_)
        End If
        Return New Rectangle((Me.CenterLine - (barWidth_ \ 2)), (rectangle1.Y + barIndent_), barWidth_, (rectangle1.Height - (barIndent_ * 2)))
      End Get
    End Property


    <Category("Appearance"), Description("Bar Width"), DefaultValue(10)> _
    Public Property BarWidth() As Integer
      Get
        Return barWidth_
      End Get
      Set(ByVal value As Integer)
        If barWidth_ <> value Then barWidth_ = value : Invalidate()
      End Set
    End Property


    Protected Overrides ReadOnly Property CenterLine() As Integer
      Get
        Dim num1 As Integer
        Dim num2 As Integer
        Dim rectangle1 As Rectangle = Me.DisplayRectangle
        If (Me.Orientation = Orientation.Vertical) Then
          num2 = rectangle1.X
          num1 = rectangle1.Width
        Else
          num2 = rectangle1.Y
          num1 = rectangle1.Height
        End If
        Dim num3 As Integer = (num2 + (num1 \ 2))
        Dim position1 As SliderElementsPosition = MyBase.LabelsPosition
        Select Case position1
          Case SliderElementsPosition.TopLeft
            Dim ret As Integer = (num2 + num1) - (indicatorWidth_ \ 2)
            If TicksPosition = SliderElementsPosition.BottomRight OrElse _
                 TicksPosition = SliderElementsPosition.Both Then ret -= TicksLength + 2
            Return ret
          Case SliderElementsPosition.BottomRight
            Dim ret As Integer = num2 + (indicatorWidth_ \ 2)
            If TicksPosition = SliderElementsPosition.TopLeft OrElse _
               TicksPosition = SliderElementsPosition.Both Then ret += TicksLength + 2
            Return ret
        End Select
        Return num3
      End Get
    End Property


    Protected Overrides ReadOnly Property IndicatorRectangle() As Rectangle
      Get
        Dim rectangle1 As Rectangle = Me.DisplayRectangle
        If (Me.Orientation = Orientation.Horizontal) Then
          Return New Rectangle((rectangle1.X + barIndent_), (Me.CenterLine - (indicatorWidth_ \ 2)), (rectangle1.Width - (barIndent_ * 2)), indicatorWidth_)
        End If
        Return New Rectangle((Me.CenterLine - (indicatorWidth_ \ 2)), (rectangle1.Y + barIndent_), indicatorWidth_, (rectangle1.Height - (barIndent_ * 2)))
      End Get
    End Property


    <Description("Bar Width"), Category("Appearance"), DefaultValue(20)> _
    Public Property IndicatorWidth() As Integer
      Get
        Return indicatorWidth_
      End Get
      Set(ByVal value As Integer)
        If (indicatorWidth_ <> value) Then
          indicatorWidth_ = value
          MyBase.Invalidate()
        End If
      End Set
    End Property
  End Class


  Public Class SlidingGauge : Inherits ValueFormatControl
    Private mainInterval_ As Integer = 10, pointerColor_ As Color = Color.Red, _
            showGradient_ As Boolean, subDivisions_ As Integer = 10

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      DrawGauge(e.Graphics)
    End Sub

    Private Sub DrawGauge(ByVal g As Graphics)
      Dim rect As Rectangle = DisplayRectangle
      Dim container As GraphicsContainer = g.BeginContainer
      g.IntersectClip(rect)
      If showGradient_ Then
        Dim rectangle2 As New Rectangle(rect.X, rect.Y, rect.Width, rect.Height \ 2)
        Using brush As New LinearGradientBrush(rectangle2, Color.Black, BackColor, LinearGradientMode.Vertical)
          g.FillRectangle(brush, rectangle2)
        End Using
        rectangle2 = New Rectangle(rect.X, (rect.Top + rect.Bottom) \ 2, rect.Width, rect.Height \ 2)
        Using brush As New LinearGradientBrush(rectangle2, BackColor, Color.Black, LinearGradientMode.Vertical)
          g.FillRectangle(brush, rectangle2)
        End Using
      End If
      Dim num As Integer = (rect.Top + rect.Bottom) \ 2
      Dim num2 As Integer = 20
      Dim num3 As Integer = ((rect.Height \ num2) + 1)
      If ((num3 Mod 2) <> 0) Then
        num3 += 1
      End If
      Dim num4 As Integer = CInt(Value / mainInterval_)
      Dim num5 As Integer = (num4 * mainInterval_)
      Dim num6 As Integer = (num5 - (mainInterval_ * (num3 \ 2)))
      Dim num7 As Integer = (num6 + (mainInterval_ * num3))
      Using forePen As New Pen(ForeColor), foreBrush As New SolidBrush(ForeColor)
        For i As Integer = num6 To num7 Step mainInterval_
          Dim num9 As Integer = (num + CInt(((CDbl((Value - i)) / CDbl(mainInterval_)) * num2)))
          g.DrawLine(forePen, (rect.Left + 6), num9, (rect.Left + 12), num9)
          g.DrawLine(forePen, (rect.Right - 6), num9, (rect.Right - 12), num9)
          For j As Integer = 1 To subDivisions_ - 1
            Dim num11 As Integer = (num9 - CInt(((CSng(num2) / CSng(subDivisions_)) * j)))
            g.DrawLine(forePen, (rect.Left + 9), num11, (rect.Left + 12), num11)
            g.DrawLine(forePen, (rect.Right - 9), num11, (rect.Right - 12), num11)
          Next j
          Dim num12 As Single = rect.X + rect.Width \ 2
          Dim num13 As Single = num9
          Dim text As String = ScaleAndFormatValue(i)
          Dim ef As SizeF = g.MeasureString([text], Font)
          num12 = (num12 - (ef.Width / 2.0!))
          num13 = (num13 - (ef.Height / 2.0!))
          g.DrawString([text], Font, foreBrush, CInt(num12), CInt(num13))
        Next i
      End Using
      Using pointerPen As New Pen(pointerColor_)
        g.DrawLine(pointerPen, rect.Left, num, rect.Right, num)
      End Using
      g.EndContainer(container)
    End Sub


    ' Properties
    <DefaultValue(GetType(Color), "Red"), Description("Color of the  pointer"), Category("Appearance")> _
    Public Property PointerColor() As Color
      Get
        Return pointerColor_
      End Get
      Set(ByVal value As Color)
        If pointerColor_ <> value Then pointerColor_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(False), Category("Appearance"), Description("Gets or sets whether control should use gradient background")> _
    Public Property ShowGradient() As Boolean
      Get
        Return showGradient_
      End Get
      Set(ByVal value As Boolean)
        If showGradient_ <> value Then showGradient_ = value : Invalidate()
      End Set
    End Property
  End Class

  Public Class Switch : Inherits ButtonBase
    Private activeLedColor_ As Color = Color.LightGreen, depth_ As Integer = 4, _
            faceStyle_ As FaceStyleValue = FaceStyleValue.RaisedNotch, inactiveLedColor_ As Color = Color.Green

    Public Enum FaceStyleValue
      Smooth
      RaisedNotch
      LoweredNotch
    End Enum

    Public Sub New()
      Behaviour = ButtonBehaviour.CheckBox
    End Sub

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      DrawToggle(e.Graphics)
    End Sub

    Private Sub DrawToggle(ByVal g As Graphics)
      Dim wrc As Rectangle = DisplayRectangle
      If Pressed Then
        g.FillRectangle(Brushes.Silver, wrc.X, wrc.Y, wrc.Width, wrc.Height \ 2)
        g.FillRectangle(Brushes.Gainsboro, wrc.X, (wrc.Top + wrc.Bottom) \ 2, wrc.Width, wrc.Height \ 2 - depth_)
        g.FillRectangle(Brushes.Gray, wrc.X, wrc.Bottom - depth_, wrc.Width, depth_)
      Else
        g.FillRectangle(Brushes.Gainsboro, wrc.X, wrc.Y, wrc.Width, depth_)
        g.FillRectangle(Brushes.DarkGray, wrc.X, wrc.Y + depth_, wrc.Width, wrc.Height \ 2 - depth_)
        g.FillRectangle(Brushes.Silver, wrc.X, (wrc.Top + wrc.Bottom) \ 2, wrc.Width, wrc.Height \ 2)
      End If
      Dim brush As Brush
      If Pressed Then
        brush = New SolidBrush(activeLedColor_)
      Else
        brush = New SolidBrush(inactiveLedColor_)
      End If
      Dim rect As New Rectangle((wrc.X + 3), ((wrc.Y + depth_) + 3), (wrc.Width - 7), 4)
      If Pressed Then rect.Y -= depth_
      g.FillRectangle(brush, rect)
      brush.Dispose()
      If faceStyle_ <> FaceStyleValue.Smooth Then
        Dim h As Integer = wrc.Height - 4
        If Pressed Then h -= depth_
        Dim controlLightLight As Pen = SystemPens.ControlLightLight
        Dim pen As Pen = SystemPens.ControlDark
        If faceStyle_ = FaceStyleValue.RaisedNotch Then
          pen = SystemPens.ControlLightLight
          controlLightLight = SystemPens.ControlDark
        End If
        For y As Integer = (wrc.Top + wrc.Bottom) \ 2 + 2 To h Step 3
          g.DrawLine(pen, wrc.X, wrc.Y + y, wrc.Right - 4, wrc.Y + y)
          g.DrawLine(controlLightLight, wrc.Left + 2, wrc.Top + y + 1, wrc.Right - 4, wrc.Y + y + 1)
        Next y
      End If
    End Sub


    ' Properties
    <Category("Appearance"), DefaultValue(GetType(Color), "LightGreen"), Description("Active led color")> _
    Public Property ActiveLedColor() As Color
      Get
        Return activeLedColor_
      End Get
      Set(ByVal value As Color)
        If activeLedColor_ <> value Then activeLedColor_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(GetType(ButtonBehaviour), "CheckBox")> _
    Public Overrides Property Behaviour() As ButtonBehaviour
      Get
        Return MyBase.Behaviour
      End Get
      Set(ByVal value As ButtonBehaviour)
        MyBase.Behaviour = value
      End Set
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(32, 48)
      End Get
    End Property

    <Description("Press depth"), DefaultValue(4), Category("Appearance")> _
    Public Property Depth() As Integer
      Get
        Return depth_
      End Get
      Set(ByVal value As Integer)
        If depth_ <> value Then depth_ = value : Invalidate()
      End Set
    End Property

    <Description("Control face appearrance"), Category("Appearance"), DefaultValue(GetType(FaceStyleValue), "RaisedNotch")> _
    Public Property FaceStyle() As FaceStyleValue
      Get
        Return faceStyle_
      End Get
      Set(ByVal value As FaceStyleValue)
        If faceStyle_ <> value Then faceStyle_ = value : Invalidate()
      End Set
    End Property

    <Description("Inactive led color"), Category("Appearance"), DefaultValue(GetType(Color), "Green")> _
    Public Property InactiveLedColor() As Color
      Get
        Return inactiveLedColor_
      End Get
      Set(ByVal value As Color)
        If inactiveLedColor_ <> value Then inactiveLedColor_ = value : Invalidate()
      End Set
    End Property
  End Class


  Public Class Tank : Inherits SliderBase
    Private depth_ As Integer = 8, liquidColor_ As Color = Color.Blue, _
            tankColor_ As Color = Color.Gainsboro, tankWidth_ As Integer = 3

    Public Sub New()
      Orientation = Orientation.Vertical
      BarWidth = 30
      IndicatorWidth = 35
      BarIndent = 25
    End Sub

    Private Function CreatePath(ByVal ir As Rectangle) As GraphicsPath
      Dim ret As New GraphicsPath
      If Orientation = Orientation.Horizontal Then
        ret.StartFigure()
        ret.AddLine(ir.X, ir.Y, (ir.X + ir.Width), ir.Y)
        ret.AddArc(((ir.X + ir.Width) - (ir.Height \ 2)), ir.Y, ir.Height, ir.Height, -90.0!, 180.0!)
        ret.AddLine((ir.X + ir.Width), (ir.Y + ir.Height), ir.X, (ir.Y + ir.Height))
        ret.AddArc((ir.X - (ir.Height \ 2)), ir.Y, ir.Height, ir.Height, 90.0!, 180.0!)
        ret.CloseFigure()
        Return ret
      End If
      ret.StartFigure()
      ret.AddLine(ir.X, ir.Y, ir.X, (ir.Y + ir.Height))
      ret.AddArc(ir.X, ((ir.Y - (ir.Width \ 2)) + ir.Height), ir.Width, ir.Width, 0.0!, 180.0!)
      ret.AddLine((ir.X + ir.Width), (ir.Y + ir.Height), (ir.X + ir.Width), ir.Y)
      ret.CloseFigure()
      Return ret
    End Function

    Private Function CreateTransparencyGradient(ByVal gradRect As Rectangle) As LinearGradientBrush
      Dim color As Color = color.FromArgb(128, 0, 0, 0)
      color.FromArgb(128, 255, 255, 255)
      color.FromArgb(0, 0, 0, 0)
      Dim color2 As Color = color.FromArgb(100, 0, 0, 0)
      Dim num As Integer : If Orientation = Orientation.Horizontal Then num = 90
      Dim ret As New LinearGradientBrush(gradRect, color2, color, CSng(num))
      Dim blend As New ColorBlend(4)
      blend.Positions(0) = 0.0!
      blend.Positions(1) = 0.25!
      blend.Positions(2) = 0.5!
      blend.Positions(3) = 1.0!
      blend.Colors(0) = color.FromArgb(128, 0, 0, 0)
      blend.Colors(1) = color.FromArgb(128, 255, 255, 255)
      blend.Colors(2) = color.FromArgb(0, 0, 0, 0)
      blend.Colors(3) = color.FromArgb(100, 0, 0, 0)
      ret.InterpolationColors = blend
      Return ret
    End Function

    Protected Overrides Sub DrawBar(ByVal g As Graphics)
      g.SmoothingMode = SmoothingMode.AntiAlias
      Dim ir As Rectangle = BarRectangle
      ir.Y -= ir.Width \ 3
      ir.Height += ir.Width \ 3
      Using path As GraphicsPath = CreatePath(ir)
        Dim gradRect As Rectangle = ir
        If Orientation = Orientation.Horizontal Then
          gradRect.Inflate(ir.Height \ 2, 0)
        Else
          gradRect.Inflate(0, ir.Width \ 2)
        End If
        Using brush As Brush = New SolidBrush(tankColor_)
          g.FillPath(brush, path)
        End Using
        If Value > Minimum Then
          Dim rectangle3 As Rectangle = ir
          If Orientation = Orientation.Horizontal Then
            rectangle3.Width = ValueToPoint(Value).X - rectangle3.X
          Else
            rectangle3.Y = ValueToPoint(Value).Y
            rectangle3.Height = ((ir.Y + ir.Height) - rectangle3.Y)
          End If
          rectangle3.Inflate(-tankWidth_, 0)
          rectangle3.Height = (rectangle3.Height - tankWidth_)
          Using path2 As GraphicsPath = CreatePath(rectangle3)
            Using brush2 As Brush = New SolidBrush(GraphicsUtils.ScaleColor(liquidColor_, 0.7!))
              g.FillPath(brush2, path2)
            End Using
          End Using
          If depth_ > 0 Then
            Dim point As Point = ValueToPoint(Value)
            Dim rect As New Rectangle(ir.X, (point.Y - depth_), ir.Width, (depth_ * 2))
            rect.Inflate(-tankWidth_, -tankWidth_)
            Using brush3 As Brush = New SolidBrush(GraphicsUtils.ScaleColor(liquidColor_, 1.0!))
              g.FillEllipse(brush3, rect)
            End Using
          End If
        End If
        Using brush4 As Brush = CreateTransparencyGradient(gradRect)
          g.FillPath(brush4, path)
        End Using
        If depth_ > 0 Then
          Dim rectangle5 As New Rectangle(ir.X, ir.Y - depth_, ir.Width, depth_ * 2)
          Using brush5 As Brush = New SolidBrush(GraphicsUtils.ScaleColor(tankColor_, 0.8!))
            g.FillEllipse(brush5, rectangle5)
          End Using
          rectangle5.Inflate(-tankWidth_, -tankWidth_)
          Using brush6 As Brush = New SolidBrush(GraphicsUtils.ScaleColor(tankColor_, 1.0!))
            g.FillEllipse(brush6, rectangle5)
          End Using
        End If
        g.SmoothingMode = SmoothingMode.Default
      End Using
    End Sub


    ' Properties
    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(100, 200)
      End Get
    End Property

    <Description("3D depth"), Category("Appearance"), DefaultValue(8)> _
    Public Property Depth() As Integer
      Get
        Return depth_
      End Get
      Set(ByVal value As Integer)
        If depth_ <> value Then depth_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "Blue"), Description("Color of the liquid")> _
    Public Property LiquidColor() As Color
      Get
        Return liquidColor_
      End Get
      Set(ByVal value As Color)
        If liquidColor_ <> value Then liquidColor_ = value : Invalidate()
      End Set
    End Property

    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property Orientation() As Orientation
      Get
        Return MyBase.Orientation
      End Get
      Set(ByVal value As Orientation)
        If value <> Orientation.Vertical Then
          Throw New Exception("Horizontal orientation not supported")
        End If
        MyBase.Orientation = value
      End Set
    End Property

    <Category("Appearance"), Description("Color of the tank"), DefaultValue(GetType(Color), "Gainsboro")> _
    Public Property TankColor() As Color
      Get
        Return tankColor_
      End Get
      Set(ByVal value As Color)
        If tankColor_ <> value Then tankColor_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Width of the tank"), DefaultValue(3)> _
    Public Property TankWidth() As Integer
      Get
        Return tankWidth_
      End Get
      Set(ByVal value As Integer)
        If tankWidth_ <> value Then tankWidth_ = value : Invalidate()
      End Set
    End Property
  End Class

  Public Class Thermometer : Inherits BoundedValueControl
    Private largeTickFrequency_ As Integer = 10, numberFrequency_ As Integer = 20, _
            pointerColor_ As Color = Color.Red, showNumbers_ As Boolean = True, _
            smallTickFrequency_ As Integer = 2
    Private Const r1 As Integer = 1, r2 As Integer = 5

    ' Want this back
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
      Public Overrides Property Text() As String
      Get
        Return MyBase.Text
      End Get
      Set(ByVal value As String)
        MyBase.Text = value
      End Set
    End Property

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      Dim g As Graphics = e.Graphics
      Dim crc As Rectangle = DisplayRectangle
      Using pen As New Pen(ForeColor), pointerBrush As New SolidBrush(pointerColor_), _
            foreBrush As New SolidBrush(ForeColor)
        Dim top As Integer = crc.Y + 7, bottom As Integer = crc.Bottom - 23, _
            x As Integer = crc.Right - 23 - Font.Height
        g.DrawArc(pen, x - 4, top - 4, 8, 8, 180, 180)
        g.DrawLine(pen, x - 4, top, x - 4, bottom + 3)
        g.DrawLine(pen, x + 4, top, x + 4, bottom + 3)

        Dim num4 As Integer = bottom + 10
        g.FillEllipse(pointerBrush, x - 5, num4 - 5, 10, 10)
        Dim val As Integer = CInt(((Value - Minimum) * (bottom - top)) / (Maximum - Minimum))
        g.FillRectangle(pointerBrush, x - 1, bottom - val, 3, val + 8)
        g.DrawArc(pen, x - 8, num4 - 8, 16, 16, -60, 300)

        For i As Integer = Minimum To Maximum Step smallTickFrequency_
          Dim y As Integer = bottom - ((i - Minimum) * (bottom - top)) \ (Maximum - Minimum)
          Dim w As Integer = 3 : If i Mod largeTickFrequency_ = 0 Then w = 7
          If showNumbers_ AndAlso i Mod numberFrequency_ = 0 Then
            Dim txt As String = ScaleAndFormatValue(i)
            Dim siz As Size = g.MeasureString(txt, Font).ToSize
            g.DrawString(txt, Font, foreBrush, x - 14 - siz.Width, y + 2 - siz.Height \ 2)
          End If
          g.DrawLine(pen, x - 7, y, x - 7 - w, y)
          g.DrawLine(pen, x + 7, y, x + 7 + w, y)
        Next i

        If Not String.IsNullOrEmpty(Text) Then
          Dim siz As Size = g.MeasureString(Text, Font).ToSize
          Dim container As Drawing2D.GraphicsContainer = g.BeginContainer
          g.RotateTransform(-90)
          g.DrawString(Text, Font, foreBrush, -(DisplayRectangle.Height \ 2 + siz.Width \ 2), x + 14 + siz.Height \ 2)
          g.EndContainer(container)
        End If
      End Using
    End Sub

    Protected Overrides Sub OnTextChanged(ByVal e As EventArgs)
      Invalidate()
      MyBase.OnTextChanged(e)
    End Sub


    ' Properties
    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(80, 184)
      End Get
    End Property

    <DefaultValue(10), Description("Define frequency of large ticks"), Category("Appearance")> _
    Public Property LargeTickFrequency() As Integer
      Get
        Return largeTickFrequency_
      End Get
      Set(ByVal value As Integer)
        If largeTickFrequency_ <> value Then largeTickFrequency_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Define frequency of numbers"), DefaultValue(20)> _
    Public Property NumberFrequency() As Integer
      Get
        Return numberFrequency_
      End Get
      Set(ByVal value As Integer)
        If numberFrequency_ <> value Then numberFrequency_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(GetType(Color), "Red"), Description("Color of the  pointer"), Category("Appearance")> _
    Public Property PointerColor() As Color
      Get
        Return pointerColor_
      End Get
      Set(ByVal value As Color)
        If pointerColor_ <> value Then pointerColor_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Indicate if the control paints scale numbers"), DefaultValue(True)> _
    Public Property ShowNumbers() As Boolean
      Get
        Return showNumbers_
      End Get
      Set(ByVal value As Boolean)
        If showNumbers_ <> value Then showNumbers_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Define frequency of small ticks"), DefaultValue(2)> _
    Public Property SmallTickFrequency() As Integer
      Get
        Return smallTickFrequency_
      End Get
      Set(ByVal value As Integer)
        If smallTickFrequency_ <> value Then smallTickFrequency_ = value : Invalidate()
      End Set
    End Property
  End Class


  Public Class Toggle : Inherits ButtonBase
    Private activeLedColor_ As Color = Color.LightGreen, inactiveLedColor_ As Color = Color.Green, _
            faceStyle_ As FaceStyleValue = FaceStyleValue.RaisedNotch

    Public Enum FaceStyleValue
      Smooth
      RaisedNotch
      LoweredNotch
    End Enum

    Public Sub New()
      Behaviour = ButtonBehaviour.CheckBox
    End Sub

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      Me.DrawToggle(e.Graphics)
    End Sub

    Private Sub DrawToggle(ByVal g As Graphics)
      Dim wrc As Rectangle = DisplayRectangle
      wrc.Height \= 2
      If Not Pressed Then wrc.Y += wrc.Height
      wrc.Width -= 1 : wrc.Height -= 1
      Dim location As Point = wrc.Location
      Dim point2 As Point = wrc.Location + wrc.Size
      Dim controlLightLight As Pen = SystemPens.ControlLightLight
      Dim pen As Pen = SystemPens.ControlDark
      Dim rectangle3 As Rectangle = wrc
      Using brush As Brush = New LinearGradientBrush(rectangle3, Color.White, Color.Gray, LinearGradientMode.Horizontal)
        g.FillRectangle(brush, wrc)
      End Using

      g.DrawLine(controlLightLight, location.X, location.Y, location.X, point2.Y)
      g.DrawLine(controlLightLight, location.X, location.Y, point2.X, location.Y)
      g.DrawLine(pen, location.X, point2.Y, point2.X, point2.Y)
      g.DrawLine(pen, point2.X, location.Y, point2.X, point2.Y)
      If faceStyle_ <> FaceStyleValue.Smooth Then
        For y As Integer = 8 To wrc.Height - 4 Step 3
          If faceStyle_ = FaceStyleValue.LoweredNotch Then
            g.DrawLine(pen, wrc.X + 2, wrc.Y + y, wrc.Right - 4, wrc.Y + y)
            g.DrawLine(controlLightLight, wrc.X + 2, wrc.Y + y + 1, wrc.Right - 4, wrc.Y + y + 1)
          Else
            g.DrawLine(controlLightLight, wrc.X + 2, wrc.Y + y, wrc.Right - 4, wrc.Y + y)
            g.DrawLine(pen, wrc.X + 2, wrc.Y + y + 1, wrc.Right - 4, wrc.Y + y + 1)
          End If
        Next y
      End If

      Dim brush2 As Brush
      If Pressed Then
        brush2 = New SolidBrush(activeLedColor_)
      Else
        brush2 = New SolidBrush(inactiveLedColor_)
      End If
      g.FillRectangle(brush2, wrc.X + 3, wrc.Y + 2, wrc.Width - 7, 4)
      brush2.Dispose()
    End Sub


    ' Properties
    <DefaultValue(GetType(Color), "LightGreen"), Category("Appearance"), Description("Active led color")> _
    Public Property ActiveLedColor() As Color
      Get
        Return activeLedColor_
      End Get
      Set(ByVal value As Color)
        If activeLedColor_ <> value Then activeLedColor_ = value : Invalidate()
      End Set
    End Property

    <DefaultValue(GetType(ButtonBehaviour), "CheckBox")> _
    Public Overrides Property Behaviour() As ButtonBehaviour
      Get
        Return MyBase.Behaviour
      End Get
      Set(ByVal value As ButtonBehaviour)
        MyBase.Behaviour = value
      End Set
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(32, 48)
      End Get
    End Property

    <Category("Appearance"), DefaultValue(GetType(FaceStyleValue), "RaisedNotch"), _
     Description("Control face appearance")> _
    Public Property FaceStyle() As FaceStyleValue
      Get
        Return faceStyle_
      End Get
      Set(ByVal value As FaceStyleValue)
        If faceStyle_ <> value Then faceStyle_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), Description("Inactive led color"), DefaultValue(GetType(Color), "Green")> _
    Public Property InactiveLedColor() As Color
      Get
        Return inactiveLedColor_
      End Get
      Set(ByVal value As Color)
        If inactiveLedColor_ <> value Then inactiveLedColor_ = value : Invalidate()
      End Set
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property Text() As String
      Get
        Return MyBase.Text
      End Get
      Set(ByVal value As String)
        MyBase.Text = value
      End Set
    End Property
  End Class

  Public Class ValueLabel : Inherits ValueFormatControl
    Private textAlign_ As ContentAlignment = ContentAlignment.TopLeft

    Public Sub New()
      MyBase.AutoSize = True
    End Sub

    <DefaultValue(True), EditorBrowsable(EditorBrowsableState.Always), Description("LabelAutoSizeDescr"), Category("Layout"), _
     RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(True)> _
    Public Overrides Property AutoSize() As Boolean
      Get
        Return MyBase.AutoSize
      End Get
      Set(ByVal value As Boolean)
        If AutoSize <> value Then
          MyBase.AutoSize = value
          AdjustSize()
        End If
      End Set
    End Property

    Private Function ShouldSerializeSize() As Boolean
      Return Not AutoSize
    End Function

    <Category("Appearance"), DefaultValue(GetType(ContentAlignment), "TopLeft"), Description("LabelTextAlignDescr")> _
    Public Overridable Property TextAlign() As ContentAlignment
      Get
        Return textAlign_
      End Get
      Set(ByVal value As ContentAlignment)
        If textAlign_ = value Then Exit Property
        textAlign_ = value : Invalidate()
      End Set
    End Property

    Protected Overrides Sub OnValueChanged(ByVal e As EventArgs)
      AdjustSize()
      MyBase.OnValueChanged(e)
    End Sub
    Protected Overrides Sub OnFormatChanged(ByVal e As EventArgs)
      AdjustSize()
      MyBase.OnFormatChanged(e)
    End Sub
    Public Overrides Function GetPreferredSize(ByVal proposedSize As Size) As Size
      Return GetPreferredTextSize(ScaleAndFormatValue(Value), proposedSize)
    End Function

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      DrawAlignedText(e.Graphics, ScaleAndFormatValue(Value), textAlign_)
    End Sub
  End Class

  Public MustInherit Class BoundedValueControl : Inherits ValueFormatControl
    Private maximum_ As Integer = 100, minimum_ As Integer, increment_ As Integer = 10
    Private dragging_ As Boolean

    Private Sub DecrementValue()
      Value -= increment_
    End Sub

    Private Sub IncrementValue()
      Value += increment_
    End Sub

    Protected Overridable Function PointToValue(ByVal p As Point) As Double
      Return 0
    End Function

    Protected Overridable ReadOnly Property Editable() As Boolean
      Get
        Return False
      End Get
    End Property

    Protected Overrides Function IsInputKey(ByVal keyData As Keys) As Boolean
      If Editable AndAlso Enabled Then
        Select Case keyData
          Case Keys.Up, Keys.Down, Keys.Left, Keys.Right
            Return True
        End Select
      End If
      Return False
    End Function

    Protected Overrides Sub OnEnter(ByVal e As EventArgs)
      Invalidate()
      MyBase.OnEnter(e)
    End Sub

    Protected Overrides Sub OnKeyDown(ByVal e As KeyEventArgs)
      MyBase.OnKeyDown(e)
      If Editable AndAlso Enabled Then
        Select Case e.KeyCode
          Case Keys.Left, Keys.Down
            DecrementValue()
            e.Handled = True
          Case Keys.Up, Keys.Right
            IncrementValue()
            e.Handled = True
        End Select
      End If
    End Sub

    Protected Overrides Sub OnLeave(ByVal e As EventArgs)
      Invalidate()
      MyBase.OnLeave(e)
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
      MyBase.OnMouseDown(e)
      If Not Editable OrElse Not Enabled OrElse e.Button <> Windows.Forms.MouseButtons.Left Then Exit Sub

      Capture = True
      dragging_ = True
      Value = PointToValue(New Point(e.X, e.Y))
      If TabStop Then Focus()
      Invalidate()
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
      MyBase.OnMouseMove(e)
      If Editable AndAlso dragging_ Then
        Value = PointToValue(New Point(e.X, e.Y))
      End If
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
      MyBase.OnMouseUp(e)
      If Editable AndAlso dragging_ AndAlso e.Button = Windows.Forms.MouseButtons.Left Then
        Capture = False
        dragging_ = False
        Invalidate()
      End If
    End Sub


    <Description("The increment interval for keyboard controlling"), DefaultValue(10), Category("Behavior")> _
    Public Overridable Property Increment() As Integer
      Get
        Return increment_
      End Get
      Set(ByVal value As Integer)
        If increment_ <> value Then increment_ = value : Invalidate()
      End Set
    End Property

    <Description("The upper bound of the value property"), Category("Behavior"), DefaultValue(100)> _
    Public Overridable Property Maximum() As Integer
      Get
        Return maximum_
      End Get
      Set(ByVal value As Integer)
        If maximum_ <> value Then maximum_ = value : Invalidate()
      End Set
    End Property

    <Description("The lower bound of the value property"), DefaultValue(0), Category("Behavior")> _
    Public Overridable Property Minimum() As Integer
      Get
        Return minimum_
      End Get
      Set(ByVal value As Integer)
        If minimum_ <> value Then minimum_ = value : Invalidate()
      End Set
    End Property

    Public Overrides Property Value() As Double
      Get
        Return MyBase.Value
      End Get
      Set(ByVal value As Double)
        If value > maximum_ Then value = maximum_
        If value < minimum_ Then value = minimum_
        MyBase.Value = value
      End Set
    End Property
  End Class

  <DefaultBindingProperty("Value"), DefaultProperty("Value")> _
  Public MustInherit Class ValueFormatControl : Inherits BorderTransparentControl
    <Category("Action"), Description("Occurs when value of the control changed")> _
    Public Event ValueChanged As EventHandler

    Private value_ As Double, format_ As String, numberScale_ As Integer = 1

    Protected Function ScaleAndFormatValue(ByVal val As Object) As String
      If val Is Nothing Then Return String.Empty
      If numberScale_ <> 0 AndAlso numberScale_ <> 1 Then
        val = CDbl(val) / numberScale_
      End If

      Dim str As String
#If 0 Then
' TODO: don't want this any more - need {0:f1}C for instance
      If Not String.IsNullOrEmpty(format_) Then str = String.Format(format_, val)
#Else
      If Not String.IsNullOrEmpty(format_) Then
        Dim formattable As IFormattable = TryCast(val, IFormattable)
        ' TODO: watch out, %'s need a \ in front of them
        If formattable IsNot Nothing Then str = formattable.ToString(format_, Nothing)
      End If
#End If
      If str Is Nothing Then str = val.ToString
      Return str
    End Function

    ' The format to be used - can include fixed text
    ' Use a 0 to put a number in
    ' Put literals inside single-quotes if necessary
    ' Commas and dots are also good for numbers
    <Category("Appearance")> _
    Public Property Format() As String
      Get
        Return format_
      End Get
      Set(ByVal value As String)
        If format_ = value Then Exit Property
        format_ = value
        OnFormatChanged(EventArgs.Empty)
      End Set
    End Property

    Protected Overridable Sub OnFormatChanged(ByVal e As EventArgs)
      Invalidate()
    End Sub


    Protected Overridable Sub OnValueChanged(ByVal e As EventArgs)
      Invalidate()
      RaiseEvent ValueChanged(Me, e)
    End Sub

    <Category("Appearance"), DefaultValue(1)> _
    Public Property NumberScale() As Integer
      Get
        Return numberScale_
      End Get
      Set(ByVal value As Integer)
        If numberScale_ <> value Then numberScale_ = value : Invalidate()
      End Set
    End Property

    <Category("Behavior"), DefaultValue(0.0), Bindable(True)> _
    Public Overridable Property Value() As Double
      Get
        Return value_
      End Get
      Set(ByVal value As Double)
        If value <> value_ Then
          value_ = value
          OnValueChanged(EventArgs.Empty)
        End If
      End Set
    End Property
  End Class

  ' ----------------------------------------------------
  ' Instrumentation Widgets are above - others follow

  Public Class KeyPad : Inherits BorderTransparentControl
    Private style_ As KeypadStyle
    Private buttonsAcross_, buttonsDown_ As Integer  ' the global layout of the buttons
    Private smallFont_ As Font
    Private touchButtons_ As New TouchButtonCollection
    Private pressedButton_ As TouchButton

    Public Sub New()
      Me.Style = KeypadStyle.Numeric ' the default
    End Sub

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(168, 168)
      End Get
    End Property


    Protected Overrides Sub OnResize(ByVal e As EventArgs)
      Dim rc As Rectangle = ClientRectangle : If rc.Width = 0 OrElse rc.Height = 0 Then Exit Sub
      ' Calculate the effective width and height
      Const interButtonSpacing As Integer = 4

      Dim ew As Integer = rc.Width() - (buttonsAcross_ + 1) * interButtonSpacing, _
          eh As Integer = rc.Height() - (buttonsDown_ + 1) * interButtonSpacing

      ' Set positions for each button
      For Each tb As TouchButton In touchButtons_
        Dim x0 As Integer = rc.Left + (tb.X * ew) \ buttonsAcross_ + (tb.X + 1) * interButtonSpacing, _
            y0 As Integer = rc.Top + (tb.Y * eh) \ buttonsDown_ + (tb.Y + 1) * interButtonSpacing, _
            w0 As Integer = rc.Left + ((tb.X + 1) * ew) \ buttonsAcross_ + (tb.X + 1) * interButtonSpacing - x0, _
            h0 As Integer = rc.Top + ((tb.Y + 1) * eh) \ buttonsDown_ + (tb.Y + 1) * interButtonSpacing - y0
        tb.SetBounds(New Rectangle(x0, y0, w0, h0))
      Next tb

      ' And make suitable fonts
      Dim mainFontHeight As Integer = (eh * 4) \ (buttonsDown_ * 5) - 7
      If mainFontHeight < 6 Then Exit Sub ' save errors
      Font = New Font("Tahoma", mainFontHeight, GraphicsUnit.Pixel)
      smallFont_ = New Font("Tahoma", (mainFontHeight * 2) \ 3, GraphicsUnit.Pixel)
    End Sub


    <Category("Appearance")> _
    Public Property Style() As KeypadStyle
      Get
        Return style_
      End Get
      Set(ByVal value As KeypadStyle)
        style_ = value

        ' Make the table of touch-buttons
        touchButtons_.Clear()

        Dim keys0() As Keys = Nothing ' set this based on the select
        Select Case value
          Case KeypadStyle.Numeric
            buttonsAcross_ = 3 : buttonsDown_ = 4
            keys0 = New Keys() {Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, _
                                Keys.Delete, Keys.D0, Keys.Back}

          Case KeypadStyle.NumericWithOk
            buttonsAcross_ = 3 : buttonsDown_ = 5
            keys0 = New Keys() {Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, _
                                            Keys.Delete, Keys.D0, Keys.Back, Keys.Escape, Keys.Return}

          Case KeypadStyle.Alpha
            buttonsAcross_ = 8 : buttonsDown_ = 5
            keys0 = New Keys() {Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, _
                                Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, _
                                Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z, Keys.Space, Keys.OemQuestion, _
                                Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, _
                                Keys.Delete, Keys.Back}

          Case KeypadStyle.Qwerty
            buttonsAcross_ = 10 : buttonsDown_ = 4
            keys0 = New Keys() {Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0, _
                                Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U, Keys.I, Keys.O, Keys.P, _
                                Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.L, Keys.Space, _
                                Keys.Back, Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.N, Keys.M, Keys.Escape, Keys.Return}

          Case KeypadStyle.AlphaWithOk
            buttonsAcross_ = 8 : buttonsDown_ = 6
            keys0 = New Keys() {Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, _
                                Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, _
                                Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z, Keys.Space, Keys.OemQuestion, _
                                Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, _
                                Keys.Delete, Keys.Back, Keys.Escape, Keys.Return}
        End Select
        If keys0 IsNot Nothing Then
          For i As Integer = 0 To keys0.Length - 1
            touchButtons_.Add(New TouchButton(keys0(i), i Mod buttonsAcross_, i \ buttonsAcross_))
          Next i
        End If

        pressedButton_ = Nothing  ' nothing pressed
        If IsHandleCreated Then OnResize(Nothing) : Invalidate()
      End Set
    End Property


    ' ---------------------------------------------------------------------
    Private Class TouchButton
      Private ReadOnly key_ As Keys, text_ As String, x_, y_ As Integer
      Private rc_ As Rectangle   ' position within window

      Public Sub New(ByVal key As Keys, ByVal x As Integer, ByVal y As Integer)
        key_ = key : x_ = x : y_ = y
        Select Case key
          Case Keys.D0 To Keys.D9 : text_ = Convert.ToChar(key)
          Case Keys.Escape : text_ = "Esc"
          Case Keys.Delete : text_ = "Clr"
          Case Keys.Return : text_ = "OK"
          Case Keys.Back : text_ = "Bsp"
          Case Keys.Space : text_ = "sp"
          Case Keys.OemQuestion : text_ = "?"
          Case Else
            text_ = key.ToString
        End Select
      End Sub

      Public Sub SetBounds(ByVal value As Rectangle)
        rc_ = value
      End Sub
      Public ReadOnly Property Bounds() As Rectangle
        Get
          Return rc_
        End Get
      End Property

      Public ReadOnly Property Key() As Keys
        Get
          Return key_
        End Get
      End Property
      Public ReadOnly Property Text() As String
        Get
          Return text_
        End Get
      End Property
      Public ReadOnly Property X() As Integer
        Get
          Return x_
        End Get
      End Property
      Public ReadOnly Property Y() As Integer
        Get
          Return y_
        End Get
      End Property
    End Class

    Private Class TouchButtonCollection : Inherits Generic.List(Of TouchButton)
    End Class

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      Dim rc As Rectangle = DisplayRectangle, graphics As Graphics = e.Graphics

      ' Draw the buttons
      For Each tb As TouchButton In touchButtons_
        If tb.Bounds.Width > 4 AndAlso tb.Bounds.Height > 4 Then
          Dim isPressed As Boolean = (tb Is pressedButton_)
          Dim state As ButtonState = ButtonState.Normal
          If isPressed Then state = state Or ButtonState.Pushed
          ControlPaint.DrawButton(graphics, tb.Bounds, state)
          ' Use a suitable font
          Dim f As Font = Nothing : If tb.Text.Length > 1 Then f = smallFont_
          If f Is Nothing Then f = Font
          ' Draw the character in the middle
          Dim trc As Rectangle = tb.Bounds : If isPressed Then trc.X += 1 : trc.Y += 1
          TextRenderer.DrawText(graphics, tb.Text, f, trc, Color.Black, TextFormatFlags.HidePrefix Or TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        End If
      Next tb
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
      Dim pt As New Point(e.X, e.Y)
      For Each tb As TouchButton In touchButtons_
        If tb.Bounds.Contains(pt) Then
          ' Push the key in
          pressedButton_ = tb : Invalidate(tb.Bounds) : Update()

          ' We take the key-stroke right now because that makes sense for a touch screen.
          Dim keys As String = Nothing
          Select Case tb.Key
            Case System.Windows.Forms.Keys.Delete
              ' With luck, clears the field
              Dim ctl As Control = Control.FromHandle(GetFocus()) : If ctl IsNot Nothing Then ctl.Text = ""
            Case System.Windows.Forms.Keys.Back : keys = "{BS}"
            Case System.Windows.Forms.Keys.Space : keys = " "
            Case System.Windows.Forms.Keys.Escape : keys = "{ESC}"
            Case System.Windows.Forms.Keys.Return : keys = "{ENTER}"
            Case Else : keys = tb.Text
          End Select
          If keys IsNot Nothing Then SendKeys.Send(keys)
          Exit For
        End If
      Next tb
    End Sub
    Private Declare Function GetFocus Lib "user32" () As IntPtr

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
      If pressedButton_ Is Nothing Then Exit Sub
      With pressedButton_
        pressedButton_ = Nothing
        Invalidate(.Bounds) : Update()
      End With
    End Sub

    Protected Overrides Sub OnHandleCreated(ByVal e As EventArgs)
      OnResize(Nothing)
    End Sub
  End Class

  Public Enum KeypadStyle
    Numeric
    Alpha
    Qwerty
    NumericWithOk
    AlphaWithOk
  End Enum


  ' -----------------------------------------------------
  <ToolboxBitmap(GetType(Lamp)), DefaultBindingPropertyAttribute("Value")> _
  Public Class Lamp : Inherits BorderTransparentControl
    Private value_ As Boolean, onColor_ As Color = Color.Lime, offColor_ As Color = Color.DarkGray

    <Category("Appearance"), DefaultValue(GetType(Color), "Lime")> _
    Public Property OnColor() As Color
      Get
        Return onColor_
      End Get
      Set(ByVal value As Color)
        If onColor_ = value Then Exit Property
        onColor_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "DarkGray")> _
    Public Property OffColor() As Color
      Get
        Return offColor_
      End Get
      Set(ByVal value As Color)
        If offColor_ = value Then Exit Property
        offColor_ = value : Invalidate()
      End Set
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(20, 20)
      End Get
    End Property

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      Dim graphics As Graphics = e.Graphics, rc As Rectangle = DisplayRectangle

      ' Get the colors to fill with
      Dim innerColor As Color
      If value_ Then
        innerColor = onColor_
      Else
        innerColor = offColor_
      End If
      Dim outerColor As Color = Color.FromArgb(innerColor.R * 192 \ 255, innerColor.G * 192 \ 255, innerColor.B * 192 \ 255)

      rc.Width -= 1 : rc.Height -= 1
      Using br As New SolidBrush(outerColor)
        graphics.FillEllipse(br, rc)
      End Using
      graphics.DrawEllipse(Pens.Black, rc)
      rc.Inflate(-3, -3)
      Using br As New SolidBrush(innerColor)
        graphics.FillEllipse(br, rc)
      End Using
      graphics.DrawEllipse(Pens.Black, rc)
    End Sub

    <Category("Data"), DefaultValue(False), Bindable(True)> _
    Public Property Value() As Boolean
      Get
        Return value_
      End Get
      Set(ByVal value As Boolean)
        If value_ = value Then Exit Property
        value_ = value
        Invalidate()
      End Set
    End Property
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property Font() As Font
      Get
        Return MyBase.Font
      End Get
      Set(ByVal value As Font)
        MyBase.Font = value
      End Set
    End Property
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property ForeColor() As Color
      Get
        Return MyBase.ForeColor
      End Get
      Set(ByVal value As Color)
        MyBase.ForeColor = value
      End Set
    End Property
  End Class


  ' --------------------------------------------------------------
  Public Class ProportionalValve : Inherits ValueFormatControl
    Private orientation_ As Orientation

    Public Sub New()
      BackColor = Color.Black
      ForeColor = Color.Yellow
    End Sub

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      Dim graphics As Graphics = e.Graphics, rc As Rectangle = DisplayRectangle

      'Make the angle for the position 0 - 90 degrees
      Dim valveAngle As Single
      If Value > 0 Then
        If Value > 1000 Then
          valveAngle = 90
        Else
          valveAngle = CSng(Math.Abs((89.5 * Value) / 1000))
        End If
      End If
      ' Draw the positioner
      '      brushes.Yellow 
      Using fillBrush As New SolidBrush(ForeColor)
        If orientation_ = Windows.Forms.Orientation.Horizontal Then
          '       UserControl.Height = 375 : UserControl.Width= 255
          rc.Inflate(-10, -2)
          graphics.FillPie(fillBrush, rc.Left - rc.Width, rc.Top, 2 * rc.Width, 2 * rc.Height, 0, -valveAngle)
        Else
          '        UserControl.Height = 255 : UserControl.Width = 375
          rc.Inflate(-2, -10)
          graphics.FillPie(fillBrush, rc.Left - rc.Width, rc.Top - rc.Height, 2 * rc.Width, 2 * rc.Height, 90, -valveAngle)
        End If
      End Using
    End Sub

    <DefaultValue(GetType(Color), "Black")> _
    Public Overrides Property BackColor() As Color
      Get
        Return MyBase.BackColor
      End Get
      Set(ByVal value As Color)
        MyBase.BackColor = value
      End Set
    End Property

    <DefaultValue(GetType(Color), "Yellow")> _
    Public Overrides Property ForeColor() As Color
      Get
        Return MyBase.ForeColor
      End Get
      Set(ByVal value As Color)
        MyBase.ForeColor = value
      End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Orientation), "Horizontal")> _
    Public Property Orientation() As Orientation
      Get
        Return orientation_
      End Get
      Set(ByVal value As Orientation)
        If orientation_ = value Then Exit Property
        orientation_ = value
        Invalidate()
      End Set
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property Font() As Font
      Get
        Return MyBase.Font
      End Get
      Set(ByVal value As Font)
        MyBase.Font = value
      End Set
    End Property
  End Class


  ' ---------------------------------------------------------------------
  <DefaultBindingPropertyAttribute("Value")> _
  Public Class Valve : Inherits BorderTransparentControl
    Private value_ As Boolean, orientation_ As Orientation, _
            valveColor_ As Color = Color.Black, onColor_ As Color = Color.Lime, offColor_ As Color = Color.DarkGray

    Public Sub New()

    End Sub

    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
      MyBase.OnResize(e)
      Stop
    End Sub
    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(20, 20)
      End Get
    End Property

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      Dim graphics As Graphics = e.Graphics, rc As Rectangle = DisplayRectangle
      Using valveBrush As New SolidBrush(valveColor_)
        ' We draw in a slightly strange rectangle to get the best effect at small sizes
        graphics.FillEllipse(valveBrush, rc.Left - 1, rc.Top - 1, rc.Width - 1 + 2, rc.Height - 1 + 2)
      End Using
      Dim rc2 As Rectangle
      If (orientation_ = Windows.Forms.Orientation.Horizontal) Xor value_ Then
        Dim x As Integer = rc.Width \ 4 + 1
        rc2 = New Rectangle(rc.Left + x, rc.Top, rc.Width - 2 * x - 1, rc.Height - 1)
      Else
        Dim x As Integer = rc.Height \ 4 + 1
        rc2 = New Rectangle(rc.Left, rc.Top + x, rc.Width - 1, rc.Height - 2 * x - 1)
      End If
      Using valvePen As New Pen(valveColor_)
        graphics.DrawRectangle(valvePen, rc2)
      End Using
      rc2.X += 1 : rc2.Y += 1 : rc2.Width -= 1 : rc2.Height -= 1
      Dim fillColor As Color
      If value_ Then
        fillColor = onColor_
      Else
        fillColor = offColor_
      End If
      Using fillBrush As New SolidBrush(fillColor)
        graphics.FillRectangle(fillBrush, rc2)
      End Using
    End Sub

    <Category("Appearance"), DefaultValue(GetType(Color), "Black")> _
    Public Property ValveColor() As Color
      Get
        Return valveColor_
      End Get
      Set(ByVal value As Color)
        If valveColor_ = value Then Exit Property
        valveColor_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "Lime")> _
    Public Property OnColor() As Color
      Get
        Return onColor_
      End Get
      Set(ByVal value As Color)
        If onColor_ = value Then Exit Property
        onColor_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Color), "DarkGray")> _
    Public Property OffColor() As Color
      Get
        Return offColor_
      End Get
      Set(ByVal value As Color)
        If offColor_ = value Then Exit Property
        offColor_ = value : Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Orientation), "Horizontal")> _
    Public Property Orientation() As Orientation
      Get
        Return orientation_
      End Get
      Set(ByVal value As Orientation)
        If orientation_ = value Then Exit Property
        orientation_ = value
        Invalidate()
      End Set
    End Property

    <Category("Data"), DefaultValue(False), Bindable(True)> _
    Public Property Value() As Boolean
      Get
        Return value_
      End Get
      Set(ByVal value As Boolean)
        If value_ = value Then Exit Property
        value_ = value
        Invalidate()
      End Set
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property BackColor() As Color
      Get
        Return MyBase.BackColor
      End Get
      Set(ByVal value As Color)
        MyBase.BackColor = value
      End Set
    End Property
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property BackgroundImage() As Image
      Get
        Return MyBase.BackgroundImage
      End Get
      Set(ByVal value As Image)
        MyBase.BackgroundImage = value
      End Set
    End Property
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property BackgroundImageLayout() As ImageLayout
      Get
        Return MyBase.BackgroundImageLayout
      End Get
      Set(ByVal value As ImageLayout)
        MyBase.BackgroundImageLayout = value
      End Set
    End Property
    ' These don't do anything
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property ForeColor() As Color
      Get
        Return MyBase.ForeColor
      End Get
      Set(ByVal value As Color)
        MyBase.ForeColor = value
      End Set
    End Property
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property Font() As Font
      Get
        Return MyBase.Font
      End Get
      Set(ByVal value As Font)
        MyBase.Font = value
      End Set
    End Property
    <Bindable(False), Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
      Public Overrides Property Text() As String
      Get
        Return MyBase.Text
      End Get
      Set(ByVal value As String)
      End Set
    End Property
  End Class


  ' -----------------------------------------------------------------------
  <Description("A nice looking heat exchanger")> _
  Public Class HeatExchanger : Inherits ValueFormatControl
    Private temperatureChange_ As TemperatureChange, orientation_ As Orientation

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(160, 30)
      End Get
    End Property

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      Dim graphics As Graphics = e.Graphics, rc As Rectangle = DisplayRectangle
      ' Lines
      Const margin As Integer = 12, extraLines As Integer = 3
      Dim backBrush As Brush
      Select Case temperatureChange_
        Case TemperatureChange.Heating : backBrush = Brushes.Maroon
        Case TemperatureChange.Cooling : backBrush = Brushes.MediumBlue
        Case Else : backBrush = Brushes.Gray
      End Select

      If orientation_ = Windows.Forms.Orientation.Horizontal Then
        rc.Inflate(-margin, 0)
        graphics.DrawLine(Pens.Black, rc.Left, rc.Top, rc.Left, rc.Bottom - 1)
        graphics.DrawLine(Pens.Black, rc.Left + 1, rc.Top, rc.Left + 1, rc.Bottom - 1)
        graphics.DrawLine(Pens.Black, rc.Right - 1, rc.Top, rc.Right - 1, rc.Bottom - 1)
        graphics.DrawLine(Pens.Black, rc.Right - 2, rc.Top, rc.Right - 2, rc.Bottom - 1)
        rc.Inflate(-2, 0) : graphics.FillRectangle(backBrush, rc)
        ' Extra lines 
        For i As Integer = 0 To extraLines - 1
          Dim y As Integer = rc.Top + (rc.Height * (i + 1)) \ (extraLines + 1)
          graphics.DrawLine(Pens.Black, rc.Left, y, rc.Right, y)
        Next i
      Else
        rc.Inflate(0, -margin)
        graphics.DrawLine(Pens.Black, rc.Left, rc.Top, rc.Right - 1, rc.Top)
        graphics.DrawLine(Pens.Black, rc.Left, rc.Top + 1, rc.Right - 1, rc.Top + 1)
        graphics.DrawLine(Pens.Black, rc.Left, rc.Bottom - 1, rc.Right - 1, rc.Bottom - 1)
        graphics.DrawLine(Pens.Black, rc.Left, rc.Bottom - 2, rc.Right - 1, rc.Bottom - 2)
        rc.Inflate(0, -2) : graphics.FillRectangle(backBrush, rc)
        For i As Integer = 0 To extraLines - 1
          Dim x As Integer = rc.Left + (rc.Width * (i + 1)) \ (extraLines + 1)
          graphics.DrawLine(Pens.Black, x, rc.Top, x, rc.Bottom)
        Next i
      End If

      ' Draw the label area
      Dim siz As Size = TextRenderer.MeasureText("100.0%", Font)
      Dim lblWidth As Integer = siz.Width + 8, lblHeight As Integer = siz.Height
      Dim rcLabel As New Rectangle((rc.Left + rc.Right - lblWidth) \ 2, (rc.Top + rc.Bottom - lblHeight) \ 2, lblWidth, lblHeight)
      graphics.FillRectangle(Brushes.White, rcLabel)

      TextRenderer.DrawText(graphics, ScaleAndFormatValue(Value), Font, rcLabel, ForeColor, TextFormatFlags.HorizontalCenter Or TextFormatFlags.NoPrefix)
    End Sub


    <Category("Appearance"), DefaultValue(GetType(TemperatureChange), "None")> _
    Public Property TemperatureChange() As TemperatureChange
      Get
        Return temperatureChange_
      End Get
      Set(ByVal value As TemperatureChange)
        If temperatureChange_ = value Then Exit Property
        temperatureChange_ = value
        Invalidate()
      End Set
    End Property

    <Category("Appearance"), DefaultValue(GetType(Orientation), "Horizontal")> _
    Public Property Orientation() As Orientation
      Get
        Return orientation_
      End Get
      Set(ByVal value As Orientation)
        If orientation_ = value Then Exit Property
        orientation_ = value
        Invalidate()
      End Set
    End Property
  End Class

  Public Enum TemperatureChange
    None
    Heating
    Cooling
  End Enum

  ' -------------------------------------------------------
  <DefaultBindingPropertyAttribute("Value")> _
  Public Class Input : Inherits BorderTransparentControl
    Private uiEnabled_ As Boolean = True, value_ As Boolean

    Public Sub New()
      inNew_ += 1
      MyBase.ForeColor = Color.LimeGreen
      inNew_ -= 1
    End Sub

    <DefaultValue(True)> _
    Public Property UIEnabled() As Boolean
      Get
        Return uiEnabled_
      End Get
      Set(ByVal value As Boolean)
        uiEnabled_ = value
      End Set
    End Property

    <DefaultValue(GetType(Color), "LimeGreen")> _
    Public Overrides Property ForeColor() As Color
      Get
        Return MyBase.ForeColor
      End Get
      Set(ByVal value As Color)
        If MyBase.ForeColor = value Then Exit Property
        MyBase.ForeColor = value
        Invalidate()
      End Set
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(16, 16)
      End Get
    End Property

    <Category("Data"), DefaultValue(False), Bindable(True)> _
    Public Property Value() As Boolean
      Get
        Return value_
      End Get
      Set(ByVal value As Boolean)
        If value_ = value Then Exit Property
        value_ = value
        Invalidate()
      End Set
    End Property

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
      ' Flip the value using the mouse
      If uiEnabled_ Then Value = Not Value
    End Sub

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      Dim graphics As Graphics = e.Graphics, rc As Rectangle = DisplayRectangle
      If value_ Then
        Using br As New SolidBrush(ForeColor)
          graphics.FillEllipse(br, rc)
        End Using
      Else
        graphics.FillEllipse(Brushes.DarkGray, rc)
      End If
      Dim lightColor, darkColor As Color
      If value_ Then
        lightColor = Color.FromArgb(96, 255, 96)
        darkColor = Color.FromArgb(0, 96, 0)
      Else
        lightColor = Color.FromArgb(223, 223, 223)
        darkColor = Color.FromArgb(96, 96, 96)
      End If

      Using lightPen As New Pen(lightColor), darkPen As New Pen(darkColor)
        rc.Inflate(-1, -1)
        For i As Integer = 0 To 2 - 1
          graphics.DrawArc(lightPen, rc, 135, 180)
          graphics.DrawArc(darkPen, rc, 315, 180)
          rc.Inflate(-2, -2)
        Next i
      End Using
    End Sub
  End Class


  ' -------------------------------------------------------
  <DefaultBindingPropertyAttribute("Value")> _
  Public Class Output : Inherits BorderTransparentControl
    Private uiEnabled_ As Boolean = True, value_ As Boolean

    Public Sub New()
      inNew_ += 1
      MyBase.ForeColor = Color.Red
      inNew_ -= 1
    End Sub

    <DefaultValue(True)> _
    Public Property UIEnabled() As Boolean
      Get
        Return uiEnabled_
      End Get
      Set(ByVal value As Boolean)
        uiEnabled_ = value
      End Set
    End Property


    <DefaultValue(GetType(Color), "Red")> _
    Public Overrides Property ForeColor() As Color
      Get
        Return MyBase.ForeColor
      End Get
      Set(ByVal value As Color)
        If MyBase.ForeColor = value Then Exit Property
        MyBase.ForeColor = value
        Invalidate()
      End Set
    End Property

    Protected Overrides ReadOnly Property DefaultSize() As Size
      Get
        Return New Size(16, 16)
      End Get
    End Property

    <Category("Data"), DefaultValue(False), Bindable(True)> _
    Public Property Value() As Boolean
      Get
        Return value_
      End Get
      Set(ByVal value As Boolean)
        If value_ = value Then Exit Property
        value_ = value
        Invalidate()
      End Set
    End Property

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
      ' Flip the value using the mouse
      If uiEnabled_ Then Value = Not Value
    End Sub

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      Dim graphics As Graphics = e.Graphics, rc As Rectangle = DisplayRectangle

      If value_ Then
        Using br As New SolidBrush(ForeColor)
          graphics.FillRectangle(br, rc)
        End Using
      Else
        graphics.FillRectangle(SystemBrushes.ButtonFace, rc)
      End If

      Dim lightColor, darkColor As Color
      If value_ Then
        lightColor = Color.FromArgb(255, 128, 128)
        darkColor = Color.Maroon
      Else
        lightColor = Color.FromArgb(223, 223, 223)
        darkColor = Color.FromArgb(96, 96, 96)
      End If
      Using lightPen As New Pen(lightColor), darkPen As New Pen(darkColor)
        For i As Integer = 0 To 2 - 1
          graphics.DrawLines(lightPen, New Point() {New Point(rc.Right - 1, rc.Top), New Point(rc.Left, rc.Top), New Point(rc.Left, rc.Bottom)})
          graphics.DrawLines(darkPen, New Point() {New Point(rc.Right - 1, rc.Top + 1), New Point(rc.Right - 1, rc.Bottom - 1), New Point(rc.Left, rc.Bottom - 1)})
          If i = 0 Then rc.Inflate(-2, -2)
        Next i
      End Using
    End Sub
  End Class

  <ToolboxBitmap(GetType(Label)), _
   DefaultBindingPropertyAttribute("Text"), DefaultProperty("Text"), _
   Designer(GetType(Design.FormatLabelDesigner))> _
  Public Class Label : Inherits BorderTransparentControl
    Private textAlign_ As ContentAlignment = ContentAlignment.TopLeft

    Public Sub New()
      MyBase.AutoSize = True
    End Sub

    <DefaultValue(True), EditorBrowsable(EditorBrowsableState.Always), Description("LabelAutoSizeDescr"), Category("Layout"), _
     RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(True)> _
    Public Overrides Property AutoSize() As Boolean
      Get
        Return MyBase.AutoSize
      End Get
      Set(ByVal value As Boolean)
        If AutoSize <> value Then
          MyBase.AutoSize = value
          AdjustSize()
        End If
      End Set
    End Property

    Private Function ShouldSerializeSize() As Boolean
      Return Not AutoSize
    End Function

    <Category("Appearance"), DefaultValue(GetType(ContentAlignment), "TopLeft"), Description("LabelTextAlignDescr")> _
    Public Overridable Property TextAlign() As ContentAlignment
      Get
        Return textAlign_
      End Get
      Set(ByVal value As ContentAlignment)
        If textAlign_ = value Then Exit Property
        textAlign_ = value : Invalidate()
      End Set
    End Property

    ' Want this back
    <Editor(GetType(MultilineStringEditor), GetType(UITypeEditor)), _
      Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
      Public Overrides Property Text() As String
      Get
        Return MyBase.Text
      End Get
      Set(ByVal value As String)
        MyBase.Text = value
      End Set
    End Property

    Protected Overrides Sub OnTextChanged(ByVal e As System.EventArgs)
      MyBase.OnTextChanged(e)
      AdjustSize()
    End Sub

    Protected Overrides Sub OnDraw(ByVal e As PaintEventArgs)
      MyBase.OnDraw(e)
      DrawAlignedText(e.Graphics, Text, textAlign_)
    End Sub

    Public Overrides Function GetPreferredSize(ByVal proposedSize As Size) As Size
      Return GetPreferredTextSize(Text, proposedSize)
    End Function
  End Class

  Public NotInheritable Class CloneControl
    Private Sub New()
    End Sub
    Public Shared Function Clone(ByVal value As Control, ByVal z As Integer) As Control
      ' If z = 1, just return the given Control.
      ' If z > 1, make a clone of it and return that
      If z = 1 Then Return value

      Dim ret As Control = DirectCast(value.GetType.GetConstructor(Type.EmptyTypes).Invoke(Nothing), Control)
      ret.SuspendLayout()

      For Each source As Control In value.Controls
        Dim dest As Control = DirectCast(source.GetType.GetConstructor(Type.EmptyTypes).Invoke(Nothing), Control)
        CopyMostProperties(source, dest)

        ' Take special action for Name, Text, Font, Bounds and Parent
        Dim nam As String = source.Name
        If nam.EndsWith("1") Then
          dest.Name = nam.Substring(0, nam.Length - 1) & z.ToString(InvariantCulture)
        Else
          dest.Name = nam
        End If
        Dim txt2 As String = source.Text
        If txt2.EndsWith("1") Then dest.Text = txt2.Substring(0, txt2.Length - 1) & z.ToString(InvariantCulture)
        dest.Font = source.Font
        dest.Bounds = source.Bounds
        dest.Parent = ret
      Next source
      ret.ResumeLayout(False)


      CopyMostProperties(value, ret)
      ret.BackColor = value.BackColor
      ret.Font = value.Font

      Dim txt As String = value.Text
      If txt.EndsWith("1") Then ret.Text = txt.Substring(0, txt.Length - 1) & z.ToString(InvariantCulture)

      ret.Parent = value.Parent
      '      ret.SetBounds(value.Left + value.Width * (z - 1), value.Top, value.Width, value.Height)
      Return ret
    End Function

    Private Shared Sub CopyMostProperties(ByVal source As Control, ByVal dest As Control)
      ' Set the values of all suitable properties
      If Not TypeOf dest Is TextBox Then
        For Each pi As Reflection.PropertyInfo In dest.GetType.GetProperties(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public)
          If pi.DeclaringType IsNot GetType(Control) AndAlso pi.CanRead AndAlso pi.CanWrite Then
            ' Only if Browsable - so we ignore quite a few
            Dim attrs() As Object = pi.GetCustomAttributes(GetType(BrowsableAttribute), True)
            If attrs.Length = 0 OrElse DirectCast(attrs(0), BrowsableAttribute).Browsable Then
              ' Copy across the property value
              pi.SetValue(dest, pi.GetValue(source, Nothing), Nothing)
              If dest.IsHandleCreated Then Stop
            End If
          End If
        Next pi
      End If

    End Sub
  End Class
End Namespace

Namespace MimicControls.Design
  ' A FormatLabel should be designed taking account of its AutoResize property
  Friend Class FormatLabelDesigner : Inherits System.Windows.Forms.Design.ControlDesigner
    Public Sub New()
      AutoResizeHandles = True
    End Sub
  End Class
End Namespace

Public Module MimicControlsModule
#If 0 Then
  Public Function IsControlObscured(ByVal control As Control) As Boolean
    Return FormsX.IsControlObscured(control)
  End Function
#End If
End Module