using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm : Form
{
    // ---------------------------------------------------------------
    // Constantes de mise en page partagées entre les builders
    // ---------------------------------------------------------------

    private const int CardWidth  = 460;
    private const int CardTop    = 95;
    private const int CardHeight = 255;
    private const int CardBottom = CardTop + CardHeight;   // 350

    private int CardLeft => (ClientSize.Width - CardWidth) / 2;

    // ---------------------------------------------------------------
    // Point d'entrée principal
    // ---------------------------------------------------------------

    /// <summary>Configure la fenêtre principale et construit tous les contrôles.</summary>
    private void SetupModernUI()
    {
        Text            = " Speedtest Iperf";
        Size            = new Size(580, 760);
        BackColor       = AppColors.Background;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        StartPosition   = FormStartPosition.CenterScreen;

        var lblTitle = new Label
        {
            Text      = "DÉBIT RÉSEAU",
            Font      = new Font("Segoe UI Variable Display", 16F, FontStyle.Bold),
            ForeColor = AppColors.Accent,
            Location  = new Point(0, 45),
            Size      = new Size(ClientSize.Width, 35),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var menuStrip = BuildMenuStrip();
        var pnlCard   = BuildConfigCard();
        BuildActionsArea();

        _mainToolTip.SetToolTip(cbPresets,   "Sélectionnez un profil pré-enregistré.");
        _mainToolTip.SetToolTip(txtServer,   "Adresse IP ou nom d'hôte du serveur Iperf3.");
        _mainToolTip.SetToolTip(txtPort,     "Port de destination (souvent 5201 ou 9240).");
        _mainToolTip.SetToolTip(txtChannels, "Nombre de flux TCP parallèles (recommandé : 8).");
        _mainToolTip.SetToolTip(btnCancel,   "Annule le test en cours.");

        MainMenuStrip = menuStrip;
        Controls.Add(menuStrip);
        Controls.AddRange([lblTitle, pnlCard, btnStart, btnCancel, txtLog, btnExportNew, btnExportAppend]);

        LoadConfigIntoUI();
    }

    // ---------------------------------------------------------------
    // Builder : barre de menus
    // ---------------------------------------------------------------

    private MenuStrip BuildMenuStrip()
    {
        var ms = new MenuStrip
        {
            BackColor = AppColors.Card,
            Padding   = new Padding(6, 4, 0, 4)
        };

        var menuProfils = new ToolStripMenuItem("Profils")
        {
            ForeColor = AppColors.Accent,
            Font      = new Font("Segoe UI Semibold", 9F)
        };
        menuProfils.Click += (_, _) => OpenSettings();

        var menuConfig = new ToolStripMenuItem("Configuration")
        {
            ForeColor = AppColors.TextMuted,
            Font      = new Font("Segoe UI", 9F)
        };
        menuConfig.DropDownItems.AddRange([
            new ToolStripMenuItem("Importer une configuration...", null, (_, _) => ImportConfiguration()),
            new ToolStripMenuItem("Exporter une configuration...", null, (_, _) => ExportConfiguration())
        ]);

        var menuInfo = new ToolStripMenuItem("Informations")
        {
            ForeColor = AppColors.TextMuted,
            Font      = new Font("Segoe UI", 9F)
        };
        menuInfo.Click += (_, _) => ShowAboutBox();

        ms.Items.AddRange([menuProfils, menuConfig, menuInfo]);
        return ms;
    }
}
