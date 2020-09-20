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
        public bool PlayOnAwake = false;
        public bool IsLooping = false;

        private SoundEffect SoundEffect;
        private SoundEffectInstance SoundEffectInstance;
        private float volume = 1;
        private float pitch = 0;
        private float pan = 0;

        public AudioSource(string AudioName)
        {
            SoundEffect = Setup.Content.Load<SoundEffect>(AudioName);

            if (PlayOnAwake)
                Play();
        }

        public void LoadSoundEffect(string Path)
        {
            SoundEffect = Setup.Content.Load<SoundEffect>(Path);
        }

        public void Play()
        {
            if(SoundEffectInstance != null)
                SoundEffectInstance.Dispose();

            SoundEffectInstance = SoundEffect.CreateInstance();
            SoundEffectInstance.IsLooped = IsLooping;
            SoundEffectInstance.Volume = volume;
            SoundEffectInstance.Pitch = pitch;
            SoundEffectInstance.Pan = pan;
            SoundEffectInstance.Play();
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
                }
            }
        }
    }
}