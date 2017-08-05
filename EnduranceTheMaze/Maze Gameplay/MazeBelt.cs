using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Moves blocks on top according to its direction unless blocked.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeBelt : GameObj
    {
        //Relevant assets.
        public static Texture2D TexBelt { get; private set; }

        //Sprite information.
        public SpriteAtlas spriteAtlas; //Set by MngrLvl.cs.

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeBelt(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            BlockType = Type.Belt;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexBelt);
            BlockSprite.depth = 0.401f;
            BlockSprite.originOffset = true;
            BlockSprite.drawBehavior = SpriteDraw.all;

            //Custom variables.
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 9, 1, 9);
            spriteAtlas.CenterOrigin();
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexBelt = Content.Load<Texture2D>("Content/Sprites/Game/sprBelt");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeBelt newBlock = new MazeBelt(game, X, Y, Layer);
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
            newBlock.spriteAtlas = spriteAtlas;
            return newBlock;
        }

        /// <summary>
        /// Determines belt orientation. Belt movements handled by MngrLvl.
        /// </summary>
        public override void Update()
        {
            //Updates the belt sprite by direction.
            //Depends on the texture frames.
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

            spriteAtlas.Update(true);
            base.Update();

            //Determines the belt's image speed.
            if (IsEnabled)
            {
                spriteAtlas.frameSpeed = 0.25f;
            }
            else
            {
                spriteAtlas.frameSpeed = 0;
            }
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
                game.mngrLvl.tooltip += "Belt | ";
            }
        }
    }
}