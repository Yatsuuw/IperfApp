using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Helpers;

namespace IperfApp.UI.Forms.SettingsForm;

public partial class SettingsForm
{
    // ---------------------------------------------------------------
    // Sélection
    // ---------------------------------------------------------------

    private void OnPresetSelectionChanged(object? sender, EventArgs e)
    {
        if (lstPresets.SelectedItem is Preset p)
            LoadPresetIntoFields(p);
    }

    private void LoadPresetIntoFields(Preset p)
    {
        lblHeader.Text         = p.Name;
        txtName.Text           = p.Name;
        txtServer.Text         = p.Server;
        txtPort.Text           = p.Port.ToString();
        txtChannels.Text       = p.Channels.ToString();
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
            MessageBox.Show(
                $"Profil créé en mémoire mais non sauvegardé sur le disque :\n{ex.Message}",
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
            MessageBox.Show("Le profil \"Défaut\" ne peut pas être supprimé.",
                "Action impossible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            $"Supprimer le profil \u00ab {p.Name} \u00bb ? Cette action est irréversible.",
            "Confirmer la suppression",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes) return;

        _data.Presets.Remove(p);

        try
        {
            ConfigService.Save(_data);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Profil supprimé en mémoire mais la sauvegarde a échoué :\n{ex.Message}",
                "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        UpdateList();
    }

    // ---------------------------------------------------------------
    // Sauvegarde
    // ---------------------------------------------------------------

    private async Task SaveDataAsync()
    {
        if (lstPresets.SelectedItem is not Preset current) return;

        // --- Lecture et validation des champs ---
        string name   = txtName.Text.Trim();
        string server = txtServer.Text.Trim();

        if (!int.TryParse(txtPort.Text,     out int port)     || port     is < 1 or > 65535)
        {
            MessageBox.Show("Port invalide — doit être un entier entre 1 et 65 535.",
                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!int.TryParse(txtChannels.Text, out int channels) || channels is < 1 or > 128)
        {
            MessageBox.Show("Canaux invalides — doit être un entier entre 1 et 128.",
                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var updated = new Preset
        {
            Name      = name,
            Server    = server,
            Port      = port,
            Channels  = channels,
            Duration  = current.Duration > 0 ? current.Duration : 10,
            IpVersion = IpVersionExtensions.FromComboIndex(cbIpVersion.SelectedIndex)
        };

        string? validationError = updated.Validate();
        if (validationError is not null)
        {
            MessageBox.Show(validationError, "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // --- Mise à jour du modèle ---
        int index = _data.Presets.IndexOf(current);
        if (index >= 0)
            _data.Presets[index] = updated;

        // --- Sauvegarde ---
        try
        {
            ConfigService.Save(_data);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Sauvegarde échouée :\n{ex.Message}",
                "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // --- Feedback visuel ---
        var originalColor = btnSave.BackColor;
        btnSave.Text      = "✓ Enregistré";
        btnSave.BackColor = IperfApp.UI.Constants.AppColors.Success;
        await Task.Delay(1500);
        btnSave.Text      = "ENREGISTRER";
        btnSave.BackColor = originalColor;

        UpdateList(updated.Name);
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    private void UpdateList(string? selectName = null)
    {
        lstPresets.SelectedIndexChanged -= OnPresetSelectionChanged;
        lstPresets.DataSource = null;
        lstPresets.DataSource = _data.Presets;
        lstPresets.DisplayMember = nameof(Preset.Name);

        if (selectName is not null)
        {
            var target = _data.Presets.FirstOrDefault(p => p.Name == selectName);
            if (target is not null) lstPresets.SelectedItem = target;
        }

        lstPresets.SelectedIndexChanged += OnPresetSelectionChanged;
        lstPresets.Refresh();
    }
}
