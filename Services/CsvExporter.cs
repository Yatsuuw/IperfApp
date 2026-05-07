using IperfApp.Models;

namespace IperfApp.Services;

/// <summary>Exporte les résultats d'un test de débit au format CSV.</summary>
public static class CsvExporter
{
    private const string Separator = ";";

    /// <summary>
    /// UTF-8 avec BOM : permet à Excel (Windows) de détecter l'encodage automatiquement.
    /// </summary>
    private static readonly System.Text.Encoding CsvEncoding =
        new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

    /// <summary>En-tête CSV — calculé une seule fois.</summary>
    private static readonly string Header =
        string.Join(Separator, "Date", "Heure", "Serveur", "Port", "Canaux", "Upload_Mbps", "Download_Mbps");

    /// <summary>
    /// Enregistre un résultat dans un fichier CSV.
    /// </summary>
    /// <param name="path">Chemin complet du fichier de destination.</param>
    /// <param name="result">Résultat du test à exporter.</param>
    /// <param name="preset">Profil utilisé pour le test.</param>
    /// <param name="append">
    ///   <c>true</c> → ajoute une ligne (sans réécrire l'en-tête si déjà présent).
    ///   <c>false</c> → écrase le fichier.
    /// </param>
    /// <exception cref="IOException">Propagée à l'appelant si l'écriture échoue.</exception>
    public static void Save(string path, TestResult result, Preset preset, bool append)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(preset);

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
