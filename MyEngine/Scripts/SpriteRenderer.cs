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
        public Effect Effect;
        public static Effect LastEffect;

        static SpriteRenderer()
        {
            LastEffect = null;
        }

        public SpriteRenderer()
        {
            Sprite = null;
            Color = Color.White;
            SpriteEffects = SpriteEffects.None;
            Effect = null;
        }

        public override void Start()
        {
            gameObject.Layer = 1;
            Transform = gameObject.Transform;

            if (Sprite == null)
                Sprite = new Sprite(Transform);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Sprite != null)
            {
                if (LastEffect != Effect)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, Effect, Setup.Camera.GetViewTransformationMatrix());
                }

                spriteBatch.Draw(Sprite.Texture, Transform.Position, Sprite.SourceRectangle, Color, Transform.Rotation, Sprite.Origin, Transform.Scale, SpriteEffects, gameObject.Layer);

                LastEffect = Effect;
            }
        }

        public override GameObjectComponent DeepCopy(GameObject clone)
        {
            SpriteRenderer Clone = this.MemberwiseClone() as SpriteRenderer;
            Clone.Transform = clone.Transform;
            Clone.Sprite = Sprite.DeepCopy(clone);
            Clone.Effect = (Effect == null) ? null : Effect.Clone();

            return Clone;
        }
    }
}
