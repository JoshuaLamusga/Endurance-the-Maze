using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ImpossiMaze
{
    public class SmoothRect : IEquatable<SmoothRect>
    {
        public float X, Y, Width, Height;

        public SmoothRect(float X, float Y, float Width, float Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
        }

        public SmoothRect(Vector2 pos, float Width, float Height)
        {
            X = pos.X;
            Y = pos.Y;
            this.Width = Width;
            this.Height = Height;
        }

        public SmoothRect(SmoothRect rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }

        public float Top
        {
            get
            {
                return Y;
            }
        }
        public float Bottom
        {
            get
            {
                return Y + Height;
            }
        }
        public float Left
        {
            get
            {
                return X;
            }
        }
        public float Right
        {
            get
            {
                return X + Width;
            }
        }
        public Vector2 Position
        {
            get
            {
                return new Vector2(X, Y);
            }
        }
        public static SmoothRect Empty
        {
            get
            {
                return new SmoothRect(0, 0, 0, 0);
            }
        }

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
    }
}