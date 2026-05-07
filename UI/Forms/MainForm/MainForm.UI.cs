using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    // ---------------------------------------------------------------
    // Constantes de mise en page partagées entre les builders
    // ---------------------------------------------------------------

    private const int CardWidth  = 480;
    private const int CardTop    = 100;
    private const int CardHeight = 295;
    private const int CardBottom = CardTop + CardHeight;  // 395

    /// <summary>Position horizontale gauche de la carte, centrée dans la fenêtre.</summary>
    private int CardLeft => (ClientSize.Width - CardWidth) / 2;

    // ---------------------------------------------------------------
    // Point d'entrée principal
    // ---------------------------------------------------------------

    /// <summary>Configure la fenêtre principale et construit tous les contrôles.</summary>
    private void SetupModernUI()
    {
        Text            = " Speedtest Iperf";
        Size            = new Size(600, 800);
        BackColor       = AppColors.Background;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        StartPosition   = FormStartPosition.CenterScreen;

        // Capture CardLeft une seule fois : la fenêtre est FixedSingle,
        // donc la valeur est stable pour toute la durée de vie de la form.
        int cardLeft = CardLeft;

        var lblTitle = new Label
        {
            Text      = "DÉBIT RÉSEAU",
            Font      = _fonts.Track(new Font("Segoe UI Variable Display", 18F, FontStyle.Bold)),
            ForeColor = AppColors.Accent,
            Location  = new Point(0, 48),
            Size      = new Size(ClientSize.Width, 40),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var menuStrip = BuildMenuStrip();
        var pnlCard   = BuildConfigCard(cardLeft);
        BuildActionsArea(cardLeft);

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
            Font      = _fonts.Track(new Font("Segoe UI Semibold", 9F))
        };
        menuProfils.Click += (_, _) => OpenSettings();

        var menuConfig = new ToolStripMenuItem("Configuration")
        {
            ForeColor = AppColors.TextMuted,
            Font      = _fonts.Track(new Font("Segoe UI", 9F))
        };
        menuConfig.DropDownItems.AddRange([
            new ToolStripMenuItem("Importer une configuration...", null, (_, _) => ImportConfiguration()),
            new ToolStripMenuItem("Exporter une configuration...", null, (_, _) => ExportConfiguration())
        ]);

        var menuInfo = new ToolStripMenuItem("Informations")
        {
            ForeColor = AppColors.TextMuted,
            Font      = _fonts.Track(new Font("Segoe UI", 9F))
        };
        menuInfo.Click += (_, _) => ShowAboutBox();

        ms.Items.AddRange([menuProfils, menuConfig, menuInfo]);
        return ms;
    }
}
