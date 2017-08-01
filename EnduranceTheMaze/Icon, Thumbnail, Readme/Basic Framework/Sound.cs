using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ImpossiMaze
{
    /// <summary>
    /// Plays individual sounds and has a primitive shuffle playlist.
    /// </summary>
    public class Playlist
    {
        public List<Song> music; //List of all music to play.
        private int _index;
        private float _vol;
        public float vol
        {
            get
            {
                return _vol;
            }

            set
            {
                if (value >= 0 && value <= 1)
                {
                    _vol = value;
                }
                else
                {
                    throw new Exception("Volume must be from 0 to 1.");
                }
            }
        }

        //Contains the game instance.
        private MainLoop game;

        /// <summary>
        /// Initializes default values.
        /// </summary>
        public Playlist(MainLoop game)
        {
            this.game = game;

            //Sets the sound playlist.
            music = new List<Song>();

            //Sets the volume to full (default).
            vol = 1;

            //The index in the music list for the current song.
            _index = 0;
        }

        /// <summary>
        /// Plays directly from a song.
        /// </summary>
        public static void Play(Song snd, float vol)
        {
            MediaPlayer.Volume = vol;
            MediaPlayer.Play(snd);
        }

        /// <summary>
        /// Plays directly from a sound. Allows multiple instances.
        /// </summary>
        public static void Play(SoundEffect snd)
        {
            snd.Play();
        }

        /// <summary>
        /// Plays directly from a sound. Allows multiple instances.
        /// </summary>
        public void Play(SoundEffect snd, int x, int y)
        {
            SoundEffectInstance sound = snd.CreateInstance();

            #region Interaction: MngrLvl.cs
            if (game.mngrLvl.actor != null)
            {
                //Attenuates the volume based on distance to sound.
                int xPos = Math.Abs(x - game.mngrLvl.actor.x);
                int yPos = Math.Abs(y - game.mngrLvl.actor.y);

                if (xPos + yPos != 0)
                {
                    sound.Volume = 1f / (xPos + yPos);
                }
            }
            #endregion

            sound.Play();
        }

        /// <summary>
        /// Plays directly from a sound, returning the instance.
        /// </summary>
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
        /// Plays the given playlist.
        /// </summary>
        public void Begin()
        {
            _index = Utils.rng.Next(0, music.Count);

            if (music.Count != 0)
            {
                MediaPlayer.Play(music[_index]);
            }
        }

        /// <summary>
        /// Pauses the active sound from the playlist.
        /// </summary>
        public void Pause(bool pause)
        {
            if (pause)
            {
                MediaPlayer.Pause();
            }
            else if (MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
            }
        }

        /// <summary>
        /// When music have finished, plays another randomly.
        /// </summary>
        public void Update()
        {
            //When the current sound has stopped (assuming it finished), plays
            //a different one on shuffle.
            if (MediaPlayer.State == MediaState.Stopped)
            {
                int newIndex; //The new song to play by index.

                //Gets a random song from the list that isn't the current one.
                do
                {
                    newIndex = Utils.rng.Next(0, music.Count);
                } while (_index == newIndex);

                //Sets the new index and plays its associated music.
                _index = newIndex;
                MediaPlayer.Play(music[_index]);
            }
        }
    }
}
