using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>
    /// Verrou pour sérialiser les sauvegardes concurrentes déclenchées par
    /// <see cref="CbPresets_SelectedIndexChanged"/>.
    /// Un changement rapide de profil lance autant de Task.Run que de clics ;
    /// sans ce verrou, plusieurs écritures simultanées sur config.json
    /// pourraient corrompre le fichier.
    /// </summary>
    private readonly SemaphoreSlim _saveSemaphore = new(1, 1);

    /// <summary>
    /// Repeuple le <see cref="ComboBox"/> des profils et sélectionne le dernier utilisé.
    /// N'écrase pas l'état du bouton Start si un test est en cours (<see cref="_testRunning"/>).
    /// </summary>
    /// <param name="warnIfEmpty">
    /// <c>true</c> pour afficher un <see cref="MessageBox"/> si aucun profil n'est configuré.
    /// Passer <c>true</c> uniquement au démarrage de l'application.
    /// Laisser <c>false</c> (défaut) après fermeture de SettingsForm ou après import :
    /// l'état UI est mis à jour silencieusement.
    /// </param>
    internal void RefreshPresetList(bool warnIfEmpty = false)
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

            if (warnIfEmpty)
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

        // Sauvegarde hors thread UI avec verrou pour éviter les écritures concurrentes
        // en cas de changement rapide de profil (plusieurs clics successifs).
        _ = Task.Run(async () =>
        {
            await _saveSemaphore.WaitAsync();
            try   { ConfigService.Save(_config); }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MainForm] Échec sauvegarde sélection profil : {ex.Message}");
            }
            finally { _saveSemaphore.Release(); }
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
