using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Represents a button used to change properties of an active item.
    /// </summary>
    public class PropButton
    {
        //Relevant assets.
        public static Texture2D TexPropActionInd1 { get; private set; }
        public static Texture2D TexPropActionInd2 { get; private set; }
        public static Texture2D TexPropActionType { get; private set; }
        public static Texture2D TexPropCustInt1 { get; private set; }
        public static Texture2D TexPropCustInt2 { get; private set; }
        public static Texture2D TexPropCustStr { get; private set; }
        public static Texture2D TexPropDir { get; private set; }
        public static Texture2D TexPropIsEnabled { get; private set; }

        public static Texture2D TexOpGameDelay { get; private set; }
        public static Texture2D TexOpLvlLink { get; private set; }
        public static Texture2D TexOpMaxSteps { get; private set; }
        public static Texture2D TexOpMinGoals { get; private set; }
        public static Texture2D TexOpSyncActors { get; private set; }
        public static Texture2D TexOpSyncDeath { get; private set; }

        //Refers to the game instance.
        protected MainLoop game;

        //Contains a sprite and atlas.
        public Sprite BttnSprite { get; protected set; }

        //Object location in pixels.
        public Vector2 Pos { get; private set; }

        //If the button is hovered or clicked.
        public bool IsHovered { get; private set; }

        //If the button is visible (buttons are invisible when the active item
        //cannot make use of them).
        public bool IsVisible { get; internal set; }

        /// <summary>
        /// Sets all values.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="pos">The position of the button.</param>
        /// <param name="sprite">A sprite with a texture.</param>
        public PropButton(MainLoop game, Sprite sprite, Vector2 pos)
        {
            this.game = game;

            //Sets default values.
            this.BttnSprite = sprite;
            this.Pos = pos;            
            IsHovered = false;

            //Sets the sprite position.
            sprite.rectDest.X = pos.X;
            sprite.rectDest.Y = pos.Y;
        }

        ///<summary>
        ///Loads relevant graphics into memory.
        /// </summary>
        public static void LoadContent(ContentManager Content)
        {
            TexPropActionInd1 = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropActionInd1");
            TexPropActionInd2 = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropActionInd2");
            TexPropActionType = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropActionType");
            TexPropCustInt1 = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropCustInt1");
            TexPropCustInt2 = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropCustInt2");
            TexPropCustStr = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropCustStr");
            TexPropDir = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropDir");
            TexPropIsEnabled = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropIsEnabled");
            TexOpGameDelay = Content.Load<Texture2D>("Content/Sprites/Gui/sprOpGameDelay");
            TexOpLvlLink = Content.Load<Texture2D>("Content/Sprites/Gui/sprOpLvlLink");
            TexOpMaxSteps = Content.Load<Texture2D>("Content/Sprites/Gui/sprOpMaxSteps");
            TexOpMinGoals = Content.Load<Texture2D>("Content/Sprites/Gui/sprOpMinGoals");
            TexOpSyncActors = Content.Load<Texture2D>("Content/Sprites/Gui/sprOpSyncActors");
            TexOpSyncDeath = Content.Load<Texture2D>("Content/Sprites/Gui/sprOpSyncDeath");
        }

        /// <summary>
        /// Updates the sprite atlas for sprites, esp. animated ones.
        /// </summary>
        public virtual void Update()
        {
            //If hovered, sets hovered to true. Else, sets it to false.
            if (game.MsState.X >= Pos.X && game.MsState.X <= Pos.X + 32 &&
                game.MsState.Y >= Pos.Y && game.MsState.Y <= Pos.Y + 32)
            {
                IsHovered = true;
            }
            else
            {
                IsHovered = false;
            }
        }

        /// <summary>
        /// Draws the sprite.
        /// </summary>
        public virtual void Draw()
        {
            if (IsHovered)
            {
                BttnSprite.color = Color.LightGreen;
            }
            else
            {
                BttnSprite.color = Color.White;
            }

            BttnSprite.Draw(game.GameSpriteBatch);
        }
    }
}