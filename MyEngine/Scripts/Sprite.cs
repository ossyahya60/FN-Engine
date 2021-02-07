using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    //Not entirely sure if it should be a gameobject component or not
    public class Sprite
    {
        public Texture2D Texture
        {
            set
            {
                texture = value;
                SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            }
            get
            {
                return texture;
            }
        }
        public Rectangle SourceRectangle;  //P.S: scale??
        public Vector2 Origin;
        public Transform Transform;

        private Texture2D texture;
        private Rectangle HandyRectangle; //To avoid stack allocating a lot if memory in a short time

        public Sprite(Transform transform)
        {
            Transform = transform;
            Origin = Vector2.Zero;
            SourceRectangle = new Rectangle();
            texture = null;
        }

        public void LoadTexture(string path)
        {
            Texture = Setup.Content.Load<Texture2D>(path);
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

        public void SetCenterAsOrigin()
        {
            Origin = new Vector2(SourceRectangle.Width / 2, SourceRectangle.Height / 2);
        }
    }
}
