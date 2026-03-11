using System.Text.Json;

namespace IperfApp.Services; // Vérifie bien que ce namespace est le même partout

public class Preset
{
  public string Name { get; set; } = "";
  public string Server { get; set; } = "";
  public string Port { get; set; } = "5201";
  public string Channels { get; set; } = "8";
}

public class ConfigData
{
  public string SelectedPresetName { get; set; } = "Défaut";
  public List<Preset> Presets { get; set; } = new();
}

public static class ConfigService
{
  // Le fichier s'appellera "config.json"
  private static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "config.json");

  public static bool IsValidConfig(string json, out ConfigData? data)
  {
    data = null;
    try
    {
      data = JsonSerializer.Deserialize<ConfigData>(json);
      // Vérification : il faut au moins un profil et le nom du profil sélectionné ne doit pas être vide
      return data != null && data.Presets != null && data.Presets.Count > 0;
    }
    catch
    {
      return false;
    }
  }

  public static ConfigData Load()
  {
    if (!File.Exists(ConfigPath)) return CreateDefault();
    try
    {
      var json = File.ReadAllText(ConfigPath);
      if (IsValidConfig(json, out ConfigData? data)) return data!;
      return CreateDefault();
    }
    catch { return CreateDefault(); }
  }

  public static void Save(ConfigData data)
  {
    var options = new JsonSerializerOptions { WriteIndented = true };
    File.WriteAllText(ConfigPath, JsonSerializer.Serialize(data, options));
  }

  private static ConfigData CreateDefault()
  {
    var data = new ConfigData();
    data.Presets.Add(new Preset { 
      Name = "Défaut", 
      Server = "poi.cubic.iperf.bytel.fr", 
      Port = "9240", 
      Channels = "8" 
    });
    Save(data);
    return data;
  }
}