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
    }
}
