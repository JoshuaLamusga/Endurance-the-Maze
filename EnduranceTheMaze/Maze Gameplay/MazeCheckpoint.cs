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
    /// Saves the current progress of the player.
    /// 
    /// Custom properties of custInt1:
    /// 0: Saves every time an actor occupies the area.
    /// 1: Saves once and vanishes.
    /// 
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeCheckpoint : GameObj
    {
        //Relevant assets.
        public static Texture2D texCheckpoint { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        //Contains whether the checkpoint has been activated or not.
        bool hasActivated;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeCheckpoint(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Checkpoint;

            //Sets sprite information.
            sprite = new Sprite(true, texCheckpoint);
            sprite.depth = 0.208f;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 19, 2, 10);
            spriteAtlas.frameSpeed = 0.35f;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texCheckpoint = Content.Load<Texture2D>("Content/Sprites/Game/sprCheckpoint");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeCheckpoint newBlock = new MazeCheckpoint(game, x, y, layer);
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
            newBlock.hasActivated = hasActivated;
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);
            return newBlock;
        }

        /// <summary>
        /// Saves if touched by an actor, deleting itself if custInt1 == 1.
        /// </summary>
        public override void Update()
        {
            //Gets a list of all actors in the same position.
            List<GameObj> items = game.mngrLvl.items.Where(o =>
                o.x == x && o.y == y && o.layer == layer &&
                o.type == Type.Actor).ToList();

            if (items.Count > 0) //Attempts to save.
            {
                if (!hasActivated)
                {
                    game.mngrLvl.doCheckpoint = true;

                    if (custInt1 == 1)
                    {
                        game.mngrLvl.RemoveItem(this);
                    }
                }

                hasActivated = true;
            }
            else //Doesn't attempt to save.
            {
                hasActivated = false;
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
                game.mngrLvl.tooltip += "Checkpoint";
                
                if (custInt1 == 1)
                {
                    game.mngrLvl.tooltip += "(disappears on touch)";
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