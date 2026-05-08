using IperfApp.Models;
using IperfApp.Services;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
    // ---------------------------------------------------------------
    // Ouverture de la fenêtre Profils
    // ---------------------------------------------------------------

    /// <summary>
    /// Ouvre la fenêtre de gestion des profils de manière modale.
    /// Après fermeture, repeuple la liste sans afficher de MessageBox
    /// (l'utilisateur a délibérément vidé la liste s'il n'y a plus de profils).
    /// </summary>
    private void OpenSettings()
    {
        using var settings = new SettingsForm.SettingsForm(this, _config);
        settings.ShowDialog(this);

        // warnIfEmpty: false — pas de popup après fermeture de SettingsForm
        RefreshPresetList();
    }

    // ---------------------------------------------------------------
    // Export CSV
    // ---------------------------------------------------------------

    /// <summary>
    /// Ouvre une boîte de dialogue de sauvegarde CSV et exporte le dernier résultat.
    /// </summary>
    /// <param name="append">
    /// <c>false</c> pour créer un nouveau fichier,
    /// <c>true</c> pour ajouter les données à un fichier existant.
    /// </param>
    private void HandleSave(bool append)
    {
        if (_lastResult is null || _lastPreset is null) return;

        using var dlg = new SaveFileDialog
        {
            Filter      = "CSV (*.csv)|*.csv",
            DefaultExt  = "csv",
            Title       = append ? "Ajouter au fichier CSV" : "Enregistrer le rapport CSV",
            FileName    = $"iperf_result_{DateTime.Now:yyyyMMdd_HHmmss}"
        };

        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        string path = dlg.FileName;

        if (!append && File.Exists(path))
        {
            var confirm = MessageBox.Show(this,
                $"Le fichier \u00ab\u202f{Path.GetFileName(path)}\u202f\u00bb existe déjà.\nVoulez-vous le remplacer\u00a0?",
                "Confirmer le remplacement",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;
        }

        try
        {
            CsvExporter.Save(path, _lastResult, _lastPreset, append);

            MessageBox.Show(this,
                $"Fichier enregistré :\n{path}",
                "Export réussi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this,
                $"Impossible d'enregistrer le fichier :\n\n{ex.Message}",
                "Erreur d'export", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ---------------------------------------------------------------
    // Import JSON (configuration)
    // ---------------------------------------------------------------

    /// <summary>Importe une configuration depuis un fichier JSON.</summary>
    private void ImportConfig()
    {
        using var dlg = new OpenFileDialog
        {
            Filter     = "JSON (*.json)|*.json",
            Title      = "Importer une configuration",
            DefaultExt = "json"
        };

        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        string json;
        try
        {
            json = File.ReadAllText(dlg.FileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this,
                $"Impossible de lire le fichier :\n\n{ex.Message}",
                "Erreur de lecture", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (!ConfigService.TryParse(json, out ConfigData? data, out string parseError) || data is null)
        {
            MessageBox.Show(this,
                $"Fichier JSON invalide :\n\n{parseError}",
                "Erreur d'import", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _config = data;
        try
        {
            ConfigService.Save(_config);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this,
                $"Configuration importée mais non persistante (erreur d'écriture) :\n\n{ex.Message}",
                "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // warnIfEmpty: false — l'utilisateur vient d'importer, pas besoin de popup
        RefreshPresetList();

        MessageBox.Show(this,
            "Configuration importée avec succès.",
            "Import réussi", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>Exporte la configuration courante vers un fichier JSON.</summary>
    private void ExportConfig()
    {
        using var dlg = new SaveFileDialog
        {
            Filter      = "JSON (*.json)|*.json",
            DefaultExt  = "json",
            Title       = "Exporter la configuration",
            FileName    = $"iperf_config_{DateTime.Now:yyyyMMdd_HHmmss}"
        };

        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        string path = dlg.FileName;

        if (File.Exists(path))
        {
            var confirm = MessageBox.Show(this,
                $"Le fichier \u00ab\u202f{Path.GetFileName(path)}\u202f\u00bb existe déjà.\nVoulez-vous le remplacer\u00a0?",
                "Confirmer le remplacement",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;
        }

        try
        {
            var copy = new ConfigData
            {
                SelectedPresetName = _config.SelectedPresetName,
                Presets            = [.. _config.Presets]
            };
            JsonExporter.SaveToFile(path, copy);

            MessageBox.Show(this,
                $"Configuration exportée :\n{path}",
                "Export réussi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this,
                $"Impossible d'exporter la configuration :\n\n{ex.Message}",
                "Erreur d'export", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
