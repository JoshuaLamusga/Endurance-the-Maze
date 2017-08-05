using System.IO;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Represents a chain of connected levels.
    /// </summary>
    public class CmpgnSeries
    {
        /// <summary>
        /// Stores the current level number. Begins at 1.
        /// </summary>
        public int LevelNum { get; set; }

        /// <summary>
        /// The base filename used to load levels.
        /// </summary>
        public string LevelFileName { get; set; }

        //The current game instance.
        private MainLoop game;

        /// <summary>
        /// A series of levels.
        /// </summary>
        public CmpgnSeries(MainLoop game, string baseFileName)
        {
            this.game = game;
            this.LevelNum = 1; //The first level to load.
            this.LevelFileName = baseFileName;
        }

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
    }
}
