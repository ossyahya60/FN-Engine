using ImGuiNET;
using Microsoft.Xna.Framework;

namespace MyEngine.FN_Editor
{
    public class EditorScene: GameObjectComponent
    {
        public override void Update(GameTime gameTime)
        {
            if (ImGui.IsMouseDragging(ImGuiMouseButton.Right))
                Setup.Camera.Move(Input.MouseDelta(), 1);
        }
    }
}
