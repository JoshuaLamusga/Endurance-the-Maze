namespace EnduranceTheMaze
{
    /// <summary>
    /// Represents different game states.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// The level editor.
        /// </summary>
        stateEditor,

        /// <summary>
        /// Playing a built-in level.
        /// </summary>
        stateGameplay,

        /// <summary>
        /// Playing a level in the level editor.
        /// </summary>
        stateGameplayEditor,

        /// <summary>
        /// Briefly shows that the difficulty series is complete.
        /// </summary>
        stateGameplaySeriesComplete,

        /// <summary>
        /// How to play submenu.
        /// </summary>
        stateHowtoPlay,

        /// <summary>
        /// The main menu.
        /// </summary>
        stateMenu,

        /// <summary>
        /// The edit menu.
        /// </summary>
        stateMenuEditor,

        /// <summary>
        /// The campaign levels.
        /// </summary>
        stateCampaignModes
    }
}