using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGui;
using System.Collections.Generic;
using System.IO;

namespace MyEngine
{
    public class Scene
    {
        public bool Active = true;
        public List<GameObject> GameObjects;
        public string Name;
        public int ID
        {
            set
            {
                foreach (int id in IDs)
                    if (id == value)
                        throw new System.Exception("Scene ID must be unique");
                IDs.Add(value);
                Id = value;
            }
            get
            {
                return Id;
            }
        }

        private static List<int> IDs;
        private int Id;
        private int GameObjectCount = 0;
        private List<GameObject> HandyList;

        private static RenderTarget2D ImGUI_RenderTarget = null;
        private static ImGUIRenderer GuiRenderer = null; //This is the ImGuiRenderer

        public Scene(string name)
        {
            GameObjects = new List<GameObject>();
            IDs = new List<int>();
            Name = name;
            HandyList = new List<GameObject>();
        }

        public Scene(string name, int _ID)
        {
            GameObjects = new List<GameObject>();
            IDs = new List<int>();
            Name = name;
            ID = _ID;
            HandyList = new List<GameObject>();
        }

        public void AddGameObject(GameObject GO) //=> Implement it using "Recursion"
        {
            if (!GameObjects.Contains(GO))
            {
                GameObjects.Insert(GameObjectCount, GO);
                GameObjectCount++;
            }
        }

        /* //Didn't work as Gameobjects have to be in the simulation in order to be find by "GetChildrenMethod" :(
        public void AddGameObject(GameObject GO) //=> Implement it using "Recursion"
        {
            GameObjects.Insert(GameObjectCount, GO);
            GameObjectCount++;

            AddGameObjectRecursive(GO);
        }

        private void AddGameObjectRecursive(GameObject GO)
        {
            GameObject[] Children = GO.GetChildrenIfExist();

            if (Children.Length == 0)
                return;

            foreach (GameObject Child in Children)
            {
                GameObjects.Insert(GameObjectCount, Child);
                GameObjectCount++;

                AddGameObjectRecursive(Child);
            }
        }
        */

        public void RemoveGameObject(GameObject GO) //=> Implement it using "Recursion"
        {
            if (GO == null)
                return;

            foreach (GameObject Child in GO.Children)
                RemoveGameObject(Child);

            GO.Destroy();
            GameObjects.Remove(GO);
            GameObjectCount--;
        }

        public void Start()
        {
            if(GuiRenderer == null)
                GuiRenderer = new ImGUIRenderer(Setup.Game).Initialize().RebuildFontAtlas();

            if(ImGUI_RenderTarget == null)
                ImGUI_RenderTarget = new RenderTarget2D(Setup.GraphicsDevice, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight, false, Setup.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            int Count = GameObjects.Count - 1;

            if (Active)
                for (int i = Count; i >= 0; i--)
                    GameObjects[Count - i].Start();
        }

        public void Update(GameTime gameTime)
        {
            int Count = GameObjects.Count - 1;

            if (Active)
                for (int i = Count; i >= 0; i--)
                    GameObjects[Count - i].Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int Count = GameObjects.Count - 1;

            if (Active)
                for (int i = Count; i >= 0; i--)
                    GameObjects[Count - i].Draw(spriteBatch);

            foreach (GameObject GO in GameObjects.FindAll(item => item.ShouldBeDeleted == true))
            {
                GO.Destroy();
                RemoveGameObject(GO);
            }
        }

        public void DrawUI(GameTime gameTime)
        {
            Setup.GraphicsDevice.SetRenderTarget(ImGUI_RenderTarget);
            Setup.GraphicsDevice.Clear(Color.Transparent);

            GuiRenderer.BeginLayout(gameTime); // Must be called prior to calling any ImGui controls
            
            int Count = GameObjects.Count - 1;

            if (Active)
                for (int i = Count; i >= 0; i--)
                    GameObjects[Count - i].DrawUI();

            GuiRenderer.EndLayout(); // Must be called after ImGui control calls
            Setup.GraphicsDevice.SetRenderTarget(null);
        }

        public void ShowUI(SpriteBatch spriteBatch)
        {
            // Drawing ImGUI Stuff
            Setup.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, null);
            spriteBatch.Draw(ImGUI_RenderTarget, Vector2.Zero, new Rectangle(0, 0, Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
        }

        public GameObject FindGameObjectWithTag(string Tag)
        {
            int Count = GameObjects.Count - 1;

            for (int i = Count; i >= 0; i--)
                if (GameObjects[Count - i].Tag == Tag)
                    return GameObjects[Count - i];

            return null;
        }

        public GameObject FindGameObjectWithName(string Name)
        {
            int Count = GameObjects.Count - 1;

            for (int i = Count; i >= 0; i--)
                if (GameObjects[Count - i].Name == Name)
                    return GameObjects[Count - i];

            return null;
        }

        public GameObject[] FindGameObjectsWithTag(string Tag)
        {
            HandyList.Clear();

            int Counter = 0;
            int Count = GameObjects.Count - 1;

            for (int i = Count; i >= 0; i--)
                if (GameObjects[Count - i].Tag == Tag)
                    HandyList.Insert(Counter++, GameObjects[Count - i]);

            return HandyList.ToArray();
        }

        public T[] FindGameObjectComponents<T>() where T : GameObjectComponent
        {
            List<T> HandyList2 = new List<T>();

            int Counter = 0;
            int Count = GameObjects.Count - 1;

            for (int i = Count; i >= 0; i--)
            {
                var GOC = GameObjects[Count - i].GetComponent<T>();
                if (GOC != null && GOC.gameObject.IsActive() == true)
                {
                    HandyList2.Insert(Counter++, GOC);
                }
            }

            return HandyList2.ToArray() as T[];
        }

        public void SortGameObjectsWithLayer()
        {
            GameObjects.Sort(GameObject.SortByLayer());
        }

        public void Serialize()
        {
            using (StreamWriter SW = new StreamWriter(Setup.Content.RootDirectory + "/" + Name + ".txt", false)) //This Path might be platform specific!
            {
                List<GameObject> EditorGOs = GameObjects.FindAll(item => item.IsEditor == true);
                int NumberOfEditorObjects = (EditorGOs == null) ? 0 : EditorGOs.Count;

                SW.WriteLine(ToString());

                SW.Write("Active:\t" + Active.ToString() + "\n");
                SW.Write("Name:\t" + Name + "\n");
                SW.Write("Id:\t" + Id + "\n");
                SW.Write("GameObjectCount:\t" + (GameObjectCount - NumberOfEditorObjects).ToString() + "\n");

                MediaSource.Serialize(SW);
                Setup.Camera.Serialize(SW);

                foreach (GameObject GO in GameObjects)
                    if(!GO.IsEditor)
                        GO.Serialize(SW);

                SW.WriteLine("End Of " + ToString());

                SW.Close();
            }
        }

        public void Deserialize(string Path)
        {
            using (StreamReader SR = new StreamReader(Setup.Content.RootDirectory + "/" + Path + ".txt", false)) //This Path might be platform specific!
            {
                SR.ReadLine();

                Active = bool.Parse(SR.ReadLine().Split('\t')[1]);
                Name = SR.ReadLine().Split('\t')[1];
                Id = int.Parse(SR.ReadLine().Split('\t')[1]);
                int _GameObjectCount = int.Parse(SR.ReadLine().Split('\t')[1]);

                //MediaSource Stuff
                SR.ReadLine();

                MediaSource.Volume = float.Parse(SR.ReadLine().Split('\t')[1]);
                MediaSource.IsMuted = bool.Parse(SR.ReadLine().Split('\t')[1]);
                MediaSource.IsLooping = bool.Parse(SR.ReadLine().Split('\t')[1]);
                string SongName = SR.ReadLine().Split('\t')[1];
                if(SongName != "null")
                    MediaSource.LoadTrack(SongName);

                SR.ReadLine();

                //Camera Stuff
                SR.ReadLine();

                Setup.Camera.Zoom  = float.Parse(SR.ReadLine().Split('\t')[1]);
                Setup.Camera.Rotation  = float.Parse(SR.ReadLine().Split('\t')[1]);
                string[] CamPos = SR.ReadLine().Split('\t');
                Setup.Camera.Position = new Vector2(float.Parse(CamPos[1]), float.Parse(CamPos[2]));

                SR.ReadLine();

                //Deserializing GameObjects
                for (int i = 0; i < _GameObjectCount; i++)
                {
                    GameObject GO = new GameObject();
                    GO.Deserialize(SR);
                    AddGameObject(GO);
                }

                SR.ReadLine();

                SR.Close();

                //Assiging parents and children properly
                for (int i = 0; i < _GameObjectCount; i++)
                {
                    //Finding Parent if there is one
                    if (GameObjects[i].Parent.Name == "null")
                        GameObjects[i].Parent = null;
                    else
                        GameObjects[i].Parent = FindGameObjectWithName(GameObjects[i].Parent.Name);

                    //Finding children
                    for (int j = 0; j < GameObjects[i].Children.Count; j++)
                        GameObjects[i].Children[j] = FindGameObjectWithName(GameObjects[i].Children[j].Name);
                }
            }
        }
    }
}
