namespace IperfApp.Models;

/// <summary>Résultat immuable d'un test de débit complet (Upload + Download).</summary>
/// <param name="Upload">Débit montant mesuré en Mbps.</param>
/// <param name="Download">Débit descendant mesuré en Mbps.</param>
/// <param name="Timestamp">Date et heure de la mesure.</param>
public record TestResult(double Upload, double Download, DateTime Timestamp);
