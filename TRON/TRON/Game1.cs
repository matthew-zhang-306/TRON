/* *********** *
 * TRON: Game1 *
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
    /// The main type for the game. Manages in game behavior.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // GRAPHICS ESSENTIALS
        GraphicsDeviceManager graphics;
        SpriteBatch sb;

        // CONSTANTS: change these!
        const int WIDTH = 1080;
        const int HEIGHT = 720;
        const int BIKE_WIDTH = 36;
        const int BIKE_HEIGHT = 12;
        const int WALL_WIDTH = 5;
        const int COUNTDOWN_LENGTH = 3;
        const int BLINK_SPEED = 60;
        const string pressspace = "PRESS SPACE";
        const string instructions = "     Use WASD to control blue\nUse arrow keys to control orange\n    If you hit a wall you lose!";
        const string BLUE_BIKE_NAME = "blue";
        const string ORANGE_BIKE_NAME = "orange";
        const string BLUE_TRAIL_NAME = "b";
        const string ORANGE_TRAIL_NAME = "o";
        const string WALL_NAME = "w";
        const string LARGE_FONT = "SpriteFont1";
        const string SMALL_FONT = "SpriteFont2";
        const Keys ADVANCE_KEY = Keys.Space;

        // OBJECTS
        Random r;
        Bike blue, orange;
        Texture2D white;
        List<Trail> trails;
        
        // TEXT DISPLAY
        int timer;              // Counter for the countdown stage 
        int blinktimer;         // Counter for the blinking text
        String display;
        SpriteFont sf;      // Large font
        SpriteFont sf2;     // Small font

        // GAME STATE
        enum GameState { START, COUNTDOWN, PLAY, WIN1, WIN2 }
        GameState gameState;

        // INPUT
        KeyboardState oldkb;
        Keys[] blueinputs = { Keys.D, Keys.S, Keys.A, Keys.W };
        Keys[] orangeinputs = { Keys.Right, Keys.Down, Keys.Left, Keys.Up };

        /// <summary>
        /// Game1 constructor. Manages essential graphics setup stuff.
        /// </summary>
        public Game1()
        {
            // GRAPHICS
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // SCREEN SIZE
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Some variables are initialized here. Others are done in LoadContent because they require content.
            r = new Random();
            timer = 0;
            display = "";
            blinktimer = 0;
            oldkb = Keyboard.GetState();
            gameState = GameState.START;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place where all content is loaded.
        /// </summary>
        protected override void LoadContent()
        {
            // SPRITEBATCH
            sb = new SpriteBatch(GraphicsDevice);

            // LOADING CONTENT
            sf = Content.Load<SpriteFont>(LARGE_FONT);
            sf2 = Content.Load<SpriteFont>(SMALL_FONT);
            white = Content.Load<Texture2D>(WALL_NAME);
            InitBikes();
        }

        /// <summary>
        /// InitBikes will be called each time the game starts or restarts. It refreshes all relevant game objects.
        /// </summary>
        protected void InitBikes()
        {
            int side = r.Next(2);

            // BIKES
            blue = new Bike(Content.Load<Texture2D>(BLUE_BIKE_NAME), Content.Load<Texture2D>(BLUE_TRAIL_NAME), GetRandomRect(side), 180 * side);
            orange = new Bike(Content.Load<Texture2D>(ORANGE_BIKE_NAME), Content.Load<Texture2D>(ORANGE_TRAIL_NAME), GetRandomRect(side + 1), 180 * (side + 1));

            // WALLS AND TRAILS
            trails = new List<Trail>();
            trails.Add(blue.CurrentTrail);
            trails.Add(orange.CurrentTrail);
            trails.Add(new Trail(white, new Rectangle(-1 * WALL_WIDTH, -1 * WALL_WIDTH, WALL_WIDTH * 2, HEIGHT + WALL_WIDTH * 2)));     // left wall
            trails.Add(new Trail(white, new Rectangle(WIDTH - WALL_WIDTH, -1 * WALL_WIDTH, WALL_WIDTH * 2, HEIGHT + WALL_WIDTH * 2)));  // right wall
            trails.Add(new Trail(white, new Rectangle(-1 * WALL_WIDTH, -1 * WALL_WIDTH, WIDTH + WALL_WIDTH * 2, WALL_WIDTH * 2)));      // top wall
            trails.Add(new Trail(white, new Rectangle(-1 * WALL_WIDTH, HEIGHT - WALL_WIDTH, WIDTH + WALL_WIDTH * 2, WALL_WIDTH * 2)));  // bottom wall

            // COUNTDOWN TIMER
            timer = COUNTDOWN_LENGTH * 60;
        }

        /// <summary>
        /// Generates a new random rectangle for a new Bike on a single side of the arena, given by the parameter.
        /// </summary>
        /// <param name="side">The starting side of the bike. Even numbers signify the left side, odd numbers signify the right side.</param>
        /// <returns>A Rectangle with the center location of a Bike and its size. Meant to be passed into a Bike constructor for use.</returns>
        protected Rectangle GetRandomRect(int side)
        {
            int x = side % 2 == 0 ? BIKE_WIDTH / 2 + WALL_WIDTH : WIDTH - BIKE_WIDTH / 2 - WALL_WIDTH;
            int y = r.Next(HEIGHT - 2 * WALL_WIDTH - 2 * BIKE_HEIGHT) + BIKE_HEIGHT + WALL_WIDTH;

            return new Rectangle(x, y, BIKE_WIDTH, BIKE_HEIGHT);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Nothing to unload
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, and gathering input.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();

            // EXIT
            if (kb.IsKeyDown(Keys.Escape))
                this.Exit();

            // BLINKING TIMER     
            blinktimer = (blinktimer + 1) % (BLINK_SPEED * 2);

            // MAIN SWITCH
            switch (gameState)
            {
                case GameState.START:           // TITLE SCREEN
                    display = "TRON";
                    if (kb.IsKeyDown(ADVANCE_KEY) && oldkb.IsKeyUp(ADVANCE_KEY))
                    {
                        gameState = GameState.COUNTDOWN;
                    }
                    break;

                case GameState.COUNTDOWN:       // COUNTDOWN BEFORE GAMEPLAY
                    timer--;
                    display = timer / 60 + 1 + "";

                    if (timer == 0)
                    {
                        display = "";
                        gameState = GameState.PLAY;
                    }

                    break;
        
                case GameState.PLAY:            // GAMEPLAY
                    // Checks for turns
                    for (int i = 0; i < blueinputs.Length; i++)
                    {
                        if (kb.IsKeyDown(blueinputs[i]) && oldkb.IsKeyUp(blueinputs[i]) && blue.Direction % 180 == (i % 2 == 0 ? 90 : 0))   // (Note: the direction check makes sure that the player can't turn directly backwards)
                        {
                            blue.Turn(90 * i);
                            trails.Add(blue.CurrentTrail);
                        }
                        if (kb.IsKeyDown(orangeinputs[i]) && oldkb.IsKeyUp(orangeinputs[i]) && orange.Direction % 180 == (i % 2 == 0 ? 90 : 0))
                        {
                            orange.Turn(90 * i);
                            trails.Add(orange.CurrentTrail);
                        }
                    }

                    // Bike behavior
                    blue.Update(gameTime);
                    orange.Update(gameTime);

                    // Checks for collisions
                    foreach (Trail trail in trails)
                    { 
                        if (trail != blue.CurrentTrail && trail.Rect.Intersects(blue.Collision))
                            gameState = GameState.WIN2;
                        else if (trail != orange.CurrentTrail && trail.Rect.Intersects(orange.Collision))
                            gameState = GameState.WIN1;
                    } // (Note that if both bikes crash into the same trail on the same frame, the orange bike will be favored. Also note that if both bikes crash head on into each other, the blue bike will be favored.)
                    if (blue.Collision.Intersects(orange.Rect))
                        gameState = GameState.WIN2;
                    if (orange.Collision.Intersects(blue.Rect))
                        gameState = GameState.WIN1;

                    break;

                case GameState.WIN1:            // GAME OVER
                case GameState.WIN2:
                    display = (gameState == GameState.WIN1 ? "BLUE" : "ORANGE") + " wins!";

                    if (kb.IsKeyDown(ADVANCE_KEY) && oldkb.IsKeyUp(ADVANCE_KEY))
                    {
                        gameState = GameState.COUNTDOWN;
                        InitBikes();
                    }

                    break;
            }

            oldkb = kb;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself. No variables are set or changed in this process.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            sb.Begin();
                // WALLS AND TRAILS
                foreach (Trail trail in trails)
                    trail.Draw(sb, gameTime);
                // BIKES
                blue.Draw(sb, gameTime);
                orange.Draw(sb, gameTime);
                // TEXT
                sb.DrawString(sf, display, CenterString(sf, display, Vector2.Zero), Color.White);
                if (gameState == GameState.START || gameState == GameState.WIN1 || gameState == GameState.WIN2)
                    sb.DrawString(sf2, pressspace, CenterString(sf2, pressspace, new Vector2(0,sf2.LineSpacing)), // offset includes a single line space so that the text appears underneath the bigger words
                        blinktimer / BLINK_SPEED == 0 ? Color.White : new Color(0,0,0,0)); // ternary operation controls blinking
                if (gameState == GameState.START)
                    sb.DrawString(sf2, instructions, CenterString(sf2, instructions, new Vector2(0, -1 * HEIGHT / 4)), Color.White);

            sb.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Returns a position on the screen that can be used to draw text with center alignment.
        /// </summary>
        /// <param name="spritefont">The spritefont with which to measure the text, and with which the text should be drawn for this to work.</param>
        /// <param name="text">The word or phrase desired to be drawn to the screen.</param>
        /// <param name="offset">A Vector2 indicating a possible desired shift from the center. Use Vector2.Zero if the text should be perfectly centered.</param>
        /// <returns>A Vector2 position on the screen at which if a given text is drawn with a given spritefont, the text will be centered, along with a certain offset.</returns>
        private Vector2 CenterString(SpriteFont spritefont, String text, Vector2 offset)
        {
            Vector2 centerScreen = new Vector2(WIDTH / 2, HEIGHT / 2);
            return centerScreen - (spritefont.MeasureString(text) / 2) + offset;
        }
    }
}
