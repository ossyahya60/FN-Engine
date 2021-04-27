using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MyEngine.FN_Editor
{
    public class ContentWindow: GameObjectComponent
    {
        public static object DraggedAsset = null;

        public string GameContentPath = null;

        private Microsoft.Xna.Framework.Graphics.Texture2D T2D = null;
        private IntPtr TexID;
        private DateTime StartingDate = DateTime.Now;
        private bool DirectoryChanged = true;
        private Regex TexRegex = null;
        private Regex MusicRegex = null;
        private Regex ShaderRegex = null;
        private string ContentFolderDirectory = null;
        private List<IntPtr> TexIDs;
        private string AssName = "";

        public override void Start()
        {
            // Allowing Drag and Drop to Engine
            var form = Form.FromHandle(Setup.GameWindow.Handle);
            form.AllowDrop = true;
            form.DragEnter += DrageEnter;
            form.DragDrop += DragDrop;
            /////////////////////////////

            if (GameContentPath == null)
                GameContentPath = Directory.GetCurrentDirectory();
            
            T2D = Setup.Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Temp");
            TexID = Scene.GuiRenderer.BindTexture(T2D); //Don't forget to unbind texture upon deletion
            ContentFolderDirectory = Directory.GetCurrentDirectory();
            TexIDs = new List<IntPtr>();
            TexRegex = new Regex(@"([\.]\b(png|jpg|jpeg)\b)$", RegexOptions.IgnoreCase);
            MusicRegex = new Regex(@"([\.]\b(wav|ogg|wma|mp3)\b)$", RegexOptions.IgnoreCase);
            ShaderRegex = new Regex(@"([\.]\b(fx)\b)$", RegexOptions.IgnoreCase);
        }
        
        private void DrageEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        private void DragDrop(object sender, DragEventArgs e)
        {
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach(string F in filePaths)
            {
                if(File.Exists(F))
                {
                    string[] AssetPath = F.Split('\\');
                    string AssetName = AssetPath[AssetPath.Length - 1];
                    if (TexRegex.IsMatch(AssetName) || MusicRegex.IsMatch(AssetName) || ShaderRegex.IsMatch(AssetName))
                        File.Copy(F, GameContentPath + "\\" + AssetName);
                    else
                        continue;

                    Utility.BuildContentItem(GameContentPath + "\\" + AssetName);
                }
            }
        }

        public override void DrawUI()
        {
            ImGui.Begin("Content Manager");

            //Update ContentWindow when there is a change in the directory
            if (Directory.GetLastWriteTime(GameContentPath).CompareTo(StartingDate) > 0)
            {
                StartingDate = Directory.GetLastWriteTime(GameContentPath);
                DirectoryChanged = true;
            }

            // Back Button
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
                TexIDs.Clear(); //?
                TexIDs.Add(Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\FolderIcon")));
                TexIDs.Add(Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\MusicIcon")));
                TexIDs.Add(Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\ShaderIcon")));
            }

            int Enumerator = 3;

            //Folders
            bool Entered = false;
            int ID_F = 0;
            foreach (string Dir in Directory.GetDirectories(GameContentPath))
            {
                Entered = true;

                ImGui.BeginGroup();
                ImGui.PushID(ID_F++);
                if (ImGui.ImageButton(TexIDs[0], new Vector2(64.0f, 64.0f), Vector2.Zero, Vector2.One, 0, Vector4.Zero))
                {
                    GameContentPath = Dir;
                    DirectoryChanged = true;
                    return;
                }
                ImGui.PopID();
                ImGui.PushTextWrapPos(ImGui.GetItemRectMin().X + 64);
                ImGui.Text(Dir.Remove(0, GameContentPath.Length + 1));
                ImGui.PopTextWrapPos();
                ImGui.EndGroup();

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
                    string[] AssetPath = Files[i].Split('\\');
                    string AssetName = AssetPath[AssetPath.Length - 1];
                    string[] AssetLoadNameComposite = Files[i].Remove(0, ContentFolderDirectory.Length + 1).Split('.');
                    string AssetLoadName = AssetLoadNameComposite[AssetLoadNameComposite.Length - 2];

                    if (TexRegex.IsMatch(AssetName)) //Found a texture
                    {
                        if (DirectoryChanged) //Rebuild content
                            TexIDs.Add(Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>(AssetLoadName)));

                        IntPtr ID = TexIDs[Enumerator++];
                        if (ImGui.ImageButton(ID, new Vector2(64.0f, 64.0f)))
                        {
                            ImGui.OpenPopup("AssetName");
                            AssName = AssetLoadName + "." + AssetLoadNameComposite[AssetLoadNameComposite.Length - 1];
                        }

                        // Dragging an asset
                        if (ImGui.BeginDragDropSource())
                        {
                            ImGui.SetDragDropPayload("Asset", IntPtr.Zero, 0);

                            DraggedAsset = AssetLoadName;

                            ImGui.EndDragDropSource();
                        }

                        ImGui.SameLine();
                    }
                    else if (MusicRegex.IsMatch(AssetName)) //Found a song or a soundeffect
                    {
                        ImGui.PushID(ID_F++);
                        bool EnteredButton = false;
                        if (ImGui.ImageButton(TexIDs[1], new Vector2(64.0f, 64.0f)))
                        {
                            EnteredButton = true;
                            ImGui.PopID();

                            ImGui.OpenPopup("AssetName");
                            AssName = AssetLoadName + "." + AssetLoadNameComposite[AssetLoadNameComposite.Length - 1];
                        }

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
                        if (ImGui.ImageButton(TexIDs[2], new Vector2(64.0f, 64.0f)))
                        {
                            EnteredButton = true;
                            ImGui.PopID();

                            ImGui.OpenPopup("AssetName");
                            AssName = AssetLoadName + "." + AssetLoadNameComposite[AssetLoadNameComposite.Length - 1];
                        }

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
                }
            }
            DirectoryChanged = false;

            if (ImGui.BeginPopup("AssetName"))
            {
                ImGui.Text(AssName);
                ImGui.EndPopup();
            }

            ImGui.EndChild();

            ImGui.End();
        }
    }
}
