using System;
using System.Collections.Generic;

namespace MyEngine
{
    public static class SceneManager
    {
        public static Scene ActiveScene;
        public static List<Action> InitializerList = new List<Action>();

        private static List<int> Scenes;

        public static void Start()
        {
            InitializerList = new List<Action>();
            Scenes = new List<int>();
            ActiveScene = null;
        }

        public static void AddScene(int id)
        {
            foreach (int _id in Scenes)
                if (id == _id)
                    throw new Exception("Scene ID can't be duplicated!");

            Scenes.Add(id);
        }

        public static void RemoveScene(int id)
        {
            Scenes.Remove(id);
        }

        private static void UnloadScene()
        {
            //Freeing the currently loaded assets, must be called before loading any new assets!!
            if (ActiveScene != null)
            {
                Setup.Content.Unload();
                ActiveScene = null;
                GC.Collect();
            }
        }

        public static void AddInitializer(Action initializer, int Index)
        {
            InitializerList.Insert(Index, initializer);
        }

        public static void LoadScene(Scene scene)
        {
            UnloadScene();

            foreach (int _id in Scenes)
                if (scene.ID == _id)
                    ActiveScene = scene;
        }
    }
}
