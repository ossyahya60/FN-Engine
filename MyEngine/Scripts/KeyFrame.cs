using Microsoft.Xna.Framework;

namespace MyEngine
{
    public class KeyFrame
    {
        public bool ReverseAfterFinishing = false;
        public bool Finished = true;
        public bool IsLooping = false;
        public string Tag;

        private float SourceValue = 0;
        private float DestinationValue = 1;
        private float TimeDuration = 1;
        private float TimeCounter = 0;
        private bool Paused = false;

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
                TimeCounter = MathCompanion.Clamp(TimeCounter + (float)gameTime.ElapsedGameTime.TotalSeconds, 0, TimeDuration);

                if (TimeCounter >= TimeDuration)
                {
                    Finished = true;

                    if(ReverseAfterFinishing)
                    {
                        ReverseAfterFinishing = false;
                        ReverseKeyFrame();
                    }

                    if (IsLooping)
                        Play();
                }
            }
        }

        public void Play()
        {
            TimeCounter = 0;
            Finished = false;
            Paused = false;
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

        public void Pause()
        {
            Paused = true;
        }

        public void Resume()
        {
            Paused = false;
        }

        public void ReverseKeyFrame()
        {
            float TempConatiner = SourceValue;
            SourceValue = DestinationValue;
            DestinationValue = TempConatiner;
            //TimeCounter = TimeDuration - TimeCounter;
        }
    }
}
