namespace IperfApp.UI;

public partial class Form1 : Form
{
  private void SetupModernUI()
  {
    // Fenêtre compacte (580x720)
    Text = " Speedtest Iperf";
    Size = new Size(580, 720);
    BackColor = _colorBackground;
    FormBorderStyle = FormBorderStyle.FixedSingle;
    MaximizeBox = false;
    StartPosition = FormStartPosition.CenterScreen;

    // Barre menu
    var ms = new MenuStrip { BackColor = _colorCard, Padding = new Padding(6, 4, 0, 4) };
    var info = new ToolStripMenuItem("Informations") { ForeColor = Color.DimGray };
    info.Click += (s, e) => ShowAboutBox();
    ms.Items.Add(info);
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

    // Carte de Configuration
    var pnlCard = new Panel {
      BackColor = _colorCard,
      Size = new Size(cardW, 160),
      Location = new Point(startX, 95)
    };
    pnlCard.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, pnlCard.ClientRectangle, Color.FromArgb(230, 235, 240), ButtonBorderStyle.Solid);

    int internalTop = 20;
    int labelW = 100, inputW = 280, gap = 15;
    int rowX = (cardW - (labelW + gap + inputW)) / 2;

    txtServer = AddModernInput(pnlCard, ref internalTop, "Serveur :", "poi.cubic.iperf.bytel.fr", "Adresse du serveur", rowX, labelW, inputW, gap);
    txtPort = AddModernInput(pnlCard, ref internalTop, "Port :", "9240", "Port (5201 par défaut)", rowX, labelW, inputW, gap);
    txtChannels = AddModernInput(pnlCard, ref internalTop, "Canaux :", "8", "Flux parallèles", rowX, labelW, inputW, gap);

    // Bouton d'Analyse
    btnStart = new Button
    {
      Text = "LANCER L'ANALYSE",
      Top = 270,
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
    txtLog = new TextBox
    {
      Multiline = true,
      ReadOnly = true,
      ScrollBars = ScrollBars.Vertical,
      Top = 335,
      Width = cardW,
      Height = 220,
      BackColor = _colorTerminal,
      ForeColor = Color.FromArgb(220, 220, 220),
      Font = new Font("Consolas", 9F),
      BorderStyle = BorderStyle.None,
      Left = startX
    };

    // Boutons Export (Ligne unique)
    int btnExportW = (cardW / 2) - 5;
    btnExportNew = CreateGhostButton("Nouveau fichier", 570, startX, btnExportW);
    btnExportNew.Click += (s, e) => HandleSave(false);

    btnExportAppend = CreateGhostButton("Ajouter au fichier", 570, startX + btnExportW + 10, btnExportW);
    btnExportAppend.Click += (s, e) => HandleSave(true);

    // --- INFOBULLES ---
    _mainToolTip.SetToolTip(btnStart, "Démarre le test de débit montant et descendant.");
    _mainToolTip.SetToolTip(btnExportNew, "Crée un nouveau fichier CSV avec les résultats.");
    _mainToolTip.SetToolTip(btnExportAppend, "Ajoute les résultats à la suite d'un fichier existant.");

    _mainToolTip.SetToolTip(txtServer, "Adresse ou IP du serveur Iperf3 à tester.");
    _mainToolTip.SetToolTip(txtPort, "Port de communication du serveur (généralement 5201).");
    _mainToolTip.SetToolTip(txtChannels, "Nombre de connexions TCP simultanées (recommandé : 8).");

    Controls.AddRange([lblTitle, pnlCard, btnStart, txtLog, btnExportNew, btnExportAppend]);
  }
}