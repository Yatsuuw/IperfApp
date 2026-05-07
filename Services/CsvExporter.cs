using System.Text;
using IperfApp.Models;

namespace IperfApp.Services;

/// <summary>Exporte les résultats d'un test de débit au format CSV.</summary>
public static class CsvExporter
{
    /// <summary>Séparateur de colonnes. Modifiez cette constante pour changer le format CSV.</summary>
    private const string Separator = ";";

    /// <summary>
    /// Encodage UTF-8 avec BOM : permet à Excel (Windows) de détecter
    /// automatiquement l'encodage sans étape d'importation manuelle.
    /// </summary>
    private static readonly Encoding CsvEncoding =
        new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

    private static readonly string Header =
        string.Join(Separator,
            "Date", "Heure", "Serveur", "Port", "Canaux", "Upload_Mbps", "Download_Mbps");

    /// <summary>
    /// Enregistre un résultat dans un fichier CSV.
    /// </summary>
    /// <param name="path">Chemin complet du fichier de destination.</param>
    /// <param name="result">Résultat du test à exporter.</param>
    /// <param name="preset">Profil utilisé pour le test (serveur, port, canaux).</param>
    /// <param name="append">
    ///   Si <c>true</c>, ajoute une ligne à la fin du fichier existant
    ///   (sans réécrire l'en-tête s'il est déjà présent).
    ///   Si <c>false</c>, écrase le fichier.
    /// </param>
    /// <exception cref="IOException">Propagée à l'appelant si l'écriture échoue.</exception>
    public static void Save(string path, TestResult result, Preset preset, bool append)
    {
        bool fileExists = File.Exists(path) && new FileInfo(path).Length > 0;
        bool needHeader = !append || !fileExists;

        using var sw = new StreamWriter(path, append: append, encoding: CsvEncoding);

        if (needHeader)
            sw.WriteLine(Header);

        sw.WriteLine(string.Join(Separator,
            result.Timestamp.ToString("dd/MM/yyyy"),
            result.Timestamp.ToString("HH:mm:ss"),
            preset.Server,
            preset.Port,
            preset.Channels,
            result.Upload.ToString("F2"),
            result.Download.ToString("F2")));
    }
}
