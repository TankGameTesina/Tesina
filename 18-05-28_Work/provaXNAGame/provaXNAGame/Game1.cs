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
using System.Text;


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
        //bool gaming;
        int myId;
        string myMask;
        string myBroadcast;
        private string username;
        int serverTick;
        Thread thSender;
        Pacchetto[] SendBuffer;
        bool SendFull;
        int SendRead;
        int SendWrite;
        bool invio;
        Semaphore sem = new Semaphore(1, 2);
        Semaphore time_sem = new Semaphore(1, 1);
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
        int postiMax;
        #endregion

        #region VARIABILIGRAFICA
        Form form;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private List<GameObject> players;
        private List<BulletObject> bullets;
        private List<Messaggio> messages;
        private Texture2D background;

        static public MouseState mouseState;
        static public MouseState previousState;
        private int punteggio = 0;

        private int altezzaSchermoClient = Screen.PrimaryScreen.Bounds.Height;
        private int larghezzaSchermoClient = Screen.PrimaryScreen.Bounds.Width;
        private int altezzaSchermoServer = 50;
        private int larghezzaSchermoServer = 50;
        private double rapportoAltezzaClientServer;
        private double rapportoLarghezzaClientServer;
        private const int costante = 50;
        private const int margine = 150;

        #endregion

        #region GRAFICA
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            players = new List<GameObject>();
            bullets = new List<BulletObject>();
            messages = new List<Messaggio>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            SendBuffer = new Pacchetto[10000];
            SendRead = 0;
            SendWrite = 0;
            invio = true;

            thSender = new Thread(() => Invio());
            thSender.Start();

            // TODO: Add your initialization logic here
            base.Initialize();

            serverStatus = false;
            serverTick = 0;
            IsMouseVisible = true;
            form = (Form)Form.FromHandle(Window.Handle);
            form.WindowState = FormWindowState.Maximized;

            Form2 IP;
            intUdpLocalPort = LOCAL_PORT;
            intUdpRemotePort = REMOTE_PORT;

            var app = clsWin32_Network.GetMultipleIpAddress(AddressFamily.InterNetwork);

            if (app.Count > 1)
            {
                IP = new Form2(app);
                IP.ShowDialog();
                myAddress = IP.IPScheda;
                IP.Dispose();
            }
            else
                myAddress = app[0];

            username = Environment.UserName;

            clsWin32_Network.GetMaskBroadcast(myAddress, ref myMask, ref myBroadcast);
            Bind();

            InitTimer();
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
        /// 
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keystate = Keyboard.GetState();
            Pacchetto pacchetto = null;
            previousState = mouseState;
            mouseState = Mouse.GetState();

            foreach (GameObject g in players)
            {
                g.Update();
            }

            if(serverStatus)
            {
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
                            msg = "blt;" + b.Id + ";" + result + ";" + life;
                        }

                        foreach (GameObject tnk in players)
                            Send(new Pacchetto(msg, tnk.Address));
                    }
                }
            }
            else
            {
                foreach (BulletObject b in bullets)
                {
                    b.Update();
                }
            }
            

            if (players.Count != 0)
            {
                if (previousState != mouseState)
                {
                    //if(players.FirstOrDefault(x => x.Id == myId).Position == players.FirstOrDefault(x => x.Id == myId).LastPosition)
                    pacchetto = new Pacchetto("ang;" + players.FirstOrDefault(x => x.Id == myId).setAngle(), serverAddress);
                }

                if (keystate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                {
                    pacchetto = new Pacchetto("for;" + players.FirstOrDefault(x => x.Id == myId).setAngle(), serverAddress);
                }

                if (keystate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                {
                    pacchetto = new Pacchetto("bck;" + players.FirstOrDefault(x => x.Id == myId).setAngle(), serverAddress);
                }

                if (previousState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    if(players.FirstOrDefault(x => x.Id == myId).Fire)
                        pacchetto = new Pacchetto("fire;" + players.FirstOrDefault(x => x.Id == myId).setAngle(), serverAddress);
                }

                if(pacchetto != null)
                    Send(pacchetto);

            }
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
            GameObject app = players.FirstOrDefault(x => x.Id == myId);

            if (players.Count != 0 && !app.ricaricando)
                spriteBatch.DrawString(scorefont, app.caricatore.ToString() + " colpi rimanenti", new Vector2(10, 10), Color.Red);
            else
                if(players.Count != 0 && app.ricaricando)
                    spriteBatch.DrawString(scorefont,"Ricaricando " +app.tempRicarica.ToString(), new Vector2(10, 10), Color.Red);

            int multi = 1;

            var rev = messages.Reverse<Messaggio>().ToList();
            if(rev.Count > 6)
                rev = rev.GetRange(0, 6);

            foreach (Messaggio msg in rev.Reverse<Messaggio>())
            {
                if(msg.LifeTime == 1000)
                {
                    messages.Remove(msg);
                }
                else
                {
                    Vector2 size = scorefont.MeasureString(msg.Msg);
                    spriteBatch.DrawString(scorefont, msg.Msg, new Vector2(larghezzaSchermoClient - size.X, multi * 20), msg.Color);
                    msg.LifeTime++;
                }

                multi++;
            }

            foreach (GameObject plr in players.Reverse<GameObject>())
            {
                plr.Draw(spriteBatch);
            }

            foreach(BulletObject blt in bullets.Reverse<BulletObject>().Reverse<BulletObject>())
            {
                blt.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private int BulletsCollision(BulletObject blt, out int life)
        {
            life = -1;
            foreach (GameObject g in players.Where(x => x.Id != blt.IdTank))
            {
                //gestione collisioni
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
            //uscita dallo schermo
            if (posizioneFutura.Position.X <= 0 || posizioneFutura.Position.X + posizioneFutura.BoundingBox.Width >= larghezzaSchermoServer
                || posizioneFutura.Position.Y <= 0 || posizioneFutura.Position.Y + posizioneFutura.BoundingBox.Height >= altezzaSchermoServer)
            {
                return true;
            }

            //controllo collisioni muri
           /* foreach (WallObject w in muri)
            {
                //gestione collisioni
                if (posizioneFutura.WallColliding(w))
                {
                    return true;
                }
            }*/

            //controllo collisioni altri carri armati
            foreach (GameObject g in players.Where(x => x.Id != posizioneFutura.Id))
            {
                //gestione collisioni
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

            //fino a che non trovo una posizione valida continuo a cercare 
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

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (serverStatus)
            {
                InviaPacchetto(new Pacchetto("srv;dct;"+myId, myBroadcast));
                serverStatus = false;
                reader.Abort();
            }
            else
                InviaPacchetto(new Pacchetto("dct", serverAddress));

            thSender.Abort();
            sem.Close();
            time_sem.Close();
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

                    if (SendRead == 9999)//9999
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

            if (SendWrite == 9999)//9999
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
                            initMyPlayer(Convert.ToInt32(ann[2])); //CHECK
                            larghezzaSchermoServer = Convert.ToInt32(ann[3]);
                            altezzaSchermoServer = Convert.ToInt32(ann[4]);
                            FunzioneDiMarco();
                            messages.Add(new Messaggio("Ti sei unito alla partita", Color.Blue));      
                            break;

                        case "ref":
                            //rimango nel menu, gli spazi sono pieni oppure sono gia all'interno
                            break;

                        case "dct":
                            app = players.FirstOrDefault(x => x.Id == Convert.ToInt32(ann[2]));
                            players.Remove(app); //rimuovo il giocatore che era server
                            MessageBox.Show("Server disconnesso");
                            //il server è disconnesso, torno al menu
                            break;

                        case "str":
                            //gaming = true;
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
                        if (players.Count < postiMax && (players.FirstOrDefault(x => x.Username == ann[1] && x.Address == pkt.Address)) == null) //ann[1] username
                        {
                            app = new GameObject();
                            players.Add(app);
                            app.InitEnemy(PosizioneInizialeCarri(), Content.Load<Texture2D>("tankEnemy"), Content.Load<Texture2D>("bulletMin"), ann[1], pkt.Address, players.IndexOf(app));
                            messages.Add(new Messaggio(ann[1] + " si e' unito", Color.Blue));
                            Send(new Pacchetto("srv;acc;" + app.Id + ";" + 1600 + ";" + 900 + ";", pkt.Address));                            
                            Send(new Pacchetto(app.Id + ";pos;" + app.ToString(), pkt.Address));
                            SendPlayersInformation(app);
                            SendPlayerInformation(app);
                        }
                        else
                        {
                            Send(new Pacchetto("srv;ref", pkt.Address));
                        }
                    }

                    break;

                case "ang":
                    if (serverStatus)
                    {
                        app = players.FirstOrDefault(x => x.Address == pkt.Address);
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
                        app = players.FirstOrDefault(x => x.Address == pkt.Address);
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
                        app = players.FirstOrDefault(x => x.Address == pkt.Address);
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
                    if(serverStatus)
                    {
                        app = players.FirstOrDefault(x => x.Address == pkt.Address);
                        if (app != null)
                        {
                            players.Remove(app);
                            messages.Add(new Messaggio(app.Username + " esce dalla partita", Color.Blue));

                            foreach (GameObject tnk in players.Where(x => x.Id != myId))
                            {
                                Send(new Pacchetto(app.Id + ";dct", tnk.Address));
                            }
                        }
                    }

                    break;

                case "fire":
                    if(serverStatus)
                    {
                        app = players.FirstOrDefault(x => x.Address == pkt.Address);
                        if (app != null)
                        {
                            int id = bullets.Count == 0 ? 0 : bullets[bullets.Count - 1].Id + 1;

                            blt = initBullet(app, ann, id);

                            foreach (GameObject tnk in players.Where(x => x.Id != myId))
                            {
                                Send(new Pacchetto("blt;" + blt.Id + ";crt;" + blt.ToString(), tnk.Address));
                            }
                        }
                    }

                    break;

                case "user":
                    app = new GameObject();
                    players.Add(app);
                    app.InitEnemy(new Vector2(100, 400), Content.Load<Texture2D>("tankEnemy"), Content.Load<Texture2D>("bulletMin"), ann[2], pkt.Address,Convert.ToInt32(ann[1]));
                    messages.Add(new Messaggio(ann[2] + " si unisce alla partita", Color.Blue));
                    break;

                case "blt":
                    blt = bullets.FirstOrDefault(x => x.Id == Convert.ToInt32(ann[1]));

                    if(blt != null)
                    {
                        if (ann[2] == "wtr")
                        {
                            bullets.Remove(blt);
                            messages.Add(new Messaggio("Colpo a vuoto", Color.Coral));
                        }
                        else
                        {
                            app = players.FirstOrDefault(x => x.Id == Convert.ToInt32(ann[2]));
                            app.Health = Convert.ToInt32(ann[3]);
                            bullets.Remove(blt);
                            messages.Add(new Messaggio(app.Username + " colpito", Color.Red));
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

                    if(Int32.TryParse(ann[0], out num))
                    {
                        app = players.FirstOrDefault(x => x.Id == num);
                        if (players.Contains(app))
                        {
                            if (ann[1] == "pos")
                            {
                                app.Position = new Vector2(Convert.ToInt32(Convert.ToInt32(ann[2]) * rapportoLarghezzaClientServer), Convert.ToInt32(Convert.ToInt32(ann[3]) * rapportoLarghezzaClientServer));
                                app.Angle = (float)Convert.ToDouble(ann[4]);
                            }
                            else
                            {
                                if (ann[1] == "dct")
                                {
                                    messages.Add(new Messaggio(app.Username + " esce dalla partita", Color.Blue));
                                    players.Remove(app);
                                }
                                else
                                {
                                    if (ann[1] == "win")
                                    {
                                        //vittoria del giocatore ID
                                    }
                                }
                            }

                        }
                    }                    

                    break;
            }
        }

        private void FunzioneDiMarco()
        {
            rapportoAltezzaClientServer = Convert.ToDouble(altezzaSchermoClient / altezzaSchermoServer);
            rapportoLarghezzaClientServer = Convert.ToDouble(larghezzaSchermoClient / larghezzaSchermoServer);
        }

        private BulletObject initBullet(GameObject app, string[] ann, int id)
        {
            BulletObject blt = new BulletObject();
            blt.Init(new Vector2(app.Position.X, app.Position.Y), Content.Load<Texture2D>("bulletMin"), (float)Convert.ToDouble(ann[1]));
            blt.Id = id;
            blt.IdTank = app.Id;
            bullets.Add(blt);
            /*var time = new myTimer(blt, 30000);
            time.Elapsed += myTimerServerTick;
            time.Start();*/

            return blt;
        }

        private BulletObject initBullet(string[] ann)
        {
            BulletObject blt = new BulletObject();
            blt.Init(new Vector2(Convert.ToInt32(Convert.ToInt32(ann[3]) * rapportoLarghezzaClientServer), Convert.ToInt32(Convert.ToInt32(ann[4]) * rapportoAltezzaClientServer)), Content.Load<Texture2D>("bulletMin"), (float)Convert.ToDouble(ann[5]));
            blt.Id = Convert.ToInt32(ann[1]);
            bullets.Add(blt);
            /*var time = new myTimer(blt, 10);
            time.Elapsed += myTimerTick;
            time.Start();
            */
            return blt;
        }

        public void initMyPlayer(int id)
        {
            GameObject player = new GameObject();
            players.Add(player);
            player.Init(PosizioneInizialeCarri(), Content.Load<Texture2D>("tank"), Content.Load<Texture2D>("bulletMin"), username, myAddress, id);
            myId = id;
        }

        private void SendPlayerInformation(GameObject player) //invio a tutti i giocatori, a parte l'host e il giocatore interessato, le nuove informazioni : 2 pacchetti[user, pos]
        {
            foreach (GameObject tnk in players.Where(x => (x.Id != myId) && (x.Id != player.Id)))
            {
                Send(new Pacchetto("user;" + player.Id + ";" + player.Username, tnk.Address)); //crea il tank e associa username
                Send(new Pacchetto(player.Id + ";pos;" + player.ToString(), tnk.Address));//posiziona il tank nell'arena
            }
        }

        private void SendPlayersInformation(GameObject player) //invio a tutti i giocatori, a parte l'host e il giocatore interessato, le nuove informazioni : 2 pacchetti[user, pos]
        {
            foreach (GameObject tnk in players.Where(x => (x.Id != player.Id)))
            {
                Send(new Pacchetto("user;" + tnk.Id + ";" + tnk.Username, player.Address)); //crea il tank e associa username
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

                    if (read == 9999)//9999
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
            int a = 0;

                try
                {
                
                    if (udpSocket == null)
                    {
                        return;
                    }

                
                    ipEPRx = new IPEndPoint(IPAddress.Any, 0);

                     ep = (EndPoint)ipEPRx;
                
                    udpSocket.EndReceiveFrom(ar, ref ep);
                
                    astr = ep.ToString().Split(':');
                
                    strReceived = Encoding.UTF8.GetString(abytRx);                
                    idx = strReceived.IndexOf((char)0);
                
                    if (idx > -1)
                    {
                        strReceived = strReceived.Substring(0, idx);

                    }
                
                    if (serverStatus)
                    {
                        buffer[write] = new Pacchetto(strReceived, astr[0]);

                        if (write == 9999)//9999
                        {
                            write = 0;
                            buffer_full = true;
                        }
                        else
                            write++;
                    }
                    else
                    {                    
                        Analisi(new Pacchetto(strReceived, astr[0]));                   
                    }

                    //ep = (EndPoint)new IPEndPoint(IPAddress.Any, intUdpLocalPort); // o 0
                    abytRx = new byte[abytRx.Length];                
                    udpSocket.BeginReceiveFrom(abytRx, 0, abytRx.Length, SocketFlags.None, ref ep, new AsyncCallback(OnReceive), ep);
            }
                catch (ObjectDisposedException ex)
                {
                    MessageBox.Show("OnReceive(): Eccezione ObjectDisposedException\n" + ex.Message);
                    /*udpSocket = null;
                    Bind();*/
                }
                catch (Exception ex)
                {
                    MessageBox.Show("OnReceive(): Eccezione Exception\n" + ex.Message + a);
                   /* udpSocket = null;
                    Bind();*/
                }
        }

        private delegate void del_OnSend(IAsyncResult ar);
        private void OnSend(IAsyncResult ar)
        {
            try
            {
                SocketError error;

                udpSocket.EndSend(ar, out error);

                if (error.ToString() != "Success")
                {
                    //MessageBox.Show(error.ToString());
                }

                sem.Release();
            }
            catch(SocketException ex)
            {
                MessageBox.Show("SocketException\n" + ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                MessageBox.Show("ObjectDisposed\n"+ex.Message);
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
                MessageBox.Show("Bind(): eccezione SocketException\n" + ex.Message);
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
            if (serverTick < 1)
            {
                Send(new Pacchetto("l", myBroadcast));
                
                serverTick++;
            }
            else //sotto se non trovo il server dopo 5 secondi, 10 tick
            {
                //tmrServer.Enabled = false;
                lTimer.Stop();

                serverStatus = true;
                larghezzaSchermoServer = Screen.PrimaryScreen.Bounds.Width;
                altezzaSchermoServer = Screen.PrimaryScreen.Bounds.Height;
                FunzioneDiMarco();
                read = 0;
                write = 0;
                buffer_full = false;
                buffer = new Pacchetto[10000];
                postiMax = 10;
                myId = 0;
                serverTick = 0;
                serverAddress = myAddress;
                udpSocket = null;
                //intUdpRemotePort = intUdpLocalPort; // la porta al momento assegnatami... tanto la cambio ad ogni invio
                intUdpLocalPort = REMOTE_PORT; //5001 ricevo
                intUdpRemotePort = LOCAL_PORT; //5000 invio

                Bind();

                initMyPlayer(0);
                reader = new Thread(() => ServerAnalisi());
                reader.Start();
               
                /*thBullet = new Thread(() => BulletsMove());
                thBullet.Start();*/
            }
        }
        #endregion
    }
}
