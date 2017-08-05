using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Allows the player to finish the level on contact if enough
    /// maze goals have been acquired.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeFinish : GameObj
    {
        //Relevant assets.
        public static Texture2D TexFinish { get; private set; }

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeFinish(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            BlockType = Type.Finish;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexFinish);
            BlockSprite.depth = 0.417f;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexFinish = Content.Load<Texture2D>("Content/Sprites/Game/sprFinish");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeFinish newBlock = new MazeFinish(game, X, Y, Layer);
            newBlock.ActionIndex = ActionIndex;
            newBlock.ActionIndex2 = ActionIndex2;
            newBlock.ActionType = ActionType;
            newBlock.CustInt1 = CustInt1;
            newBlock.CustInt2 = CustInt2;
            newBlock.CustStr = CustStr;
            newBlock.BlockDir = BlockDir;
            newBlock.IsActivated = IsActivated;
            newBlock.IsEnabled = IsEnabled;
            newBlock.IsVisible = IsVisible;

            //Sets specific variables.
            newBlock.BlockSprite = BlockSprite;
            return newBlock;
        }

        /// <summary>
        /// Adds to the score and deletes itself on contact.
        /// </summary>
        public override void Update()
        {
            //Gets a list of all actors on the finish object.
            List<GameObj> items = game.mngrLvl.items.Where(o =>
                o.X == X && o.Y == Y && o.Layer == Layer &&
                o.BlockType == Type.Actor).ToList();

            //The player wins if they have enough goals and touch a finish.
            if (items.Count != 0)
            {
                if (game.mngrLvl.ActorGoals >= game.mngrLvl.OpReqGoals)
                {
                    game.mngrLvl.doWin = true;
                }
            }

            base.Update();
        }

        /// <summary>
        /// Draws the sprite. Sets an informational tooltip.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            //Sets the tooltip to display information on hover.
            if (Sprite.IsIntersecting(BlockSprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                Layer == game.mngrLvl.actor.Layer)
            {
                game.mngrLvl.tooltip += "Finish | ";
            }
        }
    }
}