using ImGuiNET;

namespace MyEngine.FN_Editor
{
    class InspectorWindow: GameObjectComponent
    {
        public override void DrawUI()
        {
            ImGui.Begin("Inspector");

            GameObject Selected_GO = FN_Editor.GameObjects_Tab.WhoIsSelected;

            if(Selected_GO != null)
            {
                //Name Of GameObject
                ImGui.Indent((ImGui.GetWindowSize().X - ImGui.CalcTextSize(Selected_GO.Name + " ---- ").X) * 0.5f);
                ImGui.Text("-- " + Selected_GO.Name + " --");
                ImGui.Unindent((ImGui.GetWindowSize().X - ImGui.CalcTextSize(Selected_GO.Name + " ---- ").X) * 0.5f);
                ImGui.Text("\n");

                //Contents Of GameObject
                foreach(GameObjectComponent GOC in Selected_GO.GameObjectComponents)
                {
                    if(ImGui.CollapsingHeader(GOC.ToString().Remove(0, 9), ImGuiTreeNodeFlags.DefaultOpen)) //8 is "MyEngine.", change it if you change the name of the namespace
                    {
                        ImGui.Checkbox("Enabled", ref GOC.Enabled);
                        ImGui.PopID();

                        
                    }
                }
                
            }

            ImGui.End();
        }
    }
}
