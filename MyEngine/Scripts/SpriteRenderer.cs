using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public class SpriteRenderer: GameObjectComponent
    {
        public Sprite Sprite { set; get; }
        public SpriteEffects SpriteEffects;
        public Color Color;
        public Transform Transform;

        public SpriteRenderer()
        {
            Sprite = null;
            Color = Color.White;
            SpriteEffects = SpriteEffects.None;
        }

        public override void Start()
        {
            Transform = gameObject.Transform;

            if (Sprite == null)
                Sprite = new Sprite(Transform);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Sprite != null)
                spriteBatch.Draw(Sprite.Texture, Transform.Position, Sprite.SourceRectangle, Color, Transform.Rotation, Sprite.Origin, Transform.Scale, SpriteEffects, Sprite.Layer);
        }

        public override GameObjectComponent DeepCopy(GameObject clone)
        {
            SpriteRenderer Clone = this.MemberwiseClone() as SpriteRenderer;
            Clone.Transform = clone.Transform;
            Clone.Sprite = Sprite.DeepCopy(clone);

            return Clone;
        }
    }
}
