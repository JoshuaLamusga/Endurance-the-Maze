using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

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
        public static Texture2D texTurret { get; private set; }

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
            isSolid = true;
            type = Type.Turret;
            msDelay = custInt1;

            //Sets sprite information.
            sprite = new Sprite(true, texTurret);
            sprite.depth = 0.419f;
            sprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 8, 2, 4);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texTurret = Content.Load<Texture2D>("Content/Sprites/Game/sprTurret");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeTurret newBlock =
                new MazeTurret(game, x, y, layer);
            newBlock.actionIndex = actionIndex;
            newBlock.actionIndex2 = actionIndex2;
            newBlock.actionType = actionType;
            newBlock.custInt1 = custInt1;
            newBlock.custInt2 = custInt2;
            newBlock.custStr = custStr;
            newBlock.dir = dir;
            newBlock.isActivated = isActivated;
            newBlock.isEnabled = isEnabled;
            newBlock.isVisible = isVisible;
            newBlock.sprite = sprite;
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
            if (dir == Dir.Right)
            {
                spriteAtlas.frame = 0;
            }
            else if (dir == Dir.Down)
            {
                spriteAtlas.frame = 1;
            }
            else if (dir == Dir.Left)
            {
                spriteAtlas.frame = 2;
            }
            else if (dir == Dir.Up)
            {
                spriteAtlas.frame = 3;
            }
            if (!isEnabled)
            {
                spriteAtlas.frame += 4;
            }
            #endregion

            //Manages turret bullet spawning.
            if (isEnabled)
            {
                msDelay -= 1;
                if (msDelay <= 0)
                {
                    msDelay = custInt1;

                    MazeTurretBullet bullet = new MazeTurretBullet(game,
                            x * 32 + 16, y * 32 + 16, layer);

                    //Updates the bullet position after adjusting it.                    
                    bullet.x += (int)((Utils.DirVector(dir)).X * 16 - 4);
                    bullet.y += (int)((Utils.DirVector(dir)).Y * 16 - 4);

                    bullet.sprite.rectDest.X = bullet.x;
                    bullet.sprite.rectDest.Y = bullet.y;

                    bullet.dir = dir;
                    bullet.custInt2 = custInt2; //Provides the bullet speed.
                    game.mngrLvl.AddItem(bullet);
                }

                //Fires a bullet when activated.
                if (isActivated && actionType == 5)
                {
                    isActivated = false;

                    MazeTurretBullet bullet = new MazeTurretBullet(game,
                            x * 32 + 16, y * 32 + 16, layer);

                    //Updates the bullet position after adjusting it.                    
                    bullet.x += (int)((Utils.DirVector(dir)).X * 16 - 4);
                    bullet.y += (int)((Utils.DirVector(dir)).Y * 16 - 4);

                    bullet.sprite.rectDest.X = bullet.x;
                    bullet.sprite.rectDest.Y = bullet.y;

                    bullet.dir = dir;
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
            if (Sprite.isIntersecting(sprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                layer == game.mngrLvl.actor.layer)
            {
                game.mngrLvl.tooltip += "Turret";

                if (!isEnabled)
                {
                    game.mngrLvl.tooltip += "(disabled)";
                }

                game.mngrLvl.tooltip += " | ";
            }
        }
    }
}