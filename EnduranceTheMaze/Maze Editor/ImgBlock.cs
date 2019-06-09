using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
        public Sprite BlockSprite { get; protected set; }
        public SpriteAtlas BlockSpriteAtlas { get; protected set; }

        //Block location.
        public int X { get; internal set; }
        public int Y { get; internal set; }
        public int Layer { get; protected set; }

        //Block identity by type.
        public Type BlockType { get; set; }

        //Block's facing direction.
        public Dir BlockDir { get; internal set; }

        //If the block is enabled or not.
        public bool IsEnabled { get; internal set; }

        //Block activation.
        public int ActionIndex { get; internal set; } //The activation channel.
        public int ActionIndex2 { get; internal set; } //Actuator channels.
        public int ActionType { get; internal set; } //The activation behavior.

        //Custom block properties.
        public int CustInt1 { get; internal set; }
        public int CustInt2 { get; internal set; }
        public string CustStr { get; internal set; }
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
            this.BlockType = type;
            this.X = x;
            this.Y = y;
            this.Layer = layer;

            //Sets default values.
            ActionIndex = 0;
            ActionIndex2 = 0;
            ActionType = 0;
            CustInt1 = 0;
            CustInt2 = 0;
            CustStr = "";
            BlockDir = Dir.Right;
            IsEnabled = true;

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
            if (BlockSpriteAtlas != null)
            {
                BlockSpriteAtlas.Update(true);
            }

            //Handles constant sprite animations.
            if (BlockType == Type.Click && IsEnabled)
            {
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
                    BlockSpriteAtlas.frame = 0;
                    BlockSprite.scaleX += 0.01f;
                    BlockSprite.scaleY += 0.01f;
                    if (BlockSprite.scaleX >= 1)
                    {
                        isShrinking = true;
                    }
                }
            }
            else if (BlockType == Type.Freeze)
            {
                BlockSprite.angle += 0.05f;
            }
            else if (BlockType == Type.Spike)
            {
                BlockSprite.angle += 0.02f;
            }


        }

        /// <summary>
        /// Draws the sprite.
        /// </summary>
        public virtual void Draw()
        {
            BlockSprite.Draw(game.GameSpriteBatch);
        }

        /// <summary>
        /// Adjusts the sprite to match the type, dir, etc. This is called
        /// whenever the sprite is updated.
        /// </summary>
        public void AdjustSprite()
        {
            BlockSprite = new Sprite();

            switch (BlockType)
            {
                case Type.Actor:
                    BlockSprite = new Sprite(true, MazeActor.TexActor);
                    BlockSprite.depth = 0.1f;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 6, 2, 3);
                    #region Chooses sprite by direction.
                    if (BlockDir == Dir.Right)
                    {
                        BlockSpriteAtlas.frame = 0;
                        BlockSprite.spriteEffects = SpriteEffects.None;
                    }
                    else if (BlockDir == Dir.Down)
                    {
                        BlockSpriteAtlas.frame = 1;
                        BlockSprite.spriteEffects = SpriteEffects.None;
                    }
                    else if (BlockDir == Dir.Left)
                    {
                        BlockSpriteAtlas.frame = 0;
                        BlockSprite.spriteEffects = SpriteEffects.FlipHorizontally;
                    }
                    else
                    {
                        BlockSpriteAtlas.frame = 2;
                        BlockSprite.spriteEffects = SpriteEffects.None;
                    }
                    #endregion
                    break;
                case Type.Belt:
                    BlockSprite = new Sprite(true, MazeBelt.TexBelt);
                    BlockSprite.depth = 0.401f;
                    BlockSprite.doDrawOffset = true;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 9, 1, 9);
                    BlockSpriteAtlas.CenterOrigin();
                    #region Chooses sprite by direction and isEnabled.
                    if (BlockDir == Dir.Right)
                    {
                        BlockSprite.angle = 0;
                    }
                    else if (BlockDir == Dir.Down)
                    {
                        BlockSprite.angle = (float)(Math.PI / 2);
                    }
                    else if (BlockDir == Dir.Left)
                    {
                        BlockSprite.angle = (float)(Math.PI);
                    }
                    else
                    {
                        BlockSprite.angle = (float)(-Math.PI / 2);
                    }

                    //Determines the belt's image speed.
                    if (IsEnabled)
                    {
                        BlockSpriteAtlas.frameSpeed = 0.25f;
                    }
                    else
                    {
                        BlockSpriteAtlas.frameSpeed = 0;
                    }
                    #endregion
                    break;
                case Type.Checkpoint:
                    //Sets sprite information.
                    BlockSprite = new Sprite(true, MazeCheckpoint.TexCheckpoint);
                    BlockSprite.depth = 0.208f;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 19, 2, 10);
                    BlockSpriteAtlas.frameSpeed = 0.35f;
                    break;
                case Type.Click:
                    //Sets sprite information.
                    BlockSprite = new Sprite(true, MazeClick.TexClick);
                    BlockSprite.depth = 0.201f;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 2, 1, 2);
                    #region Adjusts sprite and handles growing/shrinking animation.
                    if (IsEnabled)
                    {
                        BlockSpriteAtlas.frame = 0;
                    }
                    else
                    {
                        BlockSpriteAtlas.frame = 1;
                    }
                    #endregion
                    break;
                case Type.Coin:
                    BlockSprite = new Sprite(true, MazeCoin.TexCoin);
                    BlockSprite.depth = 0.205f;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 19, 2, 10);
                    BlockSpriteAtlas.frameSpeed = 0.2f;
                    break;
                case Type.CoinLock:
                    BlockSprite = new Sprite(true, MazeCoinLock.TexCoinLock);
                    BlockSprite.depth = 0.410f;
                    break;
                case Type.Crate:
                    BlockSprite = new Sprite(true, MazeCrate.TexCrate);
                    BlockSprite.depth = 0.3f;
                    BlockSprite.rectSrc = new SmoothRect(0, 0, 32, 32);
                    BlockSprite.rectDest.Width = 32;
                    BlockSprite.rectDest.Height = 32;
                    BlockSprite.drawBehavior = SpriteDraw.basicAnimated;
                    break;
                case Type.CrateHole:
                    BlockSprite = new Sprite(true, MazeCrateHole.TexCrateHole);
                    BlockSprite.depth = 0.403f;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 2, 1, 2);
                    break;
                case Type.EAuto:
                    //Sets sprite information.
                    BlockSprite = new Sprite(true, MazeEAuto.TexEAuto);
                    BlockSprite.depth = 0.417f;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 3, 2, 3);
                    #region Adjusts sprite.
                    if (IsEnabled)
                    {
                        BlockSpriteAtlas.frameSpeed = 0.2f;
                    }
                    else
                    {
                        BlockSpriteAtlas.frameSpeed = 0;
                    }
                    #endregion
                    break;
                case Type.ELight:
                    BlockSprite = new Sprite(true, MazeELight.TexELight);
                    BlockSprite.depth = 0.416f;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 2, 1, 2);
                    BlockSpriteAtlas.frame = 1;
                    break;
                case Type.Enemy:
                    BlockSprite = new Sprite(true, MazeEnemy.TexEnemy);
                    BlockSprite.depth = 0.4f;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 4, 1, 4);
                    #region Chooses sprite by direction and isEnabled.
                    if (Utils.DirCardinal(BlockDir))
                    {
                        if (IsEnabled)
                        {
                            BlockSpriteAtlas.frame = 0;
                        }
                        else
                        {
                            BlockSpriteAtlas.frame = 1;
                        }
                    }
                    else
                    {
                        if (IsEnabled)
                        {
                            BlockSpriteAtlas.frame = 2;
                        }
                        else
                        {
                            BlockSpriteAtlas.frame = 3;
                        }
                    }
                    #endregion
                    break;
                case Type.EPusher:
                    BlockSprite = new Sprite(true, MazeEPusher.TexEPusher);
                    BlockSprite.depth = 0.415f;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 64, 32, 3, 1, 3);
                    BlockSprite.doDrawOffset = true;
                    BlockSprite.origin.X = 16;
                    BlockSprite.origin.Y = 16;
                    #region Adjusts sprite.
                    if (BlockDir == Dir.Right)
                    {
                        BlockSprite.angle = 0;
                    }
                    else if (BlockDir == Dir.Down)
                    {
                        BlockSprite.angle = (float)(Math.PI / 2);
                    }
                    else if (BlockDir == Dir.Left)
                    {
                        BlockSprite.angle = (float)(Math.PI);
                    }
                    else
                    {
                        BlockSprite.angle = (float)(-Math.PI / 2);
                    }         
                    if (IsEnabled)
                    {
                        BlockSpriteAtlas.frame = 0;
                    }
                    else
                    {
                        BlockSpriteAtlas.frame = 2;
                    }
                    #endregion
                    break;
                case Type.Filter:
                    BlockSprite = new Sprite(true, MazeFilter.TexFilter);
                    BlockSprite.depth = 0.405f;
                    BlockSprite.doDrawOffset = true;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 19, 2, 10);
                    BlockSpriteAtlas.CenterOrigin();
                    #region Chooses frame speed by isEnabled.
                    if (IsEnabled)
                    {
                        BlockSpriteAtlas.frameSpeed = 0.35f;
                    }
                    else
                    {
                        BlockSpriteAtlas.frame = 0;
                        BlockSpriteAtlas.frameSpeed = 0;
                    }
                    #endregion
                    break;
                case Type.Finish:
                    BlockSprite = new Sprite(true, MazeFinish.TexFinish);
                    BlockSprite.depth = 0.417f;
                    break;
                case Type.Floor:
                    BlockSprite = new Sprite(true, MazeFloor.TexFloor);
                    BlockSprite.depth = 0.6f;
                    break;
                case Type.Freeze:
                    BlockSprite = new Sprite(true, MazeFreeze.TexFreeze);
                    BlockSprite.depth = 0.203f;
                    BlockSprite.doDrawOffset = true;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 10, 1, 10);
                    BlockSpriteAtlas.frameSpeed = 0.4f;
                    BlockSpriteAtlas.CenterOrigin();
                    break;
                case Type.Gate:
                    BlockSprite = new Sprite(true, MazeGate.TexGate);
                    BlockSprite.depth = 0.102f;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 2, 1, 2);
                    if (CustInt2 == 1)
                    {
                        BlockSpriteAtlas.frame = 1;
                    }
                    break;
                case Type.Goal:
                    BlockSprite = new Sprite(true, MazeGoal.TexGoal);
                    BlockSprite.depth = 0.202f;
                    BlockSprite.doDrawOffset = true;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 9, 1, 9);
                    BlockSpriteAtlas.frameSpeed = 0.2f;
                    BlockSpriteAtlas.CenterOrigin();
                    break;
                case Type.Health:
                    BlockSprite = new Sprite(true, MazeHealth.TexHealth);
                    BlockSprite.depth = 0.206f;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 19, 2, 10);
                    BlockSpriteAtlas.frameSpeed = 0.2f;
                    break;
                case Type.Ice:
                    BlockSprite = new Sprite(true, MazeIce.TexIce);
                    BlockSprite.depth = 0.5f;
                    break;
                case Type.Key:
                    BlockSprite = new Sprite(true, MazeKey.TexKey);
                    BlockSprite.depth = 0.207f;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 19, 2, 10);
                    BlockSpriteAtlas.frameSpeed = 0.2f;
                    #region Chooses key color by custInt1.
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
                    #endregion
                    break;
                case Type.Lock:
                    BlockSprite = new Sprite(true, MazeLock.TexLock);
                    BlockSprite.depth = 0.407f;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    #region Chooses lock color by custInt1.
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
                    #endregion
                    break;
                case Type.Message:
                    BlockSprite = new Sprite(true, MazeMessage.TexMessage);
                    BlockSprite.depth = 0.209f;
                    break;
                case Type.Mirror:
                    BlockSprite = new Sprite(true, MazeMirror.TexMirror);
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSprite.depth = 0.420f;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 4, 1, 4);
                    if (BlockDir == Dir.Right)
                    {
                        BlockSpriteAtlas.frame = 0;
                    }
                    else if (BlockDir == Dir.Down)
                    {
                        BlockSpriteAtlas.frame = 1;
                    }
                    else if (BlockDir == Dir.Left)
                    {
                        BlockSpriteAtlas.frame = 2;
                    }
                    else
                    {
                        BlockSpriteAtlas.frame = 3;
                    }
                    break;
                case Type.MultiWay:
                    BlockSprite = new Sprite(true, MazeMultiWay.TexMultiWay);
                    BlockSprite.depth = 0.408f;
                    BlockSprite.doDrawOffset = true;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 4, 1, 4);
                    BlockSpriteAtlas.CenterOrigin();
                    #region Chooses sprite by direction and frame.
                    //Updates the sprite by direction.
                    if (BlockDir == Dir.Right)
                    {
                        BlockSprite.angle = 0;
                    }
                    else if (BlockDir == Dir.Down)
                    {
                        BlockSprite.angle = (float)(Math.PI / 2);
                    }
                    else if (BlockDir == Dir.Left)
                    {
                        BlockSprite.angle = (float)(Math.PI);
                    }
                    else
                    {
                        BlockSprite.angle = (float)(-Math.PI / 2);
                    }

                    //Determines the frame used.
                    //Dependent on frame order.
                    if (CustInt1 == 0)
                    {
                        BlockSpriteAtlas.frame = 0;
                    }
                    else
                    {
                        BlockSpriteAtlas.frame = 2;
                    }
                    if (!IsEnabled)
                    {
                        BlockSpriteAtlas.frame += 1;
                    }
                    #endregion
                    break;
                case Type.Panel:
                    BlockSprite = new Sprite(true, MazePanel.TexPanel);
                    BlockSprite.depth = 0.414f;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 4, 1, 4);
                    break;
                case Type.Rotate:
                    BlockSprite = new Sprite(true, MazeRotate.TexRotate);
                    BlockSprite.depth = 0.418f;
                    BlockSprite.doDrawOffset = true;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 2, 1, 2);
                    BlockSpriteAtlas.CenterOrigin();
                    #region Adjusts sprite.
                    if (IsEnabled)
                    {
                        BlockSpriteAtlas.frame = 0;
                    }
                    else
                    {
                        BlockSpriteAtlas.frame = 1;
                    }
                    #endregion
                    break;
                case Type.Spawner:
                    BlockSprite = new Sprite(true, MazeSpawner.TexSpawner);
                    BlockSprite.depth = 0.402f;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 8, 2, 4);
                    #region Chooses sprite by direction and isEnabled.
                    if (BlockDir == Dir.Right)
                    {
                        BlockSpriteAtlas.frame = 0;
                    }
                    else if (BlockDir == Dir.Down)
                    {
                        BlockSpriteAtlas.frame = 1;
                    }
                    else if (BlockDir == Dir.Left)
                    {
                        BlockSpriteAtlas.frame = 2;
                    }
                    else
                    {
                        BlockSpriteAtlas.frame = 3;
                    }
                    if (!IsEnabled)
                    {
                        BlockSpriteAtlas.frame += 4;
                    }
                    #endregion
                    break;
                case Type.Spike:
                    BlockSprite = new Sprite(true, MazeSpike.TexSpike);
                    BlockSprite.depth = 0.409f;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSprite.doDrawOffset = true;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 19, 2, 10);
                    BlockSpriteAtlas.frameSpeed = 0.2f;
                    BlockSpriteAtlas.CenterOrigin();
                    break;
                case Type.Stairs:
                    BlockSprite = new Sprite(true, MazeStairs.TexStairs);
                    BlockSprite.depth = 0.406f;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 2, 1, 2);
                    #region Chooses frame by custInt1.
                    //Adjusts the sprite frame.
                    if (CustInt1 == 0)
                    {
                        BlockSpriteAtlas.frame = 0; //up.
                    }
                    else
                    {
                        BlockSpriteAtlas.frame = 1; //down.
                    }
                    #endregion
                    break;
                case Type.Teleporter:
                    BlockSprite = new Sprite(true, MazeTeleporter.TexTeleporter);
                    BlockSprite.depth = 0.412f;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 4, 1, 4);
                    #region Chooses sprite by frame and isEnabled.
                    //Adjusts the sprite frame.
                    if (CustInt1 == 0)
                    {
                        BlockSpriteAtlas.frame = 0; //Sender.
                    }
                    else
                    {
                        BlockSpriteAtlas.frame = 2; //Receiver.
                    }
                    //Depends on frame positions and texture.
                    if (!IsEnabled)
                    {
                        BlockSpriteAtlas.frame++;
                    }
                    #endregion
                    break;
                case Type.Thaw:
                    BlockSprite = new Sprite(true, MazeThaw.TexThaw);
                    BlockSprite.depth = 0.204f;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 13, 1, 13);
                    BlockSpriteAtlas.frameSpeed = 0.25f;
                    break;
                case Type.Turret:
                    BlockSprite = new Sprite(true, MazeTurret.TexTurret);
                    BlockSprite.depth = 0.419f;
                    BlockSprite.drawBehavior = SpriteDraw.all;
                    BlockSpriteAtlas = new SpriteAtlas(BlockSprite, 32, 32, 8, 2, 4);
                    #region Chooses sprite by direction and isEnabled.
                    if (BlockDir == Dir.Right)
                    {
                        BlockSpriteAtlas.frame = 0;
                    }
                    else if (BlockDir == Dir.Down)
                    {
                        BlockSpriteAtlas.frame = 1;
                    }
                    else if (BlockDir == Dir.Left)
                    {
                        BlockSpriteAtlas.frame = 2;
                    }
                    else
                    {
                        BlockSpriteAtlas.frame = 3;
                    }
                    if (!IsEnabled)
                    {
                        BlockSpriteAtlas.frame += 4;
                    }
                    #endregion
                    break;
                case Type.Wall:
                    BlockSprite = new Sprite(true, MazeWall.TexWall);
                    BlockSprite.depth = 0.413f;
                    break;
            }

            //Synchronizes sprite position to location.
            BlockSprite.rectDest.X = X * 32 - 16; //-16 for camera offset.
            BlockSprite.rectDest.Y = Y * 32 - 16; //-16 for camera offset.
            if (BlockSpriteAtlas != null)
            {
                BlockSpriteAtlas.Update(true);
            }
        }
    }
}