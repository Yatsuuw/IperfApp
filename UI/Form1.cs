using IperfApp.Services;

namespace IperfApp.UI;

public partial class Form1 : Form
{
  private ConfigData _config = null!; // Cette ligne manquait !
  private double _lastUp = 0;
  private double _lastDown = 0;
  private readonly IperfEngine _engine = new();

  // Variables d'interface
  private TextBox txtServer = null!, txtPort = null!, txtChannels = null!, txtLog = null!;
  private Button btnStart = null!, btnExportNew = null!, btnExportAppend = null!;
  private ComboBox cbPresets = null!; 
  private readonly ToolTip _mainToolTip = new();

  // Couleurs
  private readonly Color _colorBackground = Color.FromArgb(240, 243, 247);
  private readonly Color _colorCard = Color.White;
  private readonly Color _colorAccent = Color.FromArgb(0, 120, 215);
  private readonly Color _colorTerminal = Color.FromArgb(28, 28, 30);

  // Constructeur
  public Form1()
  {
    InitializeComponent();
    
    string iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "favicon.ico");
    if (File.Exists(iconPath)) Icon = new Icon(iconPath);

    // Événement de log Iperf
    _engine.OnLogReceived += (msg) => Invoke(() =>
    { 
      txtLog.AppendText($" {msg}{Environment.NewLine}"); 
      txtLog.SelectionStart = txtLog.Text.Length;
      txtLog.ScrollToCaret();
    });

    // Appels aux méthodes situées dans les autres fichiers partial
    SetupModernUI();      
    LoadConfigIntoUI();   
  }
}