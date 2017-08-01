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
    /// Acts like a wall, except that objects can pass through in one or two
    /// directions. Interaction logic stored in MazeActor and MngrLvl.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1:
    /// 0: one-way.
    /// 1: two-way.
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeMultiWay : GameObj
    {
        //Relevant assets.
        public static Texture2D texMultiWay { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeMultiWay(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            isSolid = true;
            type = Type.MultiWay;

            //Sets sprite information.
            sprite = new Sprite(true, texMultiWay);
            sprite.depth = 0.408f;
            sprite.originOffset = true;
            sprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 4, 1, 4);
            spriteAtlas.CenterOrigin();
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texMultiWay = Content.Load<Texture2D>("Content/Sprites/Game/sprMultiWay");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeMultiWay newBlock = new MazeMultiWay(game, x, y, layer);
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

            //Sets custom variables.
            newBlock.sprite = sprite;
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);

            return newBlock;
        }

        /// <summary>
        /// Determines orientation. Multiway interaction handled by
        /// MngrLvl and MazeActor.
        /// </summary>
        public override void Update()
        {
            //Updates the sprite by direction.
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

            //Determines the frame used.
            //Dependent on frame order.
            if (custInt1 == 0)
            {
                spriteAtlas.frame = 0;
            }
            else
            {
                spriteAtlas.frame = 2;
            }
            if (!isEnabled)
            {
                spriteAtlas.frame += 1;
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
                    game.mngrLvl.tooltip += "One-way | ";
                }
                else
                {
                    game.mngrLvl.tooltip += "Two-way | ";
                }
            }            
        }
    }
}