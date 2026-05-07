using IperfApp.Models;
using IperfApp.Services;

namespace IperfApp.UI;

public partial class Form1
{
  /// <summary>Ouvre la fenêtre de gestion des profils.</summary>
  private void OpenSettings()
  {
    using var settings = new SettingsForm(this, _config);
    settings.ShowDialog();
    RefreshPresetList();
  }

  /// <summary>Exporte le dernier résultat vers un fichier CSV.</summary>
  /// <param name="append">Si <c>true</c>, ajoute au fichier existant ; sinon crée un nouveau.</param>
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
        ConfigService.Save(_config);
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
      // Délègue à JsonExporter — aucun contact avec config.json
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
