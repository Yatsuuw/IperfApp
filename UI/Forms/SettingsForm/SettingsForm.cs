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

    /// <summary>
    /// Libère les ressources GDI non gérées (objets <see cref="Font"/>)
    /// allouées dans <c>SetupUI</c> et les helpers.
    /// WinForms ne dispose pas automatiquement les fonts affectées
    /// aux propriétés <c>.Font</c> lorsqu'elles ont été créées en dehors
    /// du Designer généré — il faut les libérer explicitement.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            lblHeader.Font?.Dispose();
            btnSave.Font?.Dispose();
            btnAdd.Font?.Dispose();
            btnRemove.Font?.Dispose();
            lstPresets.Font?.Dispose();

            foreach (var tb in new[] { txtName, txtServer, txtPort, txtChannels })
                tb.Font?.Dispose();

            cbIpVersion.Font?.Dispose();
        }
        base.Dispose(disposing);
    }
}
