using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

/// <summary>Fenêtre principale de l'application Speedtest Iperf.</summary>
public partial class MainForm : Form
{
    private TestResult? _lastResult;
    private Preset?     _lastPreset;

    private readonly IperfEngine _engine = new();
    private CancellationTokenSource? _testCts;
    private ConfigData _config;

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

    /// <summary>
    /// Fontes allouées inline dans BuildActionsArea, BuildConfigCard et
    /// CreateGhostButton. Disposées dans <see cref="Dispose(bool)"/>.
    /// </summary>
    private readonly List<Font> _trackedFonts = [];

    public MainForm()
    {
        InitializeComponent();
        _config = ConfigService.Load();

        string iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "favicon.ico");
        if (File.Exists(iconPath))
        {
            var oldIcon = Icon;
            Icon = new Icon(iconPath);
            oldIcon?.Dispose();
        }

        _engine.OnLogReceived += msg =>
        {
            if (IsDisposed || !IsHandleCreated) return;
            if (txtLog.InvokeRequired)
                txtLog.Invoke(() => AppendLog(msg));
            else
                AppendLog(msg);
        };

        SetupModernUI();
    }

    private void AppendLog(string msg)
    {
        txtLog.AppendText($" {msg}{Environment.NewLine}");
        txtLog.SelectionStart = txtLog.Text.Length;
        txtLog.ScrollToCaret();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _testCts?.Cancel();
        base.OnFormClosing(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _testCts?.Dispose();
            _mainToolTip.Dispose();
            _engine.Dispose();

            foreach (var f in _trackedFonts)
                f.Dispose();
            _trackedFonts.Clear();
        }
        base.Dispose(disposing);
    }
}
