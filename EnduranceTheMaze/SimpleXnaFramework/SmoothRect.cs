using Microsoft.Xna.Framework;
using System;

namespace EnduranceTheMaze
{
    /// <summary>
    /// A floating-point rectangle which allows for smooth animation.
    /// </summary>
    public class SmoothRect : IEquatable<SmoothRect>
    {
        #region Members
        public float X, Y, Width, Height;
        #endregion

        #region Properties
        /// <summary>
        /// The first Y coordinate.
        /// </summary>
        public float Top
        {
            get
            {
                return Y;
            }
        }

        /// <summary>
        /// The second Y coordinate.
        /// </summary>
        public float Bottom
        {
            get
            {
                return Y + Height;
            }
        }

        /// <summary>
        /// The first X coordinate.
        /// </summary>
        public float Left
        {
            get
            {
                return X;
            }
        }

        /// <summary>
        /// The second X coordinate.
        /// </summary>
        public float Right
        {
            get
            {
                return X + Width;
            }
        }

        /// <summary>
        /// The X and Y coordinates.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return new Vector2(X, Y);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a rectangle from a position and dimensions.
        /// </summary>
        /// <param name="X">The x-coordinate.</param>
        /// <param name="Y">The y-coordinate.</param>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        public SmoothRect(float X, float Y, float Width, float Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
        }

        /// <summary>
        /// Creates a rectangle from a position and dimensions.
        /// </summary>
        /// <param name="pos">The X and Y coordinates.</param>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        public SmoothRect(Vector2 pos, float Width, float Height)
        {
            X = pos.X;
            Y = pos.Y;
            this.Width = Width;
            this.Height = Height;
        }

        /// <summary>
        /// Clone constructor.
        /// </summary>
        public SmoothRect(SmoothRect rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified Object is equivalent.
        /// </summary>
        /// <param name="other">
        /// The Object to compare with the current Rectangle.
        /// </param>
        public bool Equals(SmoothRect other)
        {
            if (X == other.X &&
                Y == other.Y &&
                Width == other.Width &&
                Height == other.Height)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a Rectangle instance.
        /// </summary>
        public Rectangle ToRect()
        {
            return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// An empty rectangle with all values set to zero.
        /// </summary>
        public static SmoothRect Empty
        {
            get
            {
                return new SmoothRect(0, 0, 0, 0);
            }
        }

        /// <summary>
        /// Returns a SmoothRect large enough to fit two other SmoothRects.
        /// </summary>
        public static SmoothRect Union(SmoothRect rect1, SmoothRect rect2)
        {
            float posX, posY, posWidth, posHeight;
            posX = Math.Min(rect1.X, rect2.X);
            posY = Math.Min(rect1.Y, rect2.Y);
            posWidth = Math.Max(rect1.X + rect1.Width, rect2.X + rect2.Width);
            posHeight = Math.Max(rect1.Y + rect1.Height, rect2.Y + rect2.Height);

            //Creates a new SmoothRect.
            return new SmoothRect(posX, posY, posWidth, posHeight);
        }

        /// <summary>
        /// Returns whether or not two SmoothRects intersect.
        /// </summary>
        public static bool IsIntersecting(SmoothRect rect1, SmoothRect rect2)
        {
            //Returns true if the rectangles are both the exact same.
            if (rect1.X == rect2.X && rect1.Y == rect2.Y &&
                rect1.Width == rect2.Width && rect1.Height == rect2.Height)
            {
                return true;
            }

            //Guarantees that the corners of the rectangles are aligned such
            //that there is always one intersection.
            if (rect1.X < rect2.X + rect2.Width &&
                rect2.X < rect1.X + rect1.Width &&
                rect1.Y < rect2.Y + rect2.Height &&
                rect2.Y < rect1.Y + rect1.Height)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}