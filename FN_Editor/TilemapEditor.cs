using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace FN_Engine.FN_Editor
{
    internal class TilemapEditor: GameObjectComponent
    {
        public static bool IsWindowOpen = false;

        public List<TileSetStruct> TileSets = null;

        private string ChosenTileSet = "";
        private bool EnsureClipDeletion = false;

        public TilemapEditor()
        {
            TileSets = new List<TileSetStruct>();
        }

        internal struct TileSetStruct
        {
            public string Name;
            public Texture2D Tex;
            public IntPtr TexPtr;
            public Vector4[] Rects;
        }

        public override void DrawUI()
        {
            if(IsWindowOpen)
            {
                ImGui.Begin("Tilemap Editor", ImGuiWindowFlags.AlwaysAutoResize);

                if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Escape)))
                    IsWindowOpen = false;

                if(ImGui.BeginCombo("TileSets", ChosenTileSet))
                {
                    foreach(TileSetStruct TS in TileSets)
                    {
                        if(ImGui.Selectable(TS.Name))
                        {
                            ChosenTileSet = TS.Name;
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.Separator();

                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.4f, 0.4f, 1));

                if(ImGui.Button("New Tileset"))
                {

                }

                ImGui.SameLine();

                if(ImGui.Button("Delete Tileset"))
                {
                    ImGui.OpenPopup("Are You Sure?" + "##2");
                    EnsureClipDeletion = true;
                }

                if(ImGui.BeginPopupModal("Are You Sure?" + "##2", ref EnsureClipDeletion, ImGuiWindowFlags.AlwaysAutoResize))
                {
                    if(ImGui.Button("Yes, delete this"))
                    {


                        ImGui.CloseCurrentPopup();
                    }

                    if (ImGui.Button("No"))
                    {


                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }

                ImGui.PopStyleColor();

                ImGui.End();
            }
        }
    }
}
