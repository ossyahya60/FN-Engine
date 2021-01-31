using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MyEngine
{
    public class Animator: GameObjectComponent
    {
        public List<Animation> AnimationClips;
        public Animation ActiveClip = null;

        public Animator()
        {
            AnimationClips = new List<Animation>();
        }

        public void AddClip(Animation AM)
        {
            AnimationClips.Add(AM);
        }

        public void AddClip(Animation AM, bool SetAsActive)
        {
            AnimationClips.Add(AM);

            if (SetAsActive)
                ActiveClip = AM;
        }

        public void PlayWithTag(string Tag)
        {
            foreach (Animation Anim in AnimationClips)
                if (Anim.Tag == Tag)
                {
                    Anim.Play();
                    ActiveClip = Anim;
                }
        }

        public override void Update(GameTime gameTime)
        {
            if (ActiveClip != null)
                ActiveClip.Update(gameTime);
        }

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            Animator clone = this.MemberwiseClone() as Animator;
            clone.ActiveClip = ActiveClip.DeepCopy(Clone);
            clone.AnimationClips = new List<Animation>();

            foreach (Animation animation in AnimationClips)
                clone.AddClip(animation.DeepCopy(Clone));

            return clone;
        }
    }
}
