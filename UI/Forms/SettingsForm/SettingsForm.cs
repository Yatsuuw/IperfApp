using IperfApp.Models;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.SettingsForm;

public partial class SettingsForm : Form
{
  private readonly ConfigData _data;
  private readonly FontTracker _fonts = new();

  private readonly ListBox lstPresets = new();
  private readonly TextBox txtName = new(),
    txtServer = new(),
    txtPort = new(),
    txtChannels = new(),
    txtDuration = new();
  private readonly ComboBox cbIpVersion = new();
  private readonly Button   btnAdd = new(),
    btnRemove = new(),
    btnSave = new();
  private readonly Label lblHeader = new();

  public SettingsForm(Form parent, ConfigData data)
  {
    _data = data;
    SetupUI(parent);

    string firstProfile = _data.Presets.FirstOrDefault()?.Name ?? string.Empty;
    UpdateList(firstProfile);
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
      _fonts.Dispose();
    base.Dispose(disposing);
  }
}
