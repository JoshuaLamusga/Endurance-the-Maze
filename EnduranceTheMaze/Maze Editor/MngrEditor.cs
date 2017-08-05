using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace EnduranceTheMaze
{
    /// <summary>
    /// The level editor. Handles level design logic and UI.
    /// 
    /// Dependencies: MainLoop.cs, MngrLvl, All MazeBlock children.
    /// </summary>
    public class MngrEditor
    {
        //Refers to the game instance.
        private MainLoop game;

        //The bottom toolbar, left sidebar, and block selector.
        private Sprite sprToolbar, sprSidebar;
        public int sidebarScroll = 0; //Amount of vertical sidebar scrolling.

        //Toolbar objects.
        public string tooltip; //Informational tip.
        private PropButton bttnActionIndex1, bttnActionIndex2,
            bttnActionType, bttnVal1, bttnVal2, bttnText, bttnDir,
            bttnIsEnabled, bttnGameDelay, bttnLvlLink, bttnMaxSteps,
            bttnMinGoals, bttnSyncActors, bttnSyncDeath;

        //Sidebar objects.
        public List<ImgType> itemTypes; //Items by type.

        //All positions occupied as a result of this mouse click + drag.
        public List<Vector2> itemDragPos;

        public Type activeType; //The active block type selected.
        public ImgBlock activeItem; //The active existing block selected.
        public List<ImgBlock> items; //All items in the level.

        //Contains the camera position and zoom.
        public Matrix Camera { get; private set; }
        public int camX, camY, camLayer;
        private float camZoom;

        //Contains the level settings.
        internal int opGameDelay, opMaxSteps, opMinGoals;
        internal string opLvlLink;
        internal bool opSyncActors, opSyncDeath;

        //Copied properties. These become default for new blocks.
        private int copyActIndex1, copyActIndex2, copyCustInt1, copyCustInt2,
            copyActType;
        private Type copyType;
        private Dir copyDir;
        private bool copyIsEnabled;
        private string copyCustStr;

        /// <summary>
        /// Sets the game instance and default level options.
        /// </summary>
        /// <param name="game">The game instance to use.</param>
        public MngrEditor(MainLoop game)
        {
            this.game = game;

            //Sets up toolbar objects.
            tooltip = "";

            itemDragPos = new List<Vector2>();

            activeType = Type.Actor;
            activeItem = null;
            items = new List<ImgBlock>();

            //Sets up the camera.
            camX = 0;
            camY = 0;
            camLayer = 0;
            camZoom = 1;

            //Updates the camera position.
            Camera = Matrix.CreateTranslation(new Vector3(-camX, -camY, 0)) *
                Matrix.CreateScale(new Vector3(camZoom, camZoom, 1)) *
                Matrix.CreateTranslation(
                new Vector3(game.GetScreenSize().X * 0.5f,
                            game.GetScreenSize().Y * 0.5f, 0));

            //Sets the default level settings.
            opGameDelay = 8;
            opMaxSteps = 0;
            opMinGoals = 0;
            opLvlLink = "";
            opSyncActors = false;
            opSyncDeath = false;

            //Sets the default copy values.
            copyActIndex1 = copyActIndex2 = 0;
            copyCustInt1 = copyCustInt2 = 0;
            copyActType = 0;
            copyDir = Dir.Right;
            copyIsEnabled = true;
            copyCustStr = "";
            copyType = 0;
        }

        /// <summary>
        /// Sets the default block properties based on the block type.
        /// </summary>
        private void SetDefaults(ImgBlock block)
        {
            switch (block.BlockType)
            {
                case Type.Click:
                case Type.Panel:
                    block.ActionIndex2 = 1;
                    block.ActionType = 5;
                    break;
                case Type.EAuto:
                    block.ActionIndex2 = 1;
                    block.ActionType = 5;
                    block.CustInt1 = 10;
                    break;
                case Type.ELight:
                    block.ActionType = 5;
                    break;
                case Type.EPusher:
                    block.ActionType = 5;
                    break;
                case Type.Gate:
                    block.ActionType = 5;
                    block.CustInt2 = 1;
                    break;
                case Type.Rotate:
                    block.ActionType = 5;
                    block.CustInt1 = 2;
                    break;
                case Type.Turret:
                    block.ActionType = 5;
                    block.CustInt1 = 10;
                    block.CustInt2 = 4;
                    break;
            }
        }

        ///<summary>
        ///Loads relevant graphics into memory.
        ///
        /// Dependencies: MainLoop.cs, MngrLvl.cs
        /// </summary>
        public void LoadContent()
        {
            //Loads sidebar assets.
            PropButton.LoadContent(game.Content);

            //Sets up the bottom toolbar.
            sprToolbar = new Sprite(true, MngrLvl.TexPixel);
            sprToolbar.color = Color.Gray;
            sprToolbar.alpha = 0.5f;
            sprToolbar.rectDest = new SmoothRect(new Vector2(
                0, game.GetScreenSize().Y - 32),
                game.GetScreenSize().X, 32);

            //Sets up toolbar buttons.
            //Level settings.
            bttnGameDelay = new PropButton(game,
                new Sprite(true, PropButton.TexOpGameDelay),
                new Vector2(32, game.GetScreenSize().Y - 32));
            bttnLvlLink = new PropButton(game,
                new Sprite(true, PropButton.TexOpLvlLink),
                new Vector2(66, game.GetScreenSize().Y - 32));
            bttnMaxSteps = new PropButton(game,
                new Sprite(true, PropButton.TexOpMaxSteps),
                new Vector2(100, game.GetScreenSize().Y - 32));
            bttnMinGoals = new PropButton(game,
                new Sprite(true, PropButton.TexOpMinGoals),
                new Vector2(134, game.GetScreenSize().Y - 32));
            bttnSyncActors = new PropButton(game,
                new Sprite(true, PropButton.TexOpSyncActors),
                new Vector2(168, game.GetScreenSize().Y - 32));
            bttnSyncDeath = new PropButton(game,
                new Sprite(true, PropButton.TexOpSyncDeath),
                new Vector2(202, game.GetScreenSize().Y - 32));

            //Active item properties.
            bttnActionIndex1 = new PropButton(game,
                new Sprite(true, PropButton.TexPropActionInd1),
                new Vector2(32, game.GetScreenSize().Y - 32));
            bttnActionIndex2 = new PropButton(game,
                new Sprite(true, PropButton.TexPropActionInd2),
                new Vector2(66, game.GetScreenSize().Y - 32));
            bttnActionType = new PropButton(game,
                new Sprite(true, PropButton.TexPropActionType),
                new Vector2(100, game.GetScreenSize().Y - 32));
            bttnVal1 = new PropButton(game,
                new Sprite(true, PropButton.TexPropCustInt1),
                new Vector2(134, game.GetScreenSize().Y - 32));
            bttnVal2 = new PropButton(game,
                new Sprite(true, PropButton.TexPropCustInt2),
                new Vector2(168, game.GetScreenSize().Y - 32));
            bttnText = new PropButton(game,
                new Sprite(true, PropButton.TexPropCustStr),
                new Vector2(202, game.GetScreenSize().Y - 32));
            bttnIsEnabled = new PropButton(game,
                new Sprite(true, PropButton.TexPropIsEnabled),
                new Vector2(236, game.GetScreenSize().Y - 32));
            bttnDir = new PropButton(game,
                new Sprite(true, PropButton.TexPropDir),
                new Vector2(268, game.GetScreenSize().Y - 32));

            //Sets up the sidebar.
            sprSidebar = new Sprite(true, MngrLvl.TexPixel);
            sprSidebar.color = Color.Gray;
            sprSidebar.alpha = 0.5f;
            sprSidebar.rectDest = new SmoothRect
                (Vector2.Zero, 32, game.GetScreenSize().Y - 32);

            //Sets up the selectable blocks.
            itemTypes = new List<ImgType>();
            itemTypes.Add(new ImgType(game, Type.Actor));
            itemTypes.Add(new ImgType(game, Type.Belt));
            itemTypes.Add(new ImgType(game, Type.Checkpoint));
            itemTypes.Add(new ImgType(game, Type.Click));
            itemTypes.Add(new ImgType(game, Type.Coin));
            itemTypes.Add(new ImgType(game, Type.CoinLock));
            itemTypes.Add(new ImgType(game, Type.Crate));
            itemTypes.Add(new ImgType(game, Type.CrateHole));
            itemTypes.Add(new ImgType(game, Type.EAuto));
            itemTypes.Add(new ImgType(game, Type.ELight));
            itemTypes.Add(new ImgType(game, Type.Enemy));
            itemTypes.Add(new ImgType(game, Type.EPusher));
            itemTypes.Add(new ImgType(game, Type.Filter));
            itemTypes.Add(new ImgType(game, Type.Finish));
            itemTypes.Add(new ImgType(game, Type.Floor));
            itemTypes.Add(new ImgType(game, Type.Freeze));
            itemTypes.Add(new ImgType(game, Type.Gate));
            itemTypes.Add(new ImgType(game, Type.Goal));
            itemTypes.Add(new ImgType(game, Type.Health));
            itemTypes.Add(new ImgType(game, Type.Ice));
            itemTypes.Add(new ImgType(game, Type.Key));
            itemTypes.Add(new ImgType(game, Type.Lock));
            itemTypes.Add(new ImgType(game, Type.Message));
            itemTypes.Add(new ImgType(game, Type.Mirror));
            itemTypes.Add(new ImgType(game, Type.MultiWay));
            itemTypes.Add(new ImgType(game, Type.Panel));
            itemTypes.Add(new ImgType(game, Type.Rotate));
            itemTypes.Add(new ImgType(game, Type.Spawner));
            itemTypes.Add(new ImgType(game, Type.Spike));
            itemTypes.Add(new ImgType(game, Type.Stairs));
            itemTypes.Add(new ImgType(game, Type.Teleporter));
            itemTypes.Add(new ImgType(game, Type.Thaw));
            itemTypes.Add(new ImgType(game, Type.Turret));
            itemTypes.Add(new ImgType(game, Type.Wall));
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
        /// Updates all block logic.
        /// </summary>
        public void Update()
        {
            //Records mouse positions for convenience.
            int mouseX = (int)GetCoordsMouse().X;
            int mouseY = (int)GetCoordsMouse().Y;

            //Resets the tooltip.
            tooltip = "";

            #region Handles input controls
            if (game.IsActive)
            {
                //Enables scrolling the sidebar.
                if (game.MsState.X <= 32)
                {
                    if (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue)
                    {
                        sidebarScroll -= 32;

                        //Clamps the scrolling to the last item in the list.
                        //480 = screen height / 32.
                        if (sidebarScroll < itemTypes.Count * -32 + (int)game.GetScreenSize().Y)
                        {
                            sidebarScroll = itemTypes.Count * -32 + (int)game.GetScreenSize().Y;
                        }
                    }
                    else if (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue)
                    {
                        sidebarScroll += 32;
                        if (sidebarScroll > 0)
                        {
                            sidebarScroll = 0;
                        }
                    }
                }

                //If the mouse isn't over the toolbar or sidebar.
                if (game.MsState.X > 32 && game.MsState.Y <
                    game.GetScreenSize().Y - 32)
                {
                    //When the mouse is no longer held, resets holding list.
                    if (game.MsState.LeftButton == ButtonState.Released &&
                        game.MsState.RightButton == ButtonState.Released)
                    {
                        itemDragPos = new List<Vector2>();
                    }

                    #region Creating blocks.
                    //Whether the player holds Ctrl + V.
                    bool isPasting =
                        ((game.KbState.IsKeyDown(Keys.LeftControl) ||
                        game.KbState.IsKeyDown(Keys.RightControl)) &&
                        game.KbState.IsKeyDown(Keys.V));

                    //If clicking to set a block or pressing Ctrl + V.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.KbState.IsKeyUp(Keys.LeftControl) &&
                        game.KbState.IsKeyUp(Keys.RightControl)) ||
                        isPasting)
                    {
                        //Gets a list of all blocks in the current position.
                        List<GameObj> blocks = new List<GameObj>();
                        foreach (ImgBlock block in items.Where(o =>
                            o.X == (int)Math.Round(mouseX / 32f) &&
                            o.Y == (int)Math.Round(mouseY / 32f) &&
                            o.Layer == camLayer))
                        {
                            blocks.Add(Utils.BlockFromType
                                (game, block.BlockType, 0, 0, 0));
                        }

                        //If the position is open for the mouse.
                        if (itemDragPos.Where(o =>
                            o.X == (int)Math.Round(mouseX / 32f) &&
                            o.Y == (int)Math.Round(mouseY / 32f))
                            .Count() == 0)
                        {
                            //If the block to be placed is not solid.
                            Type typeToUse = (isPasting) ? copyType : activeType;

                            if (!(Utils.BlockFromType
                                (game, typeToUse, 0, 0, 0).IsSolid &&
                                blocks.Where(o => o.IsSolid).Count() != 0) &&
                                !(blocks.Where(o => o.BlockType == typeToUse)
                                .Count() > 0 && typeToUse != Type.Filter &&
                                typeToUse != Type.Teleporter))
                            {
                                //Adds the position.
                                itemDragPos.Add(new Vector2(
                                    (int)Math.Round(mouseX / 32f),
                                    (int)Math.Round(mouseY / 32f)));

                                //Adds the item.
                                items.Add(new ImgBlock(game, typeToUse,
                                    (int)Math.Round(mouseX / 32f),
                                    (int)Math.Round(mouseY / 32f), camLayer));

                                //Sets the new item as the active one.
                                activeItem = items[items.Count - 1];

                                //Copies saved properties over it.
                                if (isPasting)
                                {
                                    activeItem.ActionIndex = copyActIndex1;
                                    activeItem.ActionIndex2 = copyActIndex2;
                                    activeItem.CustInt1 = copyCustInt1;
                                    activeItem.CustInt2 = copyCustInt2;
                                    activeItem.ActionType = copyActType;
                                    activeItem.BlockDir = copyDir;
                                    activeItem.IsEnabled = copyIsEnabled;
                                    activeItem.CustStr = copyCustStr;
                                    activeItem.AdjustSprite();
                                    activeItem.BlockType = typeToUse;
                                }
                                else
                                {
                                    SetDefaults(activeItem);
                                }

                                //Adds a floor panel if there aren't any.
                                if (blocks.Where(o => o.BlockType == Type.Floor)
                                    .Count() == 0 && typeToUse != Type.Floor)
                                {
                                    items.Add(new ImgBlock(game, Type.Floor,
                                        (int)Math.Round(mouseX / 32f),
                                        (int)Math.Round(mouseY / 32f),
                                        camLayer));
                                }

                                //Resets all button visibility.
                                bttnActionIndex1.IsVisible = true;
                                bttnActionIndex2.IsVisible = true;
                                bttnActionType.IsVisible = true;
                                bttnDir.IsVisible = true;
                                bttnVal1.IsVisible = true;
                                bttnVal2.IsVisible = true;
                                bttnIsEnabled.IsVisible = true;
                                bttnText.IsVisible = true;
                            }
                        }
                    }
                    #endregion
                    #region Deleting blocks.
                    if (game.MsState.RightButton == ButtonState.Pressed)
                    {
                        //Gets a list of all items in the grid location.
                        List<ImgBlock> tempList = items.Where(o =>
                            o.X == (int)Math.Round(mouseX / 32f) &&
                            o.Y == (int)Math.Round(mouseY / 32f) &&
                            o.Layer == camLayer).ToList();

                        //If the position is open, removes the item.
                        if (itemDragPos.Where(o =>
                            o.X == (int)Math.Round(mouseX / 32f) &&
                            o.Y == (int)Math.Round(mouseY / 32f))
                            .Count() == 0)
                        {
                            //Adds the position.
                            itemDragPos.Add(new Vector2(
                                (int)Math.Round(mouseX / 32f),
                                (int)Math.Round(mouseY / 32f)));

                            //Removes the topmost item in the list.
                            if (tempList.Count > 0)
                            {
                                //Organizes by depth so top item is topmost.
                                tempList.OrderBy(o => o.BlockSprite.depth);
                                tempList.Reverse();

                                //Removes the item.
                                items.Remove(tempList[0]);

                                //Removes the active item status as necessary.
                                if (activeItem == tempList[0])
                                {
                                    activeItem = null;
                                }

                                //Removes the floor block if it stands alone.
                                tempList.RemoveAt(0);
                                if (tempList.Where(o => o.BlockType != Type.Floor)
                                    .Count() == 0)
                                {
                                    foreach (ImgBlock item in tempList)
                                    {
                                        items.Remove(item);

                                        //Removes the active item status as necessary.
                                        if (activeItem == item)
                                        {
                                            activeItem = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #region Selecting blocks.
                    if (game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released &&
                        (game.KbState.IsKeyDown(Keys.LeftControl) ||
                        game.KbState.IsKeyDown(Keys.RightControl)))
                    {
                        //Gets a list of all objects under the mouse.
                        List<ImgBlock> tempList = items.Where(o =>
                            o.X == (int)Math.Round(mouseX / 32f) &&
                            o.Y == (int)Math.Round(mouseY / 32f) &&
                            o.Layer == camLayer).ToList();

                        //Organizes the list by depth.
                        if (tempList.Count != 0)
                        {
                            tempList.OrderBy(o => o.BlockSprite.depth);
                            tempList.Reverse();

                            //Toggles active item status on the block.
                            if (tempList[0] != activeItem)
                            {
                                activeItem = tempList[0];
                                bttnActionIndex1.IsVisible = true;
                                bttnActionIndex2.IsVisible = true;
                                bttnActionType.IsVisible = true;
                                bttnDir.IsVisible = true;
                                bttnVal1.IsVisible = true;
                                bttnVal2.IsVisible = true;
                                bttnIsEnabled.IsVisible = true;
                                bttnText.IsVisible = true;
                            }
                            else
                            {
                                activeItem = null;
                            }
                        }
                    }
                    #endregion
                    #region Copying block properties.
                    if (activeItem != null)
                    {
                        //If Ctrl + C is pressed.
                        if ((game.KbState.IsKeyDown(Keys.LeftControl) ||
                            game.KbState.IsKeyDown(Keys.RightControl)) &&
                            game.KbState.IsKeyDown(Keys.C) &&
                            game.KbStateOld.IsKeyUp(Keys.C))
                        {
                            copyActIndex1 = activeItem.ActionIndex;
                            copyActIndex2 = activeItem.ActionIndex2;
                            copyCustInt1 = activeItem.CustInt1;
                            copyCustInt2 = activeItem.CustInt2;
                            copyActType = activeItem.ActionType;
                            copyDir = activeItem.BlockDir;
                            copyIsEnabled = activeItem.IsEnabled;
                            copyCustStr = activeItem.CustStr;
                            copyType = activeItem.BlockType;
                        }
                    }
                    #endregion
                }

                //Handles moving up/down layers.
                if (!bttnText.IsHovered) //If not typing text.
                {
                    if ((game.KbState.IsKeyDown(Keys.OemPlus) &&
                        game.KbStateOld.IsKeyUp(Keys.OemPlus)))
                    {
                        camLayer++;
                        itemDragPos = new List<Vector2>(); //resets drag list.
                    }
                    if ((game.KbState.IsKeyDown(Keys.OemMinus) &&
                        game.KbStateOld.IsKeyUp(Keys.OemMinus)))
                    {
                        camLayer--;
                        itemDragPos = new List<Vector2>(); //resets drag list.
                    }
                }
            }
            #endregion
            #region Handles camera functionality.
            if (game.IsActive)
            {
                //Enables basic zooming.
                if (game.MsState.X > 32 && game.MsState.Y <
                    game.GetScreenSize().Y - 32)
                {
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
                            camZoom = ((int)Math.Round(10 *
                                (camZoom + 0.1))) / 10f;
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
                            camZoom = ((int)Math.Round(10 *
                                (camZoom - 0.1))) / 10f;
                        }
                    }
                }

                //Handles moving the camera.
                if (!bttnText.IsHovered)
                {
                    if ((game.KbState.IsKeyDown(Keys.D) ||
                        (game.KbState.IsKeyDown(Keys.Right))))
                    {
                        camX += 8;
                    }
                    else if ((game.KbState.IsKeyDown(Keys.S) ||
                        (game.KbState.IsKeyDown(Keys.Down))))
                    {
                        camY += 8;
                    }
                    else if ((game.KbState.IsKeyDown(Keys.A) ||
                        (game.KbState.IsKeyDown(Keys.Left))))
                    {
                        camX -= 8;
                    }
                    else if ((game.KbState.IsKeyDown(Keys.W) ||
                        (game.KbState.IsKeyDown(Keys.Up))))
                    {
                        camY -= 8;
                    }
                }
            }

            //Updates the camera position.
            Camera = Matrix.CreateTranslation(new Vector3(-camX, -camY, 0)) *
                Matrix.CreateScale(new Vector3(camZoom, camZoom, 1)) *
                Matrix.CreateTranslation(
                new Vector3(game.GetScreenSize().X * 0.5f,
                            game.GetScreenSize().Y * 0.5f, 0));
            #endregion
            #region Handles toolbar, sidebar, and block properties.
            //Always updates the active item's sprite to account for changes.
            if (activeItem == null)
            {
                //Updates toolbar buttons.
                bttnGameDelay.Update();
                bttnLvlLink.Update();
                bttnMaxSteps.Update();
                bttnMinGoals.Update();
                bttnSyncActors.Update();
                bttnSyncDeath.Update();

                #region Handles changing level settings / block properties.
                if (bttnGameDelay.IsHovered)
                {
                    //Left clicking or scrolling up.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue))
                    {
                        opGameDelay++;
                    }

                    //Right clicking or scrolling down.
                    if ((game.MsState.RightButton == ButtonState.Pressed &&
                        game.MsStateOld.RightButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue))
                    {
                        opGameDelay--;
                    }
                }
                else if (bttnLvlLink.IsHovered)
                {
                    //Left clicking or scrolling up.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue))
                    {
                        //Opens a file to determine it exists. If so, uses
                        OpenFileDialog diag = new OpenFileDialog();

                        //Sets the initial directory for user friendliness.
                        if (opLvlLink != "")
                        {
                            diag.InitialDirectory =
                                Path.GetDirectoryName(opLvlLink);
                        }

                        //Shows the dialog and sets the level link if it can.
                        if (diag.ShowDialog() == DialogResult.OK)
                        {
                            opLvlLink = diag.FileName;
                        }
                    }

                    //Right clicking or scrolling down.
                    if ((game.MsState.RightButton == ButtonState.Pressed &&
                        game.MsStateOld.RightButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue))
                    {
                        opLvlLink = "";
                    }
                }
                else if (bttnMaxSteps.IsHovered)
                {
                    //Left clicking or scrolling up.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue))
                    {
                        opMaxSteps++;
                    }

                    //Right clicking or scrolling down.
                    if ((game.MsState.RightButton == ButtonState.Pressed &&
                        game.MsStateOld.RightButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue))
                    {
                        opMaxSteps--;
                    }
                }
                else if (bttnMinGoals.IsHovered)
                {
                    //Left clicking or scrolling up.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue))
                    {
                        opMinGoals++;
                    }

                    //Right clicking or scrolling down.
                    if ((game.MsState.RightButton == ButtonState.Pressed &&
                        game.MsStateOld.RightButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue))
                    {
                        opMinGoals--;
                    }
                }
                else if (bttnSyncActors.IsHovered)
                {
                    //Left clicking or scrolling up.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue))
                    {
                        opSyncActors = true;
                    }

                    //Right clicking or scrolling down.
                    if ((game.MsState.RightButton == ButtonState.Pressed &&
                        game.MsStateOld.RightButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue))
                    {
                        opSyncActors = false;
                    }
                }
                else if (bttnSyncDeath.IsHovered)
                {
                    //Left clicking or scrolling up.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue))
                    {
                        opSyncDeath = true;
                    }

                    //Right clicking or scrolling down.
                    if ((game.MsState.RightButton == ButtonState.Pressed &&
                        game.MsStateOld.RightButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue))
                    {
                        opSyncDeath = false;
                    }
                }
                #endregion
                #region Sets the bounds for block properties.
                if (opGameDelay < 2)
                {
                    opGameDelay = 2;
                }
                if (!File.Exists(opLvlLink))
                {
                    opLvlLink = "";
                }
                if (opMaxSteps < 0)
                {
                    opMaxSteps = 0;
                }
                if (opMinGoals < 0)
                {
                    opMinGoals = 0;
                }
                #endregion
            }
            else
            {
                //Updates toolbar buttons.
                bttnActionIndex1.Update();
                bttnActionIndex2.Update();
                bttnActionType.Update();
                bttnDir.Update();
                bttnVal1.Update();
                bttnVal2.Update();
                bttnIsEnabled.Update();
                bttnText.Update();

                #region Handles changing active block values.
                if (bttnActionIndex1.IsHovered && bttnActionIndex1.IsVisible)
                {
                    //Left clicking or scrolling up.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue))
                    {
                        activeItem.ActionIndex++;
                    }

                    //Right clicking or scrolling down.
                    if ((game.MsState.RightButton == ButtonState.Pressed &&
                        game.MsStateOld.RightButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue))
                    {
                        activeItem.ActionIndex--;
                    }
                }
                else if (bttnActionIndex2.IsHovered &&
                    bttnActionIndex2.IsVisible)
                {
                    //Left clicking or scrolling up.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue))
                    {
                        activeItem.ActionIndex2++;
                    }

                    //Right clicking or scrolling down.
                    if ((game.MsState.RightButton == ButtonState.Pressed &&
                        game.MsStateOld.RightButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue))
                    {
                        activeItem.ActionIndex2--;
                    }
                }
                else if (bttnActionType.IsHovered && bttnActionType.IsVisible)
                {
                    //Left clicking or scrolling up.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue))
                    {
                        activeItem.ActionType++;
                    }

                    //Right clicking or scrolling down.
                    if ((game.MsState.RightButton == ButtonState.Pressed &&
                        game.MsStateOld.RightButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue))
                    {
                        activeItem.ActionType--;
                    }
                }
                else if (bttnDir.IsHovered && bttnDir.IsVisible)
                {
                    //Left clicking or scrolling up.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue))
                    {
                        if (activeItem.BlockType == Type.Enemy)
                        {
                            activeItem.BlockDir = Utils.DirNextAll(activeItem.BlockDir);
                        }
                        else
                        {
                            activeItem.BlockDir = Utils.DirNext(activeItem.BlockDir);
                        }
                    }

                    //Right clicking or scrolling down.
                    if ((game.MsState.RightButton == ButtonState.Pressed &&
                        game.MsStateOld.RightButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue))
                    {
                        if (activeItem.BlockType == Type.Enemy)
                        {
                            activeItem.BlockDir = Utils.DirPrevAll(activeItem.BlockDir);
                        }
                        else
                        {
                            activeItem.BlockDir = Utils.DirPrev(activeItem.BlockDir);
                        }
                    }
                }
                else if (bttnVal1.IsHovered && bttnVal1.IsVisible)
                {
                    //Left clicking or scrolling up.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue))
                    {
                        activeItem.CustInt1++;
                        if (activeItem.BlockType == Type.Filter &&
                        activeItem.CustInt1 == 0)
                        {
                            activeItem.CustInt1 = 1;
                        }
                    }

                    //Right clicking or scrolling down.
                    if ((game.MsState.RightButton == ButtonState.Pressed &&
                        game.MsStateOld.RightButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue))
                    {
                        activeItem.CustInt1--;
                        if (activeItem.BlockType == Type.Filter &&
                            activeItem.CustInt1 == 0)
                        {
                            activeItem.CustInt1 = -1;
                        }
                    }
                }
                else if (bttnVal2.IsHovered && bttnVal2.IsVisible)
                {
                    //Left clicking or scrolling up.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue))
                    {
                        activeItem.CustInt2++;
                    }

                    //Right clicking or scrolling down.
                    if ((game.MsState.RightButton == ButtonState.Pressed &&
                        game.MsStateOld.RightButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue))
                    {
                        activeItem.CustInt2--;
                    }
                }
                else if (bttnIsEnabled.IsHovered && bttnIsEnabled.IsVisible)
                {
                    //Left clicking or scrolling up.
                    if ((game.MsState.LeftButton == ButtonState.Pressed &&
                        game.MsStateOld.LeftButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue >
                        game.MsStateOld.ScrollWheelValue))
                    {
                        activeItem.IsEnabled = true;
                    }

                    //Right clicking or scrolling down.
                    if ((game.MsState.RightButton == ButtonState.Pressed &&
                        game.MsStateOld.RightButton == ButtonState.Released) ||
                        (game.MsState.ScrollWheelValue <
                        game.MsStateOld.ScrollWheelValue))
                    {
                        activeItem.IsEnabled = false;
                    }
                }
                else if (bttnText.IsHovered && bttnText.IsVisible)
                {
                    //Gets all keys and whether shift is pressed.
                    Keys[] keys = game.KbState.GetPressedKeys();
                    bool shiftPressed = false;

                    //Determines if shift is pressed.
                    if (game.KbState.IsKeyDown(Keys.LeftShift) ||
                        game.KbState.IsKeyDown(Keys.RightShift))
                    {
                        shiftPressed = true;
                    }

                    //Iterates through each held key.
                    foreach (Keys key in keys)
                    {
                        //If the key was just pressed.
                        if (game.KbState.IsKeyDown(key) &&
                            game.KbStateOld.IsKeyUp(key))
                        {
                            //Adds the smart string representation.
                            activeItem.CustStr = KeyboardStateExt.KeyToString(
                                activeItem.CustStr, key, shiftPressed);
                        }
                    }
                }
                #endregion
                #region Sets block property ranges and button visibility.
                //actionIndex.
                if (activeItem.ActionIndex < 0)
                {
                    activeItem.ActionIndex = 0;
                }

                //actionIndex2.
                if (activeItem.ActionIndex2 < 0)
                {
                    activeItem.ActionIndex2 = 0;
                }
                if (activeItem.BlockType != Type.EAuto &&
                    activeItem.BlockType != Type.Panel &&
                    activeItem.BlockType != Type.Click)
                {
                    bttnActionIndex2.IsVisible = false;
                }

                //actionType.
                if (activeItem.ActionType < 0)
                {
                    activeItem.ActionType = 0;
                }
                else if (activeItem.BlockType == Type.Crate ||
                    activeItem.BlockType == Type.EPusher ||
                    activeItem.BlockType == Type.ELight)
                {
                    if (activeItem.ActionType > 5)
                    {
                        activeItem.ActionType = 5;
                    }
                }
                else if (activeItem.BlockType == Type.Gate)
                {
                    if (activeItem.ActionType > 7)
                    {
                        activeItem.ActionType = 7;
                    }
                }
                else if (activeItem.BlockType == Type.Click ||
                    activeItem.BlockType == Type.EAuto ||
                    activeItem.BlockType == Type.Panel ||
                    activeItem.BlockType == Type.Rotate)
                {
                    if (activeItem.ActionType > 7)
                    {
                        activeItem.ActionType = 7;
                    }
                }
                else if (activeItem.BlockType == Type.Spawner ||
                    activeItem.BlockType == Type.Filter)
                {
                    if (activeItem.ActionType - 4 >
                        Enum.GetNames(typeof(Type)).Length)
                    {
                        activeItem.ActionType =
                            Enum.GetNames(typeof(Type)).Length + 4;
                    }
                }
                else if (activeItem.BlockType == Type.Turret)
                {
                    if (activeItem.ActionType > 5)
                    {
                        activeItem.ActionType = 5;
                    }
                }
                else if (activeItem.ActionType > 4)
                {
                    activeItem.ActionType = 4;
                }

                //custInt1.
                if (activeItem.BlockType == Type.Crate)
                {
                    if (activeItem.CustInt1 >
                        Enum.GetNames(typeof(Type)).Length)
                    {
                        activeItem.CustInt1 =
                            Enum.GetNames(typeof(Type)).Length;
                    }
                    if (activeItem.CustInt1 < 0)
                    {
                        activeItem.CustInt1 = 0;
                    }
                }
                else if (activeItem.BlockType == Type.Rotate)
                {
                    //No maximum limit on rotation grid's size.
                    if (activeItem.CustInt1 < 0)
                    {
                        activeItem.CustInt1 = 0;
                    }
                }
                else if (activeItem.BlockType == Type.CoinLock)
                {
                    //No maximum limit on number of coins required.
                    if (activeItem.CustInt1 < 1)
                    {
                        activeItem.CustInt1 = 0;
                    }
                }
                else if (activeItem.BlockType == Type.EAuto)
                {
                    if (activeItem.CustInt1 < 2)
                    {
                        activeItem.CustInt1 = 2;
                    }
                }
                else if (activeItem.BlockType == Type.Filter)
                {
                    if (activeItem.CustInt1 == 0)
                    {
                        activeItem.CustInt1 = 1;
                    }
                    if (activeItem.CustInt1 < -1)
                    {
                        activeItem.CustInt1 = -1;
                    }
                }
                else if (activeItem.BlockType == Type.Checkpoint ||
                    activeItem.BlockType == Type.Click ||
                    activeItem.BlockType == Type.Gate ||
                    activeItem.BlockType == Type.Stairs ||
                    activeItem.BlockType == Type.Panel ||
                    activeItem.BlockType == Type.MultiWay ||
                    activeItem.BlockType == Type.Teleporter)
                {
                    if (activeItem.CustInt1 > 1)
                    {
                        activeItem.CustInt1 = 1;
                    }
                    else if (activeItem.CustInt1 < 0)
                    {
                        activeItem.CustInt1 = 0;
                    }
                }
                else if (activeItem.BlockType == Type.Key ||
                    activeItem.BlockType == Type.Lock)
                {
                    if (activeItem.CustInt1 > 9)
                    {
                        activeItem.CustInt1 = 9;
                    }
                    else if (activeItem.CustInt1 < 0)
                    {
                        activeItem.CustInt1 = 0;
                    }
                }
                else if (activeItem.BlockType == Type.Turret)
                {
                    if (activeItem.CustInt1 < 2)
                    {
                        activeItem.CustInt1 = 2;
                    }
                    else if (activeItem.CustInt1 < 0)
                    {
                        activeItem.CustInt1 = 0;
                    }
                }
                else
                {
                    if (activeItem.CustInt1 != 0)
                    {
                        activeItem.CustInt1 = 0;
                    }

                    bttnVal1.IsVisible = false;
                }

                //custInt2.
                if (activeItem.CustInt2 < 0)
                {
                    activeItem.CustInt2 = 0;
                }
                if (activeItem.BlockType == Type.Teleporter)
                {
                    //No max limit on teleporter channel.
                }
                else if (activeItem.BlockType == Type.Click ||
                    activeItem.BlockType == Type.CoinLock ||
                    activeItem.BlockType == Type.Filter ||
                    activeItem.BlockType == Type.EAuto ||
                    activeItem.BlockType == Type.Gate)
                {
                    if (activeItem.CustInt2 > 1)
                    {
                        activeItem.CustInt2 = 1;
                    }
                }
                else if (activeItem.BlockType == Type.Turret)
                {
                    if (activeItem.CustInt2 < 3)
                    {
                        activeItem.CustInt2 = 3;
                    }
                    else if (activeItem.CustInt2 > 15)
                    {
                        activeItem.CustInt2 = 15;
                    }
                }
                else
                {
                    if (activeItem.CustInt2 > 0)
                    {
                        activeItem.CustInt2 = 0;
                    }

                    bttnVal2.IsVisible = false;
                }

                //dir.
                if (activeItem.BlockType != Type.Actor &&
                    activeItem.BlockType != Type.Belt &&
                    activeItem.BlockType != Type.Crate &&
                    activeItem.BlockType != Type.Enemy &&
                    activeItem.BlockType != Type.EPusher &&
                    activeItem.BlockType != Type.Mirror &&
                    activeItem.BlockType != Type.MultiWay &&
                    activeItem.BlockType != Type.Spawner &&
                    activeItem.BlockType != Type.Turret)
                {
                    activeItem.BlockDir = Dir.Right;
                    bttnDir.IsVisible = false;
                }

                //Only enemy blocks can have diagonal directions.
                if (activeItem.BlockType != Type.Enemy &&
                    !Utils.DirCardinal(activeItem.BlockDir))
                {
                    activeItem.BlockDir = Utils.DirNextAll(activeItem.BlockDir);
                }

                //These aren't affected by enabledness.
                if (activeItem.BlockType == Type.Checkpoint ||
                    activeItem.BlockType == Type.Coin ||
                    activeItem.BlockType == Type.Crate ||
                    activeItem.BlockType == Type.CrateHole ||
                    activeItem.BlockType == Type.Finish ||
                    activeItem.BlockType == Type.Floor ||
                    activeItem.BlockType == Type.Freeze ||
                    activeItem.BlockType == Type.Goal ||
                    activeItem.BlockType == Type.Health ||
                    activeItem.BlockType == Type.Ice ||
                    activeItem.BlockType == Type.Key ||
                    activeItem.BlockType == Type.Lock ||
                    activeItem.BlockType == Type.Message ||
                    activeItem.BlockType == Type.Mirror ||
                    activeItem.BlockType == Type.Spike ||
                    activeItem.BlockType == Type.Stairs ||
                    activeItem.BlockType == Type.Thaw ||
                    activeItem.BlockType == Type.Wall)
                {
                    activeItem.IsEnabled = true;
                    bttnIsEnabled.IsVisible = false;
                }

                //Text.
                if (activeItem.BlockType != Type.Message)
                {
                    bttnText.IsVisible = false;
                }
                #endregion

                activeItem.AdjustSprite();
            }

            //Updates the sidebar position on the far left.
            sprSidebar.rectDest.X = 0;
            sprSidebar.rectDest.Y = 0;
            
            //Updates the positions of all selectable block types.
            for (int i = 0; i < itemTypes.Count; i++)
            {
                itemTypes[i].X = 0;
                itemTypes[i].Y = i;
                itemTypes[i].Update();
            }
            #endregion
            #region Handles updating blocks.
            //Updates each item.
            foreach (ImgBlock item in items)
            {
                item.Update();
            }
            #endregion

            //Adds some level information.
            if (activeItem == null)
            {
                tooltip += "Sidebar: " + activeType.ToString();
                tooltip += " | Blocks: " + items.Count + " | Layer: " +
                    camLayer;
            }
            else
            {
                tooltip += "Block: " + activeItem.BlockType;
            }
        }

        /// <summary>
        /// Draws blocks, including adjacent layers at 25% alpha.
        /// </summary>
        public void Draw()
        {
            //Organizes all items by sprite depth.
            items = items.OrderByDescending(o => o.BlockSprite.depth).ToList();

            //Draws each item.
            foreach (ImgBlock item in items)
            {
                //Renders above/below layers at 25% alpha.
                if (item.Layer == camLayer + 1 ||
                    item.Layer == camLayer - 1)
                {
                    item.BlockSprite.alpha = 0.25f;
                }
                else
                {
                    item.BlockSprite.alpha = 1;
                }

                //Only draws the current, below, and above layers.
                if (item.Layer == camLayer ||
                    item.Layer == camLayer + 1 ||
                    item.Layer == camLayer - 1)
                {
                    item.Draw();
                }
            }

            //Draws the active item indicator.
            if (activeItem != null)
            {
                game.GameSpriteBatch.Draw(MngrLvl.TexPixel, new Rectangle(
                    activeItem.X * 32 - 16, activeItem.Y * 32 - 16,
                    32, 32), Color.Yellow * 0.5f);
            }

            //Draws the selector.
            if (game.MsState.X > 32 &&
                game.MsState.Y < game.GetScreenSize().Y - 32)
            {
                int selX = (int)(Math.Round((int)GetCoordsMouse().X / 32f));
                int selY = (int)(Math.Round((int)GetCoordsMouse().Y / 32f));

                ImgBlock blk;

                if (game.KbState.IsKeyDown(Keys.LeftControl) ||
                    game.KbState.IsKeyDown(Keys.RightControl))
                {
                    blk = new ImgBlock(game, (Type)copyType,
                        selX, selY, 0);

                    blk.ActionIndex = copyActIndex1;
                    blk.ActionIndex2 = copyActIndex2;
                    blk.ActionType = copyActType;
                    blk.CustInt1 = copyCustInt1;
                    blk.CustInt2 = copyCustInt2;
                    blk.BlockDir = copyDir;
                    blk.IsEnabled = copyIsEnabled;
                }
                else
                {
                    blk = new ImgBlock(game, (Type)activeType,
                        selX, selY, 0);

                    SetDefaults(blk);
                }

                blk.AdjustSprite();
                blk.BlockSprite.alpha = 0.5f;
                if (blk.BlockType != Type.Key && blk.BlockType != Type.Lock)
                {
                    blk.BlockSprite.color = Color.Green;
                }

                blk.Draw();
            }
        }

        /// <summary>
        /// Draws all sprites which do not shift with the camera.
        /// </summary>
        public void DrawHud()
        {
            //Draws the toolbar, sidebar, and selector.
            sprToolbar.Draw(game.GameSpriteBatch);
            sprSidebar.Draw(game.GameSpriteBatch);

            //Draws all selectable block types in the sidebar.
            for (int i = 0; i < itemTypes.Count; i++)
            {
                itemTypes[i].Draw();
            }

            //Draws toolbar buttons and information for active item.
            //Sets up a text outline to be used by all buttons.
            SpriteText tempText = new SpriteText();
            tempText.font = game.fntBold;
            tempText.color = Color.Black;
            tempText.drawBehavior = SpriteDraw.all;

            if (activeItem == null)
            {
                #region Draws level settings + text.
                bttnGameDelay.Draw(); //Delay in game timer.
                tempText.text = opGameDelay.ToString();
                tempText.position = new Vector2(
                    bttnGameDelay.Pos.X + 16,
                    bttnGameDelay.Pos.Y + 16);
                tempText.CenterOrigin();
                tempText.Draw(game.GameSpriteBatch);
                bttnLvlLink.Draw(); //Next level to play when completed.
                bttnMaxSteps.Draw(); //Maximum steps allowed.
                tempText.text = opMaxSteps.ToString();
                tempText.position = new Vector2(
                    bttnMaxSteps.Pos.X + 16,
                    bttnMaxSteps.Pos.Y + 16);
                tempText.CenterOrigin();
                tempText.Draw(game.GameSpriteBatch);
                bttnMinGoals.Draw(); //Minimum goals required.
                tempText.text = opMinGoals.ToString();
                tempText.position = new Vector2(
                    bttnMinGoals.Pos.X + 16,
                    bttnMinGoals.Pos.Y + 16);
                tempText.CenterOrigin();
                tempText.Draw(game.GameSpriteBatch);
                bttnSyncActors.Draw(); //If all actors copy active movements.
                if (opSyncActors)
                {
                    tempText.text = "on";
                }
                else
                {
                    tempText.text = "off";
                }
                tempText.position = new Vector2(
                    bttnSyncActors.Pos.X + 16,
                    bttnSyncActors.Pos.Y + 16);
                tempText.CenterOrigin();
                tempText.Draw(game.GameSpriteBatch);
                bttnSyncDeath.Draw(); //If any actor death reverts to chkpt.
                if (opSyncDeath)
                {
                    tempText.text = "on";
                }
                else
                {
                    tempText.text = "off";
                }
                tempText.position = new Vector2(
                    bttnSyncDeath.Pos.X + 16,
                    bttnSyncDeath.Pos.Y + 16);
                tempText.CenterOrigin();
                tempText.Draw(game.GameSpriteBatch);
                #endregion
                #region Draws level settings based on values.
                if (bttnGameDelay.IsHovered)
                {
                    tooltip = "The game timer delay. Lower values make " +
                        "moving objects faster, and vice versa.";
                }
                if (bttnLvlLink.IsHovered)
                {
                    if (opLvlLink == "")
                    {
                        tooltip = "The next level to automatically play when " +
                            "this one is conquered.";
                    }
                    else
                    {
                        tooltip = "Next lvl: " + opLvlLink;
                    }
                }
                if (bttnMaxSteps.IsHovered)
                {
                    tooltip = "If not zero: Limits the number of movements " +
                        "the player can make for a puzzle twist.";
                }
                if (bttnMinGoals.IsHovered)
                {
                    tooltip = "The number of goal objects required to beat " +
                        "the level.";
                }
                if (bttnSyncActors.IsHovered)
                {
                    tooltip = "If on, all actors try to copy your movements.";
                }
                if (bttnSyncDeath.IsHovered)
                {
                    tooltip = "If on, the level reverts to last checkpoint " +
                        "on any actor death.";
                }

                //Draws the tooltip in the toolbar.
                game.GameSpriteBatch.DrawString(game.fntDefault, tooltip,
                    new Vector2(236, (int)game.GetScreenSize().Y - 27),
                    Color.Black);
                #endregion
            }
            else
            {
                #region Draws active item properties + text.

                //Draws all toolbar buttons with text.
                if (bttnActionIndex1.IsVisible)
                {
                    bttnActionIndex1.Draw();
                    tempText.text = activeItem.ActionIndex.ToString();
                    tempText.position = new Vector2(
                        bttnActionIndex1.Pos.X + 16,
                        bttnActionIndex1.Pos.Y + 16);
                    tempText.CenterOrigin();
                    tempText.Draw(game.GameSpriteBatch);
                }
                if (bttnActionIndex2.IsVisible)
                {
                    bttnActionIndex2.Draw();
                    tempText.text = activeItem.ActionIndex2.ToString();
                    tempText.position = new Vector2(
                        bttnActionIndex2.Pos.X + 16,
                        bttnActionIndex2.Pos.Y + 16);
                    tempText.CenterOrigin();
                    tempText.Draw(game.GameSpriteBatch);
                }
                if (bttnDir.IsVisible)
                {
                    bttnDir.Draw();
                }
                if (bttnActionType.IsVisible)
                {
                    bttnActionType.Draw();
                    tempText.text = activeItem.ActionType.ToString();
                    tempText.position = new Vector2(
                        bttnActionType.Pos.X + 16,
                        bttnActionType.Pos.Y + 16);
                    tempText.CenterOrigin();
                    tempText.Draw(game.GameSpriteBatch);
                }
                if (bttnVal1.IsVisible)
                {
                    bttnVal1.Draw();
                    tempText.text = activeItem.CustInt1.ToString();
                    tempText.position = new Vector2(
                        bttnVal1.Pos.X + 16,
                        bttnVal1.Pos.Y + 16);
                    tempText.CenterOrigin();
                    tempText.Draw(game.GameSpriteBatch);
                }
                if (bttnVal2.IsVisible)
                {
                    bttnVal2.Draw();
                    tempText.text = activeItem.CustInt2.ToString();
                    tempText.position = new Vector2(
                        bttnVal2.Pos.X + 16,
                        bttnVal2.Pos.Y + 16);
                    tempText.CenterOrigin();
                    tempText.Draw(game.GameSpriteBatch);
                }
                if (bttnIsEnabled.IsVisible)
                {
                    if (activeItem.IsEnabled)
                    {
                        tempText.text = "on";
                    }
                    else
                    {
                        tempText.text = "off";
                    }

                    bttnIsEnabled.Draw();
                    tempText.position = new Vector2(
                        bttnIsEnabled.Pos.X + 16,
                        bttnIsEnabled.Pos.Y + 16);
                    tempText.CenterOrigin();
                    tempText.Draw(game.GameSpriteBatch);
                }
                if (bttnText.IsVisible)
                {
                    bttnText.Draw();
                }
                #endregion
                #region Draws active block properties based on values.
                #region Handles ActionIndex1
                if (bttnActionIndex1.IsHovered && bttnActionIndex1.IsVisible)
                {
                    tooltip = "The activation channel. Matching actuator " +
                        "channels can activate this block.";
                }
                #endregion
                #region Handles ActionIndex2
                else if (bttnActionIndex2.IsHovered &&
                    bttnActionIndex2.IsVisible)
                {
                    tooltip = "The actuator channel. The activation " +
                        "channel to be activated by panels, for example.";
                }
                #endregion
                #region Handles ActionType
                else if (bttnActionType.IsHovered && bttnActionType.IsVisible)
                {
                    tooltip = "When activated: ";

                    if (activeItem.ActionType < 5)
                    {
                        switch (activeItem.ActionType)
                        {
                            case 0:
                                tooltip += "toggles block visibility.";
                                break;
                            case 1:
                                tooltip += "toggle block enabled.";
                                break;
                            case 2:
                                tooltip += "rotates clockwise.";
                                break;
                            case 3:
                                tooltip += "rotates counter-clockwise.";
                                break;
                            case 4:
                                tooltip += "destroys the block.";
                                break;
                        }
                    }
                    else
                    {
                        switch (activeItem.BlockType)
                        {
                            case Type.Click:
                                if (activeItem.ActionType == 5)
                                {
                                    tooltip += "activates linked items " +
                                        "on trigger.";
                                }
                                else if (activeItem.ActionType == 6)
                                {
                                    tooltip += "deactivates linked items " +
                                        "on trigger.";
                                }
                                else if (activeItem.ActionType == 7)
                                {
                                    tooltip += "de/activates linked items " +
                                        "each other trigger.";
                                }
                                break;
                            case Type.Crate:
                                if (activeItem.ActionType == 5)
                                {
                                    tooltip += "breaks open.";
                                }
                                break;
                            case Type.EAuto:
                                if (activeItem.ActionType == 5)
                                {
                                    tooltip += "activates linked items " +
                                        "on trigger.";
                                }
                                else if (activeItem.ActionType == 6)
                                {
                                    tooltip += "deactivates linked items " +
                                        "on trigger.";
                                }
                                else if (activeItem.ActionType == 7)
                                {
                                    tooltip += "de/activates linked items " +
                                        "each other trigger.";
                                }
                                break;
                            case Type.ELight:
                                if (activeItem.ActionType == 5)
                                {
                                    tooltip += "lights up while active.";
                                }
                                break;
                            case Type.EPusher:
                                if (activeItem.ActionType == 5)
                                {
                                    tooltip += "pushes blocks if possible.";
                                }
                                break;
                            case Type.Filter:
                                tooltip += "becomes " +
                                    (Type)(activeItem.ActionType - 5) +
                                    " if possible.";
                                break;
                            case Type.Gate:
                                if (activeItem.ActionType == 5)
                                {
                                    tooltip += "toggles block solidity.";
                                }
                                else if (activeItem.ActionType == 6)
                                {
                                    tooltip += "is solid, but otherwise " +
                                        "is not.";
                                }
                                else if (activeItem.ActionType == 7)
                                {
                                    tooltip += "isn't solid, but otherwise " +
                                        "is.";
                                }
                                break;
                            case Type.Panel:
                                if (activeItem.ActionType == 5)
                                {
                                    tooltip += "activates and deactivates " +
                                        "linked items when pressed.";
                                }
                                else if (activeItem.ActionType == 6)
                                {
                                    tooltip += "activates linked items " +
                                        "when pressed.";
                                }
                                else if (activeItem.ActionType == 7)
                                {
                                    tooltip += "activates linked items " +
                                        "when pressed, then disables itself.";
                                }
                                break;
                            case Type.Rotate:
                                if (activeItem.ActionType == 5)
                                {
                                    tooltip += "rotates 90 degrees " +
                                        "clockwise.";
                                }
                                else if (activeItem.ActionType == 6)
                                {
                                    tooltip += "rotates 90 degrees " +
                                        "counter-clockwise.";
                                }
                                else if (activeItem.ActionType == 7)
                                {
                                    tooltip += "rotates 180 degrees.";
                                }
                                break;
                            case Type.Spawner:
                                tooltip += "creates " +
                                    (Type)(activeItem.ActionType - 5) +
                                    " in the pointed direction if possible.";
                                break;
                            case Type.Turret:
                                if (activeItem.ActionType == 5)
                                {
                                    tooltip += "fires a bullet.";
                                }
                                break;
                        }
                    }
                }
                #endregion
                #region Handles CustDir (custom direction)
                else if (bttnDir.IsHovered && bttnDir.IsVisible)
                {
                    tooltip = activeItem.BlockDir.ToString();
                }
                #endregion
                #region Handles CustInt1
                else if (bttnVal1.IsHovered && bttnVal1.IsVisible)
                {
                    switch (activeItem.BlockType)
                    {
                        case Type.Checkpoint:
                            if (activeItem.CustInt1 == 0)
                            {
                                tooltip = "Saves every time an actor " +
                                    "touches the block.";
                            }
                            else if (activeItem.CustInt1 == 1)
                            {
                                tooltip = "Saves when an actor touches " +
                                    "the block, then deletes it.";
                            }
                            break;
                        case Type.Click:
                            if (activeItem.CustInt1 == 0)
                            {
                                tooltip = "Functions normally.";
                            }
                            if (activeItem.CustInt1 == 1)
                            {
                                tooltip = "Deletes itself after one use.";
                            }
                            break;
                        case Type.CoinLock:
                            tooltip = "Opens on contact when the player has " +
                                activeItem.CustInt1 + "coins.";
                            break;
                        case Type.Crate:
                            if (activeItem.CustInt1 > 0)
                            {
                                tooltip = "When crate breaks, contains: " +
                                    (Type)(activeItem.CustInt1 - 1);
                            }
                            else
                            {
                                tooltip = "Contains nothing when broken.";
                            }
                            break;
                        case Type.EAuto:
                            tooltip = "Triggers every " +
                                activeItem.CustInt1 + " frames.";
                            break;
                        case Type.Filter:
                            if (activeItem.CustInt1 > 0)
                            {
                                tooltip = "If passed over " +
                                    activeItem.CustInt1 + " times, it " +
                                    "activates.";
                            }
                            else if (activeItem.CustInt1 == -1)
                            {
                                tooltip = "Cannot be activated by passing " +
                                    "over it.";
                            }
                            break;
                        case Type.Gate:
                            if (activeItem.CustInt1 == 0)
                            {
                                tooltip = "Can't close on solid objects.";
                            }
                            else if (activeItem.CustInt1 == 1)
                            {
                                tooltip = "May close on actors. Trapped " +
                                    "actors automatically lose all health.";
                            }
                            break;
                        case Type.Key:
                        case Type.Lock:
                            if (activeItem.CustInt1 == 0)
                            {
                                tooltip = "Blue.";
                            }
                            else if (activeItem.CustInt1 == 1)
                            {
                                tooltip = "Red.";
                            }
                            else if (activeItem.CustInt1 == 2)
                            {
                                tooltip = "Yellow.";
                            }
                            else if (activeItem.CustInt1 == 3)
                            {
                                tooltip = "Purple.";
                            }
                            else if (activeItem.CustInt1 == 4)
                            {
                                tooltip = "Orange.";
                            }
                            else if (activeItem.CustInt1 == 5)
                            {
                                tooltip = "Black.";
                            }
                            else if (activeItem.CustInt1 == 6)
                            {
                                tooltip = "Dark blue.";
                            }
                            else if (activeItem.CustInt1 == 7)
                            {
                                tooltip = "Dark red.";
                            }
                            else if (activeItem.CustInt1 == 8)
                            {
                                tooltip = "Dark yellow.";
                            }
                            else if (activeItem.CustInt1 == 9)
                            {
                                tooltip = "Dark orange.";
                            }
                            break;
                        case Type.MultiWay:
                            if (activeItem.CustInt1 == 0)
                            {
                                tooltip = "The block is one-way.";
                            }
                            else if (activeItem.CustInt1 == 1)
                            {
                                tooltip = "The block is two-way.";
                            }
                            break;
                        case Type.Panel:
                            if (activeItem.CustInt1 == 0)
                            {
                                tooltip = "Works for linked items " +
                                    "regardless of layer.";
                            }
                            else if (activeItem.CustInt1 == 1)
                            {
                                tooltip = "Works for linked items on the " +
                                    "same layer only.";
                            }
                            break;
                        case Type.Rotate:
                                tooltip = "Rotates " + activeItem.CustInt1 +
                                    " * " + activeItem.CustInt1 + " blocks.";
                            break;
                        case Type.Stairs:
                            if (activeItem.CustInt1 == 0)
                            {
                                tooltip = "Stairs lead upwards.";
                            }
                            else if (activeItem.CustInt1 == 1)
                            {
                                tooltip = "Stairs lead downwards.";
                            }
                            break;
                        case Type.Teleporter:
                            if (activeItem.CustInt1 == 0)
                            {
                                tooltip = "Teleporter is a sender. Blocks " +
                                    "enter this to teleport.";
                            }
                            else if (activeItem.CustInt1 == 1)
                            {
                                tooltip = "Teleporter is a receiver. " +
                                    "Blocks appear here when they teleport.";
                            }
                            break;
                        case Type.Turret:
                            tooltip = "Shoots bullets every " +
                                activeItem.CustInt1 +
                                " frames.";
                            break;
                    }
                }
                #endregion
                #region Handles CustInt2
                else if (bttnVal2.IsHovered && bttnVal2.IsVisible)
                {
                    switch (activeItem.BlockType)
                    {
                        case Type.CoinLock:
                            if (activeItem.CustInt2 == 0)
                            {
                                tooltip = "Doesn't subtract coins on unlocking.";
                            }
                            else if (activeItem.CustInt2 == 1)
                            {
                                tooltip = "Subtracts coins on unlocking.";
                            }
                            break;
                        case Type.Click:
                        case Type.EAuto:
                            if (activeItem.CustInt2 == 0)
                            {
                                tooltip = "Works for linked items " +
                                    "regardless of layer.";
                            }
                            else if (activeItem.CustInt2 == 1)
                            {
                                tooltip = "Works for linked items on the " +
                                    "same layer only.";
                            }
                            break;
                        case Type.Filter:
                            if (activeItem.CustInt2 == 0)
                            {
                                tooltip = "Objects can pass through.";
                            }
                            else if (activeItem.CustInt2 == 1)
                            {
                                tooltip = "Objects can not pass through.";
                            }
                            break;
                        case Type.Gate:
                            if (activeItem.CustInt2 == 0)
                            {
                                tooltip = "Is not solid.";
                            }
                            else if (activeItem.CustInt2 == 1)
                            {
                                tooltip = "Is solid.";
                            }
                            break;
                        case Type.Teleporter:
                            tooltip = "Teleport channel: " +
                                activeItem.CustInt2 + ". Senders link to " +
                                "receivers on the same channel to function.";
                            break;
                        case Type.Turret:
                            tooltip = "bullet speed: " +
                                activeItem.CustInt2 + ".";
                            break;
                    }
                }
                #endregion
                #region Handles CustStr (custom string)
                else if (bttnText.IsHovered && bttnText.IsVisible)
                {
                    if (activeItem.CustStr.Length == 0)
                    {
                        tooltip = "Type text while hovered over button for " +
                            "custom text.";
                    }
                    else
                    {
                        tooltip = activeItem.CustStr;
                    }
                }
                #endregion
                #region Handles IsEnabled.
                else if (bttnIsEnabled.IsHovered && bttnIsEnabled.IsVisible)
                {
                    if (activeItem.IsEnabled)
                    {
                        tooltip = "Block is enabled.";
                    }
                    else
                    {
                        tooltip = "Block is disabled.";
                    }
                }
                #endregion
                #endregion

                //Draws the tooltip in the toolbar after buttons.
                game.GameSpriteBatch.DrawString(game.fntDefault, tooltip,
                    new Vector2(302, (int)game.GetScreenSize().Y - 27),
                    Color.Black);
            }
        }

        /// <summary>
        /// Returns an equivalent MazeBlock list to the representative
        /// ImgBlock list for actual use.
        /// </summary>
        public void LoadTest()
        {
            //Loads the level from the editor for testing.
            List<GameObj> items2 = new List<GameObj>();
            foreach (ImgBlock item in items)
            {
                GameObj block = Utils.BlockFromType(game,
                    item.BlockType, item.X, item.Y, item.Layer);
                block.ActionIndex = item.ActionIndex;
                block.ActionIndex2 = item.ActionIndex2;
                block.ActionType = item.ActionType;
                block.CustInt1 = item.CustInt1;
                block.CustInt2 = item.CustInt2;
                block.CustStr = item.CustStr;
                block.BlockDir = item.BlockDir;
                block.IsEnabled = item.IsEnabled;
                items2.Add(block);
            }

            game.mngrLvl.LevelStart(items2);
        }

        /// <summary>
        /// Loads a level from a file with a given path.
        /// </summary>
        /// <param name="path">The path and filename.</param>
        public void LoadEdit(string path)
        {
            if (File.Exists(path))
            {
                //Creates a stream object to the file.
                Stream stream = File.OpenRead(path);

                //Opens a text reader on the file.
                TextReader txtRead = new StreamReader(stream);

                //Gets all block items with each entry as a block.
                //Includes level settings.
                List<string> strItems =
                    txtRead.ReadToEnd().Split('|').ToList();

                //Closes the text reader.
                txtRead.Close();

                //Clears the level if the loaded level has content.
                if (strItems.Count > 0)
                {
                    items.Clear();
                }

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

                            Int32.TryParse(strBlock[1], out opGameDelay);
                            opLvlLink = strBlock[2];
                            Int32.TryParse(strBlock[3], out opMaxSteps);
                            Int32.TryParse(strBlock[4], out opMinGoals);
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
                            ImgBlock tempBlock;
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
                            tempBlock = new ImgBlock(game, tempType,
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
                            items.Add(tempBlock);

                            //Adjusts the block sprite.
                            tempBlock.AdjustSprite();
                        }
                    }
                }

                //Closes resources.
                stream.Close();
            }
        }

        /// <summary>
        /// Loads a level from a file.
        /// </summary>
        public void LoadEdit()
        {
            //Creates a dialog to display.
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Level files (*.lvl)|*.lvl|txt files (*.txt)|*.txt";

            //If a result is chosen from the dialog.
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                LoadEdit(dlg.FileName);
            }
        }

        /// <summary>
        /// Writes a level to a file in binary.
        /// </summary>
        /// <param name="path">The path and filename.</param>
        public void LevelSave()
        {
            //Creates a stream object.
            Stream stream;

            //Creates a dialog to display.
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Level files (*.lvl)|*.lvl|txt files (*.txt)|*.txt";

            //If a result is chosen from the dialog.
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //If the stream is in working order.
                if ((stream = dlg.OpenFile()) != null)
                {

                    //Opens a text writer on the string.
                    TextWriter txtWrite = new StreamWriter(stream);

                    //Writes the version.
                    txtWrite.Write("v1.0.0|");

                    //Writes all level settings.
                    txtWrite.Write("ops,");
                    txtWrite.Write(opGameDelay + ",");
                    txtWrite.Write(opLvlLink + ",");
                    txtWrite.Write(opMaxSteps + ",");
                    txtWrite.Write(opMinGoals + ",");
                    txtWrite.Write(opSyncActors + ",");
                    txtWrite.Write(opSyncDeath + "|");

                    //Writes all block items. Converts text , to tabs.
                    foreach (ImgBlock item in game.mngrEditor.items)
                    {
                        txtWrite.Write("blk,");
                        txtWrite.Write((int)item.BlockType + ",");
                        txtWrite.Write(item.X + ",");
                        txtWrite.Write(item.Y + ",");
                        txtWrite.Write(item.Layer + ",");
                        txtWrite.Write(item.ActionIndex + ",");
                        txtWrite.Write(item.ActionIndex2 + ",");
                        txtWrite.Write(item.ActionType + ",");
                        txtWrite.Write(item.CustInt1 + ",");
                        txtWrite.Write(item.CustInt2 + ",");
                        txtWrite.Write(item.BlockDir + ",");
                        txtWrite.Write(item.IsEnabled + ",");
                        txtWrite.Write(item.CustStr.Replace(",", "\t") + "|");
                    }

                    //Closes resources.
                    txtWrite.Close();
                    stream.Close();
                }
            }
        }
    }
}