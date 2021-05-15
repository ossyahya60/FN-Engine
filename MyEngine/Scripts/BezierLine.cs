using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
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

        public override void DrawUI()
        {
            if (FN_Editor.GameObjects_Tab.WhoIsSelected == gameObject)
            {
                ImGui.Begin("Gizoms");

                System.Numerics.Vector2 TransPos = new System.Numerics.Vector2(gameObject.Transform.Position.X, gameObject.Transform.Position.Y);

                //Point 1
                ImGui.SetNextItemWidth(10);
                ImGui.SetCursorPos(new System.Numerics.Vector2(P1.X, P1.Y) + TransPos);
                ImGui.PushID(gameObject.Name + "P1");
                ImGui.SmallButton("");
                ImGui.PopID();

                if (ImGui.IsItemActive())
                    P1 = (Input.GetMousePosition() - gameObject.Transform.Position).ToPoint();

                //Point 2
                ImGui.SetNextItemWidth(10);
                ImGui.SetCursorPos(new System.Numerics.Vector2(P2.X, P2.Y) + TransPos);
                ImGui.PushID(gameObject.Name + "P2");
                ImGui.SmallButton("");
                ImGui.PopID();

                if (ImGui.IsItemActive())
                    P2 = (Input.GetMousePosition() - gameObject.Transform.Position).ToPoint();

                //Point 3
                ImGui.SetNextItemWidth(10);
                ImGui.SetCursorPos(new System.Numerics.Vector2(P3.X, P3.Y) + TransPos);
                ImGui.PushID(gameObject.Name + "P3");
                ImGui.SmallButton("");
                ImGui.PopID();

                if (ImGui.IsItemActive())
                    P3 = (Input.GetMousePosition() - gameObject.Transform.Position).ToPoint();

                //Point 4
                if (Quadratic)
                {
                    ImGui.SetNextItemWidth(10);
                    ImGui.SetCursorPos(new System.Numerics.Vector2(P4.X, P4.Y) + TransPos);
                    ImGui.PushID(gameObject.Name + "P4");
                    ImGui.SmallButton("");
                    ImGui.PopID();

                    if (ImGui.IsItemActive())
                        P4 = (Input.GetMousePosition() - gameObject.Transform.Position).ToPoint();
                }

                ImGui.End();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Point TransPos = gameObject.Transform.Position.ToPoint();

            if (Quadratic)
                HitBoxDebuger.BezierLine(P1 + TransPos, P2 + TransPos, P3 + TransPos, P4 + TransPos, color, gameObject.Layer, Quality);
            else
                HitBoxDebuger.BezierLine(P1 + TransPos, P2 + TransPos, P3 + TransPos, color, gameObject.Layer, Quality);
        }
    }
}
