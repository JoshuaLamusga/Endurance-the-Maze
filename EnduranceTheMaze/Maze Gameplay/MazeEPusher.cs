using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace EnduranceTheMaze
{
    /// <summary>
    /// When activated, it pushes all solids in the nearby cell according to
    /// direction (if possible).
    /// 
    /// Activation types:
    /// 5: Activates to push solids if possible.
    /// 
    /// Custom properties of custInt1: none.
    /// Custom properties of custStr: none.
    /// Custom properties of custInt2: none.
    /// 
    /// </summary>
    public class MazeEPusher : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndActivatePush;
        public static Texture2D texEPusher { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        //When pressed, pusher waits this many frames to draw pushing frame.
        int pressTimer, pressTimerMax;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeEPusher(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.EPusher;
            isSolid = true;

            //Sets sprite information.
            sprite = new Sprite(true, texEPusher);
            sprite.depth = 0.415f;
            sprite.drawBehavior = SpriteDraw.all;            
            spriteAtlas = new SpriteAtlas(sprite, 64, 32, 3, 1, 3);
            sprite.originOffset = true;
            sprite.origin.X = 16;
            sprite.origin.Y = 16;

            //Sets timer information.
            pressTimer = pressTimerMax = 5;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndActivatePush = Content.Load<SoundEffect>("Content/Sounds/sndActivatePush");
            texEPusher = Content.Load<Texture2D>("Content/Sprites/Game/sprEPusher");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeEPusher newBlock = new MazeEPusher(game, x, y, layer);
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

            //Sets specific variables.
            newBlock.sprite = sprite;
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, true);
            newBlock.pressTimer = pressTimer;
            newBlock.pressTimerMax = pressTimerMax;
            return newBlock;
        }

        /// <summary>
        /// Pushes solids out of the way.
        /// </summary>
        public override void Update()
        {
            //Causes a delay in sprite drawing.
            if (spriteAtlas.frame == 1 && pressTimer > 0)
            {
                pressTimer--;
                if (pressTimer == 0)
                {
                    spriteAtlas.frame = 0;
                    pressTimer = pressTimerMax;
                    if (!isEnabled)
                    {
                        spriteAtlas.frame = 2;
                    }
                }
            }

            #region Adjusts sprite.
            if (dir == Dir.Right)
            {
                sprite.angle = 0;
            }
            else if (dir == Dir.Down)
            {
                sprite.angle = (float)(Math.PI / 2);
            }
            else if (dir == Dir.Left)
            {
                sprite.angle = (float)(Math.PI);
            }
            else
            {
                sprite.angle = (float)(-Math.PI / 2);
            }

            if (isActivated && actionType > 4)
            {
                spriteAtlas.frame = 1;
            }
            if (isEnabled)
            {
                if (!isActivated && pressTimer == 0)
                {
                    spriteAtlas.frame = 0;
                }
            }
            else
            {
                spriteAtlas.frame = 2;
            }
            #endregion

            if (isActivated)
            {
                if (actionType == 5)
                {
                    isActivated = false;

                    if (isEnabled)
                    {
                        //Gets a list of all solid blocks to be pushed and all
                        //solid blocks that may prevent movement.
                        List<GameObj> items = game.mngrLvl.items.Where(o =>
                            o.x == x + (int)Utils.DirVector(dir).X &&
                            o.y == y + (int)Utils.DirVector(dir).Y &&
                            o.layer == layer && o.isSolid).ToList();
                        List<GameObj> items2 = game.mngrLvl.items.Where(o =>
                            o.x == x + (int)Utils.DirVector(dir).X * 2 &&
                            o.y == y + (int)Utils.DirVector(dir).Y * 2 &&
                            o.layer == layer && o.isSolid).ToList();

                        //Solid blocks in the destination prevent pushing.
                        if (items2.Count != 0)
                        {
                            spriteAtlas.frame = 0;
                        }
                        else
                        {
                            game.playlist.Play(sndActivatePush, x, y);

                            foreach (GameObj item in items)
                            {
                                item.x += (int)Utils.DirVector(dir).X;
                                item.y += (int)Utils.DirVector(dir).Y;
                            }
                        }
                    }
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
            if (Sprite.isIntersecting(sprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                layer == game.mngrLvl.actor.layer)
            {
                game.mngrLvl.tooltip += "E-pusher | ";
            }
        }
    }
}