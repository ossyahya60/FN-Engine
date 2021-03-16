using Microsoft.Xna.Framework;
using System;

namespace MyEngine
{
    public static class Utility
    {
        public static void Vector2Int(ref Vector2 vector)
        {
            vector.X = (int)vector.X;
            vector.Y = (int)vector.Y;
        }

        public static Vector2 Vector2Int(Vector2 vector)
        {
            return new Vector2((int)vector.X, (int)vector.Y);
        }

        public static object GetInstance(string ObjectType)
        {
            Type T = Type.GetType(ObjectType);
            return Activator.CreateInstance(T);
        }
    }
}
