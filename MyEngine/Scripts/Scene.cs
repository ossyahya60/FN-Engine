using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MyEngine
{
    public class Scene
    {
        public bool Active = false;
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

        public Scene(string name)
        {
            GameObjects = new List<GameObject>();
            IDs = new List<int>();
            Name = name;
        }

        public Scene(string name, int _ID)
        {
            GameObjects = new List<GameObject>();
            IDs = new List<int>();
            Name = name;
            ID = _ID;
        }

        public void CleanScene()
        {
            Active = true;
            GameObjects.Clear();
            GameObjectCount = 0;
            SceneManager.ActiveScene = this;
        }

        public void CleanScene(string Name)
        {
            Active = true;
            GameObjects.Clear();
            GameObjectCount = 0;
            this.Name = Name;
            SceneManager.ActiveScene = this;
        }

        public void CleanScene(string Name, int ID)
        {
            Active = true;
            GameObjects.Clear();
            GameObjectCount = 0;
            this.Name = Name;
            this.ID = ID;
            SceneManager.ActiveScene = this;
        }

        public void AddGameObject(GameObject GO)
        {
            GameObjects.Insert(GameObjectCount, GO);
            GameObjectCount++;
        }

        public void RemoveGameObject(GameObject GO)
        {
            foreach (GameObject gameObject in GameObjects.ToArray())
                if (gameObject.Parent == GO)
                    if(GameObjects.Remove(gameObject))
                        GameObjectCount--;

            if (GameObjects.Remove(GO))
                GameObjectCount--;
        }

        public void Start()
        {
            if(Active)
                foreach (GameObject GO in GameObjects)
                    GO.Start();
        }

        public void Update(GameTime gameTime)
        {
            if(Active)
                foreach (GameObject GO in GameObjects)
                    GO.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(Active)
                foreach (GameObject GO in GameObjects)
                    GO.Draw(spriteBatch);
        }

        public GameObject FindGameObjectWithTag(string Tag)
        {
            foreach (GameObject GO in GameObjects)
                if (GO.Tag == Tag)
                    return GO;

            return null;
        }

        public GameObject FindGameObjectWithName(string Name)
        {
            foreach (GameObject GO in GameObjects)
                if (GO.Name == Name)
                    return GO;

            return null;
        }

        public GameObject[] FindGameObjectsWithTag(string Tag)
        {
            GameObject[] GOs = new GameObject[GameObjects.Count];

            for (int i = 0; i < GameObjects.Count; i++)
                if (GameObjects[i].Tag == Tag)
                    GOs[i] = GameObjects[i];

            return GOs;
        }
    }
}
