using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private static Vector2 Resolution = Vector2.Zero;

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

            scene.GameObjects.Clear();
            SceneToBeLoaded = scene;
        }

        public static void LoadScene_Serialization(string Name) //Use this
        {
            FN_Editor.InspectorWindow.Members.Clear();

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
            FN_Editor.InspectorWindow.Members.Clear();

            if (SceneToBeLoaded == null)
                return;

            UnloadScene();
            ActiveScene = SceneToBeLoaded;

            SceneToBeLoaded.Start();
            SceneToBeLoaded.Deserialize(SceneToBeLoaded.Name);

            SceneToBeLoaded = null;
        }

        public static void Initialize()
        {
            if (ActiveScene != null)
                ActiveScene.Start();
        }

        public static void Update(GameTime gameTime)
        {
            if (!Setup.Game.IsActive) //Pause Game when minimized
                return;

            Input.GetState(); //This has to be called at the start of update method!!

            /////////Resolution related//////////// -> Mandatory
            if (Resolution != new Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight))
                Setup.resolutionIndependentRenderer.InitializeResolutionIndependence(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight, Setup.Camera);

            Resolution = new Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight);

            if (ActiveScene != null)
                ActiveScene.Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            if (ActiveScene != null)
            {
                if (!Setup.Game.IsActive) //Pause Game when minimized
                    return;

                ActiveScene.DrawUI(gameTime); //Draw UI

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

                LoadSceneNow();
            }
        }
    }
}