using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Numerics;

namespace MyEngine.FN_Editor
{
    public class GizmosVisualizer: GameObjectComponent
    {
        private IntPtr Arrow;

        public override void Start()
        {
            Arrow = Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\Arrow"));
        }

        public override void DrawUI()
        {
            ImGui.SetNextWindowPos(Vector2.Zero);
            ImGui.SetNextWindowSize(new Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight));
            ImGui.Begin("Gizoms", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoBringToFrontOnFocus);

            GameObject SelectedGO = GameObjects_Tab.WhoIsSelected;
            if (SelectedGO != null)
            {
                //Vertical Arrow
                ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X - 8, SelectedGO.Transform.Position.Y - 64));
                ImGui.PushID("VertArrow");
                ImGui.ImageButton(Arrow, new Vector2(16, 64), Vector2.UnitX * (96.0f / 512.0f), new Vector2(159.0f / 512.0f, 1), 0, Vector4.Zero, new Vector4(0, 1, 0, 1)) ;
                ImGui.PopID();

                //Horizontal Arrow
                if (ImGui.IsItemActive())
                    SelectedGO.Transform.MoveY(Input.MouseDeltaY());

                ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X + 8, SelectedGO.Transform.Position.Y));
                ImGui.PushID("HorizArrow");
                ImGui.ImageButton(Arrow, new Vector2(64, 16), new Vector2(256.0f / 512.0f, 96.0f / 256.0f), new Vector2(1, 159.0f / 256.0f), 0, Vector4.Zero, new Vector4(0, 1, 1, 1));
                ImGui.PopID();

                //Cube (Full Movement)
                if (ImGui.IsItemActive())
                    SelectedGO.Transform.MoveX(Input.MouseDeltaX());

                ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X - 8, SelectedGO.Transform.Position.Y));
                ImGui.PushID("Cube");
                ImGui.ImageButton(Arrow, new Vector2(16, 16), Vector2.Zero, new Vector2(32.0f / 512.0f, 32.0f / 256.0f), 1, Vector4.Zero, new Vector4(1, 1, 0, 1));
                ImGui.PopID();

                if (ImGui.IsItemActive())
                    SelectedGO.Transform.Move(Input.MouseDelta());
            }

            ImGui.End();
        }
    }
}
