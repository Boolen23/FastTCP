using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastTCP
{
    public class TCPConnector
    {
        public TCPConnector(ConnectionType type, int port)
        {
            Conntype = type;
            Port = port;
            iPAddress = IPAddress.Any;
        }
        public TCPConnector(ConnectionType type, int port, string iP)
        {
            Conntype = type;
            iPAddress = IPAddress.Parse(iP);
            Port = port;
        }
        public async void Initialize()
        {
            if (Conntype == ConnectionType.Server)
            {
                tcpListener = new TcpListener(iPAddress, Port);
                listenThread = new Thread(tcpListener.Start);
                listenThread.Start();
                Connect();
            }
            else
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(iPAddress, Port);
                PeerConnected?.Invoke(null, null);
            }
        }
        public async void Connect()
        {
            await Task.Delay(500);
            tcpClient = await tcpListener.AcceptTcpClientAsync();
            listenThread.Abort();
            PeerConnected?.Invoke(null, null);
        }
        private ConnectionType Conntype;
        private TcpListener tcpListener;
        private TcpClient tcpClient;
        private Thread listenThread;
        private IPAddress iPAddress;
        private int Port;
        private NetworkStream netStream;
        public event EventHandler PeerConnected;
        public event EventHandler PeerDisconnected;
        public string ClientIp => ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
        public bool IsConnected
        {
            get
            {
                if (tcpClient == null) return false;
                else return tcpClient.Connected;
            }
        }
        public void OpenStream() => netStream = tcpClient.GetStream();
        public async void SendDataAsync(byte[] data)
        {
            await Task.Run(() =>
            {
                try
                {
                    OpenStream();
                    netStream.Write(data, 0, data.Length);
                }
                catch(Exception ex)
                {
                    PeerDisconnected?.Invoke(ex.ToString(), EventArgs.Empty);
                }
            });
        }
        public async void SendDataAsync(byte[] data, byte[] EndBytes)
        {
            await Task.Run(() =>
            {
                try
                {
                    OpenStream();
                    netStream.Write(data, 0, data.Length);
                    netStream.Write(EndBytes, 0, EndBytes.Length);
                    netStream.Flush();
                }
                catch (Exception ex)
                {
                    PeerDisconnected?.Invoke(ex.ToString(), EventArgs.Empty);
                }
            });
        }

    }
}
