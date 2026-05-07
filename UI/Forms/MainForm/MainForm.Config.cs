using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>Charge la configuration depuis le disque et met à jour l'UI.</summary>
    private void LoadConfigIntoUI()
    {
        // _config est déjà chargé dans le constructeur ; on se contente de
        // rafraîchir la liste des profils dans l'interface.
        RefreshPresetList();
    }

    /// <summary>
    /// Repeuple le <see cref="ComboBox"/> des profils et sélectionne le dernier utilisé.
    /// Si la configuration ne contient aucun profil, désactive le bouton de lancement
    /// et affiche un message d'information.
    /// </summary>
    internal void RefreshPresetList()
    {
        if (_config.Presets.Count == 0)
        {
            cbPresets.DataSource = null;
            cbPresets.Items.Clear();
            btnStart.Enabled = false;
            btnStart.Text    = "AUCUN PROFIL CONFIGURÉ";
            MessageBox.Show(
                "Aucun profil n'est configuré.\n\nOuvrez le menu \"Profils\" pour en créer un.",
                "Configuration vide", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Réactive le bouton si des profils sont disponibles
        btnStart.Enabled = true;
        btnStart.Text    = "LANCER L'ANALYSE";

        cbPresets.SelectedIndexChanged -= CbPresets_SelectedIndexChanged;

        cbPresets.DataSource    = null;
        cbPresets.DataSource    = _config.Presets;
        cbPresets.DisplayMember = "Name";

        var selected =
            _config.Presets.FirstOrDefault(p => p.Name == _config.SelectedPresetName)
            ?? _config.Presets[0];

        cbPresets.SelectedItem = selected;
        ApplyPreset(selected);

        cbPresets.SelectedIndexChanged += CbPresets_SelectedIndexChanged;
    }

    private void CbPresets_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cbPresets.SelectedItem is Preset p)
        {
            _config.SelectedPresetName = p.Name;
            try
            {
                ConfigService.Save(_config);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[MainForm] Échec sauvegarde sélection profil : {ex.Message}");
            }
            ApplyPreset(p);
        }
    }

    private void ApplyPreset(Preset p)
    {
        txtServer.Text            = p.Server;
        txtPort.Text              = p.Port.ToString();
        txtChannels.Text          = p.Channels.ToString();
        cbIpVersion.SelectedIndex = IpVersionExtensions.ToComboIndex(p.IpVersion);
    }
}
