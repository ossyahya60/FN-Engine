using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace MyEngine
{
    public class GameObjectComponent
    {
        public GameObject gameObject;  //Every component belongs to a certain GameObject
        public bool Enabled = true;

        public GameObjectComponent()
        {

        }

        public virtual void Start() //Called once per component
        {
            
        }

        public virtual void Update(GameTime gameTime)  //Called every frame
        {
            
        }

        public virtual void LateUpdate(GameTime gameTime)  //Called every frame after Update()
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)  //Called every frame after "Update()" execution
        {

        }

        public virtual void DrawUI() //Called every frame (For UI Purposes)
        {

        }

        public virtual void Serialize(StreamWriter SW)
        {
            SW.Write("gameObject:\t" + gameObject.Name + "\n");
            SW.Write("Enabled:\t" + Enabled.ToString() + "\n");
        }

        public virtual void Deserialize(StreamReader SR)
        {
            SR.ReadLine();
            Enabled = bool.Parse(SR.ReadLine().Split('\t')[1]);
        }

        public virtual void Destroy()
        {
            //gameObject.RemoveComponent<GameObjectComponent>(this);
        }

        public virtual GameObjectComponent DeepCopy(GameObject Clone)
        {
            return this.MemberwiseClone() as GameObjectComponent;
        }
    }
}
