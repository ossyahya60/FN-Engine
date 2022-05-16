using Microsoft.Xna.Framework;

namespace FN_Engine
{
    public static class TextureManipulator
    {
        public static GameObject[] SliceAndFetch(GameObject go, bool Horizontal)
        {
            GameObject otherSlice = GameObject.Instantiate(go);

            Rectangle sourceRect = go.GetComponent<SpriteRenderer>().Sprite.SourceRectangle;

            Rectangle rect = new Rectangle(sourceRect.X, sourceRect.Y, Horizontal ? sourceRect.Width : sourceRect.Width / 2, Horizontal ? sourceRect.Height / 2 : sourceRect.Height);

            go.GetComponent<SpriteRenderer>().Sprite.SourceRectangle = rect;

            if (Horizontal)
                rect = new Rectangle(sourceRect.X, sourceRect.Y + sourceRect.Height / 2, sourceRect.Width, sourceRect.Height / 2);
            else
                rect = new Rectangle(sourceRect.X + sourceRect.Width / 2, sourceRect.Y, sourceRect.Width / 2, sourceRect.Height);

            otherSlice.GetComponent<SpriteRenderer>().Sprite.SourceRectangle = rect;

            if (Horizontal)
                otherSlice.Transform.Position.Y += go.GetComponent<SpriteRenderer>().Sprite.DynamicScaledRect().Height + 2;
            else
                otherSlice.Transform.Position.X += go.GetComponent<SpriteRenderer>().Sprite.DynamicScaledRect().Width + 2;

            return new GameObject[] { go, otherSlice };
        }

        public static void Slice(GameObject go, bool Horizontal)
        {
            GameObject otherSlice = GameObject.Instantiate(go);

            Rectangle sourceRect = go.GetComponent<SpriteRenderer>().Sprite.SourceRectangle;

            Rectangle rect = new Rectangle(sourceRect.X, sourceRect.Y, Horizontal ? sourceRect.Width : sourceRect.Width / 2, Horizontal ? sourceRect.Height / 2 : sourceRect.Height);

            go.GetComponent<SpriteRenderer>().Sprite.SourceRectangle = rect;

            if (Horizontal)
                rect = new Rectangle(sourceRect.X, sourceRect.Y + sourceRect.Height / 2, sourceRect.Width, sourceRect.Height / 2);
            else
                rect = new Rectangle(sourceRect.X + sourceRect.Width / 2, sourceRect.Y, sourceRect.Width / 2, sourceRect.Height);

            otherSlice.GetComponent<SpriteRenderer>().Sprite.SourceRectangle = rect;

            if (Horizontal)
                otherSlice.Transform.Position.Y += go.GetComponent<SpriteRenderer>().Sprite.DynamicScaledRect().Height + 2;
            else
                otherSlice.Transform.Position.X += go.GetComponent<SpriteRenderer>().Sprite.DynamicScaledRect().Width + 2;
        }
    }
}
