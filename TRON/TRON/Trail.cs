/* *********** *
 * TRON: Trail *
 *             *
 * By: myName  *
 * Version: 1  *
 * *********** */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace TRON
{
    /// <summary>
    /// The main type for the trail. Manages the length of the trail.
    /// </summary>
    public class Trail
    {
        // DRAWING
        Texture2D text;
        Rectangle rect;

        /// <summary>
        /// Constructor for a single trail.
        /// </summary>
        /// <param name="text">The Texture2D with which the Trail will be drawn.</param>
        /// <param name="rect">The Rectangle with which the Trail will be drawn.</param>
        public Trail(Texture2D text, Rectangle rect)
        {
            this.text = text;
            this.rect = rect;
        }

        /// <summary>
        /// Allows the trail to elongate in a single direction.
        /// </summary>
        /// <param name="direction">The side to extend. Either 0, 90, 180, or 270, corresponding to right, bottom, left, and top, respectively.</param>
        /// <param name="velocity"></param>
        public void ExtendRect(int direction, int velocity)
        {
            if (direction % 180 == 0)   // LEFT OR RIGHT
            {
                rect.Width += velocity;
                if (direction == 180)
                    rect.X -= velocity;
            }
            else                        // TOP OR BOTTOM
            {
                rect.Height += velocity;
                if (direction == 270)
                    rect.Y -= velocity;
            }
        }

        /// <summary>
        /// This is called when the trail should draw itself. No variables are set or changed in this process.
        /// </summary>
        /// <param name="sb">The SpriteBatch from the game. Assumes the batch has already begun and that it will be ended after the method.</param>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            sb.Draw(text, rect, Color.White);
        }



        /*
         * 
         *  PUBLIC GETTER VARIABLES
         *
         * */
        public Rectangle Rect
        {
            get { return rect; }
        }
    }
}
