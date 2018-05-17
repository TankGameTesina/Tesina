using System;
using System.Runtime.InteropServices;  // per DllImport e altro
using System.Collections;  // per ArrayList
using System.Security;
// per SuppressUnmanagedCodeSecurityAttribute
//using System.DirectoryServices;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ComponentModel;
using System.DirectoryServices;
using System.Collections.Generic;

static class clsWin32_Network
{
  public static string strMsg;

  #region PINVOKE
  /// <summary>
  /// L'Api NetServerEnum() di Netapi32.dll restituisce tutti i nomi delle macchine in rete (quale, se piu' di una scheda?)
  /// Si puo' specificare quale tipo di macchina (Server e macchine normali) con flag opportuni
  /// </summary>
  /// 
  [DllImport("Netapi32", CharSet = CharSet.Auto, SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
  private static extern int NetServerEnum(string ServerNane, int dwLevel, ref IntPtr pBuf, int dwPrefMaxLen, out int dwEntriesRead, out int dwTotalEntries, int dwServerType, string domain, out int dwResumeHandle);

  /// <summary>
  /// L'Api NetApiBufferFree() di Netapi32.dll dealloca i buffer usati da NetServerEnum()
  /// </summary>
  [DllImport("Netapi32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
  private static extern int NetApiBufferFree(IntPtr pBuf);

  private const int LEVEL_100 = 100;
  private const int MAX_PREFERRED_LENGTH = -1;
  private const int SV_TYPE_WORKSTATION = 1;
  private const int SV_TYPE_SERVER = 2;

  // Struttura _SERVER_INFO_100 STRUCTURE di supporto a NetServerEnum()
  [StructLayout(LayoutKind.Sequential)]
  public struct _SERVER_INFO_100
  {
    internal int sv100_platform_id;  // contiene l'id. di OS, tra PLATFORM_ID_DOS, PLATFORM_ID_OS2, PLATFORM_ID_NT, PLATFORM_ID_OSF, or PLATFORM_ID_VMS in lmcons.h
    [MarshalAs(UnmanagedType.LPWStr)]
    internal string sv100_name;
  }
    #endregion

    public static List<String> GetMultipleIpAddress(AddressFamily familyAddress)
    {
        int i;
        IPAddress[] IPs;
        List<String> result = new List<String>();

        try
        {
            IPs = Dns.GetHostAddresses(Dns.GetHostName());
            if (IPs.Length > 0)
            {
                for (i = 0; i < IPs.Length; i++)
                {
                    if (IPs[i].AddressFamily == familyAddress)
                    {
                        result.Add(IPs[i].ToString());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            strMsg = ex.Message;
            return null;
        }

        return result;
    }

    public static String GetMyHostName()
  {
        return Dns.GetHostName();
  }

  public static string GetMyIp()
    {
        return Hostname2Dotted(GetMyHostName());
    }

  public static String GetHostName(string address)
    {
        return Dns.GetHostEntry(address).HostName;
    }

  public static String Hostname2Dotted(String hostname)
  {
    int i;
    IPAddress[] IPs;
    String strIpDotted = "";

    try
    {
      IPs = Dns.GetHostAddresses(hostname);
      if (IPs.Length > 0)
      {
        for (i = 0; i < IPs.Length; i++)
        {
          if (IPs[i].AddressFamily == AddressFamily.InterNetwork)
          {
            strIpDotted = IPs[i].ToString();
            break;
          }
        }
      }
    }
    catch (Exception ex)
    {
      strMsg = ex.Message;
      return "";
    }

    return strIpDotted;
  }

    public static bool IsInDomain()
    {
        Win32.NetJoinStatus status = Win32.NetJoinStatus.NetSetupUnknownStatus;
        IntPtr pDomain = IntPtr.Zero;
        int result = Win32.NetGetJoinInformation(null, out pDomain, out status);
        if (pDomain != IntPtr.Zero)
        {
            Win32.NetApiBufferFree(pDomain);
        }
        if (result == Win32.ErrorSuccess)
        {
            return status == Win32.NetJoinStatus.NetSetupDomainName;
        }
        else
        {
            throw new Exception("Domain Info Get Failed", new Win32Exception());
        }
    }

    internal class Win32
    {
        public const int ErrorSuccess = 0;

        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int NetGetJoinInformation(string server, out IntPtr domain, out NetJoinStatus status);

        [DllImport("Netapi32.dll")]
        public static extern int NetApiBufferFree(IntPtr Buffer);

        public enum NetJoinStatus
        {
            NetSetupUnknownStatus = 0,
            NetSetupUnjoined,
            NetSetupWorkgroupName,
            NetSetupDomainName
        }
    }

    // Ritorna un Arraylist che rappresenta la lista delle macchine in rete
    public static ArrayList getNetworkComputers()
  {
    ArrayList NomiComputers; // Conterrà la lista dei nomi delle macchine da restituire al chiamante
    IntPtr pbuffHosts = IntPtr.Zero;     // Puntatore al buffer restituito dalla funzione. Contiene i nomi delle macchine in rete
    IntPtr pbuffHost = IntPtr.Zero;      // Cast del puntatore precedente, per raggiungere un singolo nome di macchina
    int entriesRead = 0;   // Quanti elementi letti (nomi di macchine)
    int totalEntries = 0;  // Quanti elementi restituiti (nomi di macchine)
    int resHandle = 0;     // riservato
    int sizeofINFO; // Dimensione in byte del buffer del singolo nome di macchina
    _SERVER_INFO_100 svrInfo; // Struttura nel buffer che contiene il singolo nome macchina
    int ret;

    sizeofINFO = Marshal.SizeOf(typeof(_SERVER_INFO_100));

    NomiComputers = new ArrayList();
    try
    {
      

      // Richiedo le macchine, di tipo SV_TYPE_WORKSTATION | SV_TYPE_SERVER (sono tutte?)
      ret = NetServerEnum(null, LEVEL_100, ref pbuffHosts, MAX_PREFERRED_LENGTH, out entriesRead, out totalEntries, SV_TYPE_WORKSTATION | SV_TYPE_SERVER, null, out resHandle);
      if (ret == 0) // Ok
      {
        // Si cicla per estrarre i singoli nomi di macchina
        for (int i = 0; i < totalEntries; i++)
        {
          pbuffHost = new IntPtr((int)pbuffHosts + (i * sizeofINFO));  // i-esimo blocco nel buffer
          svrInfo = (_SERVER_INFO_100)Marshal.PtrToStructure(pbuffHost, typeof(_SERVER_INFO_100));  // cast del buffer nella struttura
          NomiComputers.Add(svrInfo.sv100_name); // Ecco il nome della macchina
        }
        System.Windows.Forms.Application.DoEvents();
      }
    }
    catch (Exception ex)
    {
      strMsg = ex.Message;
      return null;
    }
    finally
    {
      // Comunque dealloco il buffer usato
      NetApiBufferFree(pbuffHosts);
    }

    // Ecco la lista delle macchine
    return NomiComputers;

  } //usa SMB

  public static ArrayList getDomainComputers(String strLDAPDomain)
  {
    ArrayList NomiComputers; // Conterrà la lista dei nomi delle macchine da restituire al chiamante
    DirectoryEntry ADentry; // Oggetto su cui effettuare una ricerca (il Dominio di AD)
    DirectorySearcher mySearcher; // Oggetto che effettuerà la ricerca (sul Dominio di AD)

    ADentry = new DirectoryEntry(strLDAPDomain); // handle al Dominio su cui effettuare la ricerca
    mySearcher = new DirectorySearcher(ADentry);
    mySearcher.Filter = ("(objectClass=computer)");  // Filtro che indica la classe di oggetti da cercare
    mySearcher.SizeLimit = 0;  // Leggere tutti gli oggetti (senza limiti)
    mySearcher.PageSize = 250; // Dimensione della pagina di memoria (non rilevante)
    mySearcher.PropertiesToLoad.Add("name");  // Prorietà da restituire della classe di oggetti ricercata

    NomiComputers = new ArrayList();
    try
    {
      System.Windows.Forms.Application.DoEvents();
      foreach (SearchResult resEnt in mySearcher.FindAll())
      {
        if (resEnt.Properties["name"].Count > 0)
        {
          string computerName = (string)resEnt.Properties["name"][0];
          NomiComputers.Add(computerName);
        }
      }
    }
    catch (Exception ex)
    {
      strMsg = ex.Message;
      return null;
    }
    finally
    {
    }
    return NomiComputers;

  }

  public static int GetMaskBroadcast(String _address, ref String _mask, ref String _broadcast)
  {
    String str = "";

    foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
    {
      if (netInterface.Supports(NetworkInterfaceComponent.IPv4))
      {
        foreach (var unicast in netInterface.GetIPProperties().UnicastAddresses)
        {
          int addressInt;
          int maskInt;
          int broadcastInt;

          if (unicast.Address.AddressFamily != AddressFamily.InterNetwork) { continue; }

          var address = unicast.Address;
          var mask = unicast.IPv4Mask;

          str = address.ToString();

          // Quello richiesto?
          if (str == _address)
          {
            if (address != null)
            {
              addressInt = BitConverter.ToInt32(address.GetAddressBytes(), 0);
              if (mask != null)
              {
                maskInt = BitConverter.ToInt32(mask.GetAddressBytes(), 0);
                broadcastInt = addressInt | ~maskInt;
                var broadcastAddress = new IPAddress(BitConverter.GetBytes(broadcastInt));
                Console.WriteLine(string.Format("{0,16} {1,16} {2,20}", address, mask, broadcastAddress));

                _mask = mask.ToString();
                _broadcast = broadcastAddress.ToString();

                return 0;
              }
            }
          }

        }
      }
    }
    strMsg = "Impossibile ricavare maschera e indirizzo broadcast";
    return 1;
  }

} // della classe

