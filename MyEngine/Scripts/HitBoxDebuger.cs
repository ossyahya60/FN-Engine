using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public static class HitBoxDebuger
    {
        public static void DrawRectangle(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Rectangle Rect)  //Draw filledRectangle
        {
            Texture2D _texture;

            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.DarkSlateGray });

            spriteBatch.Draw(_texture, Rect, Color.White);
        }
    }
}
