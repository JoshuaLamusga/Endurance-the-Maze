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
    /// Disables whichever actor it touches.
    /// Thaw reactivates all disabled actors.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeFreeze : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndFreeze;
        public static Texture2D texFreeze { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeFreeze(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Freeze;

            //Sets sprite information.
            sprite = new Sprite(true, texFreeze);
            sprite.depth = 0.203f;
            sprite.originOffset = true;
            sprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 10, 1, 10);
            spriteAtlas.frameSpeed = 0.4f;
            spriteAtlas.CenterOrigin();
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndFreeze = Content.Load<SoundEffect>("Content/Sounds/sndFreeze");
            texFreeze = Content.Load<Texture2D>("Content/Sprites/Game/sprFreeze");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeFreeze newBlock = new MazeFreeze(game, x, y, layer);
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

            //Custom variables.
            newBlock.sprite = sprite;
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas);

            return newBlock;
        }

        /// <summary>
        /// If actors are synced, disables them on touch and deletes itself.
        /// </summary>
        public override void Update()
        {
            //Slowly rotates the sprite.
            sprite.angle += 0.05f;

            //If actors are synchronized.
            if (game.mngrLvl.opSyncActors)
            {
                //Gets a list of all actors on the freeze object.
                List<GameObj> items = game.mngrLvl.items.Where(o =>
                    o.x == x && o.y == y && o.layer == layer &&
                    o.type == Type.Actor).ToList();

                //Disables all actors touching the freeze ice.
                foreach (GameObj item in items)
                {
                    item.isEnabled = false;
                    game.mngrLvl.RemoveItem(this);
                    game.playlist.Play(sndFreeze, x, y);
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
            if (Sprite.isIntersecting(sprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                layer == game.mngrLvl.actor.layer)
            {
                game.mngrLvl.tooltip += "Freeze | ";
            }
        }
    }
}