using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace PalmMapEditor.JsonConverters;

public class RectangleConverter : JsonConverter<Rectangle>
{
    public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;
        var x = jsonObject.GetProperty("X").GetInt32();
        var y = jsonObject.GetProperty("Y").GetInt32();
        var width = jsonObject.GetProperty("Width").GetInt32();
        var height = jsonObject.GetProperty("Height").GetInt32();
        return new Rectangle(x, y, width, height);
    }

    public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteNumber("Width", value.Width);
        writer.WriteNumber("Height", value.Height);
        writer.WriteEndObject();
    }
}
