namespace IperfApp.UI;

public partial class Form1 : Form
{
  private ComboBox cbPresets = null!;
  private readonly Button btnSettings = null!;

  private void SetupModernUI()
  {
    // Fenêtre (580x720)
    Text = " Speedtest Iperf";
    Size = new Size(580, 720);
    BackColor = _colorBackground;
    FormBorderStyle = FormBorderStyle.FixedSingle;
    MaximizeBox = false;
    StartPosition = FormStartPosition.CenterScreen;

    // Barre menu
    var ms = new MenuStrip { BackColor = _colorCard, Padding = new Padding(6, 4, 0, 4) };

    // Entrée "Profils" avec style semi-bold
    var menuProfils = new ToolStripMenuItem("Profils") { 
      ForeColor = _colorAccent, 
      Font = new Font("Segoe UI Semibold", 9F) 
    };
    menuProfils.Click += (s, e) => OpenSettings();
    
    var info = new ToolStripMenuItem("Informations") { 
      ForeColor = Color.DimGray, 
      Font = new Font("Segoe UI", 9F) 
    };
    info.Click += (s, e) => ShowAboutBox();

    ms.Items.AddRange([menuProfils, info]);
    MainMenuStrip = ms;
    Controls.Add(ms);

    int clientW = ClientSize.Width;
    int cardW = 460;
    int startX = (clientW - cardW) / 2;

    // Titre
    var lblTitle = new Label {
      Text = "DÉBIT RÉSEAU",
      Font = new Font("Segoe UI Variable Display", 16F, FontStyle.Bold),
      ForeColor = _colorAccent,
      Location = new Point(0, 45),
      Size = new Size(clientW, 35),
      TextAlign = ContentAlignment.MiddleCenter
    };

    // Carte de Configuration (Agrandie à 210px pour le menu Profil)
    var pnlCard = new Panel {
      BackColor = _colorCard,
      Size = new Size(cardW, 210),
      Location = new Point(startX, 95)
    };
    pnlCard.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, pnlCard.ClientRectangle, Color.FromArgb(230, 235, 240), ButtonBorderStyle.Solid);

    int internalTop = 20;
    int labelW = 100, inputW = 280, gap = 15;
    int rowX = (cardW - (labelW + gap + inputW)) / 2;

      // --- ZONE PROFIL / SCÉNARIO ---
    var lblPreset = new Label { 
      Text = "Profil :", Top = internalTop + 3, Left = rowX, Width = labelW, 
      Font = new Font("Segoe UI Semibold", 9F), TextAlign = ContentAlignment.MiddleRight, ForeColor = _colorAccent 
    };
    cbPresets = new ComboBox { 
      Top = internalTop, Left = rowX + labelW + gap, Width = inputW - 45, 
      DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) 
    };

    pnlCard.Controls.AddRange([lblPreset, cbPresets, btnSettings]);
    internalTop += 45;

    // --- CHAMPS DE SAISIE ---
    txtServer = AddModernInput(pnlCard, ref internalTop, "Serveur :", "", "Adresse du serveur", rowX, labelW, inputW, gap);
    txtPort = AddModernInput(pnlCard, ref internalTop, "Port :", "", "5201", rowX, labelW, inputW, gap);
    txtChannels = AddModernInput(pnlCard, ref internalTop, "Canaux :", "", "8", rowX, labelW, inputW, gap);

    // Bouton d'Analyse (Recentré avec btnStart.Left)
    btnStart = new Button
    {
      Text = "LANCER L'ANALYSE",
      Top = 320,
      Width = cardW,
      Height = 50,
      BackColor = _colorAccent,
      ForeColor = Color.White,
      FlatStyle = FlatStyle.Flat,
      Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
      Cursor = Cursors.Hand,
      Left = startX
    };
    btnStart.FlatAppearance.BorderSize = 0;
    btnStart.Click += async (s, e) => await RunFullTest();

    // Console
    txtLog = new TextBox {
      Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
      Top = 385, Width = cardW, Height = 170,
      BackColor = _colorTerminal, ForeColor = Color.FromArgb(220, 220, 220),
      Font = new Font("Consolas", 9F),
      BorderStyle = BorderStyle.None,
      Left = startX
    };

    // Boutons Export
    int btnExportW = (cardW / 2) - 5;
    btnExportNew = CreateGhostButton("Nouveau rapport", 570, startX, btnExportW);
    btnExportNew.Click += (s, e) => HandleSave(false);

    btnExportAppend = CreateGhostButton("Ajouter au fichier", 570, startX + btnExportW + 10, btnExportW);
    btnExportAppend.Click += (s, e) => HandleSave(true);

    // Infobulles
    if (cbPresets != null) _mainToolTip.SetToolTip(cbPresets, "Sélectionnez un profil pré-enregistré.");
    if (txtServer != null) _mainToolTip.SetToolTip(txtServer, "Adresse IP ou nom d'hôte du serveur Iperf3.");
    if (txtPort != null) _mainToolTip.SetToolTip(txtPort, "Port de destination (souvent 5201 ou 9240).");
    if (txtChannels != null) _mainToolTip.SetToolTip(txtChannels, "Nombre de flux TCP parallèles (recommandé : 8).");

    Controls.AddRange([lblTitle, pnlCard, btnStart, txtLog, btnExportNew, btnExportAppend]);

    // Charger la config après l'init de l'UI
    LoadConfigIntoUI();
  }
}