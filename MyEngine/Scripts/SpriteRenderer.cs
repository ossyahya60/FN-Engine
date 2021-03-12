using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ImGuiNET;

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

        private Rectangle DestRect;

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
            DestRect = Rectangle.Empty;
        }

        public override void Start()
        {
            //gameObject.Layer = 1;
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

                //spriteBatch.Draw(Sprite.Texture, Transform.Position, Sprite.SourceRectangle, Color, Transform.Rotation, Sprite.Origin, Transform.Scale, SpriteEffects, gameObject.Layer);
                DestRect.Location = Transform.Position.ToPoint();
                DestRect.Width = (int)(Sprite.SourceRectangle.Width * Transform.Scale.X);
                DestRect.Height = (int)(Sprite.SourceRectangle.Height * Transform.Scale.Y);
                spriteBatch.Draw(Sprite.Texture, null, DestRect, Sprite.SourceRectangle, Sprite.Origin, Transform.Rotation, Vector2.One, Color, SpriteEffects, gameObject.Layer);

                LastEffect = Effect;
            }
        }

        public override void DrawUI()
        {
            //ImGui.Begin("LOOL");
            //ImGui.SetWindowPos(System.Numerics.Vector2.Zero);
            //ImGui.SetWindowSize(new System.Numerics.Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight));
            //ImGui.TextColored(new System.Numerics.Vector4(1, 0, 0, 1), "This is Cool");
            //ImGui.End();
        }

        public override GameObjectComponent DeepCopy(GameObject clone)
        {
            SpriteRenderer Clone = this.MemberwiseClone() as SpriteRenderer;
            Clone.Transform = clone.Transform;
            Clone.Sprite = Sprite.DeepCopy(clone);
            Clone.Sprite.Transform = clone.Transform;
            Clone.Effect = (Effect == null) ? null : Effect.Clone();

            return Clone;
        }
    }
}
