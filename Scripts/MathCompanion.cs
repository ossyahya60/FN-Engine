using Microsoft.Xna.Framework;
using System;

namespace FN_Engine
{
    public static class MathCompanion
    {
        public static float Abs(float number)
        {
            if (number >= 0)
                return number;
            else
                return -number;
        }

        public static Vector2 Abs(Vector2 number)
        {
            return new Vector2(Math.Abs(number.X), Math.Abs(number.Y));
        }

        public static System.Numerics.Vector2 Abs(System.Numerics.Vector2 number)
        {
            return new System.Numerics.Vector2(Math.Abs(number.X), Math.Abs(number.Y));
        }

        public static Point Abs(Point number)
        {
            return new Point(Math.Abs(number.X), Math.Abs(number.Y));
        }

        public static bool IsPositive(float number)
        {
            if (number >= 0)
                return true;
            else
                return false;
        }

        public static float Clamp(float Value, float Min, float Max)
        {
            if (Value < Min)
                return Min;
            else if (Value > Max)
                return Max;
            else
                return Value;
        }

        public static Vector2 Clamp(Vector2 Value, Vector2 Min, Vector2 Max)
        {
            Vector2 Output = Value;
            if (Value.X <= Min.X)
                Output.X = Min.X;
            else if (Value.X > Max.X)
                Output.X = Max.X;

            if (Value.Y <= Min.Y)
                Output.Y = Min.Y;
            else if (Value.Y > Max.Y)
                Output.Y = Max.Y;

            return Output;
        }

        public static int Sign(float number)
        {
            if (number >= 0)
                return 1;
            else
                return -1;
        }

        public static double Atan(float y, float x)
        {
            return Math.Atan2(y, x);
        }

        public static float GetAngle(Vector2 V1, Vector2 V2)
        {
            return MathHelper.ToDegrees((float)Atan(V2.Y - V1.Y, V2.X - V1.X));
        }

        public static float GetAngle_Rad(Vector2 V1, Vector2 V2)
        {
            return (float)Atan(V2.Y - V1.Y, V2.X - V1.X);
        }

        public static int GetNumerOfValuesBetween(int A1, int A2, float ScaleDownFactor)
        {
            float sign = 0;
            if (A2 != A1)
                sign = Sign(A2 - A1);

            return (int)((A2 - A1) * ScaleDownFactor - sign);
        }

        public static float CrossProduct(Vector2 V1, Vector2 V2)
        {
            return (V1.X* V2.Y) - (V1.Y* V2.X);
        }
    }
}
