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

        public static bool IsThisWindowHovered = true;
        public static bool ShowGizmos = true;
        public static Vector4 SceneWindow;

        private static IntPtr Arrow;
        private static IntPtr ScaleGizmo;
        private static IntPtr RotationGizmo;
        private static ActiveGizmo ActiveGizmo = ActiveGizmo.Movement;
        private static bool EnteredRotationZone = false;
        private static bool WasMouseHeld = false;
        private static object OldTransVal = null;

        public override void Start()
        {
            Arrow = Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\Arrow"));
            ScaleGizmo = Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\ScaleGizmo"));
            RotationGizmo = Scene.GuiRenderer.BindTexture(HitBoxDebuger.CreateCircleTextureShell(64, 62, Microsoft.Xna.Framework.Color.LightYellow));
        }

        public static void DrawGizmos()
        {
            SceneWindow = new Vector4(ImGui.GetWindowPos().X, ImGui.GetWindowPos().Y, ImGui.GetWindowWidth(), ImGui.GetWindowHeight());

            IsThisWindowHovered = ImGui.IsWindowHovered();

            var Vec3 = Setup.Camera.GetViewTransformationMatrix().Translation;
            Vector2 Bias = new Vector2(-Vec3.X, -Vec3.Y);

            GameObject SelectedGO = GameObjects_Tab.WhoIsSelected;
            if (ShowGizmos && SelectedGO != null && SelectedGO.Active && SelectedGO.Transform != null)
            {
                var PlayerPos = SelectedGO.Transform.Position * Setup.Camera.Zoom;

                //Transform Visualization
                if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
                    ActiveGizmo = ActiveGizmo.Movement;
                else if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                    ActiveGizmo = ActiveGizmo.Rotation;
                else if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.E))
                    ActiveGizmo = ActiveGizmo.Scale;

                ImGui.PushStyleColor(ImGuiCol.Button, 0);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0);
                switch (ActiveGizmo)
                {
                    case ActiveGizmo.Movement:

                        //Vertical Arrow
                        ImGui.SetCursorPos(new Vector2(PlayerPos.X, PlayerPos.Y - 44) - Bias);
                        ImGui.PushID("SelectedGO.Name + VertArrow");
                        ImGui.ImageButton(Arrow, new Vector2(16, 64), Vector2.UnitX * (96.0f / 512.0f), new Vector2(159.0f / 512.0f, 1), 0, Vector4.Zero, new Vector4(0, 1, 0, 1));
                        ImGui.PopID();

                        if (ImGui.IsItemActive())
                        {
                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                OldTransVal = PlayerPos;

                            SelectedGO.Transform.MoveY(Input.MouseDeltaY());
                            WasMouseHeld = true;
                        }

                        //Horizontal Arrow
                        ImGui.SetCursorPos(new Vector2(PlayerPos.X + 16, PlayerPos.Y + 20) - Bias);
                        ImGui.PushID(SelectedGO.Name + "HorizArrow");
                        ImGui.ImageButton(Arrow, new Vector2(64, 16), new Vector2(256.0f / 512.0f, 96.0f / 256.0f), new Vector2(1, 159.0f / 256.0f), 1, Vector4.Zero, new Vector4(0, 1, 1, 1));
                        ImGui.PopID();

                        if (ImGui.IsItemActive())
                        {
                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                OldTransVal = PlayerPos;

                            SelectedGO.Transform.MoveX(Input.MouseDeltaX());
                            WasMouseHeld = true;
                        }

                        //Cube (Full Movement)
                        ImGui.SetCursorPos(new Vector2(PlayerPos.X, PlayerPos.Y + 20) - Bias);
                        ImGui.PushID("SelectedGO.Name + Cube");
                        ImGui.ImageButton(Arrow, new Vector2(16, 16), Vector2.Zero, new Vector2(32.0f / 512.0f, 32.0f / 256.0f), 1, Vector4.Zero, new Vector4(1, 1, 0, 1));
                        ImGui.PopID();

                        if (ImGui.IsItemActive())
                        {
                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                OldTransVal = PlayerPos;

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
                        ImGui.SetCursorPos(new Vector2(PlayerPos.X - 57, PlayerPos.Y - 37) - Bias);
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

                            Microsoft.Xna.Framework.Vector2 SameBias = new Microsoft.Xna.Framework.Vector2(BiasSceneWindow.X, BiasSceneWindow.Y);
                            if (EnteredRotationZone || Utility.CircleContains(PlayerPos + SameBias, 64, Input.GetMousePosition()))
                            {
                                SelectedGO.Transform.Rotation = MathCompanion.GetAngle_Rad(PlayerPos + SameBias, Input.GetMousePosition());
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
                        ImGui.SetCursorPos(new Vector2(PlayerPos.X, PlayerPos.Y - 44) - Bias);
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
                        ImGui.SetCursorPos(new Vector2(PlayerPos.X + 16, PlayerPos.Y + 20) - Bias);
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
                        ImGui.SetCursorPos(new Vector2(PlayerPos.X, PlayerPos.Y + 20) - Bias);
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
                ImGui.PopStyleColor(3);
            }

            if (ImGui.IsWindowHovered() && ImGui.IsMouseReleased(ImGuiMouseButton.Left) && ContentWindow.DraggedAsset is GameObject)
            {
                GameObject prefab = ContentWindow.DraggedAsset as GameObject;
                GameObject Instance = GameObject.Instantiate(prefab);
                var NewBias = Bias - ImGui.GetWindowPos();
                Instance.Transform.Position = Input.GetMousePosition() + new Microsoft.Xna.Framework.Vector2(NewBias.X, NewBias.Y);

                GameObjects_Tab.AddToACircularBuffer(GameObjects_Tab.Undo_Buffer, new KeyValuePair<object, Operation>(Instance, Operation.Create));
                GameObjects_Tab.Redo_Buffer.Clear();

                ImGui.EndDragDropTarget();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameObjects_Tab.WhoIsSelected != null && GameObjects_Tab.WhoIsSelected.Active)
            {
                //Collider Visualization
                var Colliders = GameObjects_Tab.WhoIsSelected.GameObjectComponents.FindAll(Item => Item is Collider2D);

                if (Colliders.Count != 0) // Scale with Scene Camera?
                    foreach (Collider2D collider in Colliders)
                        collider.Visualize();
            }
        }
    }
}