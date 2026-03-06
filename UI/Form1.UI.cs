namespace IperfApp.UI;

public partial class Form1 : Form
{
  private void SetupModernUI()
  {
    // Fenêtre
    Text = " Speedtest Iperf";
    Size = new Size(620, 900);
    BackColor = _colorBackground;
    FormBorderStyle = FormBorderStyle.FixedSingle;
    MaximizeBox = false;
    StartPosition = FormStartPosition.CenterScreen;

    // Barre menu
    var ms = new MenuStrip { BackColor = _colorCard, Padding = new Padding(6, 2, 0, 2) };
    var info = new ToolStripMenuItem("Informations") { ForeColor = _colorText };
    info.Click += (s, e) => ShowAboutBox();
    ms.Items.Add(info);
    MainMenuStrip = ms;
    Controls.Add(ms);

    // Centrage des éléments
    int clientW = ClientSize.Width;
    int labelW = 130;
    int inputW = 310;
    int gap = 15;
    int totalRowWidth = labelW + gap + inputW;
    int startX = (clientW - totalRowWidth) / 2;

    // Titre
    var lblTitle = new Label
    {
      Text = "DÉBIT RÉSEAU",
      Font = new Font("Segoe UI Variable Display", 18F, FontStyle.Bold),
      ForeColor = _colorAccent,
      Location = new Point(0, 60),
      Size = new Size(clientW, 45),
      TextAlign = ContentAlignment.MiddleCenter
    };

    // Formulaire de configuration
    var pnlCard = new Panel
    {
      BackColor = _colorCard,
      Size = new Size(totalRowWidth + 60, 220),
      Location = new Point((clientW - (totalRowWidth + 60)) / 2, 120)
    };
    
    // Bordure
    pnlCard.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, pnlCard.ClientRectangle, Color.FromArgb(225, 230, 235), ButtonBorderStyle.Solid);

    // Champs de saisie
    int internalTop = 30;
    txtServer = AddModernInput(pnlCard, ref internalTop, "Serveur :", "poi.cubic.iperf.bytel.fr", "Adresse du serveur Iperf", 30, labelW, inputW, gap);
    txtPort = AddModernInput(pnlCard, ref internalTop, "Port :", "9240", "Port du serveur Iperf (5201 par défaut)", 30, labelW, inputW, gap);
    txtChannels = AddModernInput(pnlCard, ref internalTop, "Canaux :", "8", "Nombre de connexions (1 par défaut)", 30, labelW, inputW, gap);

    // Bouton d'analyse
    btnStart = new Button
    {
      Text = "LANCER L'ANALYSE",
      Top = 330, Width = totalRowWidth + 60, Height = 65,
      BackColor = _colorAccent, ForeColor = Color.White,
      FlatStyle = FlatStyle.Flat,
      Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
      Cursor = Cursors.Hand
    };
    btnStart.Left = (clientW - btnStart.Width) / 2;
    btnStart.FlatAppearance.BorderSize = 0;
    btnStart.Click += async (s, e) => await RunFullTest();

    // Console
    txtLog = new TextBox
    {
      Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
      Top = 420, Width = totalRowWidth + 60, Height = 300,
      BackColor = _colorTerminal, ForeColor = Color.FromArgb(210, 210, 210),
      Font = new Font("Consolas", 10F),
      BorderStyle = BorderStyle.None
    };
    txtLog.Left = (clientW - txtLog.Width) / 2;

    // Export
    btnExportNew = CreateGhostButton("Créer un nouveau rapport CSV", 740, txtLog.Left, txtLog.Width);
    btnExportNew.Click += (s, e) => HandleSave(false);

    btnExportAppend = CreateGhostButton("Ajouter au rapport existant", 795, txtLog.Left, txtLog.Width);
    btnExportAppend.Click += (s, e) => HandleSave(true);

    // Ajouts contrôles
    Controls.AddRange([lblTitle, pnlCard, btnStart, txtLog, btnExportNew, btnExportAppend]);
  }
}
