using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyEngine
{
    public static class SaveLoadManager
    {
        private static string GamePath;

        public static void InitializeDirectory() //call this the first time you use this class
        {
            GamePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            string GameName = "";
            foreach (char C in System.AppDomain.CurrentDomain.FriendlyName)
            {
                if (C != '.')
                    GameName += C;
                else
                    break;
            }
            var SubFolders = new DirectoryInfo(GamePath).CreateSubdirectory("From Newbies").CreateSubdirectory(GameName);
            GamePath = System.IO.Path.Combine(GamePath, "From Newbies", GameName);
        }

        public static void SaveData<T>(T HighScore, string SaveFileName)
        {
            BinaryFormatter Formatter = new BinaryFormatter();
            string SavePath = System.IO.Path.Combine(GamePath, SaveFileName);
            FileStream stream = new FileStream(SavePath, FileMode.Create);

            T dataTobeSaved = HighScore;

            Formatter.Serialize(stream, dataTobeSaved);
            stream.Close();
        }

        public static T LoadData<T>(string SaveFileName)
        {
            string SavePath = System.IO.Path.Combine(GamePath, SaveFileName);

            if (File.Exists(SavePath))
            {
                BinaryFormatter Formatter = new BinaryFormatter();
                FileStream stream = new FileStream(SavePath, FileMode.Open);
                var dataTobeSaved = Formatter.Deserialize(stream);
                stream.Close();

                return (T)dataTobeSaved;
            }
            else
                return default(T);
        }

        public static void ResetData(string SaveFileName)
        {
            string SavePath = System.IO.Path.Combine(GamePath, SaveFileName);

            File.Delete(SavePath);
        }
    }
}