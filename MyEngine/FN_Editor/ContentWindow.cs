using ImGuiNET;

namespace MyEngine.FN_Editor
{
    public class ContentWindow: GameObjectComponent
    {
        public override void Start()
        {
            
        }

        public override void DrawUI()
        {
            ImGui.Begin("Content Manager");



            ImGui.End();
        }
    }
}
