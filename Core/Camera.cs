using Microsoft.Xna.Framework;

namespace PalmMapEditor.Core;

public class Camera
{
    public Vector2 Translation => new Vector2(TranslationMatrix.Translation.X, TranslationMatrix.Translation.Y);
    public Matrix TranslationMatrix => CalculateTranslation();
    public Vector2 Target { get; set; }

    public Camera(Vector2 target)
    {
        Target = target;
    }
    
    private Matrix CalculateTranslation()
    {
        var dx = Globals.VirtualGameSize.X / 2 - Target.X;
        var dy = Globals.VirtualGameSize.Y / 2 - Target.Y;
        return Matrix.CreateTranslation(dx, dy, 0f);
    }
}