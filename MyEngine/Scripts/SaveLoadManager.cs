using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyEngine
{
    public class SaveLoadManager
    {
        public static void SaveData<T>(T HighScore)
        {
            BinaryFormatter Formatter = new BinaryFormatter();
            string Path = "C:/MonoGame Projects/FlappyShit/SaveData/HighScore.Data";
            FileStream stream = new FileStream(Path, FileMode.Create);

            T dataTobeSaved = HighScore;

            Formatter.Serialize(stream, dataTobeSaved);
            stream.Close();
        }

        public static T LoadData<T>()
        {
            string Path = "C:/MonoGame Projects/FlappyShit/SaveData/HighScore.Data";

            if (File.Exists(Path))
            {
                BinaryFormatter Formatter = new BinaryFormatter();
                FileStream stream = new FileStream(Path, FileMode.Open);
                var dataTobeSaved = Formatter.Deserialize(stream);
                stream.Close();

                return (T)dataTobeSaved;
            }
            else
                return default(T);
        }

        public static void ResetData()
        {
            string Path = "C:/MonoGame Projects/FlappyShit/SaveData/HighScore.Data";
            File.Delete(Path);
        }
    }
}
