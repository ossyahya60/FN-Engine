using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace MyEngine.FN_Editor
{
    class GameObjects_Tab: GameObjectComponent
    {
        public static GameObject WhoIsSelected = null;

        public override void Start()
        {
            WhoIsSelected = null;
        }

        public override void DrawUI()
        {
            //ImGui.ShowDemoWindow();

            //Scene Tab
            ImGui.Begin(SceneManager.ActiveScene.Name);

            if (ImGui.IsMouseClicked(0) && ImGui.IsWindowHovered(ImGuiHoveredFlags.RootWindow))
                WhoIsSelected = null;

            ImGui.Indent();

            foreach (GameObject GO in SceneManager.ActiveScene.GameObjects)
                if (!GO.IsEditor && GO.Parent == null && !GO.ShouldBeDeleted)
                    TreeRecursive(GO, true);

            //Deleting a GameObject
            if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Delete)) && ImGui.IsWindowFocused())
                if (WhoIsSelected != null)
                    WhoIsSelected.ShouldBeDeleted = true;

            //Some Scene window functionalities
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) && ImGui.IsWindowFocused() && ImGui.IsWindowHovered())
            {
                ImGui.OpenPopup("Functionalities");
            }

            if(Input.GetKey(Microsoft.Xna.Framework.Input.Keys.LeftControl) && Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.C) && ImGui.IsWindowFocused() && ImGui.IsWindowHovered())
            {
                if (WhoIsSelected != null)
                    ImGui.SetClipboardText(WhoIsSelected.Name);
            }

            if(ImGui.BeginPopup("Functionalities"))
            {
                if(ImGui.Selectable("New GameObject"))
                {
                    GameObject GO = new GameObject();
                    GO.Name = "Unique Name";
                    GO.AddComponent(new Transform());

                    GO.Start();
                    SceneManager.ActiveScene.AddGameObject(GO);

                    //Should sort game objects?
                }
                //ImGui.Separator();
                //ImGui.Selectable("Noice");

                ImGui.EndPopup();
            }

            ImGui.End();

            ImGui.ShowDemoWindow();
        }

        private void TreeRecursive(GameObject GO, bool Root)
        {
            if (GO.ShouldBeDeleted)
                return;

            int ChildrenCount = GO.Children.Count;

            if(ChildrenCount == 0)
            {
                if (!Root)
                    ImGui.Indent();

                if (!GO.IsActive())
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0.8f, 0.8f, 0.8f, 1));
                }
                ImGui.Selectable(GO.Name, WhoIsSelected == GO);
                if (!GO.IsActive())
                {
                    ImGui.PopStyleColor();
                    ImGui.PopStyleColor();
                }

                if (!Root)
                    ImGui.Unindent();

                if (ImGui.IsItemClicked())
                    WhoIsSelected = GO;

                return;
            }

            if (Root)
                ImGui.Unindent();

            if (!GO.IsActive())
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0.8f, 0.8f, 0.8f, 1));
            }

            bool Open = ImGui.TreeNodeEx(GO.Name, (GO == WhoIsSelected)?ImGuiTreeNodeFlags.Selected:ImGuiTreeNodeFlags.None);

            if (!GO.IsActive())
            {
                ImGui.PopStyleColor();
                ImGui.PopStyleColor();
            }

            if (ImGui.IsItemClicked())
                WhoIsSelected = GO;

            for (int i = 0; i < ChildrenCount; i++)
            {
                if(Open)
                    TreeRecursive(GO.Children[i], false);
            }

            if(Open)
                ImGui.TreePop();

            if(Root)
                ImGui.Indent();
        }
    }
}
