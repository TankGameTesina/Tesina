using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Windows.Forms;
using System;
using System.Net.Sockets;
using System.Net;
using clientServer;
using System.Threading;
using ServerChoosingDemo;
using System.Linq;
using System.Timers;
using System.Text;
using System.Threading.Tasks;

namespace provaXNAGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        #region VARAIABILIGIOCO
        System.Windows.Forms.Timer lTimer;
        string myAddress;
        string serverAddress;
        bool gaming;
        int myId;
        string myMask;
        string myBroadcast;
        private string username;
        private Color colorTank;
        int serverTick;
        Thread thSender;
        Pacchetto[] SendBuffer;
        bool SendFull;
        int SendRead;
        int SendWrite;
        bool invio;
        Semaphore sem;
        Semaphore time_sem;
        bool ricercaTerminata;
        bool partitaValida;
        GameObject Winner;
        #endregion

        #region VARIABILIUDP
        Socket udpSocket;
        EndPoint ep;
        byte[] abytRx = new byte[1024];
        byte[] abytTx = new byte[1024];

        int intUdpLocalPort;
        int intUdpRemotePort; //la porta IN del server

        private const int LOCAL_PORT = 64000; //il solo host cambia l'ordine, local 5001 e remote 5000
        private const int REMOTE_PORT = 64001;
        private const string NOME_LDAP_DOMINIO = "LDAP://itis/DC=itis, DC=pr, DC=it";
        #endregion

        #region VARIABILISERVER
        bool serverStatus;
        Pacchetto[] buffer;
        int read;
        int write;
        bool buffer_full;
        Thread reader;
        Thread thBullet;
        int bltCount;
        int postiMax;
        int postiMin;
        #endregion

        #region VARIABILIGRAFICA
        Form form;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private List<GameObject> playersInGame;
        private List<GameObject> players;
        private List<BulletObject> bullets;
        private List<Messaggio> messages;
        private Texture2D background;

        static public MouseState mouseState;
        static public MouseState previousState;
        private int punteggio = 0;

        private int altezzaSchermoClient = Screen.PrimaryScreen.Bounds.Height;
        private int larghezzaSchermoClient = Screen.PrimaryScreen.Bounds.Width;
        private int altezzaSchermoServer = 400;
        private int larghezzaSchermoServer = 400;
        private double rapportoAltezzaClientServer;
        private double rapportoLarghezzaClientServer;
        private const int costante = 50;
        private const int margine = 150;

        //public Vector2 healthBarPosition = new Vector2(0, 550);
        public Texture2D riempimentoBarProiettili;
        public Texture2D contornoBarProiettili;
        public Texture2D texPointer;
        public Rectangle rettangoloRiempimentoBarProiettili;
        public Rectangle rettangoloContornoBarProiettili;

        public Texture2D riempimentoBarVita;
        public Texture2D contornoBarVita;
        public Rectangle rettangoloRiempimentoBarVita;
        public Rectangle rettangoloContornoBarVita;

        #endregion

        #region VARIABILIMENU

        public enum GameState
        {
            StartMenu,
            Loading,
            Waiting,
            Playing,
            Paused,
            Die,
            Finish
        }
        private Button btnStart;
        private Button btnExit;
        private Button btnResume;
        private Button btnSetTank;
        private bool tankSetted;
        private bool tankInSet;
        private Texture2D loadingScreen;
        private Texture2D backgroundMenu;
        private Texture2D backgroundGame;
        private Texture2D backgroundPause;
        private GameState gameState;
        private Thread backgroundThread;
        private bool isLoading = false;
        private GameObject tankMenu;

        #endregion

        #region GRAFICA
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f);
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            #region menu
            IsMouseVisible = true;
            btnStart = new Button();
            btnSetTank = new Button();
            btnExit = new Button();
            btnResume = new Button();

            gameState = GameState.StartMenu;

            mouseState = Mouse.GetState();
            previousState = mouseState;

            tankMenu = new GameObject();
            #endregion

            form = (Form)Form.FromHandle(Window.Handle);
            form.WindowState = FormWindowState.Maximized;
            base.Initialize();
        }


        private void inizializza()
        {
            Winner = new GameObject();
            sem = new Semaphore(1, 2);
            time_sem = new Semaphore(1, 1);

            playersInGame = new List<GameObject>();
            players = new List<GameObject>();
            bullets = new List<BulletObject>();
            messages = new List<Messaggio>();

            SendBuffer = new Pacchetto[10000];
            SendRead = 0;
            SendWrite = 0;
            invio = true;

            thSender = new Thread(() => Invio());
            thSender.Start();

            serverStatus = false;
            serverTick = 0;
            IsMouseVisible = true;
            intUdpLocalPort = LOCAL_PORT;
            intUdpRemotePort = REMOTE_PORT;

            clsWin32_Network.GetMaskBroadcast(myAddress, ref myMask, ref myBroadcast);
            Bind();

            ricercaTerminata = false;
            InitTimer();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            loadContenuti();
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void loadContenuti()
        {
            #region menu
            username = RandomString(7);
            tankMenu.Init(new Vector2(larghezzaSchermoClient / 2, altezzaSchermoClient / 2), Content.Load<Texture2D>("tankMenu"), username, new Color(), "", 0);
            btnSetTank.Init(Content.Load<Texture2D>(@"setTankS"), Content.Load<Texture2D>(@"setTankR"), new Vector2(100, altezzaSchermoClient / 2 - Content.Load<Texture2D>(@"setTankR").Height));
            btnStart.Init(Content.Load<Texture2D>(@"startS"), Content.Load<Texture2D>(@"startR"), new Vector2(100, btnSetTank.Position.Y - 200));
            btnExit.Init(Content.Load<Texture2D>(@"exitS"), Content.Load<Texture2D>(@"exitR"), new Vector2(100, btnSetTank.Position.Y + 200));
            texPointer = Content.Load<Texture2D>("pointer");
            backgroundMenu = Content.Load<Texture2D>("menu");
            background = backgroundMenu;
            loadingScreen = Content.Load<Texture2D>(@"loading");
            #endregion

            contornoBarProiettili = Content.Load<Texture2D>("Contorno proiettili");
            riempimentoBarProiettili = Content.Load<Texture2D>("riempimentoProiettile");

            contornoBarVita = Content.Load<Texture2D>("ContornoVita");
            riempimentoBarVita = Content.Load<Texture2D>("riempimentoVita");
            colorTank = tankMenu.GetColor();
        }
        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            previousState = mouseState;
            mouseState = Mouse.GetState();

            #region menu
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                this.Exit();

            if (gameState == GameState.Die)
            {
                foreach (GameObject g in playersInGame.Reverse<GameObject>())
                {
                    g.Update();
                }

                if (serverStatus)
                {
                    if (playersInGame.Count > 1)
                        partitaValida = true;

                    if (playersInGame.Count == 1 && partitaValida)
                    {
                        string msg = playersInGame.First().Id + ";win";
                        foreach (GameObject tnk in players)
                            Send(new Pacchetto(msg, tnk.Address));
                    }

                    foreach (BulletObject b in bullets)
                    {
                        b.Update();

                        int life;
                        int result = BulletsCollision(b, out life);
                        string msg = "";

                        if (result != -2)
                        {
                            if (result == -1)
                            {
                                msg = "blt;" + b.Id + ";wtr";
                            }
                            else
                            {
                                if (life <= 0)
                                {
                                    msg = result + ";die;" + b.IdTank; //id;die;idOwner
                                }
                                else
                                {
                                    msg = "blt;" + b.Id + ";" + result + ";" + life;
                                }
                            }

                            foreach (GameObject tnk in players)
                                Send(new Pacchetto(msg, tnk.Address));
                        }
                    }
                }
                else
                {
                    foreach (BulletObject b in bullets.Reverse<BulletObject>())
                    {
                        b.Update();
                    }
                }

                if (btnExit.Update())
                    clear();
            }

            if (gameState == GameState.Finish)
            {
                if (btnExit.Update())
                    clear();
            }

            if (gameState == GameState.Loading && !isLoading)
            {
                backgroundThread = new Thread(LoadGame);
                isLoading = true;
                backgroundThread.Start();
            }

            if (gameState == GameState.Waiting)
            {
                KeyboardState keystate = Keyboard.GetState();

                if (keystate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                {
                    gameState = GameState.Paused;
                }
            }

            if (gameState == GameState.Playing)
            {
                KeyboardState keystate = Keyboard.GetState();
                Pacchetto pacchetto = null;

                rettangoloContornoBarProiettili = new Rectangle(20, 20, contornoBarProiettili.Width, contornoBarProiettili.Height);
                rettangoloRiempimentoBarProiettili = new Rectangle(24, 24, playersInGame.First(p => p.Id == myId).caricatore * riempimentoBarProiettili.Width, riempimentoBarProiettili.Height);

                rettangoloContornoBarVita = new Rectangle(20, altezzaSchermoClient - 100 - 4, contornoBarVita.Width, contornoBarVita.Height);
                rettangoloRiempimentoBarVita = new Rectangle(24, altezzaSchermoClient - 100, playersInGame.First(p => p.Id == myId).Health, riempimentoBarVita.Height);

                foreach (GameObject g in playersInGame.Reverse<GameObject>())
                {
                    g.Update();
                }

                if (serverStatus)
                {
                    if (playersInGame.Count > 1)
                        partitaValida = true;

                    if (playersInGame.Count == 1 && partitaValida)
                    {
                        string msg = playersInGame.First().Id + ";win";
                        foreach (GameObject tnk in players)
                            Send(new Pacchetto(msg, tnk.Address));
                    }

                    foreach (BulletObject b in bullets.Reverse<BulletObject>())
                    {
                        b.Update();

                        int life;
                        int result = BulletsCollision(b, out life);
                        string msg = "";

                        if (result != -2)
                        {
                            if (result == -1)
                            {
                                msg = "blt;" + b.Id + ";wtr";
                            }
                            else
                            {
                                if (life <= 0)
                                {
                                    msg = result + ";die;" + b.IdTank; //id;die
                                }
                                else
                                {
                                    msg = "blt;" + b.Id + ";" + result + ";" + life;
                                }
                            }
                            foreach (GameObject tnk in players)
                                Send(new Pacchetto(msg, tnk.Address));
                        }
                    }
                }
                else
                {
                    foreach (BulletObject b in bullets.Reverse<BulletObject>())
                    {
                        b.Update();
                    }
                }


                if (playersInGame.Count != 0)
                {
                    if (previousState != mouseState)
                    {
                        pacchetto = new Pacchetto("ang;" + playersInGame.FirstOrDefault(x => x.Id == myId).GetAngle(), serverAddress);
                    }

                    if (keystate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                    {
                        pacchetto = new Pacchetto("for;" + playersInGame.FirstOrDefault(x => x.Id == myId).GetAngle(), serverAddress);
                    }

                    if (keystate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                    {
                        pacchetto = new Pacchetto("bck;" + playersInGame.FirstOrDefault(x => x.Id == myId).GetAngle(), serverAddress);
                    }

                    if (previousState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        var app = playersInGame.FirstOrDefault(x => x.Id == myId);
                        if (app.Fire)
                            pacchetto = new Pacchetto("fire;" + playersInGame.FirstOrDefault(x => x.Id == myId).GetAngle(), serverAddress);

                        if (app.caricatore == 0 && !app.ricaricando)
                        {
                            app.Ricarica();
                        }

                    }

                    if (keystate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                    {
                        gameState = GameState.Paused;
                    }

                    if (pacchetto != null)
                        Send(pacchetto);

                }
            }

            if (gameState == GameState.StartMenu)
            {
                tankMenu.Angle = tankMenu.GetAngle();
                tankMenu.Update();

                if (!tankInSet)
                {
                    if (btnStart.Update())
                    {
                        if (!tankSetted)
                        {
                            DialogResult dg = MessageBox.Show("Sei sicuro di non voler configurare \r\n il tuo giocatore? Ne verrà creato uno casuale", "Attenzione", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                            if (dg == DialogResult.Yes)
                            {
                                tankInSet = true;
                                AvviaCaricamento();
                                tankInSet = false;
                            }
                        }
                        else
                        {
                            tankInSet = true;
                            AvviaCaricamento();
                            tankInSet = false;
                        }
                    }
                    else
                        if (btnSetTank.Update())
                    {
                        tankInSet = true;
                        FormSelectTank fm = new FormSelectTank();
                        var result = fm.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            System.Drawing.Color cl = fm.ReturnValue1;
                            colorTank = new Color(cl.R, cl.G, cl.B);
                            username = fm.ReturnValue2;
                            tankMenu.SetColor(colorTank);
                        }
                        else
                        {
                            fm.Dispose();
                        }

                        tankSetted = true;
                        tankInSet = false;
                    }

                    if (btnExit.Update())
                        Exit();
                }
            }

            if (gameState == GameState.Paused)
            {
                if (btnExit.Update())
                    clear();

                if (btnResume.Update())
                {
                    if(gaming)
                    {
                        gameState = GameState.Playing;
                        background = backgroundGame;
                    }
                    else
                        gameState = GameState.Waiting;

                    background = backgroundGame;
                }
            }

            if (gameState == GameState.Waiting && isLoading)
            {
                LoadGame();
                isLoading = false;
            }


            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            Rectangle mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            spriteBatch.Draw(background, mainFrame, Color.White);

            #region menu
            if (gameState == GameState.StartMenu)
            {
                IsMouseVisible = true;

                if (tankSetted)
                {                    
                    GameObject player = new GameObject();
                    player.Init(new Vector2(altezzaSchermoClient, larghezzaSchermoClient), Content.Load<Texture2D>("tank"), username, colorTank, "127.0.0.1", 3241);
                    player.Update();
                    player.Draw(spriteBatch);
                }

                spriteBatch.Draw(tankMenu.Texture, tankMenu.Position, tankMenu.SourceRectangle, Color.White, tankMenu.Angle, tankMenu.Origin, 1.0f, SpriteEffects.None, 1);
                tankMenu.Draw(spriteBatch);

                btnStart.Draw(spriteBatch);    
                btnSetTank.Draw(spriteBatch);
                btnExit.Draw(spriteBatch);
            }

            if (gameState == GameState.Loading)
            {
                spriteBatch.Draw(loadingScreen, new Vector2((GraphicsDevice.Viewport.Width / 2) - (loadingScreen.Width / 2), (GraphicsDevice.Viewport.Height / 2) - (loadingScreen.Height / 2)), Color.YellowGreen);
            }

            if (gameState == GameState.Die)
            {
                IsMouseVisible = true;
                spriteBatch.Draw(contornoBarProiettili, rettangoloContornoBarProiettili, Color.White);
                spriteBatch.Draw(riempimentoBarProiettili, rettangoloRiempimentoBarProiettili, Color.White);

                spriteBatch.Draw(contornoBarVita, rettangoloContornoBarVita, Color.White);
                spriteBatch.Draw(riempimentoBarVita, rettangoloRiempimentoBarVita, Color.White);

                foreach (GameObject plr in playersInGame.Reverse<GameObject>())
                {
                    plr.Draw(spriteBatch);
                }

                foreach (BulletObject blt in bullets.Reverse<BulletObject>().Reverse<BulletObject>())
                {
                    blt.Draw(spriteBatch);
                }

                btnExit.setPosition(new Vector2(larghezzaSchermoClient / 2 - btnExit.Width / 2, altezzaSchermoClient - btnExit.Height / 2 - 100));
                btnExit.Draw(spriteBatch);
            }

            if (gameState == GameState.Waiting)
            {
                SpriteFont scorefont = Content.Load<SpriteFont>("scoreFont");
                Vector2 size = scorefont.MeasureString("In attesa di altri giocatori");
                spriteBatch.DrawString(scorefont, "In attesa di altri giocatori", new Vector2(larghezzaSchermoClient / 2 - size.X / 2 , this.Window.ClientBounds.Height /2 - size.Y/2), Color.Green);
            }

            if (gameState == GameState.Playing)
            {
                SpriteFont scorefont = Content.Load<SpriteFont>("scoreFont");
                GameObject app = playersInGame.FirstOrDefault(x => x.Id == myId);

                IsMouseVisible = false;
                spriteBatch.Draw(contornoBarProiettili, rettangoloContornoBarProiettili, Color.White);
                spriteBatch.Draw(riempimentoBarProiettili, rettangoloRiempimentoBarProiettili, Color.White);

                spriteBatch.Draw(contornoBarVita, rettangoloContornoBarVita, Color.White);
                spriteBatch.Draw(riempimentoBarVita, rettangoloRiempimentoBarVita, Color.White);

                spriteBatch.DrawString(scorefont, app.Health.ToString() + "%", new Vector2(rettangoloContornoBarVita.X + rettangoloContornoBarVita.Width - 50, rettangoloContornoBarVita.Y + rettangoloContornoBarVita.Height / 2 - 15), Color.Green);

                if (playersInGame.Count != 0 && !app.ricaricando)
                    spriteBatch.DrawString(scorefont, app.caricatore.ToString(), new Vector2(rettangoloContornoBarProiettili.Width + 3, rettangoloContornoBarProiettili.Height / 2 + 5), Color.DarkBlue);

                if (playersInGame.Count != 0 && app.ricaricando)
                    spriteBatch.DrawString(scorefont, "Ricaricando " + app.tempRicarica.ToString(), new Vector2(rettangoloContornoBarProiettili.Width / 3, rettangoloContornoBarProiettili.Height / 2 + 5), Color.DarkBlue);

                int multi = 1;

                var rev = messages.Reverse<Messaggio>().ToList();
                if (rev.Count > 6)
                    rev = rev.GetRange(0, 6);

                foreach (Messaggio msg in rev.Reverse<Messaggio>())
                {
                    if (msg.LifeTime == 1000)
                    {
                        messages.Remove(msg);
                    }
                    else
                    {
                        Vector2 size = scorefont.MeasureString(msg.Msg);
                        spriteBatch.DrawString(scorefont, msg.Msg, new Vector2(larghezzaSchermoClient - size.X, multi * 20), Color.Green);
                        msg.LifeTime++;
                    }

                    multi++;
                }

                foreach (GameObject plr in playersInGame.Reverse<GameObject>())
                {
                    plr.Draw(spriteBatch);
                }

                foreach (BulletObject blt in bullets.Reverse<BulletObject>().Reverse<BulletObject>())
                {
                    blt.Draw(spriteBatch);
                }

                spriteBatch.Draw(texPointer, new Vector2(mouseState.X - texPointer.Width / 2, mouseState.Y - texPointer.Height / 2), Color.White);
            }

            if (gameState == GameState.Paused)
            {
                IsMouseVisible = true;
                background = backgroundPause;
                btnResume.setPosition(new Vector2(larghezzaSchermoClient / 2 - btnExit.Width / 2, altezzaSchermoClient - btnExit.Height / 2 - 400));
                btnResume.Draw(spriteBatch);
                btnExit.setPosition(new Vector2(larghezzaSchermoClient / 2 - btnExit.Width / 2, altezzaSchermoClient - btnExit.Height / 2 - 100));
                btnExit.Draw(spriteBatch);
            }

            if (gameState == GameState.Finish)
            {
                IsMouseVisible = true;

                if (Winner != new GameObject())
                {
                    SpriteFont scorefont = Content.Load<SpriteFont>("fontWinner");
                    spriteBatch.DrawString(scorefont, "Il vincitore e'", new Vector2(larghezzaSchermoClient / 2 - 300, altezzaSchermoClient / 2), Color.Gold);
                    spriteBatch.DrawString(scorefont, Winner.Username, new Vector2(larghezzaSchermoClient / 2 - 150, altezzaSchermoClient / 2 + 70), Color.Gold);
                    btnExit.setPosition(new Vector2(larghezzaSchermoClient / 2 - btnExit.Width / 2, altezzaSchermoClient - btnExit.Height / 2 - 100));
                    btnExit.Draw(spriteBatch);
                }
            }
            #endregion

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void LoadGame()
        {
            bool end = false;
            btnResume.Init(Content.Load<Texture2D>(@"resumeS"), Content.Load<Texture2D>(@"resumeR"), new Vector2(altezzaSchermoClient / 2 - btnResume.Width / 2, altezzaSchermoClient - 600));

            backgroundPause = Content.Load<Texture2D>("pause");
            backgroundGame = Content.Load<Texture2D>("map");
            background = backgroundGame;

            while(!end)
            {
                if (ricercaTerminata)
                {
                    if(gaming)
                        gameState = GameState.Playing;
                    else
                        gameState = GameState.Waiting;

                    isLoading = false;
                    end = true;
                }
            }

        }

        public bool setMyIp()
        {
            Form2 IP;
            var app = clsWin32_Network.GetMultipleIpAddress(AddressFamily.InterNetwork);

            if (app.Count > 1)
            {
                IP = new Form2(app);
                IP.ShowDialog();

                if (IP.IPScheda != null)
                    myAddress = IP.IPScheda;
                else
                    return false;

                IP.Dispose();
            }
            else
                myAddress = app[0];

            return true;
        }

        public void AvviaCaricamento()
        {
            if(setMyIp())
            {
                inizializza();
                gameState = GameState.Loading;
                isLoading = false;
            }
        }

        private int BulletsCollision(BulletObject blt, out int life)
        {
            life = -1;
            foreach (GameObject g in playersInGame.Where(x => x.Id != blt.IdTank))
            {
                if (blt.TankColliding(g))
                {
                    life = g.Health - 50;

                    return g.Id;
                }
            }

            if (blt.Position.X <= 0 || blt.Position.X + blt.BoundingBox.Width >= larghezzaSchermoServer + costante
                || blt.Position.Y <= 0 || blt.Position.Y + blt.BoundingBox.Height >= altezzaSchermoServer + costante)
            {
                return -1;
            }

            return -2;
        }

        private bool ControlloCollisioni(BaseObject posizioneFutura)
        {
            if (posizioneFutura.Position.X <= 0 || posizioneFutura.Position.X + posizioneFutura.BoundingBox.Width >= larghezzaSchermoServer
                || posizioneFutura.Position.Y <= 0 || posizioneFutura.Position.Y + posizioneFutura.BoundingBox.Height >= altezzaSchermoServer)
            {
                return true;
            }

            foreach (GameObject g in playersInGame.Where(x => x.Id != posizioneFutura.Id))
            {
                if (posizioneFutura.TankColliding(g))
                {
                    return true;
                }
            }

            return false;
        }

        private Vector2 PosizioneInizialeCarri()
        {
            Random rand = new Random();
            bool collide = true;
            BaseObject tank = new BaseObject();
            int ordinata = 0;
            int ascissa = 0;

            while (collide)
            {
                switch (rand.Next(1, 3))
                {
                    case 1:
                        ascissa = rand.Next(1, 3) == 1 ? rand.Next(10, margine) : rand.Next(larghezzaSchermoServer - margine, larghezzaSchermoServer);
                        ordinata = rand.Next(5, altezzaSchermoServer);
                        break;
                    case 2:
                        ascissa = rand.Next(5, larghezzaSchermoServer);
                        ordinata = rand.Next(1, 3) == 1 ? rand.Next(10, margine) : rand.Next(altezzaSchermoServer - margine, altezzaSchermoServer);
                        break;
                }
                tank = new BaseObject(new Rectangle(ascissa, ordinata, 100, 100), new Vector2(ascissa, ordinata));
                collide = false;

                collide = ControlloCollisioni(tank);
            }
            return new Vector2(tank.BoundingBox.X, tank.BoundingBox.Y);
        }
        private void clear()
        {
            if (serverStatus)
            {
                InviaPacchetto(new Pacchetto("srv;dct;" + myId, myBroadcast));
                serverStatus = false;
                partitaValida = false;
                reader.Abort();
            }
            else
                InviaPacchetto(new Pacchetto("dct", serverAddress));

            udpSocket.Shutdown(SocketShutdown.Both);
            udpSocket.Close();
            thSender.Abort();
            sem.Close();
            time_sem.Close();

            loadContenuti();
            gaming = false;
            gameState = GameState.StartMenu;

            players = new List<GameObject>();
            playersInGame = new List<GameObject>();
            bullets = new List<BulletObject>();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (gameState != GameState.StartMenu)
            {
                thSender.Abort();
                sem.Close();
                time_sem.Close();

                if (serverStatus)
                {
                    InviaPacchetto(new Pacchetto("srv;dct;" + myId, myBroadcast));
                    serverStatus = false;
                    reader.Abort();
                }else
                {
                    InviaPacchetto(new Pacchetto("dct", serverAddress));
                }               

                udpSocket.Shutdown(SocketShutdown.Both);
                udpSocket.Close();
            }

            base.OnExiting(sender, args);
        }
        #endregion

        #region LOGIC

        public void Invio()
        {
            while (invio)
            {
                if (SendRead < SendWrite || SendFull)
                {
                    sem.WaitOne();
                    InviaPacchetto(SendBuffer[SendRead]);

                    if (SendRead == 9999)
                    {
                        SendRead = 0;
                        SendFull = false;
                    }
                    else
                        SendRead++;
                }
            }

            return;
        }

        public void Send(Pacchetto pkt)
        {
            SendBuffer[SendWrite] = pkt;

            if (SendWrite == 9999)
            {
                SendWrite = 0;
                SendFull = true;
            }
            else
                SendWrite++;
        }

        private void Analisi(Pacchetto pkt)
        {
            GameObject app;
            BulletObject blt;

            var ann = new String[1];
            ann[0] = pkt.Msg;

            if (pkt.Msg.Contains(';'))
                ann = pkt.Msg.Split(';');

            switch (ann[0])
            {
                case "l":
                    if (serverStatus)
                    {
                        Send(new Pacchetto("srv;" + myAddress, pkt.Address));
                    }

                    break;

                case "srv": //messaggi dal server
                    switch (ann[1])
                    {
                        case "acc": //gaming true o comunque sono in partita
                            ricercaTerminata = true;
                            initMyPlayer(Convert.ToInt32(ann[2])); //CHECK
                            larghezzaSchermoServer = Convert.ToInt32(ann[3]);
                            altezzaSchermoServer = Convert.ToInt32(ann[4]);
                            rapportSchermo();
                            messages.Add(new Messaggio("Ti sei unito alla partita", Color.Blue));
                            break;

                        case "ref":
                            MessageBox.Show("Il server non ha accettato la tua richiesta");
                            clear();

                            //rimango nel menu, gli spazi sono pieni oppure sono gia all'interno
                            break;

                        case "dct":
                            app = playersInGame.FirstOrDefault(x => x.Id == Convert.ToInt32(ann[2]));
                            if (app != null)
                                playersInGame.Remove(app); //rimuovo il giocatore che era server
                            else
                                players.Remove(app);

                            clear();
                            MessageBox.Show("Server disconnesso");
                            //il server è disconnesso, torno al menu
                            break;

                        case "str":
                            gaming = true;
                            gameState = GameState.Playing;
                            break;

                        default:
                            lTimer.Stop();
                            serverAddress = pkt.Address;

                            //operazione che eseguo solo adesso in testing
                            if (serverTick != 0)
                                Send(new Pacchetto("r;" + username, serverAddress));

                            serverTick = 0;
                            break;
                    }
                    break;

                case "r":
                    if (serverStatus)
                    {
                        bool playerDie = false;
                        foreach (GameObject player in players)
                            if (pkt.Address == player.Address) //controllo se il giocatore è gia nella partita
                                playerDie = true;

                        if (!playerDie && playersInGame.Count < postiMax && (playersInGame.FirstOrDefault(x => x.Username == ann[1])) == null) //ann[1] username
                        {
                            app = new GameObject(PosizioneInizialeCarri(), ann[1], pkt.Address);
                            players.Add(app);

                            int aId = players.IndexOf(app);
                            aId = aId == -1 ? 0 : aId;

                            app.Id = aId;

                            Send(new Pacchetto("srv;acc;"  + aId + ";" + larghezzaSchermoServer + ";" + altezzaSchermoServer, app.Address));
                            Send(new Pacchetto(aId + ";pos;" + app.ToString(), app.Address));
                            SendPlayersInformation(app); //notifico agli altri del nuovo ingresso
                            SendPlayerInformation(app); //notifico il nuovo giocatore di tutti gli altri

                            if(players.Count >= postiMin) //implementabile timer che dopo aver raggiunto i posti minimi faccia un countdown, mentre possono ancora entrare giocatori e nel caso uscire, 
                            {
                                partitaValida = true;
                                //Send(new Pacchetto("srv;str", myBroadcast));

                                foreach (GameObject tnk in players)
                                {
                                    Send(new Pacchetto("srv;str", tnk.Address));
                                }
                            }
                        }
                        else
                        {
                            Send(new Pacchetto("srv;ref", pkt.Address)); //volendo si può inserire un numero che dice il motivo del rifiuto
                        }
                    }

                    break;

                case "ang":
                    if (serverStatus)
                    {
                        app = playersInGame.FirstOrDefault(x => x.Address == pkt.Address);
                        if (app != null)
                        {
                            app.Angle = (float)Convert.ToDouble(ann[1]);
                            foreach (GameObject tnk in players)
                            {
                                Send(new Pacchetto(app.Id + ";pos;" + app.ToString(), tnk.Address));
                            }
                        }
                    }

                    break;

                case "for":
                    if (serverStatus)
                    {
                        app = playersInGame.FirstOrDefault(x => x.Address == pkt.Address);
                        if (app != null)
                        {
                            app.Angle = (float)Convert.ToDouble(ann[1]);

                            if (!ControlloCollisioni(app.PosizioneFutura("f")))
                            {
                                app.moveForward();
                                //e a tutti quelli all'interno della lobby
                                foreach (GameObject tnk in players)
                                {
                                    Send(new Pacchetto(app.Id + ";pos;" + app.ToString(), tnk.Address));
                                }
                            }

                        }
                    }
                    break;

                case "bck":
                    if (serverStatus)
                    {
                        app = playersInGame.FirstOrDefault(x => x.Address == pkt.Address);
                        if (app != null)
                        {
                            app.Angle = (float)Convert.ToDouble(ann[1]);

                            if (!ControlloCollisioni(app.PosizioneFutura("b")))
                            {
                                app.moveBack();
                                foreach (GameObject tnk in players)
                                {
                                    Send(new Pacchetto(app.Id + ";pos;" + app.ToString(), tnk.Address));
                                }
                            }
                        }
                    }

                    break;

                case "dct":
                    if (serverStatus)
                    {
                        app = playersInGame.FirstOrDefault(x => x.Address == pkt.Address);
                        if (app != null)
                        {
                            playersInGame.Remove(app);

                            foreach (GameObject tnk in players.Where(x => x.Id != myId))
                            {
                                Send(new Pacchetto(app.Id + ";dct", tnk.Address));
                            }

                            players.Remove(app);
                        }
                    }

                    break;

                case "fire":
                    if (serverStatus)
                    {
                        app = playersInGame.FirstOrDefault(x => x.Address == pkt.Address);
                        if (app != null)
                        {
                            int bId = bltCount;
                            bltCount++;
                            blt = new BulletObject(new Vector2(app.Position.X, app.Position.Y), (float)Convert.ToDouble(ann[1]), bId, app.Id);

                            blt.Init(new Vector2(app.Position.X, app.Position.Y), Content.Load<Texture2D>("bulletMin"), (float)Convert.ToDouble(ann[1]), bId, app.Id);

                            bullets.Add(blt);

                            foreach (GameObject tnk in players.Where(x => x.Id != myId).ToList())
                            {
                                Send(new Pacchetto("blt;" + blt.Id + ";crt;" + blt.ToString(), tnk.Address));
                            }
                        }
                    }

                    break;

                case "user":
                    app = new GameObject();
                    app.InitEnemy(new Vector2(100, 400), Content.Load<Texture2D>("tankEnemy"), ann[2], ann[3], Convert.ToInt32(ann[1]));
                    messages.Add(new Messaggio(ann[2] + " si unisce alla partita", Color.Blue));
                    playersInGame.Add(app);

                    if(!serverStatus)
                        players.Add(app);
                    
                    break;

                case "blt":
                    blt = bullets.FirstOrDefault(x => x.Id == Convert.ToInt32(ann[1]));

                    if (blt != null)
                    {
                        if (ann[2] == "wtr")
                        {
                            bullets.Remove(blt);
                        }
                        else
                        {
                            app = playersInGame.FirstOrDefault(x => x.Id == Convert.ToInt32(ann[2]));
                            app.Health = Convert.ToInt32(ann[3]);
                            bullets.Remove(blt);
                            messages.Add(new Messaggio(app.Username + " colpito, rimane a " + app.Health + " di vita", Color.Red));
                        }
                    }
                    else
                    {
                        if (ann[2] == "crt")
                        {
                            initBullet(ann);
                        }
                    }

                    break;

                default:
                    int num;

                    if (Int32.TryParse(ann[0], out num))
                    {
                        app = playersInGame.FirstOrDefault(x => x.Id == num);
                        if (playersInGame.Contains(app))
                        {
                            if (ann[1] == "die")
                            {
                                messages.Add(new Messaggio(players.FirstOrDefault(x => x.Id == Int32.Parse(ann[2])).Username + " ha eliminato " + app.Username, Color.Red));
                                app.Position = new Vector2(-200, -200);
                                playersInGame.Remove(app);
                                if (app.Id == myId)
                                    gameState = GameState.Die;
                            }
                            if (ann[1] == "pos")
                            {
                                app.Position = new Vector2(Convert.ToInt32(Convert.ToInt32(ann[2]) * rapportoLarghezzaClientServer), Convert.ToInt32(Convert.ToInt32(ann[3]) * rapportoAltezzaClientServer));
                                app.Angle = (float)Convert.ToDouble(ann[4]);
                            }
                            else
                            {
                                if (ann[1] == "dct")
                                {
                                    messages.Add(new Messaggio(app.Username + " esce dalla partita", Color.Blue));
                                    playersInGame.Remove(app);
                                }
                                else
                                {
                                    if (ann[1] == "win")
                                    {
                                        Winner = playersInGame.FirstOrDefault(x => x.Id == app.Id);
                                        gameState = GameState.Finish;
                                    }

                                }
                            }

                        }
                    }

                    break;
            }
        }

        private void rapportSchermo()
        {
            rapportoAltezzaClientServer = Convert.ToDouble(altezzaSchermoClient) / Convert.ToDouble(altezzaSchermoServer);
            rapportoLarghezzaClientServer = Convert.ToDouble(larghezzaSchermoClient) / Convert.ToDouble(larghezzaSchermoServer);
        }
        private BulletObject initBullet(GameObject app, string[] ann, int id)
        {
            BulletObject blt = new BulletObject();
            blt.Init(new Vector2(app.Position.X, app.Position.Y), Content.Load<Texture2D>("bulletMin"), (float)Convert.ToDouble(ann[1]), id, app.Id);
            
            bullets.Add(blt);
            return blt; 
        }

        private BulletObject initBullet(string[] ann)
        {
            BulletObject blt = new BulletObject();          
            blt.Init(new Vector2(Convert.ToInt32(Convert.ToInt32(ann[3]) * rapportoLarghezzaClientServer), Convert.ToInt32(Convert.ToInt32(ann[4]) * rapportoAltezzaClientServer)), Content.Load<Texture2D>("bulletMin"), (float)Convert.ToDouble(ann[5]), Convert.ToInt32(ann[1]), -1);
            bullets.Add(blt);
            return blt;
        }

        public void initMyPlayer(int id)
        {
            GameObject player = new GameObject();
            player.Init(PosizioneInizialeCarri(), Content.Load<Texture2D>("tank"), username, colorTank, myAddress, id);
            myId = id;
            playersInGame.Add(player);
            //players.Add(player);
        }

        private void SendPlayerInformation(GameObject player) //invio a tutti i giocatori, a parte il giocatore interessato, le sue informazioni : 2 pacchetti[user, pos]
        {
            foreach (GameObject tnk in playersInGame.Where(x => x.Id != player.Id))
            {
                Send(new Pacchetto("user;" + player.Id + ";" + player.Username + ";" + player.Address, tnk.Address)); //crea il tank e associa username
                Send(new Pacchetto(player.Id + ";pos;" + player.ToString(), tnk.Address));//posiziona il tank nell'arena
            }
        }

        private void SendPlayersInformation(GameObject player) //invio a al giocatore interessato, tuute le informazioni : 2 pacchetti[user, pos]
        {
            foreach (GameObject tnk in playersInGame.Where(x => (x.Id != player.Id)))
            {
                Send(new Pacchetto("user;" + tnk.Id + ";" + tnk.Username + ";" + tnk.Address, player.Address)); //crea il tank e associa username
                Send(new Pacchetto(tnk.Id + ";pos;" + tnk.ToString(), player.Address));//posiziona il tank nell'arena
            }
        }

        public void ServerAnalisi()
        {
            while (serverStatus)
            {
                if (read < write || buffer_full)
                {
                    Analisi(buffer[read]);

                    if (read == 9999)
                    {
                        read = 0;
                        buffer_full = false;
                    }
                    else
                        read++;
                }
            }

            return;
        }

        #endregion

        #region UDPMETODS
        private void InviaPacchetto(Pacchetto pkt)
        {
            if (pkt != null || pkt.Address == null)
            {
                IPEndPoint ipEP;
                IPAddress ipAddress;
                int port = pkt.Address == myAddress ? intUdpLocalPort : intUdpRemotePort;

                ipAddress = IPAddress.Parse(pkt.Address);

                ipEP = new IPEndPoint(ipAddress, port);
                ep = (EndPoint)ipEP;

                try
                {
                    abytTx = Encoding.UTF8.GetBytes(pkt.Msg);

                    udpSocket.BeginSendTo(abytTx, 0, pkt.Msg.Length, SocketFlags.None, ep, new AsyncCallback(OnSend), null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Send(): eccezione Exception\n" + ex.Message);
                }
            }

        }

        private delegate void del_OnReceive(IAsyncResult ar);
        private void OnReceive(IAsyncResult ar)
        {
            string strReceived;
            int idx;
            IPEndPoint ipEPRx;
            string[] astr;

            try
            {
                ipEPRx = new IPEndPoint(IPAddress.Any, 0);
                ep = (EndPoint)ipEPRx;

                udpSocket.EndReceiveFrom(ar, ref ep);

                astr = ep.ToString().Split(':');
                strReceived = Encoding.UTF8.GetString(abytRx);


                idx = strReceived.IndexOf((char)0);
                if (idx > -1)
                    strReceived = strReceived.Substring(0, idx);

                if (serverStatus)
                {
                    buffer[write] = new Pacchetto(strReceived, astr[0]);

                    if (write == 9999)
                    {
                        write = 0;
                        buffer_full = true;
                    }
                    else
                        write++;
                }
                else
                    Analisi(new Pacchetto(strReceived, astr[0]));
                abytRx = new byte[abytRx.Length];
                udpSocket.BeginReceiveFrom(abytRx, 0, abytRx.Length, SocketFlags.None, ref ep, new AsyncCallback(OnReceive), ep);
            }
            catch (ObjectDisposedException ex)
            {
                return;
            }
            catch (Exception ex)
            {
                udpSocket.BeginReceiveFrom(abytRx, 0, abytRx.Length, SocketFlags.None, ref ep, new AsyncCallback(OnReceive), ep);

                return;
            }
        }

        private delegate void del_OnSend(IAsyncResult ar);
        private void OnSend(IAsyncResult ar)
        {
            try
            {
                SocketError error;

                udpSocket.EndSend(ar, out error);

                sem.Release();
            }
            catch (SocketException ex)
            {
                MessageBox.Show("SocketException\n" + ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                //MessageBox.Show("ObjectDisposed\n" + ex.Message);
                sem.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eccezione Exception\n" + ex.Message);
            }
        }

        private void Bind() //da effettuare solo quando cambio la porta locale
        {
            try
            {
                IPEndPoint ipEP;

                udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                ipEP = new IPEndPoint(IPAddress.Any, intUdpLocalPort);
                udpSocket.Bind(ipEP);
                ep = (EndPoint)ipEP;

                udpSocket.BeginReceiveFrom(abytRx, 0, abytRx.Length, SocketFlags.None, ref ep, new AsyncCallback(OnReceive), null);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Bind(): eccezione Exception\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bind(): eccezione Exception\n" + ex.Message);
            }
        }
        #endregion

        #region COMPONENTS

        private void InitTimer()
        {
            lTimer = new System.Windows.Forms.Timer();
            lTimer.Interval = 1000;
            lTimer.Tick += new EventHandler(tmrServer_Tick);
            lTimer.Start();
        }

        private void tmrServer_Tick(object sender, EventArgs e)
        {
            if (serverTick < 6)
            {
                Send(new Pacchetto("l", myBroadcast));

                serverTick++;
            }
            else //sotto se non trovo il server dopo 5 secondi, 10 tick
            {
                lTimer.Stop();

                serverStatus = true;
                larghezzaSchermoServer = Screen.PrimaryScreen.Bounds.Width;
                altezzaSchermoServer = Screen.PrimaryScreen.Bounds.Height;
                rapportSchermo();
                read = 0;
                write = 0;
                buffer_full = false;
                buffer = new Pacchetto[10000];
                postiMax = 12;
                postiMin = 2;
                //myId = 0;
                bltCount = 0;
                serverTick = 0;
                serverAddress = myAddress;

                //udpSocket = null;
                udpSocket.Shutdown(SocketShutdown.Both);
                udpSocket.Close();
                intUdpLocalPort = REMOTE_PORT;
                intUdpRemotePort = LOCAL_PORT;

                Bind();

                //initMyPlayer(0);
                Send(new Pacchetto("r;" + username, serverAddress));
                reader = new Thread(() => ServerAnalisi());
                reader.Start();
                ricercaTerminata = true;
            }
        }
        #endregion
    }
}
