using IperfApp.Models;
using IperfApp.Services;

namespace IperfApp.UI.Forms.SettingsForm;

public partial class SettingsForm
{
    // ---------------------------------------------------------------
    // Commandes liées aux boutons
    // ---------------------------------------------------------------

    /// <summary>Crée un nouveau profil vide, le sélectionne et positionne le focus sur le nom.</summary>
    private void CreateNew()
    {
        var newP = new Preset
        {
            Name     = $"Profil {_data.Presets.Count + 1}",
            Server   = string.Empty,
            Port     = 5201,
            Channels = 4,
            Duration = 10
        };

        _data.Presets.Add(newP);
        UpdateList(newP.Name);
        txtName.Focus();
        txtName.SelectAll();
    }

    /// <summary>
    /// Supprime le profil sélectionné après confirmation de l'utilisateur.
    /// Le profil nommé "Défaut" ne peut pas être supprimé.
    /// </summary>
    private void DeleteSelected()
    {
        if (lstPresets.SelectedItem is not Preset p) return;

        if (p.Name == "Défaut")
        {
            MessageBox.Show(this,
                "Le profil \"Défaut\" ne peut pas être supprimé.",
                "Suppression impossible",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(this,
            $"Supprimer le profil \u00ab\u202f{p.Name}\u202f\u00bb\u00a0?\n\nCette action est irréversible.",
            "Confirmer la suppression",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes) return;

        _data.Presets.Remove(p);
        UpdateList();
    }

    /// <summary>
    /// Valide le profil sélectionné, l'enregistre sur le disque de manière asynchrone,
    /// puis affiche un feedback visuel.
    /// <para>
    /// <see cref="ConfigService.Save"/> est exécuté sur un thread de pool
    /// (<see cref="Task.Run"/>) pour éviter de geler le thread UI sur
    /// un système de fichiers lent (réseau, clé USB).
    /// </para>
    /// </summary>
    private async Task SaveDataAsync()
    {
        if (lstPresets.SelectedItem is not Preset p) return;

        // Lire les champs UI avant tout basculement de contrôle
        p.Name     = txtName.Text.Trim();
        p.Server   = txtServer.Text.Trim();
        _ = int.TryParse(txtPort.Text,     out int port);     p.Port     = port;
        _ = int.TryParse(txtChannels.Text, out int channels); p.Channels = channels;
        _ = int.TryParse(txtDuration.Text, out int duration); p.Duration = duration;
        p.IpVersion = IpVersionExtensions.FromComboIndex(cbIpVersion.SelectedIndex);

        string? error = p.Validate();
        if (error is not null)
        {
            MessageBox.Show(this, error, "Valeurs invalides",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnSave.Enabled = false;
        btnSave.Text    = "Enregistrement...";

        try
        {
            // I/O hors thread UI
            await Task.Run(() => ConfigService.Save(_data));

            btnSave.Text    = "\u2713 Enregistré";
            btnSave.BackColor = Color.FromArgb(40, 167, 69);

            await Task.Delay(1500);
        }
        catch (Exception ex)
        {
            // Afficher l'erreur sur le thread UI
            if (!IsDisposed)
                Invoke(() => MessageBox.Show(this,
                    $"Impossible d'enregistrer la configuration :\n\n{ex.Message}",
                    "Erreur d'enregistrement",
                    MessageBoxButtons.OK, MessageBoxIcon.Error));
        }
        finally
        {
            if (!IsDisposed)
            {
                btnSave.Text      = "Enregistrer";
                btnSave.BackColor = Color.FromArgb(0, 120, 212);
                btnSave.Enabled   = true;
                UpdateList(p.Name);
            }
        }
    }

    // ---------------------------------------------------------------
    // Chargement d'un profil dans les champs
    // ---------------------------------------------------------------

    /// <summary>Peuple les champs de formulaire depuis le profil sélectionné.</summary>
    private void LoadPresetIntoFields(Preset p)
    {
        txtName.Text              = p.Name;
        txtServer.Text            = p.Server;
        txtPort.Text              = p.Port.ToString();
        txtChannels.Text          = p.Channels.ToString();
        txtDuration.Text          = p.Duration.ToString();
        cbIpVersion.SelectedIndex = p.IpVersion.ToComboIndex();
    }
}
