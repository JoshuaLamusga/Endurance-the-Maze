using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        public Sprite BttnSprite { get; protected set; }
        public SpriteAtlas BttnSpriteAtlas { get; protected set; }

        //If the button is hovered or clicked.
        public bool IsHovered { get; protected set; }
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
            IsHovered = false;
            isClicked = false;

            //Sets up the relevant sprite.
            BttnSprite = new Sprite(true, tex);
            BttnSprite.rectDest.X = xPos;
            BttnSprite.rectDest.Y = yPos;
            BttnSprite.drawBehavior = SpriteDraw.all;

            BttnSpriteAtlas = new SpriteAtlas(BttnSprite, 133, 28, 10, 2, 5);
            BttnSpriteAtlas.frame = frame;
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
            if (Sprite.IsIntersecting(BttnSprite, new SmoothRect(
                game.MsState.X, game.MsState.Y, 1, 1)))
            {
                if (!IsHovered)
                {
                    IsHovered = true;

                    //Intentional truncation.
                    BttnSpriteAtlas.frame += BttnSpriteAtlas.atlasCols;
                    SfxPlaylist.Play(sndBttnHover);
                }
            }
            //If the mouse is no longer hovered.
            else if (IsHovered)
            {
                IsHovered = false;
                BttnSpriteAtlas.frame -= BttnSpriteAtlas.atlasCols;
            }
            //If the mouse is hovered and clicked.
            if (IsHovered && game.MsState.LeftButton ==
                ButtonState.Released && game.MsStateOld.LeftButton ==
                ButtonState.Pressed)
            {
                isClicked = true;
                SfxPlaylist.Play(sndBttnClick);
            }

            BttnSpriteAtlas.Update(true); //updates the atlas.
        }

        /// <summary>
        /// Draws the button.
        /// </summary>
        public void Draw()
        {
            BttnSprite.Draw(game.GameSpriteBatch);
        }
    }
}
