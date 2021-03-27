using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MyEngine
{
    public class GameObject: IComparer<GameObject>
    {
        public bool IsEditor = false;

        public Transform Transform;
        public float Layer = 1;
        public GameObject Parent = null; 
        public List<GameObjectComponent> GameObjectComponents; //List of all  GO componets in a certain scene(scene is not yet implemented)
        public string Tag = null;
        public int GameComponentsCount = 0;
        public bool Active { set; private get; }
        public string Name = "Default";
        public bool ShouldBeDeleted = false;
        public int UI_Layer = 1;
        public List<GameObject> Children;

        private readonly string[] CanBeAddedMultipleTimes = { "BoxCollider2D", "AudioSource", "ParticleEffect", "CircleCollider" };

        public GameObject()
        {
            Active = true;
            GameObjectComponents = new List<GameObjectComponent>();
            Children = new List<GameObject>();
        }

        public bool IsActive()
        {
            GameObject parent = Parent;

            while (parent != null)
            {
                if (!parent.Active)
                    return false;

                parent = parent.Parent;
            }

            return Active;
        }

        public GameObject(bool IsEditor)
        {
            this.IsEditor = IsEditor;
            Active = true;
            GameObjectComponents = new List<GameObjectComponent>();
            Children = new List<GameObject>();
        }

        public bool AddChild(GameObject Child)
        {
            if (Child == null)
                return false;

            Child.Parent = this;
            Children.Add(Child);

            return true;
        }

        public bool RemoveChild(GameObject Child)
        {
            if (Child == null)
                return false;

            foreach (GameObject child in Children)
            {
                if(Children.Contains(Child))
                {
                    Child.Parent = null;
                    Children.Remove(Child);
                    return true;
                }

                if (RemoveChild(child))
                    break;
            }

            return false;
        }

        public bool RemoveChild(string Tag)
        {
            foreach (GameObject child in Children)
            {
                if (child.Tag == Tag)
                {
                    child.Parent = null;
                    Children.Remove(child);
                    return true;
                }

                if (RemoveChild(Tag))
                    break;
            }

            return false;
        }

        public void RemoveChildren(string Tag)
        {
            foreach (GameObject child in Children)
            {
                RemoveChild(Tag);

                if (child.Tag == Tag)
                {
                    child.Parent = null;
                    Children.Remove(child);
                }
            }
        }

        public bool RemoveChildWithName(string Name)
        {
            foreach (GameObject child in Children)
            {
                if (child.Tag == Name)
                {
                    child.Parent = null;
                    Children.Remove(child);
                    return true;
                }

                if (RemoveChild(Name))
                    break;
            }

            return false;
        }

        public GameObject GetChild(string Tag)
        {
            foreach (GameObject child in Children)
            {
                if (child.Tag == Tag)
                    return child;

                if (GetChild(Tag) != null)
                    break;
            }

            return null;
        }

        public GameObject GetChildWithName(string Name)
        {
            foreach (GameObject child in Children)
            {
                if (child.Name == Name)
                    return child;

                if (GetChild(Name) != null)
                    break;
            }

            return null;
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

        public GameObjectComponent[] GetAllComponents()
        {
            if (GameObjectComponents.Count == 0)
                return null;
            else
                return GameObjectComponents.ToArray();
        }

        public GameObject[] GetChildren() //Returns First-Level children
        {
            if (Children.Count == 0)
                return null;

            return Children.ToArray();
        }

        public GameObject[] GetALLChildren()
        {
            if (Children.Count == 0)
                return null;

            List<GameObject> Arr = new List<GameObject>();
            GetAllChildrenRecursive(Arr, Children);

            return Arr.ToArray();
        }

        private void GetAllChildrenRecursive(List<GameObject> Arr, List<GameObject> children)
        {
            if (children.Count == 0)
                return;

            foreach(GameObject Child in children)
            {
                GetAllChildrenRecursive(Arr, Child.Children);
                Arr.Add(Child);
            }
        }

        public virtual void Start()
        {
            foreach (GameObjectComponent GOC in GameObjectComponents)
                if (GOC.Enabled)
                    GOC.Start();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (IsActive())
                foreach (GameObjectComponent GOC in GameObjectComponents)
                    if (GOC.Enabled)
                        GOC.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive())
                foreach (GameObjectComponent GOC in GameObjectComponents)
                    if (GOC.Enabled)
                        GOC.Draw(spriteBatch);
        }

        public virtual void DrawUI()
        {
            if (IsActive())
                foreach (GameObjectComponent GOC in GameObjectComponents)
                    if (GOC.Enabled)
                        GOC.DrawUI();
        }

        //public static void Destroy(GameObject GO)  //Scene not yet implemented
        //{
        //    Scene.DestroyGameObject(GO); 
        //}

        //Update: Implemented using recursion, this means, you don't have to add every child to the simulation, just add the parent and you are good to go
        public static GameObject Instantiate(GameObject GO)  //=> Implement it using "Recursion"
        {
            GameObject Clone = new GameObject(); //Add Children pls
            if (GO.Parent != null)
                GO.Parent.AddChild(Clone);
            else
                Clone.Parent = null;
            Clone.Tag = GO.Tag;
            Clone.Active = GO.Active;
            Clone.Layer = GO.Layer;

            for (int i = 0; i < GO.GameObjectComponents.Count; i++)
                Clone.AddComponent<GameObjectComponent>(GO.GameObjectComponents[i].DeepCopy(Clone));

            SceneManager.ActiveScene.AddGameObject(Clone);

            InstantiateRecursive(GO, Clone);

            return Clone;
        }

        public static GameObject Instantiate(GameObject GO, GameObject parent)  //=> Implement it using "Recursion"
        {
            GameObject Clone = new GameObject();
            if (parent != null)
                parent.AddChild(Clone);
            else
                Clone.Parent = null;
            Clone.Tag = GO.Tag;
            Clone.Active = GO.Active;
            Clone.Layer = GO.Layer;

            for (int i = 0; i < GO.GameObjectComponents.Count; i++)
                Clone.AddComponent<GameObjectComponent>(GO.GameObjectComponents[i].DeepCopy(Clone));

            SceneManager.ActiveScene.AddGameObject(Clone);

            InstantiateRecursive(GO, Clone);

            return Clone;
        }

        private static void InstantiateRecursive(GameObject GO, GameObject Parent)
        {
            if (GO == null)
                return;

            if(GO.Children.Count != 0)
                foreach (GameObject Child in GO.Children)
                {
                    GameObject ChildClone = new GameObject();
                    if (Parent != null)
                        Parent.AddChild(ChildClone);
                    else
                        ChildClone.Parent = null;
                    ChildClone.Tag = Child.Tag;
                    ChildClone.Active = Child.Active;
                    ChildClone.Layer = Child.Layer;

                    for (int i = 0; i < Child.GameObjectComponents.Count; i++)
                        ChildClone.AddComponent<GameObjectComponent>(Child.GameObjectComponents[i].DeepCopy(ChildClone));

                    SceneManager.ActiveScene.AddGameObject(ChildClone);

                    InstantiateRecursive(Child, ChildClone);
                }
        }

        public void Destroy()
        {
            foreach (GameObjectComponent GOC in GameObjectComponents)
                GOC.Destroy();
        }

        public int Compare(GameObject x, GameObject y)
        {
            if (x.Layer < y.Layer)
                return 1;
            if (x.Layer > y.Layer)
                return -1;
            return 0;
        }

        public static IComparer<GameObject> SortByLayer()
        {
            return new GameObject();
        }

        //public void Serialize(StreamWriter SW) //Parent should be assigned after finding all gameObjects
        //{
        //    SW.WriteLine("Start Of " + Name);

        //    SW.Write("GameComponentsCount:\t" + GameComponentsCount.ToString() + "\n"); // Get It First
        //    SW.Write("Layer:\t" + Layer.ToString() + "\n");
        //    if(Parent != null)
        //        SW.Write("Parent:\t" + Parent.Name + "\n");
        //    else
        //        SW.Write("Parent:\t" + "null\n");
        //    SW.Write("Tag:\t" + Tag + "\n");
        //    SW.Write("Active:\t" + Active.ToString() + "\n");
        //    SW.Write("Name:\t" + Name + "\n");

        //    string _Children = "Children:\t" + Children.Count.ToString() + "\t";
        //    foreach (GameObject Child in Children)
        //        _Children += Child.Name.ToString() + "\t";
        //    SW.Write(_Children + "\n");

        //    foreach (GameObjectComponent GOC in GameObjectComponents)
        //        GOC.Serialize(SW);

        //    SW.WriteLine("End Of " + Name);
        //}

        public void Serialize(StreamWriter SW) //Parent should be assigned after finding all gameObjects
        {
            Utility.Serialize(SW, this);

            SW.WriteLine("End Of " + Name);
        }

        public void Deserialize(StreamReader SR)
        {
            SR.ReadLine();

            int _GameComponentsCount = int.Parse(SR.ReadLine().Split('\t')[1]);
            Layer = float.Parse(SR.ReadLine().Split('\t')[1]);
            Parent = new GameObject() { Name = SR.ReadLine().Split('\t')[1] };
            Tag = SR.ReadLine().Split('\t')[1];
            Active = bool.Parse(SR.ReadLine().Split('\t')[1]);
            Name = SR.ReadLine().Split('\t')[1];
            string[] children = SR.ReadLine().Split('\t');
            for (int i = 0; i < int.Parse(children[1]); i++)
                Children.Add(new GameObject() { Name = children[i + 2] });

            for (int i = 0; i < _GameComponentsCount; i++)
            {
                GameObjectComponent GOC = Utility.GetInstance(SR.ReadLine()) as GameObjectComponent;
                GOC.gameObject = this;
                GOC.Start();
                GOC.Deserialize(SR);
                AddComponent(GOC);
            }

            SR.ReadLine();
        }
    }
}