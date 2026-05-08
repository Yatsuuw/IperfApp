using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>
    /// Repeuple le <see cref="ComboBox"/> des profils et sélectionne le dernier utilisé.
    /// N'écrase pas l'état du bouton Start si un test est en cours (<see cref="_testRunning"/>).
    /// </summary>
    internal void RefreshPresetList()
    {
        if (_config.Presets.Count == 0)
        {
            cbPresets.DataSource = null;
            cbPresets.Items.Clear();

            if (!_testRunning)
            {
                btnStart.Enabled = false;
                btnStart.Text    = "AUCUN PROFIL CONFIGURÉ";
            }

            MessageBox.Show(this,
                "Aucun profil n'est configuré.\n\nOuvrez le menu \"Profils\" pour en créer un.",
                "Configuration vide", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!_testRunning)
        {
            btnStart.Enabled = true;
            btnStart.Text    = "LANCER L'ANALYSE";
        }

        cbPresets.SelectedIndexChanged -= CbPresets_SelectedIndexChanged;
        cbPresets.DataSource    = null;
        cbPresets.DataSource    = _config.Presets;
        cbPresets.DisplayMember = nameof(Preset.Name);

        var selected =
            _config.Presets.FirstOrDefault(p => p.Name == _config.SelectedPresetName)
            ?? _config.Presets[0];

        cbPresets.SelectedItem = selected;
        ApplyPreset(selected);
        cbPresets.SelectedIndexChanged += CbPresets_SelectedIndexChanged;
    }

    private void CbPresets_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cbPresets.SelectedItem is not Preset p) return;

        _config.SelectedPresetName = p.Name;

        // Sauvegarde hors thread UI : fire-and-forget avec catch intégré.
        // ApplyPreset est appelé immédiatement pour que l'UI reste réactive.
        _ = Task.Run(() =>
        {
            try   { ConfigService.Save(_config); }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MainForm] Échec sauvegarde sélection profil : {ex.Message}");
            }
        });

        ApplyPreset(p);
    }

    /// <summary>Remplit les champs UI depuis un profil.</summary>
    private void ApplyPreset(Preset p)
    {
        txtServer.Text            = p.Server;
        txtPort.Text              = p.Port.ToString();
        txtChannels.Text          = p.Channels.ToString();
        cbIpVersion.SelectedIndex = p.IpVersion.ToComboIndex();
    }
}
