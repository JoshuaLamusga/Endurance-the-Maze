using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Handles the transition between campaign levels.
    /// </summary>
    public class MngrCampaign
    {
        #region Members
        /// <summary>
        /// Stores the number of times the update method is executed.
        /// </summary>
        private int messageTicks;

        /// <summary>
        /// The current game instance.
        /// </summary>
        private MainLoop game;

        /// <summary>
        /// The list of all built-in level series.
        /// </summary>
        private List<LevelSeries> seriesList;

        /// <summary>
        /// The current series of levels.
        /// </summary>
        private int seriesIndex;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an empty campaign manager.
        /// </summary>
        public MngrCampaign(MainLoop game)
        {
            messageTicks = 0;
            this.game = game;
            seriesList = new List<LevelSeries>();
            seriesIndex = 0;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Removes all series from the list.
        /// </summary>
        public void Clear()
        {
            seriesList.Clear();
        }

        /// <summary>
        /// Adds another level series to the end.
        /// </summary>
        public void AddSeries(LevelSeries series)
        {
            seriesList.Add(series);
        }

        /// <summary>
        /// Displays a message as a transition between series.
        /// </summary>
        public void Update()
        {
            messageTicks++;

            //Sends an event to end the message display.
            if (messageTicks == 100)
            {
                messageTicks = 0;
                NextSeries();
            }
        }

        /// <summary>
        /// Draws a message between series for a duration.
        /// </summary>
        public void Draw()
        {
            //Constructs a congratulatory message.
            string text = "Series complete! ";
            if (seriesList[seriesIndex] == game.LvlSeriesEasy)
            {
                text += "Onto bigger and better things.";
            }
            else if (seriesList[seriesIndex] == game.LvlSeriesNormal)
            {
                text += "Let's make things harder, shall we?";
            }
            else if (seriesList[seriesIndex] == game.LvlSeriesHard)
            {
                text += "What's harder than hard? Oh, this.";
            }
            else if (seriesList[seriesIndex] == game.LvlSeriesDoom)
            {
                text += "Thanks for playing! Consider making levels in the level editor.";
            }

            game.GameSpriteBatch.DrawString(game.fntBold,
                text,
                game.GetScreenSize() / 2 - (game.fntBold.MeasureString(text) / 2),
                Color.Black);
        }

        /// <summary>
        /// Advances to the next series, if there is one. Returns success.
        /// </summary>
        public bool NextSeries()
        {
            if (seriesIndex < seriesList.Count - 1)
            {
                seriesIndex++;

                //Goes to the first level of next series if possible.
                if (seriesList[seriesIndex].LevelExists())
                {
                    game.GmState = GameState.stateGameplay;
                    game.SetScreenCaption("Gameplay");
                    seriesList[seriesIndex].LoadCampaign();
                }
                else
                {
                    game.GmState = GameState.stateCampaignModes;
                }

                return true;
            }
            else
            {
                game.GmState = GameState.stateCampaignModes;
            }

            return false;
        }

        /// <summary>
        /// Sets the series if it exists in the list. Returns success.
        /// </summary>
        public bool SetSeries(LevelSeries series)
        {
            int foundIndex = seriesList.IndexOf(series);
            if (foundIndex == -1)
            {
                return false;
            }

            seriesIndex = foundIndex;

            //Goes to the first level of next series if possible.
            if (seriesList[seriesIndex].LevelExists())
            {
                game.GmState = GameState.stateGameplay;
                game.SetScreenCaption("Gameplay");
                seriesList[seriesIndex].LoadCampaign();
            }
            else
            {
                game.GmState = GameState.stateCampaignModes;
            }

            return true;
        }

        /// <summary>
        /// Returns the current level series.
        /// </summary>
        public LevelSeries CurrentSeries()
        {
            return seriesList[seriesIndex];
        }
        #endregion
    }
}
