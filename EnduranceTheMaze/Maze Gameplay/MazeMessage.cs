using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Displays a message when hovering the mouse.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: The desired message.
    /// </summary>
    public class MazeMessage : GameObj
    {
        //Relevant assets.
        public static Texture2D texMessage { get; private set; }

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeMessage(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Message;

            //Sets sprite information.
            sprite = new Sprite(true, texMessage);
            sprite.depth = 0.209f;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texMessage = Content.Load<Texture2D>("Content/Sprites/Game/sprMessage");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeMessage newBlock = new MazeMessage(game, x, y, layer);
            newBlock.actionIndex = actionIndex;
            newBlock.actionIndex2 = actionIndex2;
            newBlock.actionType = actionType;
            newBlock.custInt1 = custInt1;
            newBlock.custInt2 = custInt2;
            newBlock.custStr = custStr;
            newBlock.dir = dir;
            newBlock.isActivated = isActivated;
            newBlock.isEnabled = isEnabled;
            newBlock.isVisible = isVisible;

            //Sets specific variables.
            newBlock.sprite = sprite;
            return newBlock;
        }

        public override void Update()
        {
            base.Update();

            //Shows a message when clicked.
            if (game.MsState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                Sprite.isIntersecting(sprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                layer == game.mngrLvl.actor.layer)
            {
                game.mngrLvl.isMessageShown = true;
                game.mngrLvl.message = custStr;
            }
        }

        /// <summary>
        /// Draws the sprite. Sets a custom tooltip.
        /// </summary>
        public override void Draw()
        {
            base.Draw();
        }
    }
}