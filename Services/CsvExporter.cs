using IperfApp.Models;

namespace IperfApp.Services;

/// <summary>Exporte les résultats d'un test de débit au format CSV.</summary>
public static class CsvExporter
{
    private const string Separator = ";";

    /// <summary>
    /// UTF-8 avec BOM : permet à Excel (Windows) de détecter l'encodage automatiquement.
    /// </summary>
    private static readonly Encoding CsvEncoding =
        new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

    /// <summary>
    /// Sauvegarde <paramref name="result"/> dans <paramref name="filePath"/>.
    /// Si <paramref name="append"/> est <c>true</c> et que le fichier existe,
    /// le résultat est ajouté à la suite ; sinon un nouveau fichier est créé.
    /// </summary>
    /// <exception cref="IOException">Propagée à l'appelant si l'écriture échoue.</exception>
    public static void Save(string filePath, TestResult result, Preset preset, bool append)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(preset);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // Capturer le timestamp une seule fois pour garantir la cohérence
        // entre l'en-tête et la ligne de données, même si l'horloge tourne
        // entre les deux écritures.
        DateTime exportTime = DateTime.Now;

        bool fileExists = File.Exists(filePath);
        bool needHeader = !append || !fileExists;

        using var sw = new StreamWriter(filePath, append: append, encoding: CsvEncoding);

        if (needHeader)
            sw.WriteLine(BuildHeader(exportTime));

        sw.WriteLine(string.Join(Separator,
            exportTime.ToString("dd/MM/yyyy HH:mm:ss"),
            result.Timestamp.ToString("dd/MM/yyyy HH:mm:ss"),
            EscapeCsv(preset.Name),
            EscapeCsv(preset.Server),
            preset.Port,
            preset.Channels,
            preset.Duration,
            result.Upload.ToString("F2", CultureInfo.InvariantCulture),
            result.Download.ToString("F2", CultureInfo.InvariantCulture)));
    }

    // ---------------------------------------------------------------
    // Privé
    // ---------------------------------------------------------------

    /// <summary>
    /// Construit l'en-tête CSV avec le timestamp d'export capturé à l'entrée de <see cref="Save"/>.
    /// Méthode statique plutôt que propriété pour rendre explicite que le timestamp
    /// est une valeur injectée, non un side-effect de l'heure courante.
    /// </summary>
    private static string BuildHeader(DateTime exportTime) =>
        string.Join(Separator,
            "Date export", "Horodatage mesure",
            "Profil", "Serveur", "Port", "Canaux", "Durée (s)",
            "Upload (Mbps)", "Download (Mbps)",
            $"Export généré le : {exportTime:dd/MM/yyyy HH:mm:ss}");

    /// <summary>
    /// Échappe une valeur CSV : si elle contient <c>;</c>, <c>"</c> ou un saut de ligne,
    /// elle est entourée de guillemets doubles et les guillemets internes sont doublés.
    /// </summary>
    private static string EscapeCsv(string value)
    {
        if (value.Contains(';') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
