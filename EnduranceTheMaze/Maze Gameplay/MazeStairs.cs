using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Transfers actors up/down a layer on contact.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1:
    /// 0: Stairs ascend.
    /// 1: Stairs descend.
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeStairs : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndStairsDown;
        public static SoundEffect sndStairsUp;
        public static Texture2D TexStairs { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeStairs(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            BlockType = Type.Stairs;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexStairs);
            BlockSprite.depth = 0.406f;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 2, 1, 2);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndStairsDown = Content.Load<SoundEffect>("Content/Sounds/sndStairsDown");
            sndStairsUp = Content.Load<SoundEffect>("Content/Sounds/sndStairsUp");
            TexStairs = Content.Load<Texture2D>("Content/Sprites/Game/sprStairs");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeStairs newBlock = new MazeStairs(game, X, Y, Layer);
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
            return newBlock;
        }

        /// <summary>
        /// Transfers actors up/down a layer on contact if possible.
        /// </summary>
        public override void Update()
        {
            //Adjusts the sprite frame.
            if (CustInt1 == 0)
            {
                spriteAtlas.frame = 0; //up.
            }
            else
            {
                spriteAtlas.frame = 1; //down.
            }

            //Gets a list of all actors on the stairs object.
            List<GameObj> items = game.mngrLvl.items.Where(o =>
                o.X == X && o.Y == Y && o.Layer == Layer &&
                (o.BlockType == Type.Actor || o.BlockType == Type.Enemy ||
                o.BlockType == Type.Crate)).ToList();

            //If there is at least one actor/enemy/crate touching the stairs.
            foreach (GameObj item in items)
            {
                //Gets a list of all solids in the destination.
                List<GameObj> itemsDest;
                if (CustInt1 == 0)
                {
                    itemsDest = game.mngrLvl.items.Where(o => o.IsSolid &&
                        o.X == X && o.Y == Y && o.Layer == Layer + 1).ToList();
                }
                else
                {
                    itemsDest = game.mngrLvl.items.Where(o => o.IsSolid &&
                        o.X == X && o.Y == Y && o.Layer == Layer - 1).ToList();
                }

                #region Interaction: MazeMultiWay.cs
                itemsDest = itemsDest.Where(o => !(o.BlockType == Type.MultiWay &&
                    o.IsEnabled && ((o.CustInt1 == 0 && o.BlockDir == item.BlockDir) ||
                    (o.CustInt1 != 0 && (o.BlockDir == item.BlockDir ||
                    o.BlockDir == Utils.DirOpp(item.BlockDir)))))).ToList();
                #endregion

                //Removes crates from the list if they can be pushed out of
                //the way by objects ascending/descending the stairs.
                List<GameObj> itemsFront;
                for (int i = itemsDest.Count - 1; i >= 0; i--)
                {
                    #region Interaction: MazeCrate.cs
                    if (itemsDest[i].BlockType == Type.Crate)
                    {
                        if (CustInt1 == 0)
                        {
                            itemsFront = game.mngrLvl.items.Where(o => o.IsSolid &&
                                o.X == X + (int)Utils.DirVector(item.BlockDir).X &&
                                o.Y == Y + (int)Utils.DirVector(item.BlockDir).Y &&
                                o.Layer == Layer + 1).ToList();
                        }
                        else
                        {
                            itemsFront = game.mngrLvl.items.Where(o => o.IsSolid &&
                                o.X == X + (int)Utils.DirVector(item.BlockDir).X &&
                                o.Y == Y + (int)Utils.DirVector(item.BlockDir).Y &&
                                o.Layer == Layer - 1).ToList();
                        }

                        #region Interaction: MazeMultiWay.cs
                        itemsFront = itemsFront.Where(o => !(o.IsEnabled &&
                            o.BlockType == Type.MultiWay && ((o.CustInt1 == 0 &&
                            o.BlockDir == item.BlockDir) || (o.CustInt1 != 0 &&
                            (o.BlockDir == item.BlockDir ||
                            o.BlockDir == Utils.DirOpp(BlockDir)))))).ToList();
                        #endregion

                        if (itemsFront.Count == 0)
                        {
                            //Moves the crate and removes it from itemsTop.
                            itemsDest[i].X += (int)Utils.DirVector(item.BlockDir).X;
                            itemsDest[i].Y += (int)Utils.DirVector(item.BlockDir).Y;
                            itemsDest[i].BlockDir = item.BlockDir;
                            itemsDest.RemoveAt(i);
                        }
                    }
                    #endregion
                }

                //Transports the block if nothing covers the destination.
                if (itemsDest.Count == 0)
                {
                    if (CustInt1 == 0)
                    {
                        item.Layer++;
                        game.playlist.Play(sndStairsUp, X, Y);
                    }
                    else
                    {
                        item.Layer--;
                        game.playlist.Play(sndStairsDown, X, Y);
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
                if (CustInt1 == 0)
                {
                    game.mngrLvl.tooltip += "Stairs (ascending) | ";
                }
                else
                {
                    game.mngrLvl.tooltip += "Stairs (descending) | ";
                }
            }
        }
    }
}