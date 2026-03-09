using IperfApp.Services;

namespace IperfApp.UI;

public partial class SettingsForm : Form
{
  private readonly ConfigData _data;
  private readonly ListBox lstPresets = new();
  private readonly TextBox txtName = new(), txtServer = new(), txtPort = new(), txtChannels = new();
  private readonly Button btnAdd = new(), btnRemove = new(), btnSave = new();
  private readonly Label lblHeader = new();

  public SettingsForm(Form parent, ConfigData data)
  {
    _data = data;

    // Initialisation de la structure et de la logique
    SetupUI(parent);
    UpdateList();
    SetLockedState(true);
  }
}