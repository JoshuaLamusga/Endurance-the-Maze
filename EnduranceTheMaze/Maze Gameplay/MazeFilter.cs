using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EnduranceTheMaze
{
    /// <summary>
    /// After custInt1 passes over the block, filters are replaced by a block.
    /// 
    /// Activation types: Creates an object of the desired type.
    /// >= 5: The object is the nth - 5 entry of the Type enum.
    /// 
    /// Custom properties of custInt1:
    /// > 0: Number of passes to be made.
    /// -1: Cannot be activated by passing over it.
    /// 
    /// Custom properties of custInt2:
    /// 0: not solid
    /// 1: solid
    /// 
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeFilter : GameObj
    {
        //Relevant assets.
        public static Texture2D TexFilter { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeFilter(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            BlockType = Type.Filter;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexFilter);
            BlockSprite.depth = 0.405f;
            BlockSprite.doDrawOffset = true;
            BlockSprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 19, 2, 10);
            spriteAtlas.CenterOrigin();
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexFilter = Content.Load<Texture2D>("Content/Sprites/Game/sprFilter");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeFilter newBlock = new MazeFilter(game, X, Y, Layer);
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

            //Custom variables.
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);

            return newBlock;
        }

        /// <summary>
        /// Handled by MazeActor.cs
        /// </summary>
        public override void Update()
        {
            //Determines solidity by custInt1.
            if (CustInt2 == 1)
            {
                IsSolid = true;
            }
            else
            {
                IsSolid = false;
            }

            //Determines animation by enabledness.
            if (IsEnabled)
            {
                spriteAtlas.frameSpeed = 0.35f;
            }
            else
            {
                spriteAtlas.frame = 0;
                spriteAtlas.frameSpeed = 0;
            }

            //Handles activation behavior.
            if (IsEnabled && (IsActivated || CustInt1 == 0))
            {
                //Plays the activation sound.
                game.playlist.Play(sndActivated, X, Y);

                //Removes this block from the level.
                game.mngrLvl.RemoveItem(this);

                //Creates different blocks based on action type.
                if (ActionType > 4)
                {
                    game.mngrLvl.AddItem(Utils.BlockFromType
                        (game, (Type)(ActionType - 5), X, Y, Layer));
                }
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
                if (CustInt1 > 0)
                {
                    game.mngrLvl.tooltip += "Filter: " + CustInt1 +
                        " more passe(s) ";
                }
                else
                {
                    game.mngrLvl.tooltip += "Filter";
                }
                if (!IsEnabled)
                {
                    game.mngrLvl.tooltip += "(disabled)";
                }

                game.mngrLvl.tooltip += " | ";
            }
        }
    }
}