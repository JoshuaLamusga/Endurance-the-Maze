using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EnduranceTheMaze
{
    /// <summary>
    /// An enemy which bounces in its indicated directions.
    /// Interacts with environment. Damages actor on contact.
    /// 
    /// Dependencies: MngrLvl, MazeBlock, MazeBelt.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeEnemy : GameObj
    {
        //Relevant assets.
        public static Texture2D TexEnemy { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;     

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeEnemy(MainLoop game, int x, int y, int layer) :
            base(game, x, y, layer)
        {
            //Sets default values.
            IsSolid = true;
            BlockType = Type.Enemy;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexEnemy);
            BlockSprite.depth = 0.4f;
            BlockSprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 4, 1, 4);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexEnemy = Content.Load<Texture2D>("Content/Sprites/Game/sprEnemy");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeEnemy newBlock =
                new MazeEnemy(game, X, Y, Layer);
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

            //Sets specific variables.
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);
            return newBlock;
        }

        /// <summary>
        /// Updates the atlas. Behavior handled by MngrLvl.cs.
        /// </summary>
        public override void Update()
        {
            #region Determines sprite by dir and isEnabled.
            if (Utils.DirCardinal(BlockDir))
            {
                if (IsEnabled)
                {
                    spriteAtlas.frame = 0;
                }
                else
                {
                    spriteAtlas.frame = 1;
                }
            }
            else
            {
                if (IsEnabled)
                {
                    spriteAtlas.frame = 2;
                }
                else
                {
                    spriteAtlas.frame = 3;
                }
            }
            #endregion

            spriteAtlas.Update(true);
            base.Update();
        }

        /// <summary>
        /// Draws the enemy. When hovered, draws enabledness/info.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            //Sets the tooltip to display disabled status and info.
            if (Sprite.IsIntersecting(BlockSprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                Layer == game.mngrLvl.actor.Layer)
            {
                game.mngrLvl.tooltip += "Enemy";

                if (!IsEnabled)
                {
                    game.mngrLvl.tooltip += "(disabled)";
                }

                game.mngrLvl.tooltip += " | ";
            }
        }
    }
}