using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.MainForm;

/// <summary>Fenêtre principale de l'application Speedtest Iperf.</summary>
public partial class MainForm : Form
{
    // --- Résultats du dernier test ---
    private TestResult? _lastResult;
    private Preset?     _lastPreset;

    // --- Services ---
    private readonly IperfEngine _engine      = new();
    private CancellationTokenSource? _testCts;
    private ConfigData _config;

    // --- Contrôles UI (initialisés dans SetupModernUI) ---
    private TextBox  txtServer       = null!;
    private TextBox  txtPort         = null!;
    private TextBox  txtChannels     = null!;
    private TextBox  txtLog          = null!;
    private Button   btnStart        = null!;
    private Button   btnCancel       = null!;
    private Button   btnExportNew    = null!;
    private Button   btnExportAppend = null!;
    private ComboBox cbPresets       = null!;
    private ComboBox cbIpVersion     = null!;

    // --- Ressources libérables ---
    private readonly ToolTip    _mainToolTip = new();
    private readonly FontTracker _fonts      = new();

    public MainForm()
    {
        InitializeComponent();
        _config = ConfigService.Load();

        LoadApplicationIcon();

        _engine.OnLogReceived += OnEngineLog;

        SetupModernUI();
    }

    // ---------------------------------------------------------------
    // Gestion de l'icône
    // ---------------------------------------------------------------

    private void LoadApplicationIcon()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "favicon.ico");
        if (!File.Exists(iconPath)) return;
        try
        {
            var old = Icon;
            Icon = new Icon(iconPath);
            old?.Dispose();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[MainForm] Impossible de charger l'icône : {ex.Message}");
        }
    }

    // ---------------------------------------------------------------
    // Réception des logs moteur → UI thread
    // ---------------------------------------------------------------

    private void OnEngineLog(string msg)
    {
        if (IsDisposed || !IsHandleCreated) return;
        if (InvokeRequired)
            BeginInvoke(() => AppendLog(msg));
        else
            AppendLog(msg);
    }

    private void AppendLog(string msg)
    {
        txtLog.AppendText($" {msg}{Environment.NewLine}");
        txtLog.SelectionStart = txtLog.Text.Length;
        txtLog.ScrollToCaret();
    }

    // ---------------------------------------------------------------
    // Cycle de vie
    // ---------------------------------------------------------------

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _testCts?.Cancel();
        base.OnFormClosing(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 1. Annule tout test en cours avant de tuer le moteur
            _testCts?.Cancel();
            _testCts?.Dispose();
            _testCts = null;

            // 2. Détache le handler AVANT de disposer le moteur
            _engine.OnLogReceived -= OnEngineLog;
            _engine.Dispose();

            // 3. Libère les ressources UI
            _mainToolTip.Dispose();
            _fonts.Dispose();
        }
        base.Dispose(disposing);
    }
}
