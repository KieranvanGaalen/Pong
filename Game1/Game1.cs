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
        Texture2D Redplayer, Blueplayer, Ball, Redheart, Blueheart, Menu, RedWins, BlueWins, Pause, Player, SPower; //Textures van de sprites.
        Vector2 RedplayerPosition, BlueplayerPosition, BallPosition, BlueHeartLocation, RedHeartLocation, YellowplayerPosition, GreenplayerPosition; //Vectoren voor de posities van de sprites.
        KeyboardState currentKeyboardState, lastkeyboardstate; //Status toetsenbord voor beweging paddles.
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
        int GreenPlayerX;
        int YellowPlayerX;
        double BallMiddleY; //Midden van de bal (Y)
        double RedMiddleY; //Midden van de rode speler (Y)
        double BlueMiddleY; //Midden van de blauwe speler (Y)
        int Redlives = 3; //Rode Levens.
        int Bluelives = 3; //Blauwe Levens.
        SoundEffect BounceSound; //Stuitergeluidje.
        int Gamestate;
        float menuscale;
        int lastgamestate;
        int SPEEDPOWER;
        int SPEEDy;
        int SPEEDx;

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
            Reset();
            Gamestate = 0;
            graphics.PreferredBackBufferHeight = 450;
            graphics.ApplyChanges();
            menuscale = GraphicsDevice.Viewport.Width/(float)3840;
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
            Pause = Content.Load<Texture2D>("PauseScreen4k");
            Player = Content.Load<Texture2D>("Speler");
            SPower = Content.Load<Texture2D>("Speed");
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

            lastkeyboardstate = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            lastgamestate = Gamestate;

            if (Gamestate == 0)
            {
                if(Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    Gamestate = 2;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Escape) && lastkeyboardstate.IsKeyUp(Keys.Escape))
                    Exit(); //als je op escape drukt sluit het spel.
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Reset();
                }

                    
                if (Keyboard.GetState().IsKeyDown(Keys.F) && graphics.IsFullScreen == false)
                {
                    //De volgende 4 lines zorgen ervoor dat het spel op fullscreen gezet wordt zonder het uit te rekken.
                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    graphics.IsFullScreen = true;
                    graphics.ApplyChanges();
                    menuscale = GraphicsDevice.Viewport.Width / (float)3840;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.F) && graphics.IsFullScreen == true)
                {
                    //De volgende 4 lines zorgen ervoor dat het spel weer terug gaat naar windowed mode. Het is niet de bedoeling om terug naar
                    //windowed mode te gaan als je nog een spel aan het spelen bent, en de posities van de bal en peddels worden dus ook niet aangepast.
                    graphics.PreferredBackBufferWidth = 800;
                    graphics.PreferredBackBufferHeight = 450;
                    graphics.IsFullScreen = false;
                    graphics.ApplyChanges();
                    menuscale = GraphicsDevice.Viewport.Width / (float)3840;
                }
            }

            if (Gamestate == 1)
            {
                                                            //Hier wordt voor de hitbox het midden (Y) van de sprites berekent
                BallMiddleY = ybalposition + 8;
                RedMiddleY = RedPlayerY + 48;
                BlueMiddleY = BluePlayerY + 48;

                //pauze
                if (Keyboard.GetState().IsKeyDown(Keys.P) && lastkeyboardstate.IsKeyUp(Keys.P) && lastgamestate != 5)
                {
                    Gamestate = 5;
                }
                //Snelheid en richting bal berekenen
                xbalposition += xbalvel;
                ybalposition += ybalvel;
                totalbalvel = System.Math.Sqrt(2) * Math.Abs(xbalvel);

                if (Keyboard.GetState().IsKeyDown(Keys.Escape)) //Als je op esc drukt ga je terug naar het menu
                {
                    Gamestate = 0;
                }

                SpeedPowerUp();
                
                //Als de bal achter de paddels komt reset hij
                if (xbalposition >= GraphicsDevice.Viewport.Width + 20 || xbalposition <= -20)
                {
                    if (xbalposition <= -20)
                    {
                        Redlives -= 1;
                        if (Redlives == 0)
                        {
                            Gamestate = 2;
                        }
                    }
                    if (xbalposition >= GraphicsDevice.Viewport.Width + 20)
                    {
                        Bluelives -= 1;
                        if (Bluelives == 0)
                        {
                            Gamestate = 3;
                        }
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
            if (Gamestate == 2)
            {
                {
                    //Hier wordt voor de hitbox het midden (Y) van de sprites berekent
                    BallMiddleY = ybalposition + 8;
                    RedMiddleY = RedPlayerY + 48;
                    BlueMiddleY = BluePlayerY + 48;

                    //pauze
                    if (Keyboard.GetState().IsKeyDown(Keys.P) && lastkeyboardstate.IsKeyUp(Keys.P) && lastgamestate != 5)
                    {
                        Gamestate = 5;
                    }
                    //Snelheid en richting bal berekenen
                    xbalposition += xbalvel;
                    ybalposition += ybalvel;
                    totalbalvel = System.Math.Sqrt(2) * Math.Abs(xbalvel);

                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        Gamestate = 0;
                    }

                    //Als de bal achter de paddels komt reset hij
                    if (xbalposition >= GraphicsDevice.Viewport.Width + 20 || xbalposition <= -20)
                    {
                        if (xbalposition <= -20)
                        {
                            Redlives -= 1;
                            if (Redlives == 0)
                            {
                                Gamestate = 2;
                            }
                        }
                        if (xbalposition >= GraphicsDevice.Viewport.Width + 20)
                        {
                            Bluelives -= 1;
                            if (Bluelives == 0)
                            {
                                Gamestate = 3;
                            }
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
                    GreenplayerPosition = new Vector2(GreenPlayerX, GraphicsDevice.Viewport.Height - 10);
                    YellowplayerPosition = new Vector2(YellowPlayerX, 10);
                }
            }
            if (Gamestate == 3)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Reset();
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    Gamestate = 0;
                }
            }
            if (Gamestate == 4)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Reset();
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    Gamestate = 0;
                }
            }
            if (Gamestate ==  5)
            {
                if (currentKeyboardState.IsKeyDown(Keys.P) && lastkeyboardstate.IsKeyUp(Keys.P) && lastgamestate != 1)
                {
                    Gamestate = 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    Gamestate = 0;
                }
            }


            base.Update(gameTime);
        }
        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);
            //tekenen sprites
            spriteBatch.Begin();
            if (Gamestate == 0)
            {
                spriteBatch.Draw(Menu, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, menuscale, SpriteEffects.None, 0f);
            }
            if (Gamestate == 1 || Gamestate == 5 || Gamestate == 2)
            {
                spriteBatch.Draw(Redplayer, RedplayerPosition, Color.White);
                spriteBatch.Draw(Blueplayer, BlueplayerPosition, Color.White);
                spriteBatch.Draw(Ball, BallPosition, Color.White);
                if (SPEEDPOWER == -1) //Alleen als er een speedobject moet zijn.
                {
                    spriteBatch.Draw(SPower, new Vector2(SPEEDx, SPEEDy), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
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
            if (Gamestate == 2)
            {
                spriteBatch.Draw(Player, YellowplayerPosition, null, Color.Yellow, (float)3.14159265358979323846264338327950288 / 2, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(Player, GreenplayerPosition, null, Color.Green, (float)-3.14159265358979323846264338327950288 / 2, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            if (Gamestate == 3)
            {
                spriteBatch.Draw(BlueWins, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, menuscale, SpriteEffects.None, 0f);
            }
            if (Gamestate == 4)
            {
                spriteBatch.Draw(RedWins, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, menuscale, SpriteEffects.None, 0f);
            }
            if (Gamestate == 5)
            {
                spriteBatch.Draw(Pause, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, menuscale, SpriteEffects.None, 0f);
            }
            
            spriteBatch.End();
        
            base.Draw(gameTime);
        }

        //EIGEN METHODE

        protected void Reset()
        {
            Redlives = 3;
            Bluelives = 3;
            RedPlayerY = GraphicsDevice.Viewport.Height / 2 - 48;
            BluePlayerY = GraphicsDevice.Viewport.Height / 2 - 48;
            YellowPlayerX = GraphicsDevice.Viewport.Width / 2 + 48;
            GreenPlayerX = GraphicsDevice.Viewport.Width / 2 - 48;
            ybalposition = GraphicsDevice.Viewport.Height / 2 - 8;
            xbalposition = GraphicsDevice.Viewport.Width / 2 - 8;
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
            SPEEDPOWER = 0;
            Gamestate = 1;
        }
        protected void SpeedPowerUp()
        {
            if (SPEEDPOWER == 400)
            {
                SPEEDy = Var.Next(0, GraphicsDevice.Viewport.Height - 31);
                SPEEDx = Var.Next(50, GraphicsDevice.Viewport.Width - 81);
                SPEEDPOWER = -1;
            }
            else if (SPEEDPOWER == -1 && SPEEDx - xbalposition >= 0 && SPEEDx - xbalposition <= 31
                    && SPEEDy - ybalposition >= 0 && SPEEDy - ybalposition <= 31)
            {
                 SPEEDPOWER = 0;
                 if (ybalvel > 0) //De X en Y kunnen beide kanten opgaan dus er moet eerst gecheckt 
                 {                //worden of de verandering in snelheid + of - moet zijn
                     ybalvel += 5;
                 }
                 else
                 {
                     ybalvel -= 5;
                 }
                 if (xbalvel > 0)
                 {
                     xbalvel += 5;
                 }
                 else
                 {
                     xbalvel -= 5;
                 }
            }
            else if (SPEEDPOWER != -1)
            {
                SPEEDPOWER = Var.Next(0, 500);
            }
        }
    }
}