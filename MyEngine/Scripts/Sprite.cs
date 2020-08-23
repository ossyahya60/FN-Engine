using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    //Not entirely sure if it should be a gameobject component or not
    public class Sprite
    {
        public Transform Transform { set; get; }
        public Texture2D Texture = null;
        public Rectangle SourceRectangle;  //P.S: scale??
        public float Layer = 0;
        public Vector2 Origin;

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
            return new Rectangle((int)(Transform.Position.X * Transform.PixelsPerUnit), (int)(Transform.Position.Y * Transform.PixelsPerUnit), (int)(SourceRectangle.Width * Transform.Scale.X), (int)(SourceRectangle.Height * Transform.Scale.Y));
        }

        public Sprite DeepCopy()
        {
            Sprite Clone = this.MemberwiseClone() as Sprite;

            Clone.Transform = new Transform();

            return Clone;
        }
    }
}
