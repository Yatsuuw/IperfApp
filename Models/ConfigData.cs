namespace IperfApp.Models;

public class ConfigData
{
  public string SelectedPresetName { get; set; } = "Défaut";
  public List<Preset> Presets { get; set; } = [];
}
