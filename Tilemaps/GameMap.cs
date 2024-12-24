using System.Linq;

namespace PalmMapEditor.Tilemaps;

public class GameMap
{
    public Tilemap[] Layers { get; }
    public TilemapObject[] Objects { get; }

    public GameMap(Tilemap[] layers, TilemapObject[] objects)
    {
        Layers = layers;
        Objects = objects;
    }

    public GameMap(GameMapData data)
    {
        Layers = data.Layers.Select(L => new Tilemap(L)).ToArray();
        Objects = data.Objects;
    }

    public void Update()
    {
        foreach(TilemapObject obj in Objects)
        {
            obj.Update();
        }
    }

    public void Draw()
    {
        foreach(Tilemap layer in Layers)
        {
            layer.Draw();
        }

        foreach(TilemapObject obj in Objects)
        {
            obj.Draw();
        }
    }
}