Endurance: The Maze, V 1.0.0

***Project Information***
This maze combines many different puzzles and ideas cultivated from various
games into one maze. It comes complete with a level editor and a selection of
levels that will challenge and teach you how to play.

Gameplay:
- WASD / arrowkeys to move active actor.
- Click other actors if OpSyncActors is off to make them active.
- Click items which enable clicking to activate them.
- Escape returns to the menu and pauses gameplay.
- Zoom in/out with mousewheel.

Level Editor:
- Select block types for placement in left sidebar.
- Scroll sidebar up/down with mousewheel while hovered.
- Hold mouse to place your block type at grid location if possible.
- Right-click or drag to delete top layer of blocks under mouse.
- Ctrl + left-click to select active block.
- Ctrl + right-click to deselect active block.
- WASD / arrowkeys to move camera.
- Zoom in/out of camera with mousewheel.
- Hit + and - to navigate up/down layers.
- While hovered over active item property button, left/right-click to
  increment or decrement. Mousewheel works similarly.

***Project Structure***
The project utilizes the SimpleXnaFramework that I won't describe here.

MainInit starts the program and begins MainLoop, which ties everything
together. It loads all of the textures, performs all updates, and draws
everything according to the GameState. The GameState specifies which "mode"
the game is in (menu, level editing, and gameplay are some).

MngrTitle handles menus. TitleItem objects make up the buttons. MngrLvl loads
and saves levels, but most importantly, ties all gameplay logic together. It
loads all block textures and updates/draws them in a loaded block list of
MazeBlock objects (which are templates for all blocks).

MngrEditor handles level editing. The toolbar contains customization tools for
each block and the sidebar contains a populated list of each item to work
with. The sidebar uses ImgType objects and the toolbar uses PropButton. The
level itself uses ImgBlock objects to represent all of the blocks.

For items, actionIndex is used to give items unique or corresponding action
indices. This is used with actuators to trigger isActivated, causing any
activated objects to perform behaviors according to their actionType. For all
blocks, 0 and 1 toggle visibility/enabled, 2 destroys the item. 3 rotates
clockwise and 4 counter-clockwise.

To add a menu item, load its texture and make a TitleItem object. Add what
happens when clicked to MngrTitle's Update method.

To add a maze block, include the sprite asset and make a MazeBlock child
object. Add the game logic as you see fit. Load its content under MngrLvl's
LoadContent method. Include it in Utils.BlockFromType. Add it to Type enum.
Add it to itemTypes in LoadContent under MngrEditor. Add it to AdjustSprite
method in ImgBlock. Add customization restrictions under MngrEditor in Update.

***Bugs, workarounds, and bad design***
Cruft tends to be encapsulated in blocks titled "Interaction:" and the name of
the class. Cruft is mainly in MazeActor and MngrLvl classes. The editor has
more than a little visible cruft, though, and needs a makeover with the
toolbar buttons (at least).

***Version changelog***

BUGS
1. Turret lasers skip over moving solids including player on ice, belts, etc.
2. Turret speeds != 4 can distort mirror bouncing and fail.
3. Things stopped on ice at any point shouldn't start moving again without cause.

1/6/14: Pre-alpha revision.
1/28/14: Beta released.
6/20/16: First release (v1.0).
6/26/16: Second release (v1.1).
7/30/17: Third release (v1.2).

VERSION 1.1 RELEASE CHANGELOG
- Changed menu graphics
- Added turrets, turret bullets, and mirrors
- Fixed numerous bugs

VERSION 1.2 RELEASE CHANGELOG
- Gates can be made solid to begin with.
- Added coin locks.
- The actor can no longer push crates adjacent to them when on ice.

TODO:
All solids can skip bullets, not just the player that was fixed.
Player on ice or conveyor will still skip bullets the same way.

When crates/actors are first spawned, if they're on ice, they slide to the right. They shouldn't. And all solids should be slide-able.

Actors next to each other trying to move in the same direction are sometimes offset as a result of how blocks are updated.
 
Add a laser-bullet-activated actuator (triggers every # bullets that hit it).
Add some way to activate a certain behavior of linked objects directly.
Add some way to activate multiple channels from one actuator. Maybe some sort of actuator linker?

Have the sidebar hotkeyed so you can press 0 - 9 on any block to toggle the
hotkey on it. The key is drawn next to the image of the block. You can then
press that key when not hovered over the sidebar to create one of those blocks.
If multi-select is active, it fills that selection.

Add multi-select to the editor.
Change the "You Win!" dialog to make it nice. Make a separate room for it.
Give coins a purpose by making some sort of coin lock.
Make more actuators and types of actuators. Think of physical interactions.
Rewrite the whole thing from the ground up. Yeah, sure. Some day.
For anything that has instances of another object, you should be able to set that object's properties too.

For actuators:
- proximity based
- absorbs X laser bullets and activates

For linked items:
- swap all linked items with an instance of another item
- set the direction of all linked items to some direction
- rotate all linked items counter, clockwise, or 180 degrees
- set or toggle visibility of all linked items
- set or toggle enabledness of all linked items
- proximity-based actuator

Items to do:
 - Make a coin lock
 - Make a goal lock
 - Trap that hurts you if you stand on it too long. No indication until you stand on it.
 - Trap that hurts you if you stand on it, but on a repeating timer. Visible.

Maze Redesign
To target lag, we need to check code and lists only when we know something
will happen. So move all item-checking code into static functions belonging
to each class:

GAME - contains all global logic, like the current player and level data.
PLAYER - bool CanMove(player obj)
ICE - bool Slide(obj, ice obj)
...and so on.

Then objects that interact with ice, like players and crates, will check in their update methods.
Update will only be called for objects that need updating. Same with draw. Controlled by doUpdate and doDraw.
There will be no tooltip bar if this is redesigned. All info like health and coins will be displayed over actor's heads when hovered.
Levels will list their modifiers when they load, displaying the level options chosen for about 1.5 seconds. Any key skips it.
Try smooth movements. Use bounding boxes for intersection.
Lots of things will be tied to properties, and everything will be made with properties. So when something must be checked when a change is encountered,
it can be hooked to a property that executes logic.

- How can I target crates containing crates containing specific items and such? With flexible properties.


Every game object:
DoUpdate - Should update logic be executed this frame?
Update() - All remaining logic for updating.
DoUpdateSprite - Should sprite-specific update logic be executed this frame?
UpdateSprite() - Determines the sprite and its alpha, color, angle, etc.
DoDraw - Should draw logic be executed this frame?
Draw() - Contains the code to draw the sprite to the screen.
IsEnabled - Is the block enabled? If not, only hears enabling activations..
IsMoved - Has the block moved? Private setter handled by PosX and PosY properties.
IsVisible - Is the block drawn? If not, draw method is skipped like !DoDraw.
IsSolid - Non-solid blocks can be placed on solid blocks in the editor.
DrawDepth - The order to draw the block in.
PosX - The x-position of the block in world coords.
PosY - The y-position of the block in world coords.
Sprite - The texture to use, including frame, origin, color, angle, & alpha.
IsActive - Whether the block is on or not.
ActivateChanged() - Abstract method. Executes when IsActive changes.
Dict<int, Func<>> ChannelBehavior - Contains functions to execute. The parent game object will implement all.
Dict<string, obj> Properties - Contains additional data that can be saved/loaded.

Game object behaviors:
0 - No action
1 - Make invisible
2 - Make visible
3 - Toggle visible
4 - Make disabled
5 - Make enabled
6 - Toggle enabled
7 - Turn clockwise
8 - Turn counterclockwise
9 - Rotate 180
10 - Move one layer up
11 - Move one layer down
12 - Move right if possible
13 - Move up if possible
14 - Move left if possible
15 - Move down if possible
16 - Delete object

The actual world coordinates of each block will always be given. This means
bullets don't need to convert coords. The grid size will be 64x64. All sprites
will be redone. The level editor's action behavior sprite will be a lightning
bolt. Draw culling will be reinstated. Consider lazy-loading textures and even
unloading them if unnecessary.

Every level will have a description and title. Escape key will pause the game
rather than return to last menu if testing or playing. The pause menu will
include resume, save and exit, and show the level name and description.

If doUpdate, doUpdateSprite, or doDraw is false, they won't be executed. If isMoved is true,
maybe set doUpdate to true in the properties. Properties will be defined
abstract for every game object.

doUpdateSprite allows the sprite to be changed without executing all update
logic, which means no need for separate editor blocks. A single variable fixed
that need...

The editor will display the words "block properties" followed by a stack of available properties so they have no gaps.