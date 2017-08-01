using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Represents a button used to change properties of an active item.
    /// </summary>
    public class PropButton
    {
        //Relevant assets.
        public static Texture2D texPropActionInd1 { get; private set; }
        public static Texture2D texPropActionInd2 { get; private set; }
        public static Texture2D texPropActionType { get; private set; }
        public static Texture2D texPropCustInt1 { get; private set; }
        public static Texture2D texPropCustInt2 { get; private set; }
        public static Texture2D texPropCustStr { get; private set; }
        public static Texture2D texPropDir { get; private set; }
        public static Texture2D texPropIsEnabled { get; private set; }

        public static Texture2D texOpGameDelay { get; private set; }
        public static Texture2D texOpLvlLink { get; private set; }
        public static Texture2D texOpMaxSteps { get; private set; }
        public static Texture2D texOpMinGoals { get; private set; }
        public static Texture2D texOpSyncActors { get; private set; }
        public static Texture2D texOpSyncDeath { get; private set; }

        //Refers to the game instance.
        protected MainLoop game;

        //Contains a sprite and atlas.
        public Sprite sprite { get; protected set; }

        //Object location in pixels.
        public Vector2 pos { get; private set; }

        //If the button is hovered or clicked.
        public bool isHovered { get; private set; }

        //If the button is visible (buttons are invisible when the active item
        //cannot make use of them).
        public bool isVisible { get; internal set; }

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
            this.sprite = sprite;
            this.pos = pos;            
            isHovered = false;

            //Sets the sprite position.
            sprite.rectDest.X = pos.X;
            sprite.rectDest.Y = pos.Y;
        }

        ///<summary>
        ///Loads relevant graphics into memory.
        /// </summary>
        public static void LoadContent(ContentManager Content)
        {
            texPropActionInd1 = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropActionInd1");
            texPropActionInd2 = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropActionInd2");
            texPropActionType = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropActionType");
            texPropCustInt1 = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropCustInt1");
            texPropCustInt2 = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropCustInt2");
            texPropCustStr = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropCustStr");
            texPropDir = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropDir");
            texPropIsEnabled = Content.Load<Texture2D>("Content/Sprites/Gui/sprPropIsEnabled");
            texOpGameDelay = Content.Load<Texture2D>("Content/Sprites/Gui/sprOpGameDelay");
            texOpLvlLink = Content.Load<Texture2D>("Content/Sprites/Gui/sprOpLvlLink");
            texOpMaxSteps = Content.Load<Texture2D>("Content/Sprites/Gui/sprOpMaxSteps");
            texOpMinGoals = Content.Load<Texture2D>("Content/Sprites/Gui/sprOpMinGoals");
            texOpSyncActors = Content.Load<Texture2D>("Content/Sprites/Gui/sprOpSyncActors");
            texOpSyncDeath = Content.Load<Texture2D>("Content/Sprites/Gui/sprOpSyncDeath");
        }

        /// <summary>
        /// Updates the sprite atlas for sprites, esp. animated ones.
        /// </summary>
        public virtual void Update()
        {
            //If hovered, sets hovered to true. Else, sets it to false.
            if (game.MsState.X >= pos.X && game.MsState.X <= pos.X + 32 &&
                game.MsState.Y >= pos.Y && game.MsState.Y <= pos.Y + 32)
            {
                isHovered = true;
            }
            else
            {
                isHovered = false;
            }
        }

        /// <summary>
        /// Draws the sprite.
        /// </summary>
        public virtual void Draw()
        {
            if (isHovered)
            {
                sprite.color = Color.LightGreen;
            }
            else
            {
                sprite.color = Color.White;
            }

            sprite.Draw(game.GameSpriteBatch);
        }
    }
}