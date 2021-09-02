using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FN_Engine
{
    public class Animator: GameObjectComponent
    {
        public static bool IsAnythingPlaying
        {
            internal set
            {
                animationPlaying = value;
            }
            get
            {
                return animationPlaying;
            }
        }
        public List<Animation> AnimationClips;

        private Animation ActiveClip = null;
        private SpriteRenderer SR = null;
        private static bool animationPlaying = false;

        public override void Start()
        {
            SR = gameObject.GetComponent<SpriteRenderer>();
        }

        public Animator()
        {
            AnimationClips = new List<Animation>();
        }

        public void AddClip(Animation AM)
        {
            AM.SR = SR;
            AnimationClips.Add(AM);
        }

        public void AddClip(Animation AM, bool SetAsActive)
        {
            AM.SR = SR;
            AnimationClips.Add(AM);

            if (SetAsActive)
                ActiveClip = AM;
        }

        public void Play(string Name)
        {
            foreach (Animation Anim in AnimationClips)
                if (Anim.Name == Name)
                {
                    Anim.SR = SR;
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
            clone.ActiveClip = ActiveClip != null? ActiveClip.DeepCopy() : null;
            clone.AnimationClips = new List<Animation>();

            foreach (Animation animation in AnimationClips)
                clone.AddClip(animation.DeepCopy());

            return clone; 
        }

        //public override void Serialize(StreamWriter SW)
        //{
        //    SW.WriteLine(ToString());

        //    base.Serialize(SW);
        //    foreach (Animation AM in AnimationClips)
        //        AM.Serialize(SW);
        //    //SW.Write("ActiveClip:\t" + ActiveClip.Tag.ToString() + "\n");

        //    SW.WriteLine("End Of " + ToString());
        //}
    }
}
