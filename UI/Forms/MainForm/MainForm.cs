using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

/// <summary>Fenêtre principale de l'application Speedtest Iperf.</summary>
public partial class MainForm : Form
{
    // --- Résultat du dernier test ---
    private TestResult? _lastResult;

    // --- Moteur iperf3 ---
    private readonly IperfEngine _engine = new();

    // --- Annulation du test en cours ---
    private CancellationTokenSource? _testCts;

    // --- Configuration active (jamais null après le constructeur) ---
    private ConfigData _config;

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

    /// <summary>Initialise la fenêtre principale.</summary>
    public MainForm()
    {
        InitializeComponent();

        // Chargement anticipé de la config (jamais null)
        _config = ConfigService.Load();

        // Icône de la fenêtre (dispose correctement l'ancienne avant remplacement)
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "favicon.ico");
        if (File.Exists(iconPath))
        {
            var oldIcon = Icon;
            Icon = new Icon(iconPath);
            oldIcon?.Dispose();
        }

        // Abonnement aux logs du moteur iperf3 avec InvokeRequired
        _engine.OnLogReceived += msg =>
        {
            if (txtLog.IsDisposed) return;
            if (txtLog.InvokeRequired)
                txtLog.Invoke(() => AppendLog(msg));
            else
                AppendLog(msg);
        };

        // Construction de l'UI puis chargement de la config dans les contrôles
        SetupModernUI();
    }

    /// <summary>Ajoute une ligne dans la console de logs.</summary>
    private void AppendLog(string msg)
    {
        txtLog.AppendText($" {msg}{Environment.NewLine}");
        txtLog.SelectionStart = txtLog.Text.Length;
        txtLog.ScrollToCaret();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _testCts?.Dispose();
            _mainToolTip.Dispose();
        }
        base.Dispose(disposing);
    }
}
