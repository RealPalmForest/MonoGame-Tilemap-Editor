using System.Text.Json.Serialization;
using PalmMapEditor.JsonConverters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PalmMapEditor.Core;

namespace PalmMapEditor.Tilemaps;

[JsonConverter(typeof(TilemapObjectJsonConverter))]
public abstract class TilemapObject
{
    // Any properties that you create will show up when adding the object to a map
    public Rectangle TargetRectangle { get; set; } = Rectangle.Empty;

    protected Texture2D texture;
    protected Rectangle sourceRect;
    protected Point assignedTile;

    public virtual void Update()
    {

    }

    public virtual void Draw()
    {
        Globals.SpriteBatch.Draw(texture, TargetRectangle, sourceRect, Color.White);
    }

    public virtual void Load()
    {
        // this.texture = ...;  Set texture here
    }

    public virtual void Move(Rectangle targetRect, Point assignedTile)
    {
        TargetRectangle = targetRect;
        this.assignedTile = assignedTile;
    }

    public Point GetAssignedTile()
    {
        return assignedTile;
    } 
}