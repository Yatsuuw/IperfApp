using IperfApp.Models;
using IperfApp.Services;

namespace IperfApp.UI;

public partial class Form1
{
  private ConfigData _config = null!;

  /// <summary>Charge la configuration depuis le disque et met à jour l'UI.</summary>
  private void LoadConfigIntoUI()
  {
    _config = ConfigService.Load();
    RefreshPresetList();
  }

  /// <summary>Repeuple le <see cref="ComboBox"/> des profils et sélectionne le dernier utilisé.</summary>
  internal void RefreshPresetList()
  {
    if (_config.Presets.Count == 0) return;

    cbPresets.SelectedIndexChanged -= CbPresets_SelectedIndexChanged;

    cbPresets.DataSource    = null;
    cbPresets.DataSource    = _config.Presets;
    cbPresets.DisplayMember = "Name";

    var selected =
      _config.Presets.FirstOrDefault(p => p.Name == _config.SelectedPresetName)
      ?? _config.Presets[0];

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
    txtServer.Text   = p.Server;
    txtPort.Text     = p.Port.ToString();
    txtChannels.Text = p.Channels.ToString();
    txtDuration.Text = p.Duration > 0 ? p.Duration.ToString() : "10";
    cbIpVersion.SelectedIndex = p.IpVersion switch
    {
      IpVersion.IPv4 => 1,
      IpVersion.IPv6 => 2,
      _              => 0
    };
  }
}
