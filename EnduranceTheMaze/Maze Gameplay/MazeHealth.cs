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
    /// Adds to the colliding actor's health.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeHealth : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndCollectHealth;
        public static Texture2D texHealth { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeHealth(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Health;

            //Sets sprite information.
            sprite = new Sprite(true, texHealth);
            sprite.depth = 0.206f;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 19, 2, 10);
            spriteAtlas.frameSpeed = 0.2f;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndCollectHealth = Content.Load<SoundEffect>("Content/Sounds/sndCollectHealth");
            texHealth = Content.Load<Texture2D>("Content/Sprites/Game/sprHealth");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeHealth newBlock = new MazeHealth(game, x, y, layer);
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
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);
            return newBlock;
        }

        /// <summary>
        /// Adds to the actor's health and deletes itself on contact.
        /// </summary>
        public override void Update()
        {
            //Gets a list of all actors on the health object.
            List<GameObj> items = game.mngrLvl.items.Where(o =>
                o.x == x && o.y == y && o.layer == layer &&
                o.type == Type.Actor).ToList();

                //If there is at least one actor touching the health, the
                //first in the list gains 25 hp (no more than 100).
                if (items.Count != 0)
                {
                    (items[0] as MazeActor).hp += 25;
                    if ((items[0] as MazeActor).hp > 100)
                    {
                        (items[0] as MazeActor).hp = 100;
                    }

                    game.mngrLvl.RemoveItem(this);
                    game.playlist.Play(sndCollectHealth, x, y);
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
                game.mngrLvl.tooltip += "Health | ";
            }
        }
    }
}