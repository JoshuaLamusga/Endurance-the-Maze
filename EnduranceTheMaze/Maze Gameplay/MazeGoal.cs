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
    /// A collectible goal object. They are sometimes required to win.
    /// 
    /// Activation types: none.
    /// 
    /// Custom properties of custInt1: none.
    /// Custom properties of custInt2: none.
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeGoal : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndCollectGoal;
        public static Texture2D texGoal { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeGoal(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            type = Type.Goal;

            //Sets sprite information.
            sprite = new Sprite(true, texGoal);
            sprite.depth = 0.202f;
            sprite.originOffset = true;
            sprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 9, 1, 9);
            spriteAtlas.frameSpeed = 0.2f;
            spriteAtlas.CenterOrigin();
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndCollectGoal = Content.Load<SoundEffect>("Content/Sounds/sndCollectGoal");
            texGoal = Content.Load<Texture2D>("Content/Sprites/Game/sprGoal");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeGoal newBlock = new MazeGoal(game, x, y, layer);
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

            //Custom variables.
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas, false);

            return newBlock;
        }

        /// <summary>
        /// Adds to number of goals on contact.
        /// </summary>
        public override void Update()
        {
            //Gets a list of all actors on the goal object.
            List<GameObj> items = game.mngrLvl.items.Where(o =>
                o.x == x && o.y == y && o.layer == layer &&
                o.type == Type.Actor).ToList();

            //If there is at least one actor touching the goal.
            if (items.Count != 0)
            {
                game.mngrLvl.actorGoals++;
                game.mngrLvl.RemoveItem(this);
                game.playlist.Play(sndCollectGoal, x, y);
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
                game.mngrLvl.tooltip += "Goal | ";
            }
        }
    }
}