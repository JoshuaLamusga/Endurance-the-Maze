namespace EnduranceTheMaze
{
    /// <summary>
    /// Decoration for a broken crate.
    /// Dependencies: MazeCrate and its texture.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeCrateBroken : GameObj
    {
        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeCrateBroken(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            BlockType = Type.CrateBroken;

            //Sets sprite information.
            BlockSprite = new Sprite(true, MazeCrate.TexCrate);
            BlockSprite.depth = 0.301f;
            BlockSprite.rectSrc = new SmoothRect(32, 0, 32, 32);
            BlockSprite.rectDest.Width = 32;
            BlockSprite.rectDest.Height = 32;
            BlockSprite.drawBehavior = SpriteDraw.basicAnimated;
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeCrateBroken newBlock = new MazeCrateBroken(game, X, Y, Layer);
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
            return newBlock;
        }

        /// <summary>
        /// Draws the sprite. Sets the tooltip.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            //Sets the tooltip to display information on hover.
            if (Sprite.IsIntersecting(BlockSprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                Layer == game.mngrLvl.actor.Layer)
            {
                game.mngrLvl.tooltip += "Crate remains | ";
            }
        }
    }
}