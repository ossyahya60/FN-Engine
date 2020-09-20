using Microsoft.Xna.Framework.Media;

namespace MyEngine
{
    public static class MediaSource //1 track at a time, doesn't support multiple tracks at once!
    {
        public static float Volume
        {
            set
            {
                MediaPlayer.Volume = MathCompanion.Clamp(value, 0, 1);
            }
            get
            {
                return MediaPlayer.Volume;
            }
        }
        public static bool IsMuted
        {
            set
            {
                MediaPlayer.IsMuted = value;
            }
            get
            {
                return MediaPlayer.IsMuted;
            }
        }
        public static bool IsLooping
        {
            set
            {
                MediaPlayer.IsRepeating = value;
            }
            get
            {
                return MediaPlayer.IsRepeating;
            }
        }

        private static Song Song = null;

        public static void LoadTrack(string TrackName)
        {
            Song = Setup.Content.Load<Song>(TrackName);
        }

        public static void Play()
        {
            if (Song != null)
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(Song);
            }
        }

        public static void Pause()
        {
            if (Song != null && MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
        }

        public static void Resume()
        {
            if (Song != null && MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
        }

        public static void Stop()
        {
            if (Song != null)
                MediaPlayer.Stop();
        }
    }
}
