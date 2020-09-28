using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public static class HitBoxDebuger
    {
        public static Texture2D _textureFilled = null;
        public static Texture2D _textureNonFilled = null;

        static HitBoxDebuger()
        {
            _textureFilled = new Texture2D(Setup.GraphicsDevice, 1, 1);
            _textureFilled.SetData(new Color[] { Color.White });

            _textureNonFilled = new Texture2D(Setup.GraphicsDevice, 1, 1);
            _textureNonFilled.SetData(new Color[] { Color.White });
        }

        public static void DrawRectangle(Rectangle Rect)  //Draw filledRectangle
        {
            Setup.spriteBatch.Draw(_textureFilled, Rect, Color.White);
        }

        public static void DrawRectangle(Rectangle Rect, Color color, float Angle, float Layer)  //Draw filledRectangle
        {
            Setup.spriteBatch.Draw(_textureFilled, Rect, null, color, MathHelper.ToRadians(Angle), Vector2.Zero, SpriteEffects.None, Layer);
        }

        public static void DrawRectangle(Rectangle Rect, Color color, float Angle, Texture2D texture, float Layer, Vector2 Origin)  //Draw filledRectangle
        {
            Setup.spriteBatch.Draw(texture, Rect, null, color, MathHelper.ToRadians(Angle), Origin, SpriteEffects.None, Layer);
        }

        public static void DrawNonFilledRectangle(Rectangle Rect) //Draw Non filledRectangle (Does it consume much memory?)
        {
            Setup.spriteBatch.Draw(_textureNonFilled, new Rectangle(Rect.Left, Rect.Top, Rect.Width, 1), Color.LightGreen);
            Setup.spriteBatch.Draw(_textureNonFilled, new Rectangle(Rect.Right, Rect.Top, 1, Rect.Height), Color.LightGreen);
            Setup.spriteBatch.Draw(_textureNonFilled, new Rectangle(Rect.Left, Rect.Bottom, Rect.Width, 1), Color.LightGreen);
            Setup.spriteBatch.Draw(_textureNonFilled, new Rectangle(Rect.Left, Rect.Top, 1, Rect.Height), Color.LightGreen);
        }

        public static void DrawLine(Rectangle Rect, Color color, float Angle, float Layer, Vector2 Origin)  //Draw filledRectangle
        {
            Setup.spriteBatch.Draw(_textureFilled, Rect, null, color, MathHelper.ToRadians(Angle), Origin, SpriteEffects.None, Layer);
        }
    }
}
