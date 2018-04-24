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

namespace ProtocolloCarroArmato
{
    public partial class Form1 : Form
    {
        private const int LOCAL_PORT = 5000; //il solo host cambia l'ordine, local 5001 e remote 5000
        private const int REMOTE_PORT = 5001;
        private const string NOME_LDAP_DOMINIO = "LDAP://itis/DC=itis, DC=pr, DC=it";

        int intUdpLocalPort;
        int intUdpRemotePort;
        string myAddress;
        string serverAddress;
        bool gaming;
        int myId;
        string myMask;
        string myBroadcast;
        List<Tank> players;
        List<Bullet> bullets;

        #region VARIABILISERVER
        bool serverStatus;
        Pacchetto[] buffer;
        int read;
        int write;
        Thread reader;
        int move;
        int postiMax;
        #endregion

        Socket udpSocket;     // socket per ricevere e trasmettere
        EndPoint ep;          // l'endpoint dell'altro capo (sia in ricezione che in spedizione) da usare con le routine asincrone di C#
        byte[] abytRx = new byte[1024];  // il buffer di ricezione
        byte[] abytTx = new byte[1024];  // il buffer di spedizione

        private string username;

        public Form1()
        {
            InitializeComponent();
            serverStatus = false;
            players = new List<Tank>();
            bullets = new List<Bullet>();
            textBox1.Text = LOCAL_PORT.ToString();
            textBox2.Text = REMOTE_PORT.ToString();
            textBox3.Text = IPAddress.Loopback.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            intUdpLocalPort = LOCAL_PORT;
            intUdpRemotePort = REMOTE_PORT;
            myAddress = clsWin32_Network.GetMyIp();
            username = clsWin32_Network.GetMyHostName();
            clsWin32_Network.GetMaskBroadcast(myAddress, ref myMask, ref myBroadcast);

            Bind();
        }

        private void btnInvio_Click(object sender, EventArgs e)
        {
            var msg = txtProtocollo.Text;

            InviaPacchetto(new Pacchetto(msg, myAddress));
        }

        private void Analisi(Pacchetto pkt)
        {
            Tank app;
            Bullet blt;
            Posizione pos;

            var ann = new String[1];
            ann[0] = pkt.Msg;

            if (pkt.Msg.Contains(';'))
                ann = pkt.Msg.Split(';');

            switch (ann[0]) 
            {
                case "l":
                    if (serverStatus)
                        InviaPacchetto(new Pacchetto("0;" + myAddress, pkt.Address));
                    break;

                case "0": //messaggi dal server
                    switch(ann[1])
                    {
                        case "acc":
                            myId = Convert.ToInt32(ann[2]);
                            break;

                        case "ref":
                            //rimango nel menu, gli spazi sono pieni oppure sono gia all'interno
                            break;

                        case "str":
                            gaming = true;
                            break;

                        default:
                            serverAddress = pkt.Address;
                            break;
                    }
                    break;

                case "r":
                    if(serverStatus)
                    {
                        app = new Tank(ann[1]);
                        
                        if (players.Count < postiMax && !players.Contains(app))
                        {
                            players.Add(app);
                            app.Id = players.IndexOf(app);
                            InviaPacchetto(new Pacchetto("0;acc;"+app.Id, pkt.Address));
                        }
                        else
                            InviaPacchetto(new Pacchetto("0;ref", pkt.Address));
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
                            foreach(Tank tnk in players.Where(x => x.Id != myId))
                            {
                                InviaPacchetto(new Pacchetto(app.Id + ";pos;" + pos.ToString(), tnk.Address));
                            }

                            Analisi(new Pacchetto(app.Id + ";pos;" + pos.ToString(), serverAddress)); // da lato client host effettuo la mossa... sono avvantaggiato come su COD
                        }
                        
                    }
                    break;

                case "bck":
                    if(serverStatus)
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
                    if(app != null)
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
                        if(ann[2] == "0")
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

            if (pkt.Msg == "l")
                ipAddress = IPAddress.Parse(myBroadcast);
            else
                ipAddress = IPAddress.Parse(pkt.Address);

            ipEP = new IPEndPoint(ipAddress, intUdpRemotePort);
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
            while(serverStatus)
            {
                if(read < write)
                {
                    Analisi(buffer[read]);
                    read++;
                    read = read++ == 10000 ? read % 10000 : read;
                }
            }

            return;
        }

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

                if (serverStatus && intUdpLocalPort == LOCAL_PORT)
                {
                    intUdpLocalPort = REMOTE_PORT;
                    intUdpRemotePort = LOCAL_PORT;

                    Bind();

                    lbDebug.Items.Insert(0, "Sono l'host");
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

                if(serverStatus)
                {
                    buffer[write] = new Pacchetto(strReceived, astr[0]);
                    write = write++ == 10000 ? write % 10000 : write;
                }
                else
                    Analisi(new Pacchetto(strReceived, astr[0]));
                

                abytRx = new byte[abytRx.Length];
                udpSocket.BeginReceiveFrom(abytRx, 0, abytRx.Length, SocketFlags.None, ref ep, new AsyncCallback(OnReceive), ep);
            }
            catch (ObjectDisposedException ex)
            {
                MessageBox.Show("OnReceive(): Eccezione ObjectDisposedException\n" + ex.Message);
                Bind();
            }
            catch (Exception ex)
            {
                MessageBox.Show("OnReceive(): Eccezione Exception\n" + ex.Message);
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
                MessageBox.Show("Bind(): eccezione SocketException\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bind(): eccezione Exception\n" + ex.Message);
            }
        }

        private void txtProtocollo_TextChanged(object sender, EventArgs e)
        {

        }

        private void tmrHost_Tick(object sender, EventArgs e)
        {
            tmrHost.Enabled = false;

            serverStatus = true;
            read = 0;
            write = 0;
            buffer = new Pacchetto[10000];
            postiMax = 2;
            move = 5;
            myId = 0;

            udpSocket.Close();
            
            textBox1.Text = REMOTE_PORT.ToString();
            textBox2.Text = LOCAL_PORT.ToString();

            reader = new Thread(() => ServerAnalisi());
            reader.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            intUdpLocalPort = Convert.ToInt32(textBox1.Text);
            intUdpRemotePort = Convert.ToInt32(textBox2.Text);

            Bind();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InviaPacchetto(new Pacchetto("l", myBroadcast));
            button2.Enabled = false;
            tmrHost.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(serverStatus)
            {
                serverStatus = false;
                reader.Abort();
            }
        }
    }
}
