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
    /// Adds to the level score.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeCoin : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndCollectCoin;
        public static Texture2D texCoin { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeCoin(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Coin;

            //Sets sprite information.
            sprite = new Sprite(true, texCoin);
            sprite.depth = 0.205f;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 19, 2, 10);
            spriteAtlas.frameSpeed = 0.2f;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndCollectCoin = Content.Load<SoundEffect>("Content/Sounds/sndCollectCoin");
            texCoin = Content.Load<Texture2D>("Content/Sprites/Game/sprCoin");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeCoin newBlock = new MazeCoin(game, x, y, layer);
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
        /// Adds to the score and deletes itself on contact.
        /// </summary>
        public override void Update()
        {
            //Gets a list of all actors on the coin object.
            List<GameObj> items = game.mngrLvl.items.Where(o =>
                o.x == x && o.y == y && o.layer == layer &&
                o.type == Type.Actor).ToList();

            //If there is at least one actor touching the coin.
            if (items.Count != 0)
            {
                game.mngrLvl.actorCoins++;
                game.mngrLvl.RemoveItem(this);
                game.playlist.Play(sndCollectCoin, x, y);
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
                game.mngrLvl.tooltip += "Coin | ";
            }
        }
    }
}