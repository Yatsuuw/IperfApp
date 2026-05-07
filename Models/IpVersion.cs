namespace IperfApp.Models;

/// <summary>Version du protocole IP utilisée par iperf3.</summary>
public enum IpVersion
{
  /// <summary>Tente IPv4 en premier, bascule en IPv6 si aucun résultat.</summary>
  Auto = 0,

  /// <summary>Force l'indicateur <c>-4</c> d'iperf3.</summary>
  IPv4 = 4,

  /// <summary>Force l'indicateur <c>-6</c> d'iperf3.</summary>
  IPv6 = 6
}
