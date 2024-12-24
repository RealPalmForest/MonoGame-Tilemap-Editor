using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using PalmMapEditor.JsonConverters;

namespace PalmMapEditor.Tilemaps;

public class GameMapData
{
    public TilemapData[] Layers { get; set; }
    public TilemapObject[] Objects { get; set; }

    [JsonConstructor]
    public GameMapData(TilemapData[] layers, TilemapObject[] objects)
    {
        Layers = layers;
        Objects = objects;
    }

    public GameMapData(GameMap map)
    {
        Layers = map.Layers.Select(L => L.SaveTilemapData()).ToArray();
        Objects = map.Objects;
    }

    public void SaveToFile(string path)
    {
        File.WriteAllText(path, SaveToObfuscatedJson());
    }

    public string SaveToJson()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // Optional: Pretty-print JSON
            Converters = 
            { 
                new TilemapObjectJsonConverter(),
                new RectangleConverter(),
                new Vector2Converter(),
                new PointConverter()
            }
        };

        return JsonSerializer.Serialize(this, options);
    }

    public static GameMapData LoadFromJson(string content)
    {
        var options = new JsonSerializerOptions
        {
            Converters = 
            { 
                new TilemapObjectJsonConverter(),
                new RectangleConverter(),
                new Vector2Converter(),
                new PointConverter()
            }
        };
        
        return JsonSerializer.Deserialize<GameMapData>(content, options);
    }


    public string SaveToObfuscatedJson()
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(SaveToJson()));
    }

    public static GameMapData LoadFromObfuscatedJson(string content)
    {
        return LoadFromJson(Encoding.UTF8.GetString(Convert.FromBase64String(content)));
    }

    
    public static GameMapData LoadFromFile(string path)
    {
        if(!File.Exists(path))
        {
            Debug.WriteLine("[ERROR] Failed to load map file at path:  " + path);
            return null;
        }

        return LoadFromObfuscatedJson(File.ReadAllText(path));
    }
}