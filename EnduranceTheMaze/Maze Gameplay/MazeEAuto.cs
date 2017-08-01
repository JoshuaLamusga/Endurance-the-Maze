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
    /// Automatically activates every couple frames.
    /// 
    /// Activation types.
    /// 5: Activates linked items on trigger.
    /// 6: Deactivates linked items on trigger.
    /// 7: De/activates linked items each other trigger.
    /// 
    /// Custom properties of custInt1:
    /// > 0: The delay in frames before each trigger.
    /// 0: Never triggers.
    /// Custom properties of custInt2:
    /// 0: All activated items are activated regardless of layer.
    /// 1: Only activated items on the same layer are activated.
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeEAuto : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndActivateAuto;
        public static Texture2D texEAuto { get; private set; }

        //Sprite information.
        private SpriteAtlas spriteAtlas;

        //Custom variables.
        private int timer;
        private bool hasActivated;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeEAuto(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.EAuto;
            isSolid = true;

            //Sets sprite information.
            sprite = new Sprite(true, texEAuto);
            sprite.depth = 0.417f;
            //Note that there are actually 6 frames.
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 3, 2, 3);

            //Sets custom variables.
            timer = custInt1; //Sets the timer to the max value.
            hasActivated = false; //The switch hasn't activated yet.
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndActivateAuto = Content.Load<SoundEffect>("Content/Sounds/sndActivateAuto");
            texEAuto = Content.Load<Texture2D>("Content/Sprites/Game/sprEAuto");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeEAuto newBlock = new MazeEAuto(game, x, y, layer);
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
            newBlock.timer = timer;
            newBlock.hasActivated = hasActivated;
            return newBlock;
        }

        /// <summary>
        // Counts down to zero and activates linked items at zero.
        /// </summary>
        public override void Update()
        {
            #region Adjusts sprite.
            if (isEnabled)
            {
                spriteAtlas.frameSpeed = 0.2f;
            }
            else
            {
                spriteAtlas.frameSpeed = 0;
            }
            #endregion

            //Counts down the timer and activates at zero.
            if (isEnabled)
            {
                timer--;
                if (timer <= 0 && custInt1 > 0)
                {
                    timer = custInt1;
                    isActivated = true;
                }

                //Handles automated activation.
                if (isActivated && actionType > 4)
                {
                    //Deactivates the item and plays a sound.
                    isActivated = false;
                    hasActivated = true;
                    game.playlist.Play(sndActivateAuto, x, y);

                    //Gets all items matching the index to affect.
                    List<GameObj> items = game.mngrLvl.items.Where(o =>
                        o.actionIndex == actionIndex2).ToList();

                    //Filters out blocks on different layers.
                    if (custInt2 == 1)
                    {
                        items = items.Where(o => o.layer == layer).ToList();
                    }

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
                else if (hasActivated)
                {
                    hasActivated = false;
                    spriteAtlas.frames = 3;
                    spriteAtlas.frame -= 3;
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
            if (hasActivated)
            {
                spriteAtlas.frames = 6;
                spriteAtlas.frame += 3;
                spriteAtlas.Update(true);
            }
            base.Draw();

            //Sets the tooltip to display information on hover.
            if (Sprite.isIntersecting(sprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                layer == game.mngrLvl.actor.layer)
            {
                game.mngrLvl.tooltip += "E-auto ";

                if (custInt1 != 0)
                {
                    game.mngrLvl.tooltip += "(triggers every " + custInt1 +
                        " frames.) ";
                }

                game.mngrLvl.tooltip += "| ";
            }
        }
    }
}