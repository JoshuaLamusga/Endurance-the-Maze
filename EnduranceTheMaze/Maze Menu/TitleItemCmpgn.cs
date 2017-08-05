using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Represents a button, recording hovering and clicking for
    /// inheriting classes to perform actions. Depends on TitleItemMain.cs.
    /// </summary>
    public class TitleItemCmpgn : TitleItemMain
    {
        /// <summary>
        /// Sets up a new button object.
        /// 
        /// </summary>
        /// <param name="xPos">The x-location.</param>
        /// <param name="yPos">The y-location.</param>
        /// <param name="frame">
        /// The frame to use.
        /// </param>
        public TitleItemCmpgn(MainLoop game, Texture2D tex, float xPos,
            float yPos, int frame) : base(game, tex, xPos, yPos, frame)
        {
            BttnSpriteAtlas = new SpriteAtlas(BttnSprite, 96, 96, 8, 2, 4);
            BttnSpriteAtlas.frame = frame;
        }

        /// <summary>
        /// Loads and returns the relevant graphics into memory. Hides
        /// inherited member by the same name.
        /// </summary>
        new public static Texture2D LoadContent(ContentManager Content)
        {
            return Content.Load<Texture2D>("Content/Sprites/Gui/sprBttnCmpgn");
        }
    }
}
