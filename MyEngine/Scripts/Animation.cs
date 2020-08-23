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
        private int CurrentFrame = 0;
        private float Timer = 0;

        public Animation(Sprite sprite, int framesCount)
        {
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
                CurrentFrame = (Reversed) ? FramesCount - 1 : 0;
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
                        if (CurrentFrame <= 0)
                        {
                            IsPlaying = false;
                            if (IsLooping)
                                Play();
                        }
                        else
                            CurrentFrame--;
                    }
                    else
                    {
                        if (CurrentFrame >= FramesCount - 1)
                        {
                            IsPlaying = false;
                            if (IsLooping)
                                Play();
                        }
                        else
                            CurrentFrame++;
                    }
                    Timer = 0;
                }

                Sprite.SourceRectangle.X = CurrentFrame * Sprite.SourceRectangle.Width;
            }
        }
    }
}
