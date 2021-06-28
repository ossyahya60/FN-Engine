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
            if (ImGui.IsMouseDragging(ImGuiMouseButton.Right))
                Setup.Camera.Move(Input.MouseDelta(), 1);

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
