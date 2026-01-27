using Networked_Drawing;
using System.Net;
using System.Net.Sockets;

class ServerMain
{
    static void Main(string[] args)
    {
        IPEndPoint ip = new IPEndPoint(IPAddress.Any, 33);
        TcpListener listener = new TcpListener(ip);
        List<ClientHandler> clients = new List<ClientHandler>();

        listener.Start();
        Console.WriteLine("Started listening {0}", listener.LocalEndpoint);
        while (true)
        {
            try
            {
                ClientHandler clientHandler = new ClientHandler(listener.AcceptTcpClient());
                clients.Add(clientHandler);
                new Thread(clientHandler.Run).Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}