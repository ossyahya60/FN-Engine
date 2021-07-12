using ImGuiNET;
using Microsoft.Xna.Framework;

namespace FN_Engine.FN_Editor
{
    internal class EditorScene: GameObjectComponent
    {
        public static bool IsThisTheEditor = false;

        public override void Start()
        {
            IsThisTheEditor = true;
        }

        public override void DrawUI()
        {
            ImGui.GetIO().ConfigWindowsResizeFromEdges = true;

            //Move windows using title bar only
            ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;

            //FPS
            //ImGui.Text(((int)(1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds)).ToString());
            ImGui.Text("Mouse Pos: " + Input.GetMousePosition().ToString());

            if (ImGui.IsMouseDragging(ImGuiMouseButton.Right))
                Setup.Camera.Move(Input.MouseDelta(), 1);

            if (ImGui.GetIO().KeyCtrl && Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                SceneManager.SerializeScene(SceneManager.ActiveScene.Name);

            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Serialize"))
                        SceneManager.SerializeScene("DefaultScene");

                    if (ImGui.MenuItem("Play"))
                    {
                        SceneManager.SerializeScene(SceneManager.ActiveScene.Name);
                        Threader.Invoke(FN_Project.VisualizeEngineStartup.RunExecutable, 0);
                    }

                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }

            //GameObject ChosenGO = SceneManager.ActiveScene.GameObjects[0];
            //if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && GameObjects_Tab.WhoIsSelected == null)
            //{
            //    foreach (GameObject GO in SceneManager.ActiveScene.GameObjects)
            //    {
            //        SpriteRenderer SR = GO.GetComponent<SpriteRenderer>();
            //        if (SR != null && SR.Sprite != null)
            //        {
            //            if (SR.Sprite.DynamicScaledRect().Contains(Input.GetMousePosition()))
            //            {
            //                if(GO.Layer < ChosenGO.Layer)
            //                    ChosenGO = GO;
            //            }
            //        }
            //    }

            //    GameObjects_Tab.WhoIsSelected = ChosenGO;
            //}
        }
    }
}
