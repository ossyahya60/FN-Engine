using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Numerics;

namespace MyEngine.FN_Editor
{
    enum ActiveGizmo { Movement, Rotation, Scale}

    public class GizmosVisualizer: GameObjectComponent
    {
        private IntPtr Arrow;
        private IntPtr ScaleGizmo;
        private IntPtr RotationGizmo;
        private ActiveGizmo ActiveGizmo = ActiveGizmo.Movement;
        private bool EnteredRotationZone = false;

        public override void Start()
        {
            Arrow = Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\Arrow"));
            ScaleGizmo = Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\ScaleGizmo"));
            RotationGizmo = Scene.GuiRenderer.BindTexture(HitBoxDebuger.CreateCircleTextureShell(64, 62, Microsoft.Xna.Framework.Color.LightYellow));
        }

        public override void DrawUI()
        {
            ImGui.SetNextWindowPos(Vector2.Zero);
            ImGui.SetNextWindowSize(new Vector2(Setup.graphics.PreferredBackBufferWidth, Setup.graphics.PreferredBackBufferHeight));
            ImGui.Begin("Gizoms", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoBringToFrontOnFocus);

            GameObject SelectedGO = GameObjects_Tab.WhoIsSelected;
            if (SelectedGO != null)
            {
                if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
                    ActiveGizmo = ActiveGizmo.Movement;
                else if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                    ActiveGizmo = ActiveGizmo.Rotation;
                else if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.E))
                    ActiveGizmo = ActiveGizmo.Scale;

                switch (ActiveGizmo)
                {
                    case ActiveGizmo.Movement:

                        //Vertical Arrow
                        ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X - 8, SelectedGO.Transform.Position.Y - 64));
                        ImGui.PushID("VertArrow");
                        ImGui.ImageButton(Arrow, new Vector2(16, 64), Vector2.UnitX * (96.0f / 512.0f), new Vector2(159.0f / 512.0f, 1), 1, Vector4.Zero, new Vector4(0, 1, 0, 1));
                        ImGui.PopID();

                        //Horizontal Arrow
                        if (ImGui.IsItemActive())
                            SelectedGO.Transform.MoveY(Input.MouseDeltaY());

                        ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X + 8, SelectedGO.Transform.Position.Y));
                        ImGui.PushID("HorizArrow");
                        ImGui.ImageButton(Arrow, new Vector2(64, 16), new Vector2(256.0f / 512.0f, 96.0f / 256.0f), new Vector2(1, 159.0f / 256.0f), 1, Vector4.Zero, new Vector4(0, 1, 1, 1));
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

                        break;
                    case ActiveGizmo.Rotation:

                        //Circular Shell
                        ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X - 64, SelectedGO.Transform.Position.Y - 64));
                        ImGui.PushID("RotationGizmo");
                        ImGui.Image(RotationGizmo, new Vector2(128, 128));
                        ImGui.PopID();

                        if (Input.GetMouseClickUp(MouseButtons.LeftClick))
                            EnteredRotationZone = false;

                        if(Input.GetMouseClick(MouseButtons.LeftClick))
                        {
                            if (EnteredRotationZone || Utility.CircleContains(SelectedGO.Transform.Position, 64, Input.GetMousePosition()))
                            {
                                SelectedGO.Transform.Rotation = MathCompanion.GetAngle_Rad(SelectedGO.Transform.Position, Input.GetMousePosition());
                                EnteredRotationZone = true;
                            }
                        }

                        break;
                    case ActiveGizmo.Scale:

                        //Vertical Scale
                        ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X - 8, SelectedGO.Transform.Position.Y - 64));
                        ImGui.PushID("VertScale");
                        ImGui.ImageButton(ScaleGizmo, new Vector2(16, 64), Vector2.UnitX * (96.0f / 512.0f), new Vector2(159.0f / 512.0f, 1), 1, Vector4.Zero, new Vector4(0, 1, 0, 1));
                        ImGui.PopID();

                        //Horizontal Scale
                        if (ImGui.IsItemActive())
                            SelectedGO.Transform.ScaleY(-Input.MouseDeltaY() * 0.01f);

                        ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X + 8, SelectedGO.Transform.Position.Y));
                        ImGui.PushID("HorizScale");
                        ImGui.ImageButton(ScaleGizmo, new Vector2(64, 16), new Vector2(256.0f / 512.0f, 96.0f / 256.0f), new Vector2(1, 159.0f / 256.0f), 1, Vector4.Zero, new Vector4(0, 1, 1, 1));
                        ImGui.PopID();

                        //Cube (Full Scale)
                        if (ImGui.IsItemActive())
                            SelectedGO.Transform.ScaleX(Input.MouseDeltaX() * 0.01f);

                        ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X - 8, SelectedGO.Transform.Position.Y));
                        ImGui.PushID("FullScale");
                        ImGui.ImageButton(ScaleGizmo, new Vector2(16, 16), Vector2.Zero, new Vector2(32.0f / 512.0f, 32.0f / 256.0f), 1, Vector4.Zero, new Vector4(1, 1, 0, 1));
                        ImGui.PopID();

                        if (ImGui.IsItemActive())
                        {
                            float AverageDelta = (float)Math.Sqrt(Input.MouseDelta().X * Input.MouseDelta().X + Input.MouseDelta().Y * Input.MouseDelta().Y) * 0.01f;
                            SelectedGO.Transform.ScaleBoth(AverageDelta * Math.Sign(Input.MouseDelta().X), -AverageDelta * Math.Sign(Input.MouseDelta().Y));
                        }

                        break;
                }
            }

            ImGui.End();
        }
    }
}
