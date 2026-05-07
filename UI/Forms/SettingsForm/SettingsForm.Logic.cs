using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Constants;

namespace IperfApp.UI.Forms.SettingsForm;

public partial class SettingsForm
{
    private void LoadSelected()
    {
        if (lstPresets.SelectedItem is Preset p)
        {
            txtName.Text     = p.Name;
            txtServer.Text   = p.Server;
            txtPort.Text     = p.Port.ToString();
            txtChannels.Text = p.Channels.ToString();
            cbIpVersion.SelectedIndex = IpVersionExtensions.ToComboIndex(p.IpVersion);

            bool isDefault = p.Name == "Défaut";
            SetLockedState(isDefault);
            lblHeader.Text    = isDefault ? "Profil Système 🔒" : "Modification";
            btnRemove.Enabled = !isDefault;
            btnSave.Enabled   = !isDefault;
        }
        else
        {
            SetLockedState(true);
            btnSave.Enabled = false;
        }
    }

    /// <summary>Valide et enregistre le profil sélectionné.</summary>
    private async Task SaveDataAsync()
    {
        if (lstPresets.SelectedItem is not Preset p || p.Name == "Défaut") return;

        // --- Validation ---
        string name = txtName.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Le nom du scénario ne peut pas être vide.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtName.Focus();
            return;
        }

        string server = txtServer.Text.Trim();
        if (string.IsNullOrWhiteSpace(server))
        {
            MessageBox.Show("L'adresse du serveur ne peut pas être vide.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtServer.Focus();
            return;
        }

        if (!int.TryParse(txtPort.Text, out int port) || port < 1 || port > 65535)
        {
            MessageBox.Show("Le port doit être un entier compris entre 1 et 65 535.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPort.Focus();
            return;
        }

        if (!int.TryParse(txtChannels.Text, out int channels) || channels < 1 || channels > 128)
        {
            MessageBox.Show("Le nombre de canaux doit être compris entre 1 et 128.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtChannels.Focus();
            return;
        }

        // --- Mise à jour du modèle ---
        p.Name      = name;
        p.Server    = server;
        p.Port      = port;
        p.Channels  = channels;
        p.IpVersion = IpVersionExtensions.FromComboIndex(cbIpVersion.SelectedIndex);

        // --- Sauvegarde avec gestion d'erreur explicite ---
        try
        {
            ConfigService.Save(_data);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erreur lors de la sauvegarde : {ex.Message}",
                "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        int currentIndex = lstPresets.SelectedIndex;
        UpdateList();
        if (currentIndex >= 0 && currentIndex < lstPresets.Items.Count)
            lstPresets.SelectedIndex = currentIndex;

        // --- Feedback visuel ---
        string oldTxt = btnSave.Text;
        Color  oldCol = btnSave.BackColor;
        btnSave.Text      = "✓ ENREGISTRÉ";
        btnSave.BackColor = AppColors.Success;
        await Task.Delay(1000);
        btnSave.Text      = oldTxt;
        btnSave.BackColor = oldCol;
    }

    private void CreateNew()
    {
        var newP = new Preset
        {
            Name      = "Nouveau profil",
            Server    = string.Empty,
            Port      = 5201,
            Channels  = 8,
            IpVersion = IpVersion.Auto
        };
        _data.Presets.Add(newP);
        UpdateList(newP.Name);
        SetLockedState(false);
        lblHeader.Text    = "Modification";
        btnRemove.Enabled = true;
        btnSave.Enabled   = true;
        txtName.Focus();
        txtName.SelectAll();
    }

    private void DeleteSelected()
    {
        if (lstPresets.SelectedItem is Preset p && p.Name != "Défaut")
        {
            _data.Presets.Remove(p);
            UpdateList();
            SetLockedState(true);
            lblHeader.Text  = "Profil";
            btnSave.Enabled = false;
        }
    }
}
