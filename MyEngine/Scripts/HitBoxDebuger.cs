using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    //Use this class for debugging only, it's not designed for real-time use!! (Inefficient)
    public static class HitBoxDebuger
    {
        public static Texture2D _textureFilled = null;
        public static Texture2D _textureNonFilled = null;
        private static Vector2 Origin = Vector2.Zero;

        static HitBoxDebuger()
        {
            _textureFilled = new Texture2D(Setup.GraphicsDevice, 1, 1);
            _textureFilled.SetData(new Color[] { Color.White });

            _textureNonFilled = new Texture2D(Setup.GraphicsDevice, 1, 1);
            _textureNonFilled.SetData(new Color[] { Color.White });
        }

        public static Texture2D RectTexture(Color color)
        {
            Texture2D textureFilled = new Texture2D(Setup.GraphicsDevice, 1, 1);
            textureFilled.SetData(new Color[] { color });

            return textureFilled;
        }

        public static void DrawRectangle(Rectangle Rect)  //Draw filledRectangle
        {
            Setup.spriteBatch.Draw(_textureFilled, Rect, Color.White);
        }

        public static void DrawRectangle(Vector2 Position, Color color)  //Draw filledRectangle
        {
            Setup.spriteBatch.Draw(_textureFilled, Position, color);
        }

        public static void DrawRectangle(Rectangle Rect, Color color, float Angle, float Layer)  //Draw filledRectangle
        {
            Setup.spriteBatch.Draw(_textureFilled, Rect, null, color, MathHelper.ToRadians(Angle), Vector2.Zero, SpriteEffects.None, Layer);
        }

        public static void DrawRectangle(Rectangle Rect, Color color, float Angle, Texture2D texture, float Layer, Vector2 Origin)  //Draw filledRectangle
        {
            Setup.spriteBatch.Draw(texture, Rect, null, color, MathHelper.ToRadians(Angle), Origin, SpriteEffects.None, Layer);
        }

        public static void DrawNonFilledRectangle(Rectangle Rect) //Draw Non filledRectangle (Does it consume much memory?) use for debugging purposes only!!
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

        public static void DrawCircleFilled(Vector2 Position, int Radius, Color color, float Layer, float Scale)
        {
            Origin.X = Radius;
            Origin.Y = Radius;
            Setup.spriteBatch.Draw(CreateCircleTexture(Radius, color), Position, null, color, 0, Origin, Scale, SpriteEffects.None, Layer);
        }

        public static void DrawCircleFilled(Transform transform, int Radius, Color color)
        {
            Origin.X = Radius;
            Origin.Y = Radius;
            Setup.spriteBatch.Draw(CreateCircleTexture(Radius, color), transform.Position, null, color, 0, Origin, transform.Scale.X, SpriteEffects.None, transform.gameObject.Layer);
        }

        public static void DrawCircleNonFilled(Vector2 Position, int OuterRadius, int InnerRadius, Color color, float Layer, float Scale)
        {
            Origin = OuterRadius * Vector2.One;
            Setup.spriteBatch.Draw(CreateCircleTextureShell(OuterRadius, InnerRadius, color), Position, null, color, 0, Origin, Scale, SpriteEffects.None, Layer);
        }

        public static void DrawCircleNonFilled(Transform transform, int OuterRadius, int InnerRadius, Color color)
        {
            Origin = OuterRadius * Vector2.One;
            Setup.spriteBatch.Draw(CreateCircleTextureShell(OuterRadius, InnerRadius, color), transform.Position, null, color, 0, Origin, transform.Scale.X, SpriteEffects.None, transform.gameObject.Layer);
        }

        //You may use the following two functions to create a texture and use it, not creating a texture every frame!
        public static Texture2D CreateCircleTexture(int Radius, Color color)
        {
            Radius = (int)MathCompanion.Clamp(Radius, 1, Radius);
            int Diameter = 2 * Radius;
            Color[] Pixels = new Color[Diameter * Diameter];

            Vector2 PointIterator = Vector2.Zero;
            Vector2 Center = Radius * Vector2.One;
            for (int i = 0; i < Diameter; i++)
            {
                for(int j = 0; j < Diameter; j++)
                {
                    PointIterator.X = j;
                    PointIterator.Y = i;
                    if ((Center - PointIterator).Length() <= Radius)
                        Pixels[i * Diameter + j] = color;
                    else
                        Pixels[i * Diameter + j] = Color.Transparent;
                }
            }

            Texture2D texture = new Texture2D(Setup.GraphicsDevice, Diameter, Diameter);
            texture.SetData(Pixels);

            return texture;
        }

        public static Texture2D CreateCircleTextureShell(int Radius1, int Radius2, Color color)
        {
            int Diameter = 2 * Radius1;
            Color[] Pixels = new Color[Diameter * Diameter];

            for (int i = 0; i < Diameter; i++)
            {
                for (int j = 0; j < Diameter; j++)
                {
                    if ((i - Radius1) * (i - Radius1) + (j - Radius1) * (j - Radius1) <= (Radius1 * Radius1) && (i - Radius1) * (i - Radius1) + (j - Radius1) * (j - Radius1) >= (Radius2 * Radius2))
                        Pixels[i * Diameter + j] = color;
                    else
                        Pixels[i * Diameter + j] = Color.Transparent;
                }
            }

            Texture2D texture = new Texture2D(Setup.GraphicsDevice, Diameter, Diameter);
            texture.SetData(Pixels);

            return texture;
        }
    }
}
