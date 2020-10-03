using Microsoft.Xna.Framework;
using System;

namespace MyEngine
{
    public class KeyFrame
    {
        public bool PlayAfterFinishing = false;
        public bool FinishedButIsLooping = false;
        public bool DeleteAfterFinishing = false;
        public bool ReverseAfterFinishing = false;
        public bool Finished = true;
        public bool IsLooping = false;
        public string Tag;

        private float SourceValue = 0;
        private float DestinationValue = 1;
        private float TimeDuration = 1;
        private float TimeCounter = 0;
        private bool Paused = false;
        private Color OriginalColor;
        private bool JustStarted = true;

        public KeyFrame(float sourceValue, float destinationValue, float timeDuration, string tag)
        {
            Tag = tag;
            SourceValue = sourceValue;
            DestinationValue = destinationValue;
            TimeDuration = (timeDuration < 0) ? 0 : timeDuration;
        }

        public void Update(GameTime gameTime)
        {
            if (!Finished)
            {
                if(!Paused)
                    TimeCounter = MathCompanion.Clamp(TimeCounter + (float)gameTime.ElapsedGameTime.TotalSeconds, 0, TimeDuration);

                if (TimeCounter >= TimeDuration)
                {
                    FinishedButIsLooping = false;
                    Finished = true;

                    if (ReverseAfterFinishing)
                    {
                        ReverseAfterFinishing = false;
                        ReverseKeyFrame();
                    }

                    if (PlayAfterFinishing)
                    {
                        Play();
                        PlayAfterFinishing = false;
                    }

                    if (IsLooping)
                    {
                        FinishedButIsLooping = true;
                        Play();
                    }
                }
            }
        }

        public void Play()
        {
            TimeCounter = 0;
            Finished = false;
            Paused = false;
            JustStarted = true;
        }

        public void GetFeedback(ref int Value)
        {
            if(!Finished)
                Value = (int)((TimeDuration != 0) ? SourceValue + ((DestinationValue - SourceValue) * (TimeCounter / TimeDuration)) : DestinationValue);
        }

        public void GetFeedback(ref float Value)
        {
            if (!Finished)
                Value = (TimeDuration != 0) ? SourceValue + ((DestinationValue - SourceValue) * (TimeCounter / TimeDuration)) : DestinationValue;
        }

        public void GetFeedback(ref Vector2 Value)
        {
            if (!Finished)
                Value = Vector2.One * ((TimeDuration != 0) ? SourceValue + ((DestinationValue - SourceValue) * (TimeCounter / TimeDuration)) : DestinationValue);
        }

        public void GetFeedback(ref Color Value)
        {
            if (JustStarted)
            {
                OriginalColor = Value;
                JustStarted = false;
            }

            if (!Finished)
                Value = OriginalColor * ((TimeDuration != 0) ? SourceValue + ((DestinationValue - SourceValue) * (TimeCounter / TimeDuration)) : DestinationValue);
        }

        public void GetFeedback(Action<float> Value)
        {
            if (!Finished)
                Value((TimeDuration != 0) ? SourceValue + ((DestinationValue - SourceValue) * (TimeCounter / TimeDuration)) : DestinationValue);
        }

        public void GetFeedback(Action<Vector2> Value)
        {
            if (!Finished)
                Value(Vector2.One * ((TimeDuration != 0) ? SourceValue + ((DestinationValue - SourceValue) * (TimeCounter / TimeDuration)) : DestinationValue));
        }

        public void Pause()
        {
            Paused = true;
        }

        public void Resume()
        {
            Paused = false;
        }

        private void ReverseKeyFrame()
        {
            float TempConatiner = SourceValue;
            SourceValue = DestinationValue;
            DestinationValue = TempConatiner;
            //TimeCounter = TimeDuration - TimeCounter;
        }
    }
}
