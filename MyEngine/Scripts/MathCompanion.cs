using Microsoft.Xna.Framework;
using System;

namespace MyEngine
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

        public static int GetNumerOfValuesBetween(int A1, int A2, float ScaleDownFactor)
        {
            float sign = 0;
            if (A2 != A1)
                sign = Sign(A2 - A1);

            return (int)((A2 - A1) * ScaleDownFactor - sign);
        }
    }
}
