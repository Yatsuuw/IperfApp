using IperfApp.Models;
using IperfApp.Services;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    /// <summary>Ouvre la fenêtre de gestion des profils.</summary>
    private void OpenSettings()
    {
        using var settings = new SettingsForm.SettingsForm(this, _config);
        settings.ShowDialog(this);
        RefreshPresetList();
    }

    /// <summary>
    /// Exporte le dernier résultat vers un fichier CSV.
    /// Utilise le snapshot <see cref="_lastPreset"/> capturé au moment du test
    /// pour garantir la cohérence même si l'utilisateur a modifié le profil depuis.
    /// </summary>
    private void HandleSave(bool append)
    {
        if (_lastResult is null || _lastPreset is null)
        {
            MessageBox.Show(this, "Aucun résultat disponible. Lancez d'abord un test.",
                "Aucun résultat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using FileDialog fd = append
            ? new OpenFileDialog { Filter = "Fichier CSV|*.csv", Title = "Choisir le fichier à compléter" }
            : new SaveFileDialog
              {
                  Filter   = "Fichier CSV|*.csv",
                  FileName = $"Debit_Reseau_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv"
              };

        if (fd.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            CsvExporter.Save(fd.FileName, _lastResult, _lastPreset, append);
            MessageBox.Show(this, append ? "Résultat ajouté au fichier." : "Export réussi !",
                "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Erreur lors de l'export : {ex.Message}", "Erreur",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Importe un fichier JSON de configuration après validation stricte.
    /// La lecture du fichier est exécutée hors du thread UI via <see cref="Task.Run"/>
    /// pour éviter tout gel de l'interface sur un disque lent ou réseau.
    /// </summary>
    private async Task ImportConfiguration()
    {
        using var ofd = new OpenFileDialog
        {
            Filter = "Configuration Iperf (*.json)|*.json",
            Title  = "Importer une configuration JSON"
        };

        if (ofd.ShowDialog(this) != DialogResult.OK) return;

        string content;
        try
        {
            // Lecture asynchrone hors thread UI.
            content = await Task.Run(() => File.ReadAllText(ofd.FileName));
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Impossible de lire le fichier :\n{ex.Message}",
                "Échec de l'importation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            MessageBox.Show(this, "Le fichier sélectionné est vide.",
                "Échec de l'importation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!ConfigService.TryParse(content, out ConfigData? imported, out string err))
        {
            MessageBox.Show(this, $"Fichier JSON invalide :\n\n{err}",
                "Échec de l'importation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (MessageBox.Show(this,
                "Le fichier est valide. Remplacer la configuration actuelle ?",
                "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            != DialogResult.Yes) return;

        _config = imported!;
        try
        {
            // Écriture asynchrone hors thread UI.
            await Task.Run(() => ConfigService.Save(_config));
        }
        catch (Exception ex)
        {
            MessageBox.Show(this,
                $"Configuration chargée mais non sauvegardée sur le disque :\n{ex.Message}",
                "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        RefreshPresetList();
        MessageBox.Show(this, "Configuration importée et appliquée !",
            "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Exporte la configuration active vers un fichier JSON.
    /// L'écriture est exécutée hors du thread UI via <see cref="Task.Run"/>
    /// pour éviter tout gel de l'interface sur un disque lent ou réseau.
    /// </summary>
    private async Task ExportConfiguration()
    {
        using var sfd = new SaveFileDialog
        {
            Filter   = "Configuration Iperf (*.json)|*.json",
            FileName = "config_iperf_export.json"
        };

        if (sfd.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            // Écriture asynchrone hors thread UI.
            await Task.Run(() => JsonExporter.SaveToFile(sfd.FileName, _config));
            MessageBox.Show(this, "Exportation terminée !", "Succès",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Erreur d'exportation : {ex.Message}", "Erreur",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
