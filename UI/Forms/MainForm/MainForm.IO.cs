using IperfApp.Models;
using IperfApp.Services;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>Ouvre la fenêtre de gestion des profils.</summary>
    private void OpenSettings()
    {
        using var settings = new SettingsForm.SettingsForm(this, _config);
        settings.ShowDialog();
        RefreshPresetList();
    }

    /// <summary>Exporte le dernier résultat vers un fichier CSV.</summary>
    private void HandleSave(bool append)
    {
        if (_lastResult is null)
        {
            MessageBox.Show("Aucun résultat disponible. Lancez d'abord un test.",
                "Aucun résultat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using FileDialog fd = append ? (FileDialog)new OpenFileDialog() : new SaveFileDialog();
        fd.Filter = "Fichier CSV|*.csv";

        if (!append && fd is SaveFileDialog sfd)
            sfd.FileName = $"Debit_Reseau_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";

        if (fd.ShowDialog() != DialogResult.OK) return;

        try
        {
            CsvExporter.Save(fd.FileName, _lastResult, BuildCurrentPreset(), append);
            MessageBox.Show("Export réussi !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de l'export : {ex.Message}", "Erreur",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>Importe un fichier JSON de configuration après validation stricte.</summary>
    private void ImportConfiguration()
    {
        using var ofd = new OpenFileDialog
        {
            Filter = "Configuration Iperf (*.json)|*.json",
            Title  = "Importer une configuration JSON"
        };

        if (ofd.ShowDialog() != DialogResult.OK) return;

        try
        {
            string content = File.ReadAllText(ofd.FileName);

            if (!ConfigService.TryParse(content, out ConfigData? imported, out string err))
            {
                MessageBox.Show($"Fichier JSON invalide :\n\n{err}",
                    "Échec de l'importation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show(
                "Le fichier est valide. Remplacer la configuration actuelle ?",
                "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                _config = imported!;
                try
                {
                    ConfigService.Save(_config);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"La configuration a été chargée mais n'a pas pu être sauvegardée sur le disque :\n{ex.Message}",
                        "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                RefreshPresetList();
                MessageBox.Show("Configuration importée et appliquée !",
                    "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur de lecture : {ex.Message}", "Erreur",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>Exporte la configuration active vers un fichier JSON.</summary>
    private void ExportConfiguration()
    {
        using var sfd = new SaveFileDialog
        {
            Filter   = "Configuration Iperf (*.json)|*.json",
            FileName = "config_iperf_export.json"
        };

        if (sfd.ShowDialog() != DialogResult.OK) return;

        try
        {
            JsonExporter.SaveToFile(sfd.FileName, _config);
            MessageBox.Show("Exportation terminée !", "Succès",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur d'exportation : {ex.Message}", "Erreur",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
