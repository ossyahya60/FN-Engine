using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public static class Utility
    {
        public static ObjectIDGenerator OIG = new ObjectIDGenerator();
        //public static readonly Type[] SupportedTypes = { typeof(float), typeof(int), typeof(string), typeof(Vector2), typeof(bool) };

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
            Color Output = Color.TransparentBlack;

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

        public static object GetInstance(string ObjectType)
        {
            Type T = Type.GetType(ObjectType);
            return Activator.CreateInstance(T);
        }

        public static void Serialize(StreamWriter SW, object OBJ)  //Make this the default one
        {
            bool FirstTime = false;
            long GID = OIG.GetId(OBJ, out FirstTime);

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
                            if(SendOneTime)
                            {
                                SW.WriteLine(item.GetType().IsValueType.ToString());
                                SendOneTime = false;
                            }

                            if(item.GetType().IsValueType)
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
                                            Setup.PM.RegisterContent("CT" + ObjectGID.ToString() + ".png");
                                            var T = Setup.PM.BuildContent(@"C:\MyEngine\MyEngine\MyEngine\Content\" + "CT" + ObjectGID.ToString() + ".png");
                                            Setup.PM.ProcessContent(T);
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
                                            Setup.PM.RegisterContent("CT" + ObjectGID.ToString() + ".png");
                                            var T = Setup.PM.BuildContent(@"C:\MyEngine\MyEngine\MyEngine\Content\" + "CT" + ObjectGID.ToString() + ".png");
                                            Setup.PM.ProcessContent(T);
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

                            if (IsNull) // A new Entry
                                SOs.Add(long.Parse(LineArr[5]), ObjectIfExist);

                            FI.SetValue(ActiveObject, ObjectIfExist);
                            break;
                        //case "MyEngine.AudioSource":
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

                    if(LoopLength == 0)
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
                else if (FI.FieldType.Name == "String")
                    FI.SetValue(ActiveObject, Line.Split('\t')[2]);
                else if (Line == "null")
                    FI.SetValue(ActiveObject, null);
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

                            if (IsNull) // A new Entry
                                SOs.Add(long.Parse(LineArr[5]), ObjectIfExist);

                            PI.SetValue(ActiveObject, ObjectIfExist);
                            break;
                        //case "MyEngine.AudioSource":
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
                else if (PI.PropertyType.Name == "String")
                    PI.SetValue(ActiveObject, Line.Split('\t')[2]);
                else if (Line == "null")
                    PI.SetValue(ActiveObject, null);
                else
                    PI.SetValue(ActiveObject, Deserialize(SR, SOs));
            }

            SR.ReadLine();

            return ActiveObject;
        }

        private static object GetSerializedObjectIfExist(Dictionary<long, object> SOs, string Key, bool SpecialHandling = false)
        {
            object SerializedObject = null;
            try
            {
                SerializedObject = SOs[long.Parse(Key)]; //Check if there is a failure?
            }
            catch (KeyNotFoundException)
            {
                if (SpecialHandling)
                    throw new KeyNotFoundException("This case shouldn't happen, check special deserialized objects");
            }

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
    }
}
