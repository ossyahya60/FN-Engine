using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace MyEngine
{
    public static class Input
    {
        static Input()
        {
            LastKeyState = new KeyboardState();
        }

        private static KeyboardState LastKeyState;

        public static bool GetKey(Keys Key)
        {
            return Keyboard.GetState().IsKeyDown(Key);
        }

        public static bool GetKeyDown(Keys Key)
        {
            if (Keyboard.GetState().IsKeyDown(Key) && LastKeyState.IsKeyUp(Key))
            {
                LastKeyState = Keyboard.GetState();
                return true;
            }
            else
            {
                LastKeyState = Keyboard.GetState();
                return false;
            }
        }

        public static bool GetKeyUp(Keys Key)
        {
            if (Keyboard.GetState().IsKeyUp(Key) && LastKeyState.IsKeyDown(Key))
            {
                LastKeyState = Keyboard.GetState();
                return true;
            }
            else
            {
                LastKeyState = Keyboard.GetState();
                return false;
            }
        }
    }
}
