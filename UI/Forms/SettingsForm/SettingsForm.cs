using IperfApp.Models;

namespace IperfApp.UI.Forms.SettingsForm;

/// <summary>Fenêtre de gestion des profils iperf3.</summary>
public partial class SettingsForm : Form
{
    private readonly ConfigData _data;
    private readonly ListBox  lstPresets  = new();
    private readonly TextBox  txtName     = new(),
                              txtServer   = new(),
                              txtPort     = new(),
                              txtChannels = new();
    private readonly ComboBox cbIpVersion = new();
    private readonly Button   btnAdd      = new(),
                              btnRemove   = new(),
                              btnSave     = new();
    private readonly Label    lblHeader   = new();

    public SettingsForm(Form parent, ConfigData data)
    {
        _data = data;
        SetupUI(parent);

        // Sélectionne le premier profil à l'ouverture pour que le panneau
        // droit soit immédiatement rempli.
        string firstProfile = _data.Presets.FirstOrDefault()?.Name ?? string.Empty;
        UpdateList(firstProfile);
    }
}
