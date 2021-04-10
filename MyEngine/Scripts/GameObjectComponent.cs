using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Reflection;

namespace MyEngine
{
    public class GameObjectComponent
    {
        public GameObject gameObject;  //Every component belongs to a certain GameObject
        public bool Enabled = true;

        public GameObjectComponent()
        {

        }

        public virtual void Start() //Called once per component
        {
            
        }

        public virtual void Update(GameTime gameTime)  //Called every frame
        {
            
        }

        public virtual void LateUpdate(GameTime gameTime)  //Called every frame after Update()
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)  //Called every frame after "Update()" execution
        {

        }

        public virtual void DrawUI() //Called every frame (For UI Purposes)
        {

        }

        //Not used now...
        public virtual void Serialize(StreamWriter SW)
        {
            SW.Write("gameObject:\t" + gameObject.Name + "\n");
            SW.Write("Enabled:\t" + Enabled.ToString() + "\n");
        }

        //Not used now...
        public virtual void Deserialize(StreamReader SR)
        {
            SR.ReadLine();
            Enabled = bool.Parse(SR.ReadLine().Split('\t')[1]);
        }

        //public virtual void Deserialize(StreamReader SR)
        //{
        //    int FieldsCount = int.Parse(SR.ReadLine().Split('\t')[1]);

        //    List<FieldInfo> fieldInfos = new List<FieldInfo>(FieldsCount);

        //    for (int i = 0; i < FieldsCount; i++)
        //    {
        //        string[] Line = SR.ReadLine().Split('\t');

        //        FieldInfo FI = GetType().GetField(Line[1], BindingFlags.Public | BindingFlags.Instance);

        //        switch (FI.DeclaringType.FullName) //Here, I handle basic types
        //        {
        //            case "System.Single": //float
        //                FI.SetValue(this, float.Parse(Line[2]));
        //                fieldInfos.Add(FI);
        //                break;
        //            case "System.Double": //double
        //                FI.SetValue(this, double.Parse(Line[2]));
        //                fieldInfos.Add(FI);
        //                break;
        //            case "System.Int32": //int 32
        //                FI.SetValue(this, int.Parse(Line[2]));
        //                fieldInfos.Add(FI);
        //                break;
        //            case "System.Int16": //int 16
        //                FI.SetValue(this, short.Parse(Line[2]));
        //                fieldInfos.Add(FI);
        //                break;
        //            case "System.Int64": //int 64
        //                FI.SetValue(this, long.Parse(Line[2]));
        //                fieldInfos.Add(FI);
        //                break;
        //            case "System.UInt32": //uint 32
        //                FI.SetValue(this, uint.Parse(Line[2]));
        //                fieldInfos.Add(FI);
        //                break;
        //            case "System.UInt16": //uint 16
        //                FI.SetValue(this, ushort.Parse(Line[2]));
        //                fieldInfos.Add(FI);
        //                break;
        //            case "System.UInt64": //uint 64
        //                FI.SetValue(this, ulong.Parse(Line[2]));
        //                fieldInfos.Add(FI);
        //                break;
        //            case "System.Boolean": //bool
        //                FI.SetValue(this, bool.Parse(Line[2]));
        //                fieldInfos.Add(FI);
        //                break;
        //            case "Microsoft.Xna.Framework.Vector2": //Vector2
        //                FI.SetValue(this, Utility.STR_To_Vector2(Line[2]));
        //                fieldInfos.Add(FI);
        //                break;
        //            case "Microsoft.Xna.Framework.Vector3": //Vector3
        //                FI.SetValue(this, Utility.STR_To_Vector3(Line[2]));
        //                fieldInfos.Add(FI);
        //                break;
        //            case "Microsoft.Xna.Framework.Vector4": //Vector4
        //                FI.SetValue(this, Utility.STR_To_Vector4(Line[2]));
        //                fieldInfos.Add(FI);
        //                break;
        //            case "Microsoft.Xna.Framework.Color": //Color
        //                FI.SetValue(this, Utility.STR_To_Color(Line[2]));
        //                fieldInfos.Add(FI);
        //                break;
        //            default:
        //                Enum EnumCandidate = Utility.GetInstance(FI.FieldType.FullName) as Enum;

        //                if(EnumCandidate != null)
        //                    FI.SetValue(this, Enum.Parse(FI.FieldType, Line[2]));
        //                else
        //                    FI.SetValue(this, null);
        //                fieldInfos.Add(FI);
        //                break;
        //        }
        //    }

        //    FN_Editor.InspectorWindow.Members.Add(fieldInfos);

        //    SR.ReadLine();
        //}

        public virtual void Destroy()
        {
            //gameObject.RemoveComponent<GameObjectComponent>(this);
        }

        public virtual GameObjectComponent DeepCopy(GameObject Clone)
        {
            return this.MemberwiseClone() as GameObjectComponent;
        }
    }
}
