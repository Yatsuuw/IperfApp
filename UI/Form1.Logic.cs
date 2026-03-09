using IperfApp.Services;

namespace IperfApp.UI;

public partial class Form1 : Form
{
    private ConfigData _config = null!;

    // --- INITIALISATION DE LA CONFIG ---
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
    var selected = _config.Presets.FirstOrDefault(p => p.Name == _config.SelectedPresetName) ?? _config.Presets[0];
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

    // --- LOGIQUE DES TESTS ---
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

  // --- GESTION DES PARAMÈTRES ET EXPORT ---
  private void OpenSettings()
  {
    using var settings = new SettingsForm(this, _config);
    if (settings.ShowDialog() == DialogResult.OK)
    {
      // Recharger la liste si des changements ont eu lieu
      RefreshPresetList();
    }
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
}