using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PalmMapEditor.Core;

namespace PalmMapEditor.UI;

public class TilemapEditMenu : UIObject
{
    public bool Active { get; set; } = false;

    public int Width => Math.Max(1, WidthTextBox.Text == "" ? 1 : int.Parse(WidthTextBox.Text));
    public int Height => Math.Max(1, WidthTextBox.Text == "" ? 1 : int.Parse(HeightTextBox.Text));
    public string Name => NameTextBox.Text;
    public string TilesetName => TilesetTextBox.Text;
    public int TileSize => Math.Max(1, WidthTextBox.Text == "" ? 1 : int.Parse(TilesizeTextBox.Text));

    public TextBox TilesetTextBox { get; }
    public TextBox NameTextBox { get; }
    public TextBox TilesizeTextBox { get; }
    public TextBox WidthTextBox { get; }
    public TextBox HeightTextBox { get; }

    public Button DoneButton { get; }

    public TilemapEditMenu()
    {
        TargetRect = new Rectangle(
            Globals.VirtualGameSize.X / 3,
            25,
            Globals.VirtualGameSize.X / 3,
            Globals.VirtualGameSize.Y - 50
            );

        NameTextBox = new TextBox(Rectangle.Empty);
        NameTextBox.TargetRect = new Rectangle(
            Globals.VirtualGameSize.X / 2 - (int)Math.Floor(NameTextBox.GetSizeOfText("A very long name for a tilemap").X / 2),
            TargetRect.Top + 40,
            (int)Math.Floor(NameTextBox.GetSizeOfText("A very long name for a tilemap").X),
            50
            );
        NameTextBox.NormalColor = Color.LightGray;
        NameTextBox.MaxCharCount = "Very long name for tilemap".Length;

        TilesetTextBox = new TextBox(Rectangle.Empty);
        TilesetTextBox.TargetRect = new Rectangle(
            Globals.VirtualGameSize.X / 2 - (int)Math.Floor(TilesetTextBox.GetSizeOfText("A very long name for a tilemap").X / 2),
            TargetRect.Top + 150,
            (int)Math.Floor(TilesetTextBox.GetSizeOfText("A very long name for a tilemap").X),
            50
            );
        TilesetTextBox.NormalColor = Color.LightGray;
        TilesetTextBox.MaxCharCount = "Very long name for tilemap".Length;

        TilesizeTextBox = new TextBox(Rectangle.Empty);
        TilesizeTextBox.TargetRect = new Rectangle(
            Globals.VirtualGameSize.X / 2 - (int)Math.Floor(TilesizeTextBox.GetSizeOfText("1234567890").X / 2),
            TargetRect.Top + 260,
            (int)Math.Floor(TilesizeTextBox.GetSizeOfText("1234567890").X),
            50
            );
        TilesizeTextBox.NormalColor = Color.LightGray;
        TilesizeTextBox.OnlyNumbers = true;
        TilesizeTextBox.MaxCharCount = "1234567890".Length;

        WidthTextBox = new TextBox(Rectangle.Empty);
        WidthTextBox.TargetRect = new Rectangle(
            Globals.VirtualGameSize.X / 2 - (int)Math.Floor(WidthTextBox.GetSizeOfText("1234567890").X / 2),
            TargetRect.Top + 370,
            (int)Math.Floor(WidthTextBox.GetSizeOfText("1234567890").X),
            50
            );
        WidthTextBox.NormalColor = Color.LightGray;
        WidthTextBox.OnlyNumbers = true;
        WidthTextBox.MaxCharCount = "1234567890".Length;

        HeightTextBox = new TextBox(Rectangle.Empty);
        HeightTextBox.TargetRect = new Rectangle(
            Globals.VirtualGameSize.X / 2 - (int)Math.Floor(HeightTextBox.GetSizeOfText("1234567890").X / 2),
            TargetRect.Top + 480,
            (int)Math.Floor(HeightTextBox.GetSizeOfText("1234567890").X),
            50
            );
        HeightTextBox.NormalColor = Color.LightGray;
        HeightTextBox.OnlyNumbers = true;
        HeightTextBox.MaxCharCount = "1234567890".Length;

        DoneButton = new Button(Rectangle.Empty);
        DoneButton.TargetRect = new Rectangle(
            Globals.VirtualGameSize.X / 2 - (int)Math.Floor(DoneButton.GetSizeOfText("--Done--").X / 2),
            TargetRect.Bottom - 60,
            (int)Math.Floor(DoneButton.GetSizeOfText("--Done--").X),
            50
            );
        DoneButton.NormalColor = Color.LightGray;
        DoneButton.Text = "Done";
    }

    public void Update()
    {
        if (!Active) return;

        NameTextBox.UpdateInputs();
        TilesetTextBox.UpdateInputs();
        TilesizeTextBox.UpdateInputs();
        WidthTextBox.UpdateInputs();
        HeightTextBox.UpdateInputs();

        DoneButton.Update();
    }

    public void Draw()
    {
        if (!Active) return;

        Globals.SpriteBatch.Draw(Globals.WhiteTexture, TargetRect, Color.White);

        Vector2 origin;

        NameTextBox.Draw();
        origin = new Vector2(Globals.TextFont.MeasureString("Name").X / 2, 0);
        Globals.SpriteBatch.DrawString(Globals.TextFont, "Name", new Vector2(Globals.VirtualGameSize.X / 2, 30), Color.Black, 0f, origin, 1.8f, SpriteEffects.None, 0);

        TilesetTextBox.Draw();
        origin = new Vector2(Globals.TextFont.MeasureString("Tileset Name").X / 2, 0);
        Globals.SpriteBatch.DrawString(Globals.TextFont, "Tileset Name", new Vector2(Globals.VirtualGameSize.X / 2, 140), Color.Black, 0f, origin, 1.8f, SpriteEffects.None, 0);

        TilesizeTextBox.Draw();
        origin = new Vector2(Globals.TextFont.MeasureString("Tile Size").X / 2, 0);
        Globals.SpriteBatch.DrawString(Globals.TextFont, "Tile Size", new Vector2(Globals.VirtualGameSize.X / 2, 250), Color.Black, 0f, origin, 1.8f, SpriteEffects.None, 0);

        WidthTextBox.Draw();
        origin = new Vector2(Globals.TextFont.MeasureString("Width").X / 2, 0);
        Globals.SpriteBatch.DrawString(Globals.TextFont, "Width", new Vector2(Globals.VirtualGameSize.X / 2, 360), Color.Black, 0f, origin, 1.8f, SpriteEffects.None, 0);

        HeightTextBox.Draw();
        origin = new Vector2(Globals.TextFont.MeasureString("Height").X / 2, 0);
        Globals.SpriteBatch.DrawString(Globals.TextFont, "Height", new Vector2(Globals.VirtualGameSize.X / 2, 470), Color.Black, 0f, origin, 1.8f, SpriteEffects.None, 0);

        DoneButton.Draw();
    }
}