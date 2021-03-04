using Microsoft.Xna.Framework;

namespace MyEngine
{
    public class LineOccluder
    {
        public Vector2 StartPoint;
        public Vector2 EndPoint;

        public LineOccluder()
        {
            StartPoint = Vector2.Zero;
            EndPoint = Vector2.UnitX;
        }

        public LineOccluder(Vector2 Start, Vector2 End)
        {
            StartPoint = Start;
            EndPoint = End;
        }

        public void SetOccluder(Vector2 Start, Vector2 End)
        {
            StartPoint = Start;
            EndPoint = End;
        }
    }
}
