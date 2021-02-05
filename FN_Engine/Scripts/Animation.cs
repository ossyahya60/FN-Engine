using Microsoft.Xna.Framework;
using System;

namespace FN_Engine
{
    //public class Animation
    //{
    //    public Sprite Sprite { get; set; }
    //    public bool IsLooping = true;
    //    public bool IsPlaying = false;
    //    public bool Reversed = false;
    //    public bool Paused = false;
    //    public bool PlayOnAwake = true;
    //    public float Speed = 1;
    //    public string Tag;

    //    private int FramesCount;
    //    private Point CurrentFrame;
    //    private float Timer = 0;
    //    public Point OrigSpriteSourceValues;

    //    public Animation(Sprite sprite, int framesCount)
    //    {
    //        CurrentFrame = Point.Zero;
    //        OrigSpriteSourceValues = sprite.SourceRectangle.Location;
    //        Sprite = sprite;
    //        FramesCount = framesCount;

    //        if (PlayOnAwake)
    //            Play();
    //    }

    //    public void Play()
    //    {
    //        if (!IsPlaying)
    //        {
    //            Timer = 0;
    //            IsPlaying = true;
    //            Resume();
    //            CurrentFrame = (Reversed) ? GetLastFrame(ref CurrentFrame) : Point.Zero;
    //        }
    //    }

    //    public void Pause()
    //    {
    //        Paused = true;
    //    }

    //    public void Resume()
    //    {
    //        Paused = false;
    //    }

    //    public void Reset()
    //    {
    //        IsPlaying = false;
    //        Play();
    //    }

    //    public void ExitAnimation()
    //    {
    //        IsPlaying = false;
    //    }

    //    public void Update(GameTime gameTime)
    //    {
    //        if (IsPlaying && !Paused)
    //        {
    //            Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

    //            if (Timer > Speed)
    //            {
    //                if (Reversed)
    //                {
    //                    if (CurrentFrame == Point.Zero)
    //                    {
    //                        IsPlaying = false;
    //                        if (IsLooping)
    //                            Play();
    //                    }
    //                    else
    //                        DecrementFrame();
    //                }
    //                else
    //                {
    //                    if (IsLastFrame())
    //                    {
    //                        IsPlaying = false;
    //                        if (IsLooping)
    //                            Play();
    //                    }
    //                    else
    //                        IncrementFrame();
    //                }
    //                Timer = 0;
    //            }

    //            Sprite.SourceRectangle.X = CurrentFrame.X * Sprite.SourceRectangle.Width + OrigSpriteSourceValues.X;
    //            Sprite.SourceRectangle.Y = CurrentFrame.Y * Sprite.SourceRectangle.Height + OrigSpriteSourceValues.Y;
    //        }
    //    }

    //    //Note: We assume that every consecutive Frame has the same Dimensions!
    //    private Point GetLastFrame(ref Point point)
    //    {
    //        Point Temp = Point.Zero;
    //        point.X = FramesCount % FramesCount - 1;
    //        point.Y = FramesCount / FramesCount;

    //        return point;
    //    }

    //    private bool IsLastFrame()
    //    {
    //        bool Valid = (CurrentFrame.X == FramesCount - 1) ? true : false; //=>Something Wrong
    //        return Valid;
    //    }

    //    private void IncrementFrame()
    //    {
    //        CurrentFrame.X += 1;
    //        CurrentFrame.X = CurrentFrame.X % FramesCount;
    //        CurrentFrame.Y = CurrentFrame.Y / FramesCount;
    //    }

    //    private void DecrementFrame()
    //    {
    //        CurrentFrame.X -= 1;
    //        CurrentFrame.X = CurrentFrame.X % FramesCount;
    //        CurrentFrame.Y = CurrentFrame.Y / FramesCount;
    //    }
    //}






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
        private Point NewSize;

        public Animation(SpriteRenderer SR, int FrameCount)
        {
            spriteRenderer = SR;
            SourceRectangle = Rectangle.Empty;
            NewSize = SourceRectangle.Size;
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

                    NewSize.X = Math.Abs(SourceRectangle.Size.X);
                    NewSize.Y = Math.Abs(SourceRectangle.Size.Y);
                    spriteRenderer.Sprite.SourceRectangle.Location = CurrentFrame;
                    spriteRenderer.Sprite.SourceRectangle.Size = NewSize;
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
    }
}
