using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Handles the logic and display of all items on the screen. Uses a tree
    /// structure, such that the update and drawing of items passes through
    /// many layers of sub-items.
    /// </summary>
    public class MainLoop : Game
    {
        //Handles graphic display and sprite drawing; respectively.
        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch GameSpriteBatch { get; private set; }

        //Contains the current game state.
        public GameState GmState { get; set; }

        //Contains all keyboard and mouse input.
        //Mouse is set by MngrEditor to avoid block placement.
        public MouseState MsState { get; internal set; }
        public MouseState MsStateOld { get; private set; }
        public KeyboardState KbState { get; private set; }
        public KeyboardState KbStateOld { get; private set; }

        //Sets up objects to control all aspects of the game.
        public SpriteFont fntDefault, fntBold;
        public MngrTitle mngrTitle;
        public MngrLvl mngrLvl;
        public MngrEditor mngrEditor;
        public MngrCampaign mngrCampaign;
        public LevelSeries LvlSeriesEasy { get; private set; }
        public LevelSeries LvlSeriesNormal { get; private set; }
        public LevelSeries LvlSeriesHard { get; private set; }
        public LevelSeries LvlSeriesDoom { get; private set; }

        //Sets up music.
        public SfxPlaylist playlist;
        public SoundEffect sndSong1, sndSong2, sndSong3, sndSong4;

        //If sound and music are audible or not.
        public bool isSoundMuted;

        /// <summary>
        /// Sets up variables and basic preferences.
        /// </summary>
        public MainLoop()
        {
            //Sets up xna components.
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            GmState = GameState.stateMenu; //set to main menu.
            SetScreenCaption("Main Menu"); //Indicates the current state.
            IsMouseVisible = true; //Makes the mouse visible.
            isSoundMuted = false; //Makes sound audible.

            //Centers the window.
            Window.Position = Window.ClientBounds.Center;

            //Sets up keyboard and mouse input.
            MsStateOld = MsState = Mouse.GetState();
            KbStateOld = KbState = Keyboard.GetState();

            //Initializes objects to control all aspects of the game.
            mngrTitle = new MngrTitle(this);
            mngrLvl = new MngrLvl(this);
            mngrEditor = new MngrEditor(this);
            
            LvlSeriesEasy = new LevelSeries(this, "EnduranceTheMaze.Levels.LvlEasy");
            LvlSeriesNormal = new LevelSeries(this, "EnduranceTheMaze.Levels.LvlNormal");
            LvlSeriesHard = new LevelSeries(this, "EnduranceTheMaze.Levels.LvlHard");
            LvlSeriesDoom = new LevelSeries(this, "EnduranceTheMaze.Levels.LvlDoom");

            mngrCampaign = new MngrCampaign(this);
            mngrCampaign.AddSeries(LvlSeriesEasy);
            mngrCampaign.AddSeries(LvlSeriesNormal);
            mngrCampaign.AddSeries(LvlSeriesHard);
            mngrCampaign.AddSeries(LvlSeriesDoom);

            //Initializes an empty playlist.
            playlist = new SfxPlaylist(this);
        }

        #region Xna methods (init, load content, update, draw)
        /// <summary>
        /// Sets up xna components.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Loads all graphics into memory.
        /// </summary>
        protected override void LoadContent()
        {
            //Creates the spriteBatch that draws all sprites.
            GameSpriteBatch = new SpriteBatch(GraphicsDevice);

            //Loads all content into the pipeline.
            fntDefault = Content.Load<SpriteFont>("Content/Fonts/fntDefault");
            fntBold = Content.Load<SpriteFont>("Content/Fonts/fntBold");
            mngrTitle.LoadContent();
            mngrLvl.LoadContent(Content);
            mngrEditor.LoadContent();

            //Loads music.
            sndSong1 = Content.Load<SoundEffect>("Content/Music/sndSong1");
            sndSong2 = Content.Load<SoundEffect>("Content/Music/sndSong2");
            sndSong3 = Content.Load<SoundEffect>("Content/Music/sndSong3");
            sndSong4 = Content.Load<SoundEffect>("Content/Music/sndSong4");

            //Populates the background music playlist and starts it.
            playlist.sounds.Add(sndSong1);
            playlist.sounds.Add(sndSong2);
            playlist.sounds.Add(sndSong3);
            playlist.sounds.Add(sndSong4);
            playlist.Update();
        }

        /// <summary>
        /// Performs update logic.
        /// </summary>
        /// <param name="gameTime">The internal game time.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime); //underlying xna component call.

            //Updates the input.
            MsStateOld = MsState;
            KbStateOld = KbState;
            MsState = Mouse.GetState();
            KbState = Keyboard.GetState();

            //Allows the player to navigate the rooms.
            if (KbState.IsKeyDown(Keys.Escape) &&
                KbStateOld.IsKeyUp(Keys.Escape))
            {
                //Goes from playing campaign to the campaign modes.
                if (GmState == GameState.stateGameplay ||
                    GmState == GameState.stateGameplaySeriesComplete)
                {
                    GmState = GameState.stateCampaignModes;
                    SetScreenCaption("Campaign");
                }
                //Goes from testing levels to the editor menu.
                else if (GmState == GameState.stateGameplayEditor ||
                    GmState == GameState.stateEditor)
                {
                    GmState = GameState.stateMenuEditor;
                    SetScreenCaption("Editor");
                }
                //Goes from submenus to the main menu.
                else if (GmState == GameState.stateCampaignModes ||
                    GmState == GameState.stateHowtoPlay ||
                    GmState == GameState.stateMenuEditor)
                {
                    GmState = GameState.stateMenu;
                    SetScreenCaption("Main menu");
                }
            }

            //Updates the game relevant to the active state.
            switch (GmState)
            {
                case GameState.stateMenu:
                case GameState.stateHowtoPlay:
                case GameState.stateMenuEditor:
                case GameState.stateCampaignModes:
                    mngrTitle.Update();
                    break;
                case GameState.stateEditor:
                    mngrEditor.Update();
                    break;
                case GameState.stateGameplay:
                case GameState.stateGameplayEditor:
                    mngrLvl.Update();
                    break;
                case GameState.stateGameplaySeriesComplete:
                    mngrCampaign.Update();
                    break;
            }

            //Switches music when songs end.
            playlist.Update();
        }

        /// <summary>
        /// Performs drawing logic.
        /// </summary>
        /// <param name="gameTime">The internal game time.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Ensures drawing over consecutive frames is clean.
            GraphicsDevice.Clear(Color.White);

            //Draws with screen translation if the state is gameplay.
            if (GmState == GameState.stateGameplay ||
                GmState == GameState.stateGameplayEditor)
            {
                GameSpriteBatch.Begin(SpriteSortMode.Deferred, null, null,
                    null, null, null, mngrLvl.Camera);
            }
            else if (GmState == GameState.stateEditor)
            {
                GameSpriteBatch.Begin(SpriteSortMode.Deferred, null, null,
                    null, null, null, mngrEditor.Camera);
            }
            else
            {
                GameSpriteBatch.Begin();
            }

            //Iterates through each game state and draws the set of objects
            //relevant to the active state.
            switch (GmState)
            {
                case GameState.stateMenu:
                case GameState.stateHowtoPlay:
                case GameState.stateMenuEditor:
                case GameState.stateCampaignModes:
                    mngrTitle.Draw();
                    break;
                case GameState.stateEditor:
                    mngrEditor.Draw();

                    //Draws static sprites afterwards.
                    GameSpriteBatch.End();
                    GameSpriteBatch.Begin();
                    mngrEditor.DrawHud();
                    break;
                case GameState.stateGameplay:
                case GameState.stateGameplayEditor:
                    mngrLvl.Draw();

                    //Draws static sprites afterwards.
                    GameSpriteBatch.End();
                    GameSpriteBatch.Begin();
                    mngrLvl.DrawHud();
                    break;
                case GameState.stateGameplaySeriesComplete:
                    mngrCampaign.Draw();
                    break;
            }

            GameSpriteBatch.End();
        }
        #endregion

        #region Convenience methods
        /// <summary>
        /// Returns the screen width and height as a vector2.
        /// "X" is the width and "Y" is the height.
        /// </summary>
        public Vector2 GetScreenSize()
        {
            return new Vector2(Graphics.PreferredBackBufferWidth,
                Graphics.PreferredBackBufferHeight);
        }

        /// <summary>
        /// Returns the visible bounds of the screen in world space as
        /// a rectangle.
        /// </summary>
        public Rectangle GetVisibleBounds(Matrix camera, float zoom)
        {
            int xPos = (int)Vector2.Transform(Vector2.Zero, Matrix.Invert(camera)).X;
            int yPos = (int)Vector2.Transform(Vector2.Zero, Matrix.Invert(camera)).Y;
            int width = (int)Math.Ceiling(GetScreenSize().X * (1 / zoom));
            int height = (int)Math.Ceiling(GetScreenSize().Y * (1 / zoom));
            return new Rectangle(xPos, yPos, width, height);
        }

        /// <summary>
        /// Sets the screen's title.
        /// </summary>
        public void SetScreenCaption(string title)
        {
            Window.Title = title;
        }
        #endregion
    }
}