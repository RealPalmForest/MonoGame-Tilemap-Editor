using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PalmMapEditor.Core;

namespace PalmMapEditor.Tilemaps.Objects;

public class TestObject : TilemapObject
{
    public string STRING_PROP_WOW { get; set; }
    public int WoahThisIsANumberYAY { get; set; }
    public Vector2 DAmn_A_VECTOR2 { get; set; }
    public Point popint { get; set; }

    public override void Load()
    {
        texture = Globals.Content.Load<Texture2D>("Tilesets/grassTileset");
        sourceRect = new Rectangle(0, 0, 16, 16);
    }
}