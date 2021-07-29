using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FN_Engine
{
    public static class Utility
    {
        public static ObjectIDGenerator OIG = new ObjectIDGenerator();

        private static Regex NameRegex = null;
        private static Regex NameFormatRegex = null;
        private static Regex TexRegex = null;
        private static Regex MusicRegex = null;
        private static Regex ShaderRegex = null;
        private static Regex FontRegex = null;

        static Utility()
        {
            NameRegex = new Regex(@"([][(][0-9]+[)])$");
            NameFormatRegex = new Regex(@"([.]*[][(][0-9]+[)])");
            TexRegex = new Regex(@"([\.]\b(png|jpg|jpeg)\b)$", RegexOptions.IgnoreCase);
            MusicRegex = new Regex(@"([\.]\b(wav|ogg|wma|mp3)\b)$", RegexOptions.IgnoreCase);
            ShaderRegex = new Regex(@"([\.]\b(fx)\b)$", RegexOptions.IgnoreCase);
            FontRegex = new Regex(@"([\.]\b(spritefont)\b)$", RegexOptions.IgnoreCase);
        }

        public static bool CircleContains(Vector2 Center, int Radius, Vector2 Point)
        {
            Vector2 Output = Center - Point;
            if (MathCompanion.Abs(Output.Length()) > Radius)
                return false;

            return true;
        }

        public static void Vector2Int(ref Vector2 vector)
        {
            vector.X = (int)vector.X;
            vector.Y = (int)vector.Y;
        }

        public static Vector2 STR_To_Vector2(string Value)
        {
            Vector2 Output = Vector2.Zero;

            string[] Vector2Format = Value.Split(' ');
            Output.X = float.Parse(Vector2Format[0].Remove(0, 3));
            string Y = Vector2Format[1].Remove(0, 2);
            Y = Y.Remove(Y.Length - 1, 1);
            Output.Y = float.Parse(Y);

            return Output;
        }

        public static Point STR_To_Point(string Value)
        {
            Point Output = Point.Zero;

            string[] PointFormat = Value.Split(' ');
            Output.X = int.Parse(PointFormat[0].Remove(0, 3));
            string Y = PointFormat[1].Remove(0, 2);
            Y = Y.Remove(Y.Length - 1, 1);
            Output.Y = int.Parse(Y);

            return Output;
        }

        public static Vector3 STR_To_Vector3(string Value)
        {
            Vector3 Output = Vector3.Zero;

            string[] Vector3Format = Value.Split(' ');
            Output.X = float.Parse(Vector3Format[0].Remove(0, 3));
            string Z = Vector3Format[2].Remove(0, 2);
            Z = Z.Remove(Z.Length - 1, 1);
            Output.Z = float.Parse(Z);
            Output.Y = float.Parse(Vector3Format[1].Remove(0, 2));

            return Output;
        }

        public static Vector4 STR_To_Vector4(string Value)
        {
            Vector4 Output = Vector4.Zero;

            string[] Vector4Format = Value.Split(' ');
            Output.X = float.Parse(Vector4Format[0].Remove(0, 3));
            string W = Vector4Format[3].Remove(0, 2);
            W = W.Remove(W.Length - 1, 1);
            Output.W = float.Parse(W);
            Output.Y = float.Parse(Vector4Format[1].Remove(0, 2));
            Output.Z = float.Parse(Vector4Format[2].Remove(0, 2));

            return Output;
        }

        public static Color STR_To_Color(string Value)
        {
            Color Output = Color.Transparent;

            string[] ColorFormat = Value.Split(' ');
            Output.R = byte.Parse(ColorFormat[0].Remove(0, 3));
            Output.G = byte.Parse(ColorFormat[1].Remove(0, 2));
            Output.B = byte.Parse(ColorFormat[2].Remove(0, 2));
            string A = ColorFormat[3].Remove(0, 2);
            A = A.Remove(A.Length - 1, 1);
            Output.A = byte.Parse(A);

            return Output;
        }

        //Parse Rectangle here
        public static Rectangle STR_To_Rect(string Value)
        {
            Rectangle Output = Rectangle.Empty;

            string[] RectangleFormat = Value.Split(' ');
            Output.X = int.Parse(RectangleFormat[0].Remove(0, 3));
            Output.Y = int.Parse(RectangleFormat[1].Remove(0, 2));
            Output.Width = int.Parse(RectangleFormat[2].Remove(0, 6));
            string W = RectangleFormat[3].Remove(0, 7);
            W = W.Remove(W.Length - 1, 1);
            Output.Height = int.Parse(W);

            return Output;
        }

        public static Vector2 Vector2Int(Vector2 vector)
        {
            return new Vector2((int)vector.X, (int)vector.Y);
        }

        public static object GetInstance(string ObjectType, object[] Args = null)
        {
            Type T = Type.GetType(ObjectType);
            return Activator.CreateInstance(T, Args);
        }

        public static object GetInstance(Type ObjectType, object[] Args = null)
        {
            return Activator.CreateInstance(ObjectType, Args);
        }

        // This function is from a stackoverflow question:
        // URL: https://stackoverflow.com/questions/437419/execute-multiple-command-lines-with-the-same-process-using-net
        public static void ExecuteCommand(string[] Commands, string Path)
        {
            string batFileName = Path + @"\" + Guid.NewGuid() + ".bat";

            using (StreamWriter batFile = new StreamWriter(batFileName))
            {
                foreach (string Comm in Commands)
                    batFile.WriteLine(Comm);
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", "/c " + batFileName);
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;

            Process p = new Process();
            p.StartInfo = processStartInfo;
            p.Start();
            p.WaitForExit();

            File.Delete(batFileName);
        }

        public static void SerializeV2(JsonTextWriter JW, object OBJ, bool ShouldWriteValue = false)
        {
            if (OBJ == null)
                return;

            long GID = OIG.GetId(OBJ, out bool FirstTime);

            //Check for Special Types
            switch (OBJ.GetType().Name)
            {
                case "Texture2D":
                    if (!ShouldWriteValue)
                        JW.WritePropertyName("Should Be Fixed");

                    if (((Texture2D)OBJ).Name == null)
                        ((Texture2D)OBJ).Name = "CustomTexture " + GID.ToString();

                    JW.WriteValue(((Texture2D)OBJ).Name);
                    return;
                case "Effect":
                    if (!ShouldWriteValue)
                        JW.WritePropertyName("Should Be Fixed");

                    JW.WriteValue(((Effect)OBJ).Name);
                    return;
            }

            if (!FirstTime) //Serialized Before!
            {
                JW.WriteValue(GID.ToString());

                return;
            }

            JW.WriteStartObject();

            JW.WritePropertyName("Type");
            JW.WriteValue(OBJ.GetType().ToString());

            JW.WritePropertyName("Generated ID");
            JW.WriteValue(GID);



            foreach (FieldInfo Member in OBJ.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Member.GetValue(OBJ) == null)
                    continue;

                Type MemType = Member.FieldType;

                if (MemType.IsValueType || MemType == typeof(string))
                {
                    JW.WritePropertyName(Member.Name);
                    JW.WriteValue(Member.GetValue(OBJ).ToString());
                }
                else if (MemType.IsArray || MemType.GetInterface("IEnumerable") != null)
                {
                    JW.WritePropertyName(Member.Name);
                    JW.WriteStartArray();

                    JW.WriteStartObject();

                    JW.WritePropertyName("Length");
                    JW.WriteValue(((ICollection)Member.GetValue(OBJ)).Count);

                    JW.WritePropertyName("Item Type");

                    if (!MemType.IsArray)
                        JW.WriteValue(((IEnumerable)Member.GetValue(OBJ)).GetType().GetGenericArguments()[0].ToString());
                    else
                        JW.WriteValue(MemType.FullName.Remove(MemType.FullName.Length - 2, 2));

                    JW.WriteEndObject();

                    if (Member.FieldType.GetInterface("IDictionary") != null)
                    {
                        var DicVal = Member.GetValue(OBJ) as IDictionary;

                        IEnumerator KEYS = DicVal.Keys.GetEnumerator(), VALUES = DicVal.Values.GetEnumerator();

                        KEYS.MoveNext();
                        VALUES.MoveNext();

                        foreach (var Item in (IEnumerable)DicVal) //Serialize each dictionary entry
                        {
                            // Key
                            if (KEYS.Current.GetType().IsValueType || KEYS.Current.GetType() == typeof(string))
                                JW.WriteValue(KEYS.Current.ToString());
                            else //Serializable class
                                SerializeV2(JW, KEYS.Current);

                            // Value
                            if (VALUES.Current.GetType().IsValueType || VALUES.Current.GetType() == typeof(string))
                                JW.WriteValue(VALUES.Current.ToString());
                            else //Serializable class
                                SerializeV2(JW, VALUES.Current);

                            KEYS.MoveNext();
                            VALUES.MoveNext();
                        }
                    }
                    else
                    {
                        foreach (var Item in (IEnumerable)Member.GetValue(OBJ))
                        {
                            if (Item.GetType().IsValueType || Item.GetType() == typeof(string))
                                JW.WriteValue(Item.ToString());
                            else
                                SerializeV2(JW, Item); //Supporting one level of lists (List<List<>> is not supported now)
                        }
                    }

                    JW.WriteEndArray();
                }
                else //Hopefully a serializable class :D
                {
                    JW.WritePropertyName(Member.Name);
                    SerializeV2(JW, Member.GetValue(OBJ), true);
                }
            }

            foreach (PropertyInfo Member in OBJ.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Member.GetValue(OBJ) == null)
                    continue;

                Type MemType = Member.PropertyType;

                if (MemType.IsValueType || MemType == typeof(string))
                {
                    JW.WritePropertyName(Member.Name);
                    JW.WriteValue(Member.GetValue(OBJ).ToString());
                }
                else if (MemType.IsArray || MemType.GetInterface("IEnumerable") != null)
                {
                    JW.WritePropertyName(Member.Name);
                    JW.WriteStartArray();

                    JW.WriteStartObject();

                    JW.WritePropertyName("Length");
                    JW.WriteValue(((ICollection)Member.GetValue(OBJ)).Count);

                    
                    JW.WritePropertyName("Item Type");

                    if (!MemType.IsArray)
                        JW.WriteValue(((IEnumerable)Member.GetValue(OBJ)).GetType().GetGenericArguments()[0].ToString());
                    else
                        JW.WriteValue(MemType.FullName.Remove(MemType.FullName.Length - 2, 2));

                    JW.WriteEndObject();

                    if (Member.PropertyType.GetInterface("IDictionary") != null)
                    {
                        var DicVal = Member.GetValue(OBJ) as IDictionary;

                        IEnumerator KEYS = DicVal.Keys.GetEnumerator(), VALUES = DicVal.Values.GetEnumerator();

                        KEYS.MoveNext();
                        VALUES.MoveNext();

                        foreach (var Item in (IEnumerable)DicVal) //Serialize each dictionary entry
                        {
                            // Key
                            if (KEYS.Current.GetType().IsValueType || KEYS.Current.GetType() == typeof(string))
                                JW.WriteValue(KEYS.Current.ToString());
                            else //Serializable class
                                SerializeV2(JW, KEYS.Current);

                            // Value
                            if (VALUES.Current.GetType().IsValueType || VALUES.Current.GetType() == typeof(string))
                                JW.WriteValue(VALUES.Current.ToString());
                            else //Serializable class
                                SerializeV2(JW, VALUES.Current);

                            KEYS.MoveNext();
                            VALUES.MoveNext();
                        }
                    }
                    else
                    {
                        foreach (var Item in (IEnumerable)Member.GetValue(OBJ))
                        {
                            if (Item.GetType().IsValueType || Item.GetType() == typeof(string))
                                JW.WriteValue(Item.ToString());
                            else
                                SerializeV2(JW, Item); //Supporting one level of lists (List<List<>> is not supported now)
                        }
                    }

                    JW.WriteEndArray();
                }
                else //Hopefully a serializable class :D
                {
                    JW.WritePropertyName(Member.Name);
                    SerializeV2(JW, Member.GetValue(OBJ), true);
                }
            }

            JW.WriteEndObject();
        }

        public static object DeserializeV2(JsonTextReader JR, Dictionary<long, object> SerializedObjects)
        {
            JR.Read(); //Object Type
            JR.Read(); //Object Type Value

            Type ObjType = Type.GetType(JR.Value.ToString());
            if (ObjType == null && FN_Editor.EditorScene.IsThisTheEditor)
                ObjType = FN_Editor.InspectorWindow.GameAssem.GetType(JR.Value.ToString());

            var OBJ = GetInstance(ObjType);

            JR.Read(); //GID
            JR.Read(); //GID Value
            long GID = long.Parse(JR.Value.ToString());

            SerializedObjects[GID] = OBJ;

            while (JR.Read())
            {
                if (JR.TokenType == JsonToken.EndObject)
                    return OBJ;
                else if (JR.TokenType == JsonToken.EndArray)
                    continue;

                FieldInfo FI = ObjType.GetField(JR.Value.ToString());
                PropertyInfo PI = ObjType.GetProperty(JR.Value.ToString());

                if (FI != null)
                {
                    if (FI.FieldType.IsValueType || FI.FieldType == typeof(string)) //Value type (Serialized Immediately)
                    {
                        JR.Read(); //Value
                        FI.SetValue(OBJ, FromJsonToPrimvTypes(JR.Value.ToString(), FI.FieldType));
                    }
                    else if (FI.FieldType.IsArray)
                    {
                        JR.Read(); //Start Array

                        //Metadata
                        JR.Read(); // Start Object
                        JR.Read(); // Array Length
                        JR.Read(); // Its value

                        int ArrLength = int.Parse(JR.Value.ToString());

                        JR.Read(); // Item Type
                        JR.Read(); // It's value

                        Type IteratedObjectType = Type.GetType(JR.Value.ToString());

                        JR.Read(); // End Object
                                   // End of Metadata

                        //Iterating Objects
                        var OBJ_Arr = (IList)GetInstance(FI.FieldType, new object[] { ArrLength });
                        for (int i = 0; i < ArrLength; i++)
                        {
                            JR.Read(); //Value

                            if (IteratedObjectType.IsValueType || IteratedObjectType == typeof(string)) //Immediate assigining
                            {
                                OBJ_Arr[i] = FromJsonToPrimvTypes(JR.Value.ToString(), IteratedObjectType);
                            }
                            else
                            {
                                if (JR.TokenType == JsonToken.StartObject)
                                    OBJ_Arr[i] = DeserializeV2(JR, SerializedObjects);
                                else if (long.TryParse(JR.Value.ToString(), out long ResultGID))
                                    OBJ_Arr[i] = SerializedObjects[ResultGID];
                                else if (FI.FieldType == typeof(Texture2D))
                                    OBJ_Arr[i] = Setup.Content.Load<Texture2D>(JR.Value.ToString());
                                else if (FI.FieldType == typeof(Effect))
                                    OBJ_Arr[i] = Setup.Content.Load<Effect>(JR.Value.ToString());

                            }
                        }

                        FI.SetValue(OBJ, OBJ_Arr);
                    }
                    else if (FI.FieldType.GetInterface("IEnumerable") != null) //Lists?
                    {
                        JR.Read(); //Start Array

                        //Metadata
                        JR.Read(); // Start Object
                        JR.Read(); // List Length
                        JR.Read(); // Its value

                        int ListLength = int.Parse(JR.Value.ToString());

                        JR.Read(); // Item Type
                        JR.Read(); // It's value

                        Type IteratedObjectType = Type.GetType(JR.Value.ToString());

                        JR.Read(); // End Object
                                   // End of Metadata

                        // Handle Dictionaries Here Key then Value respectively
                        if (FI.FieldType.GetInterface("IDictionary") != null)
                        {
                            var OBJ_Dic = (IDictionary)GetInstance(FI.FieldType);
                            Type[] KeyAndValTypes = OBJ_Dic.GetType().GetGenericArguments();

                            for (int i = 0; i < ListLength; i++)
                            {
                                object Key = null;
                                object Value = null;

                                JR.Read(); //Value
                                //Deserialize Key
                                if (KeyAndValTypes[0].IsValueType || KeyAndValTypes[0] == typeof(string)) //Immediate assigining
                                    Key = FromJsonToPrimvTypes(JR.Value.ToString(), KeyAndValTypes[0]);
                                else
                                {
                                    if (JR.TokenType == JsonToken.StartObject)
                                        Key = DeserializeV2(JR, SerializedObjects);
                                    else if (long.TryParse(JR.Value.ToString(), out long ResultGID))
                                        Key = SerializedObjects[ResultGID];
                                    else if (FI.FieldType == typeof(Texture2D))
                                        Key = Setup.Content.Load<Texture2D>(JR.Value.ToString());
                                    else if (FI.FieldType == typeof(Effect))
                                        Key = Setup.Content.Load<Effect>(JR.Value.ToString());
                                }

                                JR.Read(); //Value
                                //Deserialize Value
                                if (KeyAndValTypes[1].IsValueType || KeyAndValTypes[1] == typeof(string)) //Immediate assigining
                                    Value = FromJsonToPrimvTypes(JR.Value.ToString(), KeyAndValTypes[1]);
                                else
                                {
                                    if (JR.TokenType == JsonToken.StartObject)
                                        Value = DeserializeV2(JR, SerializedObjects);
                                    else if (long.TryParse(JR.Value.ToString(), out long ResultGID))
                                        Value = SerializedObjects[ResultGID];
                                    else if (FI.FieldType == typeof(Texture2D))
                                        Value = Setup.Content.Load<Texture2D>(JR.Value.ToString());
                                    else if (FI.FieldType == typeof(Effect))
                                        Value = Setup.Content.Load<Effect>(JR.Value.ToString());
                                }

                                OBJ_Dic.Add(Key, Value);
                            }

                            FI.SetValue(OBJ, OBJ_Dic);
                        }
                        else
                        {
                            //Iterating Objects
                            var OBJ_List = (IList)GetInstance(FI.FieldType);

                            for (int i = 0; i < ListLength; i++)
                            {
                                JR.Read(); //Value

                                if (IteratedObjectType.IsValueType || IteratedObjectType == typeof(string)) //Immediate assigining
                                {
                                    OBJ_List.Add(FromJsonToPrimvTypes(JR.Value.ToString(), IteratedObjectType));
                                }
                                else
                                {
                                    if (JR.TokenType == JsonToken.StartObject)
                                        OBJ_List.Add(DeserializeV2(JR, SerializedObjects));
                                    else if (long.TryParse(JR.Value.ToString(), out long ResultGID))
                                        OBJ_List.Add(SerializedObjects[ResultGID]);
                                    else if (FI.FieldType == typeof(Texture2D))
                                        OBJ_List.Add(Setup.Content.Load<Texture2D>(JR.Value.ToString()));
                                    else if (FI.FieldType == typeof(Effect))
                                        OBJ_List.Add(Setup.Content.Load<Effect>(JR.Value.ToString()));

                                }
                            }

                            FI.SetValue(OBJ, OBJ_List);
                        }
                    }
                    else //Serializable Objects?
                    {
                        JR.Read(); //Value

                        if (JR.TokenType == JsonToken.StartObject)
                            FI.SetValue(OBJ, DeserializeV2(JR, SerializedObjects));
                        else if (long.TryParse(JR.Value.ToString(), out long ResultGID))
                            FI.SetValue(OBJ, SerializedObjects[ResultGID]);
                        else if (FI.FieldType == typeof(Texture2D))
                            FI.SetValue(OBJ, Setup.Content.Load<Texture2D>(JR.Value.ToString()));
                        else if (FI.FieldType == typeof(Effect))
                            FI.SetValue(OBJ, Setup.Content.Load<Effect>(JR.Value.ToString()));

                    }
                }
                else if (PI != null)
                {
                    if (PI.PropertyType.IsValueType || PI.PropertyType == typeof(string)) //Value type (Serialized Immediately)
                    {
                        JR.Read(); //Value
                        PI.SetValue(OBJ, FromJsonToPrimvTypes(JR.Value.ToString(), PI.PropertyType));
                    }
                    else if (PI.PropertyType.IsArray)
                    {
                        JR.Read(); //Start Array

                        //Metadata
                        JR.Read(); // Start Object
                        JR.Read(); // Array Length
                        JR.Read(); // Its value

                        int ArrLength = int.Parse(JR.Value.ToString());

                        JR.Read(); // Item Type
                        JR.Read(); // It's value

                        Type IteratedObjectType = Type.GetType(JR.Value.ToString());

                        JR.Read(); // End Object
                                   // End of Metadata

                        //Iterating Objects
                        var OBJ_Arr = (IList)GetInstance(PI.PropertyType, new object[] { ArrLength });
                        for (int i = 0; i < ArrLength; i++)
                        {
                            JR.Read(); //Value

                            if (IteratedObjectType.IsValueType || IteratedObjectType == typeof(string)) //Immediate assigining
                            {
                                OBJ_Arr[i] = FromJsonToPrimvTypes(JR.Value.ToString(), IteratedObjectType);
                            }
                            else
                            {
                                if (JR.TokenType == JsonToken.StartObject)
                                    OBJ_Arr[i] = DeserializeV2(JR, SerializedObjects);
                                else if (long.TryParse(JR.Value.ToString(), out long ResultGID))
                                    OBJ_Arr[i] = SerializedObjects[ResultGID];
                                else if (PI.PropertyType == typeof(Texture2D))
                                    OBJ_Arr[i] = Setup.Content.Load<Texture2D>(JR.Value.ToString());
                                else if (PI.PropertyType == typeof(Effect))
                                    OBJ_Arr[i] = Setup.Content.Load<Effect>(JR.Value.ToString());

                            }
                        }

                        PI.SetValue(OBJ, OBJ_Arr);
                    }
                    else if (PI.PropertyType.GetInterface("IEnumerable") != null) //Lists?
                    {
                        JR.Read(); //Start Array

                        //Metadata
                        JR.Read(); // Start Object
                        JR.Read(); // List Length
                        JR.Read(); // Its value

                        int ListLength = int.Parse(JR.Value.ToString());

                        JR.Read(); // Item Type
                        JR.Read(); // It's value

                        Type IteratedObjectType = Type.GetType(JR.Value.ToString());

                        JR.Read(); // End Object
                                   // End of Metadata

                        // Handle Dictionaries Here Key then Value respectively
                        if (PI.PropertyType.GetInterface("IDictionary") != null)
                        {
                            var OBJ_Dic = (IDictionary)GetInstance(PI.PropertyType);
                            Type[] KeyAndValTypes = OBJ_Dic.GetType().GetGenericArguments();

                            for (int i = 0; i < ListLength; i++)
                            {
                                object Key = null;
                                object Value = null;

                                JR.Read(); //Value
                                //Deserialize Key
                                if (KeyAndValTypes[0].IsValueType || KeyAndValTypes[0] == typeof(string)) //Immediate assigining
                                    Key = FromJsonToPrimvTypes(JR.Value.ToString(), KeyAndValTypes[0]);
                                else
                                {
                                    if (JR.TokenType == JsonToken.StartObject)
                                        Key = DeserializeV2(JR, SerializedObjects);
                                    else if (long.TryParse(JR.Value.ToString(), out long ResultGID))
                                        Key = SerializedObjects[ResultGID];
                                    else if (PI.PropertyType == typeof(Texture2D))
                                        Key = Setup.Content.Load<Texture2D>(JR.Value.ToString());
                                    else if (PI.PropertyType == typeof(Effect))
                                        Key = Setup.Content.Load<Effect>(JR.Value.ToString());
                                }

                                JR.Read(); //Value
                                //Deserialize Value
                                if (KeyAndValTypes[1].IsValueType || KeyAndValTypes[1] == typeof(string)) //Immediate assigining
                                    Value = FromJsonToPrimvTypes(JR.Value.ToString(), KeyAndValTypes[1]);
                                else
                                {
                                    if (JR.TokenType == JsonToken.StartObject)
                                        Value = DeserializeV2(JR, SerializedObjects);
                                    else if (long.TryParse(JR.Value.ToString(), out long ResultGID))
                                        Value = SerializedObjects[ResultGID];
                                    else if (PI.PropertyType == typeof(Texture2D))
                                        Value = Setup.Content.Load<Texture2D>(JR.Value.ToString());
                                    else if (PI.PropertyType == typeof(Effect))
                                        Value = Setup.Content.Load<Effect>(JR.Value.ToString());
                                }

                                OBJ_Dic.Add(Key, Value);
                            }

                            PI.SetValue(OBJ, OBJ_Dic);
                        }
                        else
                        {
                            //Iterating Objects
                            var OBJ_List = (IList)GetInstance(PI.PropertyType);

                            for (int i = 0; i < ListLength; i++)
                            {
                                JR.Read(); //Value

                                if (IteratedObjectType.IsValueType || IteratedObjectType == typeof(string)) //Immediate assigining
                                {
                                    OBJ_List.Add(FromJsonToPrimvTypes(JR.Value.ToString(), IteratedObjectType));
                                }
                                else
                                {
                                    if (JR.TokenType == JsonToken.StartObject)
                                        OBJ_List.Add(DeserializeV2(JR, SerializedObjects));
                                    else if (long.TryParse(JR.Value.ToString(), out long ResultGID))
                                        OBJ_List.Add(SerializedObjects[ResultGID]);
                                    else if (PI.PropertyType == typeof(Texture2D))
                                        OBJ_List.Add(Setup.Content.Load<Texture2D>(JR.Value.ToString()));
                                    else if (PI.PropertyType == typeof(Effect))
                                        OBJ_List.Add(Setup.Content.Load<Effect>(JR.Value.ToString()));
                                }
                            }

                            PI.SetValue(OBJ, OBJ_List);
                        }
                    }
                    else //Serializable Objects?
                    {
                        JR.Read(); //Value

                        if (JR.TokenType == JsonToken.StartObject)
                            PI.SetValue(OBJ, DeserializeV2(JR, SerializedObjects));
                        else if (long.TryParse(JR.Value.ToString(), out long ResultGID))
                            PI.SetValue(OBJ, SerializedObjects[ResultGID]);
                        else if (PI.PropertyType == typeof(Texture2D))
                            PI.SetValue(OBJ, Setup.Content.Load<Texture2D>(JR.Value.ToString()));
                        else if (PI.PropertyType == typeof(Effect))
                            PI.SetValue(OBJ, Setup.Content.Load<Effect>(JR.Value.ToString()));

                    }
                }
                //else
                    //throw new Exception("Serialized member is not present in the class!");
            }

            return null; //This line shouldn't be logically reachable
        }

        private static object FromJsonToPrimvTypes(string Value, Type MemType)
        {
            if (MemType.IsEnum)
                return Enum.Parse(MemType, Value.ToString());
            else if (MemType == typeof(string))
                return Value;
            else if (MemType == typeof(int))
                return int.Parse(Value);
            else if (MemType == typeof(float))
                return float.Parse(Value);
            else if (MemType == typeof(double))
                return double.Parse(Value);
            else if (MemType == typeof(bool))
                return bool.Parse(Value);
            else if (MemType == typeof(Vector2))
                return STR_To_Vector2(Value);
            else if (MemType == typeof(Point))
                return STR_To_Point(Value);
            else if (MemType == typeof(Vector3))
                return STR_To_Vector3(Value);
            else if (MemType == typeof(Vector4))
                return STR_To_Vector4(Value);
            else if (MemType == typeof(Rectangle))
                return STR_To_Rect(Value);
            else if (MemType == typeof(Color))
                return STR_To_Color(Value);

            throw new Exception("Type Not Handled!");
        }

        public static void Serialize(StreamWriter SW, object OBJ)  //Make this the default one
        {
            long GID = OIG.GetId(OBJ, out bool FirstTime);

            if (!FirstTime)
            {
                SW.WriteLine("--SerializedBefore--\t" + OBJ.GetType().FullName + "\t" + GID);
                return;
            }

            SW.WriteLine(OBJ.ToString());

            SW.WriteLine("GeneratedID\t" + GID);
            FieldInfo[] SerializableFields = OBJ.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] SerializableProps = OBJ.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            SW.WriteLine("SerializableFields\t" + SerializableFields.Length);
            foreach (FieldInfo FI in SerializableFields)
            {
                if (FI.FieldType.GetInterfaces().Contains(typeof(IEnumerable)) && FI.FieldType != typeof(string))
                {
                    int Length = 0;
                    PropertyInfo L = FI.FieldType.GetProperty("Length");
                    if (L != null)
                        Length = (int)L.GetValue(FI.GetValue(OBJ));
                    else
                        Length = (int)FI.FieldType.GetProperty("Count").GetValue(FI.GetValue(OBJ));

                    SW.WriteLine("@\t" + FI.FieldType.FullName + "\t" + FI.Name + "\t" + Length.ToString()); // '@' Refers to an enumerable type like list...

                    if (FI.FieldType.GetInterfaces().Contains(typeof(IDictionary)))
                    {
                        var Dic = (IDictionary)FI.GetValue(OBJ);
                        IEnumerator KEYS = Dic.Keys.GetEnumerator(), VALUES = Dic.Values.GetEnumerator();

                        KEYS.MoveNext();
                        VALUES.MoveNext();

                        SW.WriteLine(KEYS.Current.GetType().IsValueType.ToString() + "\t" + VALUES.Current.GetType().IsValueType.ToString());

                        foreach (var item in (IEnumerable)FI.GetValue(OBJ)) //We Need to take another look at how Serialize works...
                        {
                            if (!KEYS.Current.GetType().IsValueType)
                                Serialize(SW, KEYS.Current);
                            else
                                SW.Write(KEYS.Current.GetType().FullName + "\t" + KEYS.Current.GetType().Name + "\t" + KEYS.Current.ToString() + "\t" + KEYS.Current.GetType().IsClass.ToString() + "\n");

                            if (!VALUES.Current.GetType().IsValueType)
                                Serialize(SW, VALUES.Current);
                            else
                                SW.Write(VALUES.Current.GetType().FullName + "\t" + VALUES.Current.GetType().Name + "\t" + VALUES.Current.ToString() + "\t" + VALUES.Current.GetType().IsClass.ToString() + "\n");

                            KEYS.MoveNext();
                            VALUES.MoveNext();
                        }
                    }
                    else
                    {
                        bool SendOneTime = true;
                        foreach (var item in (IEnumerable)FI.GetValue(OBJ))
                        {
                            if (SendOneTime)
                            {
                                SW.WriteLine(item.GetType().IsValueType.ToString());
                                SendOneTime = false;
                            }

                            if (item.GetType().IsValueType)
                                SW.Write(item.GetType().FullName + "\t" + item.GetType().Name + "\t" + item.ToString() + "\t" + item.GetType().IsClass.ToString() + "\n");
                            else
                                Serialize(SW, item);
                        }
                    }
                }
                else
                {
                    if (!FI.FieldType.IsValueType)
                    {
                        if (FI.GetValue(OBJ) != null)
                        {
                            bool FT = false;
                            long ObjectGID = -1;
                            switch (FI.FieldType.Name) //Handling Some Types differently, the '*' indicates that it's not serialized normally
                            {
                                case "Texture2D":
                                    ObjectGID = OIG.GetId(FI.GetValue(OBJ), out FT);
                                    Texture2D T2D = FI.GetValue(OBJ) as Texture2D;
                                    if (T2D.Name == null) //CUstom Texture
                                    {
                                        if (FT)
                                        {
                                            using (var CreatedFile = File.Create("CT" + ObjectGID.ToString() + ".png"))
                                            {
                                                T2D.SaveAsPng(CreatedFile, T2D.Width, T2D.Height);
                                            }

                                            //Registering and Building an Item in the Content Manager
                                            BuildContentItem(Setup.SourceFilePath + @"\CT" + ObjectGID.ToString() + ".png");
                                        }
                                        SW.Write("*\t" + FI.FieldType.FullName + "\t" + FI.Name + "\t" + "CT" + ObjectGID.ToString() + "\t" + FI.FieldType.IsClass.ToString() + "\t" + ObjectGID.ToString() + "\n");
                                    }
                                    else
                                        SW.Write("*\t" + FI.FieldType.FullName + "\t" + FI.Name + "\t" + T2D.Name + "\t" + FI.FieldType.IsClass.ToString() + "\t" + ObjectGID.ToString() + "\n");
                                    break;
                                case "Effect":
                                    ObjectGID = OIG.GetId(FI.GetValue(OBJ), out FT);
                                    SW.Write("*\t" + FI.FieldType.FullName + "\t" + FI.Name + "\t" + (FI.GetValue(OBJ) as Effect).Name + "\t" + FI.FieldType.IsClass.ToString() + "\t" + ObjectGID.ToString() + "\n");
                                    break;
                                //case "AudioSource":
                                //    ObjectGID = OIG.GetId(FI.GetValue(OBJ), out FT);
                                //    SW.Write("*\t" + FI.FieldType.FullName + "\t" + FI.Name + "\t" + (FI.GetValue(OBJ) as AudioSource).AudioName + "\t" + FI.FieldType.IsClass.ToString() + "\t" + ObjectGID.ToString() + "\n");
                                //    break;
                                case "String":
                                    SW.Write(FI.FieldType.FullName + "\t" + FI.Name + "\t" + FI.GetValue(OBJ) + "\t" + FI.FieldType.IsClass.ToString() + "\n");
                                    break;
                                default:
                                    SW.WriteLine(FI.GetValue(OBJ).ToString());
                                    Serialize(SW, FI.GetValue(OBJ));
                                    break;
                            }
                        }
                        else
                            SW.Write("null" + "\n");
                    }
                    else
                        SW.Write(FI.FieldType.FullName + "\t" + FI.Name + "\t" + FI.GetValue(OBJ) + "\t" + FI.FieldType.IsClass.ToString() + "\n");
                }
            }

            SW.WriteLine("SerializableProps\t" + SerializableProps.Length);
            foreach (PropertyInfo PI in SerializableProps) //Enumerable
            {
                if (PI.GetIndexParameters().Length == 0)
                {
                    if (!PI.PropertyType.IsValueType)
                    {
                        if (PI.GetValue(OBJ, null) != null)
                        {
                            bool FT = false;
                            long ObjectGID = -1;
                            switch (PI.PropertyType.Name) //Handling Some Types differently, the '*' indicates that it's not serialized normally
                            {
                                case "Texture2D":
                                    ObjectGID = OIG.GetId(PI.GetValue(OBJ), out FT);
                                    Texture2D T2D = PI.GetValue(OBJ) as Texture2D;
                                    if (T2D.Name == null) //CUstom Texture
                                    {
                                        if (FT)
                                        {
                                            using (var CreatedFile = File.Create("CT" + ObjectGID.ToString() + ".png"))
                                            {
                                                T2D.SaveAsPng(CreatedFile, T2D.Width, T2D.Height);
                                            }

                                            //Registering and Building an Item in the Content Manager
                                            BuildContentItem(Setup.SourceFilePath + @"\CT" + ObjectGID.ToString() + ".png");
                                        }

                                        SW.Write("*\t" + PI.PropertyType.FullName + "\t" + PI.Name + "\t" + "CT" + ObjectGID.ToString() + "\t" + PI.PropertyType.IsClass.ToString() + "\t" + ObjectGID.ToString() + "\n");
                                    }
                                    else
                                        SW.Write("*\t" + PI.PropertyType.FullName + "\t" + PI.Name + "\t" + T2D.Name + "\t" + PI.PropertyType.IsClass.ToString() + "\t" + ObjectGID.ToString() + "\n");
                                    break;
                                case "Effect":
                                    ObjectGID = OIG.GetId(PI.GetValue(OBJ), out FT);
                                    SW.Write("*\t" + PI.PropertyType.FullName + "\t" + PI.Name + "\t" + (PI.GetValue(OBJ) as Effect).Name + "\t" + PI.PropertyType.IsClass.ToString() + "\t" + ObjectGID.ToString() + "\n");
                                    break;
                                //case "AudioSource":
                                //    ObjectGID = OIG.GetId(PI.GetValue(OBJ), out FT);
                                //    SW.Write("*\t" + PI.PropertyType.FullName + "\t" + PI.Name + "\t" + (PI.GetValue(OBJ) as AudioSource).AudioName + "\t" + PI.PropertyType.IsClass.ToString() + "\t" + ObjectGID.ToString() + "\n");
                                //    break;
                                case "String":
                                    SW.Write(PI.PropertyType.FullName + "\t" + PI.Name + "\t" + PI.GetValue(OBJ) + "\t" + PI.PropertyType.IsClass.ToString() + "\n");
                                    break;
                                default:
                                    SW.WriteLine(PI.GetValue(OBJ).ToString());
                                    Serialize(SW, PI.GetValue(OBJ));
                                    break;
                            }
                        }
                        else
                            SW.Write("null" + "\n");
                    }
                    else
                        SW.Write(PI.PropertyType.FullName + "\t" + PI.Name + "\t" + PI.GetValue(OBJ) + "\t" + PI.PropertyType.IsClass.ToString() + "\n");
                }
            }

            SW.WriteLine("End Of " + OBJ.ToString());
        }

        //TODO:
        //1- Recognize and Add GOCs to Gos (Veware that you serialize components count, then add more (Redundancy))
        //2- Handle lists (Creation And Insertion)
        public static object Deserialize(StreamReader SR, Dictionary<long, object> SOs)
        {
            string MightBeSerialized = SR.ReadLine(); //Name Of Class
            string[] MightBeSerializedArr = MightBeSerialized.Split('\t');

            if (MightBeSerialized[0] == '-') //Serialized Before
                return GetSerializedObjectIfExist(SOs, MightBeSerializedArr[2], true);
            else if (MightBeSerializedArr[0] == "null")
                return null;

            object ActiveObject = GetInstance(MightBeSerialized);
            long GeneratedID = long.Parse(SR.ReadLine().Split('\t')[1]);

            SOs.Add(GeneratedID, ActiveObject);
            SR.ReadLine();
            foreach (FieldInfo FI in ActiveObject.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)) //Repeat this for properties :D
            {
                string Line = SR.ReadLine();

                if (Line[0] == '*') //Special handling is required
                {
                    string[] LineArr = Line.Split('\t');
                    switch (LineArr[1])
                    {
                        case "Microsoft.Xna.Framework.Graphics.Texture2D":
                            Texture2D ObjectIfExist = GetSerializedObjectIfExist(SOs, LineArr[5]) as Texture2D;
                            bool IsNull = ObjectIfExist == null;
                            ObjectIfExist = Setup.Content.Load<Texture2D>(LineArr[3]); //You Might have to handle assets in a custom folder in content folder
                            ObjectIfExist.Name = LineArr[3];

                            if (IsNull) // A new Entry
                                SOs.Add(long.Parse(LineArr[5]), ObjectIfExist);

                            FI.SetValue(ActiveObject, ObjectIfExist);
                            break;
                        //case "FN_Engine.AudioSource":
                        //    AudioSource ObjectIfExist_AS = GetSerializedObjectIfExist(SOs, LineArr[5]) as AudioSource;
                        //    bool IsNull_AS = ObjectIfExist_AS == null;
                        //    ObjectIfExist_AS = new AudioSource(LineArr[3]); //You Might have to handle assets in a custom folder in content folder

                        //    if (IsNull_AS) // A new Entry
                        //        SOs.Add(long.Parse(LineArr[5]), ObjectIfExist_AS);

                        //    FI.SetValue(ActiveObject, ObjectIfExist_AS);
                        //    break;
                        case "Microsoft.Xna.Framework.Graphics.Effect":
                            Effect ObjectIfExist_EF = GetSerializedObjectIfExist(SOs, LineArr[5]) as Effect;
                            bool IsNull_EF = ObjectIfExist_EF == null;
                            ObjectIfExist_EF = Setup.Content.Load<Effect>(LineArr[3]); //You Might have to handle assets in a custom folder in content folder

                            if (IsNull_EF) // A new Entry
                                SOs.Add(long.Parse(LineArr[5]), ObjectIfExist_EF);

                            FI.SetValue(ActiveObject, ObjectIfExist_EF);
                            break;
                    }
                }
                else if (Line[0] == '-') //Serialized Before
                    return GetSerializedObjectIfExist(SOs, MightBeSerializedArr[2], true);
                else if (Line[0] == '@') //Enumerable type
                {
                    string[] LineArr = Line.Split('\t');
                    int LoopLength = int.Parse(LineArr[3]);

                    if (LoopLength == 0)
                    {
                        FI.SetValue(ActiveObject, GetInstance(LineArr[1]));
                        continue;
                    }

                    string[] AreValueTypes = SR.ReadLine().Split('\t');

                    object EnumerableType = GetInstance(LineArr[1]);
                    switch (EnumerableType.GetType().Name)
                    {
                        case "List`1": //List<> //Do as dictionary here
                            var T_LIST = (IList)EnumerableType;
                            for (int i = 0; i < LoopLength; i++)
                            {
                                if (!bool.Parse(AreValueTypes[0]))
                                    T_LIST.Add(Deserialize(SR, SOs));
                                else
                                    T_LIST.Add(GetValueOfAValueType(SR.ReadLine().Split('\t')));
                            }
                            FI.SetValue(ActiveObject, T_LIST);
                            break;
                        case "List": //List //Do as dictionary here
                            var LIST = (IList)EnumerableType;
                            for (int i = 0; i < LoopLength; i++)
                            {
                                if (!bool.Parse(AreValueTypes[0]))
                                    LIST.Add(Deserialize(SR, SOs));
                                else
                                    LIST.Add(GetValueOfAValueType(SR.ReadLine().Split('\t')));
                            }
                            FI.SetValue(ActiveObject, LIST);
                            break;
                        //case "Queue`1": //Queue<>
                        //    var Q_LIST = (IList)EnumerableType;
                        //    for (int i = 0; i < int.Parse(LineArr[3]); i++)
                        //        Q_LIST.Add(Deserialize(SR, SOs));
                        //    FI.SetValue(ActiveObject, Q_LIST);
                        //    break;
                        //case "Queue": //Queue
                        //    var QNG_LIST = (IList)EnumerableType;
                        //    for (int i = 0; i < int.Parse(LineArr[3]); i++)
                        //        QNG_LIST.Add(Deserialize(SR, SOs));
                        //    FI.SetValue(ActiveObject, QNG_LIST);
                        //    break;
                        case "Dictionary`2": //Dictionary<,> deserialize the key and value 
                            var DICT = (IDictionary)EnumerableType;

                            for (int i = 0; i < LoopLength; i++)
                            {
                                object KEY = null;

                                if (!bool.Parse(AreValueTypes[0])) //not a value type
                                    KEY = Deserialize(SR, SOs); //Problem here!
                                else
                                    KEY = GetValueOfAValueType(SR.ReadLine().Split('\t'));

                                object VALUE = null;

                                if (!bool.Parse(AreValueTypes[1])) //not a value type
                                    VALUE = Deserialize(SR, SOs); //Problem here!
                                else
                                    VALUE = GetValueOfAValueType(SR.ReadLine().Split('\t'));

                                DICT.Add(KEY, VALUE); //Deserializing Key and value consecutively
                            }
                            FI.SetValue(ActiveObject, DICT);
                            break;
                        default:
                            var collection = (IList)EnumerableType;
                            if (collection.IsFixedSize) //Array or fixed size collection
                                for (int i = 0; i < LoopLength; i++)
                                    collection[i] = Deserialize(SR, SOs);
                            else
                                throw new Exception("Collection Type Not Yet Implemented! (Deserialization)");
                            break;
                    }
                }
                else if (FI.FieldType.IsValueType)
                {
                    string[] LineArr = Line.Split('\t');

                    if (FI.FieldType.IsEnum) //Parsing enums
                        FI.SetValue(ActiveObject, Enum.Parse(FI.FieldType, LineArr[2]));
                    else
                        FI.SetValue(ActiveObject, GetValueOfAValueType(LineArr));
                }
                else if (Line == "null")
                    FI.SetValue(ActiveObject, null);
                else if (FI.FieldType.Name == "String")
                    FI.SetValue(ActiveObject, Line.Split('\t')[2]);
                else
                    FI.SetValue(ActiveObject, Deserialize(SR, SOs));
            }

            //Properties :D
            SR.ReadLine();
            foreach (PropertyInfo PI in ActiveObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) //Repeat this for properties :D
            {
                string Line = SR.ReadLine();

                if (Line[0] == '*') //Special handling is required
                {
                    string[] LineArr = Line.Split('\t');
                    switch (LineArr[1])
                    {
                        case "Microsoft.Xna.Framework.Graphics.Texture2D":
                            Texture2D ObjectIfExist = GetSerializedObjectIfExist(SOs, LineArr[5]) as Texture2D;
                            bool IsNull = ObjectIfExist == null;
                            ObjectIfExist = Setup.Content.Load<Texture2D>(LineArr[3]); //You Might have to handle assets in a custom folder in content folder
                            ObjectIfExist.Name = LineArr[3];

                            if (IsNull) // A new Entry
                                SOs.Add(long.Parse(LineArr[5]), ObjectIfExist);

                            PI.SetValue(ActiveObject, ObjectIfExist);
                            break;
                        //case "FN_Engine.AudioSource":
                        //    AudioSource ObjectIfExist_AS = GetSerializedObjectIfExist(SOs, LineArr[5]) as AudioSource;
                        //    bool IsNull_AS = ObjectIfExist_AS == null;
                        //    ObjectIfExist_AS = new AudioSource(LineArr[3]); //You Might have to handle assets in a custom folder in content folder

                        //    if (IsNull_AS) // A new Entry
                        //        SOs.Add(long.Parse(LineArr[5]), ObjectIfExist_AS);

                        //    FI.SetValue(ActiveObject, ObjectIfExist_AS);
                        //    break;
                        case "Microsoft.Xna.Framework.Graphics.Effect":
                            Effect ObjectIfExist_EF = GetSerializedObjectIfExist(SOs, LineArr[5]) as Effect;
                            bool IsNull_EF = ObjectIfExist_EF == null;
                            ObjectIfExist_EF = Setup.Content.Load<Effect>(LineArr[3]); //You Might have to handle assets in a custom folder in content folder

                            if (IsNull_EF) // A new Entry
                                SOs.Add(long.Parse(LineArr[5]), ObjectIfExist_EF);

                            PI.SetValue(ActiveObject, ObjectIfExist_EF);
                            break;
                    }
                }
                else if (Line[0] == '-') //Serialized Before
                    return GetSerializedObjectIfExist(SOs, MightBeSerializedArr[2], true);
                else if (Line[0] == '@') //Enumerable type
                {
                    string[] LineArr = Line.Split('\t');
                    int LoopLength = int.Parse(LineArr[3]);

                    if (LoopLength == 0)
                    {
                        PI.SetValue(ActiveObject, GetInstance(LineArr[1]));
                        continue;
                    }

                    string[] AreValueTypes = SR.ReadLine().Split('\t');

                    object EnumerableType = GetInstance(LineArr[1]);
                    switch (EnumerableType.GetType().Name)
                    {
                        case "List`1": //List<> //Do as dictionary here
                            var T_LIST = (IList)EnumerableType;
                            for (int i = 0; i < int.Parse(LineArr[3]); i++)
                            {
                                if (!bool.Parse(AreValueTypes[0]))
                                    T_LIST.Add(Deserialize(SR, SOs));
                                else
                                    T_LIST.Add(GetValueOfAValueType(SR.ReadLine().Split('\t')));
                            }
                            PI.SetValue(ActiveObject, T_LIST);
                            break;
                        case "List": //List //Do as dictionary here
                            var LIST = (IList)EnumerableType;
                            for (int i = 0; i < int.Parse(LineArr[3]); i++)
                            {
                                if (!bool.Parse(AreValueTypes[0]))
                                    LIST.Add(Deserialize(SR, SOs));
                                else
                                    LIST.Add(GetValueOfAValueType(SR.ReadLine().Split('\t')));
                            }
                            PI.SetValue(ActiveObject, LIST);
                            break;
                        //case "Queue`1": //Queue<>
                        //    var Q_LIST = (IList)EnumerableType;
                        //    for (int i = 0; i < int.Parse(LineArr[3]); i++)
                        //        Q_LIST.Add(Deserialize(SR, SOs));
                        //    FI.SetValue(ActiveObject, Q_LIST);
                        //    break;
                        //case "Queue": //Queue
                        //    var QNG_LIST = (IList)EnumerableType;
                        //    for (int i = 0; i < int.Parse(LineArr[3]); i++)
                        //        QNG_LIST.Add(Deserialize(SR, SOs));
                        //    FI.SetValue(ActiveObject, QNG_LIST);
                        //    break;
                        case "Dictionary`2": //Dictionary<,> deserialize the key and value 
                            var DICT = (IDictionary)EnumerableType;

                            for (int i = 0; i < int.Parse(LineArr[3]); i++)
                            {
                                object KEY = null;

                                if (!bool.Parse(AreValueTypes[0])) //not a value type
                                    KEY = Deserialize(SR, SOs); //Problem here!
                                else
                                    KEY = GetValueOfAValueType(SR.ReadLine().Split('\t'));

                                object VALUE = null;

                                if (!bool.Parse(AreValueTypes[1])) //not a value type
                                    VALUE = Deserialize(SR, SOs); //Problem here!
                                else
                                    VALUE = GetValueOfAValueType(SR.ReadLine().Split('\t'));

                                DICT.Add(KEY, VALUE); //Deserializing Key and value consecutively
                            }
                            PI.SetValue(ActiveObject, DICT);
                            break;
                        default:
                            var collection = (IList)EnumerableType;
                            if (collection.IsFixedSize) //Array or fixed size collection
                                for (int i = 0; i < int.Parse(LineArr[3]); i++)
                                    collection[i] = Deserialize(SR, SOs);
                            else
                                throw new Exception("Collection Type Not Yet Implemented! (Deserialization)");
                            break;
                    }
                }
                else if (PI.PropertyType.IsValueType)
                {
                    string[] LineArr = Line.Split('\t');

                    if (PI.PropertyType.IsEnum) //Parsing enums
                        PI.SetValue(ActiveObject, Enum.Parse(PI.PropertyType, LineArr[2]));
                    else
                        PI.SetValue(ActiveObject, GetValueOfAValueType(LineArr));
                }
                else if (Line == "null")
                    PI.SetValue(ActiveObject, null);
                else if (PI.PropertyType.Name == "String")
                    PI.SetValue(ActiveObject, Line.Split('\t')[2]);
                else
                    PI.SetValue(ActiveObject, Deserialize(SR, SOs));
            }

            SR.ReadLine();

            return ActiveObject;
        }

        private static object GetSerializedObjectIfExist(Dictionary<long, object> SOs, string Key, bool SpecialHandling = false)
        {
            object SerializedObject = null;

            if (SOs.ContainsKey(long.Parse(Key)))
                SerializedObject = SOs[long.Parse(Key)]; //Check if there is a failure?

            return SerializedObject;
        }

        private static object GetValueOfAValueType(string[] LineArr)
        {
            switch (LineArr[0])
            {
                case "System.Single": //float
                    return float.Parse(LineArr[2]);
                case "System.Double": //double
                    return double.Parse(LineArr[2]);
                case "System.Int32": //int
                    return int.Parse(LineArr[2]);
                case "System.Int16": //short
                    return short.Parse(LineArr[2]);
                case "System.Int64": //long
                    return long.Parse(LineArr[2]);
                case "System.Boolean": //bool
                    return bool.Parse(LineArr[2]);
                case "Microsoft.Xna.Framework.Point": //Point
                    return STR_To_Point(LineArr[2]);
                case "Microsoft.Xna.Framework.Vector2": //vector2
                    return STR_To_Vector2(LineArr[2]);
                case "Microsoft.Xna.Framework.Vector3": //vector3
                    return STR_To_Vector3(LineArr[2]);
                case "Microsoft.Xna.Framework.Vector4": //vector4
                    return STR_To_Vector4(LineArr[2]);
                case "Microsoft.Xna.Framework.Color": //Color
                    return STR_To_Color(LineArr[2]);
                case "Microsoft.Xna.Framework.Rectangle": //Rectangle
                    return STR_To_Rect(LineArr[2]);
                default:
                    throw new Exception("Type Not Handled");
            }
        }

        public static void BuildContentItem(string Path) //Path should include the name of the asset (Relative to the Content Folder)
        {
            //Registering and Building an Item in the Content Manager
            Setup.PM.RegisterContent(Path);
            PipelineBuildEvent T = null;
            T = Setup.PM.BuildContent(Path); // Excpetion "System.Reflection.ReflectionTypeLoadException" occurs here!
            Setup.PM.ProcessContent(T);
            
        }

        public static void BuildAllContent(string CurrentDirectory)
        {
            foreach (string F in Directory.GetFiles(CurrentDirectory)) //Building Items that are buildable
            {
                string[] FileSplit = F.Split('\\');
                string TexName = FileSplit[FileSplit.Length - 1];

                if (TexRegex.IsMatch(TexName) || MusicRegex.IsMatch(TexName) || ShaderRegex.IsMatch(TexName) || FontRegex.IsMatch(TexName))
                    BuildContentItem(F);
            }

            foreach (string Dir in Directory.GetDirectories(CurrentDirectory))
                if (!Dir.Equals(CurrentDirectory + @"\bin") || Dir.Equals(CurrentDirectory + @"\obj"))
                    BuildAllContent(Dir);
        }

        public static string UniqueGameObjectName(string BaseName) // If base name is not present in the scene, then base name is returned
        {
            string UniqueName = BaseName;

            bool Duplicate = false;
            if (NameFormatRegex.IsMatch(BaseName))
                Duplicate = true;

            GameObject[] GOs = null;
            int SearchNumber = 0;

            if (Duplicate)
            {
                string[] SearchingName = BaseName.Remove(BaseName.Length - 1, 1).Split('(');
                SearchNumber = int.Parse(SearchingName[SearchingName.Length - 1]) + 1;

                GOs = SceneManager.ActiveScene.GameObjects.Where(Item => GetBaseName(Item.Name) == GetBaseName(UniqueName)).OrderBy(Item => GetSearchNumber(Item.Name)).ToArray();
            }
            else
                GOs = SceneManager.ActiveScene.GameObjects.Where(Item => (Item.Name.StartsWith(BaseName) && NameRegex.IsMatch(Item.Name.Remove(0, BaseName.Length))) || Item.Name == BaseName).OrderBy(Item => GetSearchNumber(Item.Name)).ToArray();

            for (int i = 0; i < GOs.Length; i++)
            {
                if (GOs[i].Name != BaseName)
                {
                    string[] ProcessingName = GOs[i].Name.Remove(GOs[i].Name.Length - 1, 1).Split('(');
                    int Number = int.Parse(ProcessingName[ProcessingName.Length - 1]);

                    if (SearchNumber == Number)
                        SearchNumber++;
                    else if (!Duplicate)
                        break; //?
                }
                else if (!Duplicate)
                    SearchNumber++;
            }

            if (!Duplicate && SearchNumber != 0)
                UniqueName = BaseName + " (" + SearchNumber.ToString() + ")";
            else if (Duplicate)
            {
                string[] ProcessingName = UniqueName.Remove(UniqueName.Length - 1, 1).Split('(');
                UniqueName = "";
                for (int i = 0; i < ProcessingName.Length - 1; i++)
                    UniqueName = ProcessingName[i] + "(";
                UniqueName += SearchNumber.ToString() + ")";
            }

            return UniqueName;
        }

        public static string UniqueName(string BaseName, string[] NamePool) // If base name is not present in the scene, then base name is returned
        {
            string UniqueName = BaseName;

            bool Duplicate = false;
            if (NameFormatRegex.IsMatch(BaseName))
                Duplicate = true;

            string[] Names = null;
            int SearchNumber = 0;

            if (Duplicate)
            {
                string[] SearchingName = BaseName.Remove(BaseName.Length - 1, 1).Split('(');
                SearchNumber = int.Parse(SearchingName[SearchingName.Length - 1]) + 1;

                Names = NamePool.Where(Item => GetBaseName(Item) == GetBaseName(UniqueName)).OrderBy(Item => GetSearchNumber(Item)).ToArray();
            }
            else
                Names = NamePool.Where(Item => (Item.StartsWith(BaseName) && NameRegex.IsMatch(Item.Remove(0, BaseName.Length))) || Item == BaseName).OrderBy(Item => GetSearchNumber(Item)).ToArray();

            for (int i = 0; i < Names.Length; i++)
            {
                if (Names[i] != BaseName)
                {
                    string[] ProcessingName = Names[i].Remove(Names[i].Length - 1, 1).Split('(');
                    int Number = int.Parse(ProcessingName[ProcessingName.Length - 1]);

                    if (SearchNumber == Number)
                        SearchNumber++;
                    else if (!Duplicate)
                        break; //?
                }
                else if (!Duplicate)
                    SearchNumber++;
            }

            if (!Duplicate && SearchNumber != 0)
                UniqueName = BaseName + " (" + SearchNumber.ToString() + ")";
            else if (Duplicate)
            {
                string[] ProcessingName = UniqueName.Remove(UniqueName.Length - 1, 1).Split('(');
                UniqueName = "";
                for (int i = 0; i < ProcessingName.Length - 1; i++)
                    UniqueName = ProcessingName[i] + "(";
                UniqueName += SearchNumber.ToString() + ")";
            }

            return UniqueName;
        }

        private static string GetBaseName(string Name)
        {
            string BaseName = "";

            if (Name[Name.Length - 1] != ')')
                return "";

            string[] SearchingName = Name.Remove(Name.Length - 1, 1).Split('(');
            int SearchNumber = int.Parse(SearchingName[SearchingName.Length - 1]) + 1;

            BaseName = Name.Remove(Name.Length - 3 - SearchingName[SearchingName.Length - 1].Length, 3 + SearchingName[SearchingName.Length - 1].Length);

            return BaseName;
        }

        private static int GetSearchNumber(string Name)
        {
            if (Name[Name.Length - 1] != ')')
                return -1;

            string[] SearchingName = Name.Remove(Name.Length - 1, 1).Split('(');

            return int.Parse(SearchingName[SearchingName.Length - 1]);
        }
    }
}