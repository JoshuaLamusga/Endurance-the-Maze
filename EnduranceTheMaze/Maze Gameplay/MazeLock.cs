using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EnduranceTheMaze
{
    /// <summary>
    /// A colored lock that unlocks with colored keys.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: Key colors:
    /// 0: Blue
    /// 1: Red
    /// 2: Goldenrod
    /// 3: Purple
    /// 4: Orange
    /// 5: Black
    /// 6: Dark blue
    /// 7: Dark red
    /// 8: Dark goldenrod
    /// 9: Dark orange
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeLock : GameObj
    {
        //Relevant assets.
        public static Texture2D TexLock { get; private set; }

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeLock(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            IsSolid = true;
            BlockType = Type.Lock;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexLock);
            BlockSprite.depth = 0.407f;
            BlockSprite.drawBehavior = SpriteDraw.all;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexLock = Content.Load<Texture2D>("Content/Sprites/Game/sprLock");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeLock newBlock = new MazeLock(game, X, Y, Layer);
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
        /// Deleted by actors with keys on contact. Handled by MazeActor.cs.
        /// </summary>
        public override void Update()
        {
            //Determines the lock color.
            switch (CustInt1)
            {
                case (0):
                    BlockSprite.color = Color.Blue;
                    break;
                case (1):
                    BlockSprite.color = Color.Red;
                    break;
                case (2):
                    BlockSprite.color = Color.Goldenrod;
                    break;
                case (3):
                    BlockSprite.color = Color.Purple;
                    break;
                case (4):
                    BlockSprite.color = Color.Orange;
                    break;
                case (5):
                    BlockSprite.color = Color.Black;
                    break;
                case (6):
                    BlockSprite.color = Color.DarkBlue;
                    break;
                case (7):
                    BlockSprite.color = Color.DarkRed;
                    break;
                case (8):
                    BlockSprite.color = Color.DarkGoldenrod;
                    break;
                case (9):
                    BlockSprite.color = Color.DarkOrange;
                    break;
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
                game.mngrLvl.tooltip += "Lock | ";
            }
        }
    }
}