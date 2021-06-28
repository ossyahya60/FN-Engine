using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Newtonsoft.Json;

namespace FN_Engine
{
    //Not entirely sure if it should be a gameobject component or not
    public class Sprite
    {
        public Texture2D Texture
        {
            set
            {
                texture = value;
                SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                Origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            }
            get
            {
                return texture;
            }
        }
        public Rectangle SourceRectangle;  //P.S: scale??
        public Vector2 Origin;
        public Transform Transform;

        private Texture2D texture;

        public Sprite(Transform transform)
        {
            Transform = transform;
            Origin = Vector2.Zero;
            SourceRectangle = new Rectangle();
            texture = null;
        }

        public Sprite()
        {
            Origin = Vector2.Zero;
            SourceRectangle = new Rectangle();
            texture = null;
        }

        public void LoadTexture(string path)
        {
            Texture = Setup.Content.Load<Texture2D>(path);
        }

        public Rectangle DynamicScaledRect()
        {
            Rectangle HandyRectangle = Rectangle.Empty;

            HandyRectangle.X = (int)(Transform.Position.X - Origin.X * Transform.Scale.X);
            HandyRectangle.Y = (int)(Transform.Position.Y - Origin.Y * Transform.Scale.Y);
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
            Clone.texture = texture;

            return Clone;
        }

        public void SetCenterAsOrigin()
        {
            Origin = new Vector2(SourceRectangle.Width / 2, SourceRectangle.Height / 2);
        }

        public void Serialize(StreamWriter SW, string GameObjectName) //Make the spriterender pass the transform to the sprite in deserialization
        {
            SW.WriteLine(ToString());
            
            SW.Write("SourceRectangle:\t" + SourceRectangle.X.ToString() + "\t" + SourceRectangle.Y.ToString() + "\t" + SourceRectangle.Width.ToString() + "\t" + SourceRectangle.Height.ToString() + "\n");
            SW.Write("Origin:\t" + Origin.X.ToString() + "\t" + Origin.Y.ToString() + "\n");
            if (texture != null)
            {
                if (texture.Name == null)
                {
                    texture.SaveAsPng(File.Create("Content/" + GameObjectName + ".png"), texture.Width, texture.Height);
                    SW.Write("texture:\t" + "Content/" + GameObjectName + ".png" + "\n");
                }
                else
                    SW.Write("texture:\t" + texture.Name + "\n");
            }
            else
                SW.Write("texture:\t" + "null\n");

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
            if (TexString == "null") //Should I create a default texture instead?
                texture = HitBoxDebuger.RectTexture(Color.White);
            else if (TexString.Contains(".png")) //Custom texture that is serialized
            {
                using (var fileStream = new FileStream(TexString, FileMode.Open))
                {
                    texture = Texture2D.FromStream(Setup.GraphicsDevice, fileStream);
                }
            }
            else
                texture = Setup.Content.Load<Texture2D>(TexString);

            SR.ReadLine();
        }
    }
}
