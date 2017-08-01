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
    /// A stationary hazard that kills actors on contact.
    /// 
    /// Dependencies: MngrLvl, MazeBlock.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeSpike : GameObj
    {
        //Relevant assets.
        public static Texture2D texSpike { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;     

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeSpike(MainLoop game, int x, int y, int layer) :
            base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Spike;

            //Sets sprite information.
            sprite = new Sprite(true, texSpike);
            sprite.depth = 0.409f;
            sprite.drawBehavior = SpriteDraw.all;
            sprite.originOffset = true;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 19, 2, 10);
            spriteAtlas.frameSpeed = 0.2f;
            spriteAtlas.CenterOrigin();
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            texSpike = Content.Load<Texture2D>("Content/Sprites/Game/sprSpike");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeSpike newBlock =
                new MazeSpike(game, x, y, layer);
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

            //Sets specific variables.
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);
            return newBlock;
        }

        /// <summary>
        /// Updates the atlas and damages actor on contact.
        /// </summary>
        public override void Update()
        {
            //Slowly rotates the sprite.
            sprite.angle += 0.02f;

            //Gets a list of all actor blocks on the spike.
            List<GameObj> items = game.mngrLvl.items.Where(o =>
                o.x == x && o.y == y && o.layer == layer &&
                o.type == Type.Actor).ToList();

            //Destroys all actors touching the spike.
            foreach (GameObj item in items)
            {
                (item as MazeActor).hp = 0;
                game.playlist.Play(MngrLvl.sndHit, x, y); //Depends: MngrLvl.
            }

            spriteAtlas.Update(true);
            base.Update();
        }

        /// <summary>
        /// Draws the enemy. When hovered, draws enabledness/info.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            //Sets the tooltip to display disabled status and info.
            if (Sprite.isIntersecting(sprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                layer == game.mngrLvl.actor.layer)
            {
                game.mngrLvl.tooltip += "Spike";
                if (!isEnabled)
                {
                    game.mngrLvl.tooltip += "(disabled)";
                }

                game.mngrLvl.tooltip += " | ";
            }
        }
    }
}