using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace MyEngine
{
    public class Animation //SpriteSheets should have the animation consecuive
    {
        public string Tag = "Default";
        public bool Looping = false;
        public float Speed = 1; //1 sec
        public int FramesCount = 1;
        /// <summary>
        /// You can give it negative size to reverse the animation, and give it the proper location
        /// </summary>
        public Rectangle SourceRectangle; //Should Be Set After Calling Start() of the scene

        private SpriteRenderer spriteRenderer;
        private Point CurrentFrame = Point.Zero;
        private float Counter = 0;
        private bool stop = true;
        private int FramesPassed = 0;

        public Animation(SpriteRenderer SR, int FrameCount)
        {
            spriteRenderer = SR;
            SourceRectangle = Rectangle.Empty;
            this.FramesCount = FrameCount;
        }

        public void Stop()
        {
            stop = true;
        }

        public void Resume()
        {
            stop = false;
        }

        public void Play()
        {
            stop = false;
            CurrentFrame = SourceRectangle.Location;

            spriteRenderer.Sprite.SourceRectangle.Location = CurrentFrame;
            spriteRenderer.Sprite.SourceRectangle.Size = SourceRectangle.Size;
        }

        public void Update(GameTime gameTime)
        {
            if(!stop)
            {
                if(Counter < Speed)
                    Counter += (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                {
                    Counter = 0;
                    FramesPassed++;

                    if (!GoNextFrame())
                    {
                        if (Looping)
                            CurrentFrame = SourceRectangle.Location;
                        else
                            stop = true;

                        FramesPassed = 0;
                    }

                    spriteRenderer.Sprite.SourceRectangle.Location = CurrentFrame;
                    spriteRenderer.Sprite.SourceRectangle.Size = new Point(Math.Abs(SourceRectangle.Size.X), Math.Abs(SourceRectangle.Size.Y));
                }
            }
        }

        private bool GoNextFrame()
        {
            if (FramesPassed >= FramesCount)
                return false;

            if (CurrentFrame.X + SourceRectangle.X >= spriteRenderer.Sprite.Texture.Width)
            {
                CurrentFrame.X = 0;
                CurrentFrame.Y += SourceRectangle.Height;
            }
            else
                CurrentFrame.X += SourceRectangle.Width;

            return true;
        }

        public Animation DeepCopy(GameObject Clone)
        {
            Animation clone = this.MemberwiseClone() as Animation;
            clone.spriteRenderer = Clone.GetComponent<SpriteRenderer>();

            return clone;
        }

        public void Serialize(StreamWriter SW) // Pass spriterenderer in deserialization
        {
            SW.WriteLine(ToString());

            SW.Write("Tag:\t" + Tag + "\n");
            SW.Write("Looping:\t" + Looping.ToString() + "\n");
            SW.Write("Speed:\t" + Speed.ToString() + "\n");
            SW.Write("FramesCount:\t" + FramesCount.ToString() + "\n");
            SW.Write("SourceRectangle:\t" + SourceRectangle.X.ToString() + "\t" + SourceRectangle.Y.ToString() + "\t" + SourceRectangle.Width.ToString() + "\t" + SourceRectangle.Height.ToString() + "\n");
            SW.Write("stop:\t" + stop.ToString() + "\n");

            SW.WriteLine("End Of " + ToString());
        }
    }
}
