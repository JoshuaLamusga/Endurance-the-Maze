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
    /// Single crates can be pushed in any direction and interact with all
    /// obstacles.
    /// 
    /// Activation types:
    /// 5: Breaks when activated.
    /// 
    /// Custom properties of custInt1:
    /// > 5: When broken, contained object is nth - 5 entry of the Type enum.
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeCrate : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndBreakCrate;
        public static Texture2D texCrate { get; private set; }

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeCrate(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            isSolid = true;
            type = Type.Crate;

            //Sets sprite information.
            sprite = new Sprite(true, texCrate);
            sprite.depth = 0.3f;
            sprite.rectSrc = new SmoothRect(0, 0, 32, 32);
            sprite.rectDest.Width = 32;
            sprite.rectDest.Height = 32;
            sprite.drawBehavior = SpriteDraw.basicAnimated;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texCrate = Content.Load<Texture2D>("Content/Sprites/Game/sprCrate");
            sndBreakCrate = Content.Load<SoundEffect>("Content/Sounds/sndBreakCrate");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeCrate newBlock = new MazeCrate(game, x, y, layer);
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
            return newBlock;
        }

        /// <summary>
        /// Checks if the crate is broken open.
        /// </summary>
        public override void Update()
        {
            //If the crate breaks.
            if (isActivated && actionType == 5)
            {
                //Deactivates and plays the crate breaking sound.
                isActivated = false;
                game.playlist.Play(sndBreakCrate, x, y);

                //Removes the crate, adds a broken crate picture, and adds
                //the contained item, if any.
                game.mngrLvl.RemoveItem(this);
                game.mngrLvl.AddItem(new MazeCrateBroken(game, x, y, layer));
                if (custInt1 != 0)
                {
                    game.mngrLvl.AddItem(Utils.BlockFromType
                            (game, (Type)(custInt1 - 1), x, y, layer));
                }
            }

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
                game.mngrLvl.tooltip += "Crate | ";
            }
        }
    }
}