using Microsoft.Xna.Framework;

namespace MyEngine
{
    public class Animation
    {
        public Sprite Sprite { get; set; }
        public bool IsLooping = true;
        public bool IsPlaying = false;
        public bool Reversed = false;
        public bool Paused = false;
        public bool PlayOnAwake = true;
        public float Speed = 1;
        public string Tag;

        private int FramesCount;
        private Point CurrentFrame;
        private float Timer = 0;
        private Point OrigSpriteSourceValues;

        public Animation(Sprite sprite, int framesCount)
        {
            CurrentFrame = Point.Zero;
            OrigSpriteSourceValues = sprite.SourceRectangle.Location;
            Sprite = sprite;
            FramesCount = framesCount;

            if (PlayOnAwake)
                Play();
        }

        public void Play()
        {
            if (!IsPlaying)
            {
                Timer = 0;
                IsPlaying = true;
                Resume();
                CurrentFrame = (Reversed) ? GetLastFrame(ref CurrentFrame) : Point.Zero;
            }
        }

        public void Pause()
        {
            Paused = true;
        }

        public void Resume()
        {
            Paused = false;
        }

        public void Reset()
        {
            IsPlaying = false;
            Play();
        }

        public void ExitAnimation()
        {
            IsPlaying = false;
        }

        public void Update(GameTime gameTime)
        {
            if (IsPlaying && !Paused)
            {
                Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (Timer > Speed)
                {
                    if (Reversed)
                    {
                        if (CurrentFrame == Point.Zero)
                        {
                            IsPlaying = false;
                            if (IsLooping)
                                Play();
                        }
                        else
                            DecrementFrame();
                    }
                    else
                    {
                        if (IsLastFrame())
                        {
                            IsPlaying = false;
                            if (IsLooping)
                                Play();
                        }
                        else
                            IncrementFrame();
                    }
                    Timer = 0;
                }

                Sprite.SourceRectangle.X = CurrentFrame.X * Sprite.SourceRectangle.Width + OrigSpriteSourceValues.X;
                Sprite.SourceRectangle.Y = CurrentFrame.Y * Sprite.SourceRectangle.Height + OrigSpriteSourceValues.Y;
            }
        }

        //Note: We assume that every consecutive Frame has the same Dimensions!
        private Point GetLastFrame(ref Point point)
        {
            Point Temp = Point.Zero;
            point.X = FramesCount % (Sprite.Texture.Width / Sprite.SourceRectangle.Width) - 1;
            point.Y = FramesCount / (Sprite.Texture.Width / Sprite.SourceRectangle.Width);

            return point;
        }

        private bool IsLastFrame()
        {
            bool Valid = (CurrentFrame.X == FramesCount % (Sprite.Texture.Width / Sprite.SourceRectangle.Width) - 1) && (CurrentFrame.Y == FramesCount / (Sprite.Texture.Width / Sprite.SourceRectangle.Width));
            return Valid;
        }

        private void IncrementFrame()
        {
            CurrentFrame.X += 1;
            CurrentFrame.X = CurrentFrame.X % (Sprite.Texture.Width / Sprite.SourceRectangle.Width);
            CurrentFrame.Y = CurrentFrame.Y / (Sprite.Texture.Width / Sprite.SourceRectangle.Width);
        }

        private void DecrementFrame()
        {
            CurrentFrame.X -= 1;
            CurrentFrame.X = CurrentFrame.X % (Sprite.Texture.Width / Sprite.SourceRectangle.Width);
            CurrentFrame.Y = CurrentFrame.Y / (Sprite.Texture.Width / Sprite.SourceRectangle.Width);
        }
    }
}
