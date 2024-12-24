using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using PalmMapEditor.Files;

namespace PalmMapEditor.Tilemaps;

public static class TilemapObjectLoader
{
    public static string ObjectNamespace { get; } = "PalmMapEditor.Tilemaps.Objects";
    public static Type BaseObjectType { get; } = typeof(TilemapObject);

    public static Type LoadObject()
    {
        string path = OpenFile.OpenDialog("Object Class (*.cs)\0*.cs\0All Files (*.*)\0*.*\0", "Load Tilemap Object");

        if (path == null || path == string.Empty)
            return null;

        string code = File.ReadAllText(path);

        // Regular expression to match class declarations
        string classPattern = @"\bclass\s+([A-Za-z_][A-Za-z0-9_]*)";

        // Find matches
        var matches = Regex.Matches(code, classPattern);

        // Display class names
        foreach (Match match in matches)
        {
            Type type = Type.GetType($"{ObjectNamespace}.{match.Groups[1].Value}");
            
            if(type == null || type.BaseType == null || type.BaseType != BaseObjectType)
            {
                Debug.WriteLine("[ERROR] Failed to load object file:  Unable to find valid TilemapObject class");
                break;
            }

            return Type.GetType($"{ObjectNamespace}.{match.Groups[1].Value}");
        }

        return null;
    }

    public static string[] GetPropertiesOfObject(Type type)
    {
        if (type == null)
            return new string[] { };

        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Select(P => P.Name).ToArray();
    }
}