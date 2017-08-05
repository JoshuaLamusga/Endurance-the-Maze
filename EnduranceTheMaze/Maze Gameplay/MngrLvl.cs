using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace EnduranceTheMaze
{
    /// <summary>
    /// The level manager. Handles loading/saving levels and all gameplay
    /// logic.
    /// 
    /// Dependencies: MainLoop.cs, maze block textures.
    /// </summary>
    public class MngrLvl
    {
        //Refers to the game instance.
        private MainLoop game;

        //Relevant assets.
        public static SoundEffect sndHit, sndFinish, sndWin, sndCheckpoint;

        public static Texture2D TexPixel { get; private set; }
        public static Texture2D TexMenuHud { get; private set; }

        //HUD assets (sprites and text).
        private Sprite sprHudOverlay, sprMenuHud;
        public string tooltip = "";

        /* Descriptions:
         * 1. If true, any death counts as player death.
         * 2. If true, all actors copy player movements.
         * 3. Max number of steps the player can take (0 = no effect).
         * 4. The minimum amount of goals to be eligible to win.
         * 5. The level to link to and load upon victory.
         */
        public bool opSyncDeath, opSyncActors;
        private int _lvlMaxSteps, _lvlSteps, _lvlStepsChkpt;
        private int _opReqGoals;
        public int OpMaxSteps
        {
            set
            {
                if (value >= 0)
                {
                    _lvlMaxSteps = value;
                }
            }

            get
            {
                return _lvlMaxSteps;
            }
        }
        public int LvlSteps
        {
            set
            {
                if (value >= 0)
                {
                    _lvlSteps = value;
                }
            }

            get
            {
                return _lvlSteps;
            }
        }
        public int OpReqGoals
        {
            set
            {
                if (value >= 0)
                {
                    _opReqGoals = value;
                }
            }

            get
            {
                return _opReqGoals;
            }
        }
        internal string opLvlLink;

        //Whether the level must be restarted/reverted or not.
        public bool doRestart, doRevert, doCheckpoint, doWin;

        //Whether a message is being shown or not.
        public bool isMessageShown = false;
        private bool isPaused = false;

        //The message to display.
        public string message = "";

        //Contains all maze blocks in the level, organized by original, last
        //checkpoint, and current.
        public List<GameObj> ItemsOrig { get; private set; }
        public List<GameObj> itemsChkpt;
        public List<GameObj> items;

        public MazeActor actor; //active player.
        private int _actorCoins, _actorGoals; //total coins and goals.
        private int actorCoinsChkpt, actorGoalsChkpt;
        public int ActorCoins
        {
            set
            {
                if (value >= 0)
                {
                    _actorCoins = value;
                }
            }

            get
            {
                return _actorCoins;
            }
        }
        public int ActorGoals
        {
            set
            {
                if (value >= 0)
                {
                    _actorGoals = value;
                }
            }

            get
            {
                return _actorGoals;
            }
        }

        //Controls the position of the screen.
        public Matrix Camera { get; private set; }
        private float camZoom;

        //An update timer for objects to utilize as needed.
        internal int _countdownStart, _countdown;
        public bool IsTimerZero { get; private set; }

        /// <summary>
        /// Sets the game instance and default level options.
        /// </summary>
        /// <param name="game">The game instance to use.</param>
        public MngrLvl(MainLoop game)
        {
            this.game = game;            

            //Sets default level option values.
            opSyncDeath = false;
            opSyncActors = false;
            OpMaxSteps = 0;
            LvlSteps = _lvlStepsChkpt = 0;
            OpReqGoals = 0;
            opLvlLink = "";

            //Sets default level variables.
            ActorCoins = ActorGoals = 0;
            actorCoinsChkpt = actorGoalsChkpt = 0;

            //Sets triggers to false.
            doRestart = doRevert = doCheckpoint = doWin = false;

            //Sets the timer defaults.
            _countdown = _countdownStart = 8;
            IsTimerZero = false;

            //Controls the position of the screen (zoom).
            camZoom = 1;

            //Initializes the item lists.
            ItemsOrig = new List<GameObj>();
            itemsChkpt = new List<GameObj>();
            items = new List<GameObj>();
        }

        ///<summary>
        ///Loads relevant graphics into memory.
        ///
        /// Dependencies: MainLoop.cs, maze block textures.
        /// </summary>
        public void LoadContent(ContentManager Content)
        {
            //Loads relevant assets.
            sndCheckpoint = Content.Load<SoundEffect>("Content/Sounds/sndCheckpoint");
            sndFinish = Content.Load<SoundEffect>("Content/Sounds/sndFinish");
            sndHit = Content.Load<SoundEffect>("Content/Sounds/sndHit");
            sndWin = Content.Load<SoundEffect>("Content/Sounds/sndWin");
            TexPixel = new Texture2D(game.GraphicsDevice, 1, 1);
            TexPixel.SetData(new Color[] { Color.White });
            TexMenuHud = game.Content.Load<Texture2D>("Content/Sprites/Gui/sprMenuHud");

            //Sets up hud sprites.
            sprHudOverlay = new Sprite(true, TexPixel);
            sprHudOverlay.color = Color.Gray;
            sprHudOverlay.alpha = 0.5f;
            sprHudOverlay.rectDest = new SmoothRect
                (0, game.GetScreenSize().Y - 32, game.GetScreenSize().X, 32);

            sprMenuHud = new Sprite(true, TexMenuHud);
            sprMenuHud.rectDest = new SmoothRect
                (0, game.GetScreenSize().Y - 32, 64, 32);
            
            //Loads all maze block textures.
            GameObj._LoadContent(game.Content); //base class.
            MazeActor.LoadContent(game.Content);            
            MazeBelt.LoadContent(game.Content);
            MazeCoin.LoadContent(game.Content);
            MazeCoinLock.LoadContent(game.Content);
            MazeCrate.LoadContent(game.Content);
            MazeCrateHole.LoadContent(game.Content);
            MazeEnemy.LoadContent(game.Content);
            MazeFilter.LoadContent(game.Content);
            MazeFloor.LoadContent(game.Content);
            MazeFreeze.LoadContent(game.Content);
            MazeGate.LoadContent(game.Content);
            MazeHealth.LoadContent(game.Content);
            MazeIce.LoadContent(game.Content);
            MazeKey.LoadContent(game.Content);
            MazeLock.LoadContent(game.Content);
            MazeMultiWay.LoadContent(game.Content);
            MazePanel.LoadContent(game.Content);
            MazeSpawner.LoadContent(game.Content);
            MazeSpike.LoadContent(game.Content);
            MazeStairs.LoadContent(game.Content);
            MazeTeleporter.LoadContent(game.Content);
            MazeThaw.LoadContent(game.Content);
            MazeWall.LoadContent(game.Content);
            MazeCheckpoint.LoadContent(game.Content);
            MazeEPusher.LoadContent(game.Content);
            MazeELight.LoadContent(game.Content);
            MazeEAuto.LoadContent(game.Content);
            MazeGoal.LoadContent(game.Content);
            MazeFinish.LoadContent(game.Content);
            MazeMessage.LoadContent(game.Content);
            MazeClick.LoadContent(game.Content);
            MazeRotate.LoadContent(game.Content);
            MazeTurret.LoadContent(game.Content);
            MazeTurretBullet.LoadContent(game.Content);
            MazeMirror.LoadContent(game.Content);
        }

        /// <summary>
        /// Loads a level from a list of blocks and clears old info.
        /// </summary>
        public void LevelStart(List<GameObj> blocks)
        {
            //Sets the new item list.
            items.Clear();
            foreach (GameObj block in blocks)
            {
                items.Add(block.Clone());
            }

            //Resets the item lists.
            ItemsOrig.Clear();
            itemsChkpt.Clear();
            ActorCoins = 0;
            ActorGoals = 0;
            LvlSteps = 0;
            actorCoinsChkpt = 0;
            actorGoalsChkpt = 0;
            _lvlStepsChkpt = 0;

            //Sets the active actor.
            foreach (GameObj item in items)
            {
                //Selects a default actor.
                if (item.BlockType == Type.Actor)
                {
                    actor = (MazeActor)item;
                }
            }

            //Sets up the original and checkpoint lists.
            foreach (GameObj item in items)
            {
                ItemsOrig.Add(item.Clone());
                itemsChkpt.Add(item.Clone());
            }

            //Clears any paused status.
            isPaused = false;
        }

        /// <summary>
        /// Reverts a level to clear old info.
        /// </summary>
        public void LevelRevert()
        {
            //Duplicates each item.
            items.Clear();
            foreach (GameObj block in itemsChkpt)
            {
                items.Add(block.Clone());
            }

            //Resets coins/goals/steps.
            ActorCoins = actorCoinsChkpt;
            ActorGoals = actorGoalsChkpt;
            LvlSteps = _lvlStepsChkpt;

            //Sets the active actor.
            foreach (GameObj item in items)
            {
                //Selects a default actor.
                if (item.BlockType == Type.Actor)
                {
                    actor = (MazeActor)item;
                }
            }
        }

        /// <summary>
        /// Loads a level from an embedded resource to play.
        /// </summary>
        /// <param name="path">The level path and name.</param>
        public void LoadResource(string path)
        {
            Stream stream = GetType().Assembly
                .GetManifestResourceStream(path);

            LoadPlay(stream);
        }

        /// <summary>
        /// Starts playing a level directly from a URL.
        /// </summary>
        /// <param name="path"></param>
        public void LoadPlay(string path)
        {
            if (File.Exists(path))
            {
                //Creates a stream object to the file.
                Stream stream = File.OpenRead(path);

                //Loads and plays the level from the stream.
                LoadPlay(stream);
            }
        }

        /// <summary>
        /// Loads a level from a file with a given path.
        /// Works like MngrEditor.LoadEdit, but does not make itself
        /// available for editing.
        /// </summary>
        /// <param name="path">The path and filename.</param>
        public void LoadPlay(Stream stream)
        {
            //Opens a text reader on the file.
            TextReader txtRead = new StreamReader(stream);

            //Gets all block items with each entry as a block.
            //Includes level settings.
            List<string> strItems =
                txtRead.ReadToEnd().Split('|').ToList();

            //Closes the text reader.
            txtRead.Close();

            //Creates a temporary item list.
            List<GameObj> itemsTemp = new List<GameObj>();

            for (int i = 0; i < strItems.Count; i++)
            {
                //Gets all parts of the string separately.
                List<string> strBlock =
                    strItems[i].Split(',').ToList();

                //If there is content.
                if (strBlock.Count != 0)
                {
                    //If the current object is level settings.
                    if (strBlock[0] == "ops")
                    {
                        //The format must have 7 parts.
                        if (strBlock.Count != 7)
                        {
                            continue;
                        }

                        Int32.TryParse(strBlock[1], out _countdownStart);
                        opLvlLink = strBlock[2];
                        Int32.TryParse(strBlock[3], out _lvlMaxSteps);
                        Int32.TryParse(strBlock[4], out _opReqGoals);
                        Boolean.TryParse(strBlock[5], out opSyncActors);
                        Boolean.TryParse(strBlock[6], out opSyncDeath);
                    }

                    //If the current object is a block.
                    else if (strBlock[0] == "blk")
                    {
                        //The format must have 13 parts.
                        if (strBlock.Count != 13)
                        {
                            continue;
                        }

                        //Sets up value containers.
                        GameObj tempBlock;
                        Type tempType;
                        int tempX, tempY, tempLayer;
                        int tempAInd, tempAInd2, tempAType;
                        int tempInt1, tempInt2;
                        bool tempEnabled;

                        //Gets all values.
                        Enum.TryParse(strBlock[1], out tempType);
                        Int32.TryParse(strBlock[2], out tempX);
                        Int32.TryParse(strBlock[3], out tempY);
                        Int32.TryParse(strBlock[4], out tempLayer);
                        Int32.TryParse(strBlock[5], out tempAInd);
                        Int32.TryParse(strBlock[6], out tempAInd2);
                        Int32.TryParse(strBlock[7], out tempAType);
                        Int32.TryParse(strBlock[8], out tempInt1);
                        Int32.TryParse(strBlock[9], out tempInt2);
                        Boolean.TryParse(strBlock[11],
                            out tempEnabled);

                        //Creates and adds the block with the values.
                        tempBlock = Utils.BlockFromType(game, tempType,
                            tempX, tempY, tempLayer);                                
                        tempBlock.ActionIndex = tempAInd;
                        tempBlock.ActionIndex2 = tempAInd2;
                        tempBlock.ActionType = tempAType;
                        tempBlock.CustInt1 = tempInt1;
                        tempBlock.CustInt2 = tempInt2;
                        tempBlock.BlockDir = (Dir)Enum.Parse(typeof(Dir),
                            strBlock[10]);
                        tempBlock.IsEnabled = tempEnabled;
                        tempBlock.CustStr =
                            strBlock[12].Replace("\t", ",");
                        itemsTemp.Add(tempBlock);
                    }
                }
            }

            //Preps the level for gameplay.
            LevelStart(itemsTemp);

            //Closes resources.
            stream.Close();
        }

        /// <summary>
        /// Returns the given coordinates converted from view space to
        /// world space, to work with the camera matrix.
        /// </summary>
        public Vector2 GetCoords(float x, float y)
        {
            return Vector2.Transform
                (new Vector2(x, y), Matrix.Invert(Camera));
        }
        
        /// <summary>
        /// Returns the mouse coordinates converted from view space to
        /// world space, to work with the camera matrix.
        /// </summary>
        public Vector2 GetCoordsMouse()
        {
            return Vector2.Transform(new Vector2(game.MsState.X,
                game.MsState.Y), Matrix.Invert(Camera));
        }

        /// <summary>
        /// Equivalent of items.Add(), but also works in loops.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(GameObj item)
        {
            List<GameObj> newItems = new List<GameObj>(items);
            newItems.Add(item);
            items = newItems;
        }

        /// <summary>
        /// Equivalent of items.Remove(), but also works in loops.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem(GameObj item)
        {
            List<GameObj> newItems = new List<GameObj>();

            //Creates a shallow copy of the original list by copying all
            //objects except for the specified one.
            foreach (GameObj currentItem in items)
            {
                if (item != currentItem)
                {
                    newItems.Add(currentItem);
                }                
            }

            items = newItems;
        }

        /// <summary>
        /// Updates all block logic.
        /// </summary>
        public void Update()
        {
            //Enables pausing the game.
            if (game.KbState.IsKeyDown(Keys.Space))
            {
                isPaused = true;
            }

            //Enables basic zooming.
            if (game.MsState.ScrollWheelValue >
                game.MsStateOld.ScrollWheelValue)
            {
                if (camZoom >= 2)
                {
                    camZoom = 2;
                }
                else
                {
                    //Works around inherent floating point error.
                    camZoom = ((int)Math.Round(10 * (camZoom + 0.1))) / 10f;
                }
            }
            else if (game.MsState.ScrollWheelValue <
                game.MsStateOld.ScrollWheelValue)
            {                
                if (camZoom <= 0.5)
                {
                    camZoom = 0.5f;
                }
                else
                {
                    //Works around inherent floating point error.
                    camZoom = ((int)Math.Round(10 * (camZoom - 0.1))) / 10f;
                }
            }

            //Resets the drawn tooltip.            
            if (OpMaxSteps == 0)
            {
                tooltip = "";
            }
            else
            {
                tooltip = "steps: " + LvlSteps + " / " +
                    OpMaxSteps + " | ";
            }
            if (ActorGoals > 0 || OpReqGoals > 0)
            {
                tooltip += "goals: " + ActorGoals + " / " +
                    OpReqGoals + " | ";
            }

            //Does not update the game while a message is displayed.
            if (isPaused || isMessageShown)
            {
                if (game.KbState.IsKeyDown(Keys.Enter))
                {
                    isPaused = false;
                    isMessageShown = false;
                }

                return;
            }

            //Updates the timer.
            _countdown--;
            if (_countdown == 0)
            {
                _countdown = _countdownStart;
                IsTimerZero = true;
            }
            else
            {
                IsTimerZero = false;
            }

            #region Handles MazeTurretBullet triggering
            //Gets a list of all bullets.
            List<GameObj> itemsTemp0 = items
                .Where(o => o.BlockType == Type.TurretBullet)
                .ToList();

            foreach (GameObj item in itemsTemp0)
            {
                //Moves the bullet.
                item.X += ((int)Utils.DirVector(item.BlockDir).X * item.CustInt2);
                item.Y += ((int)Utils.DirVector(item.BlockDir).Y * item.CustInt2);
                
                //Gets a list of all solids in front of the bullet.
                List<GameObj> itemsFront = items.Where(obj =>
                    Math.Abs((obj.X * 32 + 16) - ((item.X + item.CustInt2))) < 7 && //TODO: 4 or custInt2?
                    Math.Abs((obj.Y * 32 + 16) - ((item.Y + item.CustInt2))) < 7 &&
                    obj.Layer == item.Layer && obj.IsSolid).ToList();

                //Damages all actors it hits.
                foreach (GameObj item2 in itemsFront)
                {
                    if (item2.BlockType == Type.Actor)
                    {
                        (item2 as MazeActor).hp -= 25;
                        game.playlist.Play(sndHit, item.X, item.Y);
                    }

                    #region Interaction: MazeMultiWay
                    //If the multiway is in the direction of the bullet.
                    if (item2.BlockType == Type.MultiWay &&
                        (item.BlockDir == item2.BlockDir ||
                        item2.IsEnabled == false ||
                        (item2.CustInt1 == 1 &&
                        item.BlockDir == Utils.DirOpp(item2.BlockDir))))
                    {
                        continue;
                    }
                    #endregion

                    #region Interaction: MazeMirror
                    //The mirrors bend or absorb the bullets.
                    if (item2.BlockType == Type.Mirror)
                    {
                        //Bullet is coming in the opposite direction of the mirror.
                        if (item.BlockDir == Utils.DirOpp(item2.BlockDir))
                        {
                            if (!(item as MazeTurretBullet).mirrors.Contains(item2))
                            {
                                (item as MazeTurretBullet).mirrors.Add(item2);
                                item.BlockDir = Utils.DirPrev(item2.BlockDir);
                            }

                            continue;
                        }
                        //Bullet is coming in opposite to the other direction.
                        else if (item.BlockDir == Utils.DirNext(item2.BlockDir))
                        {
                            if (!(item as MazeTurretBullet).mirrors.Contains(item2))
                            {
                                (item as MazeTurretBullet).mirrors.Add(item2);
                                item.BlockDir = item2.BlockDir;
                            }

                            continue;
                        }
                        else if ((item as MazeTurretBullet).mirrors.Contains(item2))
                        {
                            continue;
                        }
                    }

                    RemoveItem(item);
                    #endregion
                }
            }
            #endregion

            //Handles the behavior of blocks when the timer is zero.
            if (IsTimerZero)
            {
                #region Handles MazeBelt triggering
                List<GameObj> itemsTemp, itemsTop, itemsFront;
                //Gets a list of all belt blocks.
                itemsTemp = items.Where(o =>
                    o.BlockType == Type.Belt && o.IsEnabled).ToList();

                //Tracks blocks that get moved and direction.
                //Moves all blocks in sync to avoid getting moved
                //multiple times in one update.
                List<GameObj> queueItems = new List<GameObj>();
                List<Vector2> queuePos = new List<Vector2>();

                foreach (GameObj belt in itemsTemp)
                {
                    //Gets a list of all objects on the belt.
                    itemsTop = items.Where(o =>
                        o.X == belt.X && o.Y == belt.Y &&
                        o.Layer == belt.Layer &&
                        o.BlockSprite.depth < belt.BlockSprite.depth).ToList();
                    itemsTop.Remove(belt); //Removes belt from list.

                    //If there are blocks on the belt.
                    if (itemsTop.Count != 0)
                    {
                        //Gets a list of all solids in front of the belt.
                        itemsFront = items.Where(o =>
                            o.X == belt.X + Utils.DirVector(belt.BlockDir).X &&
                            o.Y == belt.Y + Utils.DirVector(belt.BlockDir).Y &&
                            o.Layer == belt.Layer && o.IsSolid).ToList();

                        #region Interaction: MazeMultiWay.cs
                        itemsFront = itemsFront.Where(o =>
                            !(o.BlockType == Type.MultiWay && o.IsEnabled &&
                            ((o.CustInt1 == 0 && o.BlockDir == belt.BlockDir) ||
                            (o.CustInt1 != 0 && (o.BlockDir == belt.BlockDir ||
                            o.BlockDir == Utils.DirOpp(belt.BlockDir)))))).ToList();
                        #endregion

                        //If nothing is blocking the belt.
                        if (itemsFront.Count == 0)
                        {
                            //Moves the items on the belt over.
                            foreach (GameObj itemTop in itemsTop)
                            {
                                //Adds to queues to update in sync.
                                queueItems.Add(itemTop);
                                queuePos.Add(new Vector2(
                                    Utils.DirVector(belt.BlockDir).X,
                                    Utils.DirVector(belt.BlockDir).Y));
                            }
                        }
                    }
                }
                #endregion
                #region Handles MazeEnemy triggering
                //Gets a list of all enabled enemies.
                itemsTemp = items.Where(o => o.IsEnabled &&
                    o.BlockType == Type.Enemy).ToList();

                foreach (GameObj item in itemsTemp)
                {
                    //Gets a list of all solids in front of the enemy.
                    itemsFront = items.Where(o =>
                        o.X == item.X + Utils.DirVector(item.BlockDir).X &&
                        o.Y == item.Y + Utils.DirVector(item.BlockDir).Y &&
                        o.Layer == item.Layer && o.IsSolid).ToList();

                    #region Interaction: MazeMultiWay.cs
                    itemsFront = itemsFront.Where(o =>
                        !(o.BlockType == Type.MultiWay && o.IsEnabled &&
                        ((o.CustInt1 == 0 && o.BlockDir == item.BlockDir) ||
                        (o.CustInt1 != 0 && (o.BlockDir == item.BlockDir ||
                        o.BlockDir == Utils.DirOpp(item.BlockDir)))))).ToList();
                    #endregion

                    //Moves the enemy if there are no solids, otherwise
                    //bounces off the solid (damaging it if it's an actor).
                    if (itemsFront.Count == 0)
                    {
                        item.X += (int)Utils.DirVector(item.BlockDir).X;
                        item.Y += (int)Utils.DirVector(item.BlockDir).Y;
                    }
                    else
                    {
                        item.BlockDir = Utils.DirOpp(item.BlockDir);
                        
                        //Damages all actors it bounces off of.
                        foreach (GameObj item2 in itemsFront)
                        {
                            if (item2.BlockType == Type.Actor)
                            {
                                (item2 as MazeActor).hp -= 25;
                                game.playlist.Play(sndHit, item.X, item.Y);
                            }
                        }
                    }
                }
                #endregion
                #region Handles MazeIce triggering

                //Gets a list of all ice blocks.
                itemsTemp = items.Where(o => o.BlockType == Type.Ice).ToList();

                foreach (GameObj ice in itemsTemp)
                {
                    //Gets a list of actors/enemies/crates/belts in location.
                    itemsTop = items.Where(o => o.X == ice.X &&
                        o.Y == ice.Y && o.Layer == ice.Layer &&
                        (o.BlockType == Type.Actor || o.BlockType == Type.Enemy ||
                        o.BlockType == Type.Crate || o.BlockType == Type.Belt))
                        .ToList();

                    //if there are no belts on the ice.
                    if (itemsTop.Where(o => o.BlockType == Type.Belt
                        && o.IsEnabled).Count() == 0)
                    {
                        foreach (GameObj block in itemsTop)
                        {
                            //Gets a list of blocks in front of the block.
                            itemsFront = items.Where(o =>
                                o.X == (int)block.X +
                                    Utils.DirVector(block.BlockDir).X &&
                                o.Y == (int)block.Y +
                                    Utils.DirVector(block.BlockDir).Y &&
                                o.Layer == block.Layer).ToList();

                            #region Interaction: MazeCrate.cs
                            if (block.BlockType == Type.Crate)
                            {
                                itemsFront = itemsFront.Where(o =>
                                    o.BlockType != Type.CrateHole).ToList();
                            }
                            #endregion
                            #region Interaction: MazeMultiWay.cs
                            itemsFront = itemsFront.Where(o =>
                                !(o.BlockType == Type.MultiWay && o.IsEnabled &&
                                ((o.CustInt1 == 0 && o.BlockDir == block.BlockDir) ||
                                (o.CustInt1 != 0 && (o.BlockDir == block.BlockDir ||
                                o.BlockDir == Utils.DirOpp(block.BlockDir))))))
                                .ToList();
                            #endregion
                            #region Interaction: MazeBelt.cs
                            //Makes it so nothing stops on belts unless they
                            //are in the total opposite direction.
                            itemsFront = itemsFront.Where(o =>
                                !(o.BlockType == Type.Belt && (!o.IsEnabled ||
                                o.BlockDir != Utils.DirOpp(block.BlockDir)))).ToList();

                            //Can slide past all non-solid, non-belt objects.
                            itemsFront = itemsFront.Where(o =>
                                o.IsSolid || (o.BlockType == Type.Belt &&
                                o.IsEnabled)).ToList();

                            //Removes disabled belts from the list so they
                            //don't slide across the ice.
                            if (block.BlockType == Type.Belt)
                            {
                                continue;
                            }
                            #endregion

                            //If no solids block the path.
                            if (itemsFront.Count == 0)
                            {
                                {
                                    queueItems.Add(block);
                                    queuePos.Add(new Vector2(
                                        Utils.DirVector(block.BlockDir).X,
                                        Utils.DirVector(block.BlockDir).Y));
                                }
                            }
                        }
                    }
                }
                #endregion

                //Updates all moved blocks in sync.
                for (int i = 0; i < queueItems.Count; i++)
                {
                    queueItems[i].X += (int)queuePos[i].X;
                    queueItems[i].Y += (int)queuePos[i].Y;
                }
            }

            //Updates each item.
            foreach (GameObj item in items)
            {
                item.Update();

                //Selects a new & valid actor when needed.
                if (actor.hp <= 0 || !actor.IsEnabled)
                {
                    if (item.BlockType == Type.Actor)
                    {
                        if ((item as MazeActor).hp > 0 &&
                            (item as MazeActor).IsEnabled)
                        {
                            actor = (MazeActor)item;
                        }
                    }
                }

                //Processes win conditions.
                if (doWin)
                {
                    break;
                }
            }

            //Handles winning.
            if (doWin)
            {
                doWin = false;
                if (game.GmState != GameState.stateGameplay)
                {
                    if (opLvlLink == "")
                    {
                        SfxPlaylist.Play(sndWin);

                        if (game.GmState == GameState.stateGameplayEditor)
                        {
                            game.GmState = GameState.stateMenuEditor;
                        }
                        else
                        {
                            game.GmState = GameState.stateCampaignModes;
                        }
                    }
                    else
                    {
                        SfxPlaylist.Play(sndFinish);
                        game.mngrLvl.LoadPlay(opLvlLink);
                    }
                }
                else
                {
                    SfxPlaylist.Play(sndFinish);
                    game.mngrCampaign.CurrentSeries().LevelNum++;
                    if (game.mngrCampaign.CurrentSeries().LevelExists())
                    {
                        game.mngrCampaign.CurrentSeries().LoadCampaign();
                    }
                    else
                    {
                        SfxPlaylist.Play(sndWin);

                        if (game.GmState == GameState.stateGameplayEditor)
                        {
                            game.GmState = GameState.stateMenuEditor;
                        }
                        else
                        {
                            game.GmState = GameState.stateGameplaySeriesComplete;
                        }
                    }
                }
            }

            //If there are no valid actors, reverts.
            //If the max steps has been reached, reverts.
            if ((actor.hp <= 0 || !actor.IsEnabled) ||
                (OpMaxSteps != 0 && LvlSteps >= OpMaxSteps))
            {
                doRevert = true;
            }

            #region Player reverts/restarts level
            //If R is pressed, revert to the last checkpoint.
            if (game.KbState.IsKeyDown(Keys.R) &&
                game.KbStateOld.IsKeyUp(Keys.R))
            {
                //If control is held, restarts the whole level.
                if (game.KbState.IsKeyDown(Keys.LeftControl) ||
                    game.KbState.IsKeyDown(Keys.RightControl))
                {
                    doRestart = true;
                }
                else //otherwise, reverts to last checkpoint.
                {
                     doRevert = true;
                }
            }
            #endregion

            #region Handles checkpoints and restarting.

            //Reverts to last checkpoint if desired.
            if (doRevert)
            {
                doRevert = false;
                LevelRevert();
            }

            //Restarts the level if desired.
            else if (doRestart)
            {
                doRestart = false;
                LevelStart(new List<GameObj>(ItemsOrig));
            }

            //Saves a checkpoint if initiated.
            if (doCheckpoint)
            {
                SfxPlaylist.Play(sndCheckpoint);
                doCheckpoint = false;
                actorCoinsChkpt = ActorCoins;
                actorGoalsChkpt = ActorGoals;
                _lvlStepsChkpt = LvlSteps;

                //Checkpoints the level by overwriting the chkpt list.
                itemsChkpt = new List<GameObj>();
                foreach (GameObj item in items)
                {
                    itemsChkpt.Add(item.Clone());
                }
            }
            #endregion

            //Updates the camera position.
            Camera = Matrix.CreateTranslation(
                new Vector3(-actor.BlockSprite.rectDest.X,
                            -actor.BlockSprite.rectDest.Y, 0)) *
                Matrix.CreateScale(new Vector3(camZoom, camZoom, 1)) *
                Matrix.CreateTranslation(
                new Vector3(game.GetScreenSize().X * 0.5f,
                            game.GetScreenSize().Y * 0.5f, 0));
        }

        /// <summary>
        /// Draws blocks, including adjacent layers at 25% alpha.
        /// </summary>
        public void Draw()
        {
            //Organizes all items by sprite depth.
            items = items.OrderByDescending(o => o.BlockSprite.depth).ToList();

            //Draws each item.
            Rectangle scrnBounds = game.GetVisibleBounds(Camera, camZoom);

            //Draws a message when the screen is paused.
            if (isPaused)
            {
                //Updates the camera position.
                Camera = Matrix.CreateScale(new Vector3(camZoom, camZoom, 1));

                Vector2 scrnCenter = new Vector2(
                scrnBounds.X + scrnBounds.Width / 2f,
                scrnBounds.Y + scrnBounds.Height / 2f);

                game.GameSpriteBatch.DrawString(game.fntBold,
                    "Paused: Press enter to unpause.", scrnCenter, Color.Black,
                    0, game.fntBold.MeasureString(message) * 0.5f,
                    1, SpriteEffects.None, 0);

                return;
            }

            //Draws any custom messages to the screen.
            else if (isMessageShown)
            {
                //Updates the camera position.
                Camera = Matrix.CreateScale(new Vector3(camZoom, camZoom, 1));

                Vector2 scrnCenter = new Vector2(
                scrnBounds.X + scrnBounds.Width / 2f,
                scrnBounds.Y + scrnBounds.Height / 2f);

                game.GameSpriteBatch.DrawString(game.fntBold,
                    message, scrnCenter, Color.Black, 0,
                    game.fntBold.MeasureString(message) / 2,
                    1, SpriteEffects.None, 0);

                Vector2 scrnCenterShifted = new Vector2(
                scrnBounds.X + scrnBounds.Width / 2f,
                scrnBounds.Y + 16 + scrnBounds.Height / 2f);

                game.GameSpriteBatch.DrawString(game.fntDefault,
                    "Press enter to continue.", scrnCenter + new Vector2(0, 24),
                    Color.Black, 0, game.fntDefault.MeasureString("Press enter to continue.") / 2,
                    1, SpriteEffects.None, 0);

                return;
            }

            foreach (GameObj item in items)
            {
                //Renders above/below layers at 25% alpha.
                if (item.Layer == actor.Layer + 1 ||
                    item.Layer == actor.Layer - 1)
                {
                    item.BlockSprite.alpha = 0.25f;
                }
                else
                {
                    item.BlockSprite.alpha = 1;
                }

                //Only draws the current, below, and above layers.
                if (item.Layer == actor.Layer ||
                    item.Layer == actor.Layer + 1 ||
                    item.Layer == actor.Layer - 1)
                {
                    item.Draw();
                }
            }
        }

        /// <summary>
        /// Draws all sprites which do not shift with the camera.
        /// </summary>
        public void DrawHud()
        {
            //Sets up health and coins text.
            SpriteText hudHp =
                new SpriteText(game.fntDefault, actor.hp.ToString());
            hudHp.CenterOriginHor();
            hudHp.color = Color.Black;
            hudHp.drawBehavior = SpriteDraw.all;
            hudHp.position = new Vector2(16,
                5 + (int)game.GetScreenSize().Y - 32);

            SpriteText hudCoins =
                new SpriteText(game.fntDefault, ActorCoins.ToString());
            hudCoins.CenterOriginHor();
            hudCoins.color = Color.Black;
            hudCoins.drawBehavior = SpriteDraw.all;
            hudCoins.position = new Vector2(48,
                5 + (int)game.GetScreenSize().Y - 32);

            //Draws the bottom info bar with health and coins.
            sprHudOverlay.Draw(game.GameSpriteBatch);
            sprMenuHud.Draw(game.GameSpriteBatch);
            hudHp.Draw(game.GameSpriteBatch);
            hudCoins.Draw(game.GameSpriteBatch);

            //Removes the ending item separator on the tooltip.
            if (tooltip.EndsWith("| "))
            {
                tooltip = tooltip.Substring(0, tooltip.Length - 2);
            }

            //Draws the tooltip.
            game.GameSpriteBatch.DrawString(game.fntDefault, tooltip,
                new Vector2(68, 5 + (int)game.GetScreenSize().Y - 32),
                Color.Black);
        }
    }
}