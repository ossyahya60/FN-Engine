using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyEngine
{
    public enum MouseButtons { LeftClick, RightClick, MouseWheelClick}

    public static class Input //Supports Mouse and Keyboard
    {
        static Input()
        {
            LastKeyState = Keyboard.GetState();
            LastMouseState = new MouseState();
        }

        private static KeyboardState LastKeyState, CurrentKeyState;
        private static MouseState LastMouseState, CurrentMouseState;

        public static bool GetKey(Keys Key)
        {
            return Keyboard.GetState().IsKeyDown(Key);
        }

        public static void GetState() //Has to be called in the start of every update
        {
            LastKeyState = CurrentKeyState;
            CurrentKeyState = Keyboard.GetState();

            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        public static bool GetKeyDown(Keys Key)
        {
            if (CurrentKeyState.IsKeyDown(Key) && !LastKeyState.IsKeyDown(Key))
                return true;

            return false;
        }

        public static bool GetKeyUp(Keys Key)
        {
            if (CurrentKeyState.IsKeyUp(Key) && LastKeyState.IsKeyDown(Key))
                return true;

            return false;
        }

        public static bool GetMouseClick(MouseButtons mouseButton)
        {
            if (Setup.GameWindow.ClientBounds.Contains(GetMousePosition()))
            {
                if (mouseButton == MouseButtons.LeftClick)
                    return Mouse.GetState().LeftButton == ButtonState.Pressed;
                else if (mouseButton == MouseButtons.RightClick)
                    return Mouse.GetState().RightButton == ButtonState.Pressed;
                else if (mouseButton == MouseButtons.MouseWheelClick)
                    return Mouse.GetState().MiddleButton == ButtonState.Pressed;
            }

            return false;
        }

        public static bool GetMouseClickDown(MouseButtons mouseButton)
        {
            if (Setup.GameWindow.ClientBounds.Contains(GetMousePosition()))
            {
                if (mouseButton == MouseButtons.LeftClick)
                {
                    if (CurrentMouseState.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Released)
                        return true;
                }
                else if (mouseButton == MouseButtons.RightClick)
                {
                    if (CurrentMouseState.RightButton == ButtonState.Pressed && LastMouseState.RightButton == ButtonState.Released)
                        return true;
                }
                else if (mouseButton == MouseButtons.MouseWheelClick)
                {
                    if (CurrentMouseState.MiddleButton == ButtonState.Pressed && LastMouseState.MiddleButton == ButtonState.Released)
                        return true;
                }
            }

            return false;
        }

        public static bool GetMouseClickUp(MouseButtons mouseButton)
        {
            if (Setup.GameWindow.ClientBounds.Contains(GetMousePosition()))
            {
                if (mouseButton == MouseButtons.LeftClick)
                {
                    if (CurrentMouseState.LeftButton == ButtonState.Released && LastMouseState.LeftButton == ButtonState.Pressed)
                        return true;
                }
                else if (mouseButton == MouseButtons.RightClick)
                {
                    if (CurrentMouseState.RightButton == ButtonState.Released && LastMouseState.RightButton == ButtonState.Pressed)
                        return true;
                }
                else if (mouseButton == MouseButtons.MouseWheelClick)
                {
                    if (CurrentMouseState.MiddleButton == ButtonState.Released && LastMouseState.MiddleButton == ButtonState.Pressed)
                        return true;
                }
            }

            return false;
        }

        public static Vector2 GetMousePosition()
        {
            return Setup.resolutionIndependentRenderer.ScaleMouseToScreenCoordinates(Mouse.GetState().Position.ToVector2());
        }
    }
}
