using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Playlist to play a list of sounds in order.
    /// </summary>
    public class SfxPlaylist
    {
        #region Members
        /// <summary>
        /// Sounds to be iterated through in the playlist.
        /// </summary>
        public List<SoundEffect> sounds;

        /// <summary>
        /// The current sound.
        /// </summary>
        public SoundEffectInstance sound;

        /// <summary>
        /// The position of the sound in the list.
        /// </summary>
        public int soundIndex = 0;

        /// <summary>
        /// References the game.
        /// </summary>
        private MainLoop game;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new playlist for sound effects.
        /// </summary>
        /// <param name="sounds">
        /// Takes any number of sounds.
        /// </param>
        public SfxPlaylist(MainLoop game, params SoundEffect[] snds)
        {
            sounds = new List<SoundEffect>();
            this.game = game;

            foreach (SoundEffect sfx in snds)
            {
                sounds.Add(sfx);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Plays the given sound at the desired location with respect
        /// attenuated by distance to the listener position.
        /// </summary>
        /// <param name="snd">
        /// The sound to play.
        /// </param>
        /// <param name="x">
        /// The x-position of the sound, to be compared to the active
        /// actor's position.
        /// </param>
        /// <param name="y">
        /// The y-position of the sound, to be compared to the active
        /// actor's position.
        /// </param>
        public void Play(SoundEffect snd, int x, int y)
        {
            SoundEffectInstance sound = snd.CreateInstance();

            #region Interaction: MngrLvl.cs
            if (game.mngrLvl.actor != null)
            {
                //Attenuates the volume based on distance to sound.
                int xPos = Math.Abs(x - game.mngrLvl.actor.X);
                int yPos = Math.Abs(y - game.mngrLvl.actor.Y);

                if (xPos + yPos != 0)
                {
                    sound.Volume = 1f / (xPos + yPos);
                }
            }
            #endregion

            sound.Play();
        }

        /// <summary>
        /// Plays the given song with a volume from 0 (mute) to 1 (max).
        /// </summary>
        /// <param name="snd">
        /// The song to play.
        /// </param>
        /// <param name="vol">
        /// The volume of the song is multiplied by this value from
        /// 0 to 1, such that 0 = 0%, 0.5 = 50%, and 1 = 100%. All
        /// intermediate values can be entered.
        /// </param>
        public static void Play(Song snd, float vol)
        {
            MediaPlayer.Volume = vol;
            MediaPlayer.Play(snd);
        }

        /// <summary>
        /// Plays the given sound.
        /// </summary>
        /// <param name="snd">
        /// The sound to play.
        /// </param>
        public static void Play(SoundEffect snd)
        {
            snd.Play();
        }

        /// <summary>
        /// Plays the given sound with a volume from 0 (mute) to 1 (max).
        /// </summary>
        /// <param name="snd">
        /// The sound to play.
        /// </param>
        /// <param name="vol">
        /// The volume of the song is multiplied by this value from
        /// 0 to 1, such that 0 = 0%, 0.5 = 50%, and 1 = 100%. All
        /// intermediate values can be entered.
        /// </param>
        public static SoundEffectInstance Play(SoundEffect snd, float vol)
        {
            SoundEffectInstance sound = snd.CreateInstance();
            sound.Volume = vol;
            snd.Play();
            return sound;
        }

        /// <summary>
        /// Plays directly from a sound, returning the instance.
        /// </summary>
        public static SoundEffectInstance Play(SoundEffect snd, float vol,
            float pitch)
        {
            SoundEffectInstance sound = snd.CreateInstance();
            sound.Volume = vol;
            sound.Pitch = pitch;
            sound.Play();
            return sound;
        }

        /// <summary>
        /// Plays directly from a song.
        /// </summary>
        public static void PlayLooped(Song snd, float vol)
        {
            MediaPlayer.Volume = vol;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(snd);
        }

        /// <summary>
        /// Loop-plays directly from a sound, returning the instance.
        /// </summary>
        public static SoundEffectInstance PlayLooped(SoundEffect snd,
            float vol)
        {
            SoundEffectInstance sound = snd.CreateInstance();
            sound.IsLooped = true;
            sound.Volume = vol;
            sound.Play();
            return sound;
        }

        /// <summary>
        /// Stops the sound with the given handle.
        /// </summary>
        public static void Stop(SoundEffectInstance snd)
        {
            snd.Stop();
        }

        /// <summary>
        /// Randomly selects the next sound to play and returns sound index.
        /// Returns -1 if there are no sounds loaded.
        /// </summary>
        public int NextSoundRandom()
        {
            if (sounds.Count == 0)
            {
                return -1;
            }

            soundIndex = new Random().Next(sounds.Count);
            sound = sounds.ElementAt(soundIndex).CreateInstance();
            sound.Play();
            return soundIndex;
        }

        /// <summary>
        /// Pauses or resumes the active sound from the playlist.
        /// </summary>
        public void Pause(bool doPause)
        {
            if (doPause)
            {
                sound?.Pause();
            }
            else if (sound?.State == SoundState.Paused)
            {
                sound?.Resume();
            }
        }

        /// <summary>
        /// Shuffles the sound list.
        /// </summary>
        public void Shuffle()
        {
            Random rng = new Random();
            int numSongs = sounds.Count;

            while (numSongs > 1)
            {
                numSongs--;
                int next = rng.Next(numSongs + 1);
                SoundEffect value = sounds[next];
                sounds[next] = sounds[numSongs];
                sounds[numSongs] = value;
            }
        }

        /// <summary>
        /// Checks to see if the song ended and begins the next.
        /// </summary>
        public void Update()
        {
            //Starts playing the first sound.
            if (sound == null)
            {
                NextSoundRandom();
                return;
            }

            //When the sound finishes, start another.
            if (sound?.State == SoundState.Stopped)
            {
                //Keeps track of the old sound index for the loop.
                int tempSoundIndex = soundIndex;

                while (tempSoundIndex == soundIndex)
                {
                    sound.Stop();
                    NextSoundRandom();
                }
            }
        }
        #endregion
    }
}
