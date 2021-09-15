using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System;

namespace FN_Engine
{
    //Not entirely sure if it should be a gameobject component or not
    public class Sprite
    {
        public Texture2D Texture;
        public Rectangle SourceRectangle;  //P.S: scale??
        public Vector2 Origin;
        public Transform Transform;

        public Sprite(Transform transform)
        {
            Transform = transform;
            Origin = Vector2.One * 0.5f;
            SourceRectangle = new Rectangle();
            Texture = null;
        }

        public Sprite()
        {
            Origin = Vector2.One * 0.5f;
            SourceRectangle = new Rectangle();
            Texture = null;
        }

        public void LoadTexture(string path)
        {
            try
            { 
                Texture = Setup.Content.Load<Texture2D>(path);
                SourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            }
            catch (Exception E){ Utility.Log(E.Message); } //Log Error maybe?
        }

        public Rectangle DynamicScaledRect()
        {
            Rectangle HandyRectangle = Rectangle.Empty;

            HandyRectangle.X = (int)(Transform.Position.X - Origin.X * Transform.Scale.X * SourceRectangle.Width);
            HandyRectangle.Y = (int)(Transform.Position.Y - Origin.Y * Transform.Scale.Y * SourceRectangle.Height);
            HandyRectangle.Width = (int)(SourceRectangle.Width * Transform.Scale.X);
            HandyRectangle.Height = (int)(SourceRectangle.Height * Transform.Scale.Y);

            return HandyRectangle;
        }

        public Sprite DeepCopy(GameObject clone)
        {
            Sprite Clone = this.MemberwiseClone() as Sprite;
            Clone.SourceRectangle = new Rectangle(SourceRectangle.Location, SourceRectangle.Size);
            Clone.Origin = new Vector2(Origin.X, Origin.Y);
            Clone.Transform = clone.Transform;
            Clone.Texture = Texture;

            return Clone;
        }

        public void Serialize(StreamWriter SW, string GameObjectName) //Make the spriterender pass the transform to the sprite in deserialization
        {
            SW.WriteLine(ToString());
            
            SW.Write("SourceRectangle:\t" + SourceRectangle.X.ToString() + "\t" + SourceRectangle.Y.ToString() + "\t" + SourceRectangle.Width.ToString() + "\t" + SourceRectangle.Height.ToString() + "\n");
            SW.Write("Origin:\t" + Origin.X.ToString() + "\t" + Origin.Y.ToString() + "\n");
            if (Texture != null)
            {
                if (Texture.Name == null)
                {
                    Texture.SaveAsPng(File.Create("Content/" + GameObjectName + ".png"), Texture.Width, Texture.Height);
                    SW.Write("Texture:\t" + "Content/" + GameObjectName + ".png" + "\n");
                }
                else
                    SW.Write("Texture:\t" + Texture.Name + "\n");
            }
            else
                SW.Write("Texture:\t" + "null\n");

            SW.WriteLine("End Of " + ToString());
        }

        public void Deserialize(StreamReader SR)
        {
            SR.ReadLine();

            string[] SourceRect = SR.ReadLine().Split('\t');
            SourceRectangle = new Rectangle(int.Parse(SourceRect[1]), int.Parse(SourceRect[2]), int.Parse(SourceRect[3]), int.Parse(SourceRect[4]));
            string[] origin = SR.ReadLine().Split('\t');
            Origin = new Vector2(float.Parse(origin[1]), float.Parse(origin[2]));
            string TexString = SR.ReadLine().Split('\t')[1];
            if (TexString == "null") //Should I create a default Texture instead?
                Texture = HitBoxDebuger.RectTexture(Color.White);
            else if (TexString.Contains(".png")) //Custom Texture that is serialized
            {
                using (var fileStream = new FileStream(TexString, FileMode.Open))
                {
                    Texture = Texture2D.FromStream(Setup.GraphicsDevice, fileStream);
                }
            }
            else
                Texture = Setup.Content.Load<Texture2D>(TexString);

            SR.ReadLine();
        }
    }
}
