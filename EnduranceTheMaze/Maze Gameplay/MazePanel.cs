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
    /// When activated by being held down, it activates all blocks with the
    /// same activation index.
    /// 
    /// Activation types:
    /// 5: Activates continuously and deactivates when no longer held.
    /// 6: Activates once and doesn't deactivate when no longer held.
    /// 7: Activates once and disables itself so it can't be activated.
    /// 
    /// Custom properties of custInt1:
    /// 0: All activated items are activated regardless of layer.
    /// 1: Only activated items on the same layer are activated.
    /// Custom properties of custStr: none.
    /// Custom properties of custInt2: none.
    /// 
    /// </summary>
    public class MazePanel : GameObj
    {
        //Relevant assets.
        public static Texture2D texPanel { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        //Contains whether the panel has been activated or not.
        bool hasActivated, isHeld;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazePanel(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Panel;

            //Sets sprite information.
            sprite = new Sprite(true, texPanel);
            sprite.depth = 0.414f;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 4, 1, 4);

            //False by default because it hasn't been activated.
            hasActivated = false;
            isHeld = false;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texPanel = Content.Load<Texture2D>("Content/Sprites/Game/sprPanel");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazePanel newBlock = new MazePanel(game, x, y, layer);
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
            newBlock.hasActivated = hasActivated;
            newBlock.isHeld = isHeld;
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, true);
            return newBlock;
        }

        /// <summary>
        /// Transfers between senders and receivers on contact if possible.
        /// </summary>
        public override void Update()
        {
            //Gets a list of all solid blocks in the same location.
            List<GameObj> items = game.mngrLvl.items.Where(o =>
                o.x == x && o.y == y && o.layer == layer &&
                o.isSolid).ToList();

            if (isEnabled)
            {
                //Sets whether the panel is pressed down or not.
                if (items.Count > 0)
                {
                    isHeld = true;
                }
                else
                {
                    isHeld = false;
                }

                //If activated or held down.
                if (isActivated || isHeld)
                {
                    isActivated = false;

                    //All connected items are activated.
                    if (!hasActivated && actionType > 4)
                    {
                        //Gets all items matching the index to affect.
                        items = game.mngrLvl.items.Where(o =>
                            o.actionIndex == actionIndex2).ToList();

                        //Filters out blocks on different layers.
                        if (custInt1 == 1)
                        {
                            items = items.Where(o => o.layer == layer)
                                .ToList();
                        }

                        //If there are linked items to activate, plays sound.
                        if (items.Count != 0)
                        {
                            game.playlist.Play(sndActivated, x, y);
                        }

                        foreach (GameObj item in items)
                        {
                            item.isActivated = true;
                        }
                    }

                    //The panel is now activated.
                    hasActivated = true;
                }
                else
                {
                    if (hasActivated)
                    {
                        //Gets all items matching the index to affect.
                        items = game.mngrLvl.items.Where(o =>
                            o.actionIndex == actionIndex2).ToList();

                        //Filters out blocks on different layers.
                        if (custInt1 == 1)
                        {
                            items = items.Where(o =>
                                o.layer == layer).ToList();
                        }

                        if (actionType == 5)
                        {
                            foreach (GameObj item in items)
                            {
                                item.isActivated = false;
                            }
                        }
                        else if (actionType == 7)
                        {
                            isEnabled = false;
                        }
                    }

                    //The panel is now deactivated.
                    hasActivated = false;
                }
            }

            #region Adjusts sprite.
            //Adjusts the sprite frame.
            if (!isHeld)
            {
                spriteAtlas.frame = 0; //Not pressed.
            }
            else
            {
                spriteAtlas.frame = 1; //Pressed.
            }
            //Depends on frame positions and texture.
            if (!isEnabled)
            {
                spriteAtlas.frame += 2;
            }
            #endregion

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
                game.mngrLvl.tooltip += "Panel ";

                if (actionType == 7)
                {
                    game.mngrLvl.tooltip += "(only activates once)";
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