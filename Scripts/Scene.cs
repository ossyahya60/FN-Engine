using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;

namespace FN_Engine
{
    public class Scene
    {
        public static ImGUI.ImGuiRenderer GuiRenderer { private set; get; } = null; //This is the ImGuiRenderer
        public static bool IsSceneBeingLoaded = false;

        public bool ShouldSort = false; //Should be sorted every frame?
        public bool Active = true;
        public string Name;
        public List<GameObject> GameObjects;

        internal List<Rigidbody2D> Rigidbody2Ds;

        private static RenderTarget2D ImGUI_RenderTarget = null;

        private ImFontPtr LoadedFont;

        public Scene(string name)
        {
            GameObjects = new List<GameObject>();
            Name = name;
            Rigidbody2Ds = new List<Rigidbody2D>();
        }

        public Scene()
        {
            GameObjects = new List<GameObject>();
            Rigidbody2Ds = new List<Rigidbody2D>();
        }

        internal T[] FindGameObjectComponents<T>() where T : GameObjectComponent //not efficient. Please, don't use this!
        {
            List<T> HandyList2 = new List<T>();

            int Counter = 0;
            int Count = GameObjects.Count - 1;

            for (int i = Count; i >= 0; i--)
            {
                var GOC = GameObjects[Count - i].GetComponent<T>();
                if (GOC != null && GOC.gameObject.IsActive() == true)
                    HandyList2.Insert(Counter++, GOC);
            }

            return HandyList2.ToArray();
        }

        public void AddGameObject_Recursive(GameObject GO, bool Root = true, bool NoNameCheck = false) // Adds the GO along with its children and so on
        {
            if (GO == null)
                return;

            if (!GameObjects.Contains(GO))
            {
                if (!NoNameCheck)
                    GO.Name = Utility.UniqueGameObjectName(GO.Name);
                GameObjects.Insert(GameObjects.Count, GO);

                bool RemovedOrDeleted = false;
                //Light L = GO.GetComponent<Light>();
                if (GO.ShouldBeRemoved || GO.ShouldBeDeleted)
                {
                    RemovedOrDeleted = true;
                    GO.ShouldBeRemoved = false;
                    GO.ShouldBeDeleted = false;

                    //if (L != null)
                    //    L.Rebuild();
                }

                foreach (GameObject Child in GO.Children)
                {
                    Child.ShouldBeRemoved = RemovedOrDeleted;
                    AddGameObject_Recursive(Child, false);
                }

                if (Root)
                    SortGameObjectsWithLayer();
            }
        }

        public void RemoveGameObject(GameObject GO, bool DestroyToo = true, bool Root = true) //=> Implement it using "Recursion"
        {
            if (GO == null)
                return;

            for (int i = 0; i < GO.Children.Count; i++)
                RemoveGameObject(GO.Children[i], DestroyToo, false);

            if (GO.Parent != null && Root)
                GO.Parent.RemoveChild(GO);

            if (DestroyToo)
                GO.Destroy();
            else
            {
                Light L = GO.GetComponent<Light>();
                if (L != null)
                    L.Destroy();
            }

            GameObjects.Remove(GO);
        }

        public void Start()
        {
            if (GuiRenderer == null)
            {
                GuiRenderer = new ImGUI.ImGuiRenderer(Setup.Game);
                LoadedFont = ImGui.GetIO().Fonts.AddFontFromFileTTF("Roboto-Regular.ttf", 15);
                GuiRenderer.RebuildFontAtlas();
            }

            if (ImGUI_RenderTarget == null)
                ImGUI_RenderTarget = new RenderTarget2D(Setup.GraphicsDevice, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight, false, Setup.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            int Count = GameObjects.Count - 1;

            //if (Active)
            for (int i = Count; i >= 0; i--)
                if(!GameObjects[Count - i].IsEditor)
                    GameObjects[Count - i].Start();
        }

        internal void Init()
        {
            if (GuiRenderer == null)
            {
                GuiRenderer = new ImGUI.ImGuiRenderer(Setup.Game);
                LoadedFont = ImGui.GetIO().Fonts.AddFontFromFileTTF("Roboto-Regular.ttf", 15);
                GuiRenderer.RebuildFontAtlas();
            }

            if (ImGUI_RenderTarget == null)
                ImGUI_RenderTarget = new RenderTarget2D(Setup.GraphicsDevice, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight, false, Setup.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
        }

        public void Update(GameTime gameTime)
        {
            int Count = GameObjects.Count - 1;

            if (Active)
            {
                Rigidbody2Ds.Clear();

                for (int i = Count; i >= 0; i--)
                {
                    if (GameObjects[Count - i].Active)
                    {
                        //Adjusting Transforms
                        if (!FN_Editor.EditorScene.IsThisTheEditor && GameObjects[Count - i].Parent == null && GameObjects[Count - i].Transform != null)
                        {
                            Queue<Transform> Transforms = new Queue<Transform>();

                            Transforms.Enqueue(GameObjects[Count - i].Transform);
                            QueueInOrder(Transforms);
                        }
                    }
                }

                for (int i = Count; i >= 0; i--)
                    GameObjects[Count - i].Update(gameTime);
            }

            //Update Physics
            foreach (Rigidbody2D RB in Rigidbody2Ds)
                RB.Update(gameTime);

            CollisionHandler.Update(gameTime);
        }

        private void QueueInOrder(Queue<Transform> queue)
        {
            if (queue.Count == 0)
                return;

            Transform GO = queue.Dequeue();
            GO.AdjustedTransform = GO.AdjustTransformation();

            foreach (GameObject Child in GO.gameObject.Children)
                if (Child.Active)
                    queue.Enqueue(Child.Transform);

            QueueInOrder(queue);
        }

        internal void UpdateUI(GameTime gameTime)
        {
            int Count = GameObjects.Count - 1;

            if (Active)
            {
                for (int i = Count; i >= 0; i--)
                {
                    //Adjusting Transforms
                    if (GameObjects[Count - i].Parent == null && GameObjects[Count - i].Transform != null && GameObjects[Count - i].Active)
                    {
                        Queue<Transform> Transforms = new Queue<Transform>();

                        Transforms.Enqueue(GameObjects[Count - i].Transform);
                        QueueInOrder(Transforms);
                    }
                }

                for (int i = Count; i >= 0; i--)
                    GameObjects[Count - i].UpdateUI(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int Count = GameObjects.Count - 1;

            if (Active)
                for (int i = Count; i >= 0; i--)
                    GameObjects[Count - i].Draw(spriteBatch);

            foreach (GameObject GO in GameObjects.FindAll(item => item.ShouldBeDeleted | item.ShouldBeRemoved))
                RemoveGameObject(GO, GO.ShouldBeDeleted);
        }

        public void DrawUI(GameTime gameTime)
        {
            Setup.GraphicsDevice.SetRenderTarget(ImGUI_RenderTarget);
            Setup.GraphicsDevice.Clear(Color.Transparent);

            GuiRenderer.BeforeLayout(gameTime); // Must be called prior to calling any ImGui controls

            ImGui.PushFont(LoadedFont);
            ImGui.GetIO().ConfigWindowsResizeFromEdges = true;
            ImGui.GetStyle().FrameRounding = 12;
            //ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 2);

            int Count = GameObjects.Count - 1;

            if (Active)
                for (int i = Count; i >= 0; i--)
                    GameObjects[Count - i].DrawUI();

            if(FN_Editor.EditorScene.IsThisTheEditor && Input.GetMouseClickUp(MouseButtons.LeftClick))
            {
                FN_Editor.GameObjects_Tab.DraggedGO = null;
                FN_Editor.ContentWindow.DraggedAsset = null;
                FN_Editor.InspectorWindow.DraggedObject = null;
            }

            //ImGui.PopStyleVar();
            ImGui.PopFont();

            GuiRenderer.AfterLayout(); // Must be called after ImGui control calls
            Setup.GraphicsDevice.SetRenderTarget(null);
        }

        public void ShowUI(SpriteBatch spriteBatch)
        {
            // Drawing ImGUI Stuff
            Setup.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, null);
            spriteBatch.Draw(ImGUI_RenderTarget, Vector2.Zero, new Rectangle(0, 0, (int)ResolutionIndependentRenderer.GetVirtualRes().X, (int)ResolutionIndependentRenderer.GetVirtualRes().Y), Color.White);
            spriteBatch.End();
        }

        public GameObject FindGameObjectWithName(string Name)
        {
            return GameObjects.Find(Item => Item.Name == Name);
        }

        public List<GameObject> FindGameObjectsWithTag(string Tag)
        {
            return GameObjects.FindAll(Item => Item.Tag == Tag);
        }

        //public T[] FindGameObjectComponents<T>() where T : GameObjectComponent //not efficient
        //{
        //    List<T> HandyList2 = new List<T>();

        //    int Counter = 0;
        //    int Count = GameObjects.Count - 1;

        //    for (int i = Count; i >= 0; i--)
        //    {
        //        var GOC = GameObjects[Count - i].GetComponent<T>();
        //        if (GOC != null && GOC.gameObject.IsActive() == true)
        //            HandyList2.Insert(Counter++, GOC);
        //    }

        //    return HandyList2.ToArray();
        //}

        public void SortGameObjectsWithLayer()
        {
            ShouldSort = true;
        }

        internal void SerializeV2(string Path = "")
        {
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\" + Path.Substring(0, Path.LastIndexOf("\\")));
            using (StreamWriter SW = new StreamWriter(Path + ".scene", false)) //Remember to serialize and deserialize Camera and RIR
            {
                Utility.OIG = new System.Runtime.Serialization.ObjectIDGenerator();

                JsonTextWriter JW = new JsonTextWriter(SW);
                JW.Formatting = Formatting.Indented;

                Utility.SerializeV2(JW, this);

                JW.Close();
                SW.Close();
            }
        }
    }
}