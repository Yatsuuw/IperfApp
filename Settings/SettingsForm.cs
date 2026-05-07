using IperfApp.Models;

namespace IperfApp.UI;

public partial class SettingsForm : Form
{
  private readonly ConfigData _data;
  private readonly ListBox lstPresets = new();
  private readonly TextBox txtName = new(), txtServer = new(), txtPort = new(), txtChannels = new();
  private readonly ComboBox cbIpVersion = new();
  private readonly Button btnAdd = new(), btnRemove = new(), btnSave = new();
  private readonly Label lblHeader = new();

  public SettingsForm(Form parent, ConfigData data)
  {
    _data = data;

    SetupUI(parent);

    // Sélectionne le profil "Défaut" à l'ouverture pour que le panneau
    // droit soit immédiatement rempli et verrouillé (via LoadSelected).
    string firstProfile = _data.Presets.FirstOrDefault()?.Name ?? "";
    UpdateList(firstProfile);
  }
}
