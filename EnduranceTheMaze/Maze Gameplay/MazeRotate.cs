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
    /// When activated, rotates the blocks in a matrix.
    /// 
    /// Activation types:
    /// 5: Rotates clockwise 90 degrees.
    /// 6: Rotates counterclockwise 90 degrees.
    /// 7: Rotates 180 degrees.
    /// 
    /// Custom properties of custInt1:
    /// > 0: Number of columns (x).
    /// Custom properties of custInt2:
    /// > 0: Number of rows (y).
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeRotate : GameObj
    {
        //Relevant assets.
        public static Texture2D texRotate { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        //Stores original positional values for proper rotation.
        private int xStart, yStart;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeRotate(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Rotate;

            //Sets sprite information.
            sprite = new Sprite(true, texRotate);
            sprite.depth = 0.418f;
            sprite.originOffset = true;
            sprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 2, 1, 2);
            spriteAtlas.CenterOrigin();

            //Sets positional values.
            xStart = x;
            yStart = y;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texRotate = Content.Load<Texture2D>("Content/Sprites/Game/sprRotate");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeRotate newBlock = new MazeRotate(game, x, y, layer);
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

            //Custom variables.
            newBlock.sprite = sprite;
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);
            newBlock.xStart = xStart;
            newBlock.yStart = yStart;
            return newBlock;
        }

        /// <summary>
        /// Handled by MazeActor.cs
        /// </summary>
        public override void Update()
        {
            #region Adjusts sprite.
            if (isEnabled)
            {
                spriteAtlas.frame = 0;
            }
            else
            {
                spriteAtlas.frame = 1;
            }
            #endregion

            if (isActivated && actionType > 4)
            {
                //Deactivates the object and plays a sound.
                isActivated = false;
                game.playlist.Play(sndActivated, x, y);

                //Saves the new positions of each block so the
                //transposition doesn't affect them twice.
                List<GameObj> queueItems = new List<GameObj>();
                List<int> queueItemsX = new List<int>();
                List<int> queueItemsY = new List<int>();

                //Iterates through each affected space.
                for (int xx = 0; xx < custInt1; xx++)
                {
                    for (int yy = 0; yy < custInt1; yy++)
                    {
                        //Gets a list of all blocks in the space.
                        List<GameObj> blocks = game.mngrLvl.items
                            .Where(o => o.layer == layer &&
                            o.x == xStart + xx && o.y == yStart + yy)
                            .ToList();

                        foreach (GameObj block in blocks)
                        {
                            queueItems.Add(block);
                            if (actionType == 5 || actionType == 7)
                            {
                                queueItemsX.Add(xStart + (custInt1 - yy - 1));
                            }
                            else
                            {
                                queueItemsX.Add(xStart + yy);
                            }
                            if (actionType == 6 || actionType == 7)
                            {
                                queueItemsY.Add(yStart + (custInt1 - xx - 1));
                            }
                            else
                            {
                                queueItemsY.Add(yStart + xx);
                            }
                        }
                    }
                }

                //Moves each block synchronously.
                for (int i = 0; i < queueItems.Count; i++)
                {
                    queueItems[i].x = queueItemsX[i];
                    queueItems[i].y = queueItemsY[i];
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
                game.mngrLvl.tooltip += "Rotate";
                
                if (!isEnabled)
                {
                    game.mngrLvl.tooltip += " (disabled)";
                }

                game.mngrLvl.tooltip += " | ";
            }
        }
    }
}