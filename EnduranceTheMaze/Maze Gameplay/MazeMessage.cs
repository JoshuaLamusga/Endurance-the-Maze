using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        public static Texture2D TexMessage { get; private set; }

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeMessage(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            BlockType = Type.Message;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexMessage);
            BlockSprite.depth = 0.209f;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexMessage = Content.Load<Texture2D>("Content/Sprites/Game/sprMessage");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeMessage newBlock = new MazeMessage(game, X, Y, Layer);
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

        public override void Update()
        {
            base.Update();

            //Shows a message when clicked.
            if (game.MsState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                Sprite.IsIntersecting(BlockSprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                Layer == game.mngrLvl.actor.Layer)
            {
                game.mngrLvl.isMessageShown = true;
                game.mngrLvl.message = CustStr;
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