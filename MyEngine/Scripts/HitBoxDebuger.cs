using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MyEngine
{
    //Use this class for debugging only, it's not designed for real-time use!! (Inefficient)
    public static class HitBoxDebuger
    {
        public static Texture2D _textureFilled = null;
        public static Texture2D _textureNonFilled = null;

        private static Vector2 Origin = Vector2.Zero;
        private static BasicEffect _effect = null;
        private static int[] IntArr = new int[] { 0, 1, 2 };
        private const int CircleSegments = 32;
        private const int CircleSegmentsHQ = 64;

        static HitBoxDebuger()
        {
            _textureFilled = new Texture2D(Setup.GraphicsDevice, 1, 1);
            _textureFilled.SetData(new Color[] { Color.White });

            _textureNonFilled = new Texture2D(Setup.GraphicsDevice, 1, 1);
            _textureNonFilled.SetData(new Color[] { Color.White });

            _effect = new BasicEffect(Setup.GraphicsDevice);
            _effect.Texture = _textureFilled;
            _effect.TextureEnabled = true;
        }

        public static Texture2D RectTexture(Color color) //Constant
        {
            Texture2D textureFilled = new Texture2D(Setup.GraphicsDevice, 1, 1);
            textureFilled.SetData(new Color[] { color });

            return textureFilled;
        }

        public static Texture2D RectTexture() // Allowing Changing RGB
        {
            Texture2D textureFilled = new Texture2D(Setup.GraphicsDevice, 1, 1);
            textureFilled.SetData(new Color[] { Color.White });

            return textureFilled;
        }

        public static Texture2D RectTexture(float Alpha) //Allow setting Alpha value and RGB
        {
            Texture2D textureFilled = new Texture2D(Setup.GraphicsDevice, 1, 1);
            textureFilled.SetData(new Color[] { new Color(1.0f, 1.0f, 1.0f, MathHelper.Clamp(Alpha, 0, 1)) });

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

        public static void DrawRectangle(Rectangle Rect, Color color)  //Draw filledRectangle
        {
            Setup.spriteBatch.Draw(_textureFilled, Rect, color);
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

        public static void DrawNonFilledRectangle_Effect(Rectangle Rect) //Draw Non filledRectangle (Does it consume much memory?) use for debugging purposes only!!
        {
            DrawPolygon(new Vector2[] { new Vector2(Rect.Left, Rect.Top), new Vector2(Rect.Right, Rect.Top), new Vector2(Rect.Right, Rect.Bottom), new Vector2(Rect.Left, Rect.Bottom) });
        }

        public static void DrawRectangle_Effect(Rectangle Rect) //Draw Non filledRectangle (Does it consume much memory?) use for debugging purposes only!!
        {
            DrawTriangle(new Vector2(Rect.Left, Rect.Top), new Vector2(Rect.Right, Rect.Top), new Vector2(Rect.Right, Rect.Bottom));
            DrawTriangle(new Vector2(Rect.Left, Rect.Top), new Vector2(Rect.Left, Rect.Bottom), new Vector2(Rect.Right, Rect.Bottom));
        }

        public static void DrawLine_Effect(Vector2 V1, Vector2 V2)
        {
            VertexPositionTexture[] _vertices = new VertexPositionTexture[2];

            // This step makes no sense to me but it works :D
            V1 *= 2;
            V2 *= 2;

            V1.X -= Setup.graphics.PreferredBackBufferWidth;
            V2.X -= Setup.graphics.PreferredBackBufferWidth;
            V1.Y = -V1.Y;
            V2.Y = -V2.Y;
            V1.Y += Setup.graphics.PreferredBackBufferHeight;
            V2.Y += Setup.graphics.PreferredBackBufferHeight;

            V1.X /= Setup.graphics.PreferredBackBufferWidth;
            V2.X /= Setup.graphics.PreferredBackBufferWidth;
            V1.Y /= Setup.graphics.PreferredBackBufferHeight;
            V2.Y /= Setup.graphics.PreferredBackBufferHeight;

            _vertices[0].Position = new Vector3(V1, 0);
            _vertices[1].Position = new Vector3(V2, 0);

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                Setup.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
                (
                    PrimitiveType.LineStrip, // same result with TriangleList
                    _vertices,
                    0,
                    _vertices.Length,
                    new int[] { 0, 1 },
                    0,
                    1
                );
            }
        }

        public static void DrawTriangle(Vector2 V1, Vector2 V2, Vector2 V3)
        {
            VertexPositionTexture[] _vertices = new VertexPositionTexture[3];
            V1 *= 2;
            V2 *= 2;
            V3 *= 2;

            V1.X -= Setup.graphics.PreferredBackBufferWidth;
            V2.X -= Setup.graphics.PreferredBackBufferWidth;
            V3.X -= Setup.graphics.PreferredBackBufferWidth;
            V1.Y = -V1.Y;
            V2.Y = -V2.Y;
            V3.Y = -V3.Y;
            V1.Y += Setup.graphics.PreferredBackBufferHeight;
            V2.Y += Setup.graphics.PreferredBackBufferHeight;
            V3.Y += Setup.graphics.PreferredBackBufferHeight;

            V1.X /= Setup.graphics.PreferredBackBufferWidth;
            V2.X /= Setup.graphics.PreferredBackBufferWidth;
            V3.X /= Setup.graphics.PreferredBackBufferWidth;
            V1.Y /= Setup.graphics.PreferredBackBufferHeight;
            V2.Y /= Setup.graphics.PreferredBackBufferHeight;
            V3.Y /= Setup.graphics.PreferredBackBufferHeight;

            _vertices[0].Position = new Vector3(V1, 0);
            _vertices[1].Position = new Vector3(V2, 0);
            _vertices[2].Position = new Vector3(V3, 0);

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                Setup.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
                (
                    PrimitiveType.TriangleStrip, // same result with TriangleList
                    _vertices,
                    0,
                    _vertices.Length,
                    IntArr,
                    0,
                    1
                );
            }
        }

        public static void DrawPolygon(Vector2[] Vertices)
        {
            VertexPositionTexture[] _vertices = new VertexPositionTexture[Vertices.Length];
            int Count = Vertices.Length;

            for(int i=0; i< Count; i++)
            {
                Vertices[i] *= 2;
                Vertices[i] = new Vector2(Vertices[i].X, -Vertices[i].Y);
                Vertices[i] -= new Vector2(Setup.graphics.PreferredBackBufferWidth, -Setup.graphics.PreferredBackBufferHeight);
                Vertices[i] /= new Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight);
            }

            int[] IntArr = new int[Vertices.Length + 1];

            for (int i = 0; i < Vertices.Length; i++)
            {
                _vertices[i].Position = new Vector3(Vertices[i], 0);
                IntArr[i] = i;
            }
            IntArr[Vertices.Length] = 0;

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                Setup.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
                (
                    PrimitiveType.LineStrip, // same result with TriangleList
                    _vertices,
                    0,
                    _vertices.Length,
                    IntArr,
                    0,
                    IntArr.Length - 1
                );
            }
        }

        public static void DrawLine(Rectangle Rect, Color color, float Angle, float Layer, Vector2 Origin)  //Draw line
        {
            Setup.spriteBatch.Draw(_textureFilled, Rect, null, color, MathHelper.ToRadians(Angle), Origin, SpriteEffects.None, Layer);
        }

        public static void DrawLine(Vector2 Start, Vector2 End, Color color)  //Draw line
        {
            Rectangle Rect = Rectangle.Empty;
            Rect.Location = Start.ToPoint();
            Rect.Width = (int)(End - Start).Length();
            Rect.Height = 2;
            Setup.spriteBatch.Draw(_textureFilled, Rect, null, color, MathHelper.ToRadians(MathCompanion.GetAngle(Start, End)), Vector2.Zero, SpriteEffects.None, 0);
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

        public static void DrawCircle(Vector2 Center, int Radius, bool HighQuality = false)
        {
            float Theta = 0;
            int Segments = HighQuality ? CircleSegmentsHQ : CircleSegments;
            float Increment = 2 * (float)Math.PI / Segments;

            Vector2[] Vertices = new Vector2[Segments];
            for (int i = 0; i < Segments; i++)
            {
                Vertices[i] = new Vector2((float)Math.Cos(Theta), (float)Math.Sin(Theta)) * Radius + Center;
                Theta += Increment;
            }

            DrawPolygon(Vertices);
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
                    //else
                    //    Pixels[i * Diameter + j] = Color.Transparent;
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
                    //else
                    //    Pixels[i * Diameter + j] = Color.Transparent;
                }
            }

            Texture2D texture = new Texture2D(Setup.GraphicsDevice, Diameter, Diameter);
            texture.SetData(Pixels);

            return texture;
        }

        //Bezier Curves => Quadratic Curves

        private static int getPt(int n1, int n2, float perc)
        {
            int diff = n2 - n1;

            return (int)(n1 + (diff * perc));
        }

        private static int GetMinX(Point P1, Point P2, Point P3)
        {
            Point Res = Point.Zero;

            Res = P2;
            if (P1.X <= Res.X)
                Res = P1;
            if (P3.X <= Res.X)
                Res = P3;

            return Res.X;
        }

        private static int GetMaxX(Point P1, Point P2, Point P3)
        {
            Point Res = Point.Zero;

            Res = P2;
            if (P1.X >= Res.X)
                Res = P1;
            if (P3.X >= Res.X)
                Res = P3;

            return Res.X;
        }

        private static int GetMinY(Point P1, Point P2, Point P3)
        {
            Point Res = Point.Zero;

            Res = P2;
            if (P1.Y <= Res.Y)
                Res = P1;
            if (P3.Y <= Res.Y)
                Res = P3;

            return Res.Y;
        }

        private static int GetMaxY(Point P1, Point P2, Point P3)
        {
            Point Res = Point.Zero;

            Res = P2;
            if (P1.Y >= Res.Y)
                Res = P1;
            if (P3.Y >= Res.Y)
                Res = P3;

            return Res.Y;
        }

        public static void BezierLine(Point Start, Point Control, Point End, Color color, float Layer, int Quality = 10)
        {
            Point Auxilary = Point.Zero;
            Point PrevPoint = Point.Zero;

            // The Green Line
            int xa = getPt(Start.X, Control.X, 0);
            int ya = getPt(Start.Y, Control.Y, 0);
            int xb = getPt(Control.X, End.X, 0);
            int yb = getPt(Control.Y, End.Y, 0);

            PrevPoint = Auxilary;

            // The Black Dot
            Auxilary.X = getPt(xa, xb, 0);
            Auxilary.Y = getPt(ya, yb, 0);

            for (float i = 1.0f / Quality; i <= 1.01f; i += 1.0f/Quality)
            {
                // The Green Line
                xa = getPt(Start.X, Control.X, i);
                ya = getPt(Start.Y, Control.Y, i);
                xb = getPt(Control.X, End.X, i);
                yb = getPt(Control.Y, End.Y, i);

                PrevPoint = Auxilary;

                // The Black Dot
                Auxilary.X = getPt(xa, xb, i);
                Auxilary.Y = getPt(ya, yb, i);

                Rectangle Rect = Rectangle.Empty;
                Rect.Location = PrevPoint;
                Rect.Size = new Point((int)Math.Ceiling((Auxilary - PrevPoint).ToVector2().Length()), 3);
                DrawLine(Rect, color, MathCompanion.GetAngle(PrevPoint.ToVector2(), Auxilary.ToVector2()), Layer, Vector2.Zero);
            }
        }

        public static void BezierLine(Point Start, Point Control1, Point Control2, Point End, Color color, float Layer, int Quality = 10)
        {
            Point Auxilary = Point.Zero;
            Point PrevPoint = Point.Zero;

            // The Green Line
            int xa = getPt(Start.X, Control1.X, 0);
            int ya = getPt(Start.Y, Control1.Y, 0);
            int xb = getPt(Control1.X, Control2.X, 0);
            int yb = getPt(Control1.Y, Control2.Y, 0);
            int xc = getPt(Control2.X, End.X, 0);
            int yc = getPt(Control2.Y, End.Y, 0);

            // The Blue Line
            int xm = getPt(xa, xb, 0);
            int ym = getPt(ya, yb, 0);
            int xn = getPt(xb, xc, 0);
            int yn = getPt(yb, yc, 0);

            PrevPoint = Auxilary;

            // The Black Dot
            Auxilary.X = getPt(xm, xn, 0);
            Auxilary.Y = getPt(ym, yn, 0);

            for (float i = 1.0f / Quality; i <= 1.01f; i += 1.0f / Quality)
            {
                // The Green Line
                xa = getPt(Start.X, Control1.X, i);
                ya = getPt(Start.Y, Control1.Y, i);
                xb = getPt(Control1.X, Control2.X, i);
                yb = getPt(Control1.Y, Control2.Y, i);
                xc = getPt(Control2.X, End.X, i);
                yc = getPt(Control2.Y, End.Y, i);

                // The Blue Line
                xm = getPt(xa, xb, i);
                ym = getPt(ya, yb, i);
                xn = getPt(xb, xc, i);
                yn = getPt(yb, yc, i);

                PrevPoint = Auxilary;

                // The Black Dot
                Auxilary.X = getPt(xm, xn, i);
                Auxilary.Y = getPt(ym, yn, i);

                Rectangle Rect = Rectangle.Empty;
                Rect.Location = PrevPoint;
                Rect.Size = new Point((int)Math.Ceiling((Auxilary - PrevPoint).ToVector2().Length()), 3);
                DrawLine(Rect, color, MathCompanion.GetAngle(PrevPoint.ToVector2(), Auxilary.ToVector2()), Layer, Vector2.Zero);
            }
        }
    }
}
