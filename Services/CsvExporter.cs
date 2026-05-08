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

    /// <summary>
    /// En-tête CSV — calculée à chaque appel pour inclure le bon timestamp d'export.
    /// Une propriété statique évite l'allocation répétée de la chaîne.
    /// </summary>
    private static string Header =>
        string.Join(Separator,
            "Date export", "Horodatage mesure",
            "Profil", "Serveur", "Port", "Canaux", "Durée (s)",
            $"Upload (Mbps)", "Download (Mbps)",
            $"Export généré le : {DateTime.Now:dd/MM/yyyy HH:mm:ss}");

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

        bool fileExists = File.Exists(filePath);
        bool needHeader = !append || !fileExists;

        using var sw = new StreamWriter(filePath, append: append, encoding: CsvEncoding);

        if (needHeader)
            sw.WriteLine(Header);

        sw.WriteLine(string.Join(Separator,
            DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            result.Timestamp.ToString("dd/MM/yyyy HH:mm:ss"),
            EscapeCsv(preset.Name),
            EscapeCsv(preset.Server),
            preset.Port,
            preset.Channels,
            preset.Duration,
            result.Upload.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            result.Download.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)));
    }

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
