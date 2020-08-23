using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public class SpriteRenderer: GameObjectComponent
    {
        public Sprite Sprite { set; get; }
        public Transform Transform { set; get; }
        public SpriteEffects SpriteEffects;
        public Color Color;

        public SpriteRenderer()
        {
            Sprite = null;
            Transform = null;
            Color = Color.White;
            SpriteEffects = SpriteEffects.None;
        }

        public override void Start()
        {
            Transform = gameObject.GetComponent<Transform>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Sprite != null)
                spriteBatch.Draw(Sprite.Texture, Transform.Position*Transform.PixelsPerUnit, Sprite.SourceRectangle, Color, Transform.Rotation, Sprite.Origin, Transform.Scale, SpriteEffects, Sprite.Layer);
        }

        public override GameObjectComponent DeepCopy()
        {
            SpriteRenderer Clone = this.MemberwiseClone() as SpriteRenderer;

            Clone.Sprite = Sprite.DeepCopy();

            return Clone;
        }
    }
}
