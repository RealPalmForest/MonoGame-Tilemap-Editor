using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace PalmMapEditor.Core;

public static class InputManager
{
    public static KeyboardState Keyboard { get; private set; }
    public static KeyboardState KeyboardOld { get; private set; }
    
    public static MouseState Mouse { get; private set; }
    public static MouseState MouseOld { get; private set; }

    public static void Update()
    {
        KeyboardOld  = Keyboard;
        Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        
        MouseOld = Mouse;
        Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
    }

    public static bool GetKeyDown(Keys key)
    {
        return Keyboard.IsKeyDown(key) && KeyboardOld.IsKeyUp(key);
    }
    
    public static bool GetKeyUp(Keys key)
    {
        return Keyboard.IsKeyUp(key) && KeyboardOld.IsKeyDown(key);
    }

    /// <summary>
    /// Returns true of the mouse button is pressed this frame but not the previous
    /// </summary>
    /// <param name="mouseIndex">0 - Left Button, 1 - Right Button, 2 - Middle Button</param>
    /// <returns></returns>
    public static bool GetMouseButtonDown(int mouseIndex)
    {
        switch (mouseIndex)
        {
            case 0:
                return Mouse.LeftButton == ButtonState.Pressed && MouseOld.LeftButton == ButtonState.Released;
            case 1:
                return Mouse.RightButton == ButtonState.Pressed && MouseOld.RightButton == ButtonState.Released;
            case 2:
                return Mouse.MiddleButton == ButtonState.Pressed && MouseOld.MiddleButton == ButtonState.Released;
            case 3:
                return Mouse.XButton1 == ButtonState.Pressed && MouseOld.XButton1 == ButtonState.Released;
            case 4:
                return Mouse.XButton2 == ButtonState.Pressed && MouseOld.XButton2 == ButtonState.Released;
            default:
                return false;
        }
    }


    /// <summary>
    /// Returns true of the mouse button is released this frame but not the previous
    /// </summary>
    /// <param name="mouseIndex">0 - Left Button, 1 - Right Button, 2 - Middle Button</param>
    /// <returns></returns>
    public static bool GetMouseButtonUp(int mouseIndex)
    {
        switch (mouseIndex)
        {
            case 0:
                return Mouse.LeftButton == ButtonState.Released && MouseOld.LeftButton == ButtonState.Pressed;
            case 1:
                return Mouse.RightButton == ButtonState.Released && MouseOld.RightButton == ButtonState.Pressed;
            case 2:
                return Mouse.MiddleButton == ButtonState.Released && MouseOld.MiddleButton == ButtonState.Pressed;
            case 3:
                return Mouse.XButton1 == ButtonState.Released && MouseOld.XButton1 == ButtonState.Pressed;
            case 4:
                return Mouse.XButton2 == ButtonState.Released && MouseOld.XButton2 == ButtonState.Pressed;
            default:
                return false;
        }
    }

    /// <summary>
    /// Gets the direction movement axis of keyboard
    /// </summary>
    /// <param name="axis">"X" - Horizontal axis, "Y" - Vertical axis</param>
    /// <returns></returns>
    public static float GetDirAxis(string axis)
    {
        switch (axis)
        {
            case "X":
                float x = 0f;
                
                if (KeyboardOld.IsKeyDown(Keys.Left) || (KeyboardOld.IsKeyDown(Keys.A) && !Globals.IsTyping))
                    x--;
                if (KeyboardOld.IsKeyDown(Keys.Right) || (KeyboardOld.IsKeyDown(Keys.D) && !Globals.IsTyping))
                    x++;
                return x;
            case "Y":
                float y = 0f;

                if (KeyboardOld.IsKeyDown(Keys.Up) || (KeyboardOld.IsKeyDown(Keys.W) && !Globals.IsTyping))
                    y--;
                if (KeyboardOld.IsKeyDown(Keys.Down) || (KeyboardOld.IsKeyDown(Keys.S) && !Globals.IsTyping))
                    y++;
                return y;
            default:
                return 0f;
        }
    }
}