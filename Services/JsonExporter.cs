using System.Text.Json;

namespace IperfApp.Services;

public static class JsonExporter
{
  internal static readonly JsonSerializerOptions Options =
    new() { WriteIndented = true };

  public static void SaveToFile<T>(string path, T value)
  {
    ArgumentNullException.ThrowIfNull(value);
    File.WriteAllText(path, Serialize(value));
  }

  public static string Serialize<T>(T value) =>
    JsonSerializer.Serialize(value, Options);
}
