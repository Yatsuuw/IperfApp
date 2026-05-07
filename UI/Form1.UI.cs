namespace IperfApp.UI;

public partial class Form1 : Form
{
  // ---------------------------------------------------------------
  // Constantes de mise en page partagées entre les builders
  // ---------------------------------------------------------------

  /// <summary>Largeur de la carte de configuration centrale (px).</summary>
  private const int CardWidth = 460;

  /// <summary>Position X calculée pour centrer la carte dans la fenêtre.</summary>
  private int CardLeft => (ClientSize.Width - CardWidth) / 2;

  /// <summary>Position Y du haut de la carte.</summary>
  private const int CardTop    = 95;

  /// <summary>Hauteur de la carte (agrandie pour le champ Durée).</summary>
  private const int CardHeight = 297;

  /// <summary>Position Y du bas de la carte.</summary>
  private const int CardBottom = CardTop + CardHeight;   // 392

  // ---------------------------------------------------------------
  // Point d'entrée principal
  // ---------------------------------------------------------------

  /// <summary>Configure la fenêtre principale et construit tous les contrôles.</summary>
  private void SetupModernUI()
  {
    // --- Fenêtre ---
    Text            = " Speedtest Iperf";
    Size            = new Size(580, 980);
    BackColor       = _colorBackground;
    FormBorderStyle = FormBorderStyle.FixedSingle;
    MaximizeBox     = false;
    StartPosition   = FormStartPosition.CenterScreen;

    // --- Titre ---
    var lblTitle = new Label
    {
      Text      = "DÉBIT RÉSEAU",
      Font      = new Font("Segoe UI Variable Display", 16F, FontStyle.Bold),
      ForeColor = _colorAccent,
      Location  = new Point(0, 45),
      Size      = new Size(ClientSize.Width, 35),
      TextAlign = ContentAlignment.MiddleCenter
    };

    // --- Sections ---
    var menuStrip = BuildMenuStrip();
    var pnlCard   = BuildConfigCard();
    BuildActionsArea();
    BuildHistoryArea();

    // --- Infobulles ---
    _mainToolTip.SetToolTip(cbPresets,   "Sélectionnez un profil pré-enregistré.");
    _mainToolTip.SetToolTip(txtServer,   "Adresse IP ou nom d'hôte du serveur Iperf3.");
    _mainToolTip.SetToolTip(txtPort,     "Port de destination (souvent 5201 ou 9240).");
    _mainToolTip.SetToolTip(txtChannels, "Nombre de flux TCP parallèles (recommandé : 8).");
    _mainToolTip.SetToolTip(btnCancel,   "Annule le test en cours.");

    // --- Ajout des contrôles à la fenêtre ---
    MainMenuStrip = menuStrip;
    Controls.Add(menuStrip);
    Controls.AddRange([lblTitle, pnlCard, btnStart, btnCancel,
                       txtLog, btnExportNew, btnExportAppend,
                       _lvHistory, _chart]);

    // --- Chargement de la config (une seule fois) ---
    LoadConfigIntoUI();
  }

  // ---------------------------------------------------------------
  // Builder : barre de menus
  // ---------------------------------------------------------------

  /// <summary>Construit et retourne le <see cref="MenuStrip"/> principal.</summary>
  private MenuStrip BuildMenuStrip()
  {
    var ms = new MenuStrip
    {
      BackColor = _colorCard,
      Padding   = new Padding(6, 4, 0, 4)
    };

    var menuProfils = new ToolStripMenuItem("Profils")
    {
      ForeColor = _colorAccent,
      Font      = new Font("Segoe UI Semibold", 9F)
    };
    menuProfils.Click += (_, _) => OpenSettings();

    var menuConfig = new ToolStripMenuItem("Configuration")
    {
      ForeColor = Color.DimGray,
      Font      = new Font("Segoe UI", 9F)
    };
    menuConfig.DropDownItems.AddRange([
      new ToolStripMenuItem("Importer une configuration...", null, (_, _) => ImportConfiguration()),
      new ToolStripMenuItem("Exporter une configuration...", null, (_, _) => ExportConfiguration())
    ]);

    var menuInfo = new ToolStripMenuItem("Informations")
    {
      ForeColor = Color.DimGray,
      Font      = new Font("Segoe UI", 9F)
    };
    menuInfo.Click += (_, _) => ShowAboutBox();

    ms.Items.AddRange([menuProfils, menuConfig, menuInfo]);
    return ms;
  }
}
