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
    /// Moves blocks on top according to its direction unless blocked.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeBelt : GameObj
    {
        //Relevant assets.
        public static Texture2D texBelt { get; private set; }

        //Sprite information.
        public SpriteAtlas spriteAtlas; //Set by MngrLvl.cs.

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeBelt(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Belt;

            //Sets sprite information.
            sprite = new Sprite(true, texBelt);
            sprite.depth = 0.401f;
            sprite.originOffset = true;
            sprite.drawBehavior = SpriteDraw.all;

            //Custom variables.
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 9, 1, 9);
            spriteAtlas.CenterOrigin();
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texBelt = Content.Load<Texture2D>("Content/Sprites/Game/sprBelt");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeBelt newBlock = new MazeBelt(game, x, y, layer);
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
            newBlock.spriteAtlas = spriteAtlas;
            return newBlock;
        }

        /// <summary>
        /// Determines belt orientation. Belt movements handled by MngrLvl.
        /// </summary>
        public override void Update()
        {
            //Updates the belt sprite by direction.
            //Depends on the texture frames.
            if (dir == Dir.Right)
            {
                sprite.angle = 0;
            }
            else if (dir == Dir.Down)
            {
                sprite.angle = (float)(Math.PI / 2);
            }
            else if (dir == Dir.Left)
            {
                sprite.angle = (float)(Math.PI);
            }
            else
            {
                sprite.angle = (float)(-Math.PI / 2);
            }

            spriteAtlas.Update(true);
            base.Update();

            //Determines the belt's image speed.
            if (isEnabled)
            {
                spriteAtlas.frameSpeed = 0.25f;
            }
            else
            {
                spriteAtlas.frameSpeed = 0;
            }
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
                game.mngrLvl.tooltip += "Belt | ";
            }
        }
    }
}