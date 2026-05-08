namespace IperfApp.Models;

/// <summary>Profil de connexion iperf3 enregistré par l'utilisateur.</summary>
public class Preset
{
    /// <summary>Nom affiché dans l'interface.</summary>
    public string Name { get; set; } = "";

    /// <summary>Adresse IP ou nom d'hôte du serveur iperf3.</summary>
    public string Server { get; set; } = "";

    /// <summary>Port TCP de destination (1–65535).</summary>
    public int Port { get; set; } = 5201;

    /// <summary>Nombre de flux TCP parallèles (1–128).</summary>
    public int Channels { get; set; } = 8;

    /// <summary>Durée du test en secondes (1–120, défaut 10).</summary>
    public int Duration { get; set; } = 10;

    /// <summary>Version IP forcée pour ce profil.</summary>
    public IpVersion IpVersion { get; set; } = IpVersion.Auto;

    /// <summary>
    /// Indique si ce profil est le profil système protégé (non supprimable, non renommable).
    /// Remplace la garde <c>p.Name == "Défaut"</c> hardcodée dans SettingsForm.
    /// Initialisé à <c>false</c> pour tous les profils créés par l'utilisateur.
    /// Le profil créé par <see cref="IperfApp.Services.ConfigService.CreateDefault"/> est
    /// le seul à recevoir <c>IsDefault = true</c>.
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <inheritdoc/>
    public override string ToString() => Name;

    /// <summary>
    /// Valide tous les champs du profil.
    /// Retourne un message d'erreur localisé, ou <c>null</c> si le profil est valide.
    /// </summary>
    public string? Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))   return "Le nom du profil est obligatoire.";
        if (string.IsNullOrWhiteSpace(Server)) return "L'adresse du serveur est obligatoire.";
        if (Port     is < 1 or > 65535)        return $"Port invalide ({Port}) — doit être compris entre 1 et 65 535.";
        if (Channels is < 1 or > 128)          return $"Canaux invalides ({Channels}) — doit être compris entre 1 et 128.";
        if (Duration is < 1 or > 120)          return $"Durée invalide ({Duration}) — doit être comprise entre 1 et 120 s.";
        if (!Enum.IsDefined(IpVersion))         return $"Version IP invalide ({(int)IpVersion}).";
        return null;
    }
}
