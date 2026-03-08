namespace IperfApp.UI;

public partial class Form1 : Form
{
    private void SetupModernUI()
    {
        // Fenêtre plus compacte et élégante
        this.Text = " Speedtest Iperf Pro";
        this.Size = new Size(580, 720); // Réduction de la hauteur (900 -> 720)
        this.BackColor = _colorBackground;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;

        // Menu épuré
        var ms = new MenuStrip { BackColor = _colorCard, Padding = new Padding(6, 4, 0, 4) };
        var info = new ToolStripMenuItem("Informations") { ForeColor = Color.DimGray, Font = new Font("Segoe UI", 9F) };
        info.Click += (s, e) => ShowAboutBox();
        ms.Items.Add(info);
        this.MainMenuStrip = ms;
        this.Controls.Add(ms);

        int clientW = this.ClientSize.Width;
        int cardW = 460;
        int startX = (clientW - cardW) / 2;

        // Titre - On réduit l'espace supérieur
        var lblTitle = new Label {
            Text = "DÉBIT RÉSEAU",
            Font = new Font("Segoe UI Variable Display", 16F, FontStyle.Bold),
            ForeColor = _colorAccent,
            Location = new Point(0, 45),
            Size = new Size(clientW, 35),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Carte de Configuration - Plus serrée
        var pnlCard = new Panel {
            BackColor = _colorCard,
            Size = new Size(cardW, 160), // Réduction de 220 à 160
            Location = new Point(startX, 95)
        };
        pnlCard.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, pnlCard.ClientRectangle, Color.FromArgb(230, 235, 240), ButtonBorderStyle.Solid);

        int internalTop = 20;
        int labelW = 100, inputW = 280, gap = 15;
        int rowX = (cardW - (labelW + gap + inputW)) / 2;

        txtServer = AddModernInput(pnlCard, ref internalTop, "Serveur :", "poi.cubic.iperf.bytel.fr", "Adresse Iperf", rowX, labelW, inputW, gap);
        txtPort = AddModernInput(pnlCard, ref internalTop, "Port :", "9240", "5201", rowX, labelW, inputW, gap);
        txtChannels = AddModernInput(pnlCard, ref internalTop, "Canaux :", "8", "1", rowX, labelW, inputW, gap);

        // Bouton d'Analyse - Hauteur réduite pour l'élégance
        btnStart = new Button {
            Text = "LANCER L'ANALYSE",
            Top = 270, Width = cardW, Height = 50,
            BackColor = _colorAccent, ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnStart.Left = startX;
        btnStart.FlatAppearance.BorderSize = 0;
        btnStart.Click += async (s, e) => await RunFullTest();

        // Console - Plus basse, fond légèrement plus doux
        txtLog = new TextBox {
            Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
            Top = 335, Width = cardW, Height = 220, // Hauteur optimisée
            BackColor = Color.FromArgb(35, 35, 38), 
            ForeColor = Color.FromArgb(220, 220, 220),
            Font = new Font("Consolas", 9F),
            BorderStyle = BorderStyle.None
        };
        txtLog.Left = startX;

        // Boutons Export - Alignés sur une seule ligne pour gagner de la place
        int btnExportW = (cardW / 2) - 5;
        btnExportNew = CreateGhostButton("Nouveau rapport CSV", 570, startX, btnExportW);
        btnExportNew.Click += (s, e) => HandleSave(false);

        btnExportAppend = CreateGhostButton("Ajouter au fichier", 570, startX + btnExportW + 10, btnExportW);
        btnExportAppend.Click += (s, e) => HandleSave(true);

        this.Controls.AddRange([lblTitle, pnlCard, btnStart, txtLog, btnExportNew, btnExportAppend]);
    }
}