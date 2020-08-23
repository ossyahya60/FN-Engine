using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MyEngine
{
    public class Animator: GameObjectComponent
    {
        public List<Animation> AnimationClips;

        public Animator()
        {
            AnimationClips = new List<Animation>();
        }

        public void PlayWithTag(string Tag)
        {
            Animation ActiveClip = GetActiveClip();

            if (ActiveClip != null)
                ActiveClip.ExitAnimation();

            foreach (Animation Anim in AnimationClips)
                if (Anim.Tag == Tag)
                    Anim.Play();
        }

        public Animation GetActiveClip()
        {
            foreach (Animation Anim in AnimationClips)
                if (Anim.IsPlaying)
                    return Anim;

            return null;
        }

        public override void Update(GameTime gameTime)
        {
            Animation ActiveClip = GetActiveClip();

            if (ActiveClip != null)
            {
                gameObject.GetComponent<SpriteRenderer>().Sprite = ActiveClip.Sprite;
                ActiveClip.Update(gameTime);
            }
        }
    }
}
