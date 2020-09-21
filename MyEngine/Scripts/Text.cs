using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public class Text : UIComponent
    {
        public string Name;
        public Vector2 Position;
        public Color Color;
        public float Scale;
        public string text;
        public SpriteFont Font;
        public float Rotation;
        public SpriteEffects spriteEffects;
        public float Layer = 0f;
        public Vector2 Origin;

        public Text(string name)
        {
            Origin = Vector2.Zero;
            Name = name;
            Scale = 1;
            spriteEffects = SpriteEffects.None;
            Color = Color.White;
            Rotation = 0;
            Position = new Vector2(Setup.graphics.PreferredBackBufferWidth / 2, Setup.graphics.PreferredBackBufferHeight / 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Origin = Font.MeasureString(text) * 0.5f;
            spriteBatch.DrawString(Font, text, Position, Color, MathHelper.ToRadians(Rotation), Origin, Scale, spriteEffects, Layer);
        }

        public void LoadFont(string Name)
        {
            Font = Setup.Content.Load<SpriteFont>(Name);
        }

        public string GetName()
        {
            return Name;
        }

        public void Update(GameTime gameTime)
        {
            
        }
    }
}
