using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace FN_Engine
{
    public class SpriteRenderer: GameObjectComponent
    {
        public static Effect LastEffect;

        public string TextureName //Don't change this name! (It's used in content window in drag and drop)
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                if (Sprite == null)
                {
                    Sprite = new Sprite(gameObject.Transform);
                    Sprite.LoadTexture("Default Textures\\DefaultTexture");
                }

                //if (FN_Editor.EditorScene.IsThisTheEditor)
                //{
                //    if (FN_Editor.ContentWindow.DraggedAsset != null && FN_Editor.ContentWindow.DraggedAsset is KeyValuePair<string, Rectangle>)
                //    {
                //        KeyValuePair<string, Rectangle> DraggedSubTex = (KeyValuePair<string, Rectangle>)FN_Editor.ContentWindow.DraggedAsset;

                //        Sprite.LoadTexture(DraggedSubTex.Key);
                //        Sprite.SourceRectangle = new Rectangle((int)(Sprite.Texture.Width * DraggedSubTex.Value.X / 10000.0f), (int)(Sprite.Texture.Height * DraggedSubTex.Value.Y / 10000.0f), (int)(Sprite.Texture.Width * DraggedSubTex.Value.Width / 10000.0f), (int)(Sprite.Texture.Height * DraggedSubTex.Value.Height / 10000.0f));

                //        return;
                //    }
                //}

                if (value == Sprite.Texture.Name)
                    return;

                Sprite.LoadTexture(value);
            }
            get
            {
                if (Sprite != null && Sprite.Texture != null)
                    return Sprite.Texture.Name;
                else
                    return null;
            }
        }

        public Sprite Sprite { private set; get; } 
        public SpriteEffects SpriteEffects;
        public Color Color = Color.White;
        public Effect Effect;
        public Rectangle SourceRectangle //Don't change this name! (It's used in content window in drag and drop)
        {
            set
            {
                if (Sprite != null)
                {
                    Sprite.SourceRectangle = value;
                    Sprite.Origin = new Vector2(value.Width * 0.5f, value.Height * 0.5f);
                }
            }
            get
            {
                if (Sprite == null)
                    return Rectangle.Empty;

                return Sprite.SourceRectangle;
            }
        }

        static SpriteRenderer()
        {
            LastEffect = null;
        }

        public SpriteRenderer()
        {
            //Color = Color.White;
            SpriteEffects = SpriteEffects.None;
            Effect = null;
        }

        public override void Start()
        {
            if(string.IsNullOrEmpty(TextureName))
                TextureName = "Default Textures\\DefaultTexture";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Sprite != null && Sprite.Texture != null)
            {
                if (LastEffect != Effect)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, Effect, Setup.Camera.GetViewTransformationMatrix());
                    LastEffect = Effect;
                }

                Transform T = gameObject.Transform.AdjustedTransform;

                if (T == null)
                    return;

                Rectangle DestRect = Rectangle.Empty;
                DestRect.Location = T.Position.ToPoint();
                DestRect.Width = (int)(Sprite.SourceRectangle.Width * T.Scale.X);
                DestRect.Height = (int)(Sprite.SourceRectangle.Height * T.Scale.Y);
                spriteBatch.Draw(Sprite.Texture, DestRect, Sprite.SourceRectangle, Color, T.Rotation, Sprite.Origin, SpriteEffects, gameObject.Layer);
            }
        }

        public override GameObjectComponent DeepCopy(GameObject clone)
        {
            SpriteRenderer Clone = MemberwiseClone() as SpriteRenderer;
            Clone.Sprite = Sprite.DeepCopy(clone);
            Clone.Sprite.Transform = clone.Transform;
            Clone.Effect = (Effect == null) ? null : Effect.Clone();

            return Clone;
        }

        public override void Serialize(StreamWriter SW)
        {
            SW.WriteLine(ToString());

            base.Serialize(SW);
            if (Sprite != null)
                Sprite.Serialize(SW, gameObject.Name);
            else
                SW.WriteLine("null\n"); // Make a check in deserialization
            SW.Write("SpriteEffects:\t" + SpriteEffects.ToString() + "\n");
            SW.Write("Color:\t" + Color.R.ToString() + "\t" + Color.G.ToString() + "\t" + Color.B.ToString() + "\t" + Color.A.ToString() + "\n");
            if (Effect != null)
                SW.Write("Effect:\t" + Effect.Name + "\n");
            else
                SW.Write("Effect:\t" + "null\n");

            SW.WriteLine("End Of " + ToString());
        }

        public override void Deserialize(StreamReader SR)
        {
            //SR.ReadLine();

            base.Deserialize(SR);

            char[] IsThisNull = new char[4];
            SR.Read(IsThisNull, 0, 4);

            if (IsThisNull.ToString() != "null")
            {
                Sprite = new Sprite(gameObject.Transform);
                Sprite.Deserialize(SR);
            }

            SpriteEffects = (SpriteEffects)System.Enum.Parse(SpriteEffects.GetType(), SR.ReadLine().Split('\t')[1]); //Test this
            string[] COLOR = SR.ReadLine().Split('\t');
            Color = new Color(byte.Parse(COLOR[1]), byte.Parse(COLOR[2]), byte.Parse(COLOR[3]), byte.Parse(COLOR[4]));
            string[] effect = SR.ReadLine().Split('\t');
            if (effect[1] == "null")
                Effect = null;
            else
                Effect = Setup.Content.Load<Effect>(effect[1]);

            SR.ReadLine();
        }
    }
}
