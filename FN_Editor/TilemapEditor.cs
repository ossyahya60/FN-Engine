using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace FN_Engine.FN_Editor
{
    internal class TileSetClass
    {
        public string Name = "Default Tileset";
        public Texture2D Tex = null;
        internal IntPtr TexPtr = IntPtr.Zero;
        public Vector4[] Rects = null;
    }

    internal class TilemapEditor: GameObjectComponent
    {
        public static bool IsWindowOpen = false;

        public List<TileSetClass> TileSets = null;

        private string ChosenTileSet = "";
        private bool EnsureClipDeletion = false;
        private int ActiveTileSet = -1;
        private string TilesetName = "Drag a Tileset here!";

        public TilemapEditor()
        {
            TileSets = new List<TileSetClass>();
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
                    for (int i = 0; i < TileSets.Count; i++)
                    {
                        if(ImGui.Selectable(TileSets[i].Name))
                        {
                            ChosenTileSet = TileSets[i].Name;
                            ActiveTileSet = i;
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.Separator();

                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.4f, 0.4f, 1));

                if (ImGui.Button("New Tileset"))
                {
                    ActiveTileSet = TileSets.Count;
                    TileSets.Add(new TileSetClass());
                }

                ImGui.SameLine();

                if(ImGui.Button("Delete Tileset") && ActiveTileSet != -1)
                {
                    ImGui.OpenPopup("Are You Sure?" + "##2");
                    EnsureClipDeletion = true;
                }

                ImGui.InputText("Name", ref ChosenTileSet, 50);
                ImGui.InputText("Tileset", ref TilesetName, 50, ImGuiInputTextFlags.ReadOnly);

                if(ImGui.BeginDragDropTarget() && ImGui.IsMouseReleased(ImGuiMouseButton.Left) && ContentWindow.DraggedAsset != null && ActiveTileSet != -1)
                {
                    if (ContentWindow.DraggedAsset is KeyValuePair<string, Vector4>)
                    {
                        KeyValuePair<string, Vector4> KVP = (KeyValuePair<string, Vector4>)ContentWindow.DraggedAsset;
                        TileSetClass tileset = new TileSetClass();
                        tileset.Tex = Setup.Content.Load<Texture2D>(KVP.Key);
                        tileset.Name = "TileSet";
                        tileset.TexPtr = Scene.GuiRenderer.BindTexture(tileset.Tex);

                        int TileSetColumnCount = (int)Math.Round(10000.0f / KVP.Value.Z);
                        int TileSetRowCount = (int)Math.Round(10000.0f / KVP.Value.W);

                        tileset.Rects = new Vector4[TileSetRowCount * TileSetColumnCount];

                        for (int i = 0; i < TileSetRowCount; i++)
                            for (int j = 0; j < TileSetColumnCount; j++)
                                tileset.Rects[i * TileSetColumnCount + j] = new Vector4(j * KVP.Value.Z, i * KVP.Value.W, KVP.Value.Z, KVP.Value.W) / 10000.0f;

                        TileSets[ActiveTileSet] = tileset;

                        ContentWindow.DraggedAsset = null;
                    }
                }

                if (ImGui.BeginPopupModal("Are You Sure?" + "##2", ref EnsureClipDeletion, ImGuiWindowFlags.AlwaysAutoResize))
                {
                    if(ImGui.Button("Yes, delete this"))
                    {
                        TileSets.RemoveAt(ActiveTileSet);
                        ActiveTileSet = -1;

                        ImGui.CloseCurrentPopup();
                    }

                    if (ImGui.Button("No"))
                    {


                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }

                if (ActiveTileSet != -1 && TileSets[ActiveTileSet].Rects != null)
                {
                    ImGui.Separator();

                    TileSets[ActiveTileSet].TexPtr = TileSets[ActiveTileSet].TexPtr == IntPtr.Zero ? Scene.GuiRenderer.BindTexture(TileSets[ActiveTileSet].Tex) : TileSets[ActiveTileSet].TexPtr;

                    int ID = 0;
                    Vector2 Size = new Vector2(TileSets[ActiveTileSet].Rects[0].Z * TileSets[ActiveTileSet].Tex.Width, TileSets[ActiveTileSet].Rects[0].W * TileSets[ActiveTileSet].Tex.Height);
                    for (int j = 0; j < TileSets[ActiveTileSet].Rects.Length; j++)
                    {
                        if (j % 7 == 0 && j != 0)
                            ImGui.NewLine();

                        Vector4 Rect = TileSets[ActiveTileSet].Rects[j];
                        ImGui.PushID(ID++);
                        ImGui.Image(TileSets[ActiveTileSet].TexPtr, new Vector2(Rect.Z * TileSets[ActiveTileSet].Tex.Width, Rect.W * TileSets[ActiveTileSet].Tex.Height), new Vector2(Rect.X, Rect.Y), new Vector2(Rect.Z + Rect.X, Rect.W + Rect.Y), Vector4.One, Vector4.One);
                        ImGui.PopID();

                        ImGui.SameLine();
                    }
                }

                ImGui.PopStyleColor();

                ImGui.End();
            }
        }
    }
}
