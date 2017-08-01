using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Visually represents an item, but doesn't simulate behavior.
    /// </summary>
    public class ImgBlock
    {
        //Refers to the game instance.
        protected MainLoop game;

        //Contains a sprite.
        public Sprite sprite { get; protected set; }
        public SpriteAtlas spriteAtlas { get; protected set; }

        //Block location.
        public int x { get; internal set; }
        public int y { get; internal set; }
        public int layer { get; protected set; }

        //Block identity by type.
        public Type type { get; set; }

        //Block's facing direction.
        public Dir dir { get; internal set; }

        //If the block is enabled or not.
        public bool isEnabled { get; internal set; }

        //Block activation.
        public int actionIndex { get; internal set; } //The activation channel.
        public int actionIndex2 { get; internal set; } //Actuator channels.
        public int actionType { get; internal set; } //The activation behavior.

        //Custom block properties.
        public int custInt1 { get; internal set; }
        public int custInt2 { get; internal set; }
        public string custStr { get; internal set; }
        private bool isShrinking;

        /// <summary>
        /// Sets the block's basic values.
        /// </summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public ImgBlock(MainLoop game, Type type, int x, int y, int layer)
        {
            this.game = game;
            this.type = type;
            this.x = x;
            this.y = y;
            this.layer = layer;

            //Sets default values.
            actionIndex = 0;
            actionIndex2 = 0;
            actionType = 0;
            custInt1 = 0;
            custInt2 = 0;
            custStr = "";
            dir = Dir.Right;
            isEnabled = true;

            //Sets up the appearance information.
            AdjustSprite();

            //Sets custom variable info.
            isShrinking = true; //Used to draw MazeClick.
        }

        /// <summary>
        /// Updates the sprite atlas for sprites, esp. animated ones.
        /// </summary>
        public virtual void Update()
        {
            if (spriteAtlas != null)
            {
                spriteAtlas.Update(true);
            }

            //Handles constant sprite animations.
            if (type == Type.Click && isEnabled)
            {
                if (isShrinking)
                {
                    sprite.scaleX -= 0.01f;
                    sprite.scaleY -= 0.01f;
                    if (sprite.scaleX <= 0.5f)
                    {
                        isShrinking = false;
                    }
                }
                else
                {
                    spriteAtlas.frame = 0;
                    sprite.scaleX += 0.01f;
                    sprite.scaleY += 0.01f;
                    if (sprite.scaleX >= 1)
                    {
                        isShrinking = true;
                    }
                }
            }
            else if (type == Type.Freeze)
            {
                sprite.angle += 0.05f;
            }
            else if (type == Type.Spike)
            {
                sprite.angle += 0.02f;
            }


        }

        /// <summary>
        /// Draws the sprite.
        /// </summary>
        public virtual void Draw()
        {
            sprite.Draw(game.GameSpriteBatch);
        }

        /// <summary>
        /// Adjusts the sprite to match the type, dir, etc. This is called
        /// whenever the sprite is updated.
        /// </summary>
        public void AdjustSprite()
        {
            sprite = new Sprite();

            switch (type)
            {
                case Type.Actor:
                    sprite = new Sprite(true, MazeActor.texActor);
                    sprite.depth = 0.1f;
                    sprite.drawBehavior = SpriteDraw.all;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 6, 2, 3);
                    #region Chooses sprite by direction.
                    if (dir == Dir.Right)
                    {
                        spriteAtlas.frame = 0;
                        sprite.spriteEffects = SpriteEffects.None;
                    }
                    else if (dir == Dir.Down)
                    {
                        spriteAtlas.frame = 1;
                        sprite.spriteEffects = SpriteEffects.None;
                    }
                    else if (dir == Dir.Left)
                    {
                        spriteAtlas.frame = 0;
                        sprite.spriteEffects = SpriteEffects.FlipHorizontally;
                    }
                    else
                    {
                        spriteAtlas.frame = 2;
                        sprite.spriteEffects = SpriteEffects.None;
                    }
                    #endregion
                    break;
                case Type.Belt:
                    sprite = new Sprite(true, MazeBelt.texBelt);
                    sprite.depth = 0.401f;
                    sprite.originOffset = true;
                    sprite.drawBehavior = SpriteDraw.all;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 9, 1, 9);
                    spriteAtlas.CenterOrigin();
                    #region Chooses sprite by direction and isEnabled.
                    if (dir == Dir.Right)
                    {
                        sprite.angle = 0;
                    }
                    else if (dir == Dir.Down)
                    {
                        sprite.angle = (float)(Math.PI / 2);
                    }
                    else if (dir == Dir.Left)
                    {
                        sprite.angle = (float)(Math.PI);
                    }
                    else
                    {
                        sprite.angle = (float)(-Math.PI / 2);
                    }

                    //Determines the belt's image speed.
                    if (isEnabled)
                    {
                        spriteAtlas.frameSpeed = 0.25f;
                    }
                    else
                    {
                        spriteAtlas.frameSpeed = 0;
                    }
                    #endregion
                    break;
                case Type.Checkpoint:
                    //Sets sprite information.
                    sprite = new Sprite(true, MazeCheckpoint.texCheckpoint);
                    sprite.depth = 0.208f;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 19, 2, 10);
                    spriteAtlas.frameSpeed = 0.35f;
                    break;
                case Type.Click:
                    //Sets sprite information.
                    sprite = new Sprite(true, MazeClick.texClick);
                    sprite.depth = 0.201f;
                    sprite.drawBehavior = SpriteDraw.all;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 2, 1, 2);
                    #region Adjusts sprite and handles growing/shrinking animation.
                    if (isEnabled)
                    {
                        spriteAtlas.frame = 0;
                    }
                    else
                    {
                        spriteAtlas.frame = 1;
                    }
                    #endregion
                    break;
                case Type.Coin:
                    sprite = new Sprite(true, MazeCoin.texCoin);
                    sprite.depth = 0.205f;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 19, 2, 10);
                    spriteAtlas.frameSpeed = 0.2f;
                    break;
                case Type.CoinLock:
                    sprite = new Sprite(true, MazeCoinLock.texCoinLock);
                    sprite.depth = 0.410f;
                    break;
                case Type.Crate:
                    sprite = new Sprite(true, MazeCrate.texCrate);
                    sprite.depth = 0.3f;
                    sprite.rectSrc = new SmoothRect(0, 0, 32, 32);
                    sprite.rectDest.Width = 32;
                    sprite.rectDest.Height = 32;
                    sprite.drawBehavior = SpriteDraw.basicAnimated;
                    break;
                case Type.CrateHole:
                    sprite = new Sprite(true, MazeCrateHole.texCrateHole);
                    sprite.depth = 0.403f;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 2, 1, 2);
                    break;
                case Type.EAuto:
                    //Sets sprite information.
                    sprite = new Sprite(true, MazeEAuto.texEAuto);
                    sprite.depth = 0.417f;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 3, 2, 3);
                    #region Adjusts sprite.
                    if (isEnabled)
                    {
                        spriteAtlas.frameSpeed = 0.2f;
                    }
                    else
                    {
                        spriteAtlas.frameSpeed = 0;
                    }
                    #endregion
                    break;
                case Type.ELight:
                    sprite = new Sprite(true, MazeELight.texELight);
                    sprite.depth = 0.416f;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 2, 1, 2);
                    spriteAtlas.frame = 1;
                    break;
                case Type.Enemy:
                    sprite = new Sprite(true, MazeEnemy.texEnemy);
                    sprite.depth = 0.4f;
                    sprite.drawBehavior = SpriteDraw.all;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 4, 1, 4);
                    #region Chooses sprite by direction and isEnabled.
                    if (Utils.DirCardinal(dir))
                    {
                        if (isEnabled)
                        {
                            spriteAtlas.frame = 0;
                        }
                        else
                        {
                            spriteAtlas.frame = 1;
                        }
                    }
                    else
                    {
                        if (isEnabled)
                        {
                            spriteAtlas.frame = 2;
                        }
                        else
                        {
                            spriteAtlas.frame = 3;
                        }
                    }
                    #endregion
                    break;
                case Type.EPusher:
                    sprite = new Sprite(true, MazeEPusher.texEPusher);
                    sprite.depth = 0.415f;
                    sprite.drawBehavior = SpriteDraw.all;
                    spriteAtlas = new SpriteAtlas(sprite, 64, 32, 3, 1, 3);
                    sprite.originOffset = true;
                    sprite.origin.X = 16;
                    sprite.origin.Y = 16;
                    #region Adjusts sprite.
                    if (dir == Dir.Right)
                    {
                        sprite.angle = 0;
                    }
                    else if (dir == Dir.Down)
                    {
                        sprite.angle = (float)(Math.PI / 2);
                    }
                    else if (dir == Dir.Left)
                    {
                        sprite.angle = (float)(Math.PI);
                    }
                    else
                    {
                        sprite.angle = (float)(-Math.PI / 2);
                    }         
                    if (isEnabled)
                    {
                        spriteAtlas.frame = 0;
                    }
                    else
                    {
                        spriteAtlas.frame = 2;
                    }
                    #endregion
                    break;
                case Type.Filter:
                    sprite = new Sprite(true, MazeFilter.texFilter);
                    sprite.depth = 0.405f;
                    sprite.originOffset = true;
                    sprite.drawBehavior = SpriteDraw.all;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 19, 2, 10);
                    spriteAtlas.CenterOrigin();
                    #region Chooses frame speed by isEnabled.
                    if (isEnabled)
                    {
                        spriteAtlas.frameSpeed = 0.35f;
                    }
                    else
                    {
                        spriteAtlas.frame = 0;
                        spriteAtlas.frameSpeed = 0;
                    }
                    #endregion
                    break;
                case Type.Finish:
                    sprite = new Sprite(true, MazeFinish.texFinish);
                    sprite.depth = 0.417f;
                    break;
                case Type.Floor:
                    sprite = new Sprite(true, MazeFloor.texFloor);
                    sprite.depth = 0.6f;
                    break;
                case Type.Freeze:
                    sprite = new Sprite(true, MazeFreeze.texFreeze);
                    sprite.depth = 0.203f;
                    sprite.originOffset = true;
                    sprite.drawBehavior = SpriteDraw.all;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 10, 1, 10);
                    spriteAtlas.frameSpeed = 0.4f;
                    spriteAtlas.CenterOrigin();
                    break;
                case Type.Gate:
                    sprite = new Sprite(true, MazeGate.texGate);
                    sprite.depth = 0.102f;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 2, 1, 2);
                    if (custInt2 == 1)
                    {
                        spriteAtlas.frame = 1;
                    }
                    break;
                case Type.Goal:
                    sprite = new Sprite(true, MazeGoal.texGoal);
                    sprite.depth = 0.202f;
                    sprite.originOffset = true;
                    sprite.drawBehavior = SpriteDraw.all;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 9, 1, 9);
                    spriteAtlas.frameSpeed = 0.2f;
                    spriteAtlas.CenterOrigin();
                    break;
                case Type.Health:
                    sprite = new Sprite(true, MazeHealth.texHealth);
                    sprite.depth = 0.206f;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 19, 2, 10);
                    spriteAtlas.frameSpeed = 0.2f;
                    break;
                case Type.Ice:
                    sprite = new Sprite(true, MazeIce.texIce);
                    sprite.depth = 0.5f;
                    break;
                case Type.Key:
                    sprite = new Sprite(true, MazeKey.texKey);
                    sprite.depth = 0.207f;
                    sprite.drawBehavior = SpriteDraw.all;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 19, 2, 10);
                    spriteAtlas.frameSpeed = 0.2f;
                    #region Chooses key color by custInt1.
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
                    #endregion
                    break;
                case Type.Lock:
                    sprite = new Sprite(true, MazeLock.texLock);
                    sprite.depth = 0.407f;
                    sprite.drawBehavior = SpriteDraw.all;
                    #region Chooses lock color by custInt1.
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
                    #endregion
                    break;
                case Type.Message:
                    sprite = new Sprite(true, MazeMessage.texMessage);
                    sprite.depth = 0.209f;
                    break;
                case Type.Mirror:
                    sprite = new Sprite(true, MazeMirror.texMirror);
                    sprite.drawBehavior = SpriteDraw.all;
                    sprite.depth = 0.420f;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 4, 1, 4);
                    if (dir == Dir.Right)
                    {
                        spriteAtlas.frame = 0;
                    }
                    else if (dir == Dir.Down)
                    {
                        spriteAtlas.frame = 1;
                    }
                    else if (dir == Dir.Left)
                    {
                        spriteAtlas.frame = 2;
                    }
                    else
                    {
                        spriteAtlas.frame = 3;
                    }
                    break;
                case Type.MultiWay:
                    sprite = new Sprite(true, MazeMultiWay.texMultiWay);
                    sprite.depth = 0.408f;
                    sprite.originOffset = true;
                    sprite.drawBehavior = SpriteDraw.all;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 4, 1, 4);
                    spriteAtlas.CenterOrigin();
                    #region Chooses sprite by direction and frame.
                    //Updates the sprite by direction.
                    if (dir == Dir.Right)
                    {
                        sprite.angle = 0;
                    }
                    else if (dir == Dir.Down)
                    {
                        sprite.angle = (float)(Math.PI / 2);
                    }
                    else if (dir == Dir.Left)
                    {
                        sprite.angle = (float)(Math.PI);
                    }
                    else
                    {
                        sprite.angle = (float)(-Math.PI / 2);
                    }

                    //Determines the frame used.
                    //Dependent on frame order.
                    if (custInt1 == 0)
                    {
                        spriteAtlas.frame = 0;
                    }
                    else
                    {
                        spriteAtlas.frame = 2;
                    }
                    if (!isEnabled)
                    {
                        spriteAtlas.frame += 1;
                    }
                    #endregion
                    break;
                case Type.Panel:
                    sprite = new Sprite(true, MazePanel.texPanel);
                    sprite.depth = 0.414f;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 4, 1, 4);
                    break;
                case Type.Rotate:
                    sprite = new Sprite(true, MazeRotate.texRotate);
                    sprite.depth = 0.418f;
                    sprite.originOffset = true;
                    sprite.drawBehavior = SpriteDraw.all;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 2, 1, 2);
                    spriteAtlas.CenterOrigin();
                    #region Adjusts sprite.
                    if (isEnabled)
                    {
                        spriteAtlas.frame = 0;
                    }
                    else
                    {
                        spriteAtlas.frame = 1;
                    }
                    #endregion
                    break;
                case Type.Spawner:
                    sprite = new Sprite(true, MazeSpawner.texSpawner);
                    sprite.depth = 0.402f;
                    sprite.drawBehavior = SpriteDraw.all;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 8, 2, 4);
                    #region Chooses sprite by direction and isEnabled.
                    if (dir == Dir.Right)
                    {
                        spriteAtlas.frame = 0;
                    }
                    else if (dir == Dir.Down)
                    {
                        spriteAtlas.frame = 1;
                    }
                    else if (dir == Dir.Left)
                    {
                        spriteAtlas.frame = 2;
                    }
                    else
                    {
                        spriteAtlas.frame = 3;
                    }
                    if (!isEnabled)
                    {
                        spriteAtlas.frame += 4;
                    }
                    #endregion
                    break;
                case Type.Spike:
                    sprite = new Sprite(true, MazeSpike.texSpike);
                    sprite.depth = 0.409f;
                    sprite.drawBehavior = SpriteDraw.all;
                    sprite.originOffset = true;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 19, 2, 10);
                    spriteAtlas.frameSpeed = 0.2f;
                    spriteAtlas.CenterOrigin();
                    break;
                case Type.Stairs:
                    sprite = new Sprite(true, MazeStairs.texStairs);
                    sprite.depth = 0.406f;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 2, 1, 2);
                    #region Chooses frame by custInt1.
                    //Adjusts the sprite frame.
                    if (custInt1 == 0)
                    {
                        spriteAtlas.frame = 0; //up.
                    }
                    else
                    {
                        spriteAtlas.frame = 1; //down.
                    }
                    #endregion
                    break;
                case Type.Teleporter:
                    sprite = new Sprite(true, MazeTeleporter.texTeleporter);
                    sprite.depth = 0.412f;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 4, 1, 4);
                    #region Chooses sprite by frame and isEnabled.
                    //Adjusts the sprite frame.
                    if (custInt1 == 0)
                    {
                        spriteAtlas.frame = 0; //Sender.
                    }
                    else
                    {
                        spriteAtlas.frame = 2; //Receiver.
                    }
                    //Depends on frame positions and texture.
                    if (!isEnabled)
                    {
                        spriteAtlas.frame++;
                    }
                    #endregion
                    break;
                case Type.Thaw:
                    sprite = new Sprite(true, MazeThaw.texThaw);
                    sprite.depth = 0.204f;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 13, 1, 13);
                    spriteAtlas.frameSpeed = 0.25f;
                    break;
                case Type.Turret:
                    sprite = new Sprite(true, MazeTurret.texTurret);
                    sprite.depth = 0.419f;
                    sprite.drawBehavior = SpriteDraw.all;
                    spriteAtlas = new SpriteAtlas(sprite, 32, 32, 8, 2, 4);
                    #region Chooses sprite by direction and isEnabled.
                    if (dir == Dir.Right)
                    {
                        spriteAtlas.frame = 0;
                    }
                    else if (dir == Dir.Down)
                    {
                        spriteAtlas.frame = 1;
                    }
                    else if (dir == Dir.Left)
                    {
                        spriteAtlas.frame = 2;
                    }
                    else
                    {
                        spriteAtlas.frame = 3;
                    }
                    if (!isEnabled)
                    {
                        spriteAtlas.frame += 4;
                    }
                    #endregion
                    break;
                case Type.Wall:
                    sprite = new Sprite(true, MazeWall.texWall);
                    sprite.depth = 0.413f;
                    break;
            }

            //Synchronizes sprite position to location.
            sprite.rectDest.X = x * 32 - 16; //-16 for camera offset.
            sprite.rectDest.Y = y * 32 - 16; //-16 for camera offset.
            if (spriteAtlas != null)
            {
                spriteAtlas.Update(true);
            }
        }
    }
}