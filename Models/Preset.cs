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

    /// <inheritdoc/>
    public override string ToString() => Name;
}
