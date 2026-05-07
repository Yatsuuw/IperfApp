using IperfApp.Models;

namespace IperfApp.UI.Forms.SettingsForm;

/// <summary>Fenêtre de gestion des profils iperf3.</summary>
public partial class SettingsForm : Form
{
    private readonly ConfigData _data;

    // Contrôles UI
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

    /// <summary>
    /// Liste de toutes les <see cref="Font"/> allouées inline dans les helpers
    /// (AddInputField, AddNumericField, AddComboField, ConfigureSideButton,
    ///  propriétés directes). Disposées toutes dans <see cref="Dispose(bool)"/>.
    /// </summary>
    private readonly List<Font> _trackedFonts = [];

    public SettingsForm(Form parent, ConfigData data)
    {
        _data = data;
        SetupUI(parent);

        string firstProfile = _data.Presets.FirstOrDefault()?.Name ?? string.Empty;
        UpdateList(firstProfile);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var f in _trackedFonts)
                f.Dispose();
            _trackedFonts.Clear();
        }
        base.Dispose(disposing);
    }
}
