using Microsoft.Xna.Framework.Graphics;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Represents a drawable graphic.
    /// </summary>
    interface ISprite
    {
        /// <summary>
        /// Draws the sprite to a sprite batcher, assuming that the batching
        /// has already been initialized.
        /// </summary>
        /// <param name="spriteBatch">The batching object to use.</param>
        void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// Sets the graphic to be used with the sprite (which should store
        /// the texture dimensions) and whether the dimensions should be
        /// cropped to the new texture.
        /// </summary>
        /// <param name="doSetDimensions">
        /// Whether the dimensions of the new image should be used or not.
        /// </param>
        /// <param name="texture">
        /// The texture to use.
        /// </param>
        void SetTexture(bool doSetDimensions, Texture2D texture);
    }
}
