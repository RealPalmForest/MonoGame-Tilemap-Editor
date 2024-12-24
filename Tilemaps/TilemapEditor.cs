using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PalmMapEditor.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PalmMapEditor.Files;
using PalmMapEditor.Core;

namespace PalmMapEditor.Tilemaps;

public static class TilemapEditor
{
    public static bool Enabled { get; set; } = false;
    public static bool ShowTileIds { get; set; } = false;

    public static int SelectedTile { get; set; } = 1;
    public static Tilemap SelectedTilemap
    {
        get => selectedTilemap;
        set
        {
            selectedTilemap = value;
            outline = GetTileOutline();
            mapBorder = GetMapBorder();
        }
    }

    public static List<Tilemap> Layers { get; set; } = new List<Tilemap>();
    private static List<Button> layerButtons = new List<Button>();
    private static Button newLayerButton;

    private static Button editLayerButton;
    private static Button deleteLayerButton;

    private static TilemapEditMenu optionsMenu;

    private static Texture2D mapBorder;
    private static Texture2D outline;
    private static Tilemap selectedTilemap;


    public static bool TileSelectorEnabled
    {
        get => tileSelectorEnabled;
        set
        {
            tileSelectorEnabled = value;
            ReloadTileSelector();
        }
    }


    private static bool tileSelectorEnabled = false;
    private static Tilemap tileSelector;


    private static List<UIObject> interactableUI = new List<UIObject>();
    private static bool isMouseOverInteractableUI = false;

    private static List<Tilemap> hiddenLayers = new List<Tilemap>();

    private static bool showTooltips = true;

    private static string[] tooltips =
    {
        "[T]              ---   Toggle Editor",
        "[I]              ---   Show / Hide tile IDs",
        "[R]              ---   Reset map",
        "[WASD]       ---   Pan",
        "[0-9]          ---   Select tile 0 - 9",
        "[E/Q]          ---   Next / Previous tile",
        "[Ctrl+S]   ---   Save map",
        "[Ctrl+L]   ---   Load map",
        "[F11]          ---   Fullscreen",
        "[F8]            ---   Toggle these tooltips",
        "[Lclick]  ---   Place tiles",
        "[Rclick]  ---   Erase tiles",
        "[Mclick]  ---   Place / Edit object"
    };

    private static string tilesetFolder = "Tilesets/";

    private static List<TilemapObject> objects = new List<TilemapObject>();
    private static ObjectEditMenu objectEditMenu;

    public static void Load()
    {
        Layers.Add(new Tilemap(10, 10, 16, "grassTilese", "Layer 1"));
        SelectedTilemap = Layers[0];

        LoadUI();
    }

    public static void LoadUI()
    {
        newLayerButton = new Button(Rectangle.Empty);
        newLayerButton.TargetRect = new Rectangle(
            Globals.VirtualGameSize.X - newLayerButton.GetSizeOfText("New").ToPoint().X - 40,
            Globals.VirtualGameSize.Y - 50 - 70 * Layers.Count,
            newLayerButton.GetSizeOfText("New").ToPoint().X + 25,
            40);
        newLayerButton.Text = "New";
        newLayerButton.ButtonClicked += (sender, e) =>
        {

            if (Layers.Count < 10)
                Layers.Add(new Tilemap(10, 10, 16, "grassTileset", $"Layer {Layers.Count + 1}"));
        };
        interactableUI.Add(newLayerButton);

        editLayerButton = new Button(Rectangle.Empty);
        editLayerButton.TargetRect = new Rectangle(
            Globals.VirtualGameSize.X / 2 + 30,
            Globals.VirtualGameSize.Y - 60,
            (int)Math.Floor(editLayerButton.GetSizeOfText("Edit Layer").X + 20),
            50
        );
        editLayerButton.Text = "Edit Layer";
        editLayerButton.ButtonClicked += (sender, e) =>
        {
            OpenLayerEditMenu();
        };
        interactableUI.Add(editLayerButton);

        deleteLayerButton = new Button(Rectangle.Empty);
        deleteLayerButton.TargetRect = new Rectangle(
            (int)Math.Floor(Globals.VirtualGameSize.X / 2 - deleteLayerButton.GetSizeOfText("Delete Layer").X - 30),
            Globals.VirtualGameSize.Y - 60,
            (int)Math.Floor(deleteLayerButton.GetSizeOfText("Delete Layer").X + 20),
            50
        );
        deleteLayerButton.Text = "Delete Layer";
        deleteLayerButton.ButtonClicked += (sender, e) =>
        {
            if (Layers.Count > 1)
            {
                int newSelectedIndex = Layers.IndexOf(SelectedTilemap) - 1;
                if (newSelectedIndex < 0) newSelectedIndex = newSelectedIndex + 1;

                Layers.Remove(SelectedTilemap);
                SelectedTilemap = Layers[newSelectedIndex];
            }
        };
        interactableUI.Add(deleteLayerButton);

        optionsMenu = new TilemapEditMenu();
        optionsMenu.DoneButton.ButtonClicked += (sender, e) =>
        {
            optionsMenu.Active = false;

            selectedTilemap.TileSize = optionsMenu.TileSize;
            selectedTilemap.TilesetName = tilesetFolder + "/" + optionsMenu.TilesetName;
            selectedTilemap.Name = optionsMenu.Name;

            selectedTilemap.Width = optionsMenu.Width;
            selectedTilemap.Height = optionsMenu.Height;
            selectedTilemap.Tiles = Globals.ResizeArray<int>(selectedTilemap.Tiles, selectedTilemap.Width, selectedTilemap.Height);

            mapBorder = GetMapBorder();

            // Refresh layer buttons to show the new name of the layer
            layerButtons.Clear();
        };

        layerButtons.Clear();
        ReloadTileSelector();

        objectEditMenu = new ObjectEditMenu();
        objectEditMenu.DoneButton.ButtonClicked += (sender, e) =>
        {
            TilemapObject newObj = Activator.CreateInstance(objectEditMenu.ObjectType) as TilemapObject;

            if (newObj != null && newObj.GetType() == objectEditMenu.ObjectType)
            {
                for (int i = 0; i < objectEditMenu.Properties.Count; i++)
                {
                    var property = objectEditMenu.ObjectType.GetProperty(objectEditMenu.Properties.ElementAt(i).Key);

                    if (property.PropertyType == typeof(string))
                        property?.SetValue(newObj, objectEditMenu.Properties.ElementAt(i).Value.Text);
                    else if (property.PropertyType == typeof(int))
                        property?.SetValue(newObj, int.Parse(objectEditMenu.Properties.ElementAt(i).Value.Text == string.Empty ? "0" : objectEditMenu.Properties.ElementAt(i).Value.Text));
                    else if (property.PropertyType == typeof(bool))
                        property?.SetValue(newObj, Convert.ToBoolean(objectEditMenu.Properties.ElementAt(i).Value.Text.ToLower()));
                    else if (property.PropertyType == typeof(Point) || property.PropertyType == typeof(Vector2))
                    {
                        try
                        {
                            string[] nums = objectEditMenu.Properties.ElementAt(i).Value.Text.Split(',');

                            if (nums.Length < 2)
                                property?.SetValue(newObj, property.PropertyType == typeof(Point) ? Point.Zero : Vector2.Zero);
                            else
                                property?.SetValue(newObj, property.PropertyType == typeof(Vector2) ?
                                new Vector2(float.Parse(nums[0].Trim()), float.Parse(nums[1].Trim())) :
                                new Point(int.Parse(nums[0].Trim()), int.Parse(nums[1].Trim())));
                        }
                        catch { property?.SetValue(newObj, property.PropertyType == typeof(Vector2) ? Vector2.Zero : Point.Zero); }
                    }
                    else if (property.PropertyType == typeof(Rectangle))
                    {
                        try
                        {
                            string[] nums = objectEditMenu.Properties.ElementAt(i).Value.Text.Split(',');

                            if (nums.Length < 4)
                                property?.SetValue(newObj, Rectangle.Empty);
                            else
                                property?.SetValue(newObj, new Rectangle(int.Parse(nums[0].Trim()), int.Parse(nums[1].Trim()), int.Parse(nums[2].Trim()), int.Parse(nums[3].Trim())));
                        }
                        catch { property?.SetValue(newObj, Rectangle.Empty); }
                    }

                    newObj.Move(newObj.TargetRectangle, objectEditMenu.SelectedPointOnMap);
                    newObj.Load();

                    foreach (TilemapObject obj in objects)
                    {
                        if (obj.GetAssignedTile() == objectEditMenu.SelectedPointOnMap)
                        {
                            objects.Remove(obj);
                            break;
                        }
                    }

                    objects.Add(newObj);
                }
            }

            objectEditMenu.Shown = false;
        };
    }

    public static void Update()
    {
        if (!Globals.IsTyping && InputManager.GetKeyDown(Keys.T))
            Enabled = !Enabled;

        if (InputManager.GetKeyDown(Keys.F8))
            showTooltips = !showTooltips;

        if (!Enabled) return;

        if (optionsMenu.Active)
        {
            optionsMenu.Update();
            return;
        }

        if (InputManager.GetKeyDown(Keys.E) && !Globals.IsTyping) SelectedTile++;
        if (InputManager.GetKeyDown(Keys.Q) && !Globals.IsTyping) SelectedTile--;

        if (!Globals.IsTyping && InputManager.GetKeyDown(Keys.D1)) SelectedTile = 1;
        if (!Globals.IsTyping && InputManager.GetKeyDown(Keys.D2)) SelectedTile = 2;
        if (!Globals.IsTyping && InputManager.GetKeyDown(Keys.D3)) SelectedTile = 3;
        if (!Globals.IsTyping && InputManager.GetKeyDown(Keys.D4)) SelectedTile = 4;
        if (!Globals.IsTyping && InputManager.GetKeyDown(Keys.D5)) SelectedTile = 5;
        if (!Globals.IsTyping && InputManager.GetKeyDown(Keys.D6)) SelectedTile = 6;
        if (!Globals.IsTyping && InputManager.GetKeyDown(Keys.D7)) SelectedTile = 7;
        if (!Globals.IsTyping && InputManager.GetKeyDown(Keys.D8)) SelectedTile = 8;
        if (!Globals.IsTyping && InputManager.GetKeyDown(Keys.D9)) SelectedTile = 9;

        SelectedTile = Math.Clamp(SelectedTile, 0, SelectedTilemap.TilesetWidth * SelectedTilemap.TilesetHeight);

        isMouseOverInteractableUI = false;
        foreach (UIObject uiObj in interactableUI)
        {
            if (uiObj.TargetRect.Contains(InputManager.Mouse.Position))
            {
                isMouseOverInteractableUI = true;
                break;
            }
        }

        if (optionsMenu.Active && optionsMenu.TargetRect.Contains(InputManager.Mouse.Position))
            isMouseOverInteractableUI = true;

        Rectangle tileSelectorRect = new Rectangle(tileSelector.Offset.ToPoint(), new Point(tileSelector.WidthInPixels, tileSelector.HeightInPixels));
        if (tileSelectorRect.Contains(InputManager.Mouse.Position))
            isMouseOverInteractableUI = true;


        HandleClicksOnSelectedMap();

        if (InputManager.GetKeyDown(Keys.I) && !Globals.IsTyping)
            ShowTileIds = !ShowTileIds;

        if (tileSelectorEnabled)
            UpdateTileSelector();

        newLayerButton.Update();

        foreach (Button button in layerButtons)
        {
            button.Update();
        }

        if (!objectEditMenu.Shown && !optionsMenu.Active)
        {
            editLayerButton.Update();
            deleteLayerButton.Update();
        }

        if (InputManager.GetMouseButtonUp(1) && Layers.Count > 1)
        {
            foreach (Button button in layerButtons)
            {
                if (button.TargetRect.Contains(InputManager.Mouse.Position))
                {
                    Tilemap layer = Layers[layerButtons.IndexOf(button)];

                    if (hiddenLayers.Contains(layer))
                        hiddenLayers.Remove(layer);
                    else if (hiddenLayers.Count < Layers.Count - 1)
                        hiddenLayers.Add(layer);
                }
            }

            layerButtons.Clear();
        }

        for (int i = hiddenLayers.Count - 1; i > 0; i--)
        {
            if (!Layers.Contains(hiddenLayers[i]))
                hiddenLayers.Remove(hiddenLayers[i]);

            layerButtons.Clear();
        }

        if (hiddenLayers.Contains(SelectedTilemap))
        {
            SelectedTilemap = Layers.Where(L => !hiddenLayers.Contains(L)).First();
            layerButtons.Clear();
        }

        // Save
        if ((InputManager.Keyboard.IsKeyDown(Keys.LeftControl) || InputManager.Keyboard.IsKeyDown(Keys.RightControl)) && InputManager.GetKeyDown(Keys.S))
            SaveToMapFile();

        // Load
        if ((InputManager.Keyboard.IsKeyDown(Keys.LeftControl) || InputManager.Keyboard.IsKeyDown(Keys.RightControl)) && InputManager.GetKeyDown(Keys.L))
            LoadFromMapFile();

        if (InputManager.GetKeyDown(Keys.R))
            Reset();

        objectEditMenu.Update();
    }

    public static void WhenThisIsDrawnTranslationWillBeAppliedByAMatrix()
    {
        if (!Enabled)
            return;

        foreach (TilemapObject obj in objects)
        {
            obj.Draw();
        }
    }

    public static void DrawToWorld()
    {
        if (!Enabled) return;

        Globals.SpriteBatch.Draw(mapBorder, selectedTilemap.GetWorldPositionOfTile(Point.Zero) - new Vector2(2), Color.White);

        // Draw selected tile outline if there is no interactable UI under the mouse right now
        if (!isMouseOverInteractableUI)
        {
            Point hoverTile = SelectedTilemap.GetTileAtWorldPosition(InputManager.Mouse.Position.ToVector2());

            if (outline != null && SelectedTilemap.IsTileInBounds(hoverTile))
            {
                Globals.SpriteBatch.Draw(outline,
                    new Rectangle(
                        SelectedTilemap.GetWorldPositionOfTile(hoverTile).ToPoint(),
                        new Vector2(SelectedTilemap.TileSize * SelectedTilemap.Scale).ToPoint()),
                    new Color(Color.White, 0.4f)
                );
            }
        }

        // Add the camera offset for when the tilemap is drawn
        foreach (Tilemap layer in Layers)
        {
            if (hiddenLayers.Contains(layer))
                continue;

            Vector2 offset = layer.Offset;

            layer.Offset += GameManager.GameCamera.Translation;
            layer.Draw();
            layer.Offset = offset;
        }

        StringBuilder builder = new StringBuilder();

        for (int y = 0; y < SelectedTilemap.Height; y++)
        {
            for (int x = 0; x < SelectedTilemap.Width; x++)
            {
                int tileId = SelectedTilemap.Tiles[x, y];

                if (ShowTileIds)
                {
                    builder.Clear();
                    builder.Append(tileId);

                    Vector2 origin = Globals.NumFont.MeasureString(builder.ToString()) / 2;
                    Globals.SpriteBatch.DrawString(
                        Globals.NumFont,
                        builder,
                        SelectedTilemap.Offset + GameManager.GameCamera.Translation + new Vector2(x, y) * SelectedTilemap.TileSize * SelectedTilemap.Scale + new Vector2(SelectedTilemap.TileSize * SelectedTilemap.Scale) / 2,
                        new Color(Color.White, 0.5f),
                        0f, origin,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }
    }


    public static void DrawToScreen()
    {
        if (showTooltips)
        {
            StringBuilder sb = new StringBuilder($"{tooltips[0]}");
            if (Enabled)
            {
                for (int i = 1; i < tooltips.Length; i++)
                {
                    sb.Append("\n" + tooltips[i]);
                }
            }

            // Show tooltips at the top
            Globals.SpriteBatch.DrawString(Globals.TextFont,
                sb.ToString(),
                new Vector2(15), Color.White, 0f, Vector2.Zero,
                new Vector2(2), SpriteEffects.None, 0f);
        }


        if (!Enabled) return;

        if (optionsMenu.Active)
        {
            optionsMenu.Draw();
            return;
        }

        if (TileSelectorEnabled)
            tileSelector.Draw();

        // Draw hover tile outline on the tile selector
        if (!isMouseOverInteractableUI)
        {
            Point hoverTile = tileSelector.GetTileAtScreenPosition(InputManager.Mouse.Position.ToVector2());

            if (outline != null && tileSelector.IsTileInBounds(hoverTile))
            {
                Globals.SpriteBatch.Draw(outline,
                    new Rectangle(
                        tileSelector.GetScreenPositionOfTile(hoverTile).ToPoint(),
                        new Vector2(tileSelector.TileSize * tileSelector.Scale).ToPoint()),
                    new Color(Color.White, 0.4f)
                );
            }
        }

        editLayerButton.Draw();
        deleteLayerButton.Draw();


        // Highlight selected tile on tile selector
        Point highlightPos = new Point((SelectedTile - 1) % tileSelector.TilesetWidth, (SelectedTile - 1) / tileSelector.TilesetWidth);
        Globals.SpriteBatch.Draw(Globals.WhiteTexture,
            new Rectangle(
                tileSelector.GetScreenPositionOfTile(highlightPos).ToPoint(),
                new Vector2(tileSelector.TileSize * tileSelector.Scale).ToPoint()),
            new Color(Color.SkyBlue, 0.35f)
        );

        // Show selected tile ID
        Globals.SpriteBatch.DrawString(Globals.NumFont, SelectedTile.ToString(),
            new Vector2(20,
                Globals.VirtualGameSize.Y -
                tileSelector.Height * tileSelector.TileSize * tileSelector.Scale -
                Globals.NumFont.MeasureString(SelectedTile.ToString()).Y -
                30),
            Color.White);


        DrawLayerList();

        newLayerButton.Draw();

        objectEditMenu.Draw();
    }


    private static Texture2D GetTileOutline()
    {
        Texture2D texture = new Texture2D(Globals.GraphicsDevice, SelectedTilemap.TileSize, SelectedTilemap.TileSize);
        List<Color> data = new List<Color>(SelectedTilemap.TileSize * SelectedTilemap.TileSize);

        for (int y = 0; y < SelectedTilemap.TileSize; y++)
        {
            for (int x = 0; x < SelectedTilemap.TileSize; x++)
            {
                if (y == 0 || y == SelectedTilemap.TileSize - 1 || x == 0 || x == SelectedTilemap.TileSize - 1)
                    data.Add(Color.White);
                else data.Add(new Color(0, 0, 0, 0));
            }
        }

        texture.SetData(data.ToArray());
        return texture;
    }

    private static Texture2D GetMapBorder()
    {
        Texture2D texture = new Texture2D(Globals.GraphicsDevice,
            (int)Math.Floor(SelectedTilemap.Width * selectedTilemap.TileSize * selectedTilemap.Scale + 4),
            (int)Math.Floor(SelectedTilemap.Height * selectedTilemap.TileSize * selectedTilemap.Scale + 4));
        Color[] data = new Color[texture.Width * texture.Height];

        for (int y = 0; y < texture.Height; y++)
        {
            for (int x = 0; x < texture.Width; x++)
            {
                if (y <= 1 || y >= texture.Height - 2 || x <= 1 || x >= texture.Width - 2)
                    data[y * texture.Width + x] = new Color(Color.White, 0.7f);
                else data[y * texture.Width + x] = new Color(0, 0, 0, 0);
            }
        }

        texture.SetData(data);
        return texture;
    }


    public static void ReloadTileSelector()
    {
        tileSelector = new Tilemap(selectedTilemap.TilesetWidth, selectedTilemap.TilesetHeight, SelectedTilemap.TileSize, SelectedTilemap.TilesetName);

        tileSelector.Offset = new Vector2(10, Globals.VirtualGameSize.Y - tileSelector.Height * tileSelector.TileSize * tileSelector.Scale - 20);

        for (int y = 0; y < tileSelector.Height; y++)
        {
            for (int x = 0; x < tileSelector.Width; x++)
            {
                tileSelector.SetTileAt(new Point(x, y), y * tileSelector.Width + x + 1);
            }
        }
    }

    private static void UpdateTileSelector()
    {
        if (!TileSelectorEnabled)
            return;

        if (InputManager.GetMouseButtonUp(0))
        {
            Point hoverTile = tileSelector.GetTileAtScreenPosition(InputManager.Mouse.Position.ToVector2());

            if (tileSelector.IsTileInBounds(hoverTile))
                SelectedTile = tileSelector.GetTileAt(hoverTile);
        }
    }

    private static void DrawLayerList()
    {
        if (layerButtons.Count != Layers.Count)
        {
            layerButtons.Clear();

            for (int i = 0; i < Layers.Count; i++)
            {
                Tilemap map = Layers[i];

                Button button = new Button(Rectangle.Empty);
                button.TargetRect = new Rectangle(
                    (int)Math.Floor(Globals.VirtualGameSize.X - button.GetSizeOfText(map.Name).X - 25),
                    Globals.VirtualGameSize.Y - 55 * (i + 1),
                    (int)Math.Floor(button.GetSizeOfText(map.Name).X + 15),
                    50
                );
                button.ButtonClicked += (sender, e) =>
                {
                    if (!hiddenLayers.Contains(map))
                        SelectedTilemap = map;
                };
                button.Text = map.Name;

                button.Disabled = hiddenLayers.Contains(map);

                layerButtons.Add(button);
                interactableUI.Add(button);
            }

            newLayerButton.TargetRect = new Rectangle(
                Globals.VirtualGameSize.X - newLayerButton.GetSizeOfText("New").ToPoint().X - 40,
                Globals.VirtualGameSize.Y - 50 - 55 * Layers.Count,
                newLayerButton.GetSizeOfText("New").ToPoint().X + 25,
                40);
        }

        foreach (Button button in layerButtons)
        {
            if (layerButtons.IndexOf(button) == Layers.IndexOf(selectedTilemap))
                button.NormalColor = Color.LightBlue;
            else if (hiddenLayers.Contains(Layers[layerButtons.IndexOf(button)]))
                button.NormalColor = Color.Thistle;
            else button.NormalColor = Color.White;

            button.Draw();
        }
    }


    private static void HandleClicksOnSelectedMap()
    {
        if (isMouseOverInteractableUI || SelectedTilemap.TilesetName == null) return;

        Point hoverTile = selectedTilemap.GetTileAtWorldPosition(InputManager.Mouse.Position.ToVector2());

        // Calculate the line between the mouse position this frame and previous frame to place tiles
        if (InputManager.Mouse.LeftButton == ButtonState.Pressed)
        {
            if (InputManager.MouseOld.LeftButton == ButtonState.Pressed)
            {
                List<(int, int)> tiles = selectedTilemap.GetLine(
                    SelectedTilemap.GetTileAtWorldPosition(InputManager.MouseOld.Position.ToVector2()),
                    SelectedTilemap.GetTileAtWorldPosition(InputManager.Mouse.Position.ToVector2()));

                foreach (var pos in tiles)
                {
                    SelectedTilemap.SetTileAt(new Point(pos.Item1, pos.Item2), SelectedTile);
                }
            }
            else SelectedTilemap.SetTileAt(hoverTile, SelectedTile);
        }
        // Calculate the line between the mouse position this frame and previous frame to erase tiles
        else if (InputManager.Mouse.RightButton == ButtonState.Pressed)
        {
            if (InputManager.MouseOld.RightButton == ButtonState.Pressed)
            {
                List<(int, int)> tiles = selectedTilemap.GetLine(
                    SelectedTilemap.GetTileAtWorldPosition(InputManager.MouseOld.Position.ToVector2()),
                    SelectedTilemap.GetTileAtWorldPosition(InputManager.Mouse.Position.ToVector2()));

                foreach (var pos in tiles)
                {
                    SelectedTilemap.SetTileAt(new Point(pos.Item1, pos.Item2), 0);
                }
            }
            else SelectedTilemap.SetTileAt(hoverTile, 0);
        }
        else if (InputManager.GetMouseButtonDown(2))
        {
            objectEditMenu.SelectedPointOnMap = hoverTile;

            foreach (TilemapObject obj in objects)
            {
                if (obj.GetAssignedTile() == hoverTile)
                {
                    // There is an object assigned to this tile, so we need to edit it

                    objectEditMenu.FillPropertyTextboxes(obj); // Show the editor window for it
                    objectEditMenu.Shown = true;
                    return;
                }
            }

            // If there is no object found at this tile...

            Type objType = TilemapObjectLoader.LoadObject(); // We load the object class

            if (objType == null)
                return;

            objectEditMenu.CreatePropertyTextboxes(objType); // And show the editor window for it
            objectEditMenu.Shown = true;
        }
    }


    private static void OpenLayerEditMenu()
    {
        optionsMenu.NameTextBox.Text = SelectedTilemap.Name;
        optionsMenu.TilesetTextBox.Text = SelectedTilemap.TilesetName ?? string.Empty;
        optionsMenu.TilesizeTextBox.Text = SelectedTilemap.TileSize.ToString();
        optionsMenu.WidthTextBox.Text = SelectedTilemap.Width.ToString();
        optionsMenu.HeightTextBox.Text = SelectedTilemap.Height.ToString();

        optionsMenu.Active = true;
    }

    public static void SaveToMapFile()
    {
        string path = SaveFile.SaveDialog("Map File (*.pmf)\0*.pmf\0All Files (*.*)\0*.*\0", "Save map", "map");

        if (path == string.Empty)
            return;

        GameMap map = new GameMap(Layers.ToArray(), objects.ToArray());
        GameMapData data = new GameMapData(map);
        data.SaveToFile(path);

        Debug.WriteLine("[INFO] Saved map file to:  " + path);
    }

    public static void LoadFromMapFile()
    {
        string path = OpenFile.OpenDialog("Map File (*.pmf)\0*.pmf\0All Files (*.*)\0*.*\0", "Load map");

        if (path == string.Empty)
            return;

        Layers.Clear();
        layerButtons.Clear();

        GameMap map = new GameMap(GameMapData.LoadFromFile(path));
        Layers = map.Layers.ToList();
        objects = map.Objects.ToList();

        foreach (TilemapObject obj in objects)
        {
            obj.Move(obj.TargetRectangle, obj.GetAssignedTile());
            obj.Load();
        }

        SelectedTilemap = Layers[0];
        ReloadTileSelector();
        CenterSelectedMap();

        Debug.WriteLine("[INFO] Loaded map file from:  " + path);
    }

    public static void Reset()
    {
        layerButtons.Clear();
        Layers.Clear();
        Layers.Add(new Tilemap(10, 10, 16, string.Empty, "Layer 1"));
        SelectedTilemap = Layers[0];

        ReloadTileSelector();

        CenterSelectedMap();
    }

    public static void CenterSelectedMap()
    {
        GameManager.Movement = Layers[0].Bounds.Center.ToVector2();
    }
}