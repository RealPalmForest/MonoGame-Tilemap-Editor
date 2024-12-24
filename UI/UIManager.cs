using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PalmMapEditor.Core;
using PalmMapEditor.Tilemaps;

namespace PalmMapEditor.UI;

public static class UIManager
{
    public static bool ShowUI { get; set; } = true;
    
    public static void Load()
    {
        
    }

    public static void Update()
    {
        
    }
    
    public static void DrawUI()
    {
        if (!ShowUI) return;
        
        Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);
        TilemapEditor.DrawToWorld();
        Globals.SpriteBatch.End();

        Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: GameManager.GameCamera.TranslationMatrix, blendState: BlendState.NonPremultiplied);
        TilemapEditor.WhenThisIsDrawnTranslationWillBeAppliedByAMatrix();
        Globals.SpriteBatch.End();

        Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);
        TilemapEditor.DrawToScreen();
        Globals.SpriteBatch.End();
    }
}