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
        public static Texture2D texStairs { get; private set; }

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
            type = Type.Stairs;

            //Sets sprite information.
            sprite = new Sprite(true, texStairs);
            sprite.depth = 0.406f;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 2, 1, 2);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndStairsDown = Content.Load<SoundEffect>("Content/Sounds/sndStairsDown");
            sndStairsUp = Content.Load<SoundEffect>("Content/Sounds/sndStairsUp");
            texStairs = Content.Load<Texture2D>("Content/Sprites/Game/sprStairs");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeStairs newBlock = new MazeStairs(game, x, y, layer);
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
            return newBlock;
        }

        /// <summary>
        /// Transfers actors up/down a layer on contact if possible.
        /// </summary>
        public override void Update()
        {
            //Adjusts the sprite frame.
            if (custInt1 == 0)
            {
                spriteAtlas.frame = 0; //up.
            }
            else
            {
                spriteAtlas.frame = 1; //down.
            }

            //Gets a list of all actors on the stairs object.
            List<GameObj> items = game.mngrLvl.items.Where(o =>
                o.x == x && o.y == y && o.layer == layer &&
                (o.type == Type.Actor || o.type == Type.Enemy ||
                o.type == Type.Crate)).ToList();

            //If there is at least one actor/enemy/crate touching the stairs.
            foreach (GameObj item in items)
            {
                //Gets a list of all solids in the destination.
                List<GameObj> itemsDest;
                if (custInt1 == 0)
                {
                    itemsDest = game.mngrLvl.items.Where(o => o.isSolid &&
                        o.x == x && o.y == y && o.layer == layer + 1).ToList();
                }
                else
                {
                    itemsDest = game.mngrLvl.items.Where(o => o.isSolid &&
                        o.x == x && o.y == y && o.layer == layer - 1).ToList();
                }

                #region Interaction: MazeMultiWay.cs
                itemsDest = itemsDest.Where(o => !(o.type == Type.MultiWay &&
                    o.isEnabled && ((o.custInt1 == 0 && o.dir == item.dir) ||
                    (o.custInt1 != 0 && (o.dir == item.dir ||
                    o.dir == Utils.DirOpp(item.dir)))))).ToList();
                #endregion

                //Removes crates from the list if they can be pushed out of
                //the way by objects ascending/descending the stairs.
                List<GameObj> itemsFront;
                for (int i = itemsDest.Count - 1; i >= 0; i--)
                {
                    #region Interaction: MazeCrate.cs
                    if (itemsDest[i].type == Type.Crate)
                    {
                        if (custInt1 == 0)
                        {
                            itemsFront = game.mngrLvl.items.Where(o => o.isSolid &&
                                o.x == x + (int)Utils.DirVector(item.dir).X &&
                                o.y == y + (int)Utils.DirVector(item.dir).Y &&
                                o.layer == layer + 1).ToList();
                        }
                        else
                        {
                            itemsFront = game.mngrLvl.items.Where(o => o.isSolid &&
                                o.x == x + (int)Utils.DirVector(item.dir).X &&
                                o.y == y + (int)Utils.DirVector(item.dir).Y &&
                                o.layer == layer - 1).ToList();
                        }

                        #region Interaction: MazeMultiWay.cs
                        itemsFront = itemsFront.Where(o => !(o.isEnabled &&
                            o.type == Type.MultiWay && ((o.custInt1 == 0 &&
                            o.dir == item.dir) || (o.custInt1 != 0 &&
                            (o.dir == item.dir ||
                            o.dir == Utils.DirOpp(dir)))))).ToList();
                        #endregion

                        if (itemsFront.Count == 0)
                        {
                            //Moves the crate and removes it from itemsTop.
                            itemsDest[i].x += (int)Utils.DirVector(item.dir).X;
                            itemsDest[i].y += (int)Utils.DirVector(item.dir).Y;
                            itemsDest[i].dir = item.dir;
                            itemsDest.RemoveAt(i);
                        }
                    }
                    #endregion
                }

                //Transports the block if nothing covers the destination.
                if (itemsDest.Count == 0)
                {
                    if (custInt1 == 0)
                    {
                        item.layer++;
                        game.playlist.Play(sndStairsUp, x, y);
                    }
                    else
                    {
                        item.layer--;
                        game.playlist.Play(sndStairsDown, x, y);
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
                if (custInt1 == 0)
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