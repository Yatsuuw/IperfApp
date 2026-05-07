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

            if (data.Presets is null || data.Presets.Count == 0)
            {
                errorMessage = "La liste 'Presets' est absente ou vide.";
                return false;
            }

            foreach (var p in data.Presets)
            {
                if (string.IsNullOrWhiteSpace(p.Name))
                {
                    errorMessage = "Un profil possède un nom vide.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(p.Server))
                {
                    errorMessage = $"Le serveur du profil '{p.Name}' est vide.";
                    return false;
                }

                if (p.Port is < 1 or > 65535)
                {
                    errorMessage = $"Port invalide ({p.Port}) dans '{p.Name}'.";
                    return false;
                }

                if (p.Channels is < 1 or > 128)
                {
                    errorMessage = $"Nombre de canaux invalide ({p.Channels}) dans '{p.Name}' — doit être compris entre 1 et 128.";
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

            // Le fichier existe mais son contenu est invalide : on le remplace par le défaut.
            System.Diagnostics.Debug.WriteLine(
                $"[ConfigService] config.json invalide ({parseError}), réinitialisation au défaut.");
            return CreateAndSaveDefault();
        }
        catch (Exception ex)
        {
            // Erreur disque (permissions, fichier verrouillé…) : on trace et on utilise le défaut en mémoire.
            System.Diagnostics.Debug.WriteLine(
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

    // --- Privé ---

    private static ConfigData CreateDefault()
    {
        var data = new ConfigData();
        data.Presets.Add(new Preset
        {
            Name     = "Défaut",
            Server   = "poi.cubic.iperf.bytel.fr",
            Port     = 9240,
            Channels = 8
        });
        return data;
    }

    private static ConfigData CreateAndSaveDefault()
    {
        var data = CreateDefault();
        try   { Save(data); }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[ConfigService] Impossible d'écrire config.json par défaut : {ex.Message}");
        }
        return data;
    }
}
