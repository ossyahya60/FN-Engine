using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MyEngine
{
    public class Scene
    {
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

        public void AddGameObject(GameObject GO)
        {
            GameObjects.Add(GO);
        }

        public void RemoveGameObject(GameObject GO)
        {
            foreach (GameObject gameObject in GameObjects.ToArray())
                if (gameObject.Parent == GO)
                    GameObjects.Remove(gameObject);

            GameObjects.Remove(GO);
        }

        public void Start()
        {
            foreach (GameObject GO in GameObjects)
                GO.Start();
        }

        public void Update(GameTime gameTime)
        {
            foreach (GameObject GO in GameObjects)
                GO.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
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

        public GameObject[] FindGameObjectsWithTag(string Tag)
        {
            GameObject[] GOs = new GameObject[GameObjects.Count];
            for (int i=0; i<GameObjects.Count; i++)
                if (GameObjects[i].Tag == Tag)
                    GOs[i] = GameObjects[i];

            return GOs;
        }
    }
}
