using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Shines while activated.
    /// 
    /// Activation types.
    /// 5: Shines when active.
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeELight : GameObj
    {
        //Relevant assets.
        public static Texture2D TexELight { get; private set; }

        //Sprite information.
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeELight(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            BlockType = Type.ELight;
            IsSolid = true;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexELight);
            BlockSprite.depth = 0.416f;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 2, 1, 2);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexELight = Content.Load<Texture2D>("Content/Sprites/Game/sprELight");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeELight newBlock = new MazeELight(game, X, Y, Layer);
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
            newBlock.BlockSprite = BlockSprite;

            //Sets custom variables.
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);
            return newBlock;
        }

        /// <summary>
        /// Controls sprite frames used and toggles isSolid.
        /// </summary>
        public override void Update()
        {
            #region Adjusts sprite.
            if (!IsEnabled || !IsActivated)
            {
                spriteAtlas.frame = 1;
            }
            else if (ActionType == 5)
            {
                spriteAtlas.frame = 0;
            }
            #endregion

            spriteAtlas.Update(true);
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
                game.mngrLvl.tooltip += "E-light | ";
            }
        }
    }
}