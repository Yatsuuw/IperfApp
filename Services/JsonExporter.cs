using System.Text.Json;

namespace IperfApp.Services;

/// <summary>
/// Sérialise et désérialise du JSON avec les options partagées de l'application.
/// Centralise l'instance <see cref="JsonSerializerOptions"/> pour éviter les doublons.
/// </summary>
public static class JsonExporter
{
  /// <summary>Options communes : indentation activée, réutilisable sans réallocation.</summary>
  internal static readonly JsonSerializerOptions Options =
    new() { WriteIndented = true };

  /// <summary>
  /// Sérialise <paramref name="value"/> en JSON indenté et
  /// écrit le résultat dans <paramref name="path"/>.
  /// </summary>
  /// <typeparam name="T">Type de l'objet à sérialiser.</typeparam>
  /// <param name="path">Chemin complet du fichier de destination.</param>
  /// <param name="value">Objet à sérialiser (ne doit pas être null).</param>
  public static void SaveToFile<T>(string path, T value)
  {
    ArgumentNullException.ThrowIfNull(value);
    File.WriteAllText(path, Serialize(value));
  }

  /// <summary>Retourne la représentation JSON indentée de <paramref name="value"/>.</summary>
  public static string Serialize<T>(T value) =>
    JsonSerializer.Serialize(value, Options);
}
