using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm : Form
{
  private bool _testRunning;
  private TestResult? _lastResult;
  private Preset? _lastPreset;

  private readonly IperfEngine _engine = new();
  private CancellationTokenSource? _testCts;
  private ConfigData _config;

  private TextBox txtServer = null!;
  private TextBox txtPort = null!;
  private TextBox txtChannels = null!;
  private TextBox txtLog = null!;
  private Button btnStart = null!;
  private Button btnCancel = null!;
  private Button btnExportNew = null!;
  private Button btnExportAppend = null!;
  private ComboBox cbPresets = null!;
  private ComboBox cbIpVersion = null!;

  private readonly ToolTip     _mainToolTip = new();
  private readonly FontTracker _fonts       = new();

  public MainForm()
  {
    InitializeComponent();
    _config = ConfigService.Load();

    LoadApplicationIcon();

    _engine.OnLogReceived += OnEngineLog;

    SetupModernUI();
  }

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
      Debug.WriteLine($"[MainForm] Impossible de charger l'icône : {ex.Message}");
    }
  }

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

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      _testCts?.Cancel();
      _testCts?.Dispose();
      _testCts = null;

      _engine.OnLogReceived -= OnEngineLog;
      _engine.Dispose();

      _mainToolTip.Dispose();
      _fonts.Dispose();
    }
    base.Dispose(disposing);
  }
}
