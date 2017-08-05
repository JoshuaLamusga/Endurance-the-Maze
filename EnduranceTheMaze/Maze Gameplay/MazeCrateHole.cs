using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace EnduranceTheMaze
{
    /// <summary>
    /// A hole that can be filled by a crate. Acts like a wall.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeCrateHole : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndCrateHole;
        public static Texture2D TexCrateHole { get; private set; }

        //Sprite information.
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeCrateHole(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            IsSolid = true;
            BlockType = Type.CrateHole;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexCrateHole);
            BlockSprite.depth = 0.403f;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 2, 1, 2);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndCrateHole = Content.Load<SoundEffect>("Content/Sounds/sndCrateHole");
            TexCrateHole = Content.Load<Texture2D>("Content/Sprites/Game/sprCrateHole");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeCrateHole newBlock = new MazeCrateHole(game, X, Y, Layer);
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
        /// Deletes crates on the hole (moved to hole by MngrLvl). Controls
        /// sprite frame used.
        /// </summary>
        public override void Update()
        {
            //If a crate is on the hole, fills it and deletes the crate.
            if (IsSolid)
            {
                //Gets a list of all crates on the hole.
                List<GameObj> items = game.mngrLvl.items.Where(o =>
                    o.X == X && o.Y == Y && o.Layer == Layer &&
                    o.BlockType == Type.Crate).ToList();

                //Removes the first crate and fills the hole.
                if (items.Count != 0)
                {
                    game.mngrLvl.RemoveItem(items[0]);
                    game.playlist.Play(sndCrateHole, X, Y);

                    spriteAtlas.frame = 1;
                    IsSolid = false;
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
            base.Draw();

            //Sets the tooltip to display information on hover.
            if (Sprite.IsIntersecting(BlockSprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                Layer == game.mngrLvl.actor.Layer)
            {
                game.mngrLvl.tooltip += "Hole | ";
            }
        }
    }
}