using System.Numerics;
using ImGuiNET;

namespace MyEngine.FN_Editor
{
    class GameObjects_Tab: GameObjectComponent, Editor
    {
        public override void Start()
        {

        }

        public override void DrawUI()
        {
            ImGui.ShowDemoWindow();

            //Scene Tab
            ImGui.Begin(SceneManager.ActiveScene.Name);

            ImGui.Indent();

            foreach (GameObject GO in SceneManager.ActiveScene.GameObjects)
                if (GO.Parent == null)
                    TreeRecursive(GO, true);

            ImGui.End();
        }

        private void TreeRecursive(GameObject GO, bool Root)
        {
            int ChildrenCount = GO.Children.Count;

            if(ChildrenCount == 0)
            {
                ImGui.Selectable(GO.Name);
                return;
            }

            if(Root)
                ImGui.Unindent();

            bool Open = ImGui.TreeNode(GO.Name);

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
