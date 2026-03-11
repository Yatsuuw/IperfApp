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

  public static bool IsValidConfig(string json, out ConfigData? data, out string errorMessage)
  {
    data = null;
    errorMessage = "";
    try
    {
      // Analyse structurelle
      using JsonDocument doc = JsonDocument.Parse(json);
      JsonElement root = doc.RootElement;

      // Vérification racine : Attendu : 2 champs : SelectedPresetName et Presets
      int rootFieldCount = 0;
      foreach (var prop in root.EnumerateObject()) rootFieldCount++;

      if (rootFieldCount != 2) {
        errorMessage = $"Structure racine invalide : {rootFieldCount} champs trouvés au lieu de 2.";
        return false;
      }

      if (!root.TryGetProperty("SelectedPresetName", out _) || !root.TryGetProperty("Presets", out JsonElement presetsElem)) {
        errorMessage = "Les champs racine doivent être exactement 'SelectedPresetName' et 'Presets'.";
        return false;
      }

      // Vérification des profils
      if (presetsElem.ValueKind != JsonValueKind.Array) {
        errorMessage = "Le champ 'Presets' doit être une liste (Array).";
        return false;
      }

      foreach (JsonElement preset in presetsElem.EnumerateArray())
      {
        int fieldCount = 0;
        foreach (var prop in preset.EnumerateObject()) fieldCount++;

        // Attendu : 4 champs par profil : Name, Server, Port, Channels
        if (fieldCount != 4) {
          errorMessage = "Un profil contient un nombre de champs incorrect (attendu : 4).";
          return false;
        }

        string[] expectedFields = ["Name", "Server", "Port", "Channels"];
        foreach (var field in expectedFields) {
          if (!preset.TryGetProperty(field, out _)) {
            errorMessage = $"Champ manquant ou mal nommé : '{field}' attendu dans le profil.";
            return false;
          }
        }
      }

      // Désérialisation et validation des types/valeurs
      data = JsonSerializer.Deserialize<ConfigData>(json);
      if (data == null) return false;

      foreach (var p in data.Presets)
      {
        if (string.IsNullOrWhiteSpace(p.Name) || string.IsNullOrWhiteSpace(p.Server)) {
          errorMessage = "Le nom ou le serveur d'un profil est vide."; return false;
        }
        if (!int.TryParse(p.Port, out int port) || port < 1 || port > 65535) {
          errorMessage = $"Port invalide dans '{p.Name}'."; return false;
        }
        if (!int.TryParse(p.Channels, out int chan) || chan < 1) {
          errorMessage = $"Nombre de canaux invalide dans '{p.Name}'."; return false;
        }
      }

      return true;
    }
    catch (JsonException ex) {
      errorMessage = $"Erreur de syntaxe JSON : {ex.Message}";
      return false;
    }
  }

  public static ConfigData Load()
  {
    if (!File.Exists(ConfigPath)) return CreateDefault();
    try
    {
      var json = File.ReadAllText(ConfigPath);
      // On passe 'out _' car il n'y a pas besoin du message d'erreur au démarrage
      if (IsValidConfig(json, out ConfigData? data, out _)) return data!;
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