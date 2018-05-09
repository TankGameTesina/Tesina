using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProvaXNAGame;
using System.Collections.Generic;
using System.Windows.Forms;

namespace provaXNAGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        Form form;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private GameObject playerTank = new GameObject();
        private List<GameObject> enemysTank = new List<GameObject>();
        private int numEnemyTanks = 1;
        private Texture2D background;

        static public MouseState mouseState;
        static public MouseState previousState;
        private int punteggio = 0;
        
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
            IsMouseVisible = true;
            form = (Form)Form.FromHandle(Window.Handle);
            form.WindowState = FormWindowState.Maximized;

            playerTank.Init(new Vector2(100, 400), Content.Load<Texture2D>("tank"), Content.Load<Texture2D>("bulletMin"));
            for(int i=0; i < numEnemyTanks; i++)
            {
                GameObject enemyTank = new GameObject();
                enemyTank.Init(new Vector2(200, 200+50 * i), Content.Load<Texture2D>("tank"), Content.Load<Texture2D>("bulletMin"));
                enemysTank.Add(enemyTank);
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("map");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            KeyboardState keystate = Keyboard.GetState();
            previousState = mouseState;
            mouseState = Mouse.GetState();

            for (int i = 0; i < numEnemyTanks; i++)
            {
                playerTank.Update();
                enemysTank[i].Update();
            }

            if (keystate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            {
                for (int i = 0; i < numEnemyTanks; i++)
                {
                    //gestione collisioni
                    if(!playerTank.Colliding(enemysTank[i]))
                        {
                        playerTank.Update();
                        enemysTank[i].Update();
                        playerTank.moveForward();
                        //enemysTank[i].moveForward();
                        }
                }
            }

            if (keystate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
            {
                for (int i = 0; i < numEnemyTanks; i++)
                {
                    playerTank.Update();
                    enemysTank[i].Update();
                    playerTank.moveBack();
                    //enemysTank[i].moveBack();
                }
            }

            if (previousState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                playerTank.newBullet();
                for (int i = 0; i < numEnemyTanks; i++)
                {
                    enemysTank[i].newBullet();
                }
            }

            List<BulletObject> bulletRimuovere = new List<BulletObject>();
            foreach (BulletObject bullet in playerTank.bullets)
            {
                for (int i = 0; i < numEnemyTanks; i++)
                {
                    if (bullet.Colliding(enemysTank[i]))
                    {
                        punteggio++;
                        bulletRimuovere.Add(bullet);
                        break;
                    }
                }
            }

            foreach(BulletObject bullet in bulletRimuovere)
                playerTank.removeBullet(bullet);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            // Draw the background.
            Rectangle mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            spriteBatch.Draw(background, mainFrame, Color.White);
            SpriteFont scorefont = Content.Load<SpriteFont>("scoreFont");
            spriteBatch.DrawString(scorefont, punteggio.ToString(), new Vector2(10, 10), Color.White);

            playerTank.Draw(spriteBatch);
            for (int i = 0; i < numEnemyTanks; i++)
            {
                enemysTank[i].Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
