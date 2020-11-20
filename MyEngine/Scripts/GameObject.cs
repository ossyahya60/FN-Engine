using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MyEngine
{
    public class GameObject
    {
        public Transform Transform
        {
            get
            {
                return GetComponent<Transform>();
            }
        }
        public GameObject Parent = null; 
        public List<GameObjectComponent> GameObjectComponents; //List of all  GO componets in a certain scene(scene is not yet implemented)
        public string Tag = null;
        public int GameComponentsCount = 0;
        public bool Active
        {
            set
            {
                active = value;
                foreach (GameObject GO in GetChildrenIfExist())
                    GO.Active = value;
            }
            get
            {
                return active;
            }
        }
        public string Name = "Default";

        private readonly string[] CanBeAddedMultipleTimes = { "BoxCollider2D", "AudioSource", "ParticleEffect" };
        private List<GameObject> HandyList;
        private bool active = true;

        public GameObject()
        {
            GameObjectComponents = new List<GameObjectComponent>();
            HandyList = new List<GameObject>();
        }

        public T GetComponent<T>() where T : GameObjectComponent
        {
            foreach (GameObjectComponent GOC in GameObjectComponents)
                if (GOC is T)
                    return (T)GOC;

            return null;
        }

        public void AddComponent<T>(T component) where T : GameObjectComponent  //Add a component to a gameobject
        {
            bool CanBeAdded = false;
            foreach (string s in CanBeAddedMultipleTimes)
                if (s == component.GetType().ToString())
                    CanBeAdded = true;

            if (!GameObjectComponents.Contains(component) || CanBeAdded)
            {
                GameObjectComponents.Insert(GameComponentsCount, component);
                GameObjectComponents[GameComponentsCount].gameObject = this;
                GameComponentsCount++;
            }
        }

        public void RemoveComponent<T>(T component) where T : GameObjectComponent  //Remove a component from a gameobject
        {
            if (GameObjectComponents.Contains(component))
            {
                GameObjectComponents.Remove(component);
                GameComponentsCount--;
            }
        }

        //public T ReturnType(GameObjectComponent GOC) where T: GameObjectComponent
        //{
        //    return T;
        //}

        public GameObjectComponent[] GetAllComponents()
        {
            if (GameObjectComponents.Count == 0)
                return null;
            else
                return GameObjectComponents.ToArray();
        }

        public GameObject[] GetChildrenIfExist()
        {
            HandyList.Clear();

            foreach (GameObject GO in SceneManager.ActiveScene.GameObjects)
                if (GO.Parent == this)
                    HandyList.Add(GO);

            HandyList.Reverse();
            return HandyList.ToArray();
        }

        public virtual void Start()
        {
            foreach (GameObjectComponent GOC in GameObjectComponents)
                if (GOC.Enabled)
                    GOC.Start();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Active)
            {
                foreach (GameObjectComponent GOC in GameObjectComponents)
                    if (GOC.Enabled)
                        GOC.Update(gameTime);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                foreach (GameObjectComponent GOC in GameObjectComponents)
                    if (GOC.Enabled)
                        GOC.Draw(spriteBatch);
            }
        }

        //public static void Destroy(GameObject GO)  //Scene not yet implemented
        //{
        //    Scene.DestroyGameObject(GO); 
        //}
        
        //Update: Implemented using recursion, this means, you don't have to add every child to the simulation, just add the parent and you are good to go
        public static GameObject Instantiate(GameObject GO)  //=> Implement it using "Recursion"
        {
            GameObject Clone = new GameObject();
            Clone.Parent = GO.Parent;
            Clone.Tag = GO.Tag;
            Clone.Active = GO.Active;

            for (int i = 0; i < GO.GameObjectComponents.Count; i++)
                Clone.AddComponent<GameObjectComponent>(GO.GameObjectComponents[i].DeepCopy(Clone));

            SceneManager.ActiveScene.AddGameObject(Clone);

            InstantiateRecursive(GO, Clone);

            return Clone;
        }

        public static GameObject Instantiate(GameObject GO, GameObject parent)  //=> Implement it using "Recursion"
        {
            GameObject Clone = new GameObject();
            Clone.Parent = parent;
            Clone.Tag = GO.Tag;
            Clone.Active = GO.Active;

            for (int i = 0; i < GO.GameObjectComponents.Count; i++)
                Clone.AddComponent<GameObjectComponent>(GO.GameObjectComponents[i].DeepCopy(Clone));

            SceneManager.ActiveScene.AddGameObject(Clone);

            InstantiateRecursive(GO, Clone);

            return Clone;
        }

        private static void InstantiateRecursive(GameObject GO, GameObject Parent)
        {
            GameObject[] Children = GO.GetChildrenIfExist();

            if (Children.Length == 0)
                return;

            foreach (GameObject Child in Children)
            {
                GameObject ChildClone = new GameObject();
                ChildClone.Parent = Parent;
                ChildClone.Tag = Child.Tag;
                ChildClone.Active = Child.Active;

                for (int i = 0; i < Child.GameObjectComponents.Count; i++)
                    ChildClone.AddComponent<GameObjectComponent>(Child.GameObjectComponents[i].DeepCopy(ChildClone));

                SceneManager.ActiveScene.AddGameObject(ChildClone);

                InstantiateRecursive(Child, ChildClone);
            }
        }
    }
}