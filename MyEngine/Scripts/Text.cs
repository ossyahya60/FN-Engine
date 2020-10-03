using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MyEngine
{
    public class Text : GameObjectComponent
    {
        public string Name;
        public Color Color;
        public string text;
        public SpriteFont Font;
        public SpriteEffects spriteEffects;
        public float Layer = 0f;

        private Vector2 Origin;
        private Transform transform;

        public Text(string name)
        {
            if(gameObject != null)
                transform = gameObject.Transform;
            Origin = Vector2.Zero;
            Name = name;
            spriteEffects = SpriteEffects.None;
            Color = Color.White;
        }

        public Text(string name, SpriteFont font)
        {
            if (gameObject != null)
                transform = gameObject.Transform;
            Origin = Vector2.Zero;
            Name = name;
            spriteEffects = SpriteEffects.None;
            Color = Color.White;
            Font = font;
        }

        static Text()
        {
            LayerUI.AddLayer("Text", 10);
        }

        public override void Start()
        {
            Layer = LayerUI.GetLayer("Text");
            if (transform == null)
                transform = gameObject.Transform;

            transform.Position = new Vector2(Setup.graphics.PreferredBackBufferWidth / 2, Setup.graphics.PreferredBackBufferHeight / 2);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Origin = Font.MeasureString(text) * 0.5f;

            spriteBatch.DrawString(Font, text, transform.Position, Color, MathHelper.ToRadians(transform.Rotation), Origin, transform.Scale, spriteEffects, Layer);
        }

        public void LoadFont(string Name)
        {
            Font = Setup.Content.Load<SpriteFont>(Name);
        }

        public string GetName()
        {
            return Name;
        }

        public void Fade(float Speed)
        {
            //Color.A = Math.Round()
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
