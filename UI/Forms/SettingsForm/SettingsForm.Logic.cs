using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.SettingsForm;

public partial class SettingsForm
{
    // ---------------------------------------------------------------
    // Sélection
    // ---------------------------------------------------------------

    private void LoadPresetIntoFields(Preset p)
    {
        lblHeader.Text            = p.Name;
        txtName.Text              = p.Name;
        txtServer.Text            = p.Server;
        txtPort.Text              = p.Port.ToString();
        txtChannels.Text          = p.Channels.ToString();
        txtDuration.Text          = p.Duration.ToString();
        cbIpVersion.SelectedIndex = p.IpVersion.ToComboIndex();
    }

    // ---------------------------------------------------------------
    // Création
    // ---------------------------------------------------------------

    private void CreateNew()
    {
        var newP = new Preset
        {
            Name     = $"Profil {_data.Presets.Count + 1}",
            Server   = "exemple.iperf.fr",
            Port     = 5201,
            Channels = 4,
            Duration = 10
        };

        _data.Presets.Add(newP);

        try
        {
            ConfigService.Save(_data);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this,
                $"Profil créé en mémoire mais non sauvegardé sur le disque :\n{ex.Message}",
                "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        UpdateList(newP.Name);
    }

    // ---------------------------------------------------------------
    // Suppression
    // ---------------------------------------------------------------

    private void DeleteSelected()
    {
        if (lstPresets.SelectedItem is not Preset p) return;

        if (p.Name == "Défaut")
        {
            MessageBox.Show(this, "Le profil \"Défaut\" ne peut pas être supprimé.",
                "Action impossible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(this,
            $"Supprimer le profil « {p.Name} » ? Cette action est irréversible.",
            "Confirmer la suppression",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes) return;

        _data.Presets.Remove(p);

        // Si le profil supprimé était le profil sélectionné dans la MainForm,
        // basculer sur le premier profil restant.
        if (_data.SelectedPresetName == p.Name)
            _data.SelectedPresetName = _data.Presets.FirstOrDefault()?.Name ?? string.Empty;

        try
        {
            ConfigService.Save(_data);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this,
                $"Profil supprimé en mémoire mais la sauvegarde a échoué :\n{ex.Message}",
                "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Sélectionner le premier profil restant pour éviter un panneau vide.
        UpdateList(_data.Presets.FirstOrDefault()?.Name ?? string.Empty);
    }

    // ---------------------------------------------------------------
    // Sauvegarde
    // ---------------------------------------------------------------

    private async Task SaveDataAsync()
    {
        if (lstPresets.SelectedItem is not Preset current) return;

        string name   = txtName.Text.Trim();
        string server = txtServer.Text.Trim();

        if (!int.TryParse(txtPort.Text, out int port) || port is < 1 or > 65535)
        {
            MessageBox.Show(this, "Port invalide — doit être un entier entre 1 et 65 535.",
                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!int.TryParse(txtChannels.Text, out int channels) || channels is < 1 or > 128)
        {
            MessageBox.Show(this, "Canaux invalides — doit être un entier entre 1 et 128.",
                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!int.TryParse(txtDuration.Text, out int duration) || duration is < 1 or > 120)
        {
            MessageBox.Show(this, "Durée invalide — doit être un entier entre 1 et 120 secondes.",
                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var updated = new Preset
        {
            Name      = name,
            Server    = server,
            Port      = port,
            Channels  = channels,
            Duration  = duration,
            IpVersion = IpVersionExtensions.FromComboIndex(cbIpVersion.SelectedIndex)
        };

        string? validationError = updated.Validate();
        if (validationError is not null)
        {
            MessageBox.Show(this, validationError, "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int index = _data.Presets.IndexOf(current);
        if (index >= 0)
            _data.Presets[index] = updated;

        // Synchroniser SelectedPresetName si le nom a changé.
        if (_data.SelectedPresetName == current.Name)
            _data.SelectedPresetName = updated.Name;

        try
        {
            ConfigService.Save(_data);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Sauvegarde échouée :\n{ex.Message}",
                "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Guard AVANT Task.Delay : inutile de continuer si la fenêtre est déjà fermée.
        if (IsDisposed) return;

        // Désactiver le bouton pendant l'animation pour prévenir tout double-clic
        // qui lancerait deux SaveDataAsync() simultanés pendant les 1 500 ms d'attente.
        btnSave.Enabled   = false;
        var originalColor = btnSave.BackColor;
        var originalText  = btnSave.Text;
        btnSave.Text      = "✓ Enregistré";
        btnSave.BackColor = IperfApp.UI.Constants.AppColors.Success;

        await Task.Delay(1500);

        // Second guard après l'attente asynchrone.
        if (IsDisposed) return;

        // Ordre correct :
        // 1. UpdateList d'abord — peut déclencher OnPresetSelectionChanged → LoadPresetIntoFields.
        // 2. btnSave.Enabled = true ensuite — le bouton n'est réactif qu'une fois la liste stable.
        UpdateList(updated.Name);

        btnSave.Text      = originalText;
        btnSave.BackColor = originalColor;
        btnSave.Enabled   = true;
    }
}
