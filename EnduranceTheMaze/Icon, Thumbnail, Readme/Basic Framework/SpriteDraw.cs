using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpossiMaze
{
        /// <summary>
        /// Defines how a Sprite is drawn.
        /// basic: texture, position, and color are drawn.
        /// basicAnimated: texture, position, source pos, and color are drawn.
        /// all: everything above and angle, origin, spriteEffects, and depth are drawn.
        /// </summary>
        public enum SpriteDraw { basic, basicAnimated, all };
}
