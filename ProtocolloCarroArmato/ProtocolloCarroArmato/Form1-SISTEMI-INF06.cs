using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TankGame;

namespace ProtocolloCarroArmato
{
    public partial class Form1 : Form
    {
        private const int LOCAL_PORT = 5000; //il solo client cambia l'ordine, local 5001 e remote 5000
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

        #region VARIABILISERVER
        bool serverStatus;
        Pacchetto[] buffer;
        int read;
        int write;
        List<Tank> players;
        List<Bullet> bullets;
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
            textBox1.Text = LOCAL_PORT.ToString();
            textBox2.Text = REMOTE_PORT.ToString();
            textBox3.Text = IPAddress.Loopback.ToString();
        }

        private void btnInvio_Click(object sender, EventArgs e)
        {
            var msg = txtProtocollo.Text;

            InviaPacchetto(new Pacchetto(msg, serverAddress));
        }

        private void Analisi(string msg, string address)
        {
            Tank app;
            Bullet blt;
            Posizione pos;

            var ann = new String[1];
            ann[0] = msg;

            if (msg.Contains(';'))
                ann = msg.Split(';');

            switch (ann[0]) 
            {
                case "l":
                    if (serverStatus)
                        InviaPacchetto(new Pacchetto("0;" + myAddress, address));
                    break;

                case "0": //messaggi dal server
                    switch(ann[1])
                    {
                        case "acc":
                            myId = Convert.ToInt32(ann[2]);
                            break;

                        case "ref":
                            //rimango nel menu, gli spazi sono pieni
                            break;

                        case "str":
                            gaming = true;
                            break;

                        default:
                            serverAddress = address;
                            break;
                    }
                    break;

                case "r":
                    if(serverStatus)
                    {
                        if (players.Count < postiMax)
                        {
                            app = new Tank(ann[1]);
                            players.Add(app);
                            InviaPacchetto(new Pacchetto("0;acc;"+app.Id, serverAddress));
                        }
                        else
                            InviaPacchetto(new Pacchetto("0;ref", serverAddress));
                    }

                    break;

                case "for":
                    lbDebug.Items.Insert(0, "Ricevuto pacchetto for");
                    app = players.FirstOrDefault(x => x.Address == address);
                    if (app != null)
                    {
                        pos = app.Pos + move;
                        if (VerifyMovement(pos))
                        {
                            InviaPacchetto(new Pacchetto(app.Id + ";pos;"+ pos.ToString(), serverAddress));
                        }
                        
                    }
                    break;

                case "bck":
                    app = players.FirstOrDefault(x => x.Address == address);
                    if (app != null)
                    {
                        pos = app.Pos - move;
                        if (VerifyMovement(pos))
                        {
                            InviaPacchetto(new Pacchetto(app.Id + ";pos;" + pos.ToString(), serverAddress));
                        }
                    }
                    break;

                case "dct":
                    app = players.FirstOrDefault(x => x.Address == address);
                    if (app != null)
                    {
                        players.Remove(app);
                        InviaPacchetto(new Pacchetto(app.Id+";dct", serverAddress));
                    }
                    break;

                case "fire":
                    app = players.FirstOrDefault(x => x.Address == address);
                    if(app != null)
                        bullets.Add(new Bullet(bullets[bullets.Count - 1].Id+1, app.Id, app.Pos));
                    break;

                case "blt":
                    blt = bullets.FirstOrDefault(x => x.Id == ann[1]);
                    if (ann[2] == "pos")
                    {
                        lbDebug.Items.Add("Proiettile in nuova posizione");
                        blt.Pos = new Posizione(Convert.ToInt32(ann[3]), Convert.ToInt32(ann[4]), Convert.ToInt32(ann[5]));
                    }
                    else
                    {
                        if(ann[2] == "0")
                        {
                            lbDebug.Items.Add("Nessun giocatore colpito");
                            bullets.Remove(blt); // rimuovo il proiettile poi rimuoverò anche l'immagine
                        }
                        else
                            lbDebug.Items.Add("Giocatore colpito" + ann[2]);
                    }
                    break;

                default:
                    //metto qua i controlli per i vari giocatori, controllo se esiste in primis, lato client
                    
                     if(players.Contains(players.FirstOrDefault(x => x.Id == ann[0])))
                     {
                        app = players.FirstOrDefault(x => x.Id == ann[0]);

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
                                    MessageBox.Show(app.User + "ha vinto");
                                }
                                else
                                {
                                    if (ann[1] == "user")
                                    {
                                        app.User = ann[2];
                                    }
                                } 
                                    
                             }
                        }

                        }
                     else
                        lbDebug.Items.Add("Messaggio invalido");

                    break;
            }                
        }

        private bool VerifyMovement(Posizione pos) //passo la posizione da varificare
        {
            //verifica se la posizione è giusta

            return true;
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

        private void InviaPacchetto(Pacchetto pkt)
        {
            //string strMessage = "";
            IPEndPoint ipEP;
            IPAddress ipAddress;

            // Ecco l'IPaddress dalla stringa con l'indirizzo IP
            // o invio ad ip preciso o invio in broadcast

            if (pkt.Msg == "l")
                ipAddress = IPAddress.Parse(myBroadcast);
            else
                ipAddress = IPAddress.Parse(pkt.Address);

            // L'endpoint remoto a cui spedire
            ipEP = new IPEndPoint(ipAddress, intUdpRemotePort);
            ep = (EndPoint)ipEP;
            lbDebug.Items.Add("Invio : " + pkt.Msg + " a " + pkt.Address);

            try
            {
                abytTx = Encoding.UTF8.GetBytes(pkt.Msg);
                // Spedizione asincrona del buffer di byte
                udpSocket.BeginSendTo(abytTx, 0, pkt.Msg.Length, SocketFlags.None, ep, new AsyncCallback(OnSend), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send(): eccezione Exception\n" + ex.Message);
            }
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

                ipEPRx = new IPEndPoint(IPAddress.Any, 0);
                ep = (EndPoint)ipEPRx;
                // Ecco la fine della ricezione. Ora i dati ricevuti sono nel buffer globale
                udpSocket.EndReceiveFrom(ar, ref ep);

                // Recupero Ip e Porta dell'host remoto
                string[] astr = ep.ToString().Split(':');
                // Ecco il messaggio ricevuto. 
                strReceived = Encoding.UTF8.GetString(abytRx);  // trasformo in stringa i dati ricevuti

                // Prendo solo i caratteri che precedono il carattere nullo (il tipo string non è come l'array di char del C
                idx = strReceived.IndexOf((char)0);
                if (idx > -1)
                    strReceived = strReceived.Substring(0, idx);

                Analisi(strReceived, astr[0]); //se errore qua, devo svolgere nel catch le due istruzioni sotto

                // Reinizializzo il buffer con zeri, per evitare che la prossima ricezione sovrapponga la precedente
                abytRx = new byte[abytRx.Length];
                // Riassocio la routine di ricezione
                udpSocket.BeginReceiveFrom(abytRx, 0, abytRx.Length, SocketFlags.None, ref ep, new AsyncCallback(OnReceive), ep);
            }
            catch (ObjectDisposedException ex)
            {
                MessageBox.Show("OnReceive(): Eccezione ObjectDisposedException\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("OnReceive(): Eccezione Exception\n" + ex.Message);
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

        public void resetSocket()
        {
            // Reset Socket per gli errori OnReceive()
            udpSocket.Shutdown(SocketShutdown.Both);
            udpSocket.Close();
            udpSocket = null;
            Bind();
        }

        private void Bind() //da effettuare solo quando cambio la porta locale
        {
            try
            {
                IPEndPoint ipEP;
                // Creazione socket Udp
                udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                // L'endpoint locale per la ricezione
                ipEP = new IPEndPoint(IPAddress.Any, intUdpLocalPort);
                // Associazione degli indirizzi al socket (per la ricezione): IP locale e Porta Locale
                udpSocket.Bind(ipEP);
                ep = (EndPoint)ipEP;
                // Impostazione della ricezione asincrona sul socket
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
            serverStatus = true;
            intUdpLocalPort = REMOTE_PORT;
            intUdpRemotePort = LOCAL_PORT;
            resetSocket();

            MessageBox.Show("Assunto il ruolo di host");

            tmrHost.Enabled = false;
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
            tmrHost.Enabled = true;
        }
    }
}
