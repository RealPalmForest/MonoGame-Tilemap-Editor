using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PalmMapEditor.Core;

public static class Globals
{
    public static SpriteBatch SpriteBatch { get; set; }
    public static GraphicsDevice GraphicsDevice { get; set; }
    public static SpriteFont NumFont { get; set; }
    public static SpriteFont TextFont { get; set; }
    public static ContentManager Content { get; set; }
    public static Random Random { get; private set; } = new Random();

    public static Texture2D WhiteTexture { get; set; }
    public static Point VirtualGameSize => GraphicsDevice.Viewport.Bounds.Size;

    public static GameTime GameTime { get; set; }
    public static float DeltaTime => (float)GameTime.ElapsedGameTime.TotalSeconds;


    public static bool IsTyping { get; set; } = false;


    public static T[,] ResizeArray<T>(T[,] original, int rows, int cols)
    {
        var newArray = new T[rows, cols];
        int minRows = Math.Min(rows, original.GetLength(0));
        int minCols = Math.Min(cols, original.GetLength(1));
        for (int i = 0; i < minRows; i++)
            for (int j = 0; j < minCols; j++)
                newArray[i, j] = original[i, j];
        return newArray;
    }
}