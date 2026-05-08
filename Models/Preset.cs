using System.Text.Json.Serialization;
using IperfApp.Services;

namespace IperfApp.Models;

public class Preset
{
	public string Name { get; set; } = "";
	public string Server { get; set; } = "";

	[JsonConverter(typeof(IntOrStringConverter))]
	public int Port { get; set; } = 5201;

	[JsonConverter(typeof(IntOrStringConverter))]
	public int Channels { get; set; } = 8;

	public int Duration { get; set; } = 10;
	public IpVersion IpVersion { get; set; } = IpVersion.Auto;
	public bool IsDefault { get; set; } = false;

	public override string ToString() => Name;

	public string? Validate()
	{
		if (string.IsNullOrWhiteSpace(Name)) return "Le nom du profil est obligatoire.";
		if (string.IsNullOrWhiteSpace(Server)) return "L'adresse du serveur est obligatoire.";
		if (Port is < 1 or > 65535) return $"Port invalide ({Port}) — doit être compris entre 1 et 65 535.";
		if (Channels is < 1 or > 128) return $"Canaux invalides ({Channels}) — doit être compris entre 1 et 128.";
		if (Duration is < 1 or > 120) return $"Durée invalide ({Duration}) — doit être comprise entre 1 et 120 s.";
		if (!Enum.IsDefined(IpVersion)) return $"Version IP invalide ({(int)IpVersion}).";
		return null;
	}
}
