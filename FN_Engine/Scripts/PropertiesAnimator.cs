using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FN_Engine
{
    public class PropertiesAnimator: GameObjectComponent
    {
        private List<KeyFrame> keyFrames;

        public PropertiesAnimator()
        {
            keyFrames = new List<KeyFrame>();
        }

        public void PlayKeyFrame(string tag)
        {
            foreach (KeyFrame key in keyFrames)
            {
                if (key.Tag == tag)
                {
                    key.Play();
                    return;
                }
            }
        }

        public void AddKeyFrame(KeyFrame key)
        {
            keyFrames.Add(key);
        }

        public void AddKeyFrame(KeyFrame key, bool PlayNow)
        {
            if(PlayNow)
                key.Play();
            
            keyFrames.Add(key);
        }

        public void DeleteKeyFrame(string tag)
        {
            foreach (KeyFrame key in keyFrames)
            {
                if (key.Tag == tag)
                {
                    keyFrames.Remove(key);
                    return;
                }
            }
        }

        public KeyFrame GetKeyFrame(string tag)
        {
            foreach (KeyFrame key in keyFrames)
                if (key.Tag == tag)
                    return key;

            return null;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (KeyFrame key in keyFrames)
            {
                key.Update(gameTime);

                if (key.Finished || key.FinishedButIsLooping)
                    if (key.DeleteAfterFinishing)
                        DeleteKeyFrame(key.Tag);
            }
        }

        public int KeyFramesCount()
        {
            return keyFrames.Count;
        }
    }
}
