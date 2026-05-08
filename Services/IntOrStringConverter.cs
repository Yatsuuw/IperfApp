using System.Text.Json;
using System.Text.Json.Serialization;

namespace IperfApp.Services;

public sealed class IntOrStringConverter : JsonConverter<int>
{
	public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Number)
			return reader.GetInt32();

		if (reader.TokenType == JsonTokenType.String)
		{
			string? s = reader.GetString();
			if (int.TryParse(s, out int v)) return v;
			throw new JsonException($"Impossible de convertir '{s}' en entier.");
		}

		throw new JsonException($"Type inattendu : {reader.TokenType}.");
	}

	public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
		=> writer.WriteNumberValue(value);
}
