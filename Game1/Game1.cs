using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace Game1
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D Redplayer, Blueplayer, Ball, Redheart, Blueheart, Yellowheart, Greenheart, Menu, RedWins, BlueWins, Pause, Player, SPower, YellowWins, GreenWins; //Textures van de sprites.
        Vector2 RedplayerPosition, BlueplayerPosition, BallPosition, BlueHeartLocation, RedHeartLocation, YellowplayerPosition, GreenplayerPosition; //Vectoren voor de posities van de sprites.
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
        int GreenPlayerX; //Xpositie van de groene speler (X)
        int YellowPlayerX; //Xpositie van de gele speler (X)
        double BallMiddleY; //Midden van de bal (Y)
        double BallMiddleX; //Midden van de bal (X)
        double RedMiddleY; //Midden van de rode speler (Y)
        double BlueMiddleY; //Midden van de blauwe speler (Y)
        double GreenMiddleX; //Midden van de groene speler (X)
        double YellowMiddleX; //Midden van de gele speler (X)
        int Redlives = 3; //Rode Levens.
        int Bluelives = 3; //Blauwe Levens.
        int Yellowlives = 3; //Gele Levens.
        int Greenlives = 3; //Groene Levens.
        SoundEffect BounceSound; //Stuitergeluidje.
        Song BackGroundMusic; //Achtergonrdmuziek
        int Gamestate; //Gamestate om menus te gebruiken
        float menuscale; //Menuschaal om fullscreen een goede schaal te geven
        int lastgamestate; //Vorige gamestate zodat de gamestate niet heel vaak verandert
        int SPEEDPOWER;
        int SPEEDy;
        int SPEEDx;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Reset();
            Gamestate = 0;
            graphics.PreferredBackBufferHeight = 450;
            graphics.ApplyChanges();
            menuscale = GraphicsDevice.Viewport.Width/(float)3840;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            BackGroundMusic = Content.Load<Song>("BackGroundMusic"); //Muziek gemaakt door Kieran van Gaalen
            Redplayer = Content.Load<Texture2D>("rodeSpeler");
            Blueplayer = Content.Load<Texture2D>("blauweSpeler");
            Ball = Content.Load<Texture2D>("bal");
            Redheart = Content.Load<Texture2D>("Rood hart");
            Blueheart = Content.Load<Texture2D>("Blauw hart");
            Yellowheart = Content.Load<Texture2D>("Geel hart");
            Greenheart = Content.Load<Texture2D>("Groen hart");
            BounceSound = Content.Load<SoundEffect>("Boiiing"); //Stuitergeluid gemaakt door Kieran van Gaalen
            Menu = Content.Load<Texture2D>("MainMenu4k");
            RedWins = Content.Load<Texture2D>("RedWins4k");
            BlueWins = Content.Load<Texture2D>("BlueWins4k");
            Pause = Content.Load<Texture2D>("PauseScreen4k");
            Player = Content.Load<Texture2D>("Speler");
            SPower = Content.Load<Texture2D>("Speed");
            YellowWins = Content.Load<Texture2D>("YellowWins4k");
            GreenWins = Content.Load<Texture2D>("GreenWins4k");
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState lastkeyboardstate = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            lastgamestate = Gamestate;

            if (Gamestate == 0) //Als je in het hoofdmenu zit wordt dit gedeelte uitgevoerd
            {
                if(Keyboard.GetState().IsKeyDown(Keys.LeftShift)) //Voor 4 spelers naar gamestate 2
                {
                    Reset();
                    Gamestate = 2;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Escape) && lastkeyboardstate.IsKeyUp(Keys.Escape))
                    Exit(); //als je op escape drukt sluit het spel.
                if (Keyboard.GetState().IsKeyDown(Keys.Space)) //Voor 2 spelers naar gamestate 1
                    Reset();
  
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

            if (Gamestate == 1) //Als je met 2 spelers speelt wordt dit gedeelte uitgevoerd
            {
                //Hier wordt voor de hitbox het midden (Y) van de sprites berekent
                BallMiddleY = ybalposition + 8;
                RedMiddleY = RedPlayerY + 48;
                BlueMiddleY = BluePlayerY + 48;
                //pauze
                if (Keyboard.GetState().IsKeyDown(Keys.P) && lastkeyboardstate.IsKeyUp(Keys.P) && lastgamestate != 9)
                {
                    Gamestate = 9;
                    MediaPlayer.Pause();
                }
                //Snelheid en richting bal berekenen
                xbalposition += xbalvel;
                ybalposition += ybalvel;
                totalbalvel = Math.Sqrt(Math.Pow(xbalvel, 2) * Math.Pow(ybalvel, 2));
                if (Keyboard.GetState().IsKeyDown(Keys.Escape)) //Als je op esc drukt ga je terug naar het menu
                    Gamestate = 0;

                SpeedPowerUp();
                //Als de bal achter de paddels komt reset de bal en gaan de levens omlaag
                if (xbalposition >= GraphicsDevice.Viewport.Width + 20 || xbalposition <= -20)
                {
                    if (xbalposition <= -20)
                    {
                        Redlives -= 1;
                        if (Redlives == 0)
                            Gamestate = 3;
                    }
                    if (xbalposition >= GraphicsDevice.Viewport.Width + 20)
                    {
                        Bluelives -= 1;
                        if (Bluelives == 0)
                            Gamestate = 4;
                    }
                    //Hier geen volledige reset, want levens moeten gelijk blijven
                    ybalposition = GraphicsDevice.Viewport.Height / 2 - 8; //Positie bal resetten
                    xbalposition = GraphicsDevice.Viewport.Width / 2 - 8;
                    XRandom = Var.Next(-1, 2); //Random de snelheid van de bal (richting) resetten
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
                    ybalvel += (BallMiddleY - BlueMiddleY) / 50; //Het stuiteren wordt extremer bij de randen
                    xbalposition = GraphicsDevice.Viewport.Width - 57; //De x positie van de bal naar voor de paddle zetten zodat deze methode maar 1x kan gebeuren
                    StuiterVersnelling2P();
                }
                if (RedMiddleY - BallMiddleY <= 56 && RedMiddleY - BallMiddleY >= -56 && xbalposition <= 26 && xbalposition >= -1)
                { //Als de Y van de bal dicht genoeg bij de paddles zit, en de x zit ook in de paddels, stuiter
                    ybalvel += (BallMiddleY - RedMiddleY) / 50; //Het stuiteren wordt extremer bij de randen
                    xbalposition = 27; //De x naar voor de paddle zetten zodat deze methode maar 1x kan gebeuren
                    StuiterVersnelling2P();
                }

                if (ybalposition >= GraphicsDevice.Viewport.Height - 14 || ybalposition <= 0) //De Y stuiter gebeurt als de bal bij de randen komt
                    ybalvel = -ybalvel; //Hier geen versnelling, alleen maar omdraaien van de ybalvel
                //Hier komt de keyboardinput voor de paddles van rood
                if (RedPlayerY < GraphicsDevice.Viewport.Height - 97 && currentKeyboardState.IsKeyDown(Keys.S)) //Dit wordt gebruikt om de beweging te limiteren
                    RedPlayerY += 7;
                if (RedPlayerY > 0 && currentKeyboardState.IsKeyDown(Keys.W))
                    RedPlayerY -= 7;
                //Hier komt de keyboardinput voor de paddles van blauw
                if (BluePlayerY < GraphicsDevice.Viewport.Height - 97 && currentKeyboardState.IsKeyDown(Keys.Down)) //Dit wordt gebruikt om de beweging te limiteren
                    BluePlayerY += 7;
                if (BluePlayerY > 0 && currentKeyboardState.IsKeyDown(Keys.Up))
                    BluePlayerY -= 7;
                //Berekenen positie van de sprites voor het tekenen
                BallPosition = new Vector2((int)xbalposition, (int)ybalposition);
                RedplayerPosition = new Vector2(10, RedPlayerY);
                BlueplayerPosition = new Vector2(GraphicsDevice.Viewport.Width - 26, BluePlayerY);
            }
            if (Gamestate == 2) //Als je met 4 spelers speelt wordt dit gedeelte uitgevoerd.
            {
                {
                    //Hier wordt voor de hitbox het midden (Y) van de sprites berekent
                    BallMiddleY = ybalposition + 8;
                    BallMiddleX = xbalposition + 8;
                    RedMiddleY = RedPlayerY + 48;
                    BlueMiddleY = BluePlayerY + 48;
                    YellowMiddleX = YellowPlayerX - 48;
                    GreenMiddleX = GreenPlayerX + 48;
                    //pauze
                    if (Keyboard.GetState().IsKeyDown(Keys.P) && lastkeyboardstate.IsKeyUp(Keys.P) && lastgamestate != 10)
                    {
                        Gamestate = 10;
                        MediaPlayer.Pause();
                    }
                    //Snelheid en richting bal berekenen
                    xbalposition += xbalvel;
                    ybalposition += ybalvel;
                    totalbalvel = Math.Sqrt(Math.Pow(xbalvel, 2) * Math.Pow(ybalvel, 2));
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        Gamestate = 0;

                    SpeedPowerUp();
                    //Als de bal achter de paddels komt reset hij
                    if (xbalposition >= GraphicsDevice.Viewport.Width + 20 || xbalposition <= -20
                        || ybalposition <= -20 || ybalposition >= GraphicsDevice.Viewport.Height + 20)
                    {
                        if (xbalposition <= -20)
                            Redlives -= 1;
                        if (xbalposition >= GraphicsDevice.Viewport.Width + 20)
                            Bluelives -= 1;
                        if (ybalposition <= -20)
                            Yellowlives -= 1;
                        if (ybalposition >= GraphicsDevice.Viewport.Height +20)
                            Greenlives -= 1;
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
                        xbalposition >= GraphicsDevice.Viewport.Width - 42 && xbalposition <= GraphicsDevice.Viewport.Width - 13 && Bluelives > 0)
                        { //Als de Y van de bal dicht genoeg bij de paddles zit, en de x zit ook in de paddels, stuiter
                        ybalvel += (BallMiddleY - BlueMiddleY) / 50; //Het stuiteren wordt extremer bij de randen
                        xbalposition = GraphicsDevice.Viewport.Width - 57; //De x positie van de bal naar voor de paddle zetten zodat de versnelling maar 1x kan gebeuren
                        StuiterVersneling4P(ref ybalvel, 1);
                        if (totalbalvel < 25) //Als de snelheid onder de maximumsnelheid ligt mag er versneld worden
                            xbalvel -= .25;
                    }
                    else if (xbalposition >= GraphicsDevice.Viewport.Width - 14 && Bluelives <= 0)
                        xbalvel = -xbalvel;
                    if (RedMiddleY - BallMiddleY <= 56 && RedMiddleY - BallMiddleY >= -56 && xbalposition <= 26 && xbalposition >= -1 && Redlives > 0)
                    { //Als de Y van de bal dicht genoeg bij de paddles zit, en de x zit ook in de paddels, stuiter
                        ybalvel += (BallMiddleY - RedMiddleY) / 50; //Het stuiteren wordt extremer bij de randen
                        xbalposition = 27; //De x naar voor de paddle zetten zodat de versnelling maar 1x kan gebeuren
                        StuiterVersneling4P(ref ybalvel, 1);
                        if (totalbalvel < 25) //Als de snelheid onder de maximumsnelheid ligt mag er versneld worden
                            xbalvel += .25;
                    } 
                    else if (xbalposition <= 0 && Redlives <= 0)
                        xbalvel = -xbalvel;
                    if (YellowMiddleX - BallMiddleX <= 56 && YellowMiddleX - BallMiddleX >= -56 && ybalposition <= 26 && ybalposition >= -1 && Yellowlives > 0)
                    { //Als de Y van de bal dicht genoeg bij de paddles zit, en de x zit ook in de paddels, stuiter, maar alleen als de speler nog leeft
                        xbalvel += (BallMiddleX - YellowMiddleX) / 50; //Het stuiteren wordt extremer bij de randen
                        ybalposition = 27; //De x naar voor de paddle zetten zodat de versnelling maar 1x kan gebeuren 
                        StuiterVersneling4P(ref xbalvel, 0);
                        if (totalbalvel < 25) //Als de snelheid onder de maximumsnelheid ligt mag er versneld worden
                            ybalvel += .25;
                    }
                    else if (ybalposition <= 0 && Yellowlives <= 0) //Als de speler niet meer leeft moet de bal reageren als op een gewone rand
                        ybalvel = -ybalvel;
                    if (GreenMiddleX - BallMiddleX <= 56 && GreenMiddleX - BallMiddleX >= -56 &&
                        ybalposition >= GraphicsDevice.Viewport.Height - 42 && ybalposition <= GraphicsDevice.Viewport.Height - 13 && Greenlives > 0)
                    { //Als de X van de bal dicht genoeg bij de paddles zit, en de Y zit ook in de paddels, stuiter
                        xbalvel += (BallMiddleX - GreenMiddleX) / 50; //Het stuiteren wordt extremer bij de randen
                        ybalposition = GraphicsDevice.Viewport.Height - 57; //De x positie van de bal naar voor de paddle zetten zodat de versnelling maar 1x kan gebeuren
                        StuiterVersneling4P(ref xbalvel, 0);
                        if (totalbalvel < 25) //Als de snelheid onder de maximumsnelheid ligt mag er versneld worden
                            ybalvel -= .25;
                    }
                    else if (ybalposition >= GraphicsDevice.Viewport.Height - 14 && Greenlives <= 0)
                        ybalvel = -ybalvel;
                    //Hier komt de keyboardinput voor de paddles van rood
                    if (RedPlayerY < GraphicsDevice.Viewport.Height - 97 && currentKeyboardState.IsKeyDown(Keys.S)) //Dit wordt gebruikt om de beweging te limiteren
                        RedPlayerY += 7;
                    if (RedPlayerY > 0 && currentKeyboardState.IsKeyDown(Keys.W))
                        RedPlayerY -= 7;
                    //Hier komt de keyboardinput voor de paddles van blauw
                    if (BluePlayerY < GraphicsDevice.Viewport.Height - 97 && currentKeyboardState.IsKeyDown(Keys.Down)) //Dit wordt gebruikt om de beweging te limiteren
                        BluePlayerY += 7;
                    if (BluePlayerY > 0 && currentKeyboardState.IsKeyDown(Keys.Up))
                        BluePlayerY -= 7;
                    //Hier komt de keyboardinput voor de paddles van geel
                    if (YellowPlayerX > 96 && currentKeyboardState.IsKeyDown(Keys.H))
                        YellowPlayerX -= 7;
                    if (YellowPlayerX < GraphicsDevice.Viewport.Width && currentKeyboardState.IsKeyDown(Keys.K))
                        YellowPlayerX += 7;
                    //Hier komt de keyboardinput voor de paddles van groen
                    if (GreenPlayerX > 0 && currentKeyboardState.IsKeyDown(Keys.NumPad4))
                        GreenPlayerX -= 7;
                    if (GreenPlayerX < GraphicsDevice.Viewport.Width - 96 && currentKeyboardState.IsKeyDown(Keys.NumPad6))
                        GreenPlayerX += 7;
                    //Berekenen positie van de sprites voor het tekenen
                    BallPosition = new Vector2((int)xbalposition, (int)ybalposition);
                    RedplayerPosition = new Vector2(10, RedPlayerY);
                    BlueplayerPosition = new Vector2(GraphicsDevice.Viewport.Width - 26, BluePlayerY);
                    GreenplayerPosition = new Vector2(GreenPlayerX, GraphicsDevice.Viewport.Height - 10);
                    YellowplayerPosition = new Vector2(YellowPlayerX, 10);
                    //Berekenen welke speler mogelijk wint
                    if (Bluelives == 0 && Yellowlives == 0 && Greenlives == 0)
                        Gamestate = 5;
                    if (Redlives == 0 && Yellowlives == 0 && Greenlives == 0)
                        Gamestate = 6;
                    if (Redlives == 0 && Bluelives == 0 && Greenlives == 0)
                        Gamestate = 7;
                    if (Redlives == 0 && Bluelives == 0 && Yellowlives == 0)
                        Gamestate = 8;
                }
            }
            //Winmenu van de spelers waarbij spatie het spel reset en escape terug naar het hoofdmenu gaat
            switch(Gamestate)
            {
                case 3:
                case 4:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        Reset();
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        Gamestate = 0;
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        Reset();
                        Gamestate = 2;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        Gamestate = 0;
                    }
                    break;
                case 9:
                    if (currentKeyboardState.IsKeyDown(Keys.P) && lastkeyboardstate.IsKeyUp(Keys.P) && lastgamestate != 1)
                    {
                        Gamestate = 1;
                        MediaPlayer.Resume();
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        Gamestate = 0;
                    break;
                case 10:
                    if (currentKeyboardState.IsKeyDown(Keys.P) && lastkeyboardstate.IsKeyUp(Keys.P) && lastgamestate != 2)
                    {
                        Gamestate = 2;
                        MediaPlayer.Resume();
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        Gamestate = 0;
                    break;
            }
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);
            //tekenen sprites
            spriteBatch.Begin();
            if (Gamestate == 1 || Gamestate == 2 || Gamestate == 9 || Gamestate == 10) //2 of 4 spelermode en pauzemenus daarvan
            {
                if (Redlives > 0)
                    spriteBatch.Draw(Redplayer, RedplayerPosition, Color.White);
                if (Bluelives > 0)
                    spriteBatch.Draw(Blueplayer, BlueplayerPosition, Color.White);
                spriteBatch.Draw(Ball, BallPosition, Color.White);
                if (SPEEDPOWER == -1) //Alleen als er een speedobject moet zijn wordt er een speed powerup sprite getekend.
                {
                    spriteBatch.Draw(SPower, new Vector2(SPEEDx, SPEEDy), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                //Het tekenen van de levens
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
            if (Gamestate == 2 || Gamestate == 10) //4 spelers
            {
                if (Yellowlives > 0)
                    spriteBatch.Draw(Player, YellowplayerPosition, null, Color.Yellow, (float)Math.PI / 2, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                if (Greenlives > 0)
                    spriteBatch.Draw(Player, GreenplayerPosition, null, Color.Green, (float)-Math.PI / 2, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                for (int i = 0; i < Yellowlives; i++)
                {
                    Vector2 YellowHeartLocation = new Vector2(GraphicsDevice.Viewport.Width - (i + 1) * 13, 17);
                    spriteBatch.Draw(Yellowheart, YellowHeartLocation, Color.White);
                }
                for (int i = 0; i < Greenlives; i++)
                {
                    Vector2 GreenHeartLocation = new Vector2(i * 13, 17);
                    spriteBatch.Draw(Greenheart, GreenHeartLocation, Color.White);
                }
            }
            //Winmenus, hoofdmenu en pauzemenus
            switch (Gamestate)
            {
                case 0: //hoodmenu
                    spriteBatch.Draw(Menu, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, menuscale, SpriteEffects.None, 0f);
                    break;
                case 3:
                    spriteBatch.Draw(BlueWins, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, menuscale, SpriteEffects.None, 0f);
                    break;
                case 4:
                    spriteBatch.Draw(RedWins, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, menuscale, SpriteEffects.None, 0f);
                    break;
                case 5:
                    spriteBatch.Draw(RedWins, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, menuscale, SpriteEffects.None, 0f);
                    break;
                case 6:
                    spriteBatch.Draw(BlueWins, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, menuscale, SpriteEffects.None, 0f);
                    break;
                case 7:
                    spriteBatch.Draw(YellowWins, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, menuscale, SpriteEffects.None, 0f);
                    break;
                case 8:
                    spriteBatch.Draw(GreenWins, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, menuscale, SpriteEffects.None, 0f);
                    break;
                //pauzemenus
                case 9:
                case 10:
                    spriteBatch.Draw(Pause, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, menuscale, SpriteEffects.None, 0f);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        //Eigen methodes
        protected void Reset() //Zet alle variabelen naar de standaardwaarden om alles naar het beginpunt te zetten
        {
            MediaPlayer.Play(BackGroundMusic); //Mediaspeler opnieuw gestart
            MediaPlayer.IsRepeating = true;
            //Levens worden gereset
            Redlives = 3;
            Bluelives = 3;
            Yellowlives = 3;
            Greenlives = 3;
            //Posities worden gereset
            RedPlayerY = GraphicsDevice.Viewport.Height / 2 - 48;
            BluePlayerY = GraphicsDevice.Viewport.Height / 2 - 48;
            YellowPlayerX = GraphicsDevice.Viewport.Width / 2 + 48;
            GreenPlayerX = GraphicsDevice.Viewport.Width / 2 - 48;
            ybalposition = GraphicsDevice.Viewport.Height / 2 - 8;
            xbalposition = GraphicsDevice.Viewport.Width / 2 - 8;
            ybalposition = GraphicsDevice.Viewport.Height / 2 - 8;
            xbalposition = GraphicsDevice.Viewport.Width / 2 - 8;
            //Randompositie en richting bal wordt berekend
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
        protected void SpeedPowerUp() //De methode die verantwoordelijk is voor het maken van de speed powerup. Er kan maximaal 1 speed powerup tegelijk in het spel zijn.
        {
            if (SPEEDPOWER == 400) //Dit stuk code wordt actief als er een nieuwe willekeurige speed powerup op het veld gezet moet worden.
            {
                SPEEDy = Var.Next(0, GraphicsDevice.Viewport.Height - 31); //Maakt een willekeurige Y-cordinaat voor de speed powerup sprite die nog wel binnen het scherm valt.
                SPEEDx = Var.Next(50, GraphicsDevice.Viewport.Width - 81); //Maakt een willekeuige X-cordinaat voor de speed powerup die (ruim) binnen tussen de peddels valt.
                SPEEDPOWER = -1; //Zolang SPEEDPOWER op -1 staat weet het spel dat de speedpower ergens in het spel aanwezig is en wordt de daarbij behorende code uitgevoerd.
            }
            else if (SPEEDPOWER == -1 && xbalposition - SPEEDx >= 0 && xbalposition - SPEEDx <= 31 && ybalposition - SPEEDy >= 0 && ybalposition - SPEEDy <= 31)
            {                    //Dit stuk code wordt actief als de bal de speed powerup aanraakt.
                 SPEEDPOWER = 0; //De speedpowerup verdwijnt uit het spel omdat hij opgepakt is door de bal.
                 if (ybalvel > 0) //De X en Y kunnen beide kanten opgaan dus er moet eerst gecheckt worden of de verandering in snelheid + of - moet zijn.
                     ybalvel += 5; //Daarna wordt er 5 aan de snelheid in zowel x als y toegevoegd (dat betekend dat de bal 5 pixels per frame sneller gaat).
                 else
                     ybalvel -= 5;
                 if (xbalvel > 0)
                     xbalvel += 5;
                 else
                     xbalvel -= 5;
            }
            else if (SPEEDPOWER != -1) //Als de speedpower niet in het spel aanwezig is dan wordt de SPEEDPOWER variable een willekeurige waarde gegeven tussen 0 en 500.
            {                          //Op het moment dat deze waarte toevallig precies 400 is dan wordt er een nieuwe SPEEDPOWER powerup op het veld gezet via de code bovenaan deze methode.
                SPEEDPOWER = Var.Next(0, 500);
            }
        }
        protected void StuiterVersneling4P(ref double a, int b)
        {
            if (b == 1) //als we de vernsellingsfunctie opgeven voor de blauwe of rode paddle moet de xbalvel omgedraaid worden, anders de ybalvel.
                xbalvel = -xbalvel;
            else
                ybalvel = -ybalvel; //Omdraaien van de snelheid in de X, zodat hij terugstuitert.
            BounceSound.Play(); //Speelt een geluidje zodra de bal stuitert.
            if (totalbalvel < 25) //Als de snelheid onder de maximumsnelheid ligt mag er versneld worden
            {
                if (a > 0) //De Y kan beide kanten opgaan dus er moet eerst gecheckt worden of de verandering in snelheid + of - moet zijn
                    a += .25;
                else
                    a -= .25;
            }

        }
        protected void StuiterVersnelling2P()
        {
            xbalvel = -xbalvel; //Omdraaien van de snelheid in de X, zodat hij terugstuitert
            BounceSound.Play(); //Speelt een geluidje zodra de bal stuiterd.
            if (totalbalvel < 25) //Als de snelheid onder de maximumsnelheid ligt mag er versneld worden
            {
                if (ybalvel > 0) //De Y kan beide kanten opgaan dus de verandering in snelheid moet eerst gecheckt worden of het + of - moet zijn
                    ybalvel += .5;
                else
                    ybalvel -= .5;
                if (xbalvel > 0) //Omdat deze methode bij zowel de blauwe als de rode pedel wordt gebruikt kan ook de X beide kanten
                    xbalvel += .5; //opgaan dus de verandering in snelheid moet eerst gecheckt worden of het + of - moet zijn.
                else
                    xbalvel -= .5;
            }
        }
    }
}