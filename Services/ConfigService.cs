using System.Text.Json;
using IperfApp.Models;

namespace IperfApp.Services;

/// <summary>Gestion de la persistance de la configuration (config.json).</summary>
public static class ConfigService
{
  private static readonly JsonSerializerOptions _jsonOpts = new() { WriteIndented = true };

  /// <summary>
  /// Chemin absolu vers config.json, calculé une seule fois au démarrage.
  /// Invariant pendant toute la durée de vie du processus.
  /// </summary>
  private static readonly string ConfigPath =
    Path.Combine(AppContext.BaseDirectory, "config.json");

  /// <summary>
  /// Valide et désérialise un JSON de configuration.
  /// Retourne <c>true</c> si le JSON est valide ; <c>false</c> avec un message explicite sinon.
  /// </summary>
  /// <param name="json">Contenu JSON à valider.</param>
  /// <param name="data">Données désérialisées si succès, sinon <c>null</c>.</param>
  /// <param name="errorMessage">Message d'erreur lisible si échec, sinon vide.</param>
  public static bool TryParse(string json, out ConfigData? data, out string errorMessage)
  {
    data = null;
    errorMessage = string.Empty;

    try
    {
      data = JsonSerializer.Deserialize<ConfigData>(json);
      if (data is null)
      {
        errorMessage = "La désérialisation a produit un résultat nul.";
        return false;
      }

      if (data.Presets is null || data.Presets.Count == 0)
      { errorMessage = "La liste 'Presets' est absente ou vide."; return false; }

      foreach (var p in data.Presets)
      {
        if (string.IsNullOrWhiteSpace(p.Name))
        { errorMessage = "Un profil possède un nom vide."; return false; }

        if (string.IsNullOrWhiteSpace(p.Server))
        { errorMessage = $"Le serveur du profil '{p.Name}' est vide."; return false; }

        if (p.Port is < 1 or > 65535)
        { errorMessage = $"Port invalide ({p.Port}) dans '{p.Name}'."; return false; }

        if (p.Channels < 1)
        { errorMessage = $"Nombre de canaux invalide ({p.Channels}) dans '{p.Name}'."; return false; }
      }

      return true;
    }
    catch (JsonException ex)
    {
      errorMessage = $"Syntaxe JSON invalide : {ex.Message}";
      return false;
    }
  }

  /// <summary>
  /// Charge la configuration depuis le disque.
  /// Retourne une configuration par défaut si le fichier est absent ou invalide.
  /// </summary>
  public static ConfigData Load()
  {
    if (!File.Exists(ConfigPath))
      return CreateAndSaveDefault();

    try
    {
      string json = File.ReadAllText(ConfigPath);
      return TryParse(json, out ConfigData? data, out _) ? data! : CreateAndSaveDefault();
    }
    catch
    {
      return CreateAndSaveDefault();
    }
  }

  /// <summary>Sauvegarde la configuration sur le disque.</summary>
  /// <param name="data">Données à persister (ne doit pas être null).</param>
  public static void Save(ConfigData data)
  {
    ArgumentNullException.ThrowIfNull(data);
    File.WriteAllText(ConfigPath, JsonSerializer.Serialize(data, _jsonOpts));
  }

  // ---------------------------------------------------------------
  // Privé
  // ---------------------------------------------------------------

  private static ConfigData CreateAndSaveDefault()
  {
    var data = new ConfigData();
    data.Presets.Add(new Preset
    {
      Name     = "Défaut",
      Server   = "poi.cubic.iperf.bytel.fr",
      Port     = 9240,
      Channels = 8
    });
    Save(data);
    return data;
  }
}
