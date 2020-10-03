using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public class SpriteRenderer: GameObjectComponent
    {
        public Sprite Sprite { set; get; }
        public SpriteEffects SpriteEffects;
        public Color Color;

        private Transform Transform;

        public SpriteRenderer()
        {
            Sprite = null;
            Color = Color.White;
            SpriteEffects = SpriteEffects.None;
        }

        public override void Start()
        {
            Transform = gameObject.Transform;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Sprite != null)
                spriteBatch.Draw(Sprite.Texture, Transform.Position, Sprite.SourceRectangle, Color, Transform.Rotation, Sprite.Origin, Transform.Scale, SpriteEffects, Sprite.Layer);
        }

        public override GameObjectComponent DeepCopy()
        {
            SpriteRenderer Clone = this.MemberwiseClone() as SpriteRenderer;

            Clone.Sprite = Sprite.DeepCopy();

            return Clone;
        }
    }
}
