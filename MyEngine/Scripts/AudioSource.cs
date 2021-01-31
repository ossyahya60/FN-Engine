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
            SoundEffectInstance = SoundEffect.CreateInstance();
        }

        public override void Start()
        {
            
        }

        public void LoadSoundEffect(string Path)
        {
            SoundEffect = Setup.Content.Load<SoundEffect>(Path);

            if (SoundEffect != null)
                SoundEffectInstance.Dispose();

            SoundEffectInstance = SoundEffect.CreateInstance();
        }

        public void Play()
        {
            SoundEffectInstance.IsLooped = IsLooping;
            SoundEffectInstance.Volume = volume;
            SoundEffectInstance.Pitch = pitch;
            SoundEffectInstance.Pan = pan;
            SoundEffectInstance.Play();
        }

        public void Pause()
        {
            if (SoundEffectInstance.State == SoundState.Playing)
                SoundEffectInstance.Pause();
        }

        public void Resume()
        {
            if (SoundEffectInstance.State == SoundState.Paused)
                SoundEffectInstance.Resume();
        }

        public void Stop()
        {
            SoundEffectInstance.Stop();
        }

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            AudioSource clone = this.MemberwiseClone() as AudioSource;
            clone.volume = volume;
            clone.pitch = pitch;
            clone.pan = pan;
            clone.AudioName = AudioName;
            clone.SoundEffect = SoundEffect;
            clone.SoundEffectInstance = clone.SoundEffect.CreateInstance();

            return clone;
        }

        public float ClipLength()
        {
            return (float)SoundEffect.Duration.TotalSeconds;
        }

        public void Dispose() //Used if you will not use this sound effect on this object again
        {
            SoundEffectInstance.Dispose();
        }
    }
}