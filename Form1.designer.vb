<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserLogin
  Inherits System.Windows.Forms.Form

  'Form 覆寫 Dispose 以清除元件清單。
  <System.Diagnostics.DebuggerNonUserCode()> _
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    Try
      If disposing AndAlso components IsNot Nothing Then
        components.Dispose()
      End If
    Finally
      MyBase.Dispose(disposing)
    End Try
  End Sub

  '為 Windows Form 設計工具的必要項
  Private components As System.ComponentModel.IContainer

  '注意: 以下為 Windows Form 設計工具所需的程序
  '可以使用 Windows Form 設計工具進行修改。
  '請勿使用程式碼編輯器進行修改。
  <System.Diagnostics.DebuggerStepThrough()> _
  Private Sub InitializeComponent()
    Me.OK = New System.Windows.Forms.Button()
    Me.Label1 = New HJDOS109.MimicControls.Label()
    Me.ComboBox1 = New System.Windows.Forms.ComboBox()
    Me.SuspendLayout()
    '
    'OK
    '
    Me.OK.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.OK.Location = New System.Drawing.Point(269, 89)
    Me.OK.Name = "OK"
    Me.OK.Size = New System.Drawing.Size(75, 31)
    Me.OK.TabIndex = 0
    Me.OK.Text = "OK"
    Me.OK.UseVisualStyleBackColor = True
    '
    'Label1
    '
    Me.Label1.Font = New System.Drawing.Font("新細明體", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
    Me.Label1.ForeColor = System.Drawing.Color.Black
    Me.Label1.Location = New System.Drawing.Point(12, 12)
    Me.Label1.Name = "Label1"
    Me.Label1.Size = New System.Drawing.Size(311, 21)
    Me.Label1.TabIndex = 1
    Me.Label1.Text = "請選擇操作員來登入，然後按OK"
    '
    'ComboBox1
    '
    Me.ComboBox1.Font = New System.Drawing.Font("新細明體", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
    Me.ComboBox1.FormattingEnabled = True
    Me.ComboBox1.Items.AddRange(New Object() {"A", "B", "C", "D"})
    Me.ComboBox1.Location = New System.Drawing.Point(12, 45)
    Me.ComboBox1.Name = "ComboBox1"
    Me.ComboBox1.Size = New System.Drawing.Size(332, 29)
    Me.ComboBox1.TabIndex = 4
    '
    'UserLogin
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.BackColor = System.Drawing.Color.White
    Me.CancelButton = Me.OK
    Me.ClientSize = New System.Drawing.Size(373, 134)
    Me.Controls.Add(Me.ComboBox1)
    Me.Controls.Add(Me.Label1)
    Me.Controls.Add(Me.OK)
    Me.MaximizeBox = False
    Me.MinimizeBox = False
    Me.Name = "UserLogin"
    Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
    Me.Text = "Login"
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub

  Friend WithEvents OK As Button
  Friend WithEvents Label1 As MimicControls.Label
  Friend WithEvents ComboBox1 As ComboBox
End Class
