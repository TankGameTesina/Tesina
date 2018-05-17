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
        System.Windows.Forms.Timer lTimer = new System.Windows.Forms.Timer();

        #region VARAIABILIGIOCO
        string myAddress;
        string serverAddress;
        //bool gaming;
        int myId;
        string myMask;
        string myBroadcast;
        private string username;
        int serverTick;
        #endregion

        #region VARIABILIUDP
        Socket udpSocket;
        EndPoint ep;
        byte[] abytRx = new byte[1024];
        byte[] abytTx = new byte[1024];

        int intUdpLocalPort;
        int intUdpRemotePort; //la porta IN del server

        private const int LOCAL_PORT = 5000; //il solo host cambia l'ordine, local 5001 e remote 5000
        private const int REMOTE_PORT = 5001;
        private const string NOME_LDAP_DOMINIO = "LDAP://itis/DC=itis, DC=pr, DC=it";
        #endregion

        #region VARIABILISERVER
        bool serverStatus;
        Pacchetto[] buffer;
        int read;
        int write;
        bool buffer_full;
        int move;
        Thread reader;
        int postiMax;
        #endregion

        #region VARIABILIGRAFICA
        Form form;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private List<GameObject> players = new List<GameObject>();
        private Texture2D background;

        static public MouseState mouseState;
        static public MouseState previousState;
        private int punteggio = 0;

        private const int altezzaSchermo = 1080;
        private const int larghezzaSchermo = 1920;

        #endregion

        #region GRAFICA
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
            previousState = mouseState;
            mouseState = Mouse.GetState();

            for (int i = 0; i < players.Count; i++)
            {
                players[i].Update();
            }

            if(players.Count != 0)
            {
                if (previousState != mouseState)
                    InviaPacchetto(new Pacchetto("ang;" + players.FirstOrDefault(x => x.Id == myId).setAngle(), serverAddress));

                if (keystate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                {
                    InviaPacchetto(new Pacchetto("for", serverAddress));
                    /*for (int i = 0; i < numEnemyTanks; i++)
                    {
                        //gestione collisioni
                        if (!playerTank.Colliding(players[i]))
                        {
                            playerTank.Update();
                            players[i].Update();
                            playerTank.moveForward();
                            //enemysTank[i].moveForward();
                        }
                    }*/
                }

                if (keystate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                {
                    InviaPacchetto(new Pacchetto("bck", serverAddress));
                }

                if (previousState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    /*playerTank.newBullet();
                    for (int i = 0; i < numEnemyTanks; i++)
                    {
                        players[i].newBullet();
                    }*/
                }

                List<BulletObject> bulletRimuovere = new List<BulletObject>();
                /*
                foreach (BulletObject bullet in playerTank.bullets)
                {
                    for (int i = 0; i < numEnemyTanks; i++)
                    {
                        if (bullet.Colliding(players[i]))
                        {
                            punteggio++;
                            bulletRimuovere.Add(bullet);
                            break;
                        }
                    }
                }*/

                /*foreach (BulletObject bullet in bulletRimuovere)
                    playerTank.removeBullet(bullet);*/

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

            for (int i = 0; i < players.Count; i++)
            {
                players[i].Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (serverStatus)
            {
                serverStatus = false;
                reader.Abort();
            }
            InviaPacchetto(new Pacchetto("dct", serverAddress));
            base.OnExiting(sender, args);
        }

        private string PosizioneIniziale()
        {
            Random rand = new Random();
            bool collide = true;
            GameObject tank = new GameObject();
            int alt;
            int larg;

            //fino a che non trovo una posizione valida continuo a cercare 
            while (collide)
            {
                larg = rand.Next(larghezzaSchermo);
                alt = rand.Next(altezzaSchermo);
                tank = new GameObject(new Rectangle(larg, alt, 19, 41), new Vector2(larg, alt));
                collide = false;

                foreach (GameObject g in players)
                {
                    if (tank.Colliding(g))
                    {
                        collide = true;
                        break;
                    }
                }
            }

            return tank.ToString();
        }
        #endregion

        #region LOGIC
        private void Analisi(Pacchetto pkt)
        {
            GameObject app;
            BulletObject blt;
            string pos;
            float grade;


            var ann = new String[1];
            ann[0] = pkt.Msg;

            if (pkt.Msg.Contains(';'))
                ann = pkt.Msg.Split(';');

            switch (ann[0])
            {
                case "l":
                    if (serverStatus)
                    {
                        InviaPacchetto(new Pacchetto("0;" + myAddress, pkt.Address));                        
                    }

                    break;

                case "0": //messaggi dal server
                    switch (ann[1])
                    {
                        case "acc": //gaming true o comunque sono in partita
                            GameObject player = new GameObject();
                            player.Init(new Vector2(19, 41), Content.Load<Texture2D>("tank"), Content.Load<Texture2D>("bulletMin"), "Pippo", myAddress);
                            myId = Convert.ToInt32(ann[2]);
                            player.Id = myId;
                            players.Add(player);
                            break;

                        case "ref":
                            //rimango nel menu, gli spazi sono pieni oppure sono gia all'interno
                            break;

                        case "str":
                            //gaming = true;
                            break;

                        default:
                            lTimer.Enabled = false;
                            serverAddress = pkt.Address;

                            if (serverTick != 0)
                                InviaPacchetto(new Pacchetto("r;" + username, serverAddress));

                            serverTick = 0;
                            
                            break;
                    }
                    break;

                case "r":
                    if (serverStatus)
                    {
                        app = new GameObject();
                        
                        if (players.Count < postiMax && !players.Contains(app))
                        {
                            app.Init(new Vector2(19, 41), Content.Load<Texture2D>("tank"), Content.Load<Texture2D>("bulletMin"), ann[1], pkt.Address);
                            players.Add(app);
                            app.Id = players.IndexOf(app);
                            InviaPacchetto(new Pacchetto("0;acc;" + app.Id, pkt.Address));
                            InviaPacchetto(new Pacchetto(app.Id + ";pos;" + PosizioneIniziale() , pkt.Address));
                        }
                        else
                        {
                           InviaPacchetto(new Pacchetto("0;ref" + app.Id, pkt.Address));
                        }
                    }

                    break;

                case "for":
                    app = players.FirstOrDefault(x => x.Address == pkt.Address);
                    if (app != null)
                    {
                        pos = app.moveForward();
                        if (VerifyMovement(pos))
                        {
                            //e a tutti quelli all'interno della lobby
                            foreach (GameObject tnk in players.Where(x => x.Id != myId))
                            {
                                InviaPacchetto(new Pacchetto(app.Id + ";pos;" + pos.ToString(), tnk.Address));
                            }

                            Analisi(new Pacchetto(app.Id + ";pos;" + pos.ToString(), serverAddress)); // da lato client host effettuo la mossa... sono avvantaggiato come su COD
                        }

                    }
                    break;

                case "bck":
                    if (serverStatus)
                    {
                        app = players.FirstOrDefault(x => x.Address == pkt.Address);
                        if (app != null)
                        {
                            pos = app.moveBack();
                            if (VerifyMovement(pos))
                            {
                                foreach (GameObject tnk in players.Where(x => x.Id != myId))
                                {
                                    InviaPacchetto(new Pacchetto(app.Id + ";pos;" + pos.ToString(), tnk.Address));
                                }

                                Analisi(new Pacchetto(app.Id + ";pos;" + pos.ToString(), serverAddress));
                            }
                        }
                    }

                    break;

                case "dct":
                    app = players.FirstOrDefault(x => x.Address == pkt.Address);
                    if (app != null)
                    {
                        players.Remove(app);

                        foreach (GameObject tnk in players.Where(x => x.Id != myId))
                        {
                            InviaPacchetto(new Pacchetto(app.Id + ";dct", tnk.Address));
                        }

                    }
                    break;

                case "fire":
                    /*app = players.FirstOrDefault(x => x.Address == pkt.Address);
                    if (app != null)
                    {
                        int id = bullets.Count == 0 ? 0 : bullets[bullets.Count - 1].Id + 1;

                        blt = new Bullet(id, app.Id, app.Pos);
                        bullets.Add(blt);

                        foreach (Tank tnk in players.Where(x => x.Id != myId))
                        {
                            InviaPacchetto(new Pacchetto("blt;" + blt.Id + ";pos;" + blt.Pos.ToString(), tnk.Address));
                        }
                    }*/

                    break;

                case "blt":
                    /*
                    blt = bullets.FirstOrDefault(x => x.Id == Convert.ToInt32(ann[1]));
                    if (ann[2] == "pos")
                    {
                        blt.Pos = new Posizione(Convert.ToInt32(ann[3]), Convert.ToInt32(ann[4]), Convert.ToInt32(ann[5]));

                        foreach (Tank tnk in players.Where(x => x.Id != myId))
                        {
                            InviaPacchetto(new Pacchetto("blt;" + blt.Id + ";pos;" + blt.Pos.ToString(), tnk.Address));
                        }
                    }
                    else
                    {
                        if (ann[2] == "0")
                        {
                            //colpo a vuoto
                            bullets.Remove(blt); // rimuovo il proiettile poi rimuoverò anche l'immagine

                            foreach (Tank tnk in players.Where(x => x.Id != myId))
                            {
                                InviaPacchetto(new Pacchetto("blt;" + blt.Id + ";0", tnk.Address));
                            }
                        }
                        else
                        {
                            //colpo a segno contro ID in ann[2]

                            foreach (Tank tnk in players.Where(x => x.Id != myId))
                            {
                                InviaPacchetto(new Pacchetto("blt;" + blt.Id + ";" + ann[2], tnk.Address));
                            }

                            //creare funzione verifica colpo
                        }

                    }
                    */
                    break;

                default:
                    //metto qua i controlli per i vari giocatori, controllo se esiste in primis, lato client
                    List<int> giocatori = new List<int>();
                    if (players.Contains(players.FirstOrDefault(x => x.Id == Convert.ToInt32(ann[0]))))
                    {
                        app = players.FirstOrDefault(x => x.Id == Convert.ToInt32(ann[0]));

                        if (ann[1] == "pos")
                        {
                            app.Position = new Vector2(Convert.ToInt32(ann[2]), Convert.ToInt32(ann[3]));
                            app.BoundingBox = new Rectangle(Convert.ToInt32(ann[2]), Convert.ToInt32(ann[3]), 19, 41);
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
                                else
                                {
                                    if (ann[1] == "user")
                                    {

                                    }
                                }
                            }
                        }

                    }

                    break;
            }
        }

        private bool VerifyMovement(string pos) //passo la posizione da varificare
        {
            //verifica se la posizione è giusta

            return true;
        }

        private void InviaPacchetto(Pacchetto pkt)
        {
            IPEndPoint ipEP;
            IPAddress ipAddress;

            ipAddress = IPAddress.Parse(pkt.Address);

            ipEP = new IPEndPoint(ipAddress, intUdpRemotePort); //porta settata di norma su 5000
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

        private void InviaPacchetto(Pacchetto pkt, int port) // metodo da usare per la demo multihost in locale
        {
            IPEndPoint ipEP;
            IPAddress ipAddress;

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

        public void ServerAnalisi()
        {
            while (serverStatus)
            {
                if (read < write || buffer_full)
                {
                    Analisi(buffer[read]);

                    if (read == 99)//9999
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

            try
            {
                string strReceived;
                int idx;
                IPEndPoint ipEPRx;

                if (udpSocket == null)
                {
                    return;
                }

                ipEPRx = new IPEndPoint(IPAddress.Any, 0);
                ep = (EndPoint)ipEPRx;

                udpSocket.EndReceiveFrom(ar, ref ep);

                string[] astr = ep.ToString().Split(':');
                strReceived = Encoding.UTF8.GetString(abytRx);


                idx = strReceived.IndexOf((char)0);
                if (idx > -1)
                    strReceived = strReceived.Substring(0, idx);

                if (serverStatus)
                {
                    buffer[write] = new Pacchetto(strReceived, astr[0]);

                    if (write == 99)//9999
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
            /*if (InvokeRequired)  // Per gestire il crossthread (questa routine è chiamata da un altro thread)
            {
                BeginInvoke(new del_OnSend(OnSend), ar);
                return;
            }*/

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
            if (serverTick < 10)
            {
                InviaPacchetto(new Pacchetto("l", myBroadcast));
                
                serverTick++;
            }
            else //sotto se non trovo il server dopo 5 secondi, 10 tick
            {
                //tmrServer.Enabled = false;
                lTimer.Enabled = false;

                serverStatus = true;
                read = 0;
                write = 0;
                buffer_full = false;
                buffer = new Pacchetto[10000];
                postiMax = 2;
                move = 5;
                myId = 0;
                serverTick = 0;

                udpSocket = null;
                //intUdpRemotePort = intUdpLocalPort; // la porta al momento assegnatami... tanto la cambio ad ogni invio
                intUdpLocalPort = REMOTE_PORT; //5001 ricevo
                intUdpRemotePort = LOCAL_PORT; //5000 invio

                Bind();

                reader = new Thread(() => ServerAnalisi());
                reader.Start();
            }
        }
        #endregion
    }
}
