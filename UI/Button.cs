using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PalmMapEditor.Core;

namespace PalmMapEditor.UI
{
    public class Button : UIObject
    {
        public bool Disabled { get; set; } = false;

        public float TextSize { get; set; } = 2f;
        public string Text { get; set; } = String.Empty;
        public Texture2D Texture { get; set; }

        public Color NormalColor { get; set; } = Color.White;
        public Color SelectedColor { get; set; } = Color.Silver;
        
        
        public Button(Rectangle target)
        {
            TargetRect = target;

            Texture = Globals.WhiteTexture;
        }
        
        public void Update()
        {
            Text = Text ?? string.Empty;

            if (TargetRect.Contains(InputManager.Mouse.Position) && InputManager.GetMouseButtonUp(0) && !Disabled)
                OnButtonClicked(null);
        }

        public void Draw()
        {
            bool selected = TargetRect.Contains(InputManager.Mouse.Position) && InputManager.Mouse.LeftButton == ButtonState.Pressed;
            Globals.SpriteBatch.Draw(Texture, TargetRect, selected && !Disabled ? SelectedColor : NormalColor);
            
            StringBuilder sb = new StringBuilder(Text);
            Vector2 origin = Globals.TextFont.MeasureString(sb.ToString()) / 2;
            Globals.SpriteBatch.DrawString(Globals.TextFont, sb.ToString(), TargetRect.Center.ToVector2(), Disabled ? Color.Gray : Color.Black, 0f, origin, TextSize, SpriteEffects.None, 0f);
        }
        
        public event EventHandler ButtonClicked;

        private void OnButtonClicked(EventArgs e)
        {
            ButtonClicked?.Invoke(this, e);
        }

        public Vector2 GetSizeOfText(string text)
        {
            return Globals.TextFont.MeasureString(text ?? string.Empty) * TextSize;
        }
    }
}
