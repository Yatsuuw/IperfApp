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
    /// le résultat est ajouté à la suite ; sinon un nouveau fichier est créé.
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
            sw.WriteLine(BuildHeader());

        sw.WriteLine(string.Join(Separator,
            result.Timestamp.ToString("dd/MM/yyyy"),
            result.Timestamp.ToString("HH:mm:ss"),
            EscapeCsv(preset.Server),
            preset.Channels,
            preset.Port,
            preset.IpVersion switch
            {
                IpVersion.IPv4 => "IPv4",
                IpVersion.IPv6 => "IPv6",
                _              => "Auto"
            },
            result.Upload.ToString("F2", CultureInfo.InvariantCulture),
            result.Download.ToString("F2", CultureInfo.InvariantCulture)));
    }

    // ---------------------------------------------------------------
    // Privé
    // ---------------------------------------------------------------

    private static string BuildHeader() =>
        string.Join(Separator,
            "Date",
            "Heure",
            "Serveur",
            "Canaux",
            "Port",
            "Protocole IP",
            "Débit ascendant (Mbps)",
            "Débit descendant (Mbps)");

    /// <summary>
    /// Échappe une valeur CSV : si elle contient <c>;</c>, <c>"</c> ou un saut de ligne,
    /// elle est entourée de guillemets doubles et les guillemets internes sont doublés.
    /// </summary>
    private static string EscapeCsv(string value)
    {
        if (value.Contains(';') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
