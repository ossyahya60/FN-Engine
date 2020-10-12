using System.Collections.Generic;

namespace MyEngine
{
    public static class Layer
    {
        private static Dictionary<string, float> LayerWithValue;
        private static readonly float Maximum = 80;

        static Layer()
        {
            LayerWithValue = new Dictionary<string, float>();
            AddLayer("Default", 100);
        }

        public static bool AddLayer(string Name, int Value)
        {
            if (LayerWithValue.Count >= Maximum)
                return false;

            if (LayerWithValue.ContainsValue(Value * 0.01f))
                return false;

            if (LayerWithValue.ContainsKey(Name))
                return false;

            LayerWithValue.Add(Name, MathCompanion.Clamp(Value * 0.01f, 0.2f, 1));
            return true;
        }

        public static float GetLayer(string Name) //Invalid Input if Layer is not found
        {
            foreach (KeyValuePair<string, float> KVP in LayerWithValue)
                if (KVP.Key == Name)
                    return 1 - KVP.Value;

            return 1;
        }

        public static bool DeleteLayer(string Name)
        {
            return LayerWithValue.Remove(Name);
        }

        public static int GetLayerCount()
        {
            return LayerWithValue.Count;
        }

        public static bool IsFull()
        {
            if (LayerWithValue.Count >= Maximum)
                return true;

            return false;
        }
    }
}
