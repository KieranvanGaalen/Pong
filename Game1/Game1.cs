using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Color mycolor = new Color(255,255,255);
        Texture2D Redplayer, Blueplayer, Ball;
        Vector2 RedplayerPosition, BlueplayerPosition, BallPosition;
        KeyboardState currentKeyboardState;
        //snelheden zijn in pixels per frame
        double xbalposition = 396;
        double xbalvel = 1;
        double ybalposition = 236;
        double ybalvel = 1;
        double totalbalvel;
        double Sqrt2 = System.Math.Sqrt(2);
        int RedPlayerY = 196;
        int BluePlayerY = 196;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            // TODO: use this.Content to load your game content here
            Redplayer = Content.Load<Texture2D>("rodeSpeler");
            Blueplayer = Content.Load<Texture2D>("blauweSpeler");
            Ball = Content.Load<Texture2D>("bal");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
            currentKeyboardState = Keyboard.GetState();
            
            //Snelheid en richting bal berekenen
            xbalposition += xbalvel;
            ybalposition += ybalvel;
            totalbalvel = Sqrt2 * xbalvel;
            
            //Het stuiteren van de bal wordt hier aangegeven
            if (xbalposition >= 758 || xbalposition <= 26) //Als de bal de randen raakt
            {
                if (totalbalvel < 25) //Als de snelheid onder het maximum ligt
                {                   
                    if (ybalvel > 0) //De Y kan beide kanten opgaan dus de verandering
                    {                //in snelheid moet eerst gecheckt worden of het + of - moet zijn
                        ybalvel += 0.5;
                    }
                    else
                    {
                        ybalvel -= 0.5;
                    }
                    if (xbalvel > 0)
                    {
                        xbalvel += 0.5;
                    }
                    else
                    {
                        xbalvel -= 0.5;
                    }
                }
                xbalvel = -xbalvel; //Omdraaien van de snelheid in de X, zodat hij terugstuitert
            }
            if (ybalposition >= 466 || ybalposition <= 0) //Hier hetzelfde maar dan voor Y
            {
                ybalvel = -ybalvel;
            }
            //Hier komt de keyboardinput voor de paddles van rood
            if (RedPlayerY < 384) //Dit wordt gebruikt om de beweging te limiteren
            {
                if (currentKeyboardState.IsKeyDown(Keys.S))
                {
                    RedPlayerY += 7;
                }
            }
            if (RedPlayerY > 0)
            {
                if (currentKeyboardState.IsKeyDown(Keys.W))
                {
                    RedPlayerY -= 7;
                }
            }
            //Hier komt de keyboardinput voor de paddles van blauw
            if (BluePlayerY < 384)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Down))
                {
                    BluePlayerY += 7;
                }
            }
            if (BluePlayerY > 0)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Up))
                {
                    BluePlayerY -= 7;
                }
            }

            BallPosition = new Vector2((int)xbalposition, (int)ybalposition);
            RedplayerPosition = new Vector2(10, RedPlayerY);
            BlueplayerPosition = new Vector2(774, BluePlayerY);
        }
        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(Redplayer, RedplayerPosition, Color.White);
            spriteBatch.Draw(Blueplayer, BlueplayerPosition, Color.White);
            spriteBatch.Draw(Ball, BallPosition, Color.White);
            spriteBatch.End();
        
            base.Draw(gameTime);
        }
    }
}
//test of het goed werkt