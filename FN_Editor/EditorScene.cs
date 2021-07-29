using ImGuiNET;
using System.Numerics;
using System;
using System.IO;
using System.Linq;

namespace FN_Engine.FN_Editor
{
    internal class EditorScene: GameObjectComponent
    {
        public static bool IsThisTheEditor = false;
        public static bool AutoConfigureWindows = true;

        private string sceneName = "Default";

        public override void Start()
        {
            IsThisTheEditor = true;
        }

        public override void DrawUI()
        {
            //Move windows using title bar only
            ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;

            //FPS
            //ImGui.Text(((int)(1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds)).ToString());
            //ImGui.Text("Mouse Pos: " + Input.GetMousePosition().ToString());

            if (ImGui.IsMouseDragging(ImGuiMouseButton.Right))
                Setup.Camera.Move(Input.MouseDelta(), 1);

            if (ImGui.GetIO().KeyCtrl && Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                SceneManager.SerializeScene(SceneManager.ActiveScene.Name);

            if (ImGui.GetIO().KeyCtrl && Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.P))
            {
                SceneManager.SerializeScene(SceneManager.ActiveScene.Name);
                Threader.Invoke(FN_Project.VisualizeEngineStartup.RunExecutable, 0);
            }

            if (ImGui.BeginMainMenuBar())
            {
                ImGui.SetNextItemOpen(true);

                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.BeginMenu("New Scene"))
                    {
                        ImGui.InputText("Name", ref sceneName, 50);

                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 0.1f));

                        if (ImGui.SmallButton("Create"))
                        {
                            //Validate Name Here with the files in the directory
                            string[] Scenes = Directory.GetFiles(Environment.CurrentDirectory).Where(Item => Item.EndsWith(".scene")).ToArray();
                            sceneName = Utility.UniqueName(sceneName, Scenes);

                            //Create Scene
                            SceneManager.SerializeScene(SceneManager.ActiveScene.Name);

                            var ActiveScene = SceneManager.ActiveScene;

                            GameObject GO = new GameObject(true);
                            GO.Name = "EditorGameObject";
                            GO.Layer = -1;
                            GO.AddComponent(new FN_Editor.GameObjects_Tab());
                            GO.AddComponent(new FN_Editor.InspectorWindow());
                            GO.AddComponent(new FN_Editor.ContentWindow());
                            GO.AddComponent(new FN_Editor.GizmosVisualizer());
                            GO.AddComponent(new FN_Editor.EditorScene());

                            GameObject CamerContr = new GameObject();
                            CamerContr.Name = "Camera Controller";
                            CamerContr.AddComponent(new Transform());
                            CamerContr.AddComponent(new CameraController());

                            Scene NewScene = new Scene(sceneName);
                            SceneManager.ActiveScene = NewScene;

                            SceneManager.ActiveScene.AddGameObject_Recursive(GO);
                            SceneManager.ActiveScene.AddGameObject_Recursive(CamerContr);

                            SceneManager.Initialize();

                            SceneManager.SerializeScene(NewScene.Name);
                            SceneManager.LoadScene_Serialization(NewScene.Name);
                            SceneManager.ActiveScene = ActiveScene;

                            //settings of ImGui must be loaded or edited
                        }

                        //var GEO = SceneManager.ActiveScene.FindGameObjectWithName("EditorGameObject");
                        //Scene NewScene = new Scene("NOice Scene");
                        //NewScene.AddGameObject_Recursive(GEO);
                        //SceneManager.SerializeScene(NewScene.Name);
                        //SceneManager.LoadScene_Serialization(NewScene.Name);
                        ImGui.EndMenu();
                    }

                    if (ImGui.MenuItem("Save", "CTRL + S"))
                        SceneManager.SerializeScene(SceneManager.ActiveScene.Name);

                    if (ImGui.MenuItem("Launch Game", "CTRL + P"))
                    {
                        SceneManager.SerializeScene(SceneManager.ActiveScene.Name);
                        Threader.Invoke(FN_Project.VisualizeEngineStartup.RunExecutable, 0);
                    }

                    ImGui.Checkbox("Enable Update", ref SceneManager.ShouldUpdate);

                    ImGui.Checkbox("Auto Config Windows", ref AutoConfigureWindows);

                    ImGui.EndMenu();
                }

                if(ImGui.BeginMenu("Tools"))
                {
                    if (ImGui.MenuItem("Animation Editor"))
                        AnimationEditor.IsWindowOpen = true;

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
