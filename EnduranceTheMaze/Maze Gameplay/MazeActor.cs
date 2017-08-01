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
    /// Actors are the characters which the game is built around. Multiples
    /// are handled separately (or mimic the active one).
    /// 
    /// Dependencies: MngrLvl, MazeBlock, MazeBelt, MazeCrate, MazeLock, MazeCoinLock.
    /// 
    /// Activation types: none
    /// 
    /// Custom properties of custInt1: none
    /// Custom properties of custInt2: none
    /// Custom properties of custStr: none
    /// </summary>
    public class MazeActor : GameObj
    {
        //Relevant assets.
        public static SoundEffect sndLockOpen, sndMoveCrate;
        public static Texture2D texActor { get; private set; }

        //Sprite information.    
        private SpriteAtlas spriteAtlas;

        //Actor-specific variables.
        public int hp; //health points.
        public List<Color> keys; //Keys listed by their color.

        //An update timer for character movement.
        private int _countdownStart, _countdown;
        public bool isTimerZero { get; private set; }

        /// <summary>Sets the block location and default values.</summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public MazeActor(MainLoop game, int x, int y, int layer)
            : base(game, x, y, layer)
        {
            //Sets default values.
            isSolid = true;
            type = Type.Actor;

            //Sets default values for health and key ids.
            hp = 100;
            keys = new List<Color>();

            //Sets the timer defaults.
            _countdown = _countdownStart = 8;
            isTimerZero = false;

            //Sets sprite information.
            sprite = new Sprite(true, texActor);
            sprite.depth = 0.1f;
            sprite.drawBehavior = SpriteDraw.all;
            spriteAtlas = new SpriteAtlas(sprite, 32, 32, 6, 2, 3);

            //Sets the initial position.
            sprite.rectDest.X = x * 32;
            sprite.rectDest.Y = y * 32;
        }

        /// <summary>
        /// Loads relevant graphics into memory.
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void LoadContent(ContentManager Content)
        {
            sndLockOpen = Content.Load<SoundEffect>("Content/Sounds/sndLockOpen");
            sndMoveCrate = Content.Load<SoundEffect>("Content/Sounds/sndMoveCrate");
            texActor = Content.Load<Texture2D>("Content/Sprites/Game/sprActor");
        }

        /// <summary>
        /// Returns an exact copy of the object.
        /// </summary>
        public override GameObj Clone()
        {
            //Sets common variables.
            MazeActor newBlock = new MazeActor(game, x, y, layer);
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
            newBlock._countdown = _countdown;
            newBlock._countdownStart = _countdownStart;
            newBlock.isTimerZero = isTimerZero;
            newBlock.hp = hp;
            newBlock.keys = new List<Color>(keys);

            return newBlock;
        }

        /// <summary>
        /// Responds to movement requests.
        /// </summary>
        public override void Update()
        {
            #region Updates the sprite.
            //Updates the actor sprite by direction.
            //Depends on the texture frames and orientation.
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
            if (this == game.mngrLvl.actor)
            {
                spriteAtlas.frame += 3;
            }
            #endregion

            #region Updates timer.
            _countdown--;
            if (_countdown == 0)
            {
                _countdown = _countdownStart;
                isTimerZero = true;
            }
            else
            {
                isTimerZero = false;
            }
            #endregion

            #region Checks for actor death.
            if (hp <= 0)
            {
                if (game.mngrLvl.opSyncDeath)
                {
                    game.mngrLvl.doRevert = true;
                }
                else
                {
                    game.mngrLvl.RemoveItem(this);
                }
            }
            #endregion

            #region Player movement
            //Captures keyboard commands to move.
            if (isEnabled && (this == game.mngrLvl.actor ||
                game.mngrLvl.opSyncActors))
            {
                //Represents intent and ability to move.
                bool doMove = false, canMove = true;

                //Contains previous direction.
                Dir dirPrev = dir;

                //If dir should be reverted. Almost never set to false.
                bool doRevertDir = true;

                //If the character hasn't met the step limit.
                //Prevents keystroke processing when app is inactive.
                if ((game.mngrLvl.opMaxSteps == 0 ||
                    game.mngrLvl.lvlSteps < game.mngrLvl.opMaxSteps) &&
                    game.IsActive)
                {
                    if ((game.KbState.IsKeyDown(Keys.D) &&
                        game.KbStateOld.IsKeyUp(Keys.D)) ||
                        (game.KbState.IsKeyDown(Keys.Right) &&
                        game.KbStateOld.IsKeyUp(Keys.Right)))
                    {
                        _countdown = _countdownStart;
                        isTimerZero = true;
                    }
                    else if ((game.KbState.IsKeyDown(Keys.S) &&
                        game.KbStateOld.IsKeyUp(Keys.S)) ||
                        (game.KbState.IsKeyDown(Keys.Down) &&
                        game.KbStateOld.IsKeyUp(Keys.Down)))
                    {
                        _countdown = _countdownStart;
                        isTimerZero = true;
                    }
                    else if ((game.KbState.IsKeyDown(Keys.A) &&
                        game.KbStateOld.IsKeyUp(Keys.A)) ||
                        (game.KbState.IsKeyDown(Keys.Left) &&
                        game.KbStateOld.IsKeyUp(Keys.Left)))
                    {
                        _countdown = _countdownStart;
                        isTimerZero = true;
                    }
                    else if ((game.KbState.IsKeyDown(Keys.W) &&
                        game.KbStateOld.IsKeyUp(Keys.W)) ||
                        (game.KbState.IsKeyDown(Keys.Up) &&
                        game.KbStateOld.IsKeyUp(Keys.Up)))
                    {
                        _countdown = _countdownStart;
                        isTimerZero = true;
                    }

                    //Updates the player direction based on movement.
                    //Prevents keystroke processing when app is inactive.
                    if (game.IsActive && isTimerZero)
                    {
                        if (game.KbState.IsKeyDown(Keys.D) ||
                            game.KbState.IsKeyDown(Keys.Right))
                        {
                            dir = Dir.Right;
                            doMove = true;
                        }
                        else if (game.KbState.IsKeyDown(Keys.S) ||
                            game.KbState.IsKeyDown(Keys.Down))
                        {
                            dir = Dir.Down;
                            doMove = true;
                        }
                        else if (game.KbState.IsKeyDown(Keys.A) ||
                            game.KbState.IsKeyDown(Keys.Left))
                        {
                            dir = Dir.Left;
                            doMove = true;
                        }
                        else if (game.KbState.IsKeyDown(Keys.W) ||
                            game.KbState.IsKeyDown(Keys.Up))
                        {
                            dir = Dir.Up;
                            doMove = true;
                        }
                    }

                    if (doMove)
                    {
                        //Considers all scenarios where the player can't move.
                        //Gets a list of all blocks at the location to move.
                        List<GameObj> items = game.mngrLvl.items.Where(o =>
                            o.x == x + Utils.DirVector(dir).X &&
                            o.y == y + Utils.DirVector(dir).Y &&
                            o.layer == layer).ToList();

                        foreach (GameObj item in items)
                        {
                            #region Interaction: MazeBelt.cs
                            //Can't move into opposing belts.
                            if (item.type == Type.Belt &&
                                item.isEnabled &&
                                item.dir == Utils.DirOpp(dir))
                            {
                                canMove = false;
                            }
                            #endregion

                            //Can't move if space is occupied.
                            if (item.isSolid)
                            {
                                #region Interaction: MazeLock.cs
                                if (item.type == Type.Lock)
                                {
                                    if (keys.Count == 0)
                                    {
                                        canMove = false;
                                    }

                                    bool isFound = false;
                                    for (int i = keys.Count - 1; i >= 0; i--)
                                    {
                                        if (keys[i] == item.sprite.color)
                                        {
                                            isFound = true;
                                            game.mngrLvl.RemoveItem(item);
                                            keys.RemoveAt(i);
                                            game.playlist.Play(sndLockOpen, x, y);
                                            break;
                                        }
                                    }

                                    //Prevents walking through locks.
                                    if (!isFound)
                                    {
                                        canMove = false;
                                    }
                                }
                                #endregion

                                #region Interaction: MazeCoinLock.cs
                                if (item.type == Type.CoinLock)
                                {
                                    bool isOpened = false;

                                    //Opens if player has enough coins.
                                    if (game.mngrLvl.actorCoins >= item.custInt1)
                                    {
                                        //Subtracts the coins.
                                        if (item.custInt2 == 1)
                                        {
                                            game.mngrLvl.actorCoins -= item.custInt1;
                                        }

                                        isOpened = true;
                                        game.mngrLvl.RemoveItem(item);
                                        game.playlist.Play(sndLockOpen, x, y);
                                        break;
                                    }

                                    //Prevents walking through locks.
                                    if (!isOpened)
                                    {
                                        canMove = false;
                                    }
                                }
                                #endregion

                                #region Interaction: MazeCrate.cs
                                else if (item.type == Type.Crate)
                                {
                                    #region Interaction: MazeIce.cs
                                    //If the actor is on ice.
                                    if (game.mngrLvl.items.Any(o =>
                                        o.type == Type.Ice &&
                                        o.x == x && o.y == y &&
                                        o.layer == layer))
                                    {
                                        //If on ice without solid objects in
                                        //front, moving crates is not allowed.
                                        //This prevents moving crates to the
                                        //side of the actor while they slide.
                                        if (!game.mngrLvl.items.Any(o =>
                                            o.x == x + Utils.DirVector(dirPrev).X &&
                                            o.y == y + Utils.DirVector(dirPrev).Y &&
                                            o.layer == layer && o.isSolid))
                                        {
                                            canMove = false;
                                            continue;
                                        }
                                    }
                                    #endregion

                                    //Gets a list of all solids ahead of the
                                    //crate in the player's direction.
                                    List<GameObj> itemsFront =
                                        game.mngrLvl.items.Where(o => o.x ==
                                        item.x + Utils.DirVector(dir).X &&
                                        o.y == item.y + Utils.DirVector(dir).Y
                                        && o.layer == layer && o.type !=
                                        Type.CrateHole).ToList();

                                    //Gets a list of all items the crate is on.
                                    List<GameObj> itemsTop =
                                        game.mngrLvl.items.Where(o => o.x ==
                                        item.x && o.y == item.y && o.layer ==
                                        layer && o.isEnabled).ToList();

                                    //Removes crate from affecting itself.
                                    itemsTop.Remove(item);

                                    #region Interaction: MazeBelt.cs
                                    //Can't move crates into opposing belts.
                                    itemsFront = itemsFront.Where(o =>
                                        o.isSolid || (o.type == Type.Belt &&
                                        o.dir == Utils.DirOpp(dir) &&
                                        o.isEnabled)).ToList();

                                    //Can't move crates from belts.
                                    itemsTop = itemsTop.Where(o =>
                                        o.isSolid || (o.type == Type.Belt &&
                                        o.isEnabled && o.dir != dir)).ToList();
                                    #endregion

                                    #region Interaction: MazeMultiWay.cs
                                    itemsFront = itemsFront.Where(o =>
                                        !(o.isEnabled &&
                                        o.type == Type.MultiWay &&
                                        ((o.custInt1 == 0 && o.dir == dir) ||
                                        (o.custInt1 != 0 && (o.dir == dir ||
                                        o.dir == Utils.DirOpp(dir))))))
                                        .ToList();

                                    itemsTop = itemsTop.Where(o =>
                                        !(o.isEnabled &&
                                        o.type == Type.MultiWay &&
                                        ((o.custInt1 == 0 && o.dir == dir) ||
                                        (o.custInt1 != 0 && (o.dir == dir ||
                                        o.dir == Utils.DirOpp(dir))))))
                                        .ToList();
                                    #endregion

                                    //If nothing is in way, moves the crate.
                                    if (itemsFront.Count == 0 &&
                                        itemsTop.Count == 0)
                                    {
                                        #region Interaction: MazeTurretBullet.cs
                                        //Finds all bullets skipped over by moving 32px at a time.
                                        //The +32 at the end accounts for sprite width and height.
                                        float xMin = Math.Min(item.x * 32, (item.x + Utils.DirVector(item.dir).X) * 32);
                                        float xMax = Math.Max(item.x * 32, (item.x + Utils.DirVector(item.dir).X) * 32) + 32;
                                        float yMin = Math.Min(item.y * 32, (item.y + Utils.DirVector(item.dir).Y) * 32);
                                        float yMax = Math.Max(item.y * 32, (item.y + Utils.DirVector(item.dir).Y) * 32) + 32;
                                        List<GameObj> bullets = game.mngrLvl.items.Where(o =>
                                            o.type == Type.TurretBullet &&
                                            o.layer == item.layer &&
                                            o.x >= xMin && o.x <= xMax &&
                                            o.y >= yMin && o.y <= yMax)
                                            .ToList();

                                        for (int i = 0; i < bullets.Count; i++)
                                        {
                                            game.mngrLvl.RemoveItem(bullets[i]);
                                        }
                                        #endregion

                                        //Plays the crate-moving sound.
                                        game.playlist.Play
                                            (sndMoveCrate, x, y);

                                        item.dir = dir; //used for ice.
                                        item.x += (int)Utils.DirVector(dir).X;
                                        item.y += (int)Utils.DirVector(dir).Y;

                                        //Recalculates sprite (no flicker).
                                        item.Update();
                                    }
                                    else
                                    {
                                        canMove = false;
                                    }
                                }
                                #endregion

                                #region Interaction: MazeMultiWay.cs
                                else if (item.type == Type.MultiWay)
                                {
                                    if ((item.custInt1 == 0 &&
                                        item.dir != dir) ||
                                        (item.custInt1 != 0 &&
                                        item.dir != dir &&
                                        item.dir != Utils.DirOpp(dir)) ||
                                        (item.isEnabled == false))
                                    {
                                        canMove = false;
                                    }
                                }
                                else
                                {
                                    canMove = false;
                                }
                                #endregion
                            }
                        }

                        //Considers all scenarios where the player can't move.
                        //Gets a list of all blocks at the current location.
                        items = game.mngrLvl.items.Where(o => o.x == x &&
                            o.y == y && o.layer == layer && o.isEnabled)
                            .ToList();

                        foreach (GameObj item in items)
                        {
                            #region Interaction: MazeBelt.cs
                            //Cannot move while on a belt.
                            if (item.type == Type.Belt && item.isEnabled)
                            {
                                canMove = false;
                                doRevertDir = true;
                            }
                            #endregion

                            #region Interaction: MazeIce.cs
                            else if (item.type == Type.Ice)
                            {
                                //Gets a list of blocks in front.
                                List<GameObj> itemsTemp;
                                itemsTemp = game.mngrLvl.items.Where(o =>
                                    o.x == x + Utils.DirVector(dirPrev).X &&
                                    o.y == y + Utils.DirVector(dirPrev).Y &&
                                    o.layer == layer).ToList();

                                #region Interaction: MazeMultiWay.cs
                                itemsTemp = itemsTemp.Where(o =>
                                    !(o.isEnabled && o.type == Type.MultiWay
                                    && ((o.custInt1 == 0 && o.dir == dirPrev) ||
                                    (o.custInt1 != 0 && (o.dir == dirPrev ||
                                    o.dir == Utils.DirOpp(dirPrev)
                                    ))))).ToList();
                                #endregion
                                #region Interaction: MazeBelt.cs
                                //Blocked by solids and enabled belts.
                                itemsTemp = itemsTemp.Where(o =>
                                    o.isSolid || (o.type == Type.Belt &&
                                    o.dir != Utils.DirOpp(dir))).ToList();
                                #endregion

                                //Can't move unless blocked.
                                if (itemsTemp.Count == 0)
                                {
                                    canMove = false;
                                }
                            }
                            #endregion

                            #region Interaction: MazeMultiWay.cs
                            else if (item.type == Type.MultiWay)
                            {
                                if ((item.custInt1 == 0 && item.dir != dir) ||
                                    (item.custInt1 != 0 && item.dir != dir &&
                                    item.dir != Utils.DirOpp(dir)))
                                {
                                    canMove = false;
                                }
                            }
                            #endregion
                        }
                    }

                    //Moves the player if capable.
                    if (doMove && canMove)
                    {
                        #region Interaction: MazeFilter.cs
                        //Gets a list of all filters the actor is standing on.
                        List<GameObj> items = game.mngrLvl.items.Where(o =>
                            o.x == x && o.y == y && o.layer == layer &&
                            o.type == Type.Filter).ToList();
                        
                        //Decrements each filter's countdown.
                        foreach (GameObj item in items)
                        {
                            if (item.isEnabled && item.custInt1 > 0)
                            {
                                item.custInt1--;
                            }
                        }
                        #endregion

                        #region Interaction: MazeTurretBullet.cs
                        //Finds all bullets skipped over by moving 32px at a time.
                        //The +32 at the end accounts for sprite width and height.
                        float xMin = Math.Min(x * 32, (x + Utils.DirVector(dir).X) * 32);
                        float xMax = Math.Max(x * 32, (x + Utils.DirVector(dir).X) * 32) + 32;
                        float yMin = Math.Min(y * 32, (y + Utils.DirVector(dir).Y) * 32);
                        float yMax = Math.Max(y * 32, (y + Utils.DirVector(dir).Y) * 32) + 32;
                        List<GameObj> bullets = game.mngrLvl.items.Where(o =>
                            o.type == Type.TurretBullet &&
                            o.layer == layer &&
                            o.x >= xMin && o.x <= xMax &&
                            o.y >= yMin && o.y <= yMax)
                            .ToList();

                        List<GameObj> thisActor = new List<GameObj>();
                        thisActor.Add(this);

                        hp -= 25 * bullets.Count;
                        game.playlist.Play(MngrLvl.sndHit, x, y);

                        for (int i = 0; i < bullets.Count; i++)
                        {
                            game.mngrLvl.RemoveItem(bullets[i]);
                        }
                        #endregion

                        x += (int)Utils.DirVector(dir).X;
                        y += (int)Utils.DirVector(dir).Y;

                        //Increments the step counter if applicable.
                        if (this == game.mngrLvl.actor)
                        {
                            game.mngrLvl.lvlSteps++;
                        }
                    }
                    else if (doRevertDir)
                    {
                        dir = dirPrev;
                    }
                }
            }
            #endregion

            #region player clicks to select actor
            if (!game.mngrLvl.opSyncActors && game.IsActive)
            {
                //If the player is clicked.
                if (game.MsState.LeftButton == ButtonState.Pressed &&
                    game.MsStateOld.LeftButton == ButtonState.Released &&
                    Sprite.isIntersecting(sprite, new SmoothRect
                    (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                    layer == game.mngrLvl.actor.layer)
                {
                    if (this != game.mngrLvl.actor)
                    {
                        game.mngrLvl.actor = this;

                        //Interaction: TitleItemMain.cs
                        SfxPlaylist.Play(TitleItemMain.sndBttnClick);
                    }
                }
            }
            #endregion

            spriteAtlas.Update(true);
            base.Update();
        }

        /// <summary>
        /// Draws the actor. When hovered, draws health.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            //Sets the tooltip to display keys / disabled status on hover.
            if (Sprite.isIntersecting(sprite, new SmoothRect
                (game.mngrLvl.GetCoordsMouse(), 1, 1)) &&
                layer == game.mngrLvl.actor.layer)
            {
                game.mngrLvl.tooltip += "actor ";

                if (keys.Count > 0)
                {
                    game.mngrLvl.tooltip += "(Has " + keys.Count + " keys) ";

                    #region Interaction: MazeKey.cs
                    //Draws a key for each collected key.
                    for (int i = 0; i < keys.Count; i++)
                    {
                        game.GameSpriteBatch.Draw(MazeKey.texKey,
                            new Rectangle(x*32 + i*16, y*32 - 16, 16, 16),
                            new Rectangle(0, 0, 32, 32), keys[i]);
                    }
                    #endregion
                }
                if (!isEnabled)
                {
                    game.mngrLvl.tooltip += "(disabled) ";
                }

                game.mngrLvl.tooltip += "| ";
            }
        }
    }
}