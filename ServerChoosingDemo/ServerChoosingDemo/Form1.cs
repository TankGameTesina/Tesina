using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TankGame;

namespace ServerChoosingDemo
{
    public partial class Form1 : Form
    {

        private const int LOCAL_PORT = 5000; //il solo host cambia l'ordine, local 5001 e remote 5000
        private const int REMOTE_PORT = 5001;
        private const string NOME_LDAP_DOMINIO = "LDAP://itis/DC=itis, DC=pr, DC=it";

        #region VARAIABILIGIOCO
        string myAddress;
        string serverAddress;
        //bool gaming;
        int myId;
        string myMask;
        string myBroadcast;
        private string username;
        int serverTick;
        List<Tank> players;
        List<Bullet> bullets;
        #endregion

        #region VARIABILIUDP
        Socket udpSocket;
        EndPoint ep;
        byte[] abytRx = new byte[1024];
        byte[] abytTx = new byte[1024];
        int intUdpLocalPort;
        int intUdpRemotePort; //la porta IN del server
        #endregion

        #region VARIABILISERVER
        bool serverStatus;
        Pacchetto[] buffer;
        int read;
        int write;
		bool buffer_full;
        Thread reader;
        int move;
        int postiMax;
        #endregion

        
        int index = 0; //counter per i pacchetti arrivati o spediti

        #region FORM
        public Form1()
        {
            InitializeComponent();
            serverStatus = false;
            players = new List<Tank>();
            bullets = new List<Bullet>();
            serverTick = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            intUdpLocalPort = LOCAL_PORT;
            intUdpRemotePort = REMOTE_PORT;

            myAddress = clsWin32_Network.GetMyIp();
            username = clsWin32_Network.GetMyHostName();
            clsWin32_Network.GetMaskBroadcast(myAddress, ref myMask, ref myBroadcast);
            players.Add(new Tank("server"));
            //gaming = false;

            this.Left = 0;
            this.Top = 0;

            lbRicerca.Text = "Nessuna ricerca in corso";
            lbServer.Text = "Server non trovato";
            lbInvio.Text = "Non sto inviando";
            lbPkt.Text = "Pacchetti inviati " + index;
            lbDemo.Text = "Demo su rete";
            lbRead.Visible = false;
            lbWrite.Visible = false;

            Bind();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serverStatus)
            {
                serverStatus = false;
                reader.Abort();
            }
        }

        #endregion

        #region LOGIC
        private delegate void del_Analisi(Pacchetto pkt); //USATO PER RICHIAMARLO DA UN THREAD ESTERNO E INTERAGIRE CON I COMPONENTI
        private void Analisi(Pacchetto pkt)
        {
            Tank app;
            Bullet blt;
            Posizione pos;
                       
            lbDebug.Items.Insert(0, "Ricevuto un pacchetto " + pkt.ToString());
            lbRead.Text = "Indice read " + read;
            if (serverStatus)
            {
                index++;
                lbPkt.Text = "Pacchetti ricevuti " + index;
            }

            var ann = new String[1];
            ann[0] = pkt.Msg;

            if (pkt.Msg.Contains(';'))
                ann = pkt.Msg.Split(';');

            switch (ann[0])
            {
                case "l":
                    if (serverStatus)
                    {
                        if(cbLocal.Checked)
                            InviaPacchetto(new Pacchetto("0;" + myAddress, pkt.Address), Convert.ToInt32(ann[1]));
                        else
                            InviaPacchetto(new Pacchetto("0;" + myAddress, pkt.Address));
                    }
                        
                    break;

                case "0": //messaggi dal server
                    switch (ann[1])
                    {
                        case "acc": //gaming true o comunque sono in partita
                            myId = Convert.ToInt32(ann[2]);
                            break;

                        case "ref":
                            //rimango nel menu, gli spazi sono pieni oppure sono gia all'interno
                            break;

                        case "str":
                            //gaming = true;
                            break;

                        default:
                            serverAddress = pkt.Address;
                            tmrServer.Enabled = false;
                            lbRicerca.Text = "Ricerca conclusa";
                            lbServer.Text = "Server trovato";
                            btnSend.Enabled = true;
                            btnLooking.Enabled = true;
                            serverTick = 0;
                            break;
                    }
                    break;

                case "r":
                    if (serverStatus)
                    {
                        app = new Tank(ann[1]);

                        if (players.Count < postiMax && !players.Contains(app))
                        {
                            players.Add(app);
                            app.Id = players.IndexOf(app);
                            
                            if(cbLocal.Checked)
                                InviaPacchetto(new Pacchetto("0;acc;" + app.Id, pkt.Address), Convert.ToInt32(ann[2]));// per testring locale
                            else
                                InviaPacchetto(new Pacchetto("0;acc;" + app.Id, pkt.Address));
                        }
                        else
                        {                            
                            if (cbLocal.Checked)
                                InviaPacchetto(new Pacchetto("0;ref", pkt.Address), Convert.ToInt32(ann[2])); //per testing locale
                            else
                                InviaPacchetto(new Pacchetto("0;ref" + app.Id, pkt.Address));
                        }
                    }

                    break;

                case "for":
                    app = players.FirstOrDefault(x => x.Address == pkt.Address);
                    if (app != null)
                    {
                        pos = app.Pos + move;
                        if (VerifyMovement(pos))
                        {
                            //e a tutti quelli all'interno della lobby
                            foreach (Tank tnk in players.Where(x => x.Id != myId))
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
                            pos = app.Pos - move;
                            if (VerifyMovement(pos))
                            {
                                foreach (Tank tnk in players.Where(x => x.Id != myId))
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

                        foreach (Tank tnk in players.Where(x => x.Id != myId))
                        {
                            InviaPacchetto(new Pacchetto(app.Id + ";dct", tnk.Address));
                        }

                    }
                    break;

                case "fire":
                    app = players.FirstOrDefault(x => x.Address == pkt.Address);
                    if (app != null)
                    {
                        int id = bullets.Count == 0 ? 0 : bullets[bullets.Count - 1].Id + 1;

                        blt = new Bullet(id, app.Id, app.Pos);
                        bullets.Add(blt);

                        foreach (Tank tnk in players.Where(x => x.Id != myId))
                        {
                            InviaPacchetto(new Pacchetto("blt;" + blt.Id + ";pos;" + blt.Pos.ToString(), tnk.Address));
                        }
                    }

                    break;

                case "blt":
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
                    break;

                default:
                    //metto qua i controlli per i vari giocatori, controllo se esiste in primis, lato client

                    if (players.Contains(players.FirstOrDefault(x => x.Id == Convert.ToInt32(ann[0]))))
                    {
                        app = players.FirstOrDefault(x => x.Id == Convert.ToInt32(ann[0]));

                        if (ann[1] == "pos")
                        {
                            app.Pos = new Posizione(Convert.ToInt32(ann[2]), Convert.ToInt32(ann[3]), Convert.ToInt32(ann[4]));
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
                                        players.Add(new Tank(ann[2]));
                                    }
                                }
                            }
                        }

                    }

                    break;
            }
        }

        private bool VerifyMovement(Posizione pos) //passo la posizione da varificare
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

            lbDebug.Items.Insert(0, "Invio pacchetto " + pkt.ToString());

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
                    BeginInvoke(new del_Analisi(Analisi), buffer[read]);
                    read++;
                    if (read == 9999)
					{
						read = 0;
						buffer_full = false;
					}
                       
                }
            }

            return;
        }

        #endregion

        #region UDPMETODS
        private delegate void del_OnReceive(IAsyncResult ar);
        private void OnReceive(IAsyncResult ar)
        {
            if (InvokeRequired)  // Per gestire il crossthread (questa routine è chiamata da un altro thread)
            {
                BeginInvoke(new del_OnReceive(OnReceive), ar);

                return;
            }

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
                    write++;

                    lbWrite.Text = "Indice write " + write;
                    if (write == 9999)
					{
						write = 0;
						buffer_full = true;
					}
                        
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
            if (InvokeRequired)  // Per gestire il crossthread (questa routine è chiamata da un altro thread)
            {
                BeginInvoke(new del_OnSend(OnSend), ar);
                return;
            }

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
                this.Left += this.Width + 50;
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
        private void tmrServer_Tick(object sender, EventArgs e)
        {
            if(serverTick < 10)
            {
                if (cbLocal.Checked)
                {
                    InviaPacchetto(new Pacchetto("l;" + intUdpLocalPort, myBroadcast));
                }
                else
                {
                    InviaPacchetto(new Pacchetto("l", myBroadcast));
                }

                serverTick++;
                lbDebug.Items.Insert(0, "Nessuna risposta, riprovo altre " + (10 - serverTick) + " volte");
            }
            else //sotto se non trovo il server dopo 5 secondi, 10 tick
            {
                tmrServer.Enabled = false;

                lbDebug.Items.Insert(0, "Nessuna risposta, mi proclamo server");
                lbRicerca.Text = "Non in ricerca";
                lbServer.Text = "Autoproclamato server";
                lbPkt.Text = "Pacchetti ricevuti " + index;
                lbRead.Visible = true;
                lbWrite.Visible = true;
                lbRead.Text = "Indice read " + read;
                lbWrite.Text = "Indice write " + write;
                serverStatus = true;
                read = 0;
                write = 0;
				buffer_full = false;
                buffer = new Pacchetto[10000];
                postiMax = 2;
                move = 5;
                myId = 0;
                serverTick = 0;

                btnLooking.Enabled = true;
                btnSend.Enabled = true;

                udpSocket = null;
                intUdpRemotePort = intUdpLocalPort; // la porta al momento assegnatami... tanto la cambio ad ogni invio
                intUdpLocalPort = REMOTE_PORT; //5001 ricevo
                //intUdpRemotePort = LOCAL_PORT; //5000 invio

                Bind();

                reader = new Thread(() => ServerAnalisi());
                reader.Start();
            }
        }

        private void tmrInvio_Tick(object sender, EventArgs e)
        {
            if(cbLocal.Checked)
            {
                InviaPacchetto(new Pacchetto("r;test;" + intUdpLocalPort, serverAddress));
            }
            else
            {
                InviaPacchetto(new Pacchetto("r;test", serverAddress));
            }
            
            lbDebug.Items.Insert(0, "Invio pacchetto al server");
            index++;
            lbPkt.Text = "Pacchetti inviati " + index;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(tmrInvio.Enabled)
            {
                tmrInvio.Enabled = false;
                lbInvio.Text = "Invio pacchetti in pausa";
            }
            else
            {
                tmrInvio.Enabled = true;
                lbDebug.Items.Insert(0, "Inizio ad inviare al server un pacchetto ogni " + tmrInvio.Interval + " millisecondi");
                lbInvio.Text = "Sto inviando pacchetti";
            }                
        }

        private void btnLooking_Click(object sender, EventArgs e)
        {
            lbDebug.Items.Insert(0, "Inizio la ricerca del server");
            lbRicerca.Text = "Ricercando il server";
            btnSend.Enabled = false;
            btnLooking.Enabled = false;
            tmrServer.Enabled = true;
        }

        private void cbLocal_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLocal.Checked)
                lbDemo.Text = "Demo su localhost";
            else
                lbDemo.Text = "Demo su rete";
        }
        #endregion
    }
}
