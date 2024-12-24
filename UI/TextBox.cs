using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PalmMapEditor.Core;

namespace PalmMapEditor.UI
{
    public class TextBox : UIObject
    {
        public float TextSize { get; set; } = 2f;

        public bool IsSelected
        {
            get { return selected; }
            set
            {
                selected = value;
                Globals.IsTyping = selected;
            }
        }

        public bool OnlyNumbers { get; set; } = false;

        public string Placeholder { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int MaxCharCount { get; set; } = 999;
        public Texture2D Texture { get; set; }

        public Color NormalColor { get; set; } = Color.White;
        public Color SelectedColor { get; set; } = Color.Silver;

        private bool selected = false;


        public TextBox(Rectangle target)
        {
            TargetRect = target;

            Texture = new Texture2D(Globals.GraphicsDevice, 1, 1);
            Texture.SetData(new Color[] { Color.White });
        }

        public void UpdateInputs()
        {
            Text = Text ?? string.Empty;

            if (InputManager.GetMouseButtonUp(0))
                IsSelected = TargetRect.Contains(InputManager.Mouse.Position);

            if (!IsSelected)
                return;

            Globals.IsTyping = true;
            
            if (InputManager.GetKeyDown(Keys.Back) && Text.Length > 0) Text = Text[..^1];

            if (InputManager.GetKeyDown(Keys.Enter))
            {
                IsSelected = false;
                OnTextConfirmed(null);
            }

            if (InputManager.GetKeyDown(Keys.Delete)) Text = String.Empty;


            if(Text.Length == MaxCharCount)
                return;

            if (OnlyNumbers)
            {
                // Digits 0-9
                if (InputManager.GetKeyDown(Keys.D1)) Text += "1";
                if (InputManager.GetKeyDown(Keys.D2)) Text += "2";
                if (InputManager.GetKeyDown(Keys.D3)) Text += "3";
                if (InputManager.GetKeyDown(Keys.D4)) Text += "4";
                if (InputManager.GetKeyDown(Keys.D5)) Text += "5";
                if (InputManager.GetKeyDown(Keys.D6)) Text += "6";
                if (InputManager.GetKeyDown(Keys.D7)) Text += "7";
                if (InputManager.GetKeyDown(Keys.D8)) Text += "8";
                if (InputManager.GetKeyDown(Keys.D9)) Text += "9";
                if (InputManager.GetKeyDown(Keys.D0)) Text += "0";

                return;
            }

            bool capital = Keyboard.GetState().CapsLock || Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift);

            // Letters A-Z
            if (InputManager.GetKeyDown(Keys.A)) Text += capital ? "A" : "a";
            if (InputManager.GetKeyDown(Keys.B)) Text += capital ? "B" : "b";
            if (InputManager.GetKeyDown(Keys.C)) Text += capital ? "C" : "c";
            if (InputManager.GetKeyDown(Keys.D)) Text += capital ? "D" : "d";
            if (InputManager.GetKeyDown(Keys.E)) Text += capital ? "E" : "e";
            if (InputManager.GetKeyDown(Keys.F)) Text += capital ? "F" : "f";
            if (InputManager.GetKeyDown(Keys.G)) Text += capital ? "G" : "g";
            if (InputManager.GetKeyDown(Keys.H)) Text += capital ? "H" : "h";
            if (InputManager.GetKeyDown(Keys.I)) Text += capital ? "I" : "i";
            if (InputManager.GetKeyDown(Keys.J)) Text += capital ? "J" : "j";
            if (InputManager.GetKeyDown(Keys.K)) Text += capital ? "K" : "k";
            if (InputManager.GetKeyDown(Keys.L)) Text += capital ? "L" : "l";
            if (InputManager.GetKeyDown(Keys.M)) Text += capital ? "M" : "m";
            if (InputManager.GetKeyDown(Keys.N)) Text += capital ? "N" : "n";
            if (InputManager.GetKeyDown(Keys.O)) Text += capital ? "O" : "o";
            if (InputManager.GetKeyDown(Keys.P)) Text += capital ? "P" : "p";
            if (InputManager.GetKeyDown(Keys.Q)) Text += capital ? "Q" : "q";
            if (InputManager.GetKeyDown(Keys.R)) Text += capital ? "R" : "r";
            if (InputManager.GetKeyDown(Keys.S)) Text += capital ? "S" : "s";
            if (InputManager.GetKeyDown(Keys.T)) Text += capital ? "T" : "t";
            if (InputManager.GetKeyDown(Keys.U)) Text += capital ? "U" : "u";
            if (InputManager.GetKeyDown(Keys.V)) Text += capital ? "V" : "v";
            if (InputManager.GetKeyDown(Keys.W)) Text += capital ? "W" : "w";
            if (InputManager.GetKeyDown(Keys.X)) Text += capital ? "X" : "x";
            if (InputManager.GetKeyDown(Keys.Y)) Text += capital ? "Y" : "y";
            if (InputManager.GetKeyDown(Keys.Z)) Text += capital ? "Z" : "z";

            // Digits 0-9 with special characters when "capital" is true
            if (InputManager.GetKeyDown(Keys.D1)) Text += capital ? "!" : "1";
            if (InputManager.GetKeyDown(Keys.D2)) Text += capital ? "@" : "2";
            if (InputManager.GetKeyDown(Keys.D3)) Text += capital ? "#" : "3";
            if (InputManager.GetKeyDown(Keys.D4)) Text += capital ? "$" : "4";
            if (InputManager.GetKeyDown(Keys.D5)) Text += capital ? "%" : "5";
            if (InputManager.GetKeyDown(Keys.D6)) Text += capital ? "^" : "6";
            if (InputManager.GetKeyDown(Keys.D7)) Text += capital ? "&" : "7";
            if (InputManager.GetKeyDown(Keys.D8)) Text += capital ? "*" : "8";
            if (InputManager.GetKeyDown(Keys.D9)) Text += capital ? "(" : "9";
            if (InputManager.GetKeyDown(Keys.D0)) Text += capital ? ")" : "0";

            // Special characters with shifted versions
            if (InputManager.GetKeyDown(Keys.Space)) Text += " ";
            if (InputManager.GetKeyDown(Keys.OemComma)) Text += capital ? "<" : ",";
            if (InputManager.GetKeyDown(Keys.OemPeriod)) Text += capital ? ">" : ".";
            if (InputManager.GetKeyDown(Keys.OemQuestion)) Text += capital ? "?" : "/";
            if (InputManager.GetKeyDown(Keys.OemSemicolon)) Text += capital ? ":" : ";";
            if (InputManager.GetKeyDown(Keys.OemQuotes)) Text += capital ? "\"" : "'";
            if (InputManager.GetKeyDown(Keys.OemOpenBrackets)) Text += capital ? "{" : "[";
            if (InputManager.GetKeyDown(Keys.OemCloseBrackets)) Text += capital ? "}" : "]";
            if (InputManager.GetKeyDown(Keys.OemPipe)) Text += capital ? "\\" : "|";
            if (InputManager.GetKeyDown(Keys.OemMinus)) Text += capital ? "_" : "-";
            if (InputManager.GetKeyDown(Keys.OemPlus)) Text += capital ? "=" : "+";
            if (InputManager.GetKeyDown(Keys.OemTilde)) Text += capital ? "`" : "~";
        }

        public void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, TargetRect, IsSelected ? SelectedColor : NormalColor);

            if(Text == string.Empty)
            {
                StringBuilder sb = new StringBuilder(Placeholder);
                Vector2 origin = Globals.TextFont.MeasureString(sb.ToString()) / 2;

                Globals.SpriteBatch.DrawString(Globals.TextFont, sb.ToString(), TargetRect.Center.ToVector2(), Color.DarkGray, 0f, origin, TextSize, SpriteEffects.None, 0f);
            }
            else 
            {
                StringBuilder sb = new StringBuilder(Text);
                Vector2 origin = Globals.TextFont.MeasureString(sb.ToString()) / 2;

                Globals.SpriteBatch.DrawString(Globals.TextFont, sb.ToString(), TargetRect.Center.ToVector2(), Color.Black, 0f, origin, TextSize, SpriteEffects.None, 0f);
            }
        }

        public event EventHandler TextConfirmed;

        private void OnTextConfirmed(EventArgs e)
        {
            TextConfirmed?.Invoke(this, e);
        }

        public Vector2 GetSizeOfText(string text)
        {
            return Globals.TextFont.MeasureString(text) * TextSize;
        }
    }
}
