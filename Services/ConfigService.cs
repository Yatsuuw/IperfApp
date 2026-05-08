using System.Text.Json;
using IperfApp.Models;

namespace IperfApp.Services;

public static class ConfigService
{
	private static readonly string ConfigPath =
		Path.Combine(AppContext.BaseDirectory, "config.json");

	public static bool TryParse(string json, out ConfigData? data, out string errorMessage)
	{
		data = null;
		errorMessage = string.Empty;

		try
		{
			data = JsonSerializer.Deserialize<ConfigData>(json, JsonExporter.Options);
			if (data is null)
			{
				errorMessage = "La désérialisation a produit un résultat nul.";
				return false;
			}

			if (data.Presets is null)
			{
				errorMessage = "La propriété 'Presets' est absente du JSON.";
				return false;
			}

			MigrateDefaults(data);

			foreach (var p in data.Presets)
			{
				string? err = p.Validate();
				if (err is not null)
				{
					errorMessage = $"Profil '{p.Name}' invalide : {err}";
					return false;
				}
			}

			return true;
		}
		catch (JsonException ex)
		{
			errorMessage = $"Syntaxe JSON invalide : {ex.Message}";
			return false;
		}
	}

	public static bool NeedsMigration(string json)
	{
		try
		{
			using var doc = JsonDocument.Parse(json);
			var root = doc.RootElement;

			if (!root.TryGetProperty("Presets", out var presets)) return false;

			foreach (var p in presets.EnumerateArray())
			{
				if (p.TryGetProperty("Port", out var port) && port.ValueKind == JsonValueKind.String) return true;
				if (p.TryGetProperty("Channels", out var ch) && ch.ValueKind == JsonValueKind.String) return true;
				if (!p.TryGetProperty("Duration", out _)) return true;
				if (!p.TryGetProperty("IpVersion", out _)) return true;
			}

			return false;
		}
		catch
		{
			return false;
		}
	}

	public static ConfigData Load()
	{
		if (!File.Exists(ConfigPath))
			return CreateAndSaveDefault();

		try
		{
			string json = File.ReadAllText(ConfigPath);
			if (TryParse(json, out ConfigData? data, out string parseError))
			{
				if (NeedsMigration(json))
				{
					try { Save(data!); }
					catch (Exception ex) { Debug.WriteLine($"[ConfigService] Migration auto échouée : {ex.Message}"); }
				}
				return data!;
			}

			Debug.WriteLine($"[ConfigService] config.json invalide ({parseError}), réinitialisation au défaut.");
			return CreateAndSaveDefault();
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"[ConfigService] Impossible de lire config.json : {ex.Message}");
			return CreateDefault();
		}
	}

	public static void Save(ConfigData data)
	{
		ArgumentNullException.ThrowIfNull(data);
		JsonExporter.SaveToFile(ConfigPath, data);
	}

	private static void MigrateDefaults(ConfigData data)
	{
		foreach (var p in data.Presets)
		{
			if (p.Duration == 0) p.Duration = 10;
		}
	}

	private static ConfigData CreateDefault()
	{
		var data = new ConfigData();
		data.Presets.Add(new Preset
		{
			Name = "Défaut",
			Server = "poi.cubic.iperf.bytel.fr",
			Port = 9240,
			Channels = 8,
			Duration = 10,
			IsDefault = true
		});
		return data;
	}

	private static ConfigData CreateAndSaveDefault()
	{
		var data = CreateDefault();
		try { Save(data); }
		catch (Exception ex) { Debug.WriteLine($"[ConfigService] Impossible d'écrire config.json par défaut : {ex.Message}"); }
		return data;
	}
}
