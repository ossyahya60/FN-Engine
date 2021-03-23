using Microsoft.Xna.Framework;
using System;

namespace MyEngine
{
    public static class Utility
    {
        //public static readonly Type[] SupportedTypes = { typeof(float), typeof(int), typeof(string), typeof(Vector2), typeof(bool) };

        public static void Vector2Int(ref Vector2 vector)
        {
            vector.X = (int)vector.X;
            vector.Y = (int)vector.Y;
        }

        public static Vector2 STR_To_Vector2(string Value)
        {
            Vector2 Output = Vector2.Zero;

            string[] Vector2Format = Value.Split(' ');
            Output.X = float.Parse(Vector2Format[0].Remove(0, 3));
            string Y = Vector2Format[1].Remove(0, 2);
            Y = Y.Remove(Y.Length - 1, 1);
            Output.Y = float.Parse(Y);

            return Output;
        }

        public static Vector3 STR_To_Vector3(string Value)
        {
            Vector3 Output = Vector3.Zero;

            string[] Vector3Format = Value.Split(' ');
            Output.X = float.Parse(Vector3Format[0].Remove(0, 3));
            string Z = Vector3Format[2].Remove(0, 2);
            Z = Z.Remove(Z.Length - 1, 1);
            Output.Z = float.Parse(Z);
            Output.Y = float.Parse(Vector3Format[1].Remove(0, 2));

            return Output;
        }

        public static Vector4 STR_To_Vector4(string Value)
        {
            Vector4 Output = Vector4.Zero;

            string[] Vector4Format = Value.Split(' ');
            Output.X = float.Parse(Vector4Format[0].Remove(0, 3));
            string W = Vector4Format[3].Remove(0, 2);
            W = W.Remove(W.Length - 1, 1);
            Output.W = float.Parse(W);
            Output.Y = float.Parse(Vector4Format[1].Remove(0, 2));
            Output.Z = float.Parse(Vector4Format[2].Remove(0, 2));

            return Output;
        }

        public static Color STR_To_Color(string Value)
        {
            Color Output = Color.TransparentBlack;

            string[] ColorFormat = Value.Split(' ');
            Output.R = byte.Parse(ColorFormat[0].Remove(0, 3));
            Output.G = byte.Parse(ColorFormat[1].Remove(0, 2));
            Output.B = byte.Parse(ColorFormat[2].Remove(0, 2));
            string A = ColorFormat[3].Remove(0, 2);
            A = A.Remove(A.Length - 1, 1);
            Output.A = byte.Parse(A);

            return Output;
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
