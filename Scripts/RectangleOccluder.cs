using Microsoft.Xna.Framework;

namespace FN_Engine
{
    public class RectangleOccluder: GameObjectComponent
    {
        private LineOccluder[] lineOccluders;
        private SpriteRenderer SR = null;

        public override void Start()
        {
            SR = gameObject.GetComponent<SpriteRenderer>();

            lineOccluders = new LineOccluder[4];
        }

        public LineOccluder[] ConvertToLineOccluders(Rectangle Rect)
        {
            lineOccluders[0] = new LineOccluder(new Vector2(Rect.Left, Rect.Top), new Vector2(Rect.Right, Rect.Top));
            lineOccluders[1] = new LineOccluder(new Vector2(Rect.Right, Rect.Top), new Vector2(Rect.Right, Rect.Bottom));
            lineOccluders[2] = new LineOccluder(new Vector2(Rect.Right, Rect.Bottom), new Vector2(Rect.Left, Rect.Bottom));
            lineOccluders[3] = new LineOccluder(new Vector2(Rect.Left, Rect.Bottom), new Vector2(Rect.Left, Rect.Top));

            return lineOccluders;
        }

        public LineOccluder[] ConvertToLineOccluders()
        {
            Rectangle Occluder = SR.Sprite.DynamicScaledRect();

            lineOccluders[0] = new LineOccluder(new Vector2(Occluder.Left, Occluder.Top), new Vector2(Occluder.Right, Occluder.Top));
            lineOccluders[1] = new LineOccluder(new Vector2(Occluder.Right, Occluder.Top), new Vector2(Occluder.Right, Occluder.Bottom));
            lineOccluders[2] = new LineOccluder(new Vector2(Occluder.Right, Occluder.Bottom), new Vector2(Occluder.Left, Occluder.Bottom));
            lineOccluders[3] = new LineOccluder(new Vector2(Occluder.Left, Occluder.Bottom), new Vector2(Occluder.Left, Occluder.Top));

            return lineOccluders;
        }
    }
}
