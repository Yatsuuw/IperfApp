using IperfApp.Models;
using IperfApp.Services;

namespace IperfApp.UI;

/// <summary>Fenêtre principale de l'application Speedtest Iperf.</summary>
public partial class Form1 : Form
{
  // --- Résultats du dernier test ---
  private TestResult? _lastResult;

  // --- Moteur iperf3 ---
  private readonly IperfEngine _engine = new();

  // --- Annulation du test en cours ---
  private CancellationTokenSource? _testCts;

  // --- Contrôles UI déclarés ici pour accès depuis les partial ---
  private TextBox   txtServer       = null!;
  private TextBox   txtPort         = null!;
  private TextBox   txtChannels     = null!;
  private TextBox   txtLog          = null!;
  private Button    btnStart        = null!;
  private Button    btnCancel       = null!;
  private Button    btnExportNew    = null!;
  private Button    btnExportAppend = null!;
  private ComboBox  cbPresets       = null!;
  private ComboBox  cbIpVersion     = null!;
  private readonly ToolTip _mainToolTip = new();

  // --- Palette de couleurs (source unique de vérité) ---
  private readonly Color _colorBackground    = Color.FromArgb(240, 243, 247);
  private readonly Color _colorCard          = Color.White;
  private readonly Color _colorAccent        = Color.FromArgb(0,   120, 215);
  private readonly Color _colorAccentDisabled = Color.FromArgb(160, 174, 192); // accent grisé pendant le test
  private readonly Color _colorDanger        = Color.FromArgb(196,  43,  28);
  private readonly Color _colorTerminal      = Color.FromArgb(28,   28,  30);

  /// <summary>Initialise la fenêtre principale.</summary>
  public Form1()
  {
    InitializeComponent();

    // Icône de la fenêtre
    string iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "favicon.ico");
    if (File.Exists(iconPath))
      Icon = new Icon(iconPath);

    // Abonnement aux logs du moteur iperf3
    _engine.OnLogReceived += msg => Invoke(() =>
    {
      txtLog.AppendText($" {msg}{Environment.NewLine}");
      txtLog.SelectionStart = txtLog.Text.Length;
      txtLog.ScrollToCaret();
    });

    // Construction de l'UI (charge aussi la config via LoadConfigIntoUI)
    SetupModernUI();
  }
}
