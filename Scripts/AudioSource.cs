using Microsoft.Xna.Framework.Audio;
using System;

namespace FN_Engine
{
    public class AudioSource: GameObjectComponent //this class is supposed to play sound effects only, use media player to play songs or tracks
    {
        public string UniqueName = "sound effect";

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
        public bool IsLooping
        {
            set
            {
                isLooping = value;

                if (SoundEffect == null)
                    return;
                else if (SoundEffectInstance == null)
                    SoundEffectInstance = SoundEffect.CreateInstance();

                SoundEffectInstance.IsLooped = value;
            }
            get
            {
                return isLooping;
            }
        }
        public string AudioName
        {
            set
            {
                if (audioName == value)
                    return;

                audioName = value;
                if (value != null)
                {
                    //Clean Up
                    if(SoundEffectInstance != null && !SoundEffectInstance.IsDisposed)
                        SoundEffectInstance.Dispose();
                    if (SoundEffect != null && !SoundEffect.IsDisposed)
                        SoundEffect.Dispose();

                    try { SoundEffect = Setup.Content.Load<SoundEffect>(value); }
                    catch (Exception E) { FN_Editor.ContentWindow.LogText.Add(E.Message); } //Log error here!

                    SoundEffectInstance = SoundEffect.CreateInstance();
                }
            }
            get
            {
                return audioName;
            }
        }

        private SoundEffect SoundEffect;
        private SoundEffectInstance SoundEffectInstance;
        private float volume = 1;
        private float pitch = 0;
        private float pan = 0;
        private string audioName = null;
        private bool isLooping = false;

        public AudioSource(string AudioName)
        {
            this.AudioName = AudioName;
        }

        public AudioSource()
        {
            audioName = "null";
        }

        public override void Start()
        {
            if (AudioName != "null" && SoundEffect == null)
                LoadSoundEffect(AudioName);
        }

        public void LoadSoundEffect(string Path)
        {
            AudioName = Path;
        }

        public void Play(bool CanOverwrite = true)
        {
            if (!Enabled)
                return;

            if (CanOverwrite)
                SoundEffectInstance.Stop();

            SoundEffectInstance.Volume = volume;
            SoundEffectInstance.Pitch = pitch;
            SoundEffectInstance.Pan = pan;
            SoundEffectInstance.IsLooped = IsLooping;
            SoundEffectInstance.Play();
        }

        public void Pause()
        {
            if (SoundEffectInstance.State == SoundState.Playing)
                SoundEffectInstance.Pause();
        }

        public void Resume()
        {
            if (SoundEffectInstance.State == SoundState.Paused && Enabled)
                SoundEffectInstance.Resume();
        }

        public void Stop()
        {
            SoundEffectInstance.Stop();
        }

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            AudioSource clone = this.MemberwiseClone() as AudioSource;
            clone.gameObject = Clone;
            clone.volume = volume;
            clone.pitch = pitch;
            clone.pan = pan;
            clone.AudioName = null;
            clone.AudioName = AudioName;
            clone.UniqueName = UniqueName;

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

        //public override void Serialize(StreamWriter SW) //Load effect using audio name in deserialization
        //{
        //    SW.WriteLine(ToString());

        //    base.Serialize(SW);
        //    SW.Write("volume:\t" + volume.ToString() + "\n");
        //    SW.Write("pitch:\t" + pitch.ToString() + "\n");
        //    SW.Write("pan:\t" + pan.ToString() + "\n");
        //    SW.Write("AudioName:\t" + AudioName + "\n");

        //    SW.WriteLine("End Of " + ToString());
        //}
    }
}