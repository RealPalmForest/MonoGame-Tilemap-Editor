using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PalmMapEditor.Core;

namespace PalmMapEditor.Tilemaps;

public class Tilemap
{
    public string Name { get; set; }

    /// <summary>
    /// Width of tileset in tiles
    /// </summary>
    public int TilesetWidth => (tileset ?? new Texture2D(Globals.GraphicsDevice, TileSize, TileSize)).Width / TileSize;

    /// <summary>
    /// Height of tileset in tiles
    /// </summary>
    public int TilesetHeight => (tileset ?? new Texture2D(Globals.GraphicsDevice, TileSize, TileSize)).Height / TileSize;

    public int Width { get; set; }
    public int Height { get; set; }
    public int TileSize { get; set; }

    public string TilesetName
    {
        get { return tilesetName; }
        set
        {
            try
            {
                tileset = Globals.Content.Load<Texture2D>(value);
            }
            catch (Exception e)
            {
                if (tilesetName == null)
                    return;

                tileset = Globals.Content.Load<Texture2D>(tilesetName);
                Debug.WriteLine(e.Message);
                return;
            }

            tilesetName = value;

            // Reload tile selector to show the new tileset
            if (TilemapEditor.SelectedTilemap == this)
                TilemapEditor.ReloadTileSelector();
        }
    }

    public int[,] Tiles { get; set; }
    public float Scale { get; set; } = 4f;
    public Vector2 Offset { get; set; } = Vector2.Zero;

    public int WidthInPixels => (int)Math.Floor(Width * TileSize * Scale);
    public int HeightInPixels => (int)Math.Floor(Height * TileSize * Scale);

    public Rectangle Bounds => new Rectangle(Offset.ToPoint(), new Point((int) Math.Floor(Width * TileSize * Scale), (int) Math.Floor(Height * TileSize * Scale)));

    private Texture2D tileset;
    private string tilesetName;

    public Tilemap(int width, int height, int tileSize, string tilesetName, string name = "New Tilemap")
    {
        Width = width;
        Height = height;
        TileSize = tileSize;
        TilesetName = tilesetName;
        Tiles = new int[Width, Height];

        Name = name;
    }

    public Tilemap(TilemapData data)
    {
        Load(data);
    }


    public bool IsTileInBounds(Point position) { return position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height; }

    public void SetTileAt(Point position, int tile)
    {
        if (IsTileInBounds(position))
            Tiles[position.X, position.Y] = tile;
    }
    public int GetTileAt(Point position) { return Tiles[position.X, position.Y]; }

    public void Draw()
    {
        if (tileset == null)
        {
            Debug.WriteLine("[ERROR] Failed to draw tilemap:  Selected tileset is null");
            return;
        }

        // Go through each tile in the 2D array and draw it from the tileset
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int tileId = Tiles[x, y];

                if (tileId > 0 && tileId <= TilesetWidth * TilesetHeight)
                {
                    Rectangle source = new Rectangle((tileId - 1) % TilesetWidth * TileSize, (tileId - 1) / TilesetWidth * TileSize, TileSize, TileSize);
                    Rectangle target = new Rectangle(
                        (int)Math.Floor(Offset.X + x * TileSize * Scale),
                        (int)Math.Floor(Offset.Y + y * TileSize * Scale),
                        (int)Math.Floor(TileSize * Scale),
                        (int)Math.Floor(TileSize * Scale)
                    );
                    Globals.SpriteBatch.Draw(tileset, target, source, Color.White);
                }
            }
        }
    }

    public void Load(TilemapData data)
    {
        Name = data.Name;

        Width = data.Width;
        Height = data.Height;
        TileSize = data.TileSize;
        TilesetName = data.TilesetName;

        Tiles = new int[Width, Height];

        // Convert the 1D tile array of the provided data to a 2D array tile array
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tiles[x, y] = data.Tiles[y * Width + x];
            }
        }
    }

    public TilemapData SaveTilemapData()
    {
        int[] allTiles = new int[Width * Height];

        // Convert 2D tile array into a 1D tile array for the TilemapData
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                allTiles[y * Width + x] = Tiles[x, y];
            }
        }

        return new TilemapData()
        {
            Name = Name,
            Width = Width,
            Height = Height,
            TileSize = TileSize,
            TilesetName = TilesetName,
            Tiles = allTiles
        };
    }

    public Point GetTileAtWorldPosition(Vector2 pos)
    {
        pos = (pos - Offset - GameManager.GameCamera.Translation) / (TileSize * Scale);
        return new Point((int)Math.Floor(pos.X), (int)Math.Floor(pos.Y));
    }

    public Point GetTileAtScreenPosition(Vector2 pos)
    {
        pos = (pos - Offset) / (TileSize * Scale);
        return new Point((int)Math.Floor(pos.X), (int)Math.Floor(pos.Y));
    }

    public Vector2 GetScreenPositionOfTile(Point pos)
    {
        return new Vector2(pos.X * TileSize * Scale + Offset.X, pos.Y * TileSize * Scale + Offset.Y);
    }
    public Vector2 GetWorldPositionOfTile(Point pos)
    {
        return new Vector2(pos.X * TileSize * Scale + Offset.X, pos.Y * TileSize * Scale + Offset.Y) + GameManager.GameCamera.Translation;
    }





    /// <summary>
    /// Returns a list of points representing a line from (startX, startY) to (endX, endY) using Bresenham's Line Algorithm
    /// </summary>
    /// <param name="startTile">The starting tile coordinate</param>
    /// <param name="endTile">The ending tile coordinate</param>
    /// <returns>A list of points forming a line from the start to the end position.</returns>
    public List<(int, int)> GetLine(Point startTile, Point endTile)
    {
        List<(int, int)> line = new List<(int, int)>();

        // int startX = startPos.X;
        // int startY = startPos.Y;
        // int endX = endPos.X;
        // int endY = endPos.Y;

        int changeX = Math.Abs(endTile.X - startTile.X);
        int changeY = Math.Abs(endTile.Y - startTile.Y);
        int dirX = startTile.X < endTile.X ? 1 : -1;
        int dirY = startTile.Y < endTile.Y ? 1 : -1;
        int err = changeX - changeY;

        while (true)
        {
            // Include the current point
            line.Add((startTile.X, startTile.Y));

            // Check if the end point has been reached
            if (startTile.X == endTile.X && startTile.Y == endTile.Y)
                break;

            // Increase the X and Y in the appropriate directions
            int e2 = 2 * err;
            if (e2 > -changeY)
            {
                err -= changeY;
                startTile.X += dirX;
            }

            if (e2 < changeX)
            {
                err += changeX;
                startTile.Y += dirY;
            }
        }

        return line;
    }
}