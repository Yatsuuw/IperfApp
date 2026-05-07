using IperfApp.Models;
using IperfApp.Services;
using IperfApp.UI.Constants;
using IperfApp.UI.Helpers;

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
            MessageBox.Show("Le port doit être un entier compris entre 1 et 65 535.", "Validation",
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

        p.Name      = name;
        p.Server    = server;
        p.Port      = port;
        p.Channels  = channels;
        p.IpVersion = IpVersionExtensions.FromComboIndex(cbIpVersion.SelectedIndex);

        try
        {
            ConfigService.Save(_data);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erreur lors de la sauvegarde : {ex.Message}",
                "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        int currentIndex = lstPresets.SelectedIndex;
        UpdateList();
        if (currentIndex >= 0 && currentIndex < lstPresets.Items.Count)
            lstPresets.SelectedIndex = currentIndex;

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

        try
        {
            ConfigService.Save(_data);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[SettingsForm] Échec sauvegarde après création : {ex.Message}");
        }

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
        if (lstPresets.SelectedItem is not Preset p || p.Name == "Défaut")
            return;

        var answer = MessageBox.Show(
            $"Supprimer le profil « {p.Name} » ?\n\nCette action est irréversible.",
            "Confirmer la suppression",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (answer != DialogResult.Yes)
            return;

        _data.Presets.Remove(p);

        try
        {
            ConfigService.Save(_data);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Le profil a été supprimé de la mémoire mais n'a pas pu être persisté :\n{ex.Message}",
                "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        UpdateList();
        SetLockedState(true);
        lblHeader.Text  = "Profil";
        btnSave.Enabled = false;
    }
}
