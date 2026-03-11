using IperfApp.Services;

namespace IperfApp.UI;

public partial class Form1 : Form
{
  // Variables privées
  private TextBox txtServer = null!, txtPort = null!, txtChannels = null!, txtLog = null!;
  private Button btnStart = null!, btnExportNew = null!, btnExportAppend = null!;
  private readonly ToolTip _mainToolTip = new();
  private double _lastUp = 0;
  private double _lastDown = 0;
  private readonly IperfEngine _engine = new();


  // Couleurs
  private readonly Color _colorBackground = Color.FromArgb(240, 243, 247);
  private readonly Color _colorCard = Color.White;
  private readonly Color _colorAccent = Color.FromArgb(0, 120, 215);
  private readonly Color _colorTerminal = Color.FromArgb(28, 28, 30);

  // Constructeur
  public Form1()
  {
    InitializeComponent();
    Icon = new Icon(Path.Combine(AppContext.BaseDirectory, "Resources", "favicon.ico"));

    // Événement de log Iperf
    _engine.OnLogReceived += (msg) => Invoke(() =>
    { 
      txtLog.AppendText($" {msg}{Environment.NewLine}"); 
      txtLog.SelectionStart = txtLog.Text.Length;
      txtLog.ScrollToCaret();
    });

    // Initialiser l'interface utilisateur
    SetupModernUI();
  }
}