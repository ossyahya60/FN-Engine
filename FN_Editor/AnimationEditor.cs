using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace FN_Engine.FN_Editor
{
    internal class AnimationEditor : GameObjectComponent
    {
        public static bool IsWindowOpen = false;

        public List<AnimationInfo> AnimationClips;

        private int SelectedClip = -1;
        private IntPtr DragAndDropTex = IntPtr.Zero;
        private int DraggedFrame = -1;
        private float ChangeFrameTimer = 0;
        private int ActiveFrame = 0;
        private string SearchText = "";
        private bool EnsureClipDeletionBool = false;
        private string DraggedGO = "Drag GameObject Here";
        private List<Animation> DraggedGO_AnimClips = null;
        private int DraggedClip = -1;

        public AnimationEditor()
        {
            AnimationClips = new List<AnimationInfo>();
        }

        internal class AnimationInfo
        {
            public string Name = "Default Animation";
            public string Tag = "Default Tag";
            public List<Frame> Frames = null;
            public float Speed = 1;
            public bool PlayReverse = false;
            public bool FixedTimeBewteenFrames = true;
            public bool Loop = false;
            public float FixedTimeAmount = 0.5f;
        }

        public override void Start()
        {
            DragAndDropTex = Scene.GuiRenderer.BindTexture(Setup.Content.Load<Texture2D>("Icons\\DragAndDropIcon"));
        }

        public override void DrawUI()
        {
            if (IsWindowOpen)
            {
                if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Escape)))
                    IsWindowOpen = false;

                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 2);
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.4f, 0.4f, 1));
                ImGui.Begin("Animation Editor", ImGuiWindowFlags.AlwaysAutoResize);

                if (ImGui.Button("New Animation"))
                {
                    string[] ClipNames = new string[AnimationClips.Count];
                    for (int i = 0; i < AnimationClips.Count; i++)
                        ClipNames[i] = AnimationClips[i].Name;

                    AnimationClips.Add(new AnimationInfo() { Frames = new List<Frame>(), Name = Utility.UniqueName("Default Animation", ClipNames) });
                    SelectedClip = AnimationClips.Count - 1;
                    SearchText = AnimationClips[SelectedClip].Name;
                }

                ImGui.SameLine();

                if (ImGui.Button("Delete Clip"))
                {
                    if (SelectedClip != -1)
                    {
                        ImGui.OpenPopup("Are You Sure?");
                        EnsureClipDeletionBool = true;
                    }
                }

                if (ImGui.BeginPopupModal("Are You Sure?", ref EnsureClipDeletionBool, ImGuiWindowFlags.AlwaysAutoResize))
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

                ImGui.SameLine();

                ImGui.InputText("", ref DraggedGO, 50, ImGuiInputTextFlags.ReadOnly);

                if (ImGui.BeginDragDropTarget() && GameObjects_Tab.DraggedGO != null && ImGui.IsMouseReleased(ImGuiMouseButton.Left) && GameObjects_Tab.DraggedGO.GetComponent<Animator>() != null)
                {
                    DraggedGO = GameObjects_Tab.DraggedGO.Name;
                    DraggedGO_AnimClips = GameObjects_Tab.DraggedGO.GetComponent<Animator>().AnimationClips;
                    GameObjects_Tab.DraggedGO = null;
                }

                bool AnimatorTree = ImGui.TreeNode("Animator");

                if (ImGui.BeginDragDropTarget() && DraggedClip != -1 && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                {
                    bool SafeToAdd = true;
                    foreach (Animation AI in DraggedGO_AnimClips)
                    {
                        if (AI.Name == AnimationClips[DraggedClip].Name)
                        {
                            SafeToAdd = false;
                            break;
                        }
                    }

                    if (SafeToAdd)
                        DraggedGO_AnimClips.Add(CopyAnimation(AnimationClips[DraggedClip]));

                    DraggedClip = -1;
                    ImGui.EndDragDropTarget();
                }

                if (AnimatorTree)
                {
                    if (DraggedGO_AnimClips != null)
                    {
                        for (int i = 0; i < DraggedGO_AnimClips.Count; i++)
                        {
                            ImGui.Selectable(DraggedGO_AnimClips[i].Name);

                            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                                DraggedGO_AnimClips.RemoveAt(i--);

                            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsItemHovered())
                            {
                                for (int j = 0; j < AnimationClips.Count; j++)
                                {
                                    if (AnimationClips[j].Name == DraggedGO_AnimClips[i].Name)
                                    {
                                        SelectedClip = j;
                                        AnimationClips[j].Loop = DraggedGO_AnimClips[i].Loop;
                                        AnimationClips[j].FixedTimeAmount = DraggedGO_AnimClips[i].FixedTime;
                                        AnimationClips[j].FixedTimeBewteenFrames = DraggedGO_AnimClips[i].FixedTimeBetweenFrames;
                                        AnimationClips[j].PlayReverse = DraggedGO_AnimClips[i].Reverse;
                                        AnimationClips[j].Speed = DraggedGO_AnimClips[i].Speed;

                                        ActiveFrame = 0;
                                        SearchText = DraggedGO_AnimClips[i].Name;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    ImGui.TreePop();
                }

                ImGui.Separator();

                if (ImGui.BeginCombo("Clips", SearchText))
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
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 90);

                    ImGui.BeginChild("Animation Player", new Vector2(128, 128), false, ImGuiWindowFlags.NoScrollbar);

                    if (ImGui.BeginDragDropSource())
                    {
                        DraggedClip = SelectedClip;
                        ImGui.SetDragDropPayload("Animation Clip", IntPtr.Zero, 0);

                        ImGui.EndDragDropSource();
                    }

                    if (AnimationClips[SelectedClip].Frames.Count == 0)
                        ImGui.Image(DragAndDropTex, new Vector2(128, 128));
                    else
                    {
                        Frame F = AnimationClips[SelectedClip].Frames[ActiveFrame];
                        if (ChangeFrameTimer < (AnimationClips[SelectedClip].FixedTimeBewteenFrames ? AnimationClips[SelectedClip].FixedTimeAmount : F.Time) / AnimationClips[SelectedClip].Speed)
                            ChangeFrameTimer += ImGui.GetIO().DeltaTime;
                        else
                        {
                            if (AnimationClips[SelectedClip].PlayReverse)
                                ActiveFrame = (ActiveFrame > 0) ? ActiveFrame - 1 : AnimationClips[SelectedClip].Frames.Count - 1;
                            else
                                ActiveFrame = (ActiveFrame < AnimationClips[SelectedClip].Frames.Count - 1) ? ActiveFrame + 1 : 0;

                            ChangeFrameTimer = 0;
                        }

                        ImGui.Image(F.TexPtr, new Vector2(128, 128), new Vector2((float)F.SourceRectangle.X / F.Tex.Width, (float)F.SourceRectangle.Y / F.Tex.Height), new Vector2((float)F.SourceRectangle.Right / F.Tex.Width, (float)F.SourceRectangle.Bottom / F.Tex.Height));
                    }

                    ImGui.EndChild();

                    ImGui.SetCursorPos(CursPos1);
                    ImGui.InputText("Name", ref AnimationClips[SelectedClip].Name, 50);
                    if (ImGui.IsItemDeactivatedAfterEdit())
                    {
                        string[] ClipNames = new string[AnimationClips.Count];
                        for (int i = 0; i < AnimationClips.Count; i++)
                            ClipNames[i] = AnimationClips[i].Name;

                        AnimationClips[SelectedClip].Name = Utility.UniqueName(AnimationClips[SelectedClip].Name, ClipNames);
                    }

                    ImGui.InputText("Tag", ref AnimationClips[SelectedClip].Tag, 50);
                    ImGui.InputFloat("Speed", ref AnimationClips[SelectedClip].Speed);
                    ImGui.Checkbox("Reversed", ref AnimationClips[SelectedClip].PlayReverse);
                    ImGui.Checkbox("Loop", ref AnimationClips[SelectedClip].Loop);

                    ImGui.Checkbox("Fixed Time Bewteen Frames", ref AnimationClips[SelectedClip].FixedTimeBewteenFrames);

                    if (AnimationClips[SelectedClip].FixedTimeBewteenFrames)
                        ImGui.InputFloat("Time", ref AnimationClips[SelectedClip].FixedTimeAmount);

                    //Frames
                    ImGui.Separator();

                    for (int i = 0; i < AnimationClips[SelectedClip].Frames.Count; i++)
                    {
                        Frame F = AnimationClips[SelectedClip].Frames[i];
                        if (F.TexPtr == default(IntPtr))
                            F.TexPtr = Scene.GuiRenderer.BindTexture(F.Tex);

                        if (i % 5 == 0 && i != 0)
                            ImGui.NewLine();

                        ImGui.BeginGroup();
                        ImGui.ImageButton(F.TexPtr, new Vector2(64, 64), new Vector2((float)F.SourceRectangle.X / F.Tex.Width, (float)F.SourceRectangle.Y / F.Tex.Height), new Vector2((float)F.SourceRectangle.Right / F.Tex.Width, (float)F.SourceRectangle.Bottom / F.Tex.Height));

                        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                        {
                            AnimationClips[SelectedClip].Frames.RemoveAt(i--);
                            ActiveFrame = 0;
                        }

                        if (ImGui.BeginDragDropSource())
                        {
                            DraggedFrame = i;
                            ImGui.SetDragDropPayload("Dragged Frame", IntPtr.Zero, 0);

                            ImGui.EndDragDropSource();
                        }

                        if (ImGui.BeginDragDropTarget() && DraggedFrame != -1 && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                        {
                            Frame Source = AnimationClips[SelectedClip].Frames[DraggedFrame];
                            Frame Target = AnimationClips[SelectedClip].Frames[i];
                            AnimationClips[SelectedClip].Frames.RemoveAt(DraggedFrame);
                            AnimationClips[SelectedClip].Frames.Insert(DraggedFrame, Target);
                            AnimationClips[SelectedClip].Frames.RemoveAt(i);
                            AnimationClips[SelectedClip].Frames.Insert(i, Source);

                            DraggedFrame = -1;
                            ImGui.EndDragDropTarget();
                        }

                        if (!AnimationClips[SelectedClip].FixedTimeBewteenFrames)
                        {
                            ImGui.PushItemWidth(64);
                            ImGui.PushID(i);
                            ImGui.SliderFloat("", ref F.Time, 0, 10);
                            ImGui.PopID();
                            ImGui.PopItemWidth();
                        }

                        ImGui.EndGroup();

                        ImGui.SameLine();
                    }

                    ImGui.ImageButton(DragAndDropTex, new Vector2(64, 64));

                    //Drag and Drop
                    if (ImGui.BeginDragDropTarget() && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                    {
                        if (ContentWindow.DraggedAsset != null)
                        {
                            if (ContentWindow.DraggedAsset is string) //Whole texture
                            {
                                Frame NewFrame = new Frame();
                                bool Safe = true;
                                try { NewFrame.Tex = Setup.Content.Load<Texture2D>((string)ContentWindow.DraggedAsset); }
                                catch (Microsoft.Xna.Framework.Content.ContentLoadException) { Safe = false; }

                                if (Safe)
                                {
                                    NewFrame.SourceRectangle = NewFrame.Tex.Bounds;
                                    NewFrame.Time = 0.25f;
                                    NewFrame.TexPtr = Scene.GuiRenderer.BindTexture(NewFrame.Tex);

                                    AnimationClips[SelectedClip].Frames.Add(NewFrame);
                                    ActiveFrame = 0;
                                }
                            }
                            else if (ContentWindow.DraggedAsset is KeyValuePair<string, Vector4>) //Sliced Texture
                            {
                                Frame NewFrame = new Frame();
                                bool Safe = true;
                                try { NewFrame.Tex = Setup.Content.Load<Texture2D>(((KeyValuePair<string, Vector4>)ContentWindow.DraggedAsset).Key); }
                                catch (Microsoft.Xna.Framework.Content.ContentLoadException) { Safe = false; }

                                if (Safe)
                                {
                                    Vector4 SrcRect = ((KeyValuePair<string, Vector4>)ContentWindow.DraggedAsset).Value;
                                    NewFrame.SourceRectangle = new Microsoft.Xna.Framework.Rectangle((int)Math.Round(SrcRect.X * (NewFrame.Tex.Width / 10000.0f)), (int)Math.Round(SrcRect.Y * (NewFrame.Tex.Height / 10000.0f)), (int)Math.Round(SrcRect.Z * (NewFrame.Tex.Width / 10000.0f)), (int)Math.Round(SrcRect.W * (NewFrame.Tex.Height / 10000.0f)));
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

        private Animation CopyAnimation(AnimationInfo AI)
        {
            Animation AM = new Animation();
            AM.Name = AI.Name;
            AM.FixedTime = AI.FixedTimeAmount;
            AM.FixedTimeBetweenFrames = AI.FixedTimeBewteenFrames;
            AM.Loop = AI.Loop;
            AM.Speed = AI.Speed;
            AM.SR = SceneManager.ActiveScene.FindGameObjectWithName(DraggedGO).GetComponent<SpriteRenderer>();
            AM.Reverse = AI.PlayReverse;
            AM.Frames = new List<Frame>(AI.Frames.Count);

            for (int i = 0; i < AI.Frames.Count; i++)
                AM.Frames.Add(new Frame() { Tex = AI.Frames[i].Tex, SourceRectangle = AI.Frames[i].SourceRectangle, Time = AI.Frames[i].Time });

            return AM;
        }
    }
}