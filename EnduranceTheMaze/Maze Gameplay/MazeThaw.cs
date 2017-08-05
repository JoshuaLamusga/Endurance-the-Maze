using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Enables all actors on touch (as disabled by freeze).
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeThaw : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndThaw;
        public static Texture2D TexThaw { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeThaw(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            BlockType = Type.Thaw;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexThaw);
            BlockSprite.depth = 0.204f;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 13, 1, 13);
            spriteAtlas.frameSpeed = 0.25f;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndThaw = Content.Load<SoundEffect>("Content/Sounds/sndThaw");
            TexThaw = Content.Load<Texture2D>("Content/Sprites/Game/sprThaw");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeThaw newBlock = new MazeThaw(game, X, Y, Layer);
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

            //Sets specific variables.
            newBlock.BlockSprite = BlockSprite;
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas);
            return newBlock;
        }

        /// <summary>
        /// If actors are synced, disables them on touch and deletes itself.
        /// </summary>
        public override void Update()
        {
            //If actors are synchronized.
            if (game.mngrLvl.opSyncActors)
            {
                //Gets a list of all actors on the thaw object.
                List<GameObj> items = game.mngrLvl.items.Where(o =>
                    o.X == X && o.Y == Y && o.Layer == Layer &&
                    o.BlockType == Type.Actor).ToList();

                //Enables all actors on contact.
                if (items.Count != 0)
                {
                    items = game.mngrLvl.items.Where(o =>
                        o.BlockType == Type.Actor).ToList();

                    foreach (GameObj item in items)
                    {
                        item.IsEnabled = true;
                    }

                    game.mngrLvl.RemoveItem(this);
                    game.playlist.Play(sndThaw, X, Y);
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
                game.mngrLvl.tooltip += "Thaw | ";
            }
        }
    }
}