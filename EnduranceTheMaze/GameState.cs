using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Represents different game states.
    /// </summary>
    public enum GameState
    {
        stateEditor, //the level editor.
        stateGameplay, //playing a custom or built-in level.
        stateGameplayEditor, //testing a level in the level editor.
        stateHowtoPlay, //the section listing how objects work.
        stateMenu, //the main menu.
        stateMenuEditor, //the edit menu.
        stateCampaignModes //the campaign levels to play.
    }
}