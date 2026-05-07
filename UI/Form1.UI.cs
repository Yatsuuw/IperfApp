namespace IperfApp.UI;

public partial class Form1 : Form
{
  private void SetupModernUI()
  {
    Text = " Speedtest Iperf";
    Size = new Size(580, 760);
    BackColor = _colorBackground;
    FormBorderStyle = FormBorderStyle.FixedSingle;
    MaximizeBox = false;
    StartPosition = FormStartPosition.CenterScreen;

    // --- Barre de menu ---
    var ms = new MenuStrip { BackColor = _colorCard, Padding = new Padding(6, 4, 0, 4) };

    var menuProfils = new ToolStripMenuItem("Profils")
    {
      ForeColor = _colorAccent,
      Font = new Font("Segoe UI Semibold", 9F)
    };
    menuProfils.Click += (_, _) => OpenSettings();

    var menuConfig = new ToolStripMenuItem("Configuration")
    {
      ForeColor = Color.DimGray,
      Font = new Font("Segoe UI", 9F)
    };
    menuConfig.DropDownItems.AddRange([
      new ToolStripMenuItem("Importer une configuration...", null, (_, _) => ImportConfiguration()),
      new ToolStripMenuItem("Exporter une configuration...", null, (_, _) => ExportConfiguration())
    ]);

    var menuInfo = new ToolStripMenuItem("Informations")
    {
      ForeColor = Color.DimGray,
      Font = new Font("Segoe UI", 9F)
    };
    menuInfo.Click += (_, _) => ShowAboutBox();

    ms.Items.AddRange([menuProfils, menuConfig, menuInfo]);
    MainMenuStrip = ms;
    Controls.Add(ms);

    int clientW = ClientSize.Width;
    int cardW   = 460;
    int startX  = (clientW - cardW) / 2;

    // --- Titre ---
    var lblTitle = new Label
    {
      Text      = "DÉBIT RÉSEAU",
      Font      = new Font("Segoe UI Variable Display", 16F, FontStyle.Bold),
      ForeColor = _colorAccent,
      Location  = new Point(0, 45),
      Size      = new Size(clientW, 35),
      TextAlign = ContentAlignment.MiddleCenter
    };

    // --- Carte de configuration ---
    var pnlCard = new Panel
    {
      BackColor = _colorCard,
      Size      = new Size(cardW, 255),
      Location  = new Point(startX, 95)
    };
    pnlCard.Paint += (_, e) =>
      ControlPaint.DrawBorder(e.Graphics, pnlCard.ClientRectangle,
        Color.FromArgb(230, 235, 240), ButtonBorderStyle.Solid);

    int internalTop = 20;
    const int labelW = 100, inputW = 280, gap = 15;
    int rowX = (cardW - (labelW + gap + inputW)) / 2;

    // Profil
    var lblPreset = new Label
    {
      Text = "Profil :", Top = internalTop + 3, Left = rowX, Width = labelW,
      Font = new Font("Segoe UI Semibold", 9F),
      TextAlign = ContentAlignment.MiddleRight, ForeColor = _colorAccent
    };
    cbPresets = new ComboBox
    {
      Top = internalTop, Left = rowX + labelW + gap, Width = inputW - 45,
      DropDownStyle = ComboBoxStyle.DropDownList,
      Font = new Font("Segoe UI", 10F)
    };
    pnlCard.Controls.AddRange([lblPreset, cbPresets]);
    internalTop += 45;

    // Champs de saisie
    txtServer   = AddModernInput (pnlCard, ref internalTop, "Serveur :",  "", "Adresse du serveur", rowX, labelW, inputW, gap);
    txtPort     = AddNumericInput(pnlCard, ref internalTop, "Port :",     "", "5201",               rowX, labelW, inputW, gap);
    txtChannels = AddNumericInput(pnlCard, ref internalTop, "Canaux :",   "", "8",                  rowX, labelW, inputW, gap);

    // Protocole IP
    var lblIpVersion = new Label
    {
      Text = "Protocole :", Top = internalTop + 3, Left = rowX, Width = labelW,
      Font = new Font("Segoe UI Semibold", 9F),
      TextAlign = ContentAlignment.MiddleRight, ForeColor = Color.DimGray
    };
    cbIpVersion = new ComboBox
    {
      Top = internalTop, Left = rowX + labelW + gap, Width = inputW,
      DropDownStyle = ComboBoxStyle.DropDownList,
      Font = new Font("Segoe UI", 10F)
    };
    cbIpVersion.Items.AddRange(["Auto (défaut)", "IPv4 (-4)", "IPv6 (-6)"]);
    cbIpVersion.SelectedIndex = 0;
    _mainToolTip.SetToolTip(cbIpVersion, "Force le protocole IP utilisé par iperf3 (-4, -6, ou auto)");
    pnlCard.Controls.AddRange([lblIpVersion, cbIpVersion]);

    // --- Bouton Lancer ---
    btnStart = new Button
    {
      Text      = "LANCER L'ANALYSE",
      Top       = 365,
      Width     = cardW,
      Height    = 50,
      BackColor = _colorAccent,
      ForeColor = Color.White,
      FlatStyle = FlatStyle.Flat,
      Font      = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
      Cursor    = Cursors.Hand,
      Left      = startX
    };
    btnStart.FlatAppearance.BorderSize = 0;
    btnStart.Click += async (_, _) => await RunFullTest();

    // --- Bouton Annuler ---
    btnCancel = new Button
    {
      Text      = "ANNULER",
      Top       = 420,
      Width     = cardW,
      Height    = 30,
      BackColor = _colorDanger,
      ForeColor = Color.White,
      FlatStyle = FlatStyle.Flat,
      Font      = new Font("Segoe UI Semibold", 9F),
      Cursor    = Cursors.Hand,
      Left      = startX,
      Enabled   = false
    };
    btnCancel.FlatAppearance.BorderSize = 0;
    btnCancel.Click += (_, _) =>
    {
      _testCts?.Cancel();
      btnCancel.Enabled = false;
    };

    // --- Console de logs ---
    txtLog = new TextBox
    {
      Multiline    = true,
      ReadOnly     = true,
      ScrollBars   = ScrollBars.Vertical,
      Top          = 460,
      Width        = cardW,
      Height       = 170,
      BackColor    = _colorTerminal,
      ForeColor    = Color.FromArgb(220, 220, 220),
      Font         = new Font("Consolas", 9F),
      BorderStyle  = BorderStyle.None,
      Left         = startX
    };

    // --- Boutons Export ---
    int btnW = (cardW / 2) - 5;
    btnExportNew    = CreateGhostButton("Nouveau rapport",   645, startX,          btnW);
    btnExportAppend = CreateGhostButton("Ajouter au fichier", 645, startX + btnW + 10, btnW);
    btnExportNew.Click    += (_, _) => HandleSave(false);
    btnExportAppend.Click += (_, _) => HandleSave(true);

    // --- Infobulles ---
    _mainToolTip.SetToolTip(cbPresets,   "Sélectionnez un profil pré-enregistré.");
    _mainToolTip.SetToolTip(txtServer,   "Adresse IP ou nom d'hôte du serveur Iperf3.");
    _mainToolTip.SetToolTip(txtPort,     "Port de destination (souvent 5201 ou 9240).");
    _mainToolTip.SetToolTip(txtChannels, "Nombre de flux TCP parallèles (recommandé : 8).");
    _mainToolTip.SetToolTip(btnCancel,   "Annule le test en cours.");

    Controls.AddRange([lblTitle, pnlCard, btnStart, btnCancel, txtLog, btnExportNew, btnExportAppend]);

    // Chargement de la config (une seule fois, ici)
    LoadConfigIntoUI();
  }
}
