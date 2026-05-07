using IperfApp.Models;

namespace IperfApp.UI;

/// <summary>Fenêtre de gestion des profils iperf3.</summary>
public partial class SettingsForm : Form
{
  private readonly ConfigData _data;

  private readonly ListBox  lstPresets  = new();
  private readonly Label    lblHeader   = new();
  private readonly TextBox  txtName     = new();
  private readonly TextBox  txtServer   = new();
  private readonly TextBox  txtPort     = new();
  private readonly TextBox  txtChannels = new();
  private readonly TextBox  txtDuration = new();
  private readonly ComboBox cbIpVersion = new();
  private readonly Button   btnAdd      = new();
  private readonly Button   btnRemove   = new();
  private readonly Button   btnSave     = new();

  public SettingsForm(Form parent, ConfigData data)
  {
    _data = data;
    SetupUI(parent);
    UpdateList(_data.SelectedPresetName);
  }
}
