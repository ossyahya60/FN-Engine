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
        public static ObjectIDGenerator Texture_OIG = new ObjectIDGenerator();

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
                    foreach (var item in (IEnumerable)FI.GetValue(OBJ))
                    {
                        Serialize(SW, item);
                    }
                }
                else
                {
                    if (!FI.FieldType.IsValueType && FI.GetValue(OBJ) != null)
                    {
                        switch(FI.Name) //Handling Some Types differently, the '*' indicates that it's not serialized normally
                        {
                            case "Texture":
                                Texture2D T2D = FI.GetValue(OBJ) as Texture2D;
                                if (T2D.Name == null) //CUstom Texture
                                {
                                    bool FT = false;
                                    long TOIG = Texture_OIG.GetId(T2D, out FT);
                                    if(FT)
                                        T2D.SaveAsPng(File.Create("CT" + TOIG.ToString() + ".png"), T2D.Width, T2D.Height);
                                    SW.Write("*\t" + FI.FieldType.FullName + "\t" + FI.Name + "\t" + "CT" + TOIG.ToString() + ".png\t" + FI.FieldType.IsClass.ToString() + "\n");
                                }
                                else
                                    SW.Write("*\t" + FI.FieldType.FullName + "\t" + FI.Name + "\t" + T2D.Name + ".png\t" + FI.FieldType.IsClass.ToString() + "\n");
                                break;
                            case "Effect":
                                SW.Write("*\t" + FI.FieldType.FullName + "\t" + FI.Name + "\t" + (FI.GetValue(OBJ) as Effect).Name + "\t" + FI.FieldType.IsClass.ToString() + "\n");
                                break;
                            case "AudioSource":
                                SW.Write("*\t" + FI.FieldType.FullName + "\t" + FI.Name + "\t" + (FI.GetValue(OBJ) as AudioSource).AudioName + "\t" + FI.FieldType.IsClass.ToString() + "\n");
                                break;
                            default:
                                Serialize(SW, FI.GetValue(OBJ));
                                break;
                        }
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
                    if (!PI.PropertyType.IsValueType && PI.GetValue(OBJ, null) != null)
                    {
                        switch (PI.Name) //Handling Some Types differently, the '*' indicates that it's not serialized normally
                        {
                            case "Texture":
                                Texture2D T2D = PI.GetValue(OBJ) as Texture2D;
                                if (T2D.Name == null) //CUstom Texture
                                {
                                    bool FT = false;
                                    long TOIG = Texture_OIG.GetId(T2D, out FT);
                                    if (FT)
                                        T2D.SaveAsPng(File.Create("CT" + TOIG.ToString() + ".png"), T2D.Width, T2D.Height);
                                    SW.Write("*\t" + PI.PropertyType.FullName + "\t" + PI.Name + "\t" + "CT" + TOIG.ToString() + ".png\t" + PI.PropertyType.IsClass.ToString() + "\n");
                                }
                                else
                                    SW.Write("*\t" + PI.PropertyType.FullName + "\t" + PI.Name + "\t" + T2D.Name + ".png\t" + PI.PropertyType.IsClass.ToString() + "\n");
                                break;
                            case "Effect":
                                SW.Write("*\t" + PI.PropertyType.FullName + "\t" + PI.Name + "\t" + (PI.GetValue(OBJ) as Effect).Name + "\t" + PI.PropertyType.IsClass.ToString() + "\n");
                                break;
                            case "AudioSource":
                                SW.Write("*\t" + PI.PropertyType.FullName + "\t" + PI.Name + "\t" + (PI.GetValue(OBJ) as AudioSource).AudioName + "\t" + PI.PropertyType.IsClass.ToString() + "\n");
                                break;
                            default:
                                Serialize(SW, PI.GetValue(OBJ));
                                break;
                        }
                    }
                    else
                        SW.Write(PI.PropertyType.FullName + "\t" + PI.Name + "\t" + PI.GetValue(OBJ) + "\t" + PI.PropertyType.IsClass.ToString() + "\n");
                }
            }

            SW.WriteLine("End Of " + OBJ.ToString());
        }
    }
}
