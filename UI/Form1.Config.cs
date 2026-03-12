using IperfApp.Services;

namespace IperfApp.UI;

public partial class Form1
{
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
}