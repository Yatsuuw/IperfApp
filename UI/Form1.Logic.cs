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

  private void ExportConfiguration()
  {
    using SaveFileDialog sfd = new();
    // On propose .json par défaut pour que l'utilisateur puisse l'ouvrir facilement ailleurs
    sfd.Filter = "Fichier de configuration (*.json)|*.json|Tous les fichiers (*.*)|*.*";
    sfd.Title = "Exporter la configuration";
    sfd.FileName = "iperf_config_backup.json";

    if (sfd.ShowDialog() == DialogResult.OK)
    {
      try
      {
        // Recherche du fichier config
        string sourceFile = Path.Combine(AppContext.BaseDirectory, "config");
        
        if (File.Exists(sourceFile))
        {
          File.Copy(sourceFile, sfd.FileName, true);
          MessageBox.Show("Configuration exportée avec succès !", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
          // Si le fichier n'existe pas encore, forcer via le service avant d'exporter
          ConfigService.Save(_config);
          File.Copy(sourceFile, sfd.FileName, true);
          MessageBox.Show("Fichier généré et exporté avec succès !", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Erreur lors de l'exportation : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }

  private void ImportConfiguration()
  {
    using OpenFileDialog ofd = new();
    ofd.Filter = "Fichier de configuration (*.*)|*.*";
    ofd.Title = "Sélectionner une configuration à importer";

    if (ofd.ShowDialog() == DialogResult.OK)
    {
      var result = MessageBox.Show("L'importation remplacera tous vos profils actuels. Continuer ?", 
                                  "Confirmation d'importation", 
                                  MessageBoxButtons.YesNo, 
                                  MessageBoxIcon.Warning);

      if (result == DialogResult.Yes)
      {
        try
        {
          // La destination est "config" sans extension
          string destFile = Path.Combine(AppContext.BaseDirectory, "config");
          
          // Copie et remplacement
          File.Copy(ofd.FileName, destFile, true);

          // Rechargement immédiat via ton service
          _config = ConfigService.Load(); 
          
          // Mise à jour de l'UI
          RefreshPresetList();

          MessageBox.Show("Configuration importée et appliquée avec succès !", "Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Erreur lors de l'importation : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }
  }
}