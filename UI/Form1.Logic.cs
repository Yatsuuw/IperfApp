using IperfApp.Services;

namespace IperfApp.UI;

public partial class Form1 : Form
{
    private ConfigData _config = null!;

    // Initialisation de la config
    private void LoadConfigIntoUI()
    {
      _config = ConfigService.Load();
      RefreshPresetList();
    }

  private void RefreshPresetList()
  {
    // Désactiver l'événement temporairement pour éviter les boucles
    cbPresets.SelectedIndexChanged -= CbPresets_SelectedIndexChanged;
    
    cbPresets.DataSource = null;
    cbPresets.DataSource = _config.Presets;
    cbPresets.DisplayMember = "Name";
    
    // Sélectionner le dernier preset utilisé
    var selected = _config.Presets.FirstOrDefault(p => p.Name == _config.SelectedPresetName) ?? _config.Presets.FirstOrDefault(p => p.Name == "Défaut") ?? _config.Presets[0];
    cbPresets.SelectedItem = selected;
    ApplyPreset(selected);
      
    cbPresets.SelectedIndexChanged += CbPresets_SelectedIndexChanged;
  }

  private void CbPresets_SelectedIndexChanged(object? sender, EventArgs e)
  {
    if (cbPresets.SelectedItem is Preset p)
    {
      _config.SelectedPresetName = p.Name;
      ConfigService.Save(_config);
      ApplyPreset(p);
    }
  }

  private void ApplyPreset(Preset p)
  {
    txtServer.Text = p.Server;
    txtPort.Text = p.Port;
    txtChannels.Text = p.Channels;
  }

    // Logique des tests
  private async Task RunFullTest()
  {
    if (string.IsNullOrWhiteSpace(txtServer.Text))
    {
      MessageBox.Show("Veuillez entrer l'adresse du serveur.", "Champ requis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

    btnStart.Enabled = false;
    btnStart.Text = "ANALYSE EN COURS...";
    btnStart.BackColor = Color.FromArgb(160, 174, 192);
    txtLog.Clear();

    txtLog.AppendText(" [SYSTÈME] Démarrage des flux..." + Environment.NewLine);
    txtLog.AppendText(" >>> FLUX MONTANT (UPLOAD)" + Environment.NewLine);
    _lastUp = await _engine.ExecuteAsync(txtServer.Text, txtPort.Text, txtChannels.Text, false);

    txtLog.AppendText(Environment.NewLine + " <<< FLUX DESCENDANT (DOWNLOAD)" + Environment.NewLine);
    _lastDown = await _engine.ExecuteAsync(txtServer.Text, txtPort.Text, txtChannels.Text, true);

    txtLog.AppendText(Environment.NewLine + " ╔══════════════════════════════════════╗" + Environment.NewLine);
    txtLog.AppendText($" ║  RÉSULTATS DE LA MESURE              ║" + Environment.NewLine);
    txtLog.AppendText($" ║  Upload   : {_lastUp,10:F2} Mbps          ║" + Environment.NewLine);
    txtLog.AppendText($" ║  Download : {_lastDown,10:F2} Mbps          ║" + Environment.NewLine);
    txtLog.AppendText(" ╚══════════════════════════════════════╝" + Environment.NewLine);

    btnStart.Enabled = true;
    btnStart.Text = "RELANCER L'ANALYSE";
    btnStart.BackColor = _colorAccent;
    btnExportNew.Enabled = btnExportAppend.Enabled = true;
    btnExportNew.FlatAppearance.BorderColor = btnExportAppend.FlatAppearance.BorderColor = _colorAccent;
  }

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
          var result = MessageBox.Show(
              "Le fichier est valide. Remplacer la configuration actuelle ?",
              "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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