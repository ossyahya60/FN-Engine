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
        public Vector2 Origin;
        public bool CustomOrigin = false;

        private Transform transform;

        public Text(string name)
        {
            text = "Text";
            if(gameObject != null)
                transform = gameObject.Transform;
            Origin = Vector2.Zero;
            Name = name;
            spriteEffects = SpriteEffects.None;
            Color = Color.White;
        }

        public Text(string name, SpriteFont font)
        {
            text = "Text";
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
            gameObject.Layer = LayerUI.GetLayer("Text");
            if (transform == null)
                transform = gameObject.Transform;

            //transform.Position = new Vector2(Setup.graphics.PreferredBackBufferWidth / 2, Setup.graphics.PreferredBackBufferHeight / 2);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(!CustomOrigin)
                Origin = Font.MeasureString(text) * 0.5f;

            spriteBatch.DrawString(Font, text, transform.Position, Color, MathHelper.ToRadians(transform.Rotation), Origin, transform.Scale, spriteEffects, gameObject.Layer);
        }

        public void LoadFont(string Name)
        {
            Font = Setup.Content.Load<SpriteFont>(Name);
        }

        public string GetName()
        {
            return Name;
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
