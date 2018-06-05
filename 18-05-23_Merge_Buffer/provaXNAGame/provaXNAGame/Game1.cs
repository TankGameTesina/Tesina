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
        myTimer time;
        Semaphore sem = new Semaphore(1, 1);
        private void myTimerTick(object sender, ElapsedEventArgs e)
        {
            var timer = (myTimer)sender;
            timer.Blt.move();
            Console.WriteLine(timer.Blt);
            string msg;

            //funzione di marco per collisioni, bordi ecc

            if (true)
            {
                msg = "blt;" + timer.Blt.id + ";wtr";
            }
            else
                msg = "blt;" + timer.Blt.id + ";pos;" + timer.Blt.ToString();

            foreach (GameObject tnk in players)
                Send(new Pacchetto(msg, tnk.Address));

        }

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
        private Texture2D background;

        static public MouseState mouseState;
        static public MouseState previousState;
        private int punteggio = 0;

        #endregion

        #region GRAFICA
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            players = new List<GameObject>();
            bullets = new List<BulletObject>();
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
            /*
            for (int i = 0; i < numEnemyTanks; i++)
            {
                GameObject enemyTank = new GameObject();
                enemyTank.Init(new Vector2(200, 200 + 50 * i), Content.Load<Texture2D>("tank"), Content.Load<Texture2D>("bulletMin"));
                players.Add(enemyTank);
            }*/
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
            Pacchetto pacchetto = null;
            previousState = mouseState;
            mouseState = Mouse.GetState();

            for (int i = 0; i < players.Count; i++)
            {
                players[i].Update();
            }

            if(players.Count != 0)
            {
                if (previousState != mouseState)
                {
                    if(players.FirstOrDefault(x => x.Id == myId).Position == players.FirstOrDefault(x => x.Id == myId).LastPosition)
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
            spriteBatch.DrawString(scorefont, punteggio.ToString(), new Vector2(10, 10), Color.White);

            foreach(GameObject plr in players.Reverse<GameObject>())
            {
                plr.Draw(spriteBatch);
            }

            foreach(BulletObject blt in bullets.Reverse<BulletObject>())
            {
                blt.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (serverStatus)
            {
                serverStatus = false;
                //thBullet.Abort();
                Send(new Pacchetto("fanculoServerSpento", serverAddress));
                reader.Abort();
            }
            Send(new Pacchetto("dct", serverAddress));
            thSender.Abort();
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
            string pos;

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
                            initMyPlayer(Convert.ToInt32(ann[2]));
                            break;

                        case "ref":
                            //rimango nel menu, gli spazi sono pieni oppure sono gia all'interno
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
                            app.InitEnemy(new Vector2(100, 400), Content.Load<Texture2D>("tankEnemy"), Content.Load<Texture2D>("bulletMin"), ann[1], pkt.Address, players.IndexOf(app));
                            Send(new Pacchetto("srv;acc;" + app.Id, pkt.Address));
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
                            app.angle = (float)Convert.ToDouble(ann[1]);
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
                            app.angle = (float)Convert.ToDouble(ann[1]);
                            pos = app.moveForward();
                            if (VerifyMovement(pos))
                            {
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
                            app.angle = (float)Convert.ToDouble(ann[1]);
                            pos = app.moveBack();
                            if (VerifyMovement(pos))
                            {
                                foreach (GameObject tnk in players)
                                {
                                    Send(new Pacchetto(app.Id + ";pos;" + pos, tnk.Address));
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
                            int id = bullets.Count == 0 ? 0 : bullets[bullets.Count - 1].id + 1;

                            blt = new BulletObject();
                            blt.Init(new Vector2(app.Position.X, app.Position.Y), Content.Load<Texture2D>("bulletMin"), (float)Convert.ToDouble(ann[1]));
                            blt.id = id;
                            bullets.Add(blt);
                            time = new myTimer(blt, 50);
                            time.Elapsed += myTimerTick;
                            time.Start();

                            foreach (GameObject tnk in players.Where(x => x.Id != myId))
                            {
                                Send(new Pacchetto("blt;" + blt.id + ";crt;" + blt.ToString(), tnk.Address));
                            }
                        }
                    }

                    break;

                case "user":
                    app = new GameObject();
                    players.Add(app);
                    app.InitEnemy(new Vector2(100, 400), Content.Load<Texture2D>("tankEnemy"), Content.Load<Texture2D>("bulletMin"), ann[2], pkt.Address,Convert.ToInt32(ann[1]));
                    break;

                case "blt":
                    blt = bullets.FirstOrDefault(x => x.id == Convert.ToInt32(ann[1]));

                    if(blt != null)
                    {
                        if (ann[2] == "wtr")
                        {
                            bullets.Remove(blt);
                        }
                        else
                        {
                            if(ann[2] == "pos")
                            {
                                blt.Position = new Vector2(Convert.ToInt32(ann[3]), Convert.ToInt32(ann[4]));
                                blt.angle = (float)Convert.ToDouble(ann[5]);
                            }
                        }
                    }
                    else
                    {
                        if (ann[2] == "crt")
                        {                        
                            blt = new BulletObject();
                            blt.Init(new Vector2(Convert.ToInt32(ann[3]), Convert.ToInt32(ann[4])), Content.Load<Texture2D>("bulletMin"), (float)Convert.ToDouble(ann[5]));
                            blt.id = Convert.ToInt32(ann[1]);
                            bullets.Add(blt);
                        }
                    }

                    break;

                default:
                    //metto qua i controlli per i vari giocatori, controllo se esiste in primis, lato client
                    app = players.FirstOrDefault(x => x.Id == Convert.ToInt32(ann[0]));
                    if (players.Contains(app))
                    {
                        if (ann[1] == "pos")
                        {
                            app.Position = new Vector2(Convert.ToInt32(ann[2]), Convert.ToInt32(ann[3]));
                            app.angle = (float)Convert.ToDouble(ann[4]);
                        }
                        else
                        {
                            if (ann[1] == "dct")
                            {
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

                    break;
            }
        }

        public void initMyPlayer(int id)
        {
            GameObject player = new GameObject();
            players.Add(player);
            player.Init(new Vector2(100, 400), Content.Load<Texture2D>("tank"), Content.Load<Texture2D>("bulletMin"), "Pippo", myAddress, id);
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

        private bool VerifyMovement(string pos) //passo la posizione da varificare
        {
            //verifica se la posizione è giusta

            return true;
        }

        private void InviaPacchetto(Pacchetto pkt)
        {
            if(pkt != null)
            {
                IPEndPoint ipEP;
                IPAddress ipAddress;
                int port = pkt.Address == myAddress ? intUdpLocalPort : intUdpRemotePort;

                ipAddress = IPAddress.Parse(pkt.Address);

                ipEP = new IPEndPoint(ipAddress, port); //porta settata di norma su 5000
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

        public void BulletsMove() //sostituisco con un thread per ogni bullet risolto il problema davide fa schifo
        {
            while (serverStatus)
            {
                foreach (BulletObject b in bullets.Reverse<BulletObject>())
                {
                    if (b != null && b.inizializzato)
                    {
                        foreach (GameObject tnk in players)
                        {
                            if(b.destroy)
                            {
                                Send(new Pacchetto("blt;" + b.id + ";wtr", tnk.Address));
                                bullets.Remove(b);
                            }
                            else
                            {
                                if(b.Update())
                                    Send(new Pacchetto("blt;" + b.id + ";pos;" + b.ToString(), tnk.Address));
                            }
                        }
                    }
                }
            }

            return;
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
        private delegate void del_OnReceive(IAsyncResult ar);
        private void OnReceive(IAsyncResult ar)
        {
                /* if (InvokeRequired)  // Per gestire il crossthread (questa routine è chiamata da un altro thread)
                 {
                     BeginInvoke(new del_OnReceive(OnReceive), ar);

                     return;
                 }*/
                string strReceived;
                int idx;
                IPEndPoint ipEPRx;
                string[] astr;

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
                        strReceived = strReceived.Substring(0, idx);

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
                        Analisi(new Pacchetto(strReceived, astr[0]));


                    abytRx = new byte[abytRx.Length];
                    udpSocket.BeginReceiveFrom(abytRx, 0, abytRx.Length, SocketFlags.None, ref ep, new AsyncCallback(OnReceive), ep);
                }
                catch (ObjectDisposedException ex)
                {
                    MessageBox.Show("OnReceive(): Eccezione ObjectDisposedException\n" + ex.Message);
                    udpSocket = null;
                    Bind();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("OnReceive(): Eccezione Exception\n" + ex.Message);
                    udpSocket = null;
                    Bind();
                }
        }

        private delegate void del_OnSend(IAsyncResult ar);
        private void OnSend(IAsyncResult ar)
        {
            try
            {
                udpSocket.EndSend(ar);
            }
            catch (ObjectDisposedException ex)
            {
                MessageBox.Show("OnSend(): Eccezione ObjectDisposedException\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("OnSend(): Eccezione Exception\n" + ex.Message);
            }

            sem.Release();
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

                udpSocket.BeginReceiveFrom(abytRx, 0, abytRx.Length, SocketFlags.None, ref ep, new AsyncCallback(OnReceive), ep);
            }
            catch (SocketException ex)
            {
                //MessageBox.Show("Bind(): eccezione SocketException\n" + ex.Message);
                ex.ToString();
                MessageBox.Show("Bind()", "Rilevato altro programma ServerChoosing, cambio porta locale");
                intUdpLocalPort--;
                udpSocket = null;
                Bind();
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
                read = 0;
                write = 0;
                buffer_full = false;
                buffer = new Pacchetto[10000];
                postiMax = 2;
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
