using System.Net.Sockets;

namespace Networked_Drawing
{
    internal class ClientHandler
    {
        private TcpClient _client;
        
        public ClientHandler(TcpClient client) {
            _client = client;
            Console.WriteLine("Client connected");
        }

        public void Run()
        {
            while (true)
            {
                if (_client.Client.Poll(0, SelectMode.SelectRead) && _client.Client.Available == 0) break;
                
                Console.WriteLine("Looping");
            }

            Console.WriteLine("Client disconnected");
            _client.Close();
        }
    }
}
