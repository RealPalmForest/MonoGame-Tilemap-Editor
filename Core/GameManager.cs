using PalmMapEditor.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PalmMapEditor.Tilemaps;

namespace PalmMapEditor.Core;

public static class GameManager
{
    public static Camera GameCamera;
    public static Vector2 Movement;


    private static float moveSpeed = 400f;

    public static void Load()
    {
        TilemapEditor.Load();
        TilemapEditor.Enabled = true;
        TilemapEditor.TileSelectorEnabled = true;

        UIManager.Load();

        Movement = TilemapEditor.SelectedTilemap.Bounds.Center.ToVector2();
        GameCamera = new Camera(Movement);
    }

    public static void Update()
    {
        Vector2 velocity = new Vector2(InputManager.GetDirAxis("X"), InputManager.GetDirAxis("Y"));
        Movement += velocity * moveSpeed * Globals.DeltaTime;

        GameCamera.Target = Movement;

        TilemapEditor.Update();

        UIManager.Update();
    }

    public static void Draw()
    {
        Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: GameCamera.TranslationMatrix, blendState: BlendState.NonPremultiplied);
        Globals.SpriteBatch.End();

        UIManager.DrawUI();
    }
}