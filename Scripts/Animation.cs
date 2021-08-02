using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FN_Engine
{
    public class Frame
    {
        public Texture2D Tex = null;
        public float Time = 0.25f;
        public Microsoft.Xna.Framework.Rectangle SourceRectangle = Microsoft.Xna.Framework.Rectangle.Empty;
        internal IntPtr TexPtr;
    }

    public class Animation 
    {
        public SpriteRenderer SR = null;
        public string Name = "Default Animation";
        public List<Frame> Frames;
        public float Speed = 1.0f;
        public bool Reverse = false;
        public bool Loop = false;
        public bool FixedTimeBetweenFrames = false;
        public float FixedTime = 0.5f;
        public bool Paused = true;

        private float TimeCounter = 0;
        private int ActiveFrame = 0;

        public void Play()
        {
            Paused = false;
            ActiveFrame = Reverse ? Frames.Count - 1 : 0;
            TimeCounter = 0;
        }

        public void Update(GameTime gameTime)
        {
            if(!Paused && Frames.Count != 0)
            {
                if (TimeCounter >= (FixedTimeBetweenFrames ? FixedTime : Frames[ActiveFrame].Time))
                {
                    TimeCounter = 0;
                    
                    if(Reverse)
                        ActiveFrame = ActiveFrame <= 0 ? (Loop? Frames.Count - 1 : 0) : ActiveFrame - 1;
                    else
                        ActiveFrame = ActiveFrame >= Frames.Count - 1 ? (Loop ? 0 : Frames.Count - 1) : ActiveFrame + 1;

                    Paused = !Loop;
                }
                else
                    TimeCounter += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed;

                SR.Sprite.Texture = Frames[ActiveFrame].Tex;
                SR.Sprite.SourceRectangle = Frames[ActiveFrame].SourceRectangle;
            }
        }

        public Animation DeepCopy()
        {
            Animation Clone = (Animation)MemberwiseClone();
            Clone.Frames = new List<Frame>(Frames.Count);

            for (int i = 0; i < Frames.Count; i++)
                Clone.Frames[i] = new Frame() { SourceRectangle = Frames[i].SourceRectangle, Time = Frames[i].Time, Tex = Frames[i].Tex };

            return Clone;    
        }
    }
}
