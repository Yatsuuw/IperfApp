using IperfApp.Services;

namespace IperfApp.UI;

public partial class Form1
{
  // Gestion des paramètres et de l'export
  private void OpenSettings()
  {
    // Libérer la mémoire après fermeture
    using var settings = new SettingsForm(this, _config);
    // Affichage de la fenêtre
    settings.ShowDialog();
    // Rafraîchissement systématique du menu déroulant
    RefreshPresetList();
  }

  private void HandleSave(bool append)
  {
    using FileDialog fd = append ? new OpenFileDialog() : new SaveFileDialog();
    fd.Filter = "Fichier CSV|*.csv";
    if (fd.ShowDialog() == DialogResult.OK)
    {
      try {
        CsvExporter.Save(fd.FileName, _lastUp, _lastDown, append, txtServer.Text, txtPort.Text, txtChannels.Text);
        MessageBox.Show("Export réussi !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
      } catch (Exception ex) {
        MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }

  private void ImportConfiguration()
  {
    using OpenFileDialog ofd = new OpenFileDialog();
    ofd.Filter = "Configuration Iperf (*.json)|*.json";
    ofd.Title = "Importer une configuration JSON";

    if (ofd.ShowDialog() == DialogResult.OK)
    {
      try
      {
        string content = File.ReadAllText(ofd.FileName);

        // Appel de la validation STRICTE avec message d'erreur
        if (ConfigService.IsValidConfig(content, out ConfigData? validatedData, out string errorMsg))
        {
          var result = MessageBox.Show("Le fichier est valide. Remplacer la configuration actuelle ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
              ConfigService.Save(validatedData!); // Sauvegarde en tant que config.json
              _config = validatedData!;
              RefreshPresetList();
              MessageBox.Show("Configuration importée et appliquée !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        else
        {
          // Affichage de l'erreur précise trouvée par IsValidConfig
          MessageBox.Show($"Fichier JSON invalide :\n\n{errorMsg}", "Échec de l'importation", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Erreur de lecture : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }

  private void ExportConfiguration()
  {
    using SaveFileDialog sfd = new SaveFileDialog();
    sfd.Filter = "Configuration Iperf (*.json)|*.json";
    sfd.FileName = "config_iperf_export.json";

    if (sfd.ShowDialog() == DialogResult.OK)
    {
      try
      {
        // On s'assure que la config en mémoire est bien synchronisée sur le disque
        ConfigService.Save(_config);

        string sourceFile = Path.Combine(AppContext.BaseDirectory, "config.json");
        File.Copy(sourceFile, sfd.FileName, true);

        MessageBox.Show("Exportation terminée !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Erreur d'exportation : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }
}