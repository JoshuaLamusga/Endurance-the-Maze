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
    /// After custInt1 passes over the block, filters are replaced by a block.
    /// 
    /// Activation types: Creates an object of the desired type.
    /// >= 5: The object is the nth - 5 entry of the Type enum.
    /// 
    /// Custom properties of custInt1:
    /// > 0: Number of passes to be made.
    /// -1: Cannot be activated by passing over it.
    /// 
    /// Custom properties of custInt2:
    /// 0: not solid
    /// 1: solid
    /// 
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeFilter : GameObj
    {
        //Relevant assets.
        public static Texture2D texFilter { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeFilter(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Filter;

            //Sets sprite information.
            sprite = new Sprite(true, texFilter);
            sprite.depth = 0.405f;
            sprite.originOffset = true;
            sprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 19, 2, 10);
            spriteAtlas.CenterOrigin();
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texFilter = Content.Load<Texture2D>("Content/Sprites/Game/sprFilter");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeFilter newBlock = new MazeFilter(game, x, y, layer);
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
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);

            return newBlock;
        }

        /// <summary>
        /// Handled by MazeActor.cs
        /// </summary>
        public override void Update()
        {
            //Determines solidity by custInt1.
            if (custInt2 == 1)
            {
                isSolid = true;
            }
            else
            {
                isSolid = false;
            }

            //Determines animation by enabledness.
            if (isEnabled)
            {
                spriteAtlas.frameSpeed = 0.35f;
            }
            else
            {
                spriteAtlas.frame = 0;
                spriteAtlas.frameSpeed = 0;
            }

            //Handles activation behavior.
            if (isEnabled && (isActivated || custInt1 == 0))
            {
                //Plays the activation sound.
                game.playlist.Play(sndActivated, x, y);

                //Removes this block from the level.
                game.mngrLvl.RemoveItem(this);

                //Creates different blocks based on action type.
                if (actionType > 4)
                {
                    game.mngrLvl.AddItem(Utils.BlockFromType
                        (game, (Type)(actionType - 5), x, y, layer));
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
                if (custInt1 > 0)
                {
                    game.mngrLvl.tooltip += "Filter: " + custInt1 +
                        " more passe(s) ";
                }
                else
                {
                    game.mngrLvl.tooltip += "Filter";
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