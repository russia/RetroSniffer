using SnifferRetro.Network;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RetroSniffer.Network
{
    public class ServerConnection
    {
        public Mitm MitmObj;

        private TcpClient _serverTcpClient;

        public ServerConnection(Mitm mitm, string address, int port)
        {
            MitmObj = mitm;
            _serverTcpClient = new TcpClient();
            _serverTcpClient.Connect(address, port);
            var serverReceivingThread = new Thread(new ThreadStart(ServerReceive));
            serverReceivingThread.Start();
        }

        private void ServerReceive()
        {
            while ((_serverTcpClient != null && _serverTcpClient.Connected))
            {
                if (_serverTcpClient.Available > 0)
                {
                    byte[] array = new byte[_serverTcpClient.Available];
                    _serverTcpClient.GetStream().Read(array, 0, array.Count());
                    MitmObj.PacketHandler(array, this);
                }
                else
                    Thread.Sleep(10);
            }
        }

        public void SendPacketToServer(string msg)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(msg);
            _serverTcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }
    }
}