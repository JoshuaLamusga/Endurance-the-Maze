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
    /// A hole that can be filled by a crate. Acts like a wall.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeCrateHole : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndCrateHole;
        public static Texture2D texCrateHole { get; private set; }

        //Sprite information.
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeCrateHole(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            isSolid = true;
            type = Type.CrateHole;

            //Sets sprite information.
            sprite = new Sprite(true, texCrateHole);
            sprite.depth = 0.403f;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 2, 1, 2);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndCrateHole = Content.Load<SoundEffect>("Content/Sounds/sndCrateHole");
            texCrateHole = Content.Load<Texture2D>("Content/Sprites/Game/sprCrateHole");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeCrateHole newBlock = new MazeCrateHole(game, x, y, layer);
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

            //Sets custom variables.
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);
            return newBlock;
        }

        /// <summary>
        /// Deletes crates on the hole (moved to hole by MngrLvl). Controls
        /// sprite frame used.
        /// </summary>
        public override void Update()
        {
            //If a crate is on the hole, fills it and deletes the crate.
            if (isSolid)
            {
                //Gets a list of all crates on the hole.
                List<GameObj> items = game.mngrLvl.items.Where(o =>
                    o.x == x && o.y == y && o.layer == layer &&
                    o.type == Type.Crate).ToList();

                //Removes the first crate and fills the hole.
                if (items.Count != 0)
                {
                    game.mngrLvl.RemoveItem(items[0]);
                    game.playlist.Play(sndCrateHole, x, y);

                    spriteAtlas.frame = 1;
                    isSolid = false;
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
                game.mngrLvl.tooltip += "Hole | ";
            }
        }
    }
}