using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Represents a chain of connected levels.
    /// </summary>
    public class LevelSeries
    {
        #region Members
        /// <summary>
        /// Stores the current level number. Begins at 1.
        /// </summary>
        public int LevelNum { get; set; }

        /// <summary>
        /// The base filename used to load levels.
        /// </summary>
        public string LevelFileName { get; set; }

        /// <summary>
        /// The current game instance.
        /// </summary>
        private MainLoop game;
        #endregion

        #region Constructors
        /// <summary>
        /// A series of levels.
        /// </summary>
        public LevelSeries(MainLoop game, string baseFileName)
        {
            this.game = game;
            LevelNum = 1; //The first level to load.
            LevelFileName = baseFileName;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads the campaign level with the current number.
        /// </summary>
        public void LoadCampaign()
        {
            //Loads levels from the embedded resources.
            game.mngrLvl.LoadResource(LevelFileName + LevelNum + ".lvl");
        }

        /// <summary>
        /// Returns whether the level with the current level number exists.
        /// </summary>
        public bool LevelExists()
        {
            //Loads levels from the embedded resources.
            string path = LevelFileName + LevelNum + ".lvl";

            //If the stream is null, it doesn't exist.
            Stream stream = GetType().Assembly
                .GetManifestResourceStream(path);

            return (stream != null);
        }
        #endregion
    }
}