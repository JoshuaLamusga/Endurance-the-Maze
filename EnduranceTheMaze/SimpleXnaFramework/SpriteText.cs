using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EnduranceTheMaze
{
    public class SpriteText
    {
        //text, position, color, rotation, scale, origin, sprite effects, depth

        public SpriteFont font; //The font to be used.
        public string text; //The text to be displayed.
        public Vector2 position, origin; //Position and origin.
        public bool originOffset; //If the origin translation is drawn.
        public float angle = 0; //Rotation angle in radians.
        public float depth = 0; //Determines drawing order; smallest first.
        public Color color; //Color blending (default Color.White).
        public float alpha; //alpha blending (default 1).
        public SpriteEffects spriteEffects; //Represents image mirroring.
        public float scale = 1; //a width and height multiplier.
        public SpriteDraw drawBehavior = SpriteDraw.basic; //Image drawing.
                       
        /// <summary>
        /// Initializes an empty text sprite. The font must be set before 
        ///  drawing to avoid throwing an exception.
        /// </summary>
        public SpriteText()
        {
            text = "";
            position = Vector2.Zero;
            origin = Vector2.Zero;
            originOffset = false;
            color = Color.White;
            alpha = 1;
            spriteEffects = SpriteEffects.None;
        }

        /// <summary>
        /// Initializes a new text sprite with a font and text.
        /// </summary>
        public SpriteText(SpriteFont font, string text)
        {
            this.font = font;
            this.text = text;
            position = Vector2.Zero;
            origin = Vector2.Zero;
            originOffset = false;
            color = Color.White;
            alpha = 1;
            spriteEffects = SpriteEffects.None;
        }

        /// <summary>Sets the font to use.</summary>
        public void SetFont(SpriteFont font)
        {
            this.font = font;
        }

        /// <summary>
        /// Centers the origin of the text.
        /// </summary>
        public void CenterOrigin()
        {
            origin = new Vector2(
                font.MeasureString(text).X / 2,
                font.MeasureString(text).Y / 2);
        }

        /// <summary>
        /// Centers the origin horizontally.
        /// </summary>
        public void CenterOriginHor()
        {
            origin = new Vector2(font.MeasureString(text).X / 2, origin.Y);
        }

        /// <summary>Draws the sprite with a SpriteBatch.</summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            float xPos = position.X;
            float yPos = position.Y;

            //Adjusts to remove origin while drawing if enabled.
            if (originOffset)
            {
                xPos += origin.X;
                yPos += origin.Y;
            }

            if (drawBehavior == SpriteDraw.basic)
            {
                spriteBatch.DrawString(font, text, position, color);
            }
            else
            {
                spriteBatch.DrawString(font, text, position, color * alpha,
                    angle, origin, scale, spriteEffects, depth);
            }
        }
    }
}