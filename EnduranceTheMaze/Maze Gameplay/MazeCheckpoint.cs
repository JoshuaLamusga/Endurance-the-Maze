using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Saves the current progress of the player.
    /// 
    /// Custom properties of custInt1:
    /// 0: Saves every time an actor occupies the area.
    /// 1: Saves once and vanishes.
    /// 
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeCheckpoint : GameObj
    {
        //Relevant assets.
        public static Texture2D TexCheckpoint { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        //Contains whether the checkpoint has been activated or not.
        bool hasActivated;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeCheckpoint(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            BlockType = Type.Checkpoint;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexCheckpoint);
            BlockSprite.depth = 0.208f;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 19, 2, 10);
            spriteAtlas.frameSpeed = 0.35f;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexCheckpoint = Content.Load<Texture2D>("Content/Sprites/Game/sprCheckpoint");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeCheckpoint newBlock = new MazeCheckpoint(game, X, Y, Layer);
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

            //Custom variables.
            newBlock.BlockSprite = BlockSprite;
            newBlock.hasActivated = hasActivated;
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);
            return newBlock;
        }

        /// <summary>
        /// Saves if touched by an actor, deleting itself if custInt1 == 1.
        /// </summary>
        public override void Update()
        {
            //Gets a list of all actors in the same position.
            List<GameObj> items = game.mngrLvl.items.Where(o =>
                o.X == X && o.Y == Y && o.Layer == Layer &&
                o.BlockType == Type.Actor).ToList();

            if (items.Count > 0) //Attempts to save.
            {
                if (!hasActivated)
                {
                    game.mngrLvl.doCheckpoint = true;

                    if (CustInt1 == 1)
                    {
                        game.mngrLvl.RemoveItem(this);
                    }
                }

                hasActivated = true;
            }
            else //Doesn't attempt to save.
            {
                hasActivated = false;
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
                game.mngrLvl.tooltip += "Checkpoint";
                
                if (CustInt1 == 1)
                {
                    game.mngrLvl.tooltip += "(disappears on touch)";
                }
                if (!IsEnabled)
                {
                    game.mngrLvl.tooltip += "(disabled)";
                }

                game.mngrLvl.tooltip += " | ";
            }
        }
    }
}