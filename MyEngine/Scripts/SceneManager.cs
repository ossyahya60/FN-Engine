using System.Collections.Generic;

namespace MyEngine
{
    public static class SceneManager
    {
        private static Scene ActiveScene;
        private static List<Scene> Scenes;

        public static void Start()
        {
            Scenes = new List<Scene>();
            ActiveScene = null;
        }

        public static Scene GetActiveScene()
        {
            return ActiveScene;
        }

        public static void AddScene(Scene scene)
        {
            foreach (Scene S in Scenes)
                if (S == scene)
                    return;

            Scenes.Add(scene);
        }

        public static void LoadScene(int index)
        {
            foreach (Scene S in Scenes)
                if (S.ID == index)
                {
                    ActiveScene = S;
                    break;
                }
        }
    }
}
