using ImGuiNET;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace MyEngine.FN_Editor
{
    class InspectorWindow: GameObjectComponent
    {
        public static List<Type> ComponentsTypes = new List<Type>();

        private IntPtr intPointer;
        private IntPtr intPointerL;
        private IntPtr intPointerU32;
        private bool ColorClicked = false;
        private FieldInfo ActiveFI = null;
        private PropertyInfo ActivePI = null;
        private GameObjectComponent ActiveGOC = null;
        private bool Subscribed = false;
        private int ChosenComponent = -1;
        private List<bool> ComponentsNotRemoved = new List<bool>();

        static InspectorWindow() //This should be called again on 'Hot reloading'
        {
            GetGOCs();
        }

        private static void GetGOCs()
        {
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == Assembly.GetExecutingAssembly().GetName().Name && t.BaseType == typeof(GameObjectComponent)
                    select t;
            q.ToList().ForEach(item => ComponentsTypes.Add(item));
        }

        public override void Start()
        {
            if (!Subscribed)
            {
                Setup.Game.Exiting += OnExit;
                Subscribed = true;
            }

            intPointer = Marshal.AllocHGlobal(sizeof(short));
            intPointerL = Marshal.AllocHGlobal(sizeof(long));
            intPointerU32 = Marshal.AllocHGlobal(sizeof(uint));
        }

        public override void DrawUI()
        {
            ImGui.Begin("Inspector");

            GameObject Selected_GO = FN_Editor.GameObjects_Tab.WhoIsSelected;

            if (Selected_GO != null)
            {
                if (Selected_GO.ShouldBeDeleted)
                {
                    FN_Editor.GameObjects_Tab.WhoIsSelected = null;
                    return;
                }

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
                    var GOC_SO = FI.GetValue(Selected_GO) as GameObjectComponent;
                    if (GOC_SO != null)
                    {
                        ImGui.InputText(FI.Name, ref GOC_SO.gameObject.Name, 50, ImGuiInputTextFlags.ReadOnly);
                        continue;
                    }
                    else
                    {
                        var GO = FI.GetValue(Selected_GO) as GameObject;
                        if (GO != null)
                        {
                            ImGui.InputText(FI.Name, ref GO.Name, 50, ImGuiInputTextFlags.ReadOnly);
                            continue;
                        }
                    }

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
                        case "Microsoft.Xna.Framework.Point": //Point
                            Microsoft.Xna.Framework.Point P2 = (Microsoft.Xna.Framework.Point)FI.GetValue(Selected_GO);
                            int[] P2_ARR = new int[2] { P2.X, P2.Y };
                            ImGui.DragInt2(FI.Name, ref P2_ARR[0]);
                            FI.SetValue(Selected_GO, new Microsoft.Xna.Framework.Point(P2_ARR[0], P2_ARR[1]));
                            ImGui.PopID();
                            break;
                            //default:
                            //    Enum EnumCandidate = Utility.GetInstance(FI.FieldType.FullName) as Enum;

                            //    if (EnumCandidate != null)
                            //        ImGui.Text(FI.)

                            // break;
                    }
                }

                PropertyInfo[] PIS_GO = Selected_GO.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo PI in PIS_GO)
                {
                    if (PI.GetMethod == null || !PI.GetMethod.IsPublic || PI.SetMethod == null || !PI.SetMethod.IsPublic)
                        continue;

                    var GOC_SO = PI.GetValue(Selected_GO) as GameObjectComponent;
                    if (GOC_SO != null)
                    {
                        ImGui.InputText(PI.Name, ref GOC_SO.gameObject.Name, 50, ImGuiInputTextFlags.ReadOnly);
                        continue;
                    }
                    else
                    {
                        var GO = PI.GetValue(Selected_GO) as GameObject;
                        if (GO != null)
                        {
                            ImGui.InputText(PI.Name, ref GO.Name, 50, ImGuiInputTextFlags.ReadOnly);
                            continue;
                        }
                    }

                    switch (PI.PropertyType.FullName) //Here, I handle basic types
                    {
                        case "System.Single": //float
                            float T = (float)PI.GetValue(Selected_GO);
                            ImGui.DragFloat(PI.Name, ref T);
                            PI.SetValue(Selected_GO, T);
                            ImGui.PopID();
                            break;
                        case "System.Double": //double
                            double D = (double)PI.GetValue(Selected_GO);
                            ImGui.InputDouble(PI.Name, ref D);
                            PI.SetValue(Selected_GO, D);
                            ImGui.PopID();
                            break;
                        case "System.Int32": //int 32
                            int I = (int)PI.GetValue(Selected_GO);
                            ImGui.InputInt(PI.Name, ref I);
                            PI.SetValue(Selected_GO, I);
                            ImGui.PopID();
                            break;
                        case "System.Int16": //int 16
                            short S = (short)PI.GetValue(Selected_GO);
                            Marshal.WriteInt16(intPointer, S);
                            ImGui.InputScalar(PI.Name, ImGuiDataType.S16, intPointer);
                            PI.SetValue(Selected_GO, Marshal.ReadInt16(intPointer));
                            ImGui.PopID();
                            break;
                        case "System.Int64": //int 64
                            long L = (long)PI.GetValue(Selected_GO);
                            Marshal.WriteInt64(intPointerL, L);
                            ImGui.InputScalar(PI.Name, ImGuiDataType.S64, intPointerL);
                            PI.SetValue(Selected_GO, Marshal.ReadInt64(intPointerL));
                            ImGui.PopID();
                            break;
                        case "System.UInt32": //uint 32
                            uint U32 = (uint)PI.GetValue(Selected_GO);
                            Marshal.WriteInt32(intPointerU32, (int)U32);
                            ImGui.InputScalar(PI.Name, ImGuiDataType.U32, intPointerU32);
                            PI.SetValue(Selected_GO, Marshal.ReadInt32(intPointerU32));
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
                            bool B = (bool)PI.GetValue(Selected_GO);
                            ImGui.Checkbox(PI.Name, ref B);
                            PI.SetValue(Selected_GO, B);
                            ImGui.PopID();
                            break;
                        case "Microsoft.Xna.Framework.Vector2": //Vector2
                            Microsoft.Xna.Framework.Vector2 V2 = (Microsoft.Xna.Framework.Vector2)PI.GetValue(Selected_GO);
                            Vector2 V2_IMGUI = new Vector2(V2.X, V2.Y);
                            ImGui.DragFloat2(PI.Name, ref V2_IMGUI);
                            PI.SetValue(Selected_GO, new Microsoft.Xna.Framework.Vector2(V2_IMGUI.X, V2_IMGUI.Y));
                            ImGui.PopID();
                            break;
                        case "Microsoft.Xna.Framework.Vector3": //Vector3
                            Microsoft.Xna.Framework.Vector3 V3 = (Microsoft.Xna.Framework.Vector3)PI.GetValue(Selected_GO);
                            Vector3 V3_IMGUI = new Vector3(V3.X, V3.Y, V3.Z);
                            ImGui.DragFloat3(PI.Name, ref V3_IMGUI);
                            PI.SetValue(Selected_GO, new Microsoft.Xna.Framework.Vector3(V3_IMGUI.X, V3_IMGUI.Y, V3_IMGUI.Z));
                            ImGui.PopID();
                            break;
                        case "Microsoft.Xna.Framework.Vector4": //Vector4
                            Microsoft.Xna.Framework.Vector4 V4 = (Microsoft.Xna.Framework.Vector4)PI.GetValue(Selected_GO);
                            Vector4 V4_IMGUI = new Vector4(V4.X, V4.Y, V4.Z, V4.W);
                            ImGui.DragFloat4(PI.Name, ref V4_IMGUI);
                            PI.SetValue(Selected_GO, new Microsoft.Xna.Framework.Vector4(V4_IMGUI.X, V4_IMGUI.Y, V4_IMGUI.Z, V4_IMGUI.W));
                            ImGui.PopID();
                            break;
                        case "Microsoft.Xna.Framework.Point": //Point
                            Microsoft.Xna.Framework.Point P2 = (Microsoft.Xna.Framework.Point)PI.GetValue(Selected_GO);
                            int[] P2_ARR = new int[2] { P2.X, P2.Y };
                            ImGui.DragInt2(PI.Name, ref P2_ARR[0]);
                            PI.SetValue(Selected_GO, new Microsoft.Xna.Framework.Point(P2_ARR[0], P2_ARR[1]));
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

                GameObjectComponent GOC_Removed = null;
                int T_Counter = 0;

                while (ComponentsNotRemoved.Count < Selected_GO.GameObjectComponents.Count)
                    ComponentsNotRemoved.Add(true);

                bool[] ComponentsNotRemoved_Arr = ComponentsNotRemoved.ToArray();
                foreach (GameObjectComponent GOC in Selected_GO.GameObjectComponents)
                {
                    if (ImGui.CollapsingHeader(GOC.GetType().Name, ref ComponentsNotRemoved_Arr[T_Counter], ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        FieldInfo[] FIS = GOC.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                        foreach (FieldInfo FI in FIS)
                        {
                            var GOC_SO = FI.GetValue(GOC) as GameObjectComponent;
                            if (GOC_SO != null)
                            {
                                ImGui.InputText(FI.Name, ref GOC_SO.gameObject.Name, 50, ImGuiInputTextFlags.ReadOnly);
                                continue;
                            }
                            else
                            {
                                var GO = FI.GetValue(GOC) as GameObject;
                                if (GO != null)
                                {
                                    ImGui.InputText(FI.Name, ref GO.Name, 50, ImGuiInputTextFlags.ReadOnly);
                                    continue;
                                }
                            }

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
                                case "Microsoft.Xna.Framework.Point": //Point
                                    Microsoft.Xna.Framework.Point P2 = (Microsoft.Xna.Framework.Point)FI.GetValue(GOC);
                                    int[] P2_ARR = new int[2] {P2.X, P2.Y };
                                    ImGui.DragInt2(FI.Name, ref P2_ARR[0]);
                                    FI.SetValue(GOC, new Microsoft.Xna.Framework.Point(P2_ARR[0], P2_ARR[1]));
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

                        PropertyInfo[] PIS = GOC.GetType().GetProperties();
                        foreach (PropertyInfo PI in PIS)
                        {
                            if (PI.GetMethod == null || !PI.GetMethod.IsPublic || PI.SetMethod == null || !PI.SetMethod.IsPublic)
                                continue;

                            var GOC_SO = PI.GetValue(GOC) as GameObjectComponent;
                            if (GOC_SO != null)
                            {
                                ImGui.InputText(PI.Name, ref GOC_SO.gameObject.Name, 50, ImGuiInputTextFlags.ReadOnly);
                                continue;
                            }
                            else
                            {
                                var GO = PI.GetValue(GOC) as GameObject;
                                if (GO != null)
                                {
                                    ImGui.InputText(PI.Name, ref GO.Name, 50, ImGuiInputTextFlags.ReadOnly);
                                    continue;
                                }
                            }

                            switch (PI.PropertyType.FullName) //Here, I handle basic types
                            {
                                case "System.Single": //float
                                    float T = (float)PI.GetValue(GOC);
                                    ImGui.DragFloat(PI.Name, ref T);
                                    PI.SetValue(GOC, T);
                                    ImGui.PopID();
                                    break;
                                case "System.Double": //double
                                    double D = (double)PI.GetValue(GOC);
                                    ImGui.InputDouble(PI.Name, ref D);
                                    PI.SetValue(GOC, D);
                                    ImGui.PopID();
                                    break;
                                case "System.Int32": //int 32
                                    int I = (int)PI.GetValue(GOC);
                                    ImGui.InputInt(PI.Name, ref I);
                                    PI.SetValue(GOC, I);
                                    ImGui.PopID();
                                    break;
                                case "System.Int16": //int 16
                                    short S = (short)PI.GetValue(GOC);
                                    Marshal.WriteInt16(intPointer, S);
                                    ImGui.InputScalar(PI.Name, ImGuiDataType.S16, intPointer);
                                    PI.SetValue(GOC, Marshal.ReadInt16(intPointer));
                                    ImGui.PopID();
                                    break;
                                case "System.Int64": //int 64
                                    long L = (long)PI.GetValue(GOC);
                                    Marshal.WriteInt64(intPointerL, L);
                                    ImGui.InputScalar(PI.Name, ImGuiDataType.S64, intPointerL);
                                    PI.SetValue(GOC, Marshal.ReadInt64(intPointerL));
                                    ImGui.PopID();
                                    break;
                                case "System.UInt32": //uint 32
                                    uint U32 = (uint)PI.GetValue(GOC);
                                    Marshal.WriteInt32(intPointerU32, (int)U32);
                                    ImGui.InputScalar(PI.Name, ImGuiDataType.U32, intPointerU32);
                                    PI.SetValue(GOC, Marshal.ReadInt32(intPointerU32));
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
                                    bool B = (bool)PI.GetValue(GOC);
                                    ImGui.Checkbox(PI.Name, ref B);
                                    PI.SetValue(GOC, B);
                                    ImGui.PopID();
                                    break;
                                case "Microsoft.Xna.Framework.Vector2": //Vector2
                                    Microsoft.Xna.Framework.Vector2 V2 = (Microsoft.Xna.Framework.Vector2)PI.GetValue(GOC);
                                    Vector2 V2_IMGUI = new Vector2(V2.X, V2.Y);
                                    ImGui.DragFloat2(PI.Name, ref V2_IMGUI);
                                    PI.SetValue(GOC, new Microsoft.Xna.Framework.Vector2(V2_IMGUI.X, V2_IMGUI.Y));
                                    ImGui.PopID();
                                    break;
                                case "Microsoft.Xna.Framework.Vector3": //Vector3
                                    Microsoft.Xna.Framework.Vector3 V3 = (Microsoft.Xna.Framework.Vector3)PI.GetValue(GOC);
                                    Vector3 V3_IMGUI = new Vector3(V3.X, V3.Y, V3.Z);
                                    ImGui.DragFloat3(PI.Name, ref V3_IMGUI);
                                    PI.SetValue(GOC, new Microsoft.Xna.Framework.Vector3(V3_IMGUI.X, V3_IMGUI.Y, V3_IMGUI.Z));
                                    ImGui.PopID();
                                    break;
                                case "Microsoft.Xna.Framework.Vector4": //Vector4
                                    Microsoft.Xna.Framework.Vector4 V4 = (Microsoft.Xna.Framework.Vector4)PI.GetValue(GOC);
                                    Vector4 V4_IMGUI = new Vector4(V4.X, V4.Y, V4.Z, V4.W);
                                    ImGui.DragFloat4(PI.Name, ref V4_IMGUI);
                                    PI.SetValue(GOC, new Microsoft.Xna.Framework.Vector4(V4_IMGUI.X, V4_IMGUI.Y, V4_IMGUI.Z, V4_IMGUI.W));
                                    ImGui.PopID();
                                    break;
                                case "Microsoft.Xna.Framework.Color": //Color
                                    Microsoft.Xna.Framework.Color V4_C = (Microsoft.Xna.Framework.Color)PI.GetValue(GOC);
                                    Vector4 V4_IMGUI_C = new Vector4(V4_C.R / 255.0f, V4_C.G / 255.0f, V4_C.B / 255.0f, V4_C.A / 255.0f);
                                    ImGui.ColorButton(PI.Name, V4_IMGUI_C);
                                    if (ImGui.IsItemClicked())
                                    {
                                        if (ActivePI == PI && GOC == ActiveGOC)
                                            ColorClicked = !ColorClicked;
                                        else
                                            ColorClicked = false;
                                        ActivePI = PI;
                                        ActiveGOC = GOC;
                                    }

                                    if (ActivePI == PI && GOC == ActiveGOC && !ColorClicked)
                                    {
                                        ImGui.ColorPicker4("Color", ref V4_IMGUI_C);
                                        ImGui.PopID();
                                    }

                                    PI.SetValue(GOC, new Microsoft.Xna.Framework.Color(V4_IMGUI_C.X, V4_IMGUI_C.Y, V4_IMGUI_C.Z, V4_IMGUI_C.W));
                                    ImGui.PopID();
                                    break;
                                case "Microsoft.Xna.Framework.Point": //Point
                                    Microsoft.Xna.Framework.Point P2 = (Microsoft.Xna.Framework.Point)PI.GetValue(GOC);
                                    int[] P2_ARR = new int[2] { P2.X, P2.Y };
                                    ImGui.DragInt2(PI.Name, ref P2_ARR[0]);
                                    PI.SetValue(GOC, new Microsoft.Xna.Framework.Point(P2_ARR[0], P2_ARR[1]));
                                    ImGui.PopID();
                                    break;
                                    //default:
                                    //    Enum EnumCandidate = Utility.GetInstance(PI.PropertyType.FullName) as Enum;

                                    //    if (EnumCandidate != null)
                                    //        PI.SetValue(this, Enum.Parse(FI.FieldType, Line[2]));
                                    //    else
                                    //        PI.SetValue(this, null);
                                    //    fieldInfos.Add(PI);
                                    //    break;
                            }
                        }


                    }

                    if (!ComponentsNotRemoved_Arr[T_Counter])
                    {
                        GOC_Removed = GOC;
                        ComponentsNotRemoved.RemoveAt(T_Counter++);
                    }

                    ImGui.NewLine();
                    ImGui.Separator();
                    ImGui.NewLine();
                }

                Selected_GO.RemoveComponent(GOC_Removed);

                if (ImGui.Button("Add Component", new Vector2(ImGui.GetWindowSize().X, 20)) && ChosenComponent != -1)
                {
                    var GOC = Utility.GetInstance(ComponentsTypes[ChosenComponent]) as GameObjectComponent;
                    bool AddedSuccessfully = Selected_GO.AddComponent_Generic(GOC);

                    if (AddedSuccessfully)
                        GOC.Start();
                    else
                    {
                        GOC.Destroy();
                        GOC = null;
                    }
                }

                string[] Names = new string[ComponentsTypes.Count];
                for (int i = 0; i < Names.Length; i++)
                    Names[i] = ComponentsTypes[i].Name;

                ImGui.Combo("Components", ref ChosenComponent, Names, Names.Length);
                ImGui.PopID();
            }

            ImGui.End();
        }

        public override void Destroy()
        {
            Marshal.FreeHGlobal(intPointer);
            Marshal.FreeHGlobal(intPointerL);
            Marshal.FreeHGlobal(intPointerU32);
        }

        private void OnExit(object sender, System.EventArgs e)
        {
            Marshal.FreeHGlobal(intPointer);
            Marshal.FreeHGlobal(intPointerL);
            Marshal.FreeHGlobal(intPointerU32);
        }
    }
}
