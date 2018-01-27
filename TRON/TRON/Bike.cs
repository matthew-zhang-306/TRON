/* ********** *
 * TRON: Bike *
 *            *
 * By: myName *
 * Version: 1 *
 * ********** */

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
    /// The main type for a bike. Manages movement and trail creation.
    /// </summary>
    public class Bike
    {
        // CONSTANTS: change these!
        const int TRAIL_WIDTH = 6;
        const int BIKE_SPEED = 4;

        // BIKE ATTRIBUTES
        Texture2D bike;
        Rectangle rect;             // This is the on screen location of the body of the bike
        Rectangle rectDraw;         // This is the adjusted rectangle positioned to where it can be drawn at the desired on screen location
        Rectangle rectCollision;    // This is a smaller rectangle at the front of the bike, meant to detect head-on collision with other objects
        Vector2 origin;         // Refers to the center coordinate of the bike's texture
        int velocity;
        int direction; // NOTE: Direction should only ever be one of four values: 0, 90, 180, or 270. Each refers to right, down, left, and up, respectively.

        // TRAIL ATTRIBUTES
        Texture2D trail;
        Trail currentTrail;

        /// <summary>
        /// Constructor for a single bike.
        /// </summary>
        /// <param name="bike">The Texture2D with which the Bike will be drawn.</param>
        /// <param name="trail">The Texture2D with which all of the Bike's Trail objects will be drawn.</param>
        /// <param name="position">The initial rectangle with which the Bike will be drawn. Essentially a starting location.</param>
        /// <param name="direction">The direction of travel of the bike.</param>
        public Bike(Texture2D bike, Texture2D trail, Rectangle position, int direction)
        {
            // MOVEMENT VARIABLES
            velocity = BIKE_SPEED;
            this.direction = direction % 360;

            // DRAWING VARIABLES
            this.bike = bike;
            this.trail = trail;
            origin = new Vector2(bike.Width / 2, bike.Height / 2);
            rectDraw = position;

            // INITIALIZATION
            Turn(this.direction);
        }

        /// <summary>
        /// Like the Game1 Update, except specifically for the Bike.
        /// Allows the bike to run logic such as movement and
        /// planting its trail.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            // MOVEMENT
            rect = MoveRect(rect);
            rectDraw = MoveRect(rectDraw);
            rectCollision = MoveRect(rectCollision);

            // TRAIL EXTENSION
            currentTrail.ExtendRect(direction, velocity);
        }

        /// <summary>
        /// Shifts the position of a rectangle in the direction of the bike.
        /// </summary>
        /// <param name="r">The rectangle to move. The Bike has three of these.</param>
        /// <returns>A new Rectangle with the same size but locations shifted [velocity] pixels to the [direction].</returns>
        public Rectangle MoveRect(Rectangle r)
        {
            Rectangle o = r;
            double xtest = velocity * Math.Cos(MathHelper.ToRadians(direction));
            double ytest = velocity * Math.Sin(MathHelper.ToRadians(direction));
            o.X += (int)Math.Round(xtest);
            o.Y += (int)Math.Round(ytest);
            return o;
        }

        /// <summary>
        /// Allows the Bike to turn another direction. Can also serve as an initialization of important rectangles.
        /// </summary>
        /// <param name="d">The new direction of travel. Either 0, 90, 180, or 270, corresponding to right, down, left, and up, respectively.</param>
        public void Turn(int d)
        {
            bool validInput = true;

            // Sets the rectangles for each direction
            switch (d)
            {
                case 0:
                    rect = new Rectangle(rectDraw.X - rectDraw.Width / 2, rectDraw.Y - rectDraw.Height / 3, rectDraw.Width, (int)(rectDraw.Height / 1.5));
                    rectCollision = new Rectangle(rect.X + rect.Width - TRAIL_WIDTH, rect.Y, TRAIL_WIDTH, rect.Height);
                    break;
                case 90:
                    rect = new Rectangle(rectDraw.X - rectDraw.Height / 3, rectDraw.Y - rectDraw.Width / 2, (int)(rectDraw.Height / 1.5), rectDraw.Width);
                    rectCollision = new Rectangle(rect.X, rect.Y + rect.Height - TRAIL_WIDTH, rect.Width, TRAIL_WIDTH);
                    break;
                case 180:
                    rect = new Rectangle(rectDraw.X - rectDraw.Width / 2, rectDraw.Y - rectDraw.Height / 3, rectDraw.Width, (int)(rectDraw.Height / 1.5));
                    rectCollision = new Rectangle(rect.X, rect.Y, TRAIL_WIDTH, rect.Height);
                    break;
                case 270:
                    rect = new Rectangle(rectDraw.X - rectDraw.Height / 3, rectDraw.Y - rectDraw.Width / 2, (int)(rectDraw.Height / 1.5), rectDraw.Width);
                    rectCollision = new Rectangle(rect.X, rect.Y, rect.Width, TRAIL_WIDTH);
                    break;
                default:    // If direction is not a valid value, the method will do basically nothing
                    validInput = false;
                    break;
            }

            // Set direction and create a new trail
            if (validInput)
            {
                direction = d;
                currentTrail = new Trail(trail, new Rectangle(rectDraw.X - TRAIL_WIDTH / 2, rectDraw.Y - TRAIL_WIDTH / 2, TRAIL_WIDTH, TRAIL_WIDTH));
            }
        }

        /// <summary>
        /// This is called when the bike should draw itself. No variables are set or changed in this process.
        /// </summary>
        /// <param name="sb">The SpriteBatch from the game. Assumes the batch has already begun and that it will be ended after the method.</param>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            currentTrail.Draw(sb, gameTime);
            sb.Draw(bike, rectDraw, new Rectangle(0, 0, bike.Width, bike.Height), Color.White, MathHelper.ToRadians(direction), origin, SpriteEffects.None, 1f);
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
        public Rectangle Collision
        {
            get { return rectCollision; }
        }
        public Trail CurrentTrail
        {
            get { return currentTrail; }
        }
        public int Direction
        {
            get { return direction; }
        }

    }
}
