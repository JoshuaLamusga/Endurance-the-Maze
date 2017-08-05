using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Activates when clicked.
    /// 
    /// Activation types
    /// 5: Activates linked items on trigger.
    /// 6: Deactivates linked items on trigger.
    /// 7: De/activates linked items each other trigger.
    /// 
    /// Custom properties of custInt1:
    /// 0: Functions normally.
    /// 1: Deletes itself after one use.
    /// 
    /// Custom properties of custInt2:
    /// 0: All activated items are activated regardless of layer.
    /// 1: Only activated items on the same layer are activated.
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeClick : GameObj
    {
        //Relevant assets.
        public static Texture2D TexClick { get; private set; }

        //Custom variables.
        SpriteAtlas spriteAtlas;
        bool isShrinking = true;

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeClick(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            BlockType = Type.Click;

            //Sets sprite information.
            BlockSprite = new Sprite(true, TexClick);
            BlockSprite.depth = 0.201f;
            BlockSprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 2, 1, 2);
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            TexClick = Content.Load<Texture2D>("Content/Sprites/Game/sprClick");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeClick newBlock = new MazeClick(game, X, Y, Layer);
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

            //Custom variables.
            newBlock.BlockSprite = BlockSprite;
            newBlock.spriteAtlas = new SpriteAtlas(spriteAtlas);
            return newBlock;
        }

        /// <summary>
        /// Clicks to activate.
        /// </summary>
        public override void Update()
        {
            #region Adjusts sprite and handles growing/shrinking animation.
            if (IsEnabled)
            {
                spriteAtlas.frame = 0;
                if (isShrinking)
                {
                    BlockSprite.scaleX -= 0.01f;
                    BlockSprite.scaleY -= 0.01f;
                    if (BlockSprite.scaleX <= 0.5f)
                    {
                        isShrinking = false;
                    }
                }
                else
                {
                    spriteAtlas.frame = 0;
                    BlockSprite.scaleX += 0.01f;
                    BlockSprite.scaleY += 0.01f;
                    if (BlockSprite.scaleX >= 1)
                    {
                        isShrinking = true;
                    }
                }
            }
            else
            {
                spriteAtlas.frame = 1;
            }
            #endregion

            if (IsEnabled)
            {
                //The block activates itself when clicked.
                if (Sprite.IsIntersecting(BlockSprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                Layer == game.mngrLvl.actor.Layer)
                {
                    if (game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released)
                    {
                        IsActivated = true;
                    }
                }

                //Handles activation behavior.
                if (IsActivated && ActionType > 4)
                {
                    //Deletes itself if applicable.
                    if (CustInt1 == 1)
                    {
                        game.mngrLvl.RemoveItem(this);
                    }

                    //Gets all items matching the index to affect.
                    List<GameObj> items = game.mngrLvl.items.Where(o =>
                        o.ActionIndex == ActionIndex2).ToList();

                    //Filters out blocks on different layers.
                    if (CustInt2 == 1)
                    {
                        items = items.Where(o => o.Layer == Layer).ToList();
                    }

                    //Deactivates the item and plays sound.
                    IsActivated = false;
                    game.playlist.Play(sndActivated, X, Y);

                    if (ActionType == 5)
                    {
                        foreach (GameObj item in items)
                        {
                            item.IsActivated = true;
                        }
                    }
                    else if (ActionType == 6)
                    {
                        foreach (GameObj item in items)
                        {
                            item.IsActivated = false;
                        }
                    }
                    else if (ActionType == 7)
                    {
                        foreach (GameObj item in items)
                        {
                            item.IsActivated = !item.IsActivated;
                        }
                    }
                }
            }

            spriteAtlas.Update(true);
            base.Update();
        }

        /// <summary>
        /// Draws the sprite. Sets a tooltip.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            //Sets the tooltip to display information on hover.
            if (Sprite.IsIntersecting(BlockSprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                Layer == game.mngrLvl.actor.Layer)
            {
                game.mngrLvl.tooltip += "Clickable";

                if (!IsEnabled)
                {
                    game.mngrLvl.tooltip += "(disabled)";
                }

                game.mngrLvl.tooltip += " | ";
            }
        }
    }
}