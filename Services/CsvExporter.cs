using IperfApp.Models;

namespace IperfApp.Services;

/// <summary>Exporte les résultats d'un test de débit au format CSV.</summary>
public static class CsvExporter
{
  private const string Header = "Date;Heure;Serveur;Port;Canaux;Upload_Mbps;Download_Mbps";

  /// <summary>
  /// Enregistre un résultat dans un fichier CSV.
  /// Si <paramref name="append"/> est <c>true</c>, ajoute une ligne à la fin du fichier existant
  /// (sans réécrire l'en-tête s'il est déjà présent).
  /// </summary>
  /// <param name="path">Chemin complet du fichier de destination.</param>
  /// <param name="result">Résultat du test à exporter.</param>
  /// <param name="preset">Profil utilisé pour le test (serveur, port, canaux).</param>
  /// <param name="append">Si <c>true</c>, ouvre le fichier en mode ajout ; sinon, écrase.</param>
  public static void Save(string path, TestResult result, Preset preset, bool append)
  {
    bool fileExists = File.Exists(path) && new FileInfo(path).Length > 0;
    bool needHeader = !append || !fileExists;

    using var sw = new StreamWriter(path, append: append);
    if (needHeader)
      sw.WriteLine(Header);

    sw.WriteLine(
      $"{result.Timestamp:dd/MM/yyyy};{result.Timestamp:HH:mm:ss};" +
      $"{preset.Server};{preset.Port};{preset.Channels};" +
      $"{result.Upload:F2};{result.Download:F2}"
    );
  }
}
