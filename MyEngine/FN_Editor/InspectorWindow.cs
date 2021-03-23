using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Numerics;

namespace MyEngine.FN_Editor
{
    class InspectorWindow: GameObjectComponent
    {
        public static List<List<FieldInfo>> Members;

        private IntPtr intPointer;
        private IntPtr intPointerL;
        private IntPtr intPointerU32;
        private bool ColorClicked = false;
        private FieldInfo ActiveFI = null;
        private GameObjectComponent ActiveGOC = null;

        public override void Start()
        {
            Members = new List<List<FieldInfo>>();
            intPointer = Marshal.AllocHGlobal(sizeof(short));
            intPointerL = Marshal.AllocHGlobal(sizeof(long));
            intPointerU32 = Marshal.AllocHGlobal(sizeof(uint));
        }

        public override void DrawUI()
        {
            ImGui.Begin("Inspector");

            GameObject Selected_GO = FN_Editor.GameObjects_Tab.WhoIsSelected;

            if(Selected_GO != null)
            {
                //Name Of GameObject
                ImGui.Indent((ImGui.GetWindowSize().X - ImGui.CalcTextSize(Selected_GO.Name + " ---- ").X) * 0.5f);
                ImGui.Text("-- " + Selected_GO.Name + " --");
                ImGui.Unindent((ImGui.GetWindowSize().X - ImGui.CalcTextSize(Selected_GO.Name + " ---- ").X) * 0.5f);
                ImGui.Text("\n");

                //Contents Of GameObject
                //foreach(GameObjectComponent GOC in Selected_GO.GameObjectComponents)
                //{
                //    if(ImGui.CollapsingHeader(GOC.ToString().Remove(0, 9), ImGuiTreeNodeFlags.DefaultOpen)) //8 is "MyEngine.", change it if you change the name of the namespace
                //    {
                //        ImGui.Checkbox("Enabled", ref GOC.Enabled);
                //        ImGui.PopID();
                //    }
                //}

                FieldInfo[] FIS_GO = Selected_GO.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo FI in FIS_GO)
                {
                    switch (FI.FieldType.FullName) //Here, I handle basic types
                    {
                        case "System.Single": //float
                            float T = (float)FI.GetValue(Selected_GO);
                            ImGui.DragFloat(FI.Name, ref T);
                            FI.SetValue(Selected_GO, T);
                            ImGui.PopID();
                            break;
                        case "System.Double": //double
                            double D = (double)FI.GetValue(Selected_GO);
                            ImGui.InputDouble(FI.Name, ref D);
                            FI.SetValue(Selected_GO, D);
                            ImGui.PopID();
                            break;
                        case "System.Int32": //int 32
                            int I = (int)FI.GetValue(Selected_GO);
                            ImGui.InputInt(FI.Name, ref I);
                            FI.SetValue(Selected_GO, I);
                            ImGui.PopID();
                            break;
                        case "System.Int16": //int 16
                            short S = (short)FI.GetValue(Selected_GO);
                            Marshal.WriteInt16(intPointer, S);
                            ImGui.InputScalar(FI.Name, ImGuiDataType.S16, intPointer);
                            FI.SetValue(Selected_GO, Marshal.ReadInt16(intPointer));
                            ImGui.PopID();
                            break;
                        case "System.Int64": //int 64
                            long L = (long)FI.GetValue(Selected_GO);
                            Marshal.WriteInt64(intPointerL, L);
                            ImGui.InputScalar(FI.Name, ImGuiDataType.S64, intPointerL);
                            FI.SetValue(Selected_GO, Marshal.ReadInt64(intPointerL));
                            ImGui.PopID();
                            break;
                        case "System.UInt32": //uint 32
                            uint U32 = (uint)FI.GetValue(Selected_GO);
                            Marshal.WriteInt32(intPointerU32, (int)U32);
                            ImGui.InputScalar(FI.Name, ImGuiDataType.U32, intPointerU32);
                            FI.SetValue(Selected_GO, Marshal.ReadInt32(intPointerU32));
                            ImGui.PopID();
                            break;
                        //case "System.UInt16": //uint 16
                        //    FI.SetValue(this, ushort.Parse(Line[2]));
                        //    fieldInfos.Add(FI);
                        //    break;
                        //case "System.UInt64": //uint 64
                        //    FI.SetValue(this, ulong.Parse(Line[2]));
                        //    fieldInfos.Add(FI);
                        //    break;
                        case "System.Boolean": //bool
                            bool B = (bool)FI.GetValue(Selected_GO);
                            ImGui.Checkbox(FI.Name, ref B);
                            FI.SetValue(Selected_GO, B);
                            ImGui.PopID();
                            break;
                        case "Microsoft.Xna.Framework.Vector2": //Vector2
                            Microsoft.Xna.Framework.Vector2 V2 = (Microsoft.Xna.Framework.Vector2)FI.GetValue(Selected_GO);
                            Vector2 V2_IMGUI = new Vector2(V2.X, V2.Y);
                            ImGui.DragFloat2(FI.Name, ref V2_IMGUI);
                            FI.SetValue(Selected_GO, new Microsoft.Xna.Framework.Vector2(V2_IMGUI.X, V2_IMGUI.Y));
                            ImGui.PopID();
                            break;
                        case "Microsoft.Xna.Framework.Vector3": //Vector3
                            Microsoft.Xna.Framework.Vector3 V3 = (Microsoft.Xna.Framework.Vector3)FI.GetValue(Selected_GO);
                            Vector3 V3_IMGUI = new Vector3(V3.X, V3.Y, V3.Z);
                            ImGui.DragFloat3(FI.Name, ref V3_IMGUI);
                            FI.SetValue(Selected_GO, new Microsoft.Xna.Framework.Vector3(V3_IMGUI.X, V3_IMGUI.Y, V3_IMGUI.Z));
                            ImGui.PopID();
                            break;
                        case "Microsoft.Xna.Framework.Vector4": //Vector4
                            Microsoft.Xna.Framework.Vector4 V4 = (Microsoft.Xna.Framework.Vector4)FI.GetValue(Selected_GO);
                            Vector4 V4_IMGUI = new Vector4(V4.X, V4.Y, V4.Z, V4.W);
                            ImGui.DragFloat4(FI.Name, ref V4_IMGUI);
                            FI.SetValue(Selected_GO, new Microsoft.Xna.Framework.Vector4(V4_IMGUI.X, V4_IMGUI.Y, V4_IMGUI.Z, V4_IMGUI.W));
                            ImGui.PopID();
                            break;
                            //default:
                            //    Enum EnumCandidate = Utility.GetInstance(FI.FieldType.FullName) as Enum;

                            //    if (EnumCandidate != null)
                            //        FI.SetValue(this, Enum.Parse(FI.FieldType, Line[2]));
                            //    else
                            //        FI.SetValue(this, null);
                            //    fieldInfos.Add(FI);
                            //    break;
                    }
                }

                ImGui.NewLine();
                ImGui.Separator();
                ImGui.NewLine();

                ImGui.Indent((ImGui.GetWindowSize().X - ImGui.CalcTextSize("Components" + " ---- ").X) * 0.5f);
                ImGui.Text("-- " + "Components" + "--");
                ImGui.Unindent((ImGui.GetWindowSize().X - ImGui.CalcTextSize("Components" + "---- ").X) * 0.5f);
                ImGui.Text("\n");

                foreach (GameObjectComponent GOC in Selected_GO.GameObjectComponents)
                {
                    if (ImGui.CollapsingHeader(GOC.ToString().Remove(0, 9), ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        FieldInfo[] FIS = GOC.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                        foreach (FieldInfo FI in FIS)
                        {
                            switch (FI.FieldType.FullName) //Here, I handle basic types
                            {
                                case "System.Single": //float
                                    float T = (float)FI.GetValue(GOC);
                                    ImGui.DragFloat(FI.Name, ref T);
                                    FI.SetValue(GOC, T);
                                    ImGui.PopID();
                                    break;
                                case "System.Double": //double
                                    double D = (double)FI.GetValue(GOC);
                                    ImGui.InputDouble(FI.Name, ref D);
                                    FI.SetValue(GOC, D);
                                    ImGui.PopID();
                                    break;
                                case "System.Int32": //int 32
                                    int I = (int)FI.GetValue(GOC);
                                    ImGui.InputInt(FI.Name, ref I);
                                    FI.SetValue(GOC, I);
                                    ImGui.PopID();
                                    break;
                                case "System.Int16": //int 16
                                    short S = (short)FI.GetValue(GOC);
                                    Marshal.WriteInt16(intPointer, S);
                                    ImGui.InputScalar(FI.Name, ImGuiDataType.S16, intPointer);
                                    FI.SetValue(GOC, Marshal.ReadInt16(intPointer));
                                    ImGui.PopID();
                                    break;
                                case "System.Int64": //int 64
                                    long L = (long)FI.GetValue(GOC);
                                    Marshal.WriteInt64(intPointerL, L);
                                    ImGui.InputScalar(FI.Name, ImGuiDataType.S64, intPointerL);
                                    FI.SetValue(GOC, Marshal.ReadInt64(intPointerL));
                                    ImGui.PopID();
                                    break;
                                case "System.UInt32": //uint 32
                                    uint U32 = (uint)FI.GetValue(GOC);
                                    Marshal.WriteInt32(intPointerU32, (int)U32);
                                    ImGui.InputScalar(FI.Name, ImGuiDataType.U32, intPointerU32);
                                    FI.SetValue(GOC, Marshal.ReadInt32(intPointerU32));
                                    ImGui.PopID();
                                    break;
                                //case "System.UInt16": //uint 16
                                //    FI.SetValue(this, ushort.Parse(Line[2]));
                                //    fieldInfos.Add(FI);
                                //    break;
                                //case "System.UInt64": //uint 64
                                //    FI.SetValue(this, ulong.Parse(Line[2]));
                                //    fieldInfos.Add(FI);
                                //    break;
                                case "System.Boolean": //bool
                                    bool B = (bool)FI.GetValue(GOC);
                                    ImGui.Checkbox(FI.Name, ref B);
                                    FI.SetValue(GOC, B);
                                    ImGui.PopID();
                                    break;
                                case "Microsoft.Xna.Framework.Vector2": //Vector2
                                    Microsoft.Xna.Framework.Vector2 V2 = (Microsoft.Xna.Framework.Vector2)FI.GetValue(GOC);
                                    Vector2 V2_IMGUI = new Vector2(V2.X, V2.Y);
                                    ImGui.DragFloat2(FI.Name, ref V2_IMGUI);
                                    FI.SetValue(GOC, new Microsoft.Xna.Framework.Vector2(V2_IMGUI.X, V2_IMGUI.Y));
                                    ImGui.PopID();
                                    break;
                                case "Microsoft.Xna.Framework.Vector3": //Vector3
                                    Microsoft.Xna.Framework.Vector3 V3 = (Microsoft.Xna.Framework.Vector3)FI.GetValue(GOC);
                                    Vector3 V3_IMGUI = new Vector3(V3.X, V3.Y, V3.Z);
                                    ImGui.DragFloat3(FI.Name, ref V3_IMGUI);
                                    FI.SetValue(GOC, new Microsoft.Xna.Framework.Vector3(V3_IMGUI.X, V3_IMGUI.Y, V3_IMGUI.Z));
                                    ImGui.PopID();
                                    break;
                                case "Microsoft.Xna.Framework.Vector4": //Vector4
                                    Microsoft.Xna.Framework.Vector4 V4 = (Microsoft.Xna.Framework.Vector4)FI.GetValue(GOC);
                                    Vector4 V4_IMGUI = new Vector4(V4.X, V4.Y, V4.Z, V4.W);
                                    ImGui.DragFloat4(FI.Name, ref V4_IMGUI);
                                    FI.SetValue(GOC, new Microsoft.Xna.Framework.Vector4(V4_IMGUI.X, V4_IMGUI.Y, V4_IMGUI.Z, V4_IMGUI.W));
                                    ImGui.PopID();
                                    break;
                                case "Microsoft.Xna.Framework.Color": //Color
                                    Microsoft.Xna.Framework.Color V4_C = (Microsoft.Xna.Framework.Color)FI.GetValue(GOC);
                                    Vector4 V4_IMGUI_C = new Vector4(V4_C.R / 255.0f, V4_C.G / 255.0f, V4_C.B / 255.0f, V4_C.A / 255.0f);
                                    ImGui.ColorButton(FI.Name, V4_IMGUI_C);
                                    if (ImGui.IsItemClicked())
                                    {
                                        if (ActiveFI == FI && GOC == ActiveGOC)
                                            ColorClicked = !ColorClicked;
                                        else
                                            ColorClicked = false;
                                        ActiveFI = FI;
                                        ActiveGOC = GOC;
                                    }

                                    if (ActiveFI == FI && GOC == ActiveGOC && !ColorClicked)
                                    {
                                        ImGui.ColorPicker4("Color", ref V4_IMGUI_C);
                                        ImGui.PopID();
                                    }

                                    FI.SetValue(GOC, new Microsoft.Xna.Framework.Color(V4_IMGUI_C.X, V4_IMGUI_C.Y, V4_IMGUI_C.Z, V4_IMGUI_C.W));
                                    ImGui.PopID();
                                    break;
                                    //default:
                                    //    Enum EnumCandidate = Utility.GetInstance(FI.FieldType.FullName) as Enum;

                                    //    if (EnumCandidate != null)
                                    //        FI.SetValue(this, Enum.Parse(FI.FieldType, Line[2]));
                                    //    else
                                    //        FI.SetValue(this, null);
                                    //    fieldInfos.Add(FI);
                                    //    break;
                            }
                        }
                    }
                }
            }

            ImGui.End();
        }

        public override void Destroy()
        {
            Marshal.FreeHGlobal(intPointer);
            Marshal.FreeHGlobal(intPointerL);
            Marshal.FreeHGlobal(intPointerU32);
        }
    }
}
