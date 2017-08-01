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
    /// Impassable when solid. Activation toggles solidity.
    /// 
    /// Activation types.
    /// 5: Switch solidity.
    /// 6: Toggle solidity.
    /// 
    /// Custom properties of custInt1:
    /// 0: Won't close on anything solid.
    /// 1: May close on actors.
    /// 
    /// Custom properties of custInt2:
    /// 0: Is not solid.
    /// 1: Is solid.
    ///
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeGate : GameObj
    {
        //Relevant assets.
        public static Texture2D texGate { get; private set; }

        //Sprite information.
        private SpriteAtlas spriteAtlas;

        //True after the first call to update.
        private bool updateCalled = false;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeGate(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Gate;

            //Sets sprite information.
            sprite = new Sprite(true, texGate);
            sprite.depth = 0.102f;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 2, 1, 2);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texGate = Content.Load<Texture2D>("Content/Sprites/Game/sprGate");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeGate newBlock = new MazeGate(game, x, y, layer);
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
            return newBlock;
        }

        /// <summary>
        /// Controls sprite frames used and toggles isSolid.
        /// </summary>
        public override void Update()
        {
            if (!updateCalled)
            {
                //Starts as solid if chosen.
                if (custInt2 == 1)
                {
                    isSolid = true;
                }

                updateCalled = true;
            }

            //Determines the old solidity.
            bool wasSolid = isSolid;

            //Handles activation.
            if (isEnabled)
            {
                if (isActivated)
                {
                    //If the gate should toggle solidity.
                    if (actionType == 5)
                    {
                        isSolid = !isSolid;

                        isActivated = false;
                        game.playlist.Play(sndActivated, x, y);
                    }
                    //If the gate should be solid while active.
                    else if (actionType == 6)
                    {
                        isSolid = true;
                    }
                    //If the gate should be non-solid while active.
                    else if (actionType == 7)
                    {
                        isSolid = false;
                    }
                }
                else
                {
                    if (actionType == 6)
                    {
                        isSolid = false;
                    }
                    else if (actionType == 7)
                    {
                        isSolid = true;
                    }
                }

                //If the solidity changed.
                if (wasSolid != isSolid)
                {
                    //All solids at the gate position, except itself.
                    List<GameObj> trappedActors = new List<GameObj>();
                    List<GameObj> items = game.mngrLvl.items.Where(o =>
                        o.x == x && o.y == y && o.layer == layer && o.isSolid)
                        .ToList();
                    items.Remove(this);

                    //Solids prevent gate closure, so if it can close on actors,
                    //the actors must be removed from the list.
                    if (custInt1 == 1)
                    {
                        trappedActors = items.Where(o =>
                            o.type == Type.Actor).ToList();
                    }
                    //The gate becomes open if it can't close on solids.
                    if (items.Count - trappedActors.Count != 0)
                    {
                        isSolid = false;
                    }

                    //Actors lose if trapped by a gate.
                    if (trappedActors.Count != 0)
                    {
                        foreach (GameObj item in trappedActors)
                        {
                            (item as MazeActor).hp = 0;
                        }
                    }
                }

                //Determines the sprite via solidity.
                spriteAtlas.frame = (isSolid) ? 1 : 0;
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
                game.mngrLvl.tooltip += "Gate | ";
            }
        }
    }
}