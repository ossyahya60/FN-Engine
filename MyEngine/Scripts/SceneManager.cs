using System;
using System.Collections.Generic;

namespace MyEngine
{
    public static class SceneManager
    {
        public static Scene ActiveScene;
        public static Dictionary<int, Action> InitializerList;

        private static Scene SceneToBeLoaded = null;
        private static bool FirstTimeLoading = true;

        public static void Start()
        {
            InitializerList = new Dictionary<int, Action>();
            ActiveScene = null;
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
            InitializerList.Add(Index, initializer);
        }

        public static void LoadSceneNow() //Not For High level user
        {
            if (SceneToBeLoaded == null)
                return;

            UnloadScene();
            ActiveScene = SceneToBeLoaded;

            foreach (KeyValuePair<int, Action> KVP in InitializerList)
                if (KVP.Key == ActiveScene.ID)
                    KVP.Value.Invoke();

            SceneToBeLoaded = null;
        }

        public static void LoadScene(Scene scene) //Use this
        {
            if (FirstTimeLoading)
            {
                SceneToBeLoaded = scene;
                LoadSceneNow();
                FirstTimeLoading = false;
                return;
            }

            SceneToBeLoaded = scene;
        }
    }
}