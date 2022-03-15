using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace FN_Engine
{
    public static class SceneManager
    {
        public static Scene ActiveScene { internal set; get; }

        internal static bool ShouldUpdate = true;
        internal static string LastLoadPath = "";
        internal static IntPtr SceneTexPtr = IntPtr.Zero;

        private static Scene SceneToBeLoaded = null;
        private static bool FirstTimeLoading = true;

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
            if (FN_Editor.EditorScene.IsThisTheEditor && ShouldUpdate)
                return;

            LastLoadPath = Name;

            if (FirstTimeLoading)
            {
                SceneToBeLoaded = new Scene(Name);
                LoadSceneNow_Serialization();
                FirstTimeLoading = false;
                return;
            }

            SceneToBeLoaded = new Scene(Name);
        }

        internal static void LoadSceneNow_Serialization() //Not For High level user
        {
            if (SceneToBeLoaded == null)
                return;


            MediaSource.Stop();
            MediaSource.Dispose();

            UnloadScene();

            Scene.IsSceneBeingLoaded = true;
            if(FN_Editor.EditorScene.IsThisTheEditor)
                ActiveScene = DeserlializeV2(SceneToBeLoaded.Name + "_Editor");
            else
                ActiveScene = DeserlializeV2(SceneToBeLoaded.Name);

            Light.Reset();
            if (FN_Editor.EditorScene.IsThisTheEditor) //Some clean up
            {
                ActiveScene.Init();
                ActiveScene.GameObjects.Find(Item => Item.IsEditor).Start();

                FN_Editor.GameObjects_Tab.Undo_Buffer.Clear();
                FN_Editor.GameObjects_Tab.Redo_Buffer.Clear();
            }
            else
                ActiveScene.Start();

            Scene.IsSceneBeingLoaded = false;
            SceneToBeLoaded = null;
        }

        public static void SerializeScene(string Path = "")
        {
            if (ActiveScene != null && !(FN_Editor.EditorScene.IsThisTheEditor && ShouldUpdate))
            {
                var CamCont = ActiveScene.FindGameObjectWithName("Camera Controller").GetComponent<CameraController>();
                bool StoredVal = CamCont.Visualize;
                CamCont.Visualize = false;

                GameObject EditorGameObject = null;
                if (FN_Editor.EditorScene.IsThisTheEditor)
                {
                    EditorGameObject = ActiveScene.FindGameObjectWithName("EditorGameObject");
                    ActiveScene.RemoveGameObject(EditorGameObject, false);

                    ActiveScene.SerializeV2("Scenes\\" + Path);

                    ActiveScene.AddGameObject_Recursive(EditorGameObject);

                    ActiveScene.SerializeV2("Scenes\\" + Path + "_Editor");
                }
                else
                    ActiveScene.SerializeV2("Scenes\\" + Path);

                CamCont.Visualize = StoredVal;
            }
        }

        public static void Initialize()
        {
            if (ActiveScene != null)
                ActiveScene.Start();
        }

        public static void Update(GameTime gameTime)
        {
            // I moved this here, because Update rate is not the same as draw rate, so Input is not synchronized well
            ActiveScene.DrawUI(gameTime); //Draw UI

            if (ActiveScene != null && ShouldUpdate)
                ActiveScene.Update(gameTime);

            if (ActiveScene != null /*&& FN_Editor.EditorScene.IsThisTheEditor*/)
                ActiveScene.UpdateUI(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            if (ActiveScene != null)
            {
                //if (!Setup.Game.IsActive) //Pause Game when minimized
                    //return;

                if (ActiveScene.ShouldSort)
                {
                    ActiveScene.GameObjects = ActiveScene.GameObjects.OrderByDescending(Item => Item.Layer).ToList();
                    ActiveScene.ShouldSort = false;
                }

                ///
                Light.Init_Light();
                ResolutionIndependentRenderer.BeginDraw(); //Resolution related -> Mandatory

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