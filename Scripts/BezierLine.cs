using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FN_Engine
{
    public class BezierLine: GameObjectComponent
    {
        public int Quality = 10;
        public Color color = Color.White;
        public bool Quadratic = false;
        public Point P1;
        public Point P2;
        public Point P3;
        public Point P4;

        public override void Start()
        {
            P1 = new Point(0, 0);
            P2 = new Point(100, 0);
            P3 = new Point(150, 0);
            P4 = new Point(200, 0);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Point TransPos = gameObject.Transform.Position.ToPoint();

            if (Quadratic)
                HitBoxDebuger.BezierLine(P1 + TransPos, P2 + TransPos, P3 + TransPos, P4 + TransPos, color, gameObject.Layer, Quality);
            else
                HitBoxDebuger.BezierLine(P1 + TransPos, P2 + TransPos, P3 + TransPos, color, gameObject.Layer, Quality);
        }

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            return base.DeepCopy(Clone);
        }
    }
}
