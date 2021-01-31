using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MyEngine
{
    public class AudioSource: GameObjectComponent //this class is supposed to play sound effects only, use media player to play songs or tracks
    {
        public float Volume
        {
            set
            {
                volume = MathCompanion.Clamp(value, 0, 1);
            }
            get
            {
                return volume;
            }
        }
        public float Pitch
        {
            set
            {
                pitch = MathCompanion.Clamp(value, -1, 1);
            }
            get
            {
                return pitch;
            }
        }
        public float Pan
        {
            set
            {
                pan = MathCompanion.Clamp(value, -1, 1);
            }
            get
            {
                return pan;
            }
        }
        public bool IsLooping = false;
        public bool DestroyAfterFinishing = false;

        private SoundEffect SoundEffect;
        private SoundEffectInstance SoundEffectInstance;
        private float volume = 1;
        private float pitch = 0;
        private float pan = 0;
        private string AudioName;

        public AudioSource(string AudioName)
        {
            this.AudioName = AudioName;
            SoundEffect = Setup.Content.Load<SoundEffect>(AudioName);
        }

        public override void Start()
        {

        }

        public void LoadSoundEffect(string Path)
        {
            SoundEffect = Setup.Content.Load<SoundEffect>(Path);
        }

        public void Play()
        {
            try
            {
                //if(SoundEffectInstance != null)
                //  SoundEffectInstance.Dispose();

                SoundEffectInstance = SoundEffect.CreateInstance();
                SoundEffectInstance.IsLooped = IsLooping;
                SoundEffectInstance.Volume = volume;
                SoundEffectInstance.Pitch = pitch;
                SoundEffectInstance.Pan = pan;
                SoundEffectInstance.Play();
            }
            catch
            { }
        }

        public void Pause()
        {
            if (SoundEffectInstance != null)
                if (SoundEffectInstance.State == SoundState.Playing)
                    SoundEffectInstance.Pause();
        }

        public void Resume()
        {
            if(SoundEffectInstance != null)
                if (SoundEffectInstance.State == SoundState.Paused)
                    SoundEffectInstance.Resume();
        }

        public void Stop()
        {
            if(SoundEffectInstance != null)
                SoundEffectInstance.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            if (SoundEffectInstance != null)
            {
                if (SoundEffectInstance.State == SoundState.Stopped)
                {
                    SoundEffectInstance.Dispose();
                    SoundEffectInstance = null;

                    if (DestroyAfterFinishing)
                        Threader.Invoke(Destroy, 5);
                }
            }
        }

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            AudioSource clone = this.MemberwiseClone() as AudioSource;
            clone.volume = volume;
            clone.pitch = pitch;
            clone.pan = pan;
            clone.AudioName = AudioName;
            if (SoundEffect != null)
            {
                clone.SoundEffect = Setup.Content.Load<SoundEffect>(AudioName);
                clone.SoundEffectInstance = clone.SoundEffect.CreateInstance();
            }

            return clone;
        }

        public float ClipLength()
        {
            return (float)SoundEffect.Duration.TotalSeconds;
        }
    }
}