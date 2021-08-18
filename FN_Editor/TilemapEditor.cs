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
        private enum Tools { None, Brush, Bucket, Rectangle, Select, Delete, CustomBrush}

        public static bool IsWindowOpen = false;

        public List<TileSetClass> TileSets = null;

        private string ChosenTileSet = "";
        private bool EnsureClipDeletion = false;
        private int ActiveTileSet = -1;
        private string TilesetName = "Drag a Tileset here!";
        private int SelectedTile = -1;
        private Microsoft.Xna.Framework.Rectangle MultiSelectRect;
        private bool VisualizeMultiSelect = false;
        private bool WindowHoveredLastClick = false;
        private bool ShowUtils = false;
        private Vector2 LastWindPos;
        private IntPtr[] TexPtrs;
        private GameObject ActiveTilemap = null;
        private Tools ActiveTool = Tools.None;
        private List<GameObject> HandyList, HandyList2;

        public TilemapEditor()
        {
            TileSets = new List<TileSetClass>();
        }

        public override void Start()
        {
            TexPtrs = new IntPtr[4];

            TexPtrs[0] = Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\Brush"));
            HandyList = new List<GameObject>();
            HandyList2 = new List<GameObject>();
        }

        public override void DrawUI()
        {
            if(IsWindowOpen)
            {
                ImGui.Begin("Tilemap Editor", ImGuiWindowFlags.AlwaysAutoResize);

                if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                    ActiveTool = Tools.Delete;
                else if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.B))
                    ActiveTool = Tools.Brush;
                else if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.G))
                    ActiveTool = Tools.Bucket;
                else if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                    ActiveTool = Tools.Select;
                else if (ImGui.IsMouseClicked(ImGuiMouseButton.Middle))
                    ActiveTool = Tools.None;

                Vector2 ThisWindowPos = ImGui.GetWindowPos();
                Vector2 ThisWindowSize = ImGui.GetWindowSize();

                MultiSelectRect.X += (int)(ThisWindowPos.X - LastWindPos.X);
                MultiSelectRect.Y += (int)(ThisWindowPos.Y - LastWindPos.Y);
                LastWindPos = ThisWindowPos;

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

                ImGui.SameLine();
                ImGui.Checkbox("Show Utils", ref ShowUtils);

                ImGui.Separator();

                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.4f, 0.4f, 1));

                if (ImGui.Button("New Tileset"))
                {
                    ActiveTileSet = TileSets.Count;

                    string[] TilesetNames = new string[TileSets.Count];
                    for (int i = 0; i < TilesetNames.Length; i++)
                        TilesetNames[i] = TileSets[i].Name;

                    ChosenTileSet = Utility.UniqueName(ChosenTileSet, TilesetNames);
                    TileSets.Add(new TileSetClass() { Name = ChosenTileSet});
                }

                ImGui.SameLine();

                if(ImGui.Button("Delete Tileset") && ActiveTileSet != -1)
                {
                    ImGui.OpenPopup("Are You Sure?" + "##2");
                    EnsureClipDeletion = true;
                }

                string activeTilemap = ActiveTilemap != null ? ActiveTilemap.Name : "Drag a tilemap gameobject here!";
                ImGui.InputText("Active Tilemap", ref activeTilemap, 50, ImGuiInputTextFlags.ReadOnly);

                if(ImGui.BeginDragDropTarget() && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                {
                    if (GameObjects_Tab.DraggedGO != null && GameObjects_Tab.DraggedGO.GetComponent<Tilemap>() != null)
                        ActiveTilemap = GameObjects_Tab.DraggedGO;

                    ImGui.EndDragDropTarget();
                }

                if (ActiveTilemap != null && (ActiveTilemap.ShouldBeDeleted || ActiveTilemap.ShouldBeRemoved))
                    ActiveTilemap = null;

                if (ActiveTileSet != -1)
                {
                    ImGui.InputText("Name", ref ChosenTileSet, 50);

                    if (ImGui.IsItemDeactivatedAfterEdit())
                    {
                        string[] TilesetNames = new string[TileSets.Count];
                        for (int i = 0; i < TilesetNames.Length; i++)
                            TilesetNames[i] = TileSets[i].Name;

                        ChosenTileSet = TileSets[ActiveTileSet].Name.Equals(ChosenTileSet)? ChosenTileSet : Utility.UniqueName(ChosenTileSet, TilesetNames);
                        TileSets[ActiveTileSet].Name = ChosenTileSet;
                    }

                    ImGui.InputText("Tileset", ref TilesetName, 50, ImGuiInputTextFlags.ReadOnly);

                    if (ImGui.BeginDragDropTarget() && ImGui.IsMouseReleased(ImGuiMouseButton.Left) && ContentWindow.DraggedAsset != null && ActiveTileSet != -1)
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
                            TilesetName = tileset.Name;

                            ContentWindow.DraggedAsset = null;
                        }

                        ImGui.EndDragDropTarget();
                    }

                    float UpperGroupCursPos = ImGui.GetCursorPosY() + ImGui.GetWindowPos().Y;

                    ImGui.BeginChild("SpriteSheet", new Vector2(ImGui.GetWindowSize().X * 0.95f, 64 * 5.1f), false, ImGuiWindowFlags.AlwaysHorizontalScrollbar);
                    if (ActiveTileSet != -1 && TileSets[ActiveTileSet].Rects != null)
                    {
                        ImGui.Separator();

                        TileSets[ActiveTileSet].TexPtr = TileSets[ActiveTileSet].TexPtr == IntPtr.Zero ? Scene.GuiRenderer.BindTexture(TileSets[ActiveTileSet].Tex) : TileSets[ActiveTileSet].TexPtr;

                        //MultiSelect
                        Vector2 MousePos = ImGui.GetMousePos();
                        Microsoft.Xna.Framework.Point ScrollPos = new Microsoft.Xna.Framework.Point((int)ImGui.GetScrollX(), (int)ImGui.GetScrollY());
                        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                        {
                            WindowHoveredLastClick = ImGui.IsWindowHovered() && MousePos.Y > UpperGroupCursPos;

                            if (WindowHoveredLastClick)
                            {
                                VisualizeMultiSelect = false;
                                MultiSelectRect = new Microsoft.Xna.Framework.Rectangle((int)MousePos.X + ScrollPos.X, (int)MousePos.Y + ScrollPos.Y, 0, 0);
                            }
                        }

                        if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && WindowHoveredLastClick)
                        {
                            VisualizeMultiSelect = true;
                            MultiSelectRect = new Microsoft.Xna.Framework.Rectangle(MultiSelectRect.X, MultiSelectRect.Y, (int)MousePos.X - MultiSelectRect.X + ScrollPos.X, (int)MousePos.Y - MultiSelectRect.Y + ScrollPos.Y);

                            Microsoft.Xna.Framework.Rectangle AdjustedRect = new Microsoft.Xna.Framework.Rectangle(Math.Min(MultiSelectRect.X, MultiSelectRect.X + MultiSelectRect.Width), Math.Min(MultiSelectRect.Y, MultiSelectRect.Y + MultiSelectRect.Height), Math.Abs(MultiSelectRect.Width), Math.Abs(MultiSelectRect.Height));
                            MultiSelectRect = AdjustedRect;
                        }

                        int ID = 0;
                        Vector2 WindPos = ImGui.GetWindowPos();
                        Vector2 Min = new Vector2(int.MaxValue, int.MaxValue);
                        Vector2 Max = new Vector2(int.MinValue, int.MinValue);
                        Vector2 Size = new Vector2(TileSets[ActiveTileSet].Rects[0].Z * TileSets[ActiveTileSet].Tex.Width, TileSets[ActiveTileSet].Rects[0].W * TileSets[ActiveTileSet].Tex.Height);

                        Vector2 InnserSpacingOld = ImGui.GetStyle().ItemSpacing;

                        ImGui.GetStyle().ItemSpacing = Vector2.Zero;
                        Vector4 SelectedRect = Vector4.Zero;

                        for (int j = 0; j < TileSets[ActiveTileSet].Rects.Length; j++)
                        {
                            Vector2 CursPos = ImGui.GetCursorPos();
                            Vector4 Rect = TileSets[ActiveTileSet].Rects[j];
                            Vector2 RealSize = new Vector2(64, Size.X / Size.Y * 64);

                            bool MultiSelected = VisualizeMultiSelect && MultiSelectRect.Intersects(new Microsoft.Xna.Framework.Rectangle((int)(CursPos.X + WindPos.X), (int)(CursPos.Y + WindPos.Y), (int)RealSize.X, (int)RealSize.Y));

                            if (MultiSelected)
                            {
                                if (CursPos.X < Min.X)
                                {
                                    SelectedRect.X = Rect.X;
                                    Min.X = CursPos.X;
                                }
                                if (CursPos.Y < Min.Y)
                                {
                                    SelectedRect.Y = Rect.Y;
                                    Min.Y = CursPos.Y;
                                }
                                if (CursPos.X + RealSize.X > Max.X)
                                {
                                    SelectedRect.Z = Rect.X + Rect.Z;
                                    Max.X = CursPos.X + RealSize.X;
                                }
                                if (CursPos.Y + RealSize.Y > Max.Y)
                                {
                                    SelectedRect.W = Rect.Y + Rect.W;
                                    Max.Y = CursPos.Y + RealSize.Y;
                                }
                            }

                            ImGui.PushID(ID++);
                            ImGui.Image(TileSets[ActiveTileSet].TexPtr, RealSize, new Vector2(Rect.X, Rect.Y), new Vector2(Rect.Z + Rect.X, Rect.W + Rect.Y), (SelectedTile == j || MultiSelected) ? new Vector4(1f, 1f, 1f, 0.5f) : Vector4.One);
                            ImGui.PopID();

                            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                                SelectedTile = j;

                            if (ImGui.IsItemHovered())
                                HitBoxDebuger.DrawNonFilledRectangle_Effect(new Microsoft.Xna.Framework.Rectangle((int)(CursPos.X + WindPos.X - ImGui.GetScrollX()), (int)(CursPos.Y + WindPos.Y - ImGui.GetScrollY()), (int)RealSize.X, (int)RealSize.Y));

                            ImGui.SameLine();

                            if ((j + 1) * Size.X % TileSets[ActiveTileSet].Tex.Width == 0)
                                ImGui.NewLine();
                        }

                        ImGui.GetStyle().ItemSpacing = InnserSpacingOld;

                        ImGui.EndChild();

                        if (ImGui.BeginPopupModal("Are You Sure?" + "##2", ref EnsureClipDeletion, ImGuiWindowFlags.AlwaysAutoResize))
                        {
                            if (ImGui.Button("Yes, delete this"))
                            {
                                TileSets.RemoveAt(ActiveTileSet);
                                ActiveTileSet = -1;
                                ChosenTileSet = "";

                                ImGui.CloseCurrentPopup();
                            }

                            if (ImGui.Button("No"))
                                ImGui.CloseCurrentPopup();

                            ImGui.EndPopup();
                        }

                        if(ActiveTilemap != null) //Edit mode
                        {
                            if (GizmosVisualizer.IsThisWindowHovered && !ImGui.IsAnyItemHovered())
                            {
                                Tilemap tilemap = ActiveTilemap.GetComponent<Tilemap>();
                                
                                if (tilemap != null && Min.X != int.MaxValue)
                                {
                                    var Bias = Setup.Camera.Position - ResolutionIndependentRenderer.GetVirtualRes() * 0.5f - new Microsoft.Xna.Framework.Vector2(GameObjects_Tab.MyRegion[1].X + GameObjects_Tab.MyRegion[0].X, 0);
                                    var GridSize = tilemap.TileSize.ToVector2() * tilemap.GridSize.ToVector2() * tilemap.gameObject.Transform.Scale;
                                    var MapRect = new Microsoft.Xna.Framework.Rectangle((tilemap.gameObject.Transform.Position - GridSize * 0.5f).ToPoint(), GridSize.ToPoint());

                                    bool InsideBoundaries = MapRect.Contains(Bias + Input.GetMousePosition());

                                    if (InsideBoundaries)
                                    {
                                        switch (ActiveTool)
                                        {
                                            case Tools.Brush:
                                                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                                {
                                                    HandyList.Clear();
                                                    HandyList2.Clear();
                                                }
                                                else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                                                {
                                                    //Undo action
                                                    if(HandyList2.Count != 0)
                                                        GameObjects_Tab.AddToACircularBuffer(GameObjects_Tab.Undo_Buffer, new KeyValuePair<object, Operation>(HandyList2.ToArray(), Operation.Delete));
                                                    if (HandyList.Count != 0)
                                                        GameObjects_Tab.AddToACircularBuffer(GameObjects_Tab.Undo_Buffer, new KeyValuePair<object, Operation>(HandyList.ToArray(), Operation.Create));
                                                    GameObjects_Tab.Redo_Buffer.Clear();
                                                }
                                                else if (!ImGui.IsMouseDown(ImGuiMouseButton.Left))
                                                    break;

                                                Microsoft.Xna.Framework.Point tileSize = (tilemap.TileSize.ToVector2() * tilemap.gameObject.Transform.Scale).ToPoint();

                                                GameObject NewTile = new GameObject();
                                                NewTile.Name = Utility.UniqueGameObjectName("Tile");
                                                NewTile.Layer = ActiveTilemap.Layer;
                                                NewTile.AddComponent(new Transform());
                                                NewTile.AddComponent(new SpriteRenderer());
                                                NewTile.Start();

                                                NewTile.GetComponent<SpriteRenderer>().Sprite.Texture = TileSets[ActiveTileSet].Tex;
                                                NewTile.GetComponent<SpriteRenderer>().SourceRectangle = new Microsoft.Xna.Framework.Rectangle((int)Math.Round(SelectedRect.X * TileSets[ActiveTileSet].Tex.Width), (int)Math.Round(SelectedRect.Y * TileSets[ActiveTileSet].Tex.Height), (int)Math.Round((SelectedRect.Z - SelectedRect.X) * TileSets[ActiveTileSet].Tex.Width), (int)Math.Round((SelectedRect.W - SelectedRect.Y) * TileSets[ActiveTileSet].Tex.Height));
                                                var RealTileSize = NewTile.GetComponent<SpriteRenderer>().SourceRectangle.Size.ToVector2() * tilemap.gameObject.Transform.Scale;
                                                var NewPos = (Input.GetMousePosition() + Bias - MapRect.Location.ToVector2()) / MapRect.Size.ToVector2() / (RealTileSize / MapRect.Size.ToVector2());
                                                NewPos *= RealTileSize / tileSize.ToVector2();
                                                NewPos.X = (int)NewPos.X;
                                                NewPos.Y = (int)NewPos.Y;

                                                NewTile.Transform.Scale = tilemap.gameObject.Transform.Scale;
                                                NewTile.Transform.Position = MapRect.Location.ToVector2() + tileSize.ToVector2() * NewPos + RealTileSize * 0.5f * Microsoft.Xna.Framework.Vector2.One;

                                                bool ShouldBreak = false;
                                                foreach (GameObject Child in ActiveTilemap.Children)
                                                {
                                                    if (Utility.Vector2Int(Child.Transform.Position) == NewTile.Transform.Position)
                                                    {
                                                        SpriteRenderer DeletedOBJ_SR = Child.GetComponent<SpriteRenderer>();
                                                        SpriteRenderer NewOBJ_SR = NewTile.GetComponent<SpriteRenderer>();

                                                        if (DeletedOBJ_SR.TextureName == NewOBJ_SR.TextureName && DeletedOBJ_SR.SourceRectangle == NewOBJ_SR.SourceRectangle)
                                                        {
                                                            ShouldBreak = true;
                                                            break;
                                                        }
                                                        else
                                                            HandyList2.Add(Child);

                                                        Child.ShouldBeRemoved = true;

                                                        break;
                                                    }
                                                }

                                                if (ShouldBreak)
                                                    break;

                                                HandyList.Add(NewTile);

                                                SceneManager.ActiveScene.AddGameObject_Recursive(NewTile);
                                                ActiveTilemap.AddChild(NewTile);

                                                break;
                                            case Tools.Delete:

                                                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                                    HandyList2.Clear();
                                                else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                                                {
                                                    if (HandyList2.Count != 0)
                                                    {
                                                        GameObjects_Tab.AddToACircularBuffer(GameObjects_Tab.Undo_Buffer, new KeyValuePair<object, Operation>(HandyList2.ToArray(), Operation.Delete));
                                                        GameObjects_Tab.Redo_Buffer.Clear();
                                                    }
                                                }

                                                if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
                                                {
                                                    tileSize = (tilemap.TileSize.ToVector2() * tilemap.gameObject.Transform.Scale).ToPoint();

                                                    foreach (GameObject Child in ActiveTilemap.Children)
                                                    {
                                                        if (new Microsoft.Xna.Framework.Rectangle(Child.Transform.Position.ToPoint() - (tileSize.ToVector2() * 0.5f).ToPoint(), tileSize).Contains(Input.GetMousePosition() + Bias))
                                                        {
                                                            Child.ShouldBeRemoved = true;
                                                            HandyList2.Add(Child);
                                                            break;
                                                        }
                                                    }
                                                }

                                                break;
                                            case Tools.Bucket: //remember to take another look at the logic (Solved!)

                                                if(ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                                                {
                                                    HandyList.Clear();
                                                    HandyList2.Clear();

                                                    foreach (GameObject Child in ActiveTilemap.Children)
                                                    {
                                                        Child.ShouldBeRemoved = true;
                                                        HandyList2.Add(Child);
                                                    }

                                                    tileSize = (tilemap.TileSize.ToVector2() * tilemap.gameObject.Transform.Scale).ToPoint();
                                                    var SrcRect = new Microsoft.Xna.Framework.Rectangle((int)Math.Round(SelectedRect.X * TileSets[ActiveTileSet].Tex.Width), (int)Math.Round(SelectedRect.Y * TileSets[ActiveTileSet].Tex.Height), (int)Math.Round((SelectedRect.Z - SelectedRect.X) * TileSets[ActiveTileSet].Tex.Width), (int)Math.Round((SelectedRect.W - SelectedRect.Y) * TileSets[ActiveTileSet].Tex.Height));
                                                    RealTileSize = SrcRect.Size.ToVector2() * tilemap.gameObject.Transform.Scale;
                                                    var Ratio = new Microsoft.Xna.Framework.Point((int)Math.Round(RealTileSize.X / tileSize.X), (int)Math.Round(RealTileSize.Y / tileSize.Y));

                                                    for (int i = 0; i < tilemap.GridSize.X / Ratio.Y; i++)
                                                    {
                                                        for (int j = 0; j < tilemap.GridSize.Y / Ratio.X; j++)
                                                        {
                                                            NewTile = new GameObject();
                                                            NewTile.Name = Utility.UniqueGameObjectName("Tile");
                                                            NewTile.Layer = ActiveTilemap.Layer;
                                                            NewTile.AddComponent(new Transform());
                                                            NewTile.AddComponent(new SpriteRenderer());
                                                            NewTile.Start();

                                                            NewTile.GetComponent<SpriteRenderer>().Sprite.Texture = TileSets[ActiveTileSet].Tex;
                                                            NewTile.GetComponent<SpriteRenderer>().SourceRectangle = SrcRect;
                                                            NewPos = new Microsoft.Xna.Framework.Vector2(j * RealTileSize.X, i * RealTileSize.Y);
                                                            NewPos.X = (int)NewPos.X;
                                                            NewPos.Y = (int)NewPos.Y;

                                                            NewTile.Transform.Scale = tilemap.gameObject.Transform.Scale;
                                                            NewTile.Transform.Position = MapRect.Location.ToVector2() + NewPos + RealTileSize * 0.5f;

                                                            SceneManager.ActiveScene.AddGameObject_Recursive(NewTile);
                                                            ActiveTilemap.AddChild(NewTile);

                                                            HandyList.Add(NewTile);
                                                        }
                                                    }

                                                    //Undo action
                                                    if (HandyList2.Count != 0)
                                                        GameObjects_Tab.AddToACircularBuffer(GameObjects_Tab.Undo_Buffer, new KeyValuePair<object, Operation>(HandyList2.ToArray(), Operation.Delete));
                                                    if (HandyList.Count != 0)
                                                        GameObjects_Tab.AddToACircularBuffer(GameObjects_Tab.Undo_Buffer, new KeyValuePair<object, Operation>(HandyList.ToArray(), Operation.Create));
                                                    GameObjects_Tab.Redo_Buffer.Clear();
                                                }

                                                break;
                                        }
                                    }
                                }
                                else if(tilemap == null)
                                    ActiveTilemap = null;
                            }
                        }
                    }
                }

                ImGui.PopStyleColor();

                ImGui.End();

                if (ShowUtils)
                {
                    ImGui.Begin("Utils", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove);

                    ImGui.SetWindowPos(new Vector2(ThisWindowPos.X + ThisWindowSize.X, ThisWindowPos.Y));

                    ImGui.PushStyleColor(ImGuiCol.Button, 0);

                    if(ImGui.ImageButton(TexPtrs[0], new Vector2(32, 32))) //Brush
                        ActiveTool = Tools.Brush;

                    ImGui.PopStyleColor();

                    ImGui.End();
                }
            }
        }
    }
}
