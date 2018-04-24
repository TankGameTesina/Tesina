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

namespace PacketTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtNum.Text = "0";
            serverStatus = false;
            serverAddress = null;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            InviaPacchetto("l");
        }

        private const int LOCAL_PORT = 5000; //il solo client cambia l'ordine, local 5001 e remote 5000
        private const int REMOTE_PORT = 5001;
        private const string NOME_LDAP_DOMINIO = "LDAP://itis/DC=itis, DC=pr, DC=it";

        int index = 0;
        int intUdpLocalPort;
        int intUdpRemotePort;
        string myAddress;
        string strIpRemote;       // l'indirizzo IP del destinatario
        bool gaming;
        int myId;
        string myMask;
        string myBroadcast;

        #region VARIABILISERVER
        bool serverStatus;
        string serverAddress;
        //Pacchetto[] buffer;
        int read;
        int write;
        //List<Tank> players;
        //List<Bullet> bullets;
        int move;
        int postiMax;
        #endregion

        Socket udpSocket;     // socket per ricevere e trasmettere
        EndPoint ep;          // l'endpoint dell'altro capo (sia in ricezione che in spedizione) da usare con le routine asincrone di C#
        byte[] abytRx = new byte[1024];  // il buffer di ricezione
        byte[] abytTx = new byte[1024];  // il buffer di spedizione

        private string username;

        private void Analisi(string msg, string address)
        {
            var ann = new String[1];
            ann[0] = msg;

            listBox1.Items.Insert(0, "Pacchetto da " + address + " contiene " + msg);

            if (msg.Contains(';'))
                ann = msg.Split(';');

            switch (ann[0])
            {
                case "l":
                    if (serverStatus)
                        InviaPacchetto("0;" + myAddress); //se dopo n secondi non risponde creo l'host
                    break;

                case "0": //messaggi dal server
                    switch (ann[1])
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
                            strIpRemote = ann[1];
                            MessageBox.Show("PacketTest : trovato il server e impostato");
                            timer1.Enabled = true;

                            break;
                    }
                    break;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            strIpRemote = IPAddress.Loopback.ToString();
            intUdpLocalPort = LOCAL_PORT;
            intUdpRemotePort = REMOTE_PORT;
            myAddress = clsWin32_Network.GetMyIp();
            username = clsWin32_Network.GetMyHostName();
            clsWin32_Network.GetMaskBroadcast(myAddress, ref myMask, ref myBroadcast);

            Bind();
        }

        private void InviaPacchetto(string pacchetto)
        {
            //string strMessage = "";
            IPEndPoint ipEP;
            IPAddress ipAddress;

            // Ecco l'IPaddress dalla stringa con l'indirizzo IP
            // o invio ad ip preciso o invio in broadcast
            if (pacchetto == "l")
                ipAddress = IPAddress.Parse(myBroadcast);
            else
                ipAddress = IPAddress.Parse(strIpRemote);

            // L'endpoint remoto a cui spedire
            ipEP = new IPEndPoint(ipAddress, intUdpRemotePort);
            ep = (EndPoint)ipEP;

            try
            {
                abytTx = Encoding.UTF8.GetBytes(pacchetto);
                // Spedizione asincrona del buffer di byte
                udpSocket.BeginSendTo(abytTx, 0, pacchetto.Length, SocketFlags.None, ep, new AsyncCallback(OnSend), null);
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

        private void timer1_Tick(object sender, EventArgs e)
        { 
                InviaPacchetto("r;test");
                index++;
                txtNum.Text = "" + index;
        }
    }
}
