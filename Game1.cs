using PalmMapEditor.Tilemaps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PalmMapEditor.Core;

namespace PalmMapEditor;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private int screenWidth = 1366;
    private int screenHeight = 768;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = screenWidth;
        _graphics.PreferredBackBufferHeight = screenHeight;
        _graphics.HardwareModeSwitch = false;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.SpriteBatch = _spriteBatch;
        Globals.GraphicsDevice = GraphicsDevice;
        Globals.Content = Content;
        Globals.NumFont = Content.Load<SpriteFont>("Fonts/numFont");
        Globals.TextFont = Content.Load<SpriteFont>("Fonts/textFont");

        Globals.WhiteTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
        Globals.WhiteTexture.SetData(new Color[] { Color.White });

        GameManager.Load();
    }

    protected override void Update(GameTime gameTime)
    {
        Globals.GameTime = gameTime;

        if (IsActive)
        {
            InputManager.Update();

            if (InputManager.GetKeyDown(Keys.F11))
            {
                _graphics.ToggleFullScreen();

                if (_graphics.IsFullScreen)
                {
                    _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                }
                else
                {
                    _graphics.PreferredBackBufferWidth = screenWidth;
                    _graphics.PreferredBackBufferHeight = screenHeight;
                }

                _graphics.ApplyChanges();

                if (TilemapEditor.Enabled)
                    TilemapEditor.LoadUI();
            }

            GameManager.Update();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GameManager.Draw();

        base.Draw(gameTime);
    }

    protected override void OnExiting(object sender, ExitingEventArgs args)
    {
        TilemapEditor.SaveToMapFile();
        base.OnExiting(sender, args);
    }
}