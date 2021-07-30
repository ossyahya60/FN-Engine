using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace FN_Engine.FN_Editor
{
    internal class AnimationEditor: GameObjectComponent
    {
        public static bool IsWindowOpen = false;

        private int SelectedClip = -1;
        private IntPtr DragAndDropTex = IntPtr.Zero;
        private int DraggedFrame = -1;
        private float ChangeFrameTimer = 0;
        private int ActiveFrame = 0;
        private string SearchText = "";
        private bool EnsureClipDeletionBool = false;
        private string DragText = "Drag GameObject Here!";
        private List<Animation> AnimationClips;

        public AnimationEditor()
        {
            AnimationClips = new List<Animation>();
        }

        //internal class AnimationInfo
        //{
        //    public string Name = "Default Animation";
        //    public string Tag = "Default Tag";
        //    public List<Frame> Frames = null;
        //    public float Speed = 1;
        //    public bool PlayReverse = false;
        //    public bool FixedTimeBewteenFrames = false;
        //    public bool Loop = false;
        //    public float FixedTimeAmount = 0.5f;
        //}

        //internal class Frame
        //{
        //    public Texture2D Tex = null;
        //    public float Time = 0.25f;
        //    public Microsoft.Xna.Framework.Rectangle SourceRectangle = Microsoft.Xna.Framework.Rectangle.Empty;
        //    internal IntPtr TexPtr;
        //}

        public override void Start()
        {
            DragAndDropTex = Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\DragAndDropIcon"));
        }

        public override void DrawUI()
        {
            if (IsWindowOpen)
            {
                if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Escape)))
                {
                    IsWindowOpen = false;
                    AnimationClips.Clear();
                    SelectedClip = -1;
                    ActiveFrame = 0;
                    DragText = "Drag GameObject Here!";
                }

                //ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, 1));
                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 2);
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.4f, 0.4f, 1));
                ImGui.Begin("Animation Editor", ImGuiWindowFlags.AlwaysAutoResize);

                //GameObject Animator
                ImGui.InputText("GameObject Peeked", ref DragText, 50);

                if(ImGui.BeginDragDropTarget())
                {
                    if (GameObjects_Tab.DraggedGO != null && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                    {
                        var Anim = GameObjects_Tab.DraggedGO.GetComponent<Animator>();

                        if (Anim == null)
                        {
                            AnimationClips.Clear();
                            DragText = "Drag GameObject Here!";
                        }
                        else
                        {
                            DragText = GameObjects_Tab.DraggedGO.Name;
                            AnimationClips = Anim.AnimationClips;

                            SelectedClip = -1;
                            ActiveFrame = 0;
                        }
                    }

                    ImGui.EndDragDropTarget();
                }

                if (ImGui.Button("New Animation") && DragText != "Drag GameObject Here!")
                {
                    string[] ClipNames = new string[AnimationClips.Count];
                    for (int i = 0; i < AnimationClips.Count; i++)
                        ClipNames[i] = AnimationClips[i].Name;

                    AnimationClips.Add(new Animation() { Frames = new List<Animation.Frame>(), Name = Utility.UniqueName("Default Animation", ClipNames) });
                    SelectedClip = AnimationClips.Count - 1;
                    SearchText = AnimationClips[SelectedClip].Name;
                }

                ImGui.SameLine();

                if(ImGui.Button("Delete Clip"))
                {
                    if(SelectedClip != -1)
                    {
                        ImGui.OpenPopup("Are You Sure?");
                        EnsureClipDeletionBool = true;
                    }
                }

                if(ImGui.BeginPopupModal("Are You Sure?", ref EnsureClipDeletionBool, ImGuiWindowFlags.AlwaysAutoResize))
                {
                    if (ImGui.Button("Yes, delete this clip"))
                    {
                        AnimationClips.RemoveAt(SelectedClip);
                        ActiveFrame = 0;
                        SelectedClip = -1;

                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.SameLine();

                    if (ImGui.Button("No"))
                        ImGui.CloseCurrentPopup();

                    ImGui.EndPopup();
                }

                if (ImGui.BeginCombo("Clips", SearchText))
                {
                    if (AnimationClips != null)
                    {
                        for (int i = 0; i < AnimationClips.Count; i++)
                        {
                            if (ImGui.Selectable(AnimationClips[i].Name))
                            {
                                SelectedClip = i;
                                SearchText = AnimationClips[i].Name;
                                ActiveFrame = 0;
                            }
                        }
                    }

                    ImGui.EndCombo();
                }

                //var CursPos = ImGui.GetCursorPos();
                //ImGui.Combo("Selected Clip", ref SelectedClip, ClipsNames, AnimationClips.Count);
                //ImGui.SetCursorPos(CursPos);
                //ImGui.SetItemAllowOverlap();
                //ImGui.InputText("", ref SearchText, 50);


                if (SelectedClip >= 0)
                {
                    //Animation Player
                    var CursPos1 = ImGui.GetCursorPos();
                    ImGui.SameLine();

                    ImGui.BeginChild("Animation Player", new Vector2(128, 128), true, ImGuiWindowFlags.NoScrollbar);
                    if (AnimationClips[SelectedClip].Frames.Count == 0)
                        ImGui.Image(DragAndDropTex, new Vector2(128, 128));
                    else
                    {
                        Animation.Frame F = AnimationClips[SelectedClip].Frames[ActiveFrame];
                        if (ChangeFrameTimer < (AnimationClips[SelectedClip].FixedTimeBetweenFrames? AnimationClips[SelectedClip].FixedTime : F.Time) / AnimationClips[SelectedClip].Speed)
                            ChangeFrameTimer += ImGui.GetIO().DeltaTime;
                        else
                        {
                            if(AnimationClips[SelectedClip].Reverse)
                                ActiveFrame = (ActiveFrame > 0) ? ActiveFrame - 1 : AnimationClips[SelectedClip].Frames.Count - 1;
                            else
                                ActiveFrame = (ActiveFrame < AnimationClips[SelectedClip].Frames.Count - 1) ? ActiveFrame + 1 : 0;

                            ChangeFrameTimer = 0;
                        }

                        ImGui.Image(F.TexPtr, new Vector2(128, 128), new Vector2((float)F.SourceRect.X / F.Tex.Width, (float)F.SourceRect.Y / F.Tex.Height), new Vector2((float)F.SourceRect.Right / F.Tex.Width, (float)F.SourceRect.Bottom / F.Tex.Height));
                    }
                    ImGui.EndChild();

                    ImGui.SetCursorPos(CursPos1);
                    ImGui.InputText("Name", ref AnimationClips[SelectedClip].Name, 50);
                    if(ImGui.IsItemDeactivatedAfterEdit())
                    {
                        string[] ClipNames = new string[AnimationClips.Count];
                        for (int i = 0; i < AnimationClips.Count; i++)
                            ClipNames[i] = AnimationClips[i].Name;

                        AnimationClips[SelectedClip].Name = Utility.UniqueName(AnimationClips[SelectedClip].Name, ClipNames);
                    }

                    ImGui.InputFloat("Speed", ref AnimationClips[SelectedClip].Speed);
                    ImGui.Checkbox("Reversed", ref AnimationClips[SelectedClip].Reverse);
                    ImGui.Checkbox("Loop", ref AnimationClips[SelectedClip].Loop);

                    ImGui.Checkbox("Fixed Time Bewteen Frames", ref AnimationClips[SelectedClip].FixedTimeBetweenFrames);

                    if(AnimationClips[SelectedClip].FixedTimeBetweenFrames)
                        ImGui.InputFloat("Time", ref AnimationClips[SelectedClip].FixedTime);

                    //Frames
                    ImGui.Separator();

                    for (int i = 0; i < AnimationClips[SelectedClip].Frames.Count; i++)
                    {
                        Animation.Frame F = AnimationClips[SelectedClip].Frames[i];
                        if (F.TexPtr == default(IntPtr))
                            F.TexPtr = Scene.GuiRenderer.BindTexture(F.Tex);

                        ImGui.ImageButton(F.TexPtr, new Vector2(64, 64), new Vector2((float)F.SourceRect.X / F.Tex.Width, (float)F.SourceRect.Y / F.Tex.Height), new Vector2((float)F.SourceRect.Right / F.Tex.Width, (float)F.SourceRect.Bottom / F.Tex.Height));

                        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                        {
                            AnimationClips[SelectedClip].Frames.RemoveAt(i--);
                            ActiveFrame = 0;
                        }

                        if(ImGui.BeginDragDropSource())
                        {
                            DraggedFrame = i;
                            ImGui.SetDragDropPayload("Dragged Frame", IntPtr.Zero, 0);

                            ImGui.EndDragDropSource();
                        }

                        if (ImGui.BeginDragDropTarget() && DraggedFrame != -1 && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                        {
                            Animation.Frame Source = AnimationClips[SelectedClip].Frames[DraggedFrame];
                            Animation.Frame Target = AnimationClips[SelectedClip].Frames[i];
                            AnimationClips[SelectedClip].Frames.RemoveAt(DraggedFrame);
                            AnimationClips[SelectedClip].Frames.Insert(DraggedFrame, Target);
                            AnimationClips[SelectedClip].Frames.RemoveAt(i);
                            AnimationClips[SelectedClip].Frames.Insert(i, Source);

                            DraggedFrame = -1;
                            ImGui.EndDragDropTarget();
                        }

                        ImGui.SameLine();
                    }

                    ImGui.ImageButton(DragAndDropTex, new Vector2(64, 64));

                    //Drag and Drop
                    if(ImGui.BeginDragDropTarget() && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                    {
                        if(ContentWindow.DraggedAsset != null)
                        {
                            if(ContentWindow.DraggedAsset is string) //Whole texture
                            {
                                Animation.Frame NewFrame = new Animation.Frame();
                                bool Safe = true;
                                try { NewFrame.Tex = Setup.Content.Load<Texture2D>((string)ContentWindow.DraggedAsset); }
                                catch (Microsoft.Xna.Framework.Content.ContentLoadException) { Safe = false; }

                                if (Safe)
                                {
                                    NewFrame.SourceRect = NewFrame.Tex.Bounds;
                                    NewFrame.Time = 0.25f;
                                    NewFrame.TexPtr = Scene.GuiRenderer.BindTexture(NewFrame.Tex);

                                    AnimationClips[SelectedClip].Frames.Add(NewFrame);
                                    ActiveFrame = 0;
                                }
                            }
                            else if(ContentWindow.DraggedAsset is KeyValuePair<string, Vector4>) //Sliced Texture
                            {
                                Animation.Frame NewFrame = new Animation.Frame();
                                bool Safe = true;
                                try { NewFrame.Tex = Setup.Content.Load<Texture2D>(((KeyValuePair<string, Vector4>)ContentWindow.DraggedAsset).Key); }
                                catch (Microsoft.Xna.Framework.Content.ContentLoadException) { Safe = false; }

                                if (Safe)
                                {
                                    Vector4 SrcRect = ((KeyValuePair<string, Vector4>)ContentWindow.DraggedAsset).Value;
                                    NewFrame.SourceRect = new Microsoft.Xna.Framework.Rectangle((int)Math.Round(SrcRect.X * (NewFrame.Tex.Width / 10000.0f)), (int)Math.Round(SrcRect.Y * (NewFrame.Tex.Height / 10000.0f)), (int)Math.Round(SrcRect.Z * (NewFrame.Tex.Width / 10000.0f)), (int)Math.Round(SrcRect.W * (NewFrame.Tex.Height / 10000.0f)));
                                    NewFrame.Time = 0.25f;
                                    NewFrame.TexPtr = Scene.GuiRenderer.BindTexture(NewFrame.Tex);

                                    AnimationClips[SelectedClip].Frames.Add(NewFrame);
                                    ActiveFrame = 0;
                                }
                            }

                            ContentWindow.DraggedAsset = null;
                        }

                        ImGui.EndDragDropTarget();
                    }
                }

                ImGui.End();
                ImGui.PopStyleColor();
                ImGui.PopStyleVar();
            }
        }
    }
}
