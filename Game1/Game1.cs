using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Color mycolor = new Color(255, 255, 255); //Achtergrondkleur
        Texture2D Redplayer, Blueplayer, Ball; //Textures van de sprites
        Vector2 RedplayerPosition, BlueplayerPosition, BallPosition; //Vectoren voor de posities van de sprites
        KeyboardState currentKeyboardState; //Status toetsenbord voor beweging paddles
        Random Var = new Random(); //Random variable voor de beginsnelheden
        double XRandom; //Random variabele voor de beginsnelheden, is of -1 of 1
        double YRandom; //Random variabele voor de beginsnelheden, is of -1 of 1
        double xbalposition = 396; //De x-positie van de bal, begint op 396
        double xbalvel; //Snelheid van de bal in de x-as
        double ybalposition = 236; //de y-positie van de bal, begint op 236
        double ybalvel; //Snelheid van de bal in de y-as
        double totalbalvel; //Totale snelheid van de bal, wordt gebruikt om te checken of er nog versneld mag worden
        int RedPlayerY = 196; //Beginpositie van de rode speler (Y)
        int BluePlayerY = 196; //Beginpositie van de blauwe speler (Y)
        double BallMiddleY; //Midden van de bal (Y)
        double RedMiddleY; //Midden van de rode speler (Y)
        double BlueMiddleY; //Midden van de blauwe speler (Y)
        int Redlives = 3;
        int Bluelives = 3;
       
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
            //Hier wordt de X en Y snelheid tussen -1 en 1 gerandomized
            XRandom = Var.Next(-1, 2);
            YRandom = Var.Next(-1, 2);
            while (XRandom == 0) //Als het 0 is zal het spel niet werken, dus moet het niet 0 zijn, oftewel -1 of 1
            {
                XRandom = Var.Next(-1, 2);
            }

            while (YRandom == 0)
            {
                YRandom = Var.Next(-1, 2);
            }
            xbalvel = 2 * XRandom; //Hier wordt de snelheid gemaakt door de random*2 te doen zodat de beginsnelheid of -2 of 2 is
            ybalvel = 2 * YRandom;
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
            currentKeyboardState = Keyboard.GetState(); //Kijkt welke toetsen ingedrukt zijn
            //Hier wordt voor de hitbox het midden (Y) van de sprites berekent
            BallMiddleY = ybalposition + 8;
            RedMiddleY = RedPlayerY + 48;
            BlueMiddleY = BluePlayerY + 48;
            
            //Snelheid en richting bal berekenen
            xbalposition += xbalvel;
            ybalposition += ybalvel;
            totalbalvel = System.Math.Sqrt(2) * Math.Abs(xbalvel);

            //Als de bal achter de paddels komt reset hij
            if (xbalposition >= GraphicsDevice.Viewport.Width + 20 || xbalposition <= -20)
            {
                if (xbalposition <= -20)
                {
                    Redlives -= 1;
                }
                if (xbalposition >= >= GraphicsDevice.Viewport.Width + 20)
                {
                    Bluelives -= 1;
                }
                xbalposition = 396;
                ybalposition = 236;
                XRandom = Var.Next(-1, 2);
                YRandom = Var.Next(-1, 2);
                while (XRandom == 0)
                {
                    XRandom = Var.Next(-1, 2);
                }

                while (YRandom == 0)
                {
                    YRandom = Var.Next(-1, 2);
                }
                xbalvel = 2 * XRandom;
                ybalvel = 2 * YRandom;
            }

            //Het stuiteren van de bal wordt hier aangegeven
            if (BlueMiddleY - BallMiddleY <= 56 && BlueMiddleY - BallMiddleY >= -56 && xbalposition >= 758 && xbalposition <= 787)
            { //Als de Y van de bal dicht genoeg bij de paddles zit, en de x zit ook in de paddels, stuiter
                xbalvel = -xbalvel; //Omdraaien van de snelheid in de X, zodat hij terugstuitert
                xbalposition = 757; //De x naar voor de paddle zetten zodat deze if maar 1x kan gebeuren
                if (totalbalvel < 25) //Als de snelheid onder de maximumsnelheid ligt mag er versneld worden
                {
                    if (ybalvel > 0) //De Y kan beide kanten opgaan dus de verandering
                    {                //in snelheid moet eerst gecheckt worden of het + of - moet zijn
                        ybalvel += .5;
                    }
                    else
                    {
                        ybalvel -= .5;
                    }
                    xbalvel -= .5;
                }

            }
            if (RedMiddleY - BallMiddleY <= 56 && RedMiddleY - BallMiddleY >= -56 && xbalposition <= 26 && xbalposition >= -1)
            { //Als de Y van de bal dicht genoeg bij de paddles zit, en de x zit ook in de paddels, stuiter
                xbalvel = -xbalvel; //Omdraaien van de snelheid in de X, zodat hij terugstuitert
                xbalposition = 27; //De x naar voor de paddle zetten zodat deze if maar 1x kan gebeuren
                if (totalbalvel < 25) //Als de snelheid onder de maximumsnelheid ligt mag er versneld worden
                {
                    if (ybalvel > 0) //De Y kan beide kanten opgaan dus de verandering
                    {                //in snelheid moet eerst gecheckt worden of het + of - moet zijn
                        ybalvel += .5;
                    }
                    else
                    {
                        ybalvel -= .5;
                    }
                    xbalvel += .5;
                }
            }
            
            if (ybalposition >= 466 || ybalposition <= 0) //De Y stuiter mag als de bal bij de randen komt
            {
                ybalvel = -ybalvel; //Hier geen versnelling, alleen maar omdraaien van de ybalvel
            }
            //Hier komt de keyboardinput voor de paddles van rood
            if (RedPlayerY < 384 && currentKeyboardState.IsKeyDown(Keys.S)) //Dit wordt gebruikt om de beweging te limiteren
            {
                RedPlayerY += 7;
            }
            if (RedPlayerY > 0 && currentKeyboardState.IsKeyDown(Keys.W))
            {
                RedPlayerY -= 7;
            }
            //Hier komt de keyboardinput voor de paddles van blauw
            if (BluePlayerY < 384 && currentKeyboardState.IsKeyDown(Keys.Down)) //Dit wordt gebruikt om de beweging te limiteren
            {
                BluePlayerY += 7;
            }
            if (BluePlayerY > 0 && currentKeyboardState.IsKeyDown(Keys.Up))
            {
                BluePlayerY -= 7;
            }
            //Berekenen positie van de sprites voor het tekenen
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
            //tekenen sprites
            spriteBatch.Begin();
            spriteBatch.Draw(Redplayer, RedplayerPosition, Color.White);
            spriteBatch.Draw(Blueplayer, BlueplayerPosition, Color.White);
            spriteBatch.Draw(Ball, BallPosition, Color.White);
            spriteBatch.End();
        
            base.Draw(gameTime);
        }
    }
}