namespace PalmMapEditor.Tilemaps;

public class TilemapData
{
    public string Name { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }
    public int TileSize { get; set; }

    public string TilesetName { get; set; }

    public int[] Tiles { get; set; }
}