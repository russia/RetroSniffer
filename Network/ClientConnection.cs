using SnifferRetro.Network;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RetroSniffer.Network
{
    public class ClientConnection
    {
        public Mitm MitmObj;

        private TcpClient _clientTcpClient;

        public ClientConnection(Mitm mitm, TcpClient tcpClient)
        {
            MitmObj = mitm;
            _clientTcpClient = tcpClient;
            var clientReceivingThread = new Thread(new ThreadStart(ClientReceive));
            clientReceivingThread.Start();
        }

        private void ClientReceive()
        {
            while ((_clientTcpClient != null && _clientTcpClient.Connected))
            {
                if (_clientTcpClient.Available > 0)
                {
                    byte[] array = new byte[_clientTcpClient.Available];
                    _clientTcpClient.GetStream().Read(array, 0, array.Count());
                    MitmObj.PacketHandler(array, this);
                }
                else
                    Thread.Sleep(10);
            }
        }

        public void SendPacketToClient(string msg)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(msg += "\0");
            _clientTcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }
    }
}