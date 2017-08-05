using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace EnduranceTheMaze
{
    /// <summary>
    /// A colored key that corresponds with colored locks. Pick them up and
    /// move into locks to "unlock" (delete) them.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: Key colors:
    /// 0: Blue
    /// 1: Red
    /// 2: Goldenrod
    /// 3: Purple
    /// 4: Orange
    /// 5: Black
    /// 6: Dark blue
    /// 7: Dark red
    /// 8: Dark goldenrod
    /// 9: Dark orange
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeKey : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndCollectKey;
        public static Texture2D TexKey { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeKey(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            BlockType = Type.Key;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexKey);
            BlockSprite.depth = 0.207f;
            BlockSprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 19, 2, 10);
            spriteAtlas.frameSpeed = 0.2f;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndCollectKey = Content.Load<SoundEffect>("Content/Sounds/sndCollectKey");
            TexKey = Content.Load<Texture2D>("Content/Sprites/Game/sprKey");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeKey newBlock = new MazeKey(game, X, Y, Layer);
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
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);
            return newBlock;
        }

        /// <summary>
        /// Adds to the actor's list of keys on contact.
        /// </summary>
        public override void Update()
        {
            //Determines the key color.
            switch (CustInt1)
            {
                case (0):
                    BlockSprite.color = Color.Blue;
                    break;
                case (1):
                    BlockSprite.color = Color.Red;
                    break;
                case (2):
                    BlockSprite.color = Color.Goldenrod;
                    break;
                case (3):
                    BlockSprite.color = Color.Purple;
                    break;
                case (4):
                    BlockSprite.color = Color.Orange;
                    break;
                case (5):
                    BlockSprite.color = Color.Black;
                    break;
                case (6):
                    BlockSprite.color = Color.DarkBlue;
                    break;
                case (7):
                    BlockSprite.color = Color.DarkRed;
                    break;
                case (8):
                    BlockSprite.color = Color.DarkGoldenrod;
                    break;
                case (9):
                    BlockSprite.color = Color.DarkOrange;
                    break;
            }

            //Gets a list of all actors on the key object.
            List<GameObj> items = game.mngrLvl.items.Where(o =>
                o.X == X && o.Y == Y && o.Layer == Layer &&
                o.BlockType == Type.Actor).ToList();

                //Adds the key to the index of the first actor to touch it.
                if (items.Count != 0)
                {
                    (items[0] as MazeActor).keys.Add(BlockSprite.color);
                    game.mngrLvl.RemoveItem(this);
                    game.playlist.Play(sndCollectKey, X, Y);
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
                game.mngrLvl.tooltip += "Key | ";
            }
        }
    }
}