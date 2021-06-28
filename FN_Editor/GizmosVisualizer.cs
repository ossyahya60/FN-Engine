using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace FN_Engine.FN_Editor
{
    enum ActiveGizmo { Movement, Rotation, Scale}

    internal class GizmosVisualizer: GameObjectComponent
    {
        public static Vector2 BiasSceneWindow
        {
            get
            {
                return new Vector2(GameObjects_Tab.MyRegion[1].X, 0);
            }
        }

        public static Vector2 BiasSceneWindowSize
        {
            get
            {
                return new Vector2(Setup.graphics.PreferredBackBufferWidth - (GameObjects_Tab.MyRegion[1].X + InspectorWindow.MyRegion[1].X), Setup.graphics.PreferredBackBufferHeight - (ContentWindow.MyRegion[1].Y /*+ ContentWindow.MyRegion[1].Y*/));
            }
        }

        private IntPtr Arrow;
        private IntPtr ScaleGizmo;
        private IntPtr RotationGizmo;
        private ActiveGizmo ActiveGizmo = ActiveGizmo.Movement;
        private bool EnteredRotationZone = false;
        private bool WasMouseHeld = false;
        private object OldTransVal = null;

        public override void Start()
        {
            Arrow = Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\Arrow"));
            ScaleGizmo = Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\ScaleGizmo"));
            RotationGizmo = Scene.GuiRenderer.BindTexture(HitBoxDebuger.CreateCircleTextureShell(64, 62, Microsoft.Xna.Framework.Color.LightYellow));
        }

        public override void DrawUI()
        {
            Vector2 Bias = 0.5f * new Vector2(Setup.graphics.PreferredBackBufferWidth - Setup.Camera.Position.X * 2, Setup.graphics.PreferredBackBufferHeight - Setup.Camera.Position.Y * 2);
            ImGui.SetNextWindowPos(BiasSceneWindow);
            ImGui.SetNextWindowSize(BiasSceneWindowSize);
            ImGui.Begin("Gizoms", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoBringToFrontOnFocus);

            GameObject SelectedGO = GameObjects_Tab.WhoIsSelected;
            if (SelectedGO != null && SelectedGO.Transform != null)
            {
                //Transform Visualization
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
                        ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X - 8, SelectedGO.Transform.Position.Y - 64) + Bias);
                        ImGui.PushID("SelectedGO.Name + VertArrow");
                        ImGui.ImageButton(Arrow, new Vector2(16, 64), Vector2.UnitX * (96.0f / 512.0f), new Vector2(159.0f / 512.0f, 1), 1, Vector4.Zero, new Vector4(0, 1, 0, 1));
                        ImGui.PopID();

                        if (ImGui.IsItemActive())
                        {
                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                OldTransVal = SelectedGO.Transform.Position;

                            SelectedGO.Transform.MoveY(Input.MouseDeltaY());
                            WasMouseHeld = true;
                        }

                        //Horizontal Arrow
                        ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X + 8, SelectedGO.Transform.Position.Y) + Bias);
                        ImGui.PushID(SelectedGO.Name + "HorizArrow");
                        ImGui.ImageButton(Arrow, new Vector2(64, 16), new Vector2(256.0f / 512.0f, 96.0f / 256.0f), new Vector2(1, 159.0f / 256.0f), 1, Vector4.Zero, new Vector4(0, 1, 1, 1));
                        ImGui.PopID();

                        if (ImGui.IsItemActive())
                        {
                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                OldTransVal = SelectedGO.Transform.Position;

                            SelectedGO.Transform.MoveX(Input.MouseDeltaX());
                            WasMouseHeld = true;
                        }

                        //Cube (Full Movement)
                        ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X - 8, SelectedGO.Transform.Position.Y) + Bias);
                        ImGui.PushID("SelectedGO.Name + Cube");
                        ImGui.ImageButton(Arrow, new Vector2(16, 16), Vector2.Zero, new Vector2(32.0f / 512.0f, 32.0f / 256.0f), 1, Vector4.Zero, new Vector4(1, 1, 0, 1));
                        ImGui.PopID();

                        if (ImGui.IsItemActive())
                        {
                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                OldTransVal = SelectedGO.Transform.Position;

                            SelectedGO.Transform.Move(Input.MouseDelta());
                            WasMouseHeld = true;
                        }

                        if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && WasMouseHeld)
                        {
                            GameObjects_Tab.AddToACircularBuffer(GameObjects_Tab.Undo_Buffer, new KeyValuePair<object, Operation>(new KeyValuePair<object, object>(new KeyValuePair<object, object>(SelectedGO.Transform, typeof(Transform).GetMember("Position")[0]), OldTransVal), Operation.ChangeValue));
                            GameObjects_Tab.Redo_Buffer.Clear();
                            WasMouseHeld = false;
                        }

                        break;
                    case ActiveGizmo.Rotation:

                        //Circular Shell
                        ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X - 64, SelectedGO.Transform.Position.Y - 64) + Bias);
                        ImGui.PushID("SelectedGO.Name + RotationGizmo");
                        ImGui.Image(RotationGizmo, new Vector2(128, 128));
                        ImGui.PopID();

                        if (Input.GetMouseClickUp(MouseButtons.LeftClick))
                            EnteredRotationZone = false;

                        if(Input.GetMouseClick(MouseButtons.LeftClick))
                        {
                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                OldTransVal = SelectedGO.Transform.Rotation;

                            WasMouseHeld = true;

                            Microsoft.Xna.Framework.Vector2 SameBias = new Microsoft.Xna.Framework.Vector2(Bias.X + BiasSceneWindow.X, Bias.Y + BiasSceneWindow.Y);
                            if (EnteredRotationZone || Utility.CircleContains(SelectedGO.Transform.Position + SameBias, 64, Input.GetMousePosition()))
                            {
                                SelectedGO.Transform.Rotation = MathCompanion.GetAngle_Rad(SelectedGO.Transform.Position + SameBias, Input.GetMousePosition());
                                EnteredRotationZone = true;
                            }
                        }

                        if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && WasMouseHeld)
                        {
                            GameObjects_Tab.AddToACircularBuffer(GameObjects_Tab.Undo_Buffer, new KeyValuePair<object, Operation>(new KeyValuePair<object, object>(new KeyValuePair<object, object>(SelectedGO.Transform, typeof(Transform).GetMember("Rotation")[0]), OldTransVal), Operation.ChangeValue));
                            GameObjects_Tab.Redo_Buffer.Clear();
                            WasMouseHeld = false;
                        }

                        break;
                    case ActiveGizmo.Scale:

                        //Vertical Scale
                        ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X - 8, SelectedGO.Transform.Position.Y - 64) + Bias);
                        ImGui.PushID("SelectedGO.Name + VertScale");
                        ImGui.ImageButton(ScaleGizmo, new Vector2(16, 64), Vector2.UnitX * (96.0f / 512.0f), new Vector2(159.0f / 512.0f, 1), 1, Vector4.Zero, new Vector4(0, 1, 0, 1));
                        ImGui.PopID();

                        if (ImGui.IsItemActive())
                        {
                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                OldTransVal = SelectedGO.Transform.Scale;

                            WasMouseHeld = true;

                            SelectedGO.Transform.ScaleY(-Input.MouseDeltaY() * 0.01f);
                        }

                        //Horizontal Scale
                        ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X + 8, SelectedGO.Transform.Position.Y) + Bias);
                        ImGui.PushID("SelectedGO.Name + HorizScale");
                        ImGui.ImageButton(ScaleGizmo, new Vector2(64, 16), new Vector2(256.0f / 512.0f, 96.0f / 256.0f), new Vector2(1, 159.0f / 256.0f), 1, Vector4.Zero, new Vector4(0, 1, 1, 1));
                        ImGui.PopID();

                        if (ImGui.IsItemActive())
                        {
                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                OldTransVal = SelectedGO.Transform.Scale;

                            WasMouseHeld = true;

                            SelectedGO.Transform.ScaleX(Input.MouseDeltaX() * 0.01f);
                        }

                        //Cube (Full Scale)
                        ImGui.SetCursorPos(new Vector2(SelectedGO.Transform.Position.X - 8, SelectedGO.Transform.Position.Y) + Bias);
                        ImGui.PushID("SelectedGO.Name + FullScale");
                        ImGui.ImageButton(ScaleGizmo, new Vector2(16, 16), Vector2.Zero, new Vector2(32.0f / 512.0f, 32.0f / 256.0f), 1, Vector4.Zero, new Vector4(1, 1, 0, 1));
                        ImGui.PopID();

                        if (ImGui.IsItemActive())
                        {
                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                OldTransVal = SelectedGO.Transform.Scale;

                            WasMouseHeld = true;

                            int Sign = Input.MouseDeltaX() >= 0 ? 1 : -1;
                            float AverageDelta = (float)Math.Sqrt(Input.MouseDelta().X * Input.MouseDelta().X + Input.MouseDelta().Y * Input.MouseDelta().Y) * 0.01f;
                            SelectedGO.Transform.ScaleBoth(Microsoft.Xna.Framework.Vector2.One * AverageDelta * Sign);
                        }

                        if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && WasMouseHeld)
                        {
                            GameObjects_Tab.AddToACircularBuffer(GameObjects_Tab.Undo_Buffer, new KeyValuePair<object, Operation>(new KeyValuePair<object, object>(new KeyValuePair<object, object>(SelectedGO.Transform, typeof(Transform).GetMember("Scale")[0]), OldTransVal), Operation.ChangeValue));
                            GameObjects_Tab.Redo_Buffer.Clear();
                            WasMouseHeld = false;
                        }

                        break;
                }
                
                //Collider Visualization
                var Colliders = SelectedGO.GameObjectComponents.FindAll(Item => Item is Collider2D);

                if (Colliders.Count != 0) // Scale with Scene Camera?
                    foreach (Collider2D collider in Colliders)
                        collider.Visualize(Bias.X + BiasSceneWindow.X, Bias.Y + BiasSceneWindow.Y);
            }

            ImGui.End();
        }
    }
}