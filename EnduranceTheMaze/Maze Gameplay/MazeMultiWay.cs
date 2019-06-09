using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Acts like a wall, except that objects can pass through in one or two
    /// directions. Interaction logic stored in MazeActor and MngrLvl.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1:
    /// 0: one-way.
    /// 1: two-way.
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeMultiWay : GameObj
    {
        //Relevant assets.
        public static Texture2D TexMultiWay { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeMultiWay(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            IsSolid = true;
            BlockType = Type.MultiWay;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexMultiWay);
            BlockSprite.depth = 0.408f;
            BlockSprite.doDrawOffset = true;
            BlockSprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 4, 1, 4);
            spriteAtlas.CenterOrigin();
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexMultiWay = Content.Load<Texture2D>("Content/Sprites/Game/sprMultiWay");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeMultiWay newBlock = new MazeMultiWay(game, X, Y, Layer);
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

            //Sets custom variables.
            newBlock.BlockSprite = BlockSprite;
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);

            return newBlock;
        }

        /// <summary>
        /// Determines orientation. Multiway interaction handled by
        /// MngrLvl and MazeActor.
        /// </summary>
        public override void Update()
        {
            //Updates the sprite by direction.
            if (BlockDir == Dir.Right)
            {
                BlockSprite.angle = 0;
            }
            else if (BlockDir == Dir.Down)
            {
                BlockSprite.angle = (float)(Math.PI / 2);
            }
            else if (BlockDir == Dir.Left)
            {
                BlockSprite.angle = (float)(Math.PI);
            }
            else
            {
                BlockSprite.angle = (float)(-Math.PI / 2);
            }

            //Determines the frame used.
            //Dependent on frame order.
            if (CustInt1 == 0)
            {
                spriteAtlas.frame = 0;
            }
            else
            {
                spriteAtlas.frame = 2;
            }
            if (!IsEnabled)
            {
                spriteAtlas.frame += 1;
            }

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
                if (CustInt1 == 0)
                {
                    game.mngrLvl.tooltip += "One-way | ";
                }
                else
                {
                    game.mngrLvl.tooltip += "Two-way | ";
                }
            }            
        }
    }
}