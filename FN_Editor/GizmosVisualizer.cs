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
                Microsoft.Xna.Framework.Vector2 avgPosition = Microsoft.Xna.Framework.Vector2.Zero;
                foreach (GameObject go in GameObjects_Tab.SelectedGOs)
                    avgPosition += go.Transform.Position;
                avgPosition /= GameObjects_Tab.SelectedGOs.Count;

                var PlayerPos = avgPosition * Setup.Camera.Zoom;

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

                            foreach (GameObject go in GameObjects_Tab.SelectedGOs)
                                go.Transform.MoveY(Input.MouseDeltaY());
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

                            foreach(GameObject go in GameObjects_Tab.SelectedGOs)
                                go.Transform.MoveX(Input.MouseDeltaX());
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

                            foreach (GameObject go in GameObjects_Tab.SelectedGOs)
                                go.Transform.Move(Input.MouseDelta());
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

                        bool DidClickOnRotaionGizmo = EnteredRotationZone;
                        if (Input.GetMouseClickUp(MouseButtons.LeftClick))
                            EnteredRotationZone = false;

                        if (Input.GetMouseClickDown(MouseButtons.LeftClick))
                        {
                            Microsoft.Xna.Framework.Vector2 SameBias = Utility.Vec2NumericToVec2MG(new Vector2(PlayerPos.X - 57 + SceneWindow.X + 64, PlayerPos.Y - 37 + 64 + SceneWindow.Y) - Bias);
                            if (EnteredRotationZone || Utility.CircleContains(SameBias, 64, Input.GetMousePosition()))
                            {
                                OldTransVal = SelectedGO.Transform.Rotation;
                                foreach (GameObject go in GameObjects_Tab.SelectedGOs)
                                    go.Transform.Rotation = MathCompanion.GetAngle_Rad(SameBias, Input.GetMousePosition());
                                EnteredRotationZone = true;
                            }
                        }

                        if (Input.GetMouseClick(MouseButtons.LeftClick) && EnteredRotationZone)
                        {
                            WasMouseHeld = true;

                            Microsoft.Xna.Framework.Vector2 SameBias = Utility.Vec2NumericToVec2MG(new Vector2(PlayerPos.X - 57 + SceneWindow.X + 64, PlayerPos.Y - 37 + 64 + SceneWindow.Y) - Bias);
                            if (EnteredRotationZone || Utility.CircleContains(SameBias, 64, Input.GetMousePosition()))
                            {
                                foreach (GameObject go in GameObjects_Tab.SelectedGOs)
                                    go.Transform.Rotation = MathCompanion.GetAngle_Rad(SameBias, Input.GetMousePosition());
                                EnteredRotationZone = true;
                            }
                        }

                        if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && WasMouseHeld && DidClickOnRotaionGizmo)
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
                            foreach (GameObject go in GameObjects_Tab.SelectedGOs)
                                go.Transform.ScaleY(-Input.MouseDeltaY() * 0.01f);
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
                            foreach (GameObject go in GameObjects_Tab.SelectedGOs)
                                go.Transform.ScaleX(Input.MouseDeltaX() * 0.01f);
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
                            foreach (GameObject go in GameObjects_Tab.SelectedGOs)
                                go.Transform.ScaleBoth(Microsoft.Xna.Framework.Vector2.One * AverageDelta * Sign);
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

                //////////////////////////
                ///
                BezierLine BL = SelectedGO.GetComponent<BezierLine>();
                if (BL != null)
                {
                    System.Numerics.Vector2 TransPos = new System.Numerics.Vector2(SelectedGO.Transform.Position.X + Setup.graphics.PreferredBackBufferWidth * 0.5f, SelectedGO.Transform.Position.Y + Setup.graphics.PreferredBackBufferHeight * 0.5f);

                    //Point 1
                    ImGui.SetNextItemWidth(10);
                    ImGui.SetCursorPos(new System.Numerics.Vector2(BL.P1.X, BL.P1.Y) + TransPos);
                    ImGui.PushID(SelectedGO.Name + "P1");
                    ImGui.SmallButton("");
                    ImGui.PopID();

                    if (ImGui.IsItemActive())
                        BL.P1 = (Input.GetMousePosition() - new Microsoft.Xna.Framework.Vector2(TransPos.X + SceneWindow.X, TransPos.Y + SceneWindow.Y)).ToPoint();

                    //Point 2
                    ImGui.SetNextItemWidth(10);
                    ImGui.SetCursorPos(new System.Numerics.Vector2(BL.P2.X, BL.P2.Y) + TransPos);
                    ImGui.PushID(SelectedGO.Name + "P2");
                    ImGui.SmallButton("");
                    ImGui.PopID();

                    if (ImGui.IsItemActive())
                        BL.P2 = (Input.GetMousePosition() - new Microsoft.Xna.Framework.Vector2(TransPos.X + SceneWindow.X, TransPos.Y + SceneWindow.Y)).ToPoint();

                    //Point 3
                    ImGui.SetNextItemWidth(10);
                    ImGui.SetCursorPos(new System.Numerics.Vector2(BL.P3.X, BL.P3.Y) + TransPos);
                    ImGui.PushID(SelectedGO.Name + "P3");
                    ImGui.SmallButton("");
                    ImGui.PopID();

                    if (ImGui.IsItemActive())
                        BL.P3 = (Input.GetMousePosition() - new Microsoft.Xna.Framework.Vector2(TransPos.X + SceneWindow.X, TransPos.Y + SceneWindow.Y)).ToPoint();

                    //Point 4
                    if (BL.Quadratic)
                    {
                        ImGui.SetNextItemWidth(10);
                        ImGui.SetCursorPos(new System.Numerics.Vector2(BL.P4.X, BL.P4.Y) + TransPos);
                        ImGui.PushID(SelectedGO.Name + "P4");
                        ImGui.SmallButton("");
                        ImGui.PopID();

                        if (ImGui.IsItemActive())
                            BL.P4 = (Input.GetMousePosition() - new Microsoft.Xna.Framework.Vector2(TransPos.X + SceneWindow.X, TransPos.Y + SceneWindow.Y)).ToPoint();
                    }
                }
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