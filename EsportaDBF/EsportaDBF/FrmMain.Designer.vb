<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmMain
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla mediante l'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMain))
        Me.ReflectionLabel1 = New DevComponents.DotNetBar.Controls.ReflectionLabel()
        Me.btnRicaricaClientiFornitori = New DevComponents.DotNetBar.ButtonX()
        Me.grpTrasmettitore = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.lvwTrasmettitore = New System.Windows.Forms.ListView()
        Me.ColDettaglio = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.txtArticoli = New System.Windows.Forms.TextBox()
        Me.txtClientiFornitori = New System.Windows.Forms.TextBox()
        Me.btnSalvaImpostazioni = New DevComponents.DotNetBar.ButtonX()
        Me.txtTempo = New System.Windows.Forms.TextBox()
        Me.lblTmpAggiornamento = New System.Windows.Forms.Label()
        Me.lblPathArticoli = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblMinuti = New System.Windows.Forms.Label()
        Me.TimerRefreshImportazione = New System.Windows.Forms.Timer(Me.components)
        Me.btnRicaricaArticoli = New DevComponents.DotNetBar.ButtonX()
        Me.TimerControllerLog = New System.Windows.Forms.Timer(Me.components)
        Me.txtFattureGen = New System.Windows.Forms.TextBox()
        Me.BtnRicaricaFatture = New DevComponents.DotNetBar.ButtonX()
        Me.txtFattureRig = New System.Windows.Forms.TextBox()
        Me.txtDestinazioniClienti = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.btnSvuotaTrasmettitore = New DevComponents.DotNetBar.ButtonX()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.lblTempoEliminazione = New System.Windows.Forms.Label()
        Me.txtTempoEliminazioneFile = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblTempAggiornamentoFatt = New System.Windows.Forms.Label()
        Me.txtTempoAggiornamentoFatt = New System.Windows.Forms.TextBox()
        Me.TimerRefreshFatture = New System.Windows.Forms.Timer(Me.components)
        Me.grpTrasmettitore.SuspendLayout()
        Me.SuspendLayout()
        '
        'ReflectionLabel1
        '
        '
        '
        '
        Me.ReflectionLabel1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.ReflectionLabel1.Location = New System.Drawing.Point(14, 2)
        Me.ReflectionLabel1.Name = "ReflectionLabel1"
        Me.ReflectionLabel1.Size = New System.Drawing.Size(290, 42)
        Me.ReflectionLabel1.TabIndex = 3
        Me.ReflectionLabel1.Text = "<b><font size=""+6""><i>Connettore</i><font color=""#B02B2C""><i> MagTrace</i></font>" &
    "</font></b>"
        '
        'btnRicaricaClientiFornitori
        '
        Me.btnRicaricaClientiFornitori.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnRicaricaClientiFornitori.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRicaricaClientiFornitori.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.btnRicaricaClientiFornitori.Location = New System.Drawing.Point(672, 150)
        Me.btnRicaricaClientiFornitori.Name = "btnRicaricaClientiFornitori"
        Me.btnRicaricaClientiFornitori.Size = New System.Drawing.Size(183, 23)
        Me.btnRicaricaClientiFornitori.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnRicaricaClientiFornitori.TabIndex = 4
        Me.btnRicaricaClientiFornitori.Text = "Forza ricaricamento clienti / fornitori"
        '
        'grpTrasmettitore
        '
        Me.grpTrasmettitore.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpTrasmettitore.CanvasColor = System.Drawing.SystemColors.Control
        Me.grpTrasmettitore.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.grpTrasmettitore.Controls.Add(Me.lvwTrasmettitore)
        Me.grpTrasmettitore.DisabledBackColor = System.Drawing.Color.Empty
        Me.grpTrasmettitore.Location = New System.Drawing.Point(7, 276)
        Me.grpTrasmettitore.Name = "grpTrasmettitore"
        Me.grpTrasmettitore.Size = New System.Drawing.Size(849, 154)
        '
        '
        '
        Me.grpTrasmettitore.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2
        Me.grpTrasmettitore.Style.BackColorGradientAngle = 90
        Me.grpTrasmettitore.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground
        Me.grpTrasmettitore.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.grpTrasmettitore.Style.BorderBottomWidth = 1
        Me.grpTrasmettitore.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.grpTrasmettitore.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.grpTrasmettitore.Style.BorderLeftWidth = 1
        Me.grpTrasmettitore.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.grpTrasmettitore.Style.BorderRightWidth = 1
        Me.grpTrasmettitore.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.grpTrasmettitore.Style.BorderTopWidth = 1
        Me.grpTrasmettitore.Style.CornerDiameter = 4
        Me.grpTrasmettitore.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.grpTrasmettitore.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.grpTrasmettitore.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.grpTrasmettitore.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        '
        '
        '
        Me.grpTrasmettitore.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.grpTrasmettitore.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.grpTrasmettitore.TabIndex = 6
        Me.grpTrasmettitore.Text = "Trasmettitore"
        '
        'lvwTrasmettitore
        '
        Me.lvwTrasmettitore.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwTrasmettitore.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColDettaglio})
        Me.lvwTrasmettitore.FullRowSelect = True
        Me.lvwTrasmettitore.GridLines = True
        Me.lvwTrasmettitore.HideSelection = False
        Me.lvwTrasmettitore.Location = New System.Drawing.Point(4, 4)
        Me.lvwTrasmettitore.Name = "lvwTrasmettitore"
        Me.lvwTrasmettitore.Size = New System.Drawing.Size(836, 132)
        Me.lvwTrasmettitore.TabIndex = 0
        Me.lvwTrasmettitore.UseCompatibleStateImageBehavior = False
        Me.lvwTrasmettitore.View = System.Windows.Forms.View.Details
        '
        'ColDettaglio
        '
        Me.ColDettaglio.Text = "Dettaglio"
        Me.ColDettaglio.Width = 808
        '
        'txtArticoli
        '
        Me.txtArticoli.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtArticoli.Location = New System.Drawing.Point(12, 179)
        Me.txtArticoli.Name = "txtArticoli"
        Me.txtArticoli.Size = New System.Drawing.Size(419, 20)
        Me.txtArticoli.TabIndex = 7
        '
        'txtClientiFornitori
        '
        Me.txtClientiFornitori.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtClientiFornitori.Location = New System.Drawing.Point(436, 179)
        Me.txtClientiFornitori.Name = "txtClientiFornitori"
        Me.txtClientiFornitori.Size = New System.Drawing.Size(419, 20)
        Me.txtClientiFornitori.TabIndex = 8
        '
        'btnSalvaImpostazioni
        '
        Me.btnSalvaImpostazioni.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSalvaImpostazioni.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.btnSalvaImpostazioni.Location = New System.Drawing.Point(743, 3)
        Me.btnSalvaImpostazioni.Name = "btnSalvaImpostazioni"
        Me.btnSalvaImpostazioni.Size = New System.Drawing.Size(113, 66)
        Me.btnSalvaImpostazioni.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnSalvaImpostazioni.TabIndex = 9
        Me.btnSalvaImpostazioni.Text = "Salva Impostazioni"
        '
        'txtTempo
        '
        Me.txtTempo.Location = New System.Drawing.Point(616, 6)
        Me.txtTempo.Name = "txtTempo"
        Me.txtTempo.Size = New System.Drawing.Size(81, 20)
        Me.txtTempo.TabIndex = 10
        '
        'lblTmpAggiornamento
        '
        Me.lblTmpAggiornamento.AutoSize = True
        Me.lblTmpAggiornamento.Location = New System.Drawing.Point(452, 9)
        Me.lblTmpAggiornamento.Name = "lblTmpAggiornamento"
        Me.lblTmpAggiornamento.Size = New System.Drawing.Size(158, 13)
        Me.lblTmpAggiornamento.TabIndex = 11
        Me.lblTmpAggiornamento.Text = "Tempo di aggiornamento import:"
        '
        'lblPathArticoli
        '
        Me.lblPathArticoli.AutoSize = True
        Me.lblPathArticoli.Location = New System.Drawing.Point(11, 160)
        Me.lblPathArticoli.Name = "lblPathArticoli"
        Me.lblPathArticoli.Size = New System.Drawing.Size(82, 13)
        Me.lblPathArticoli.TabIndex = 12
        Me.lblPathArticoli.Text = "Percorso articoli"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(433, 160)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(121, 13)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "Percorso clienti/ fornitori"
        '
        'lblMinuti
        '
        Me.lblMinuti.AutoSize = True
        Me.lblMinuti.Location = New System.Drawing.Point(703, 9)
        Me.lblMinuti.Name = "lblMinuti"
        Me.lblMinuti.Size = New System.Drawing.Size(34, 13)
        Me.lblMinuti.TabIndex = 14
        Me.lblMinuti.Text = "minuti"
        '
        'TimerRefreshImportazione
        '
        Me.TimerRefreshImportazione.Enabled = True
        Me.TimerRefreshImportazione.Interval = 6000
        '
        'btnRicaricaArticoli
        '
        Me.btnRicaricaArticoli.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnRicaricaArticoli.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.btnRicaricaArticoli.Location = New System.Drawing.Point(247, 150)
        Me.btnRicaricaArticoli.Name = "btnRicaricaArticoli"
        Me.btnRicaricaArticoli.Size = New System.Drawing.Size(183, 23)
        Me.btnRicaricaArticoli.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnRicaricaArticoli.TabIndex = 15
        Me.btnRicaricaArticoli.Text = "Forza ricaricamento articoli"
        '
        'TimerControllerLog
        '
        Me.TimerControllerLog.Enabled = True
        Me.TimerControllerLog.Interval = 5000
        '
        'txtFattureGen
        '
        Me.txtFattureGen.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFattureGen.Location = New System.Drawing.Point(12, 124)
        Me.txtFattureGen.Name = "txtFattureGen"
        Me.txtFattureGen.Size = New System.Drawing.Size(418, 20)
        Me.txtFattureGen.TabIndex = 16
        '
        'BtnRicaricaFatture
        '
        Me.BtnRicaricaFatture.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.BtnRicaricaFatture.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.BtnRicaricaFatture.Location = New System.Drawing.Point(673, 94)
        Me.BtnRicaricaFatture.Name = "BtnRicaricaFatture"
        Me.BtnRicaricaFatture.Size = New System.Drawing.Size(183, 23)
        Me.BtnRicaricaFatture.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.BtnRicaricaFatture.TabIndex = 17
        Me.BtnRicaricaFatture.Text = "Forza ricaricamento fatture"
        '
        'txtFattureRig
        '
        Me.txtFattureRig.Location = New System.Drawing.Point(437, 123)
        Me.txtFattureRig.Name = "txtFattureRig"
        Me.txtFattureRig.Size = New System.Drawing.Size(418, 20)
        Me.txtFattureRig.TabIndex = 18
        '
        'txtDestinazioniClienti
        '
        Me.txtDestinazioniClienti.Location = New System.Drawing.Point(435, 218)
        Me.txtDestinazioniClienti.Name = "txtDestinazioniClienti"
        Me.txtDestinazioniClienti.Size = New System.Drawing.Size(420, 20)
        Me.txtDestinazioniClienti.TabIndex = 19
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 108)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(80, 13)
        Me.Label2.TabIndex = 20
        Me.Label2.Text = "Fatture generali"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(434, 108)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(66, 13)
        Me.Label4.TabIndex = 21
        Me.Label4.Text = "Fatture righe"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(434, 202)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(94, 13)
        Me.Label5.TabIndex = 22
        Me.Label5.Text = "Destinazioni clienti"
        '
        'btnSvuotaTrasmettitore
        '
        Me.btnSvuotaTrasmettitore.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSvuotaTrasmettitore.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.btnSvuotaTrasmettitore.Location = New System.Drawing.Point(673, 244)
        Me.btnSvuotaTrasmettitore.Name = "btnSvuotaTrasmettitore"
        Me.btnSvuotaTrasmettitore.Size = New System.Drawing.Size(183, 23)
        Me.btnSvuotaTrasmettitore.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2013
        Me.btnSvuotaTrasmettitore.TabIndex = 23
        Me.btnSvuotaTrasmettitore.Text = "Svuota lista"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(703, 52)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(32, 13)
        Me.Label6.TabIndex = 26
        Me.Label6.Text = "giorni"
        '
        'lblTempoEliminazione
        '
        Me.lblTempoEliminazione.AutoSize = True
        Me.lblTempoEliminazione.Location = New System.Drawing.Point(520, 52)
        Me.lblTempoEliminazione.Name = "lblTempoEliminazione"
        Me.lblTempoEliminazione.Size = New System.Drawing.Size(90, 13)
        Me.lblTempoEliminazione.TabIndex = 25
        Me.lblTempoEliminazione.Text = "Eliminazione File: "
        '
        'txtTempoEliminazioneFile
        '
        Me.txtTempoEliminazioneFile.Location = New System.Drawing.Point(616, 49)
        Me.txtTempoEliminazioneFile.Name = "txtTempoEliminazioneFile"
        Me.txtTempoEliminazioneFile.Size = New System.Drawing.Size(81, 20)
        Me.txtTempoEliminazioneFile.TabIndex = 24
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(703, 31)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(34, 13)
        Me.Label1.TabIndex = 29
        Me.Label1.Text = "minuti"
        '
        'lblTempAggiornamentoFatt
        '
        Me.lblTempAggiornamentoFatt.AutoSize = True
        Me.lblTempAggiornamentoFatt.Location = New System.Drawing.Point(450, 31)
        Me.lblTempAggiornamentoFatt.Name = "lblTempAggiornamentoFatt"
        Me.lblTempAggiornamentoFatt.Size = New System.Drawing.Size(160, 13)
        Me.lblTempAggiornamentoFatt.TabIndex = 28
        Me.lblTempAggiornamentoFatt.Text = "Tempo di aggiornamento fatture:"
        '
        'txtTempoAggiornamentoFatt
        '
        Me.txtTempoAggiornamentoFatt.Location = New System.Drawing.Point(616, 28)
        Me.txtTempoAggiornamentoFatt.Name = "txtTempoAggiornamentoFatt"
        Me.txtTempoAggiornamentoFatt.Size = New System.Drawing.Size(81, 20)
        Me.txtTempoAggiornamentoFatt.TabIndex = 27
        '
        'TimerRefreshFatture
        '
        Me.TimerRefreshFatture.Enabled = True
        '
        'FrmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(202, Byte), Integer), CType(CType(225, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(859, 432)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTempAggiornamentoFatt)
        Me.Controls.Add(Me.txtTempoAggiornamentoFatt)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.lblTempoEliminazione)
        Me.Controls.Add(Me.txtTempoEliminazioneFile)
        Me.Controls.Add(Me.btnSvuotaTrasmettitore)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtDestinazioniClienti)
        Me.Controls.Add(Me.txtFattureRig)
        Me.Controls.Add(Me.BtnRicaricaFatture)
        Me.Controls.Add(Me.txtFattureGen)
        Me.Controls.Add(Me.btnRicaricaArticoli)
        Me.Controls.Add(Me.lblMinuti)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.lblPathArticoli)
        Me.Controls.Add(Me.lblTmpAggiornamento)
        Me.Controls.Add(Me.txtTempo)
        Me.Controls.Add(Me.btnSalvaImpostazioni)
        Me.Controls.Add(Me.txtClientiFornitori)
        Me.Controls.Add(Me.txtArticoli)
        Me.Controls.Add(Me.grpTrasmettitore)
        Me.Controls.Add(Me.btnRicaricaClientiFornitori)
        Me.Controls.Add(Me.ReflectionLabel1)
        Me.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(875, 471)
        Me.MinimumSize = New System.Drawing.Size(875, 471)
        Me.Name = "FrmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Connettore MagTrace"
        Me.grpTrasmettitore.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ReflectionLabel1 As DevComponents.DotNetBar.Controls.ReflectionLabel
    Friend WithEvents btnRicaricaClientiFornitori As DevComponents.DotNetBar.ButtonX
    Friend WithEvents grpTrasmettitore As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents txtArticoli As TextBox
    Friend WithEvents txtClientiFornitori As TextBox
    Friend WithEvents lvwTrasmettitore As ListView
    Friend WithEvents btnSalvaImpostazioni As DevComponents.DotNetBar.ButtonX
    Friend WithEvents ColDettaglio As ColumnHeader
    Friend WithEvents txtTempo As TextBox
    Friend WithEvents lblTmpAggiornamento As Label
    Friend WithEvents lblPathArticoli As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents lblMinuti As Label
    Friend WithEvents TimerRefreshImportazione As Timer
    Friend WithEvents btnRicaricaArticoli As DevComponents.DotNetBar.ButtonX
    Friend WithEvents TimerControllerLog As Timer
    Friend WithEvents txtFattureGen As TextBox
    Friend WithEvents BtnRicaricaFatture As DevComponents.DotNetBar.ButtonX
    Friend WithEvents txtFattureRig As TextBox
    Friend WithEvents txtDestinazioniClienti As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents btnSvuotaTrasmettitore As DevComponents.DotNetBar.ButtonX
    Friend WithEvents Label6 As Label
    Friend WithEvents lblTempoEliminazione As Label
    Friend WithEvents txtTempoEliminazioneFile As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents lblTempAggiornamentoFatt As Label
    Friend WithEvents txtTempoAggiornamentoFatt As TextBox
    Friend WithEvents TimerRefreshFatture As Timer
End Class
