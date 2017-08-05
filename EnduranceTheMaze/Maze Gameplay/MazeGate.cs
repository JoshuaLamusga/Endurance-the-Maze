using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

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
        public static Texture2D TexGate { get; private set; }

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
            BlockType = Type.Gate;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexGate);
            BlockSprite.depth = 0.102f;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 2, 1, 2);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexGate = Content.Load<Texture2D>("Content/Sprites/Game/sprGate");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeGate newBlock = new MazeGate(game, X, Y, Layer);
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
                if (CustInt2 == 1)
                {
                    IsSolid = true;
                }

                updateCalled = true;
            }

            //Determines the old solidity.
            bool wasSolid = IsSolid;

            //Handles activation.
            if (IsEnabled)
            {
                if (IsActivated)
                {
                    //If the gate should toggle solidity.
                    if (ActionType == 5)
                    {
                        IsSolid = !IsSolid;

                        IsActivated = false;
                        game.playlist.Play(sndActivated, X, Y);
                    }
                    //If the gate should be solid while active.
                    else if (ActionType == 6)
                    {
                        IsSolid = true;
                    }
                    //If the gate should be non-solid while active.
                    else if (ActionType == 7)
                    {
                        IsSolid = false;
                    }
                }
                else
                {
                    if (ActionType == 6)
                    {
                        IsSolid = false;
                    }
                    else if (ActionType == 7)
                    {
                        IsSolid = true;
                    }
                }

                //If the solidity changed.
                if (wasSolid != IsSolid)
                {
                    //All solids at the gate position, except itself.
                    List<GameObj> trappedActors = new List<GameObj>();
                    List<GameObj> items = game.mngrLvl.items.Where(o =>
                        o.X == X && o.Y == Y && o.Layer == Layer && o.IsSolid)
                        .ToList();
                    items.Remove(this);

                    //Solids prevent gate closure, so if it can close on actors,
                    //the actors must be removed from the list.
                    if (CustInt1 == 1)
                    {
                        trappedActors = items.Where(o =>
                            o.BlockType == Type.Actor).ToList();
                    }
                    //The gate becomes open if it can't close on solids.
                    if (items.Count - trappedActors.Count != 0)
                    {
                        IsSolid = false;
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
                spriteAtlas.frame = (IsSolid) ? 1 : 0;
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
            if (Sprite.IsIntersecting(BlockSprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                Layer == game.mngrLvl.actor.Layer)
            {
                game.mngrLvl.tooltip += "Gate | ";
            }
        }
    }
}