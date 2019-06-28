using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Laser actuators can activate after receiving an amount of bullets.
    /// 
    /// Activation types:
    /// 5: Activates linked items every custInt1 bullets.
    /// 6: Activates/deactivates linked items every custInt1 bullets.
    /// 7: Activates linked items after custInt1 bullets, then deactivates after custInt2 frames without a bullet.
    /// 8: Activates after custInt1 bullets and doesn't deactivate.
    /// 
    /// Custom properties of custInt1:
    /// > -1: The number of bullets required to activate.
    /// Custom properties of custInt2:
    /// > -1: The number of frames to wait. At 0, deactivation is guaranteed next frame.
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeLaserActuator : GameObj
    {
        //Relevant assets.
        public static Texture2D TexLaserActuator { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        private bool receivedBullet = false;
        private bool isActivating = false;
        private int framesWithoutBullets = 0;
        private int bulletsReceived = 0;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeLaserActuator(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            IsSolid = true;
            BlockType = Type.LaserActuator;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexLaserActuator);
            BlockSprite.depth = 0.421f;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 5, 1, 5);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexLaserActuator = Content.Load<Texture2D>("Content/Sprites/Game/sprLaserActuator");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeLaserActuator newBlock = new MazeLaserActuator(game, X, Y, Layer);
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
            return newBlock;
        }

        /// <summary>
        /// Updates the sprite.
        /// </summary>
        public override void Update()
        {
            if (!IsEnabled)
            {
                spriteAtlas.frame = 0;
            }
            else if (receivedBullet)
            {
                spriteAtlas.frame = isActivating ? 3 : 4;
            }
            else
            {
                spriteAtlas.frame = isActivating ? 2 : 1;
            }

            if (!receivedBullet)
            {
                framesWithoutBullets++;

                if (ActionType == 7 && isActivating && (framesWithoutBullets > CustInt2 || CustInt2 == 0))
                {
                    framesWithoutBullets = 0;
                    bulletsReceived = 0;
                    ActivateLinkedItems((item) => false);
                    isActivating = false;
                }
            }

            spriteAtlas.Update(true);
            base.Update();
        }

        /// <summary>
        /// Toggles the expected events on linked items.
        /// </summary>
        /// <param name="setActivated"></param>
        private void ActivateLinkedItems(Func<GameObj, bool> setActivated)
        {
            isActivating = true;

            //Gets all items matching the index to affect.
            List<GameObj> items = game.mngrLvl.items.Where(o =>
                o.ActionIndex == ActionIndex2).ToList();

            //If there are linked items to activate, plays sound.
            if (items.Count != 0)
            {
                game.playlist.Play(sndActivated, X, Y);

                foreach (GameObj item in items)
                {
                    item.IsActivated = setActivated(item);
                }
            }
        }

        /// <summary>
        /// Handles receiving a bullet.
        /// </summary>
        public void ReceivedBullet()
        {
            framesWithoutBullets = 0;
            receivedBullet = true;
            bulletsReceived++;

            if ((ActionType == 5 || ActionType == 6) && bulletsReceived >= CustInt1)
            {
                bulletsReceived = 0;
                ActivateLinkedItems((item) => ActionType == 5 || !item.IsActivated);
            }
            else if ((ActionType == 7 || ActionType == 8) && !isActivating && bulletsReceived >= CustInt1)
            {
                bulletsReceived = 0;
                ActivateLinkedItems((item) => true);
            }
        }

        /// <summary>
        /// Draws the sprite. Sets an informational tooltip.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            //Sets the tooltip to display information on hover.
            if (Sprite.IsIntersecting(BlockSprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                Layer == game.mngrLvl.actor.Layer)
            {
                game.mngrLvl.tooltip += "Laser Actuator | ";
            }

            receivedBullet = false;

            if (ActionType != 7 && ActionType != 8)
            {
                isActivating = false;
            }
        }
    }
}