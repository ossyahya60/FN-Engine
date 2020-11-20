using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    //Not entirely sure if it should be a gameobject component or not
    public class Sprite
    {
        public Texture2D Texture = null;
        public Rectangle SourceRectangle;  //P.S: scale??
        public float Layer = 0;
        public Vector2 Origin;
        public Transform Transform;
        private Rectangle HandyRectangle; //To avoid stack allocating a lot if memory in a short time

        public Sprite(Transform transform)
        {
            Transform = transform;
            Origin = Vector2.Zero;
            SourceRectangle = new Rectangle();
        }

        public void LoadTexture(string path)
        {
            Texture = Setup.Content.Load<Texture2D>(path);
            SourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
        }

        public Rectangle DynamicScaledRect()
        {
            HandyRectangle.X = (int)Transform.Position.X;
            HandyRectangle.Y = (int)Transform.Position.Y;
            HandyRectangle.Width = (int)(SourceRectangle.Width * Transform.Scale.X);
            HandyRectangle.Height = (int)(SourceRectangle.Height * Transform.Scale.Y);

            return HandyRectangle;
        }

        public Sprite DeepCopy(GameObject clone)
        {
            Sprite Clone = this.MemberwiseClone() as Sprite;
            Clone.SourceRectangle = new Rectangle(SourceRectangle.Location, SourceRectangle.Size);
            Clone.Origin = new Vector2(Origin.X, Origin.Y);
            Clone.Transform = clone.Transform;

            return Clone;
        }
    }
}
