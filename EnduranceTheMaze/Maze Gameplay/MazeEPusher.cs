using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public static Texture2D TexEPusher { get; private set; }

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
            BlockType = Type.EPusher;
            IsSolid = true;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexEPusher);
            BlockSprite.depth = 0.415f;
            BlockSprite.drawBehavior = SpriteDraw.all;            
            spriteAtlas = new SpriteAtlas(BlockSprite, 64, 32, 3, 1, 3);
            BlockSprite.originOffset = true;
            BlockSprite.origin.X = 16;
            BlockSprite.origin.Y = 16;

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
            TexEPusher = Content.Load<Texture2D>("Content/Sprites/Game/sprEPusher");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeEPusher newBlock = new MazeEPusher(game, X, Y, Layer);
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
                    if (!IsEnabled)
                    {
                        spriteAtlas.frame = 2;
                    }
                }
            }

            #region Adjusts sprite.
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

            if (IsActivated && ActionType > 4)
            {
                spriteAtlas.frame = 1;
            }
            if (IsEnabled)
            {
                if (!IsActivated && pressTimer == 0)
                {
                    spriteAtlas.frame = 0;
                }
            }
            else
            {
                spriteAtlas.frame = 2;
            }
            #endregion

            if (IsActivated)
            {
                if (ActionType == 5)
                {
                    IsActivated = false;

                    if (IsEnabled)
                    {
                        //Gets a list of all solid blocks to be pushed and all
                        //solid blocks that may prevent movement.
                        List<GameObj> items = game.mngrLvl.items.Where(o =>
                            o.X == X + (int)Utils.DirVector(BlockDir).X &&
                            o.Y == Y + (int)Utils.DirVector(BlockDir).Y &&
                            o.Layer == Layer && o.IsSolid).ToList();
                        List<GameObj> items2 = game.mngrLvl.items.Where(o =>
                            o.X == X + (int)Utils.DirVector(BlockDir).X * 2 &&
                            o.Y == Y + (int)Utils.DirVector(BlockDir).Y * 2 &&
                            o.Layer == Layer && o.IsSolid).ToList();

                        //Solid blocks in the destination prevent pushing.
                        if (items2.Count != 0)
                        {
                            spriteAtlas.frame = 0;
                        }
                        else
                        {
                            game.playlist.Play(sndActivatePush, X, Y);

                            foreach (GameObj item in items)
                            {
                                item.X += (int)Utils.DirVector(BlockDir).X;
                                item.Y += (int)Utils.DirVector(BlockDir).Y;
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
            if (Sprite.IsIntersecting(BlockSprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                Layer == game.mngrLvl.actor.Layer)
            {
                game.mngrLvl.tooltip += "E-pusher | ";
            }
        }
    }
}