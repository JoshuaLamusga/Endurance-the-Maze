using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EnduranceTheMaze
{
    public class Sprite
    {
        public Texture2D texture; //The pre-loaded 2d sprite texture.
        public SmoothRect rectSrc, rectDest; //Source & dest. rectangles.
        public Vector2 origin; //Point in texture drawn at coords.
        public bool originOffset = true; //If the origin translation is drawn.
        public float angle = 0; //Rotation angle in radians.
        public float depth = 0; //Determines drawing order; smallest first.
        public Color color = Color.White; //Color blending.
        public float alpha = 1; //alpha blending (1 is opaque).
        public SpriteEffects spriteEffects; //Represents image mirroring.
        public float scaleX = 1, scaleY = 1; //a width and height multiplier.
        public SpriteDraw drawBehavior = SpriteDraw.basic; //Image drawing.
                       
        /// <summary>
        /// Initializes an empty sprite. The texture must be set before drawing
        /// to avoid throwing an exception.
        /// </summary>
        public Sprite()
        {
            texture = null;
            rectSrc = new SmoothRect(0, 0, 0, 0);
            rectDest = new SmoothRect(0, 0, 0, 0);
            origin = new Vector2(0, 0);
            spriteEffects = SpriteEffects.None;
        }

        /// <summary>
        /// Initializes a new sprite from an existing one. The texture is
        /// referenced.
        /// </summary>
        public Sprite(Sprite sprite)
        {
            rectSrc = new SmoothRect(sprite.rectSrc);
            rectDest = new SmoothRect(sprite.rectDest);
            texture = sprite.texture;
            origin = new Vector2(sprite.origin.X, sprite.origin.Y);
            originOffset = sprite.originOffset;
            angle = sprite.angle;
            depth = sprite.depth;
            color = sprite.color;
            alpha = sprite.alpha;
            spriteEffects = sprite.spriteEffects;
            scaleX = sprite.scaleX;
            scaleY = sprite.scaleY;
            drawBehavior = sprite.drawBehavior;
        }

        /// <summary>Initializes a new Sprite to control drawing.</summary>
        /// <param name="doSetDimensions">Whether or not to set the width
        /// and height based on the texture.</param>
        /// <param name="tex">The 2D texture to use.</param>
        public Sprite(
            bool doSetDimensions,
            Texture2D tex)
        {
            rectSrc = new SmoothRect(0, 0, 0, 0);
            rectDest = new SmoothRect(0, 0, 0, 0);
            SetTexture(doSetDimensions, tex);
            origin = new Vector2(0, 0);
            spriteEffects = SpriteEffects.None;
        }

        /// <summary>Includes source/dest rectangles.</summary>
        /// <param name="doSetDimensions">Whether or not to set the width and height based on the texture.</param>
        /// <param name="tex">The 2D texture to use.</param>
        /// <param name="rectSrc">The source rectangle (for spritesheets).</param>
        /// <param name="rectDest">The destination rectangle (for position and stretching). Scaled by default.</param>
        public Sprite(
            Texture2D tex,
            SmoothRect rectSrc,
            SmoothRect rectDest)
        {
            this.rectSrc = rectSrc;
            this.rectDest = rectDest;
            SetTexture(false, texture);
            origin = new Vector2(0, 0);
            spriteEffects = SpriteEffects.None;
        }

        /// <summary>
        /// Sets all parameters except the SpriteEffects used and scaling.
        /// </summary>
        /// <param name="rectSrc">The source rectangle (for spritesheets).</param>
        /// <param name="rectDest">The destination rectangle (for position and stretching). Scaled by default.</param>
        /// <param name="color">The Color object to be used.</param>
        /// <param name="angle">The angle of rotation, moving clockwise with right = 0, in radians.</param>
        /// <param name="depth">The order in which sprites are drawn. 0 is drawn first.</param>
        /// <param name="origin">Where (0,0) is located on the sprite in local coordinates.</param>
        public Sprite(
            Texture2D tex,
            SmoothRect rectSrc,
            SmoothRect rectDest,
            Vector2 origin,
            float angle,
            int depth,
            Color color
            )
        {
            this.rectSrc = rectSrc;
            this.rectDest = rectDest;
            SetTexture(false, texture);
            this.origin = origin;
            this.angle = angle;
            this.depth = depth;
            this.color = color;
            spriteEffects = SpriteEffects.None;
        }

        /// <summary>
        /// Sets all parameters + scaling, except the SpriteEffects used.
        /// </summary>
        /// <param name="rectSrc">
        /// The source rectangle (for spritesheets).
        /// </param>
        /// <param name="rectDest">
        /// The destination rectangle (for position and stretching). Scaled
        /// by default.
        /// </param>
        /// <param name="color">The Color object to be used.</param>
        /// <param name="angle">
        /// The angle of rotation, moving clockwise with right = 0, in radians.
        /// </param>
        /// <param name="depth">
        /// The order in which sprites are drawn. 0 is drawn first.
        /// </param>
        /// <param name="origin">
        /// Where (0,0) is located on the sprite in local coordinates.
        /// </param>
        public Sprite(
            Texture2D tex,
            SmoothRect rectSrc,
            SmoothRect rectDest,
            Vector2 origin,
            float angle,
            int depth,
            Color color,
            float alpha,
            float scaleX,
            float scaleY
            )
        {
            this.rectSrc = rectSrc;
            this.rectDest = rectDest;
            SetTexture(false, texture);
            this.origin = origin;
            this.angle = angle;
            this.depth = depth;
            this.color = color;
            this.alpha = alpha;
            spriteEffects = SpriteEffects.None;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
        }

        /// <summary>Sets the texture and resets scaling effects.</summary>
        /// <param name="doSetDimensions">
        /// Whether or not width and height are set as well.
        /// </param>
        /// <param name="texture">The 2D texture to use.</param>
        public void SetTexture(bool doSetDimensions, Texture2D texture)
        {
            this.texture = texture;
            if (doSetDimensions)
            {
                rectSrc.Width = texture.Width;
                rectSrc.Height = texture.Height;
                rectDest.Width = (int)(texture.Width * scaleX);
                rectDest.Height = (int)(texture.Height * scaleY);
            }
        }

        /// <summary>
        /// Centers the origin of the image.
        /// </summary>
        public void CenterOrigin()
        {
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        /// <summary>
        /// Centers the origin horizontally.
        /// </summary>
        public void CenterOriginHor()
        {
            origin = new Vector2(texture.Width / 2, origin.Y);
        }

        /// <summary>
        /// Centers the origin vertically.
        /// </summary>
        public void CenterOriginVert()
        {
            origin = new Vector2(origin.X, texture.Height / 2);
        }

        /// <summary>
        /// Checks for a bounding box intersection of two sprites. Takes
        /// scaling and origin into account. Ignores rotation.
        /// </summary>
        public static bool IsIntersecting(Sprite spr1, Sprite spr2)
        {
            //Creates smooth rects from the sprite destinations, which
            //already take scaling into account.
            SmoothRect tempRect1 = new SmoothRect(spr1.rectDest);
            SmoothRect tempRect2 = new SmoothRect(spr2.rectDest);

            //Adjusts the rectangles based on the origin.
            tempRect1.X -= spr1.origin.X;
            tempRect1.Y -= spr1.origin.Y;
            tempRect2.X -= spr2.origin.X;
            tempRect2.Y -= spr2.origin.Y;

            if (SmoothRect.IsIntersecting(tempRect1, tempRect2))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for a bounding box intersection of two sprites. Takes
        /// scaling and origin into account. Ignores rotation.
        /// </summary>
        public static bool IsIntersecting(Sprite spr1, SmoothRect rect2)
        {
            //Creates smooth rects from the sprite and another rectangle,
            //which already take scaling into account.
            SmoothRect tempRect1 = new SmoothRect(spr1.rectDest);
            SmoothRect tempRect2 = new SmoothRect(rect2);

            //Adjusts the rectangles based on the origin.
            if (!spr1.originOffset)
            {
                tempRect1.X -= spr1.origin.X;
                tempRect1.Y -= spr1.origin.Y;
            }

            if (SmoothRect.IsIntersecting(tempRect1, tempRect2))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not two sprites are colliding (pixel-perfect).
        /// Does not calculate rotated sprites.
        /// </summary>
        /// <param name="spr1">Bounding rectangle of the first sprite.</param>
        /// <param name="spr2">Bouding rectangle of the second sprite.</param>
        public static bool IsIntersectingPixels(Sprite sprA, Sprite sprB, MainLoop game)
        {
            //Sets up bounding rectangle data.
            Rectangle rect1 = sprA.rectDest.ToRect();
            Rectangle rect2 = sprB.rectDest.ToRect();

            //Adjusts the rectangles based on the origin.
            
            //Sets up texture color data.
            Color[] dataA =
                new Color[sprA.texture.Width * sprA.texture.Height];
            Color[] dataB =
                new Color[sprB.texture.Width * sprB.texture.Height];

            //Gets the texture color data.
            sprA.texture.GetData<Color>(dataA);
            sprB.texture.GetData<Color>(dataB);

            // Find the bounds of the rectangle intersection
            int top = Math.Max(rect1.Top, rect2.Top);
            int bottom = Math.Min(rect1.Bottom, rect2.Bottom);
            int left = Math.Max(rect1.Left, rect2.Left);
            int right = Math.Min(rect1.Right, rect2.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rect1.Left) +
                        (y - rect1.Top) * rect1.Width];
                    Color colorB = dataB[(x - rect2.Left) +
                        (y - rect2.Top) * rect2.Width];

                    // If the pixels pass a simple alpha check.
                    if (colorA.A > 2 && colorB.A > 2)
                    {
                        // Intersection found.
                        return true;
                    }
                }
            }

            // No intersection found.
            return false;
        }

        /// <summary>Draws the sprite with a SpriteBatch.</summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            float xPos = rectDest.ToRect().X;
            float yPos = rectDest.ToRect().Y;

            //Adjusts to remove origin while drawing if enabled.
            if (originOffset)
            {
                xPos += origin.X;
                yPos += origin.Y;
            }

            switch (drawBehavior)
            {
                case (SpriteDraw.basic):
                    spriteBatch.Draw(
                        texture,
                        new Rectangle(
                            (int)xPos,
                            (int)yPos,
                            (int)(rectDest.ToRect().Width * scaleX),
                            (int)(rectDest.ToRect().Height * scaleY)
                            ),
                        color * alpha);
                    break;
                case (SpriteDraw.basicAnimated):
                    spriteBatch.Draw(
                        texture,
                        new Rectangle(
                            (int)xPos,
                            (int)yPos,
                            (int)(rectDest.ToRect().Width * scaleX),
                            (int)(rectDest.ToRect().Height * scaleY)
                            ),
                        rectSrc.ToRect(),
                        color * alpha);
                    break;
                case (SpriteDraw.all):
                    spriteBatch.Draw(
                        texture,
                        new Rectangle(
                            (int)xPos,
                            (int)yPos,
                            (int)(rectDest.ToRect().Width * scaleX),
                            (int)(rectDest.ToRect().Height * scaleY)
                            ),
                        rectSrc.ToRect(),
                        color * alpha,
                        angle,
                        origin,
                        spriteEffects,
                        0);
                    break;
            }
        }
    }
}