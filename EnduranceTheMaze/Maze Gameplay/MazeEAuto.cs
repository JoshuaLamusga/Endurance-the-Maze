using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

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
        public static Texture2D TexEAuto { get; private set; }

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
            BlockType = Type.EAuto;
            IsSolid = true;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexEAuto);
            BlockSprite.depth = 0.417f;
            //Note that there are actually 6 frames.
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 3, 2, 3);

            //Sets custom variables.
            timer = CustInt1; //Sets the timer to the max value.
            hasActivated = false; //The switch hasn't activated yet.
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndActivateAuto = Content.Load<SoundEffect>("Content/Sounds/sndActivateAuto");
            TexEAuto = Content.Load<Texture2D>("Content/Sprites/Game/sprEAuto");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeEAuto newBlock = new MazeEAuto(game, X, Y, Layer);
            newBlock.ActionIndex = ActionIndex;
            newBlock.ActionIndex2 = ActionIndex2;
            newBlock.ActionType = ActionType;
            newBlock.CustInt1 = CustInt1;
            newBlock.CustInt2 = CustInt2;
            newBlock.CustStr = CustStr;
            newBlock.BlockDir = BlockDir;
            newBlock.IsActivated = IsActivated;
            newBlock.IsEnabled = IsEnabled;
            newBlock.IsVisible = IsVisible;
            newBlock.BlockSprite = BlockSprite;

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
            if (IsEnabled)
            {
                spriteAtlas.frameSpeed = 0.2f;
            }
            else
            {
                spriteAtlas.frameSpeed = 0;
            }
            #endregion

            //Counts down the timer and activates at zero.
            if (IsEnabled)
            {
                timer--;
                if (timer <= 0 && CustInt1 > 0)
                {
                    timer = CustInt1;
                    IsActivated = true;
                }

                //Handles automated activation.
                if (IsActivated && ActionType > 4)
                {
                    //Deactivates the item and plays a sound.
                    IsActivated = false;
                    hasActivated = true;
                    game.playlist.Play(sndActivateAuto, X, Y);

                    //Gets all items matching the index to affect.
                    List<GameObj> items = game.mngrLvl.items.Where(o =>
                        o.ActionIndex == ActionIndex2).ToList();

                    //Filters out blocks on different layers.
                    if (CustInt2 == 1)
                    {
                        items = items.Where(o => o.Layer == Layer).ToList();
                    }

                    if (ActionType == 5)
                    {
                        foreach (GameObj item in items)
                        {
                            item.IsActivated = true;
                        }
                    }
                    else if (ActionType == 6)
                    {
                        foreach (GameObj item in items)
                        {
                            item.IsActivated = false;
                        }
                    }
                    else if (ActionType == 7)
                    {
                        foreach (GameObj item in items)
                        {
                            item.IsActivated = !item.IsActivated;
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
            if (Sprite.IsIntersecting(BlockSprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                Layer == game.mngrLvl.actor.Layer)
            {
                game.mngrLvl.tooltip += "E-auto ";

                if (CustInt1 != 0)
                {
                    game.mngrLvl.tooltip += "(triggers every " + CustInt1 +
                        " frames.) ";
                }

                game.mngrLvl.tooltip += "| ";
            }
        }
    }
}