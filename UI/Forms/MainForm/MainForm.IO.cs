using IperfApp.Models;
using IperfApp.Services;

namespace IperfApp.UI.Forms.MainForm;

public partial class MainForm
{
  private void OpenSettings()
  {
    using var settings = new SettingsForm.SettingsForm(this, _config);
    settings.ShowDialog(this);

    RefreshPresetList();
  }

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
          Filter = "Fichier CSV|*.csv",
          FileName = $"Debit_Reseau_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv"
        };

    if (fd.ShowDialog(this) != DialogResult.OK) return;

    try
    {
      CsvExporter.Save(fd.FileName, _lastResult, _lastPreset, append);
      MessageBox.Show(this, append ? "Résultat ajouté au fichier." : "Export réussi !",
        "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    catch (Exception ex)
    {
      MessageBox.Show(this, $"Erreur lors de l'export : {ex.Message}", "Erreur",
        MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }

  private async Task ImportConfiguration()
  {
    using var ofd = new OpenFileDialog
    {
      Filter = "Configuration Iperf (*.json)|*.json",
      Title = "Importer une configuration JSON"
    };

    if (ofd.ShowDialog(this) != DialogResult.OK) return;

    string content;
    try
    {
      content = await Task.Run(() => File.ReadAllText(ofd.FileName));
    }
    catch (Exception ex)
    {
      MessageBox.Show(this, $"Impossible de lire le fichier :\n{ex.Message}",
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
      MessageBox.Show(this, $"Fichier JSON invalide :\n\n{err}",
        "Échec de l'importation", MessageBoxButtons.OK, MessageBoxIcon.Error);
      return;
    }

    if (MessageBox.Show(this,
      "La configuration actuelle sera remplacée. Continuer ?",
      "Confirmer l'importation",
      MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

    _config = imported!;

    try
    {
      await _saveSemaphore.WaitAsync();
      try { await Task.Run(() => ConfigService.Save(_config)); }
      finally { _saveSemaphore.Release(); }
    }
    catch (Exception ex)
    {
      MessageBox.Show(this,
        $"Configuration importée en mémoire mais non persistante (erreur d'écriture) :\n{ex.Message}",
        "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    RefreshPresetList();

    MessageBox.Show(this, "Configuration importée avec succès.",
      "Import réussi", MessageBoxButtons.OK, MessageBoxIcon.Information);
  }

  private async Task ExportConfiguration()
  {
    using var sfd = new SaveFileDialog
    {
      Filter = "Configuration Iperf (*.json)|*.json",
      Title = "Exporter la configuration JSON",
      FileName = $"iperf_config_{DateTime.Now:yyyyMMdd_HHmmss}.json",
      DefaultExt = "json"
    };

    if (sfd.ShowDialog(this) != DialogResult.OK) return;

    string path = sfd.FileName;

    if (File.Exists(path))
    {
      var confirm = MessageBox.Show(this,
        $"Le fichier \u00ab\u202f{Path.GetFileName(path)}\u202f\u00bb existe déjà.\nVoulez-vous le remplacer ?",
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
        Presets = [.. _config.Presets]
      };

      await _saveSemaphore.WaitAsync();
      try { await Task.Run(() => JsonExporter.SaveToFile(path, copy)); }
      finally { _saveSemaphore.Release(); }

      MessageBox.Show(this, $"Configuration exportée :\n{path}",
        "Export réussi", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    catch (Exception ex)
    {
      MessageBox.Show(this, $"Impossible d'exporter la configuration :\n{ex.Message}",
        "Erreur d'export", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }
}
