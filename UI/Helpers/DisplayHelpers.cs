namespace IperfApp.UI.Helpers;

/// <summary>
/// Fonctions d'affichage pure (sans dépendance UI) partagées entre les vues.
/// </summary>
internal static class DisplayHelpers
{
    /// <summary>
    /// Formate un débit en Mbps vers l'unité lisible la plus adaptée.
    /// <list type="bullet">
    ///   <item><description>≥ 1000 Mbps → Gbps</description></item>
    ///   <item><description>≥ 1 Mbps → Mbps</description></item>
    ///   <item><description>&lt; 1 Mbps → Kbps</description></item>
    /// </list>
    /// </summary>
    internal static string FormatMbps(double mbps) => mbps switch
    {
        >= 1000 => $"{mbps / 1000.0:F2} Gbps",
        >= 1    => $"{mbps:F2} Mbps",
        _       => $"{mbps * 1000.0:F1} Kbps"
    };
}
