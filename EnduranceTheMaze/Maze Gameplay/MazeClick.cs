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
    /// Activates when clicked.
    /// 
    /// Activation types
    /// 5: Activates linked items on trigger.
    /// 6: Deactivates linked items on trigger.
    /// 7: De/activates linked items each other trigger.
    /// 
    /// Custom properties of custInt1:
    /// 0: Functions normally.
    /// 1: Deletes itself after one use.
    /// 
    /// Custom properties of custInt2:
    /// 0: All activated items are activated regardless of layer.
    /// 1: Only activated items on the same layer are activated.
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeClick : GameObj
    {
        //Relevant assets.
        public static Texture2D texClick { get; private set; }

        //Custom variables.
        SpriteAtlas spriteAtlas;
        bool isShrinking = true;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeClick(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Click;

            //Sets sprite information.
            sprite = new Sprite(true, texClick);
            sprite.depth = 0.201f;
            sprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 2, 1, 2);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texClick = Content.Load<Texture2D>("Content/Sprites/Game/sprClick");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeClick newBlock = new MazeClick(game, x, y, layer);
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

            //Custom variables.
            newBlock.sprite = sprite;
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas);
            return newBlock;
        }

        /// <summary>
        /// Clicks to activate.
        /// </summary>
        public override void Update()
        {
            #region Adjusts sprite and handles growing/shrinking animation.
            if (isEnabled)
            {
                spriteAtlas.frame = 0;
                if (isShrinking)
                {
                    sprite.scaleX -= 0.01f;
                    sprite.scaleY -= 0.01f;
                    if (sprite.scaleX <= 0.5f)
                    {
                        isShrinking = false;
                    }
                }
                else
                {
                    spriteAtlas.frame = 0;
                    sprite.scaleX += 0.01f;
                    sprite.scaleY += 0.01f;
                    if (sprite.scaleX >= 1)
                    {
                        isShrinking = true;
                    }
                }
            }
            else
            {
                spriteAtlas.frame = 1;
            }
            #endregion

            if (isEnabled)
            {
                //The block activates itself when clicked.
                if (Sprite.isIntersecting(sprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                layer == game.mngrLvl.actor.layer)
                {
                    if (game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released)
                    {
                        isActivated = true;
                    }
                }

                //Handles activation behavior.
                if (isActivated && actionType > 4)
                {
                    //Deletes itself if applicable.
                    if (custInt1 == 1)
                    {
                        game.mngrLvl.RemoveItem(this);
                    }

                    //Gets all items matching the index to affect.
                    List<GameObj> items = game.mngrLvl.items.Where(o =>
                        o.actionIndex == actionIndex2).ToList();

                    //Filters out blocks on different layers.
                    if (custInt2 == 1)
                    {
                        items = items.Where(o => o.layer == layer).ToList();
                    }

                    //Deactivates the item and plays sound.
                    isActivated = false;
                    game.playlist.Play(sndActivated, x, y);

                    if (actionType == 5)
                    {
                        foreach (GameObj item in items)
                        {
                            item.isActivated = true;
                        }
                    }
                    else if (actionType == 6)
                    {
                        foreach (GameObj item in items)
                        {
                            item.isActivated = false;
                        }
                    }
                    else if (actionType == 7)
                    {
                        foreach (GameObj item in items)
                        {
                            item.isActivated = !item.isActivated;
                        }
                    }
                }
            }

            spriteAtlas.Update(true);
            base.Update();
        }

        /// <summary>
        /// Draws the sprite. Sets a tooltip.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            //Sets the tooltip to display information on hover.
            if (Sprite.isIntersecting(sprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                layer == game.mngrLvl.actor.layer)
            {
                game.mngrLvl.tooltip += "Clickable";

                if (!isEnabled)
                {
                    game.mngrLvl.tooltip += "(disabled)";
                }

                game.mngrLvl.tooltip += " | ";
            }
        }
    }
}