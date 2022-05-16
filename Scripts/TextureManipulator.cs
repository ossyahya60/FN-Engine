using Microsoft.Xna.Framework;

namespace FN_Engine
{
    public static class TextureManipulator
    {
        public static GameObject[] SliceAndFetch(GameObject go, bool Horizontal)
        {
            GameObject otherSlice = GameObject.Instantiate(go);

            Sprite SR_go = go.GetComponent<SpriteRenderer>().Sprite;
            Sprite SR_os = otherSlice.GetComponent<SpriteRenderer>().Sprite;


            Rectangle sourceRect = SR_go.SourceRectangle;

            Rectangle rect = new Rectangle(sourceRect.X, sourceRect.Y, Horizontal ? sourceRect.Width : sourceRect.Width / 2, Horizontal ? sourceRect.Height / 2 : sourceRect.Height);

            SR_go.SourceRectangle = rect;

            if (Horizontal)
            {
                go.Transform.Position.Y -= SR_go.Origin.Y * go.Transform.Scale.Y * (sourceRect.Height - rect.Height);
                rect = new Rectangle(sourceRect.X, sourceRect.Y + sourceRect.Height / 2, sourceRect.Width, sourceRect.Height / 2);
            }
            else
            {
                go.Transform.Position.X -= SR_go.Origin.X * go.Transform.Scale.X * (sourceRect.Width - rect.Width);
                rect = new Rectangle(sourceRect.X + sourceRect.Width / 2, sourceRect.Y, sourceRect.Width / 2, sourceRect.Height);
            }

            SR_os.SourceRectangle = rect;

            if (Horizontal)
                otherSlice.Transform.Position.Y -= SR_os.Origin.Y * otherSlice.Transform.Scale.Y * (sourceRect.Height - rect.Height) - (SR_go.DynamicScaledRect().Height);
            else
                otherSlice.Transform.Position.X -= SR_os.Origin.X * otherSlice.Transform.Scale.X * (sourceRect.Width - rect.Width) - (SR_go.DynamicScaledRect().Width);

            return new GameObject[] { go, otherSlice };
        }

        public static void Slice(GameObject go, bool Horizontal)
        {
            GameObject otherSlice = GameObject.Instantiate(go);

            Sprite SR_go = go.GetComponent<SpriteRenderer>().Sprite;
            Sprite SR_os = otherSlice.GetComponent<SpriteRenderer>().Sprite;


            Rectangle sourceRect = SR_go.SourceRectangle;

            Rectangle rect = new Rectangle(sourceRect.X, sourceRect.Y, Horizontal ? sourceRect.Width : sourceRect.Width / 2, Horizontal ? sourceRect.Height / 2 : sourceRect.Height);

            SR_go.SourceRectangle = rect;

            if (Horizontal)
            {
                go.Transform.Position.Y -= SR_go.Origin.Y * go.Transform.Scale.Y * (sourceRect.Height - rect.Height);
                rect = new Rectangle(sourceRect.X, sourceRect.Y + sourceRect.Height / 2, sourceRect.Width, sourceRect.Height / 2);
            }
            else
            {
                go.Transform.Position.X -= SR_go.Origin.X * go.Transform.Scale.X * (sourceRect.Width - rect.Width);
                rect = new Rectangle(sourceRect.X + sourceRect.Width / 2, sourceRect.Y, sourceRect.Width / 2, sourceRect.Height);
            }

            SR_os.SourceRectangle = rect;

            if (Horizontal)
                otherSlice.Transform.Position.Y -= SR_os.Origin.Y * otherSlice.Transform.Scale.Y * (sourceRect.Height - rect.Height) - (SR_go.DynamicScaledRect().Height);
            else
                otherSlice.Transform.Position.X -= SR_os.Origin.X * otherSlice.Transform.Scale.X * (sourceRect.Width - rect.Width) - (SR_go.DynamicScaledRect().Width);
        }
    }
}
