using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Represents a button, recording hovering and clicking for
    /// inheriting classes to perform actions.
    /// </summary>
    public class TitleItemMain
    {
        //Relevant assets.
        public static SoundEffect sndBttnClick, sndBttnHover;

        //Refers to the game instance.
        private MainLoop game;

        //The button image and atlas.
        public Sprite sprite { get; protected set; }
        public SpriteAtlas spriteAtlas { get; protected set; }

        //If the button is hovered or clicked.
        public bool isHovered { get; protected set; }
        public bool isClicked;

        /// <summary>
        /// Sets up a new button object.
        /// 
        /// Dependencies: texGuiItem.
        /// </summary>
        /// <param name="xPos">The x-location.</param>
        /// <param name="yPos">The y-location.</param>
        /// <param name="frame">
        /// The frame to use.
        /// </param>
        public TitleItemMain(MainLoop game, Texture2D tex, float xPos,
            float yPos, int frame)
        {
            //Sets the game instance.
            this.game = game;

            //Sets up detectors.
            isHovered = false;
            isClicked = false;

            //Sets up the relevant sprite.
            sprite = new Sprite(true, tex);
            sprite.rectDest.X = xPos;
            sprite.rectDest.Y = yPos;
            sprite.drawBehavior = SpriteDraw.all;

            spriteAtlas = new SpriteAtlas(sprite, 133, 28, 10, 2, 5);
            spriteAtlas.frame = frame;
        }

        /// <summary>
        /// Loads and returns the relevant graphics into memory.
        /// </summary>
        public static Texture2D LoadContent(ContentManager Content)
        {
            //Loads the sound.
            sndBttnHover = Content.Load<SoundEffect>("Content/Sounds/sndBttnHover");
            sndBttnClick = Content.Load<SoundEffect>("Content/Sounds/sndBttnClick");

            return Content.Load<Texture2D>("Content/Sprites/Gui/sprBttnMain");
        }
        
        /// <summary>
        /// Runs through detecting (hover/click) logic.
        /// </summary>
        public void Update()
        {
            //If the mouse becomes hovered.
            if (Sprite.isIntersecting(sprite, new SmoothRect(
                game.MsState.X, game.MsState.Y, 1, 1)))
            {
                if (!isHovered)
                {
                    isHovered = true;

                    //Intentional truncation.
                    spriteAtlas.frame += spriteAtlas.atlasCols;
                    SfxPlaylist.Play(sndBttnHover);
                }
            }
            //If the mouse is no longer hovered.
            else if (isHovered)
            {
                isHovered = false;
                spriteAtlas.frame -= spriteAtlas.atlasCols;
            }
            //If the mouse is hovered and clicked.
            if (isHovered && game.MsStateOld.LeftButton ==
                ButtonState.Released && game.MsState.LeftButton ==
                ButtonState.Pressed)
            {
                isClicked = true;
                SfxPlaylist.Play(sndBttnClick);
            }

            spriteAtlas.Update(true); //updates the atlas.
        }

        /// <summary>
        /// Draws the button.
        /// </summary>
        public void Draw()
        {
            sprite.Draw(game.GameSpriteBatch);
        }
    }
}
