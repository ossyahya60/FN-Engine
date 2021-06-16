using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace MyEngine
{
    public static class SceneManager
    {
        public static Scene ActiveScene = null;
        public static Dictionary<int, Action> InitializerList = new Dictionary<int, Action>();
        public static bool ShouldUpdate = true;

        private static Scene SceneToBeLoaded = null;
        private static bool FirstTimeLoading = true;
        private static Vector2 Resolution = Vector2.Zero;
        private static string ScenesDirectory = "";
        private static string DefaultScene = "Main Scene";
        private static List<string> Scenes = new List<string>();


        private static void ReadSceneFile()
        {
            using (StreamReader SR = new StreamReader(ScenesDirectory + "/Scenes/Scenes.FN", false))
            {
                DefaultScene = SR.ReadLine().Split('\t')[1];
                
                int NumberOfScenes = int.Parse(SR.ReadLine().Split('\t')[1]);
                for (int i = 0; i < NumberOfScenes; i++)
                    Scenes.Add(SR.ReadLine());
            }
        }

        private static void SaveSceneFile()
        {
            using (StreamWriter SW = new StreamWriter(ScenesDirectory + "/Scenes/Scenes.FN", false))
            {
                SW.WriteLine("Default Scene:\t" + DefaultScene);

                string[] SceneNames = Directory.GetFiles(ScenesDirectory + "/Scenes").Where(Item => Item.Contains(".FN")).ToArray();

                SW.WriteLine("Number Of Scenes:\t" + SceneNames.Length.ToString());

                foreach (string S in SceneNames)
                    SW.WriteLine(S);
            }
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

            scene.GameObjects.Clear();
            SceneToBeLoaded = scene;
        }

        private static Scene DeserlializeV2(string Path = "")
        {
            using (StreamReader SR = new StreamReader(Path + ".scene", false))
            {
                Utility.OIG = new System.Runtime.Serialization.ObjectIDGenerator();

                JsonTextReader JR = new JsonTextReader(SR);
                JR.Read(); // {

                Dictionary<long, object> SerializedObjects = new Dictionary<long, object>();
                Scene scene = Utility.DeserializeV2(JR, SerializedObjects) as Scene;

                JR.Close();
                SR.Close();

                return scene;
            }
        }

        public static void LoadScene_Serialization(string Name) //Use this
        {
            if (FirstTimeLoading)
            {
                SceneToBeLoaded = new Scene(Name);
                LoadSceneNow_Serialization();
                FirstTimeLoading = false;
                return;
            }

            SceneToBeLoaded = new Scene(Name);
        }

        public static void LoadSceneNow_Serialization() //Not For High level user
        {
            if (SceneToBeLoaded == null)
                return;

            UnloadScene();
            //Utility.BuildAllContent(Directory.GetCurrentDirectory());
            ActiveScene = DeserlializeV2(SceneToBeLoaded.Name + "_Editor");

            Light.Reset();
            ActiveScene.Start();

            SceneToBeLoaded = null;
        }

        public static void SerializeScene(string Path = "")
        {
            if (ActiveScene != null)
            {
                GameObject EditorGameObject = null;
                if (FN_Editor.EditorScene.IsThisTheEditor)
                {
                    EditorGameObject = ActiveScene.FindGameObjectWithName("EditorGameObject");
                    ActiveScene.RemoveGameObject(EditorGameObject, false);

                    ActiveScene.SerializeV2(Path);

                    ActiveScene.AddGameObject_Recursive(EditorGameObject);

                    ActiveScene.SerializeV2(Path + "_Editor");
                }
                else
                    ActiveScene.SerializeV2(Path);
            }
        }

        //public static void LoadSceneNow_Serialization() //Not For High level user
        //{
        //    if (SceneToBeLoaded == null)
        //        return;

        //    UnloadScene();
        //    ActiveScene = SceneToBeLoaded;


        //    //SceneToBeLoaded.Start();
        //    SceneToBeLoaded.DeserializeV2(SceneToBeLoaded.Name);
        //    Light.Reset();
        //    SceneToBeLoaded.Start();

        //    SceneToBeLoaded = null;
        //}

        public static void Initialize()
        {
            if (ActiveScene != null)
                ActiveScene.Start();
        }

        public static void Update(GameTime gameTime)
        {
            if (!Setup.Game.IsActive) //Pause Game when minimized
                return;

            if (Input.GetKey(Keys.RightAlt) && Input.GetKeyUp(Keys.Enter))
                Setup.graphics.ToggleFullScreen();

            /////////Resolution related//////////// -> Mandatory
            if (Resolution != new Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight))
                Setup.resolutionIndependentRenderer.InitializeResolutionIndependence(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight, Setup.Camera);

            Resolution = new Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight);

            if (ActiveScene != null && ShouldUpdate /*&& !FN_Editor.EditorScene.IsThisTheEditor*/)
                ActiveScene.Update(gameTime);

            // I moved this here, because Update rate is not the same as draw rate, so Input is not synchronized well
            ActiveScene.DrawUI(gameTime); //Draw UI
        }

        public static void Draw(GameTime gameTime)
        {
            if (ActiveScene != null)
            {
                if (!Setup.Game.IsActive) //Pause Game when minimized
                    return;

                if (ActiveScene.ShouldSort)
                {
                    ActiveScene.GameObjects = ActiveScene.GameObjects.OrderByDescending(Item => Item.Layer).ToList();
                    ActiveScene.ShouldSort = false;
                }

                ///
                Light.Init_Light();
                Setup.resolutionIndependentRenderer.BeginDraw(); //Resolution related -> Mandatory

                Setup.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Setup.Camera.GetViewTransformationMatrix()); // -> Mandatory
                SpriteRenderer.LastEffect = null; // This should be the same effect as in the begin method above
                ActiveScene.Draw(Setup.spriteBatch);
                Setup.spriteBatch.End();

                //Light (Experimental)
                Light.ApplyLighting();

                ActiveScene.ShowUI(Setup.spriteBatch);

                LoadSceneNow_Serialization();
            }
        }
    }
}