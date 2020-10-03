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

        public static void UnloadScene()
        {
            Scenes.Remove(ActiveScene);
            //Freeing the currently loaded assets, must be called before loading any new assets!!
            if (ActiveScene != null)
                Setup.Content.Unload();
        }

        public static void UnloadScene(Scene scene)
        {
            Scenes.Remove(scene);
            //Freeing the currently loaded assets, must be called before loading any new assets!!
            if (scene != null)
                Setup.Content.Unload();
        }

        public static void LoadScene(int index)
        {
            foreach (Scene S in Scenes)
            {
                if (S.ID == index)
                {
                    ActiveScene = S;
                    break;
                }
            }
        }
    }
}
