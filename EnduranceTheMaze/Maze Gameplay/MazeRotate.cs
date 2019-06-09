using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

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
        public static Texture2D TexRotate { get; private set; }

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
            BlockType = Type.Rotate;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexRotate);
            BlockSprite.depth = 0.418f;
            BlockSprite.doDrawOffset = true;
            BlockSprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 2, 1, 2);
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
            TexRotate = Content.Load<Texture2D>("Content/Sprites/Game/sprRotate");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeRotate newBlock = new MazeRotate(game, X, Y, Layer);
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
            newBlock.BlockSprite = BlockSprite;
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
            if (IsEnabled)
            {
                spriteAtlas.frame = 0;
            }
            else
            {
                spriteAtlas.frame = 1;
            }
            #endregion

            if (IsActivated && ActionType > 4)
            {
                //Deactivates the object and plays a sound.
                IsActivated = false;
                game.playlist.Play(sndActivated, X, Y);

                //Saves the new positions of each block so the
                //transposition doesn't affect them twice.
                List<GameObj> queueItems = new List<GameObj>();
                List<int> queueItemsX = new List<int>();
                List<int> queueItemsY = new List<int>();

                //Iterates through each affected space.
                for (int xx = 0; xx < CustInt1; xx++)
                {
                    for (int yy = 0; yy < CustInt1; yy++)
                    {
                        //Gets a list of all blocks in the space.
                        List<GameObj> blocks = game.mngrLvl.items
                            .Where(o => o.Layer == Layer &&
                            o.X == xStart + xx && o.Y == yStart + yy)
                            .ToList();

                        foreach (GameObj block in blocks)
                        {
                            queueItems.Add(block);
                            if (ActionType == 5 || ActionType == 7)
                            {
                                queueItemsX.Add(xStart + (CustInt1 - yy - 1));
                            }
                            else
                            {
                                queueItemsX.Add(xStart + yy);
                            }
                            if (ActionType == 6 || ActionType == 7)
                            {
                                queueItemsY.Add(yStart + (CustInt1 - xx - 1));
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
                    queueItems[i].X = queueItemsX[i];
                    queueItems[i].Y = queueItemsY[i];
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
                game.mngrLvl.tooltip += "Rotate";
                
                if (!IsEnabled)
                {
                    game.mngrLvl.tooltip += " (disabled)";
                }

                game.mngrLvl.tooltip += " | ";
            }
        }
    }
}