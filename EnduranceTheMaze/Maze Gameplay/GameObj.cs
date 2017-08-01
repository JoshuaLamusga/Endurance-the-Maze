using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Serves as a base class for all block objects.
    /// 
    /// Default activation behaviors for all derivatives:
    /// 0: Toggles visibility.
    /// 1: Toggles enabledness.
    /// 2: Switches direction clockwise.
    /// 3: Switches direction counterclockwise.
    /// 4: Deletes the object.
    /// 
    /// Custom activation behaviors are defined for most blocks. The
    /// activation index is used by blocks to discern which blocks to affect
    /// and deal with. ActuateIndex is just for actuators.
    /// </summary>
    public abstract class GameObj
    {
        //Relevant assets.
        public static SoundEffect sndActivated;

        //Refers to the game instance.
        protected MainLoop game;

        //Contains a sprite.
        private Sprite _sprite;

        //Contains a sprite.
        public Sprite sprite
        {
            get
            {
                return _sprite;
            }
            protected set
            {
                _sprite = value;

                //Sets the initial position.
                if (isSynchronized)
                {
                    _sprite.rectDest.X = x * 32;
                    _sprite.rectDest.Y = y * 32;
                }
                else
                {
                    _sprite.rectDest.X = x;
                    _sprite.rectDest.Y = y;
                }
            }
        }

        //Block location.
        private int _x;
        private int _y;
        private int _layer;
        public virtual int x
        {
            get
            {
                return _x;
            }

            internal set
            {
                _x = value;
            }
        }
        public virtual int y
        {
            get
            {
                return _y;
            }

            internal set
            {
                _y = value;
            }
        }
        public virtual int layer
        {
            get
            {
                return _layer;
            }

            internal set
            {
                _layer = value;
            }
        }

        //If synchronized, it's drawn to the grid.
        public bool isSynchronized;

        //Block identity by type.
        public Type type { get; internal set; }

        //Block's facing direction.
        private Dir _dir = Dir.Right;
        public virtual Dir dir
        {
            get
            {
                return _dir;
            }

            internal set
            {
                _dir = value;
            }
        }

        //Can items pass through, is it enabled/visible?
        private bool _isSolid;
        private bool _isEnabled;
        private bool _isVisible;
        public virtual bool isSolid
        {
            get
            {
                return _isSolid;
            }

            internal set
            {
                _isSolid = value;
            }
        }
        public virtual bool isEnabled
        {
            get
            {
                return _isEnabled;
            }

            internal set
            {
                _isEnabled = value;
            }
        }
        public virtual bool isVisible
        {
            get
            {
                return _isVisible;
            }

            internal set
            {
                _isVisible = value;
            }
        }

        //Block activation.
        private int _actionIndex = -1; //The activation channel.
        private int _actionType = -1; //The activation behavior.
        private int _actionIndex2 = -1; //The channel to activate (actuators).
        private bool _isActivated = false; //If the block is activated.
        public virtual int actionIndex
        {
            get
            {
                return _actionIndex;
            }

            internal set
            {
                _actionIndex = value;
            }
        }
        public virtual int actionType
        {
            get
            {
                return _actionType;
            }

            internal set
            {
                _actionType = value;
            }
        }
        public virtual int actionIndex2
        {
            get
            {
                return _actionIndex2;
            }

            internal set
            {
                _actionIndex2 = value;
            }
        }
        public virtual bool isActivated
        {
            get
            {
                return _isActivated;
            }

            internal set
            {
                _isActivated = value;
            }
        }

        //Custom properties for blocks. They're virtual for properties.
        private int _custInt1 = 0, _custInt2 = 0;
        private string _custStr = "";
        public virtual int custInt1
        {
            get
            {
                return _custInt1;
            }

            internal set
            {
                _custInt1 = value;
            }
        }
        public virtual int custInt2
        {
            get
            {
                return _custInt2;
            }

            internal set
            {
                _custInt2 = value;
            }
        }
        public virtual string custStr
        {
            get
            {
                return _custStr;
            }

            internal set
            {
                _custStr = value;
            }
        }

        /// <summary>
        /// Sets the block's location.
        /// </summary>
        /// <param name="x">The column number.</param>
        /// <param name="y">The row number.</param>
        /// <param name="layer">The layer in the maze.</param>
        public GameObj(MainLoop game, int x, int y, int layer)
        {
            this.game = game;
            this.x = x;
            this.y = y;
            this.layer = layer;
            isSolid = false;
            isEnabled = true;
            isVisible = true;
            isSynchronized = true;
        }

        /// <summary>
        /// Loads relevant assets into memory. (Underscore in method name
        /// used to distinguish from deriving blocks' LoadContent() methods.)
        /// </summary>
        /// <param name="Content">A game content loader.</param>
        public static void _LoadContent(ContentManager Content)
        {
            sndActivated = Content.Load<SoundEffect>("Content/Sounds/sndActivated");
        }

        /// <summary>
        /// Creates a deep copy of the object by copying all members.
        /// Must be implemented in all derivatives.
        /// </summary>
        public abstract GameObj Clone();

        /// <summary>
        /// Performs basic updates. Override to add functionality. Call last.
        /// </summary>
        public virtual void Update()
        {
            //Performs activation behaviors.
            if (isActivated)
            {
                if (actionType == 0)
                {
                    isActivated = false;
                    isVisible = !isVisible;
                }
                else if (actionType == 1)
                {
                    isActivated = false;
                    isEnabled = !isEnabled;
                }
                else if (actionType == 2 && isEnabled)
                {
                    isActivated = false;
                    dir = Utils.DirNext(dir);
                    game.playlist.Play(sndActivated, x, y);
                }
                else if (actionType == 3 && isEnabled)
                {
                    isActivated = false;
                    dir = Utils.DirPrev(dir);
                    game.playlist.Play(sndActivated, x, y);
                }
                else if (actionType == 4 && isEnabled)
                {
                    isActivated = false;
                    game.mngrLvl.RemoveItem(this);
                    game.playlist.Play(sndActivated, x, y);
                }
            }/*
            else
            {
                if (actionType == 0 && !isVisible)
                {
                    isVisible = true;
                    game.playlist.Play(sndActivated, x, y);
                }
                else if (actionType == 1 && !isEnabled)
                {
                    isEnabled = true;
                    game.playlist.Play(sndActivated, x, y);
                }
            }*/

            //Synchronizes sprite position to location.
            if (isSynchronized)
            {
                sprite.rectDest.X = x * 32;
                sprite.rectDest.Y = y * 32;
            }
            else
            {
                sprite.rectDest.X = x;
                sprite.rectDest.Y = y;
            }
        }

        /// <summary>
        /// Draws the sprite. Override to add functionality. Call last.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        public virtual void Draw()
        {
            if (isVisible)
            {
                sprite.Draw(game.GameSpriteBatch);
            }
        }
    }
}