using IperfApp.Models;

namespace IperfApp.UI.Constants;

/// <summary>
/// Extensions pour <see cref="IpVersion"/> :
/// centralise la correspondance entre l'enum et l'index du ComboBox.
/// Évite la triplication du switch dans MainForm, SettingsForm et BuildConfigCard.
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
}
