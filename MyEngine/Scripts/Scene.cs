using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

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

            GameObject[] Children = GO.GetALLChildren();

            if(Children != null)
                foreach (GameObject Child in Children)
                    if (GameObjects.Remove(Child))
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
                foreach (GameObject GO in GameObjects.ToArray())
                    GO.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
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
            HandyList.Clear();

            int Counter = 0;
            for (int i = 0; i < GameObjects.Count; i++)
                if (GameObjects[i].Tag == Tag)
                    HandyList.Insert(Counter++, GameObjects[i]);

            return HandyList.ToArray();
        }

        public void SortGameObjectsWithLayer()
        {
            GameObjects.Sort(GameObject.SortByLayer());
        }
    }
}
