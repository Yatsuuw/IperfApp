using IperfApp.Models;

namespace IperfApp.UI.Helpers;

/// <summary>
/// Extensions pour <see cref="IpVersion"/> :
/// centralise la correspondance entre l'enum et l'index du ComboBox,
/// ainsi que la conversion en libellé lisible (CSV, logs).
/// Évite la triplication du switch dans MainForm, SettingsForm et CsvExporter.
/// </summary>
internal static class IpVersionExtensions
{
    /// <summary>Convertit une valeur <see cref="IpVersion"/> en index ComboBox (0 Auto, 1 IPv4, 2 IPv6).</summary>
    public static int ToComboIndex(this IpVersion v) => v switch
    {
        IpVersion.IPv4 => 1,
        IpVersion.IPv6 => 2,
        _              => 0
    };

    /// <summary>Convertit un index ComboBox (0/1/2) en valeur <see cref="IpVersion"/>.</summary>
    public static IpVersion FromComboIndex(int index) => index switch
    {
        1 => IpVersion.IPv4,
        2 => IpVersion.IPv6,
        _ => IpVersion.Auto
    };

    /// <summary>
    /// Retourne le libellé texte de la version IP pour l'export CSV et les logs.
    /// Source unique de vérité — ne pas dupliquer ce switch ailleurs.
    /// </summary>
    public static string ToLabel(this IpVersion v) => v switch
    {
        IpVersion.IPv4 => "IPv4",
        IpVersion.IPv6 => "IPv6",
        _              => "Auto"
    };
}
