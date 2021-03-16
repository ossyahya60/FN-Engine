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
            ImGui.ShowDemoWindow();

            //Scene Tab
            ImGui.Begin(SceneManager.ActiveScene.Name);

            if (ImGui.IsMouseClicked(0) && ImGui.IsWindowHovered(ImGuiHoveredFlags.RootWindow))
                WhoIsSelected = null;

            ImGui.Indent();

            foreach (GameObject GO in SceneManager.ActiveScene.GameObjects)
                if (!GO.IsEditor && GO.Parent == null)
                    TreeRecursive(GO, true);

            ImGui.End();
        }

        private void TreeRecursive(GameObject GO, bool Root)
        {
            int ChildrenCount = GO.Children.Count;

            if(ChildrenCount == 0)
            {
                ImGui.Selectable(GO.Name, WhoIsSelected == GO);

                if (ImGui.IsItemClicked())
                    WhoIsSelected = GO;

                return;
            }

            if(Root)
                ImGui.Unindent();

            bool Open = ImGui.TreeNodeEx(GO.Name, (GO == WhoIsSelected)?ImGuiTreeNodeFlags.Selected:ImGuiTreeNodeFlags.None);

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
