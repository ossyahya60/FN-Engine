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

            if (Children != null)
            {
                int Count = GameObjects.Count - 1;

                for (int i = Count; i >= 0; i--)
                    if (GameObjects.Remove(GameObjects[Count - i]))
                        GameObjectCount--;
            }

            if (GO.Parent != null)
                GO.Parent.RemoveChild(GO);

            if (GameObjects.Remove(GO))
                GameObjectCount--;
        }

        public void Start()
        {
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

            foreach (GameObject GO in GameObjects.FindAll(item => item.ShouldBeDeleted == true))
                GO.Destroy();

            int Length = GameObjects.RemoveAll(item => item.ShouldBeDeleted == true);
            GameObjectCount -= Length;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int Count = GameObjects.Count - 1;

            if (Active)
                for (int i = Count; i >= 0; i--)
                    GameObjects[Count - i].Draw(spriteBatch);
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
                if (GOC != null && GOC.gameObject.Active == true)
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
    }
}
