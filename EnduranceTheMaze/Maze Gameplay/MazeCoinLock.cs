using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        public static Texture2D TexCoinLock { get; private set; }

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeCoinLock(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            IsSolid = true;
            BlockType = Type.CoinLock;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexCoinLock);
            BlockSprite.depth = 0.410f;
            BlockSprite.drawBehavior = SpriteDraw.all;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexCoinLock = Content.Load<Texture2D>("Content/Sprites/Game/sprCoinLock");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeCoinLock newBlock = new MazeCoinLock(game, X, Y, Layer);
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
            if (Sprite.IsIntersecting(BlockSprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                Layer == game.mngrLvl.actor.Layer)
            {
                game.mngrLvl.tooltip += "Coin Lock | ";
            }
        }
    }
}