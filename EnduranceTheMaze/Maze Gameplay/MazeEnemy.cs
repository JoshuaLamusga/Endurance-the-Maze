﻿using System;
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
    /// An enemy which bounces in its indicated directions.
    /// Interacts with environment. Damages actor on contact.
    /// 
    /// Dependencies: MngrLvl, MazeBlock, MazeBelt.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeEnemy : GameObj
    {
        //Relevant assets.
        public static Texture2D texEnemy { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;     

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeEnemy(MainLoop game, int x, int y, int layer) :
            base(game, x, y, layer)
        {
            //Sets default values.
            isSolid = true;
            type = Type.Enemy;

            //Sets sprite information.
            sprite = new Sprite(true, texEnemy);
            sprite.depth = 0.4f;
            sprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 4, 1, 4);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texEnemy = Content.Load<Texture2D>("Content/Sprites/Game/sprEnemy");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeEnemy newBlock =
                new MazeEnemy(game, x, y, layer);
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
            #region Determines sprite by dir and isEnabled.
            if (Utils.DirCardinal(dir))
            {
                if (isEnabled)
                {
                    spriteAtlas.frame = 0;
                }
                else
                {
                    spriteAtlas.frame = 1;
                }
            }
            else
            {
                if (isEnabled)
                {
                    spriteAtlas.frame = 2;
                }
                else
                {
                    spriteAtlas.frame = 3;
                }
            }
            #endregion

            spriteAtlas.Update(true);
            base.Update();
        }

        /// <summary>
        /// Draws the enemy. When hovered, draws enabledness/info.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            //Sets the tooltip to display disabled status and info.
            if (Sprite.isIntersecting(sprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                layer == game.mngrLvl.actor.layer)
            {
                game.mngrLvl.tooltip += "Enemy";

                if (!isEnabled)
                {
                    game.mngrLvl.tooltip += "(disabled)";
                }

                game.mngrLvl.tooltip += " | ";
            }
        }
    }
}