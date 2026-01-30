using Common;
using Networked_Drawing;
using System.Net;
using System.Net.Sockets;

class ServerMain
{
    static void Main(string[] args)
    {
        IPEndPoint ip = new IPEndPoint(IPAddress.Loopback, 33);
        TcpListener listener = new TcpListener(ip);
        ServerState serverState = new ServerState();

        try
        {
            listener.Start();
        } catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        Console.WriteLine("Started listening {0}", listener.LocalEndpoint);
        Console.WriteLine("Can you tell me anything??");
        while (true)
        {
            try
            {
                ClientHandler clientHandler = new ClientHandler(listener.AcceptTcpClient(), serverState);
                serverState.clients.Add(clientHandler);
                Task.Run(async () =>
                {
                    await clientHandler.Run();
                });

                // serverState.Broadcast(clientHandler, new UserConnection(clientHandler._uuid, UserConnection.ConnectionType.CONNECT));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}