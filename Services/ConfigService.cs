using System.Text.Json;
using IperfApp.Models;

namespace IperfApp.Services;

/// <summary>Gestion de la persistance de la configuration (config.json).</summary>
public static class ConfigService
{
    /// <summary>Chemin absolu vers config.json, calculé une seule fois au démarrage.</summary>
    private static readonly string ConfigPath =
        Path.Combine(AppContext.BaseDirectory, "config.json");

    /// <summary>
    /// Valide et désérialise un JSON de configuration.
    /// Retourne <c>true</c> si valide, <c>false</c> avec un message d'erreur explicite sinon.
    /// <para>
    /// Une liste <c>Presets</c> vide est considérée valide : l'UI gère l'état
    /// « aucun profil » via <c>RefreshPresetList()</c>. Seule l'absence totale
    /// de la propriété (désérialisation nulle) est rejetée.
    /// </para>
    /// </summary>
    public static bool TryParse(string json, out ConfigData? data, out string errorMessage)
    {
        data         = null;
        errorMessage = string.Empty;

        try
        {
            data = JsonSerializer.Deserialize<ConfigData>(json);
            if (data is null)
            {
                errorMessage = "La désérialisation a produit un résultat nul.";
                return false;
            }

            // Presets null  = propriété absente du JSON → erreur structurelle.
            // Presets vide  = config valide, l'UI affichera l'état « aucun profil ».
            if (data.Presets is null)
            {
                errorMessage = "La propriété 'Presets' est absente du JSON.";
                return false;
            }

            foreach (var p in data.Presets)
            {
                string? validationError = p.Validate();
                if (validationError is not null)
                {
                    errorMessage = $"Profil '{p.Name}' invalide : {validationError}";
                    return false;
                }
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
    /// Retourne une configuration par défaut si le fichier est absent, invalide ou illisible.
    /// </summary>
    public static ConfigData Load()
    {
        if (!File.Exists(ConfigPath))
            return CreateAndSaveDefault();

        try
        {
            string json = File.ReadAllText(ConfigPath);
            if (TryParse(json, out ConfigData? data, out string parseError))
                return data!;

            Debug.WriteLine(
                $"[ConfigService] config.json invalide ({parseError}), réinitialisation au défaut.");
            return CreateAndSaveDefault();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"[ConfigService] Impossible de lire config.json : {ex.Message}");
            return CreateDefault();
        }
    }

    /// <summary>
    /// Sauvegarde la configuration sur le disque.
    /// Lève une <see cref="IOException"/> si l'écriture échoue (déléguer la gestion à l'appelant).
    /// </summary>
    public static void Save(ConfigData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        JsonExporter.SaveToFile(ConfigPath, data);
    }

    // ---------------------------------------------------------------
    // Privé
    // ---------------------------------------------------------------

    private static ConfigData CreateDefault()
    {
        var data = new ConfigData();
        data.Presets.Add(new Preset
        {
            Name     = "Défaut",
            Server   = "poi.cubic.iperf.bytel.fr",
            Port     = 9240,
            Channels = 8,
            Duration = 10
        });
        return data;
    }

    private static ConfigData CreateAndSaveDefault()
    {
        var data = CreateDefault();
        try   { Save(data); }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"[ConfigService] Impossible d'écrire config.json par défaut : {ex.Message}");
        }
        return data;
    }
}
