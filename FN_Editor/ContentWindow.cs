using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;

namespace FN_Engine.FN_Editor
{
    internal class ContentWindow: GameObjectComponent
    {
        internal class SpriteEditorInfo
        {
            public bool IsSpriteSheet;
            public bool BySize;
            public bool TrimSprite;
            public int[] SizeOrCount;
            public int[] Offset;
            public int[] Spacing;
            internal ImGuiDir IsOpen = ImGuiDir.Right;
        }

        public static object DraggedAsset = null;
        public static Vector2[] MyRegion;
        public static List<IntPtr> TexPtrs;
        public static string SelectedTexture = null;

        public Dictionary<string, SpriteEditorInfo> SPIs = new Dictionary<string, SpriteEditorInfo>();

        private string GameContentPath = null;
        private bool DirectoryChanged = true;
        private Regex TexRegex = null;
        private Regex MusicRegex = null;
        private Regex ShaderRegex = null;
        private Regex SceneRegex = null;
        private string ContentFolderDirectory = null;
        private string AssName = "";
        private readonly HashSet<string> NotLoadedAssets = new HashSet<string>();
        private int SelTexIndex = 0;
        private bool IsSpriteEditorOpen = false;
        private List<Microsoft.Xna.Framework.Rectangle> SlicedTexs = null;
        private Texture2D SelTex = null;

        public override void Start()
        {
            // Allowing Drag and Drop to Engine
            //var form = Control.FromHandle(Setup.GameWindow.Handle); //returns null :(
            //form.AllowDrop = true;
            //form.DragEnter += DrageEnter;
            //form.DragDrop += DragDrop;
            /////////////////////////////
            ///

            TexPtrs = new List<IntPtr>();
            SlicedTexs = new List<Microsoft.Xna.Framework.Rectangle>();

            if (GameContentPath == null)
                GameContentPath = Directory.GetCurrentDirectory();

            ContentFolderDirectory = Directory.GetCurrentDirectory();
            TexRegex = new Regex(@"([\.]\b(png|jpg|jpeg)\b)$", RegexOptions.IgnoreCase);
            MusicRegex = new Regex(@"([\.]\b(wav|ogg|wma|mp3)\b)$", RegexOptions.IgnoreCase);
            ShaderRegex = new Regex(@"([\.]\b(fx)\b)$", RegexOptions.IgnoreCase);
            SceneRegex = new Regex(@"([\.]\b(scene)\b)$", RegexOptions.IgnoreCase);
            MyRegion = new Vector2[2];

            //Utility.BuildAllContent(GameContentPath);
        }

        private void DrageEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        private void DragDrop(object sender, DragEventArgs e)
        {
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string F in filePaths)
            {
                if (File.Exists(F))
                {
                    string[] AssetPath = F.Split('\\');
                    string AssetName = AssetPath[AssetPath.Length - 1];
                    if (TexRegex.IsMatch(AssetName) || MusicRegex.IsMatch(AssetName) || ShaderRegex.IsMatch(AssetName))
                        File.Copy(F, GameContentPath + "\\" + AssetName);
                    else
                        continue;

                    DirectoryChanged = true;
                    Utility.BuildContentItem(GameContentPath + "\\" + AssetName);
                }
            }
        }

        private void PasteFromClipboard()
        {
            var Items = Clipboard.GetFileDropList();

            foreach(string Item in Items)
            {
                if (File.Exists(Item))
                {
                    string[] AssetPath = Item.Split('\\');

                    if (AssetPath.Length - 1 <= 0)
                        return;

                    string AssetName = AssetPath[AssetPath.Length - 1];

                    if (TexRegex.IsMatch(AssetName) || MusicRegex.IsMatch(AssetName) || ShaderRegex.IsMatch(AssetName))
                        File.Copy(Item, GameContentPath + "\\" + AssetName, true);
                    else
                        continue;

                    DirectoryChanged = true;
                    Utility.BuildContentItem(GameContentPath + "\\" + AssetName);

                    NotLoadedAssets.Remove(Item);
                }
            }
        }

        public override void DrawUI()
        {
            ImGui.Begin("Content Manager");

            ///
            if (EditorScene.AutoConfigureWindows)
            {
                if (MyRegion[1].X != 0)
                {
                    float DeltaSize = MyRegion[1].X - ImGui.GetWindowSize().X;

                    if (DeltaSize != 0)
                    {
                        ImGui.SetWindowSize("Inspector", InspectorWindow.MyRegion[1] + new Vector2(DeltaSize, 0));
                        ImGui.SetWindowPos("Inspector", InspectorWindow.MyRegion[0] - new Vector2(DeltaSize, 0));
                    }
                }

                if (MyRegion[1].Y != 0)
                {
                    float DeltaSize = MyRegion[1].Y - ImGui.GetWindowSize().Y;

                    if (DeltaSize != 0)
                        ImGui.SetWindowSize(SceneManager.ActiveScene.Name, GameObjects_Tab.MyRegion[1] + new Vector2(0, DeltaSize));
                }
            }

            MyRegion[0] = ImGui.GetWindowPos();
            MyRegion[1] = ImGui.GetWindowSize();
            ///

            //Update ContentWindow when there is a change in the directory
            //if (Directory.GetLastWriteTime(GameContentPath).CompareTo(StartingDate) > 0)
            //{
            //    StartingDate = Directory.GetLastWriteTime(GameContentPath);
            //    DirectoryChanged = true;
            //}

            // Back Button
            //ImGui.PushStyleColor(ImGuiCol.Border, 1);
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 0.1f));
            if (ImGui.ArrowButton("Back", ImGuiDir.Left))
            {
                if (!GameContentPath.Equals(Directory.GetCurrentDirectory()))
                {
                    string[] GCP = GameContentPath.Split('\\');
                    GameContentPath = GameContentPath.Remove(GameContentPath.Length - GCP[GCP.Length - 1].Length - 1, GCP[GCP.Length - 1].Length + 1);
                    DirectoryChanged = true;
                }
            }
            ImGui.SameLine(0, 15);

            //New Folder
            if (ImGui.Button("New Folder"))
                ImGui.OpenPopup("NewFolder");

            ImGui.SameLine(0, 15);

            if (ImGui.Button("Paste Clipboard"))
                PasteFromClipboard();

            ImGui.PopStyleColor();

            if (ImGui.GetIO().KeyCtrl && ImGui.IsWindowFocused())
                if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.V))) //Copy
                    PasteFromClipboard();

            if (ImGui.BeginPopup("NewFolder"))
            {
                string NameBuffer = "New Folder";
                ImGui.Text("Enter Name: ");
                ImGui.InputText("Folder Name", ref NameBuffer, 50, ImGuiInputTextFlags.AutoSelectAll);
                if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Enter)))
                {
                    if (NameBuffer.Replace(" ", "").Length > 0)
                        if (!Directory.Exists(GameContentPath + "\\" + NameBuffer))
                            Directory.CreateDirectory(GameContentPath + "\\" + NameBuffer);

                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

            if (DirectoryChanged)
            {
                TexPtrs.Clear(); //?
                TexPtrs.Add(Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\FolderIcon")));
                TexPtrs.Add(Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\MusicIcon")));
                TexPtrs.Add(Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\ShaderIcon")));
                TexPtrs.Add(Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\Logo")));
            }

            int Enumerator = 4;

            ImGui.BeginChild("Assets");

            //Folders
            bool Entered = false;
            int ID_F = 0;
            foreach (string Dir in Directory.GetDirectories(GameContentPath))
            {
                Entered = true;

                ImGui.BeginGroup();
                ImGui.PushID(ID_F++);
                ImGui.PushStyleColor(ImGuiCol.Button, 0);
                if (ImGui.ImageButton(TexPtrs[0], new Vector2(64.0f, 64.0f), Vector2.Zero, Vector2.One, 0, Vector4.Zero))
                {
                    GameContentPath = Dir;
                    DirectoryChanged = true;
                    return;
                }
                ImGui.PopStyleColor();
                ImGui.PopID();
                ImGui.PushTextWrapPos(ImGui.GetItemRectMin().X + 64);
                ImGui.Text(Dir.Remove(0, GameContentPath.Length + 1));
                ImGui.PopTextWrapPos();
                ImGui.EndGroup();

                if(ImGui.GetItemRectMax().X < ImGui.GetWindowContentRegionMax().X)
                    ImGui.SameLine();
            }

            if (Entered)
            {
                ImGui.NewLine();
                ImGui.Separator();
                ImGui.NewLine();
            }

            //Assets
            ImGui.BeginChild("Files", new Vector2(ImGui.GetWindowSize().X * 0.98f, 100), false, ImGuiWindowFlags.HorizontalScrollbar);

            string[] Files = Directory.GetFiles(GameContentPath);

            Files = Directory.GetFiles(GameContentPath);
            ID_F = 0;

            if (Files != null)
            {
                for (int i = 0; i < Files.Length; i++)
                {
                    if (NotLoadedAssets.Contains(Files[i]))
                        continue;

                    string[] AssetPath = Files[i].Split('\\');
                    string AssetName = AssetPath[AssetPath.Length - 1];

                    //string[] AssetLoadNameComposite = Files[i].Remove(0, ContentFolderDirectory.Length + 1).Split('.');
                    //string AssetLoadName = AssetLoadNameComposite[AssetLoadNameComposite.Length - 2];

                    string AssPath = Files[i].Remove(0, ContentFolderDirectory.Length + 1);
                    int idx = AssPath.LastIndexOf('.');
                    string AssetLoadName = AssPath.Substring(0, idx);

                    if (TexRegex.IsMatch(AssetName)) //Found a texture
                    {
                        try
                        {
                            if (DirectoryChanged) //Rebuild content
                                TexPtrs.Add(Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>(AssetLoadName)));
                        }
                        catch(Microsoft.Xna.Framework.Content.ContentLoadException)
                        {
                            NotLoadedAssets.Add(Files[i]);
                            continue;
                        }

                        IntPtr ID = TexPtrs[Enumerator++];
                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 0.1f));
                        if (ImGui.ImageButton(ID, new Vector2(64.0f, 64.0f)))
                        {
                            //ImGui.OpenPopup("AssetName");
                            AssName = AssetLoadName + "." + AssPath.Substring(idx + 1);
                        }
                        ImGui.PopStyleColor();

                        HelpMarker(AssetPath[AssetPath.Length - 1], false);

                        //if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                        //    ImGui.OpenPopup("Rename File");

                        // Dragging an asset
                        if (ImGui.BeginDragDropSource())
                        {
                            ImGui.SetDragDropPayload("Asset", IntPtr.Zero, 0);

                            DraggedAsset = AssetLoadName;

                            ImGui.EndDragDropSource();
                        }

                        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                        {
                            ImGui.OpenPopup("Sprite Editor Props");
                            SelectedTexture = AssetLoadName;
                            SelTexIndex = Enumerator - 1;
                            IsSpriteEditorOpen = true;
                        }

                        ImGui.SameLine();

                        if (SPIs.ContainsKey(AssetLoadName))
                        {
                            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 0.1f));
                            if (ImGui.ArrowButton("ThisTex" + i.ToString(), SPIs[AssetLoadName].IsOpen))
                            {
                                SelectedTexture = AssetLoadName;
                                SelTexIndex = Enumerator - 1;
                                if (SPIs[AssetLoadName].IsOpen == ImGuiDir.Left)
                                    SPIs[AssetLoadName].IsOpen = ImGuiDir.Right;
                                else
                                {
                                    SPIs[AssetLoadName].IsOpen = ImGuiDir.Left;

                                    if (SPIs[AssetLoadName].IsSpriteSheet)
                                    {
                                        SlicedTexs.Clear();
                                        Texture2D Tex = Setup.Content.Load<Texture2D>(SelectedTexture);
                                        SpriteEditorInfo SPI = SPIs[SelectedTexture];
                                        SelTex = Tex;

                                        if (SPI.BySize)
                                        {
                                            SPI.SizeOrCount[0] = (int)Math.Clamp(SPI.SizeOrCount[0], Tex.Width / 30.0f, Tex.Width);
                                            SPI.SizeOrCount[1] = (int)Math.Clamp(SPI.SizeOrCount[1], Tex.Height / 30.0f, Tex.Height);

                                            for (int i2 = 0; i2 < Tex.Height; i2 += SPI.SizeOrCount[1])
                                            {
                                                for (int j = 0; j < Tex.Width; j += SPI.SizeOrCount[0])
                                                {
                                                    Vector2 Spacing = new Vector2((j != 0) ? Math.Abs(SPI.Spacing[0]) : 0, (i2 != 0) ? Math.Abs(SPI.Spacing[1]) : 0);
                                                    Vector2 Offset = new Vector2(SPI.Offset[0], SPI.Offset[1]);
                                                    Vector2 UV0 = new Vector2(j, i2) / new Vector2(Tex.Width, Tex.Height) + new Vector2((float)(Offset.X + Spacing.X) / Tex.Width, (float)(Offset.Y + Spacing.Y) / Tex.Height);
                                                    Vector2 UV1 = new Vector2(j + SPI.SizeOrCount[0], i2 + SPI.SizeOrCount[1]) / new Vector2(Tex.Width, Tex.Height) + new Vector2((float)(Offset.X + Spacing.X) / Tex.Width, (float)(Offset.Y + Spacing.Y) / Tex.Height);

                                                    Microsoft.Xna.Framework.Rectangle Rect = new Microsoft.Xna.Framework.Rectangle((int)Math.Clamp((UV0.X * Tex.Width), 0, Tex.Width), (int)Math.Clamp((UV0.Y * Tex.Height), 0, Tex.Height), (int)(Math.Abs(UV1.X - UV0.X) * Tex.Width), (int)(Math.Abs(UV1.Y - UV0.Y) * Tex.Height));
                                                    Rect.Width = Math.Clamp(Rect.Width, 0, Tex.Width - Rect.X - 1);
                                                    Rect.Height = Math.Clamp(Rect.Height, 0, Tex.Height - Rect.Y - 1);
                                                    Microsoft.Xna.Framework.Color[] buffer = new Microsoft.Xna.Framework.Color[Rect.Width * Rect.Height];

                                                    Tex.GetData(0, Rect, buffer, 0, Rect.Width * Rect.Height);

                                                    Vector2 Min = new Vector2(int.MaxValue, int.MaxValue), Max = new Vector2(int.MinValue, int.MinValue);

                                                    if (SPI.TrimSprite)
                                                    {
                                                        bool Empty = true;

                                                        for (int i4 = 0; i4 < Rect.Height; i4++)
                                                        {
                                                            for (int j2 = 0; j2 < Rect.Width; j2++)
                                                            {
                                                                if (buffer[i4 * Rect.Width + j2].A != 0)
                                                                {
                                                                    Empty = false;

                                                                    if (j2 < Min.X)
                                                                        Min.X = j2;

                                                                    if (i4 < Min.Y)
                                                                        Min.Y = i4;

                                                                    if (j2 > Max.X)
                                                                        Max.X = j2;

                                                                    if (i4 > Max.Y)
                                                                        Max.Y = i4;
                                                                }
                                                            }
                                                        }

                                                        if (!Empty)
                                                            SlicedTexs.Add(new Microsoft.Xna.Framework.Rectangle((int)((UV0.X + Min.X/Tex.Width) * 10000), (int)((UV0.Y + Min.Y/Tex.Height) * 10000), (int)(Math.Abs((Max.X - Min.X + 1)/Rect.Width) * Math.Abs(UV1.X - UV0.X) * 10000), (int)(Math.Abs((Max.Y - Min.Y + 1)/Rect.Height) * Math.Abs(UV1.Y - UV0.Y) * 10000)));
                                                    }
                                                    else if(!buffer.All(Item => Item.A == 0))
                                                        SlicedTexs.Add(new Microsoft.Xna.Framework.Rectangle((int)(UV0.X * 10000), (int)(UV0.Y * 10000), (int)(Math.Abs(UV1.X - UV0.X) * 10000), (int)(Math.Abs(UV1.Y - UV0.Y) * 10000)));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            SPI.SizeOrCount[0] = Math.Clamp(SPI.SizeOrCount[0], 1, 32);
                                            SPI.SizeOrCount[1] = Math.Clamp(SPI.SizeOrCount[1], 1, 32);
                                            Vector2 ImageSize = new Vector2(1.0f / SPI.SizeOrCount[1], 1.0f / SPI.SizeOrCount[0]);

                                            for (int i2 = 0; i2 < SPI.SizeOrCount[0]; i2++)
                                            {
                                                for (int j = 0; j < SPI.SizeOrCount[1]; j++)
                                                {
                                                    Vector2 Spacing = new Vector2((j != 0) ? Math.Abs(SPI.Spacing[0]) : 0, (i2 != 0) ? Math.Abs(SPI.Spacing[1]) : 0);
                                                    Vector2 Offset = new Vector2(SPI.Offset[0], SPI.Offset[1]);
                                                    Vector2 UV0 = new Vector2(j * ImageSize.X, i2 * ImageSize.Y) + new Vector2((float)(Offset.X + Spacing.X) / Tex.Width, (float)(Offset.Y + Spacing.Y) / Tex.Height);
                                                    Vector2 UV1 = new Vector2((j + 1) * ImageSize.X, (i2 + 1) * ImageSize.Y) + new Vector2((float)(Offset.X + Spacing.X) / Tex.Width, (float)(Offset.Y + Spacing.Y) / Tex.Height);

                                                    //Eliminating empty textures
                                                    Microsoft.Xna.Framework.Rectangle Rect = new Microsoft.Xna.Framework.Rectangle((int)Math.Clamp((UV0.X * Tex.Width), 0, Tex.Width), (int)Math.Clamp((UV0.Y * Tex.Height), 0, Tex.Height), (int)(Math.Abs(UV1.X - UV0.X) * Tex.Width), (int)(Math.Abs(UV1.Y - UV0.Y) * Tex.Height));
                                                    Rect.Width = Math.Clamp(Rect.Width, 0, Tex.Width - Rect.X - 1);
                                                    Rect.Height = Math.Clamp(Rect.Height, 0, Tex.Height - Rect.Y - 1);
                                                    Microsoft.Xna.Framework.Color[] buffer = new Microsoft.Xna.Framework.Color[Rect.Width * Rect.Height];

                                                    Tex.GetData(0, Rect, buffer, 0, Rect.Width * Rect.Height);

                                                    Vector2 Min = new Vector2(int.MaxValue, int.MaxValue), Max = new Vector2(int.MinValue, int.MinValue);

                                                    if (SPI.TrimSprite)
                                                    {
                                                        bool Empty = true;

                                                        for (int i4 = 0; i4 < Rect.Height; i4++)
                                                        {
                                                            for (int j2 = 0; j2 < Rect.Width; j2++)
                                                            {
                                                                if (buffer[i4 * Rect.Width + j2].A != 0)
                                                                {
                                                                    Empty = false;

                                                                    if (j2 < Min.X)
                                                                        Min.X = j2;

                                                                    if (i4 < Min.Y)
                                                                        Min.Y = i4;

                                                                    if (j2 > Max.X)
                                                                        Max.X = j2;

                                                                    if (i4 > Max.Y)
                                                                        Max.Y = i4;
                                                                }
                                                            }
                                                        }

                                                        if (!Empty)
                                                            SlicedTexs.Add(new Microsoft.Xna.Framework.Rectangle((int)((UV0.X + Min.X / Tex.Width) * 10000), (int)((UV0.Y + Min.Y / Tex.Height) * 10000), (int)(Math.Abs((Max.X - Min.X + 1) / Rect.Width) * Math.Abs(UV1.X - UV0.X) * 10000), (int)(Math.Abs((Max.Y - Min.Y + 1) / Rect.Height) * Math.Abs(UV1.Y - UV0.Y) * 10000)));
                                                    }
                                                    else if (!buffer.All(Item => Item.A == 0))
                                                        SlicedTexs.Add(new Microsoft.Xna.Framework.Rectangle((int)(UV0.X * 10000), (int)(UV0.Y * 10000), (int)(Math.Abs(UV1.X - UV0.X) * 10000), (int)(Math.Abs(UV1.Y - UV0.Y) * 10000)));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            ImGui.PopStyleColor();

                            if (SPIs[AssetLoadName].IsOpen == ImGuiDir.Left) //Texture pack is open
                            {
                                ImGui.SameLine();
                                ImGui.BeginChild(AssetLoadName, Vector2.Zero, true, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.AlwaysHorizontalScrollbar);

                                Microsoft.Xna.Framework.Rectangle Rect = SlicedTexs[0];

                                for (int i3=0; i3<SlicedTexs.Count; i3++)
                                {
                                    bool EnteredButton2 = false;
                                    ImGui.PushID(i3.ToString() + "Noice");
                                    ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 0.1f));
                                    if (ImGui.ImageButton(TexPtrs[SelTexIndex], new Vector2(64 * ((float)SlicedTexs[i3].Width / SlicedTexs[i3].Height), 64), new Vector2(SlicedTexs[i3].X / 10000.0f, SlicedTexs[i3].Y / 10000.0f), new Vector2(SlicedTexs[i3].Right / 10000.0f, SlicedTexs[i3].Bottom / 10000.0f)))
                                    {
                                        ImGui.PopID();
                                        EnteredButton2 = true;
                                    }
                                    ImGui.PopStyleColor();

                                    // Dragging an asset
                                    if (ImGui.BeginDragDropSource())
                                    {
                                        ImGui.SetDragDropPayload("Asset2", IntPtr.Zero, 0);
                                        DraggedAsset = new KeyValuePair<string, Vector4>(AssetLoadName, new Vector4(SlicedTexs[i3].X, SlicedTexs[i3].Y, SlicedTexs[i3].Width, SlicedTexs[i3].Height));

                                        ImGui.EndDragDropSource();
                                    }

                                    if (!EnteredButton2)
                                        ImGui.PopID();

                                    if (i3 != SlicedTexs.Count - 1)
                                        ImGui.SameLine();
                                }

                                ImGui.EndChild();
                            }
                        }

                        ImGui.SameLine();
                    }
                    else if (MusicRegex.IsMatch(AssetName)) //Found a song or a soundeffect
                    {
                        ImGui.PushID(ID_F++);
                        bool EnteredButton = false;

                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 0.1f));
                        if (ImGui.ImageButton(TexPtrs[1], new Vector2(64.0f, 64.0f)))
                        {
                            EnteredButton = true;
                            ImGui.PopID();

                            AssName = AssetLoadName + "." + AssPath.Substring(idx + 1);
                        }
                        ImGui.PopStyleColor();

                        HelpMarker(AssetPath[AssetPath.Length - 1], false);

                        // Dragging an asset
                        if (ImGui.BeginDragDropSource())
                        {
                            ImGui.SetDragDropPayload("Asset", IntPtr.Zero, 0);

                            DraggedAsset = AssetLoadName;

                            ImGui.EndDragDropSource();
                        }

                        if (!EnteredButton) //OpenPopup and BeginPopup have to be on the same ID stack level
                            ImGui.PopID();

                        ImGui.SameLine();
                    }
                    else if(ShaderRegex.IsMatch(AssetName)) // Found a shader
                    {
                        ImGui.PushID(ID_F++);
                        bool EnteredButton = false;

                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 0.1f));
                        if (ImGui.ImageButton(TexPtrs[2], new Vector2(64.0f, 64.0f)))
                        {
                            EnteredButton = true;
                            ImGui.PopID();

                            AssName = AssetLoadName + "." + AssPath.Substring(idx + 1);
                        }
                        ImGui.PopStyleColor();

                        HelpMarker(AssetPath[AssetPath.Length - 1], false);

                        // Dragging an asset
                        if (ImGui.BeginDragDropSource())
                        {
                            ImGui.SetDragDropPayload("Asset", IntPtr.Zero, 0);

                            DraggedAsset = AssetLoadName;

                            ImGui.EndDragDropSource();
                        }

                        if (!EnteredButton) //OpenPopup and BeginPopup have to be on the same ID stack level
                            ImGui.PopID();

                        if (ImGui.GetItemRectMax().X < ImGui.GetWindowContentRegionMax().X)
                            ImGui.SameLine();
                    }
                    else if (SceneRegex.IsMatch(AssetName)) // Found a shader
                    {
                        if (AssetLoadName.Contains("_Editor"))
                            continue;

                        ImGui.PushID(ID_F++);
                        bool EnteredButton = false;

                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 0.1f));
                        if (ImGui.ImageButton(TexPtrs[3], new Vector2(64.0f, 64.0f)))
                        {
                            EnteredButton = true;
                            ImGui.PopID();
                            
                            AssName = AssetLoadName + "." + AssPath.Substring(idx + 1);
                        }

                        ImGui.PopStyleColor();

                        if (ImGui.IsItemHovered())
                        {
                            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                            {
                                SceneManager.SerializeScene(SceneManager.ActiveScene.Name);
                                SceneManager.LoadScene_Serialization(AssetLoadName);
                            }
                        }

                        HelpMarker(AssetPath[AssetPath.Length - 1], false);

                        // Dragging an asset
                        if (ImGui.BeginDragDropSource())
                        {
                            ImGui.SetDragDropPayload("Asset", IntPtr.Zero, 0);

                            DraggedAsset = AssetLoadName;

                            ImGui.EndDragDropSource();
                        }

                        if (!EnteredButton) //OpenPopup and BeginPopup have to be on the same ID stack level
                            ImGui.PopID();

                        if (ImGui.GetItemRectMax().X < ImGui.GetWindowContentRegionMax().X)
                            ImGui.SameLine();
                    }
                }
            }
            DirectoryChanged = false;

            //Sprite editor
            ImGui.PushStyleColor(ImGuiCol.PopupBg, new Vector4(0.25f, 0.25f, 0.25f, 1));
            if (ImGui.BeginPopupModal("Sprite Editor Props", ref IsSpriteEditorOpen, ImGuiWindowFlags.AlwaysAutoResize))
            {
                string SelectedTex = SelectedTexture;

                if (SelectedTex == null)
                    return;

                if (!SPIs.ContainsKey(SelectedTex)) //First time edit
                    SPIs.Add(SelectedTex, new SpriteEditorInfo() { SizeOrCount = new int[2], Offset = new int[2], Spacing = new int[2] });

                SpriteEditorInfo SPI = SPIs[SelectedTex];

                ImGui.Checkbox("Spritesheet?", ref SPI.IsSpriteSheet);

                if (SPI.IsSpriteSheet)
                {
                    Texture2D Tex = Setup.Content.Load<Texture2D>(SelectedTexture);

                    ImGui.Checkbox("Trim Sprite", ref SPI.TrimSprite);
                    ImGui.Checkbox("Slice By Size", ref SPI.BySize);
                    ImGui.Text("Texture Dimensions: " + new Vector2(Tex.Width, Tex.Height).ToString());
                    ImGui.InputInt2("Size/Count", ref SPI.SizeOrCount[0]);
                    ImGui.InputInt2("Offset", ref SPI.Offset[0]);
                    ImGui.InputInt2("Spacing", ref SPI.Spacing[0]);

                    if(SPI.BySize)
                    {
                        SPI.SizeOrCount[0] = (int)Math.Clamp(SPI.SizeOrCount[0], Tex.Width / 30.0f, Tex.Width);
                        SPI.SizeOrCount[1] = (int)Math.Clamp(SPI.SizeOrCount[1], Tex.Height / 30.0f, Tex.Height);
                        Vector2 ImageSize = new Vector2((float)SPI.SizeOrCount[0] / Tex.Width, (float)SPI.SizeOrCount[1] / Tex.Height);

                        for (int i = 0; i < Tex.Height; i += SPI.SizeOrCount[1])
                        {
                            for (int j = 0; j < Tex.Width; j += SPI.SizeOrCount[0])
                            {
                                Vector2 Spacing = new Vector2((j != 0) ? Math.Abs(SPI.Spacing[0]) : 0, (i != 0) ? Math.Abs(SPI.Spacing[1]) : 0);
                                Vector2 Offset = new Vector2(SPI.Offset[0], SPI.Offset[1]);
                                ImGui.Image(TexPtrs[SelTexIndex], ImageSize * 512, new Vector2(j, i) / new Vector2(Tex.Width, Tex.Height) + new Vector2((float)(Offset.X + Spacing.X) / Tex.Width, (float)(Offset.Y + Spacing.Y) / Tex.Height), new Vector2(j + SPI.SizeOrCount[0], i + SPI.SizeOrCount[1]) / new Vector2(Tex.Width, Tex.Height) + new Vector2((float)(Offset.X + Spacing.X) / Tex.Width, (float)(Offset.Y + Spacing.Y) / Tex.Height), Vector4.One, Vector4.One);

                                if (j < Tex.Width - SPI.SizeOrCount[0])
                                    ImGui.SameLine();
                            }
                        }
                    }
                    else
                    {
                        SPI.SizeOrCount[0] = Math.Clamp(SPI.SizeOrCount[0], 1, 32);
                        SPI.SizeOrCount[1] = Math.Clamp(SPI.SizeOrCount[1], 1, 32);
                        Vector2 ImageSize = new Vector2(1.0f / SPI.SizeOrCount[1], 1.0f / SPI.SizeOrCount[0]);

                        for (int i=0; i<SPI.SizeOrCount[0]; i++)
                        {
                            for (int j=0; j<SPI.SizeOrCount[1]; j++)
                            {
                                Vector2 Spacing = new Vector2((j != 0) ? Math.Abs(SPI.Spacing[0]) : 0, (i != 0) ? Math.Abs(SPI.Spacing[1]) : 0);
                                Vector2 Offset = new Vector2(SPI.Offset[0], SPI.Offset[1]);
                                ImGui.Image(TexPtrs[SelTexIndex], ImageSize * 512, new Vector2(j * ImageSize.X, i * ImageSize.Y) + new Vector2((float)(Offset.X + Spacing.X) / Tex.Width, (float)(Offset.Y + Spacing.Y) / Tex.Height), new Vector2((j + 1) * ImageSize.X, (i + 1) * ImageSize.Y) + new Vector2((float)(Offset.X + Spacing.X) / Tex.Width, (float)(Offset.Y + Spacing.Y) / Tex.Height), Vector4.One, Vector4.One);

                                if(j != SPI.SizeOrCount[1] - 1)
                                    ImGui.SameLine();
                            }
                        }
                    }
                }

                if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Escape)))
                {
                    ImGui.CloseCurrentPopup();
                    IsSpriteEditorOpen = false;
                }

                ImGui.EndPopup();
            }
            ImGui.PopStyleColor();
            
            ////////////////////////

            if (ImGui.BeginPopup("AssetName"))
            {
                ImGui.Text(AssName);
                ImGui.EndPopup();
            }

            //if (ImGui.BeginPopup("Rename File"))
            //{
            //    ImGui.Text("Edit name:");
            //    ImGui.InputText("##editF", ref NameBuffer, 50, ImGuiInputTextFlags.AutoSelectAll);
            //    if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Enter)))
            //    {
            //        if (NameBuffer.Replace(" ", "").Length > 0 && NameBuffer != WhoIsSelected.Name)
            //        {
            //            KeyValuePair<GameObject, string> BufferedObject = new KeyValuePair<GameObject, string>(WhoIsSelected, WhoIsSelected.Name);
            //            AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(BufferedObject, Operation.Rename));
            //            Redo_Buffer.Clear();

            //            NameBuffer = Utility.UniqueGameObjectName(NameBuffer);
            //            WhoIsSelected.Name = NameBuffer;
            //        }
            //        ImGui.CloseCurrentPopup();
            //    }

            //    ImGui.EndPopup();
            //}

            ImGui.EndChild();

            ImGui.End();
        }

        private static void HelpMarker(string desc, bool DisplayMarker = true)
        {   
            if(DisplayMarker)
                ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(ImGui.GetFontSize()* 35.0f);
                ImGui.TextUnformatted(desc);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }
    }
}
