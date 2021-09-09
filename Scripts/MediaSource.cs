using Microsoft.Xna.Framework.Media;
using System.IO;

namespace FN_Engine
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

        public static Song Song = null;

        public static void LoadTrack(string TrackName)
        {
            Song = Setup.Content.Load<Song>(TrackName);
        }

        public static void Play()
        {
            if (Song != null)
            {
                //MediaPlayer.Stop();
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

        public static void Serialize(StreamWriter SW)
        {
            SW.WriteLine("MediaSource");

            SW.Write("Volume:\t" + Volume.ToString() + "\n");
            SW.Write("IsMuted:\t" + IsMuted.ToString() + "\n");
            SW.Write("IsLooping:\t" + IsLooping.ToString() + "\n");
            if(Song != null)
                SW.Write("Song:\t" + Song.Name + "\n");
            else
                SW.Write("Song:\t" + "null\n");

            SW.WriteLine("End Of MediaSource");
        }

        public static void Dispose()
        {
            if (Song != null)
                Song.Dispose();
        }
    }
}
