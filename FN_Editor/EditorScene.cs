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
        private bool PopUpOpen = false;

        public override void Start()
        {
            IsThisTheEditor = true;
        }

        public override void DrawUI()
        {
            //Move windows using title bar only
            ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;

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
                            CreateScene(sceneName);
                        }

                        ImGui.PopStyleColor();
                        //var GEO = SceneManager.ActiveScene.FindGameObjectWithName("EditorGameObject");
                        //Scene NewScene = new Scene("NOice Scene");
                        //NewScene.AddGameObject_Recursive(GEO);
                        //SceneManager.SerializeScene(NewScene.Name);
                        //SceneManager.LoadScene_Serialization(NewScene.Name);
                        ImGui.EndMenu();
                    }

                    if (ImGui.MenuItem("Save", "CTRL + S"))
                        SceneManager.SerializeScene(SceneManager.ActiveScene.Name);

                    if (ImGui.BeginMenu("Save As.."))
                    {
                        ImGui.InputText("Name", ref sceneName, 50);

                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 0.1f));

                        if (ImGui.SmallButton("Save"))
                        {
                            //Validate Name Here with the files in the directory
                            string[] Scenes = Directory.GetFiles(Environment.CurrentDirectory).Where(Item => Item.EndsWith(".scene")).ToArray();
                            bool SceneExists = Scenes.Any(Item => Item.Equals(Environment.CurrentDirectory + "\\" + sceneName + ".scene"));

                            if (SceneExists)
                            {
                                ImGui.OpenPopup("Overwrite File?");
                                PopUpOpen = true;
                            }
                            else
                            {
                                string OldSceneName = SceneManager.ActiveScene.Name;
                                SceneManager.ActiveScene.Name = sceneName;
                                SceneManager.SerializeScene(sceneName);
                                SceneManager.ActiveScene.Name = OldSceneName;
                            }
                        }

                        if (ImGui.BeginPopupModal("Overwrite File?", ref PopUpOpen, ImGuiWindowFlags.AlwaysAutoResize))
                        {
                            if (ImGui.Button("Yes, overwrite file"))
                            {
                                string OldSceneName = SceneManager.ActiveScene.Name;
                                SceneManager.ActiveScene.Name = sceneName;
                                SceneManager.SerializeScene(sceneName);
                                SceneManager.ActiveScene.Name = OldSceneName;

                                ImGui.CloseCurrentPopup();
                            }

                            if (ImGui.Button("No") || ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Escape)))
                                ImGui.CloseCurrentPopup();

                            ImGui.EndPopup();
                        }

                        ImGui.PopStyleColor();

                        ImGui.EndMenu();
                    }

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

                    if (ImGui.MenuItem("TileMap Editor"))
                        TilemapEditor.IsWindowOpen = true;

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

        private void CreateScene(string sceneName)
        {
            var ActiveScene = SceneManager.ActiveScene;

            Scene NewScene = new Scene(sceneName);
            SceneManager.ActiveScene = NewScene;

            SceneManager.ActiveScene.AddGameObject_Recursive(ActiveScene.FindGameObjectWithName("EditorGameObject"));
            SceneManager.ActiveScene.AddGameObject_Recursive(ActiveScene.FindGameObjectWithName("Camera Controller"));

            SceneManager.Initialize();

            SceneManager.SerializeScene(NewScene.Name);
            SceneManager.LoadScene_Serialization(NewScene.Name);
            SceneManager.ActiveScene = ActiveScene;

            //settings of ImGui must be loaded or edited
        }
    }
}
