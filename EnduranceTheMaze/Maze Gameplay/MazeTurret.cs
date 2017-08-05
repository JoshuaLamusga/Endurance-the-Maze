using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EnduranceTheMaze
{
    /// <summary>
    /// A turret which launches a bullet at some intervals.
    /// 
    /// Dependencies: MngrLvl, MazeBlock, MazeBelt.
    /// 
    /// Activation types:
    /// 5: Fires a single bullet.
    /// 
    /// Custom properties of custInt1: milliseconds between each bullet.
    /// 
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeTurret : GameObj
    {
        //Relevant assets.
        public static Texture2D TexTurret { get; private set; }

        //Sprite information.
        private SpriteAtlas spriteAtlas;
        public int msDelay;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeTurret(MainLoop game, int x, int y, int layer) :
            base(game, x, y, layer)
        {
            //Sets default values.
            IsSolid = true;
            BlockType = Type.Turret;
            msDelay = CustInt1;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexTurret);
            BlockSprite.depth = 0.419f;
            BlockSprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 8, 2, 4);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexTurret = Content.Load<Texture2D>("Content/Sprites/Game/sprTurret");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeTurret newBlock =
                new MazeTurret(game, X, Y, Layer);
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
            newBlock.msDelay = msDelay;

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
            if (BlockDir == Dir.Right)
            {
                spriteAtlas.frame = 0;
            }
            else if (BlockDir == Dir.Down)
            {
                spriteAtlas.frame = 1;
            }
            else if (BlockDir == Dir.Left)
            {
                spriteAtlas.frame = 2;
            }
            else if (BlockDir == Dir.Up)
            {
                spriteAtlas.frame = 3;
            }
            if (!IsEnabled)
            {
                spriteAtlas.frame += 4;
            }
            #endregion

            //Manages turret bullet spawning.
            if (IsEnabled)
            {
                msDelay -= 1;
                if (msDelay <= 0)
                {
                    msDelay = CustInt1;

                    MazeTurretBullet bullet = new MazeTurretBullet(game,
                            X * 32 + 16, Y * 32 + 16, Layer);

                    //Updates the bullet position after adjusting it.                    
                    bullet.X += (int)((Utils.DirVector(BlockDir)).X * 16 - 4);
                    bullet.Y += (int)((Utils.DirVector(BlockDir)).Y * 16 - 4);

                    bullet.BlockSprite.rectDest.X = bullet.X;
                    bullet.BlockSprite.rectDest.Y = bullet.Y;

                    bullet.BlockDir = BlockDir;
                    bullet.CustInt2 = CustInt2; //Provides the bullet speed.
                    game.mngrLvl.AddItem(bullet);
                }

                //Fires a bullet when activated.
                if (IsActivated && ActionType == 5)
                {
                    IsActivated = false;

                    MazeTurretBullet bullet = new MazeTurretBullet(game,
                            X * 32 + 16, Y * 32 + 16, Layer);

                    //Updates the bullet position after adjusting it.                    
                    bullet.X += (int)((Utils.DirVector(BlockDir)).X * 16 - 4);
                    bullet.Y += (int)((Utils.DirVector(BlockDir)).Y * 16 - 4);

                    bullet.BlockSprite.rectDest.X = bullet.X;
                    bullet.BlockSprite.rectDest.Y = bullet.Y;

                    bullet.BlockDir = BlockDir;
                    game.mngrLvl.AddItem(bullet);
                }
            }

            spriteAtlas.Update(true);
            base.Update();
        }

        /// <summary>
        /// Draws the turret. When hovered, draws enabledness/info.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            //Sets the tooltip to display disabled status and info.
            if (Sprite.IsIntersecting(BlockSprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                Layer == game.mngrLvl.actor.Layer)
            {
                game.mngrLvl.tooltip += "Turret";

                if (!IsEnabled)
                {
                    game.mngrLvl.tooltip += "(disabled)";
                }

                game.mngrLvl.tooltip += " | ";
            }
        }
    }
}