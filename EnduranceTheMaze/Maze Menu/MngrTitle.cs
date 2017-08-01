using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace EnduranceTheMaze
{
    /// <summary>
    /// The menu manager. Handles all menu logic.
    /// 
    /// Dependencies: MainLoop.cs, sprMenu textures.
    /// </summary>
    public class MngrTitle
    {
        //Refers to the game instance.
        private MainLoop game;

        //Relevant assets.
        private static Texture2D texCopyright;
        public static Texture2D texBttnMain { get; private set; }
        public static Texture2D texBttnEdit { get; private set; }
        public static Texture2D texBttnCmpgn { get; private set; }
        public static Texture2D texMenuTitle { get; private set; }
        public static Texture2D texMenuOptions { get; private set; }
        public static Texture2D texMenuInfo1 { get; private set; }
        public static Texture2D texMenuInfo2 { get; private set; }
        public static Texture2D texMenuInfo3 { get; private set; }

        //The title, options section, and how to play.
        Sprite sprCopyright, sprTitle, sprMenuOptions, sprMenuInfo;

        //The menu buttons.
        TitleItemMain bttnCampaign, bttnLevelEditor, bttnHowToPlay, bttnMuteSfx,
            bttnBack;

        //The level editor buttons.
        TitleItemEdit bttnEdit, bttnTest, bttnSave, bttnLoad, bttnClear;

        //The campaign buttons.
        TitleItemCmpgn bttnCmpgnEasy, bttnCmpgnNormal, bttnCmpgnHard,
            bttnCmpgnDoom;

        //The current page of the how to play screen; used when active.
        private int _infoPage;

        /// <summary>
        /// Sets the game instance.
        /// </summary>
        /// <param name="game">The game instance to use.</param>
        public MngrTitle(MainLoop game)
        {
            this.game = game;
            _infoPage = 0; //Sets the current information page.
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// 
        /// Dependencies: MainLoop.cs, sprMenu textures, screen size.
        /// </summary>
        public void LoadContent()
        {
            //Loads the button textures.
            texBttnMain = TitleItemMain.LoadContent(game.Content);
            texBttnEdit = TitleItemEdit.LoadContent(game.Content);
            texBttnCmpgn = TitleItemCmpgn.LoadContent(game.Content);

            //Sets up relevant textures.
            texCopyright = game.Content.Load<Texture2D>("Content/Sprites/Gui/sprCopyright");
            texMenuTitle = game.Content.Load<Texture2D>("Content/Sprites/Gui/sprMenuTitle");
            texMenuOptions = game.Content.Load<Texture2D>("Content/Sprites/Gui/sprMenuOptions");
            texMenuInfo1 = game.Content.Load<Texture2D>("Content/Sprites/Gui/sprMenuInfo1");
            texMenuInfo2 = game.Content.Load<Texture2D>("Content/Sprites/Gui/sprMenuInfo2");
            texMenuInfo3 = game.Content.Load<Texture2D>("Content/Sprites/Gui/sprMenuInfo3");

            //Creates buttons after the textures have loaded.
            bttnCampaign = new TitleItemMain
                (game, texBttnMain, 334, 96, 0);
            bttnLevelEditor = new TitleItemMain
                (game, texBttnMain, 334, 142, 1);
            bttnHowToPlay = new TitleItemMain
                (game, texBttnMain, 334, 188, 2);
            bttnMuteSfx = new TitleItemMain
                (game, texBttnMain, 334, 280, 4);
            bttnBack = new TitleItemMain
                (game, texBttnMain, 339, 0, 3);

            bttnEdit = new TitleItemEdit(game, texBttnEdit, 378, 96, 0);
            bttnTest = new TitleItemEdit(game, texBttnEdit, 378, 142, 1);
            bttnSave = new TitleItemEdit(game, texBttnEdit, 378, 188, 2);
            bttnLoad = new TitleItemEdit(game, texBttnEdit, 378, 234, 3);
            bttnClear = new TitleItemEdit(game, texBttnEdit, 378, 280, 4);

            bttnCmpgnEasy = new TitleItemCmpgn(game, texBttnCmpgn, 360, 32, 0);
            bttnCmpgnNormal = new TitleItemCmpgn(game, texBttnCmpgn, 360, 132, 1);
            bttnCmpgnHard = new TitleItemCmpgn(game, texBttnCmpgn, 360, 232, 2);
            bttnCmpgnDoom = new TitleItemCmpgn(game, texBttnCmpgn, 360, 332, 3);

            //Creates the copyright sprite.
            sprCopyright = new Sprite(true, texCopyright);
            sprCopyright.rectDest.Y = game.GetScreenSize().Y - 16;
            sprCopyright.rectDest.X =
                game.GetScreenSize().X / 2 -
                (sprCopyright.rectDest.Width / 2);            

            //Creates the title sprite.
            sprTitle = new Sprite(true, texMenuTitle);            
            sprTitle.rectDest.X = 194; //(scr. width - img. width) / 2
            sprTitle.rectDest.Y = 8; //img. width / 2
            
            //Creates the options sprite.
            sprMenuOptions = new Sprite(true, texMenuOptions);
            sprMenuOptions.rectDest.X = 203; //(scr. width - img. width) / 2
            sprMenuOptions.rectDest.Y = 248;
            
            //Creates the info sprite.
            sprMenuInfo = new Sprite(true, texMenuInfo1);
            sprMenuInfo.rectDest.Y = 28; //Room for a back button.
        }

        /// <summary>
        /// Updates button logic and handles click actions.
        /// </summary>
        public void Update()
        {
            //Executes button logic relevant to the active state.
            switch (game.GmState)
            {
                //If the main screen is active.
                case GameState.stateMenu:
                    //Updates the titlescreen buttons.
                    bttnCampaign.Update();
                    bttnLevelEditor.Update();
                    bttnHowToPlay.Update();
                    bttnMuteSfx.Update();

                    if (bttnCampaign.isClicked)
                    {
                        bttnCampaign.isClicked = false;

                        game.GmState = GameState.stateCampaignModes;
                        game.SetScreenCaption("Gameplay Modes");
                    }
                    else if (bttnLevelEditor.isClicked)
                    {
                        bttnLevelEditor.isClicked = false;
                        game.GmState = GameState.stateMenuEditor;
                        game.SetScreenCaption("Level editor");
                    }
                    else if (bttnHowToPlay.isClicked)
                    {
                        bttnHowToPlay.isClicked = false;
                        game.GmState = GameState.stateHowtoPlay;
                        game.SetScreenCaption("How to play");
                    }
                    else if (bttnMuteSfx.isClicked)
                    {
                        bttnMuteSfx.isClicked = false;
                        game.isSoundMuted = !game.isSoundMuted;
                        MediaPlayer.IsMuted = game.isSoundMuted;
                        SoundEffect.MasterVolume =
                            Convert.ToInt32(!game.isSoundMuted);
                    }
                    break;
                //If the how to play screen is active.
                case GameState.stateHowtoPlay:
                    bttnBack.Update();

                    //Goes back one page.
                    if (game.KbState.IsKeyDown(Keys.Left) &&
                        game.KbStateOld.IsKeyUp(Keys.Left))
                    {
                        if (_infoPage > 0)
                        {
                            _infoPage--;
                        }
                    }

                    //Goes forward one page.
                    if (game.KbState.IsKeyDown(Keys.Right) &&
                        game.KbStateOld.IsKeyUp(Keys.Right))
                    {
                        if (_infoPage < 2) //max page here.
                        {
                            _infoPage++;
                        }
                    }

                    //Determines the texture for the info sprite.
                    switch (_infoPage)
                    {
                        case 0:
                            if (sprMenuInfo.texture != texMenuInfo1)
                            {
                                sprMenuInfo.SetTexture(true, texMenuInfo1);
                            }
                            break;
                        case 1:
                            if (sprMenuInfo.texture != texMenuInfo2)
                            {
                                sprMenuInfo.SetTexture(true, texMenuInfo2);
                            }
                            break;
                        case 2:
                            if (sprMenuInfo.texture != texMenuInfo3)
                            {
                                sprMenuInfo.SetTexture(true, texMenuInfo3);
                            }
                            break;
                    }

                    //If back is pressed.
                    if (bttnBack.isClicked)
                    {
                        bttnBack.isClicked = false;
                        game.GmState = GameState.stateMenu;
                        game.SetScreenCaption("Main menu");
                    }
                    break;
                //If the edit screen is active.
                case GameState.stateMenuEditor:
                    //Updates the editing buttons.
                    bttnBack.Update();
                    bttnEdit.Update();
                    bttnTest.Update();
                    bttnSave.Update();
                    bttnLoad.Update();
                    bttnClear.Update();

                    if (bttnEdit.isClicked)
                    {
                        bttnEdit.isClicked = false;
                        game.GmState = GameState.stateEditor;
                        game.SetScreenCaption("Level Editor");
                    }
                    else if (bttnTest.isClicked)
                    {
                        bttnTest.isClicked = false;

                        //If there is at least one actor block.
                        if (game.mngrEditor.items.Count > 0 &&
                        game.mngrEditor.items.Where(o =>
                        o.type == Type.Actor).Count() > 0)
                        {
                            //Loads the level.
                            game.mngrEditor.LoadTest();

                            //Loads level settings.
                            game.mngrLvl._countdownStart =
                                game.mngrEditor.opGameDelay;
                            game.mngrLvl.opLvlLink =
                                game.mngrEditor.opLvlLink;
                            game.mngrLvl.opMaxSteps =
                                game.mngrEditor.opMaxSteps;
                            game.mngrLvl.opReqGoals =
                                game.mngrEditor.opMinGoals;
                            game.mngrLvl.opSyncActors =
                                game.mngrEditor.opSyncActors;
                            game.mngrLvl.opSyncDeath =
                                game.mngrEditor.opSyncDeath;

                            game.GmState = GameState.stateGameplayEditor;
                            game.SetScreenCaption("Level Editor");
                        }
                    }
                    else if (bttnSave.isClicked)
                    {
                        bttnSave.isClicked = false;

                        if (game.mngrEditor.items.Count > 0 &&
                        game.mngrEditor.items.Where(o =>
                        o.type == Type.Actor).Count() > 0)
                        {
                            game.mngrEditor.LevelSave();
                        }
                    }
                    else if (bttnLoad.isClicked)
                    {
                        bttnLoad.isClicked = false;

                        game.mngrEditor.LoadEdit();
                    }
                    else if (bttnClear.isClicked)
                    {
                        bttnClear.isClicked = false;

                        if (game.mngrEditor.items.Count > 0)
                        {
                            //Clears the block list.
                            game.mngrEditor.items = new List<ImgBlock>();

                            //Resets camera position.
                            game.mngrEditor.camX = 0;
                            game.mngrEditor.camY = 0;
                            game.mngrEditor.camLayer = 0;
                        }
                    }
                    else if (bttnBack.isClicked)
                    {
                        bttnBack.isClicked = false;
                        game.GmState = GameState.stateMenu;
                        game.SetScreenCaption("Main menu");
                    }
                    break;
                //If the campaign mode screen is active.
                case GameState.stateCampaignModes:
                    //Updates the editing buttons.
                    bttnBack.Update();

                    bttnCmpgnEasy.Update();
                    bttnCmpgnNormal.Update();
                    bttnCmpgnHard.Update();
                    bttnCmpgnDoom.Update();

                    if (bttnCmpgnEasy.isClicked)
                    {
                        bttnCmpgnEasy.isClicked = false;

                        //After completing the series, click to restart it.
                        if (!game.LvlSeriesEasy.LevelExists())
                        {
                            game.LvlSeriesEasy.levelNum = 1;
                        }

                        game.currentSeries = game.LvlSeriesEasy;
                        game.GmState = GameState.stateGameplay;
                        game.SetScreenCaption("Gameplay");
                        game.currentSeries.LoadCampaign();
                    }
                    else if (bttnCmpgnNormal.isClicked)
                    {
                        bttnCmpgnNormal.isClicked = false;

                        if (!game.LvlSeriesNormal.LevelExists())
                        {
                            game.LvlSeriesNormal.levelNum = 1;
                        }

                        game.currentSeries = game.LvlSeriesNormal;
                        game.GmState = GameState.stateGameplay;
                        game.SetScreenCaption("Gameplay");
                        game.currentSeries.LoadCampaign();
                    }
                    else if (bttnCmpgnHard.isClicked)
                    {
                        bttnCmpgnHard.isClicked = false;

                        if (!game.LvlSeriesHard.LevelExists())
                        {
                            game.LvlSeriesHard.levelNum = 1;
                        }

                        game.currentSeries = game.LvlSeriesHard;
                        game.GmState = GameState.stateGameplay;
                        game.SetScreenCaption("Gameplay");
                        game.currentSeries.LoadCampaign();
                    }
                    else if (bttnCmpgnDoom.isClicked)
                    {
                        bttnCmpgnDoom.isClicked = false;

                        if (!game.LvlSeriesDoom.LevelExists())
                        {
                            game.LvlSeriesDoom.levelNum = 1;
                        }

                        game.currentSeries = game.LvlSeriesDoom;
                        game.GmState = GameState.stateGameplay;
                        game.SetScreenCaption("Gameplay");
                        game.currentSeries.LoadCampaign();
                    }
                    else if (bttnBack.isClicked)
                    {
                        bttnBack.isClicked = false;
                        game.GmState = GameState.stateMenu;
                        game.SetScreenCaption("Main menu");
                    }
                    break;
            }
        }

        public void Draw()
        {
            switch (game.GmState)
            {
                //If the main screen is active.
                case GameState.stateMenu:
                    bttnCampaign.Draw();
                    bttnHowToPlay.Draw();
                    bttnLevelEditor.Draw();
                    bttnMuteSfx.Draw();
                    sprMenuOptions.Draw(game.GameSpriteBatch);
                    sprCopyright.Draw(game.GameSpriteBatch);
                    sprTitle.Draw(game.GameSpriteBatch);
                    break;
                //If the how to play screen is active.
                case GameState.stateHowtoPlay:
                    bttnBack.Draw();
                    sprMenuInfo.Draw(game.GameSpriteBatch);
                    sprCopyright.Draw(game.GameSpriteBatch);
                    break;
                //If the edit screen is active.
                case GameState.stateMenuEditor:
                    bttnBack.Draw();
                    bttnEdit.Draw();
                    bttnTest.Draw();
                    bttnSave.Draw();
                    bttnLoad.Draw();
                    bttnClear.Draw();
                    sprCopyright.Draw(game.GameSpriteBatch);
                    break;
                //If the campaign mode screen is active.
                case GameState.stateCampaignModes:
                    bttnBack.Draw();
                    bttnCmpgnEasy.Draw();
                    bttnCmpgnNormal.Draw();
                    bttnCmpgnHard.Draw();
                    bttnCmpgnDoom.Draw();

                    //Draws the current level numbers.
                    if (game.LvlSeriesEasy.LevelExists())
                    {
                        game.GameSpriteBatch.DrawString(game.fntBold,
                            "On level: " + game.LvlSeriesEasy.levelNum,
                            new Vector2(
                                bttnCmpgnEasy.sprite.rectDest.X +
                                bttnCmpgnEasy.sprite.rectDest.Width + 4,
                                bttnCmpgnEasy.sprite.rectDest.Y +
                                (bttnCmpgnEasy.sprite.rectDest.Height / 2)),
                            Color.Black);
                    }
                    else
                    {
                        game.GameSpriteBatch.DrawString(game.fntBold,
                            "Completed!",
                            new Vector2(
                                bttnCmpgnEasy.sprite.rectDest.X +
                                bttnCmpgnEasy.sprite.rectDest.Width + 4,
                                bttnCmpgnEasy.sprite.rectDest.Y +
                                (bttnCmpgnEasy.sprite.rectDest.Height / 2)),
                            Color.Green);
                    }

                    if (game.LvlSeriesNormal.LevelExists())
                    {
                        game.GameSpriteBatch.DrawString(game.fntBold,
                            "On level: " + game.LvlSeriesNormal.levelNum,
                            new Vector2(
                                bttnCmpgnNormal.sprite.rectDest.X +
                                bttnCmpgnNormal.sprite.rectDest.Width + 4,
                                bttnCmpgnNormal.sprite.rectDest.Y +
                                (bttnCmpgnNormal.sprite.rectDest.Height / 2)),
                            Color.Black);
                    }
                    else
                    {
                        game.GameSpriteBatch.DrawString(game.fntBold,
                            "Completed!",
                            new Vector2(
                                bttnCmpgnNormal.sprite.rectDest.X +
                                bttnCmpgnNormal.sprite.rectDest.Width + 4,
                                bttnCmpgnNormal.sprite.rectDest.Y +
                                (bttnCmpgnNormal.sprite.rectDest.Height / 2)),
                            Color.Green);
                    }

                    if (game.LvlSeriesHard.LevelExists())
                    {
                        game.GameSpriteBatch.DrawString(game.fntBold,
                            "On level: " + game.LvlSeriesHard.levelNum,
                            new Vector2(
                                bttnCmpgnHard.sprite.rectDest.X +
                                bttnCmpgnHard.sprite.rectDest.Width + 4,
                                bttnCmpgnHard.sprite.rectDest.Y +
                                (bttnCmpgnHard.sprite.rectDest.Height / 2)),
                            Color.Black);
                    }
                    else
                    {
                        game.GameSpriteBatch.DrawString(game.fntBold,
                            "Completed!",
                            new Vector2(
                                bttnCmpgnHard.sprite.rectDest.X +
                                bttnCmpgnHard.sprite.rectDest.Width + 4,
                                bttnCmpgnHard.sprite.rectDest.Y +
                                (bttnCmpgnHard.sprite.rectDest.Height / 2)),
                            Color.Green);
                    }

                    if (game.LvlSeriesDoom.LevelExists())
                    {
                        game.GameSpriteBatch.DrawString(game.fntBold,
                            "On level: " + game.LvlSeriesDoom.levelNum,
                            new Vector2(
                                bttnCmpgnDoom.sprite.rectDest.X +
                                bttnCmpgnDoom.sprite.rectDest.Width + 4,
                                bttnCmpgnDoom.sprite.rectDest.Y +
                                (bttnCmpgnDoom.sprite.rectDest.Height / 2)),
                            Color.Black);
                    }
                    else
                    {
                        game.GameSpriteBatch.DrawString(game.fntBold,
                            "Completed!",
                            new Vector2(
                                bttnCmpgnDoom.sprite.rectDest.X +
                                bttnCmpgnDoom.sprite.rectDest.Width + 4,
                                bttnCmpgnDoom.sprite.rectDest.Y +
                                (bttnCmpgnDoom.sprite.rectDest.Height / 2)),
                            Color.Green);
                    }

                    sprCopyright.Draw(game.GameSpriteBatch);
                    break;
            }
        }
    }
}