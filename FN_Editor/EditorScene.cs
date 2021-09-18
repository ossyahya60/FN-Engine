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
                Setup.Camera.Move(-Input.MouseDelta(), ImGui.GetIO().DeltaTime * 60 / Setup.Camera.Zoom);

            if (ImGui.GetIO().KeyCtrl && Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.S)) //Save scene
                SceneManager.SerializeScene(SceneManager.ActiveScene.Name);

            if (ImGui.GetIO().KeyCtrl && Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.G)) //Show Gizmos
                GizmosVisualizer.ShowGizmos = !GizmosVisualizer.ShowGizmos;

            if (ImGui.GetIO().KeyCtrl && Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.L)) //Launch game
            {
                SceneManager.SerializeScene(SceneManager.ActiveScene.Name);
                Threader.Invoke(FN_Project.VisualizeEngineStartup.RunExecutable, 0);
            }

            if (ImGui.GetIO().KeyCtrl && Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.P)) //Play In Editor
            {
                SceneManager.SerializeScene(SceneManager.ActiveScene.Name);

                SceneManager.ShouldUpdate = !SceneManager.ShouldUpdate;
                if (SceneManager.ShouldUpdate)
                    SceneManager.ActiveScene.Start();
                else
                    SceneManager.LoadScene_Serialization(SceneManager.LastLoadPath);
            }

            bool OpenHelp = false;
            if (ImGui.BeginMainMenuBar())
            {
                //ImGui.SetNextItemOpen(true);

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

                    if (ImGui.MenuItem("Play", "CTRL + P"))
                    {
                        SceneManager.SerializeScene(SceneManager.ActiveScene.Name);

                        SceneManager.ShouldUpdate = !SceneManager.ShouldUpdate;
                        if (SceneManager.ShouldUpdate)
                            SceneManager.ActiveScene.Start();
                        else
                            SceneManager.LoadScene_Serialization(SceneManager.LastLoadPath);
                    }

                    if (ImGui.MenuItem("Launch Game", "CTRL + L"))
                    {
                        SceneManager.SerializeScene(SceneManager.ActiveScene.Name);
                        Threader.Invoke(FN_Project.VisualizeEngineStartup.RunExecutable, 0);
                    }

                    if (ImGui.MenuItem("Open Sln"))
                        Utility.ExecuteCommand(new string[] {"start " + FN_Project.VisualizeEngineStartup.GamePath + "\\" + FN_Project.VisualizeEngineStartup.GamePath.Substring(FN_Project.VisualizeEngineStartup.GamePath.LastIndexOf("\\") + 1) + ".sln" }, FN_Project.VisualizeEngineStartup.GamePath);

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

                if (ImGui.BeginMenu("Config"))
                {
                    ImGui.Checkbox("Show Gizmos (CTRL + G)", ref GizmosVisualizer.ShowGizmos);
                    ImGui.Checkbox("Auto Config Windows", ref AutoConfigureWindows);

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.MenuItem("Shortcuts"))
                        OpenHelp = true;

                    ImGui.EndMenu();
                }

                //var CursPosX = ImGui.GetCursorPosX();

                //ImGui.SetCursorPosX(Setup.graphics.PreferredBackBufferWidth * 0.5f);
                //if (ImGui.Checkbox("Play", ref SceneManager.ShouldUpdate))
                //{
                //    if (SceneManager.ShouldUpdate)
                //        SceneManager.ActiveScene.Start();
                //    else
                //        SceneManager.LoadScene_Serialization(SceneManager.LastLoadPath);
                //}
                //ImGui.SetCursorPosX(CursPosX);

                ImGui.EndMainMenuBar();
            }

            if(OpenHelp)
                ImGui.OpenPopup("Shortcuts Popup");

            if (ImGui.BeginPopup("Shortcuts Popup"))
            {
                if (ImGui.TreeNode("General"))
                {
                    ImGui.BulletText("To save the scene in editor => Ctrl + S");
                    ImGui.BulletText("To reload the scene in editor => Ctrl + R");
                    ImGui.BulletText("To play the scene in editor => Ctrl + P");
                    ImGui.BulletText("To launch the game => Ctrl + L");
                    ImGui.BulletText("You can Undo/Redo most actions by pressing Ctrl + Z/Y");

                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Scene Window"))
                {
                    ImGui.BulletText("To multiselect gameObjects => hold Ctrl then click on a gameObject");
                    ImGui.BulletText("To copy a gameObject => Select then Ctrl + C");
                    ImGui.BulletText("To cut a gameObject => Select then Ctrl + X");
                    ImGui.BulletText("To duplicate a gameObject => Select then Ctrl + D");
                    ImGui.BulletText("To delete a gameObject => Select then press \"Delete\" button on the keyboard");
                    ImGui.BulletText("You can drag a gameObject");
                    ImGui.BulletText("You can parent/unparent a gameobject using drag/drop");
                    ImGui.BulletText("Right click on an empty space to open gamObject template creation menu");

                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Inspector Window"))
                {
                    ImGui.BulletText("To ensure that your action is registered in the undo \nbuffer, simply press enter after editing a writable widget");
                    ImGui.BulletText("Drag/Drop is supported for dragging a component from its \"Header\"\nthat contains its name");
                    ImGui.BulletText("To fine tune or edit a widget value, simply press Ctrl + Mouse click on it!\nN.B: Be careful, this allows you to exceed any constraints on the input!");

                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Content Window"))
                {
                    ImGui.BulletText("Drag/Drop is supported for dragging a Texture/Audio/Shader and dropping\nit on the appropriate field");
                    ImGui.BulletText("You can Import assets outside the engine by simply\ncopying them to the clipboad then press \"Paste Clipboard\" button");

                    ImGui.TreePop();
                }

                ImGui.EndPopup();
            }

            //Docking Part (Experimental
            //ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

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
            SceneManager.LoadScene_Serialization("Scenes\\" + NewScene.Name);
            SceneManager.ActiveScene = ActiveScene;

            //settings of ImGui must be loaded or edited
        }
    }
}
