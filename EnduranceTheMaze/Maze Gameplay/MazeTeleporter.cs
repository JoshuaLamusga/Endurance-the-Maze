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
    /// Sends/receives objects on contact.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1:
    /// 0: Sender node.
    /// 1: Receiver node.
    /// Custom properties of custInt2:
    /// The number is the teleporting channel.
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeTeleporter : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndTeleport;
        public static Texture2D texTeleporter { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeTeleporter(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Teleporter;

            //Sets sprite information.
            sprite = new Sprite(true, texTeleporter);
            sprite.depth = 0.412f;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 4, 1, 4);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndTeleport = Content.Load<SoundEffect>("Content/Sounds/sndTeleport");
            texTeleporter = Content.Load<Texture2D>("Content/Sprites/Game/sprTeleport");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeTeleporter newBlock = new MazeTeleporter(game, x, y, layer);
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
        /// Transfers between senders and receivers on contact if possible.
        /// </summary>
        public override void Update()
        {
            #region Adjusts sprite.
            //Adjusts the sprite frame.
            if (custInt1 == 0)
            {
                spriteAtlas.frame = 0; //Sender.
            }
            else
            {
                spriteAtlas.frame = 2; //Receiver.
            }
            //Depends on frame positions and texture.
            if (!isEnabled)
            {
                spriteAtlas.frame++;
            }
            #endregion

            //Sender logic.
            if (isEnabled && custInt1 == 0)
            {
                //Blocks on this block, blocks on receivers, and receivers.
                List<GameObj> itemsTop = new List<GameObj>();
                List<GameObj> itemsDestTop = new List<GameObj>();
                List<GameObj> itemsNodes = new List<GameObj>();

                //Gets a list of all blocks on the sender.
                itemsTop = game.mngrLvl.items.Where(o =>
                    o.x == x && o.y == y && o.layer == layer &&
                    (o.sprite.depth < sprite.depth)).ToList();

                #region Interaction: MazeTurretBullet
                itemsTop.AddRange(game.mngrLvl.items.Where(o =>
                    o.type == Type.TurretBullet &&
                    Math.Abs(x * 32 + 16 - o.x) < 16 &&
                    Math.Abs(y * 32 + 16 - o.y) < 16 &&
                    o.layer == layer));
                #endregion

                //Gets a list of all enabled receivers.
                itemsNodes = game.mngrLvl.items.Where(o =>
                    o.type == Type.Teleporter &&
                    o.isEnabled &&
                    o.custInt1 != 0 &&
                    o.custInt2 == custInt2).ToList();

                //Teleports blocks if receivers are available.
                foreach (GameObj item in itemsTop)
                {
                    //Filters out all incapable receivers.
                    for (int i = itemsNodes.Count - 1; i >= 0; i--)
                    {
                        //Gets a list of all solid blocks on the receiver.
                        itemsDestTop = game.mngrLvl.items.Where(o =>
                            o.x == itemsNodes[i].x &&
                            o.y == itemsNodes[i].y && o.layer ==
                            itemsNodes[i].layer && o.isSolid).ToList();

                        //Iterates through each block on the receiver.
                        for (int j = itemsDestTop.Count - 1; j >= 0; j--)
                        {
                            #region Interaction: MazeCrate
                            if (itemsDestTop[j].type == Type.Crate)
                            {
                                //Gets a list of all solid blocks in front.
                                List<GameObj> itemsDestFront =
                                    game.mngrLvl.items.Where(o => o.isSolid &&
                                    o.x == itemsNodes[i].x +
                                    (int)Utils.DirVector(item.dir).X &&
                                    o.y == itemsNodes[i].y +
                                    (int)Utils.DirVector(item.dir).Y &&
                                    o.layer == itemsNodes[i].layer).ToList();

                                //Removes valid multiways from the list.
                                #region Interaction: MazeMultiWay.cs
                                itemsDestFront = itemsDestFront.Where(o =>
                                    !(o.isEnabled &&
                                    o.type == Type.MultiWay &&
                                    ((o.custInt1 == 0 && o.dir == item.dir) ||
                                    (o.custInt1 != 0 && (o.dir == item.dir ||
                                    o.dir == Utils.DirOpp(dir)))))).ToList();
                                #endregion

                                /*Allows a block to enter the teleporter if
                                  the crate blocking the receiver can be
                                  pushed out of the way in the direction the
                                  block is traveling.*/
                                if (itemsDestFront.Count == 0)
                                {
                                    //Moves the crate; removes it from list.
                                    itemsDestTop[j].x +=
                                        (int)Utils.DirVector(item.dir).X;
                                    itemsDestTop[j].y +=
                                        (int)Utils.DirVector(item.dir).Y;
                                    itemsDestTop[j].dir = item.dir;
                                    itemsDestTop.RemoveAt(j);
                                }
                            }
                            #endregion
                        }

                        //Removes the receiver for being incapable.
                        if (itemsDestTop.Count != 0)
                        {
                            itemsNodes.RemoveAt(i);
                        }
                    }

                    if (itemsNodes.Count != 0)
                    {
                        //Selects a receiver at random.
                        GameObj receiver =
                            itemsNodes[Utils.rng.Next(itemsNodes.Count)];

                        game.playlist.Play(sndTeleport, x, y);

                        #region Interaction: MazeTurretBullet
                        if (item.type == Type.TurretBullet)
                        {
                            item.x = (int)Math.IEEERemainder(item.x, 32);
                            item.y = (int)Math.IEEERemainder(item.y, 32);
                            item.x += receiver.x * 32;
                            item.y += receiver.y * 32;
                        }
                        else
                        {
                            item.x = receiver.x;
                            item.y = receiver.y;
                        }
                        #endregion

                        item.layer = receiver.layer;
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
                    game.mngrLvl.tooltip += "Sender " +
                        "(channel " + custInt2 + ")";
                }
                else
                {
                    game.mngrLvl.tooltip += "Receiver " +
                        "(channel " + custInt2 + ")";
                }

                if (!isEnabled)
                {
                    game.mngrLvl.tooltip += "(disabled)";
                }

                game.mngrLvl.tooltip += " | ";
            }            
        }
    }
}