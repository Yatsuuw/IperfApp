using IperfApp.Models;
using IperfApp.UI.Helpers;

namespace IperfApp.Services;

public static class CsvExporter
{
  private const string Separator = ";";

  private static readonly Encoding CsvEncoding =
    new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

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
      preset.IpVersion.ToLabel(),
      result.Upload.ToString("F2", CultureInfo.InvariantCulture),
      result.Download.ToString("F2", CultureInfo.InvariantCulture)));
  }

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

  private static string EscapeCsv(string value)
  {
    if (value.Contains(';') || value.Contains('"') || value.Contains('\n'))
      return $"\"{value.Replace("\"", "\"\"")}\"";
    return value;
  }
}
