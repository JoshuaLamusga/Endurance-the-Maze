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
        public static Texture2D texKey { get; private set; }

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
            type = Type.Key;

            //Sets sprite information.
            sprite = new Sprite(true, texKey);
            sprite.depth = 0.207f;
            sprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 19, 2, 10);
            spriteAtlas.frameSpeed = 0.2f;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndCollectKey = Content.Load<SoundEffect>("Content/Sounds/sndCollectKey");
            texKey = Content.Load<Texture2D>("Content/Sprites/Game/sprKey");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeKey newBlock = new MazeKey(game, x, y, layer);
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
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);
            return newBlock;
        }

        /// <summary>
        /// Adds to the actor's list of keys on contact.
        /// </summary>
        public override void Update()
        {
            //Determines the key color.
            switch (custInt1)
            {
                case (0):
                    sprite.color = Color.Blue;
                    break;
                case (1):
                    sprite.color = Color.Red;
                    break;
                case (2):
                    sprite.color = Color.Goldenrod;
                    break;
                case (3):
                    sprite.color = Color.Purple;
                    break;
                case (4):
                    sprite.color = Color.Orange;
                    break;
                case (5):
                    sprite.color = Color.Black;
                    break;
                case (6):
                    sprite.color = Color.DarkBlue;
                    break;
                case (7):
                    sprite.color = Color.DarkRed;
                    break;
                case (8):
                    sprite.color = Color.DarkGoldenrod;
                    break;
                case (9):
                    sprite.color = Color.DarkOrange;
                    break;
            }

            //Gets a list of all actors on the key object.
            List<GameObj> items = game.mngrLvl.items.Where(o =>
                o.x == x && o.y == y && o.layer == layer &&
                o.type == Type.Actor).ToList();

                //Adds the key to the index of the first actor to touch it.
                if (items.Count != 0)
                {
                    (items[0] as MazeActor).keys.Add(sprite.color);
                    game.mngrLvl.RemoveItem(this);
                    game.playlist.Play(sndCollectKey, x, y);
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
                game.mngrLvl.tooltip += "Key | ";
            }
        }
    }
}