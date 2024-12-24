using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using PalmMapEditor.Tilemaps;
using PalmMapEditor.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PalmMapEditor.Core;

namespace PalmMapEditor.UI;

public class ObjectEditMenu : UIObject
{
    public Point SelectedPointOnMap { get; set; }
    public Type ObjectType { get; set; }

    public bool Shown { get; set; } = false;

    public Button DoneButton { get; private set; }

    public Dictionary<string, TextBox> Properties { get; private set; } = new Dictionary<string, TextBox>();

    public ObjectEditMenu()
    {
        TargetRect = new Rectangle(
            Globals.VirtualGameSize.X / 3,
            25,
            Globals.VirtualGameSize.X / 3,
            Globals.VirtualGameSize.Y - 50
            );

        DoneButton = new Button(Rectangle.Empty);
        DoneButton.TargetRect = new Rectangle(
            Globals.VirtualGameSize.X / 2 - (int) Math.Floor(DoneButton.GetSizeOfText("--Done--").X / 2),
            TargetRect.Bottom - 60,
            (int) Math.Floor(DoneButton.GetSizeOfText("--Done--").X),
            50
            );
        DoneButton.NormalColor = Color.LightGray;
        DoneButton.Text = "Done";
    }

    public void Update()
    {
        if (!Shown)
            return;

        for (int i = 0; i < Properties.Count; i++)
        {
            Properties.ElementAt(i).Value.UpdateInputs();
        }

        DoneButton.Update();
    }

    public void Draw()
    {
        if (!Shown)
            return;

        Globals.SpriteBatch.Draw(Globals.WhiteTexture, TargetRect, Color.White);

        for (int i = 0; i < Properties.Count; i++)
        {
            Globals.SpriteBatch.DrawString(
                Globals.TextFont,
                Properties.ElementAt(i).Key,
                new Vector2(TargetRect.X + TargetRect.Width / 2, 60 + i * 110),
                Color.Black,
                0f,
                Globals.TextFont.MeasureString(Properties.ElementAt(i).Key) / 2,
                2f,
                SpriteEffects.None,
                0f
                );

            Properties.ElementAt(i).Value.Draw();
        }

        DoneButton.Draw();
    }

    /// <summary>
    /// Creates an empty textbox for each parameter of the object type
    /// </summary>
    /// <param name="objectType">Type of TilemapObject to grab parameters from</param>
    public void CreatePropertyTextboxes(Type objectType)
    {
        ObjectType = objectType;

        Properties.Clear();

        for (int i = 0; i < objectType.GetProperties().Length; i++)
        {
            PropertyInfo property = objectType.GetProperties()[i];

            TextBox textBox = new TextBox(new Rectangle(TargetRect.X + 10, i * 110 + 80, TargetRect.Width - 20, 50));
            textBox.NormalColor = Color.LightGray;

            if (property.PropertyType == typeof(int))
                textBox.OnlyNumbers = true;

            Properties.Add(property.Name, textBox);
        }
    }

    /// <summary>
    /// Creates a textbox for each parameter of the provided object and fills it with the parameter value
    /// </summary>
    /// <param name="obj">TilemapObject from which to grab the parameters and values from</param>
    public void FillPropertyTextboxes(TilemapObject obj)
    {
        ObjectType = obj.GetType();

        Properties.Clear();

        for (int i = 0; i < ObjectType.GetProperties().Length; i++)
        {
            PropertyInfo property = ObjectType.GetProperties()[i];

            TextBox textBox = new TextBox(new Rectangle(TargetRect.X + 10, i * 110 + 80, TargetRect.Width - 20, 50));
            textBox.NormalColor = Color.LightGray;

            string convertedString = property.GetValue(obj).ToString();

            Debug.WriteLine(property.PropertyType);

            if(property.PropertyType == typeof(Rectangle))
                convertedString = convertedString.Replace(':', ',').Replace("Width", "").Replace("Height", "").Replace("X", "").Replace("Y", "").Replace("{", "").Replace("}", "").Replace(" ", "")[1..];
            else if(property.PropertyType == typeof(Vector2) || property.PropertyType == typeof(Point))
                convertedString = convertedString.Replace(':', ',').Replace("X", "").Replace("Y", "").Replace("{", "").Replace("}", "").Replace(" ", "")[1..];

            textBox.Text = convertedString;

            if (property.PropertyType == typeof(int))
                textBox.OnlyNumbers = true;

            Properties.Add(property.Name, textBox);
        }
    }
}