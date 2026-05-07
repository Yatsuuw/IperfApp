namespace IperfApp.Models;

/// <summary>Représente la configuration persistée de l'application.</summary>
public class ConfigData
{
    /// <summary>Nom du profil sélectionné au dernier démarrage.</summary>
    public string SelectedPresetName { get; set; } = "Défaut";

    /// <summary>Liste de tous les profils configurés.</summary>
    public List<Preset> Presets { get; set; } = [];
}
