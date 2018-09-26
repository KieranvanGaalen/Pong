using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        Texture2D Redplayer, Blueplayer, Ball, Redheart, Blueheart, Menu, RedWins, BlueWins; //Textures van de sprites.
        Vector2 RedplayerPosition, BlueplayerPosition, BallPosition, BlueHeartLocation, RedHeartLocation; //Vectoren voor de posities van de sprites.
        KeyboardState currentKeyboardState; //Status toetsenbord voor beweging paddles.
        Random Var = new Random(); //Random variable voor de beginsnelheden.
        double XRandom; //Random variabele voor de beginsnelheid van de bal, is of -1 of 1.
        double YRandom; //Random variabele voor de beginsnelheid van de bal, is of -1 of 1.
        double xbalposition; //De x-positie van de bal.
        double xbalvel; //Snelheid van de bal in de x-as.
        double ybalposition; //de y-positie van de bal.
        double ybalvel; //Snelheid van de bal in de y-as.
        double totalbalvel; //Totale snelheid van de bal, wordt gebruikt om te checken of er nog versneld mag worden.
        int RedPlayerY; //Ypositie van de rode speler (Y)
        int BluePlayerY; //Ypositie van de blauwe speler (Y)
        double BallMiddleY; //Midden van de bal (Y)
        double RedMiddleY; //Midden van de rode speler (Y)
        double BlueMiddleY; //Midden van de blauwe speler (Y)
        int Redlives = 3; //Rode Levens.
        int Bluelives = 3; //Blauwe Levens.
        SoundEffect BounceSound; //Stuitergeluidje.
        int Gamestate = 0;
        float menuscale;
       
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
            //Omdat hierboven de GraphicsDevice.Viewport.Height nog niet aangeroepen kan worden moet hier de waarde
            //van de RedPlayerY en BluePlayerY neergezet worden.
            RedPlayerY = GraphicsDevice.Viewport.Height/2 - 48;
            BluePlayerY = GraphicsDevice.Viewport.Height/2 - 48;
            ybalposition = GraphicsDevice.Viewport.Height/2 - 8;
            xbalposition = GraphicsDevice.Viewport.Width/2 - 8;

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
            graphics.PreferredBackBufferHeight = 450;
            graphics.ApplyChanges();
            menuscale = (float)GraphicsDevice.Viewport.Width/(float)3840;
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
            Redheart = Content.Load<Texture2D>("Rood hart");
            Blueheart = Content.Load<Texture2D>("Blauw hart");
            BounceSound = Content.Load<SoundEffect>("Boiiing");
            Menu = Content.Load<Texture2D>("MainMenu4k");
            RedWins = Content.Load<Texture2D>("RedWins4k");
            BlueWins = Content.Load<Texture2D>("BlueWins4k");
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit(); //als je op escape drukt sluit het spel.

            if (Keyboard.GetState().IsKeyDown(Keys.F) && graphics.IsFullScreen == false)
            {
                //graphics.ToggleFullScreen(); //Dit zorgt ervoor dat het spel uitgerekt wordt over het hele scherm en op die manier fullscreen wordt.
                //
                //De volgende 4 lines zorgen ervoor dat het spel op fullscreen gezet wordt zonder het uit te rekken.
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
                menuscale = (float)GraphicsDevice.Viewport.Width / (float)3840;
            } else if (Keyboard.GetState().IsKeyDown(Keys.F) && graphics.IsFullScreen == true)
            {
                //graphics.ToggleFullScreen(); //Dit zorgt ervoor dat het spel weer terug naar windowed mode gaat.
                //
                //De volgende 4 lines zorgen ervoor dat het spel weer terug gaat naar windowed mode. Het is niet de bedoeling om terug naar
                //windowed mode te gaan als je nog een spel aan het spelen bent, en de posities van de bal en peddels worden dus ook niet aangepast.
                graphics.PreferredBackBufferWidth = 800;
                graphics.PreferredBackBufferHeight = 450;
                graphics.IsFullScreen = false;
                graphics.ApplyChanges();
                menuscale = (float)GraphicsDevice.Viewport.Width/(float)3840;
            }

            base.Update(gameTime);
            if (Gamestate == 1)
            {
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
                    if (xbalposition >= GraphicsDevice.Viewport.Width + 20)
                    {
                        Bluelives -= 1;
                    }
                    ybalposition = GraphicsDevice.Viewport.Height / 2 - 8;
                    xbalposition = GraphicsDevice.Viewport.Width / 2 - 8;
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
                if (BlueMiddleY - BallMiddleY <= 56 && BlueMiddleY - BallMiddleY >= -56 &&
                    xbalposition >= GraphicsDevice.Viewport.Width - 42 && xbalposition <= GraphicsDevice.Viewport.Width - 13)
                { //Als de Y van de bal dicht genoeg bij de paddles zit, en de x zit ook in de paddels, stuiter
                    xbalvel = -xbalvel; //Omdraaien van de snelheid in de X, zodat hij terugstuitert
                    BounceSound.Play(); //Speelt een geluidje zodra de bal stuiterd.
                    xbalposition = GraphicsDevice.Viewport.Width - 57; //De x positie van de bal naar voor de paddle zetten zodat deze if maar 1x kan gebeuren
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
                    xbalvel = -xbalvel; //Omdraaien van de snelheid in de X, zodat hij terugstuitert.
                    BounceSound.Play(); //Speelt een geluidje zodra de bal stuiterd.
                    xbalposition = 27; //De x naar voor de paddle zetten zodat deze if maar 1x kan gebeuren
                    if (totalbalvel < 25) //Als de snelheid onder de maximumsnelheid ligt mag er versneld worden
                    {
                        if (ybalvel > 0) //De Y kan beide kanten opgaan dus er moet eerst gecheckt 
                        {                //worden of de verandering in snelheid + of - moet zijn
                            ybalvel += .5;
                        }
                        else
                        {
                            ybalvel -= .5;
                        }
                        xbalvel += .5;
                    }
                }

                if (ybalposition >= GraphicsDevice.Viewport.Height - 14 || ybalposition <= 0) //De Y stuiter mag als de bal bij de randen komt
                {
                    ybalvel = -ybalvel; //Hier geen versnelling, alleen maar omdraaien van de ybalvel
                }
                //Hier komt de keyboardinput voor de paddles van rood
                if (RedPlayerY < GraphicsDevice.Viewport.Height - 97 && currentKeyboardState.IsKeyDown(Keys.S)) //Dit wordt gebruikt om de beweging te limiteren
                {
                    RedPlayerY += 7;
                }
                if (RedPlayerY > 0 && currentKeyboardState.IsKeyDown(Keys.W))
                {
                    RedPlayerY -= 7;
                }
                //Hier komt de keyboardinput voor de paddles van blauw
                if (BluePlayerY < GraphicsDevice.Viewport.Height - 97 && currentKeyboardState.IsKeyDown(Keys.Down)) //Dit wordt gebruikt om de beweging te limiteren
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
                BlueplayerPosition = new Vector2(GraphicsDevice.Viewport.Width - 26, BluePlayerY);
            }
        }
        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);

            // TODO: Add your drawing code here
            //tekenen sprites
            spriteBatch.Begin();
            if (Gamestate == 0)
            {
                spriteBatch.Draw(Menu, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, menuscale, SpriteEffects.None, 0f);
            }
            if (Gamestate == 1)
            {
                spriteBatch.Draw(Redplayer, RedplayerPosition, Color.White);
                spriteBatch.Draw(Blueplayer, BlueplayerPosition, Color.White);
                spriteBatch.Draw(Ball, BallPosition, Color.White);
                for (int i = 0; i < Bluelives; i++)
                {
                    BlueHeartLocation = new Vector2(GraphicsDevice.Viewport.Width - (i + 1) * 13, 0);
                    spriteBatch.Draw(Blueheart, BlueHeartLocation, Color.White);
                }
                for (int i = 0; i < Redlives; i++)
                {
                    RedHeartLocation = new Vector2(i * 13, 0);
                    spriteBatch.Draw(Redheart, RedHeartLocation, Color.White);
                }
            }
            spriteBatch.End();
        
            base.Draw(gameTime);
        }
    }
}