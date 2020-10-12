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
        public GameObject Parent = null; //Not yet implemented
        public List<GameObjectComponent> GameObjectComponents; //List of all  GO componets in a certain scene(scene is not yet implemented)
        public string Tag = null;
        public int GameComponentsCount = 0;
        public bool Active = true;
        public string Name = "Default";

        private readonly string[] CanBeAddedMultipleTimes = { "BoxCollider2D", "AudioSource", "ParticleEffect" };

        public GameObject()
        {
            GameObjectComponents = new List<GameObjectComponent>();
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

        public virtual void Start()
        {
            foreach (GameObjectComponent GOC in GameObjectComponents)
                if (GOC.Enabled)
                    GOC.Start();
        }

        public virtual void Update(GameTime gameTime)
        {
            Active = (Parent != null)? Parent.Active : Active;

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

        //public static GameObject Instantiate(GameObject GO)  //Scene not yet implemented
        //{
        //    GameObject Clone = new GameObject();
        //    Clone.Parent = GO.Parent;
        //    Clone.Tag = GO.Tag;
        //    Clone.GameObjectComponents = new List<GameObjectComponent>();

        //    for (int i = 0; i < GO.GameObjectComponents.Count; i++)
        //    {
        //        Clone.AddComponent<GameObjectComponent>(GO.GameObjectComponents[i].DeepCopy());
        //        Clone.GameObjectComponents[i].gameObject = Clone;
        //    }
        //    GO.GetComponent<Transform>().Rotation = 90;
        //    return Clone;
        //}
    }
}
