using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using PalmMapEditor.Tilemaps;
using PalmMapEditor.Tilemaps.Objects;

namespace PalmMapEditor.JsonConverters;

public class TilemapObjectJsonConverter : JsonConverter<TilemapObject>
{
    public override TilemapObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;
        if (!jsonObject.TryGetProperty("Type", out var typeProperty))
        {
            throw new JsonException("Missing Type property");
        }

        var type = typeProperty.GetString();
        TilemapObject obj = type switch
        {
            "TestObject" => JsonSerializer.Deserialize<TestObject>(jsonObject.GetRawText(), options),
            _ => throw new JsonException($"Unknown Type: {type}")
        };

        return obj;
    }

    public override void Write(Utf8JsonWriter writer, TilemapObject value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Type", value.GetType().Name);
        foreach (var property in value.GetType().GetProperties())
        {
            var propertyValue = property.GetValue(value);
            writer.WritePropertyName(property.Name);
            JsonSerializer.Serialize(writer, propertyValue, options);
        }
        writer.WriteEndObject();
    }
}