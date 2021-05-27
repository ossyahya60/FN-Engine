using Newtonsoft.Json;
using System.IO;

namespace MyEngine
{
    public static class TileMapLoader
    {
        //public static string SaveJson(string Path)
        //{
        //    using (StreamWriter r = new StreamWriter(Path))
        //        r.;
        //}

        public static string LoadJson(string FileName)
        {
            using (StreamReader r = new StreamReader(FileName))
                return r.ReadToEnd();
        }

        public static T DeserializeJsonToObject<T>(string Json)
        {
            return JsonConvert.DeserializeObject<T>(Json);
        }

        public static string SerializeObjectToJson<T>(T OBJ)
        {
            return JsonConvert.SerializeObject(OBJ, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
        }
    }
}
