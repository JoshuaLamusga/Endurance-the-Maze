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
    /// When activated, it spawns a block based on activation type.
    /// 
    /// Dependencies: MngrLvl, MazeBlock.
    /// 
    /// Activation types: Creates one of the following in spawner's dir:
    /// > 5: The object is the nth - 5 entry of the Type enum.
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeSpawner : GameObj
    {
        //Relevant assets.
        public static Texture2D texSpawner { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;     

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeSpawner(MainLoop game, int x, int y, int layer) :
            base(game, x, y, layer)
        {
            //Sets default values.
            isSolid = true;
            type = Type.Spawner;

            //Sets sprite information.
            sprite = new Sprite(true, texSpawner);
            sprite.depth = 0.402f;
            sprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 8, 2, 4);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texSpawner = Content.Load<Texture2D>("Content/Sprites/Game/sprSpawner");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeSpawner newBlock =
                new MazeSpawner(game, x, y, layer);
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

            //Sets specific variables.
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);
            return newBlock;
        }

        /// <summary>
        /// Updates the atlas. Behavior handled by MngrLvl.cs.
        /// </summary>
        public override void Update()
        {
            //Performs activation behaviors.
            if (isEnabled && isActivated)
            {
                if (actionType > 4)
                {
                    //Deactivates the item and plays a sound.
                    isActivated = false;

                    //Gets a list of solid objects in the way.
                    List<GameObj> items = game.mngrLvl.items.Where(o =>
                        o.x == x + (int)Utils.DirVector(dir).X &&
                        o.y == y + (int)Utils.DirVector(dir).Y &&
                        o.layer == layer && o.isSolid).ToList();

                    #region Interaction: MazeMultiWay.cs
                items = items.Where(o =>
                    !(o.isEnabled && o.type == Type.MultiWay &&
                    ((o.custInt1 == 0 && o.dir == dir) ||
                    (o.custInt1 != 0 && (o.dir == dir ||
                    o.dir == Utils.DirOpp(dir)))))).ToList();
                #endregion

                    //Creates an item if there are no solid objects.
                    if (items.Count == 0)
                    {
                        //Plays a sound when an object is spawned.
                        game.playlist.Play(sndActivated, x, y);

                        //Creates different blocks based on action type.
                        game.mngrLvl.AddItem(Utils.BlockFromType(game,
                            (Type)(actionType - 5),
                            x + (int)Utils.DirVector(dir).X,
                            y + (int)Utils.DirVector(dir).Y, layer));
                    
                    }
                }
            }

            #region Updates the sprite.
            //Updates the actor sprite by direction.
            //Depends on the texture frames and orientation.
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
            else
            {
                spriteAtlas.frame = 3;
            }
            if (!isEnabled)
            {
                spriteAtlas.frame += 4;
            }
            #endregion

            spriteAtlas.Update(true);
            base.Update();
        }

        /// <summary>
        /// Draws the spawner. When hovered, draws enabledness/info.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            //Sets the tooltip to display disabled status and info.
            if (Sprite.isIntersecting(sprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                layer == game.mngrLvl.actor.layer)
            {
                game.mngrLvl.tooltip += "Spawner";

                if (!isEnabled)
                {
                    game.mngrLvl.tooltip += "(disabled)";
                }

                game.mngrLvl.tooltip += " | ";
            }
        }
    }
}