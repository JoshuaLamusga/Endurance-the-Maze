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
    /// A coin lock that unlocks with a certain number of coins.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: The number of coins required to open
    /// the gate.
    /// 
    /// Custom properties of custInt2:
    /// 0: Does not subtract coins on contact.
    /// 1: Subtracts the coins on contact.
    /// 
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeCoinLock : GameObj
    {
        //Relevant assets.
        public static Texture2D texCoinLock { get; private set; }

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeCoinLock(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            isSolid = true;
            type = Type.CoinLock;

            //Sets sprite information.
            sprite = new Sprite(true, texCoinLock);
            sprite.depth = 0.410f;
            sprite.drawBehavior = SpriteDraw.all;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texCoinLock = Content.Load<Texture2D>("Content/Sprites/Game/sprCoinLock");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeCoinLock newBlock = new MazeCoinLock(game, x, y, layer);
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

        /// <summary>
        /// Deleted by actors with coins on contact. Handled by MazeActor.cs.
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Draws the sprite. Sets an informational tooltip.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            //Sets the tooltip to display information on hover.
            if (Sprite.isIntersecting(sprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                layer == game.mngrLvl.actor.layer)
            {
                game.mngrLvl.tooltip += "Coin Lock | ";
            }
        }
    }
}