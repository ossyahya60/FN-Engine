using Newtonsoft.Json;
using System.IO;

namespace MyEngine
{
    public static class TileMapLoader
    {
        public static string LoadedMap;

        static TileMapLoader()
        {
            LoadedMap = "";
        }

        public class Layer
        {
            public string name { get; set; }
            public string _eid { get; set; }
            public int offsetX { get; set; }
            public int offsetY { get; set; }
            public int gridCellWidth { get; set; }
            public int gridCellHeight { get; set; }
            public int gridCellsX { get; set; }
            public int gridCellsY { get; set; }
            public string tileset { get; set; }
            public System.Collections.Generic.List<int> data { get; set; }
            public int exportMode { get; set; }
            public int arrayMode { get; set; }
        }

        public class Root
        {
            public string ogmoVersion { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int offsetX { get; set; }
            public int offsetY { get; set; }
            public System.Collections.Generic.List<Layer> layers { get; set; }
        }

        /////////////////////////////////////
        public static void LoadJson(string FileName)
        {
            using (StreamReader r = new StreamReader(FileName))
                LoadedMap = r.ReadToEnd();
        }

        public static Root GetRoot()
        {
            if (LoadedMap != "")
                return JsonConvert.DeserializeObject<TileMapLoader.Root>(LoadedMap);

            return null;
        }

        public static Layer[] GetLayers()
        {
            if (LoadedMap != "")
                return JsonConvert.DeserializeObject<TileMapLoader.Root>(LoadedMap).layers.ToArray();

            return null;
        }
    }
}
