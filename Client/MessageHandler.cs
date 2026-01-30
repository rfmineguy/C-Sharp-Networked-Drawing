using MessagePack;
using System.Net.Sockets;
using Common;

namespace Client
{
    internal class MessageHandler
    {
        private ClientState _clientState;
        private MessagePackStreamReader _reader;
        private NetworkStream _stream;

        public MessageHandler(ref ClientState clientState)
        {
            ArgumentNullException.ThrowIfNull(clientState);
            ArgumentNullException.ThrowIfNull(clientState.client);
            _clientState = clientState;
            _stream = clientState.client.GetStream();
            _reader = new MessagePackStreamReader(_stream);
        }

        public async Task Run (CancellationToken token)
        {
            Console.WriteLine("Client message handler started...");
            bool running = true;
            try
            {
                while (running)
                {
                    // if (token.IsCancellationRequested) break;
                    Console.WriteLine("Message Handler running..");

                    var readResult = await _reader.ReadAsync(token);
                    if (readResult == null) break;

                    Envelope e = MessagePackSerializer.Deserialize<Envelope>(readResult.Value);
                    Message m = e.message;
                    switch (m)
                    {
                        case SetGuid sg:
                            if (_clientState.guid != null)
                            {
                                Console.WriteLine("Failed to set guid, already set");
                                break;
                            }
                            _clientState.guid = sg.guid;
                            Console.WriteLine($"Set client guid {_clientState.guid}");
                            break;
                        case Test t:
                            break;
                        case MouseMove mouse:
                            Console.WriteLine($"{e.guid} moved to {mouse.x}, {mouse.y}");
                            break;
                        case UserConnection userConnection:
                            switch (userConnection.type)
                            {
                                case UserConnection.ConnectionType.CONNECT:
                                    _clientState.clients.Add(e.guid, new ClientData());
                                    break;
                                case UserConnection.ConnectionType.DISCONNECT:
                                    Console.WriteLine("Disconnect response");
                                    _clientState.clients.Remove(e.guid);
                                    running = false;
                                    break;
                            }
                            break;
                        default:
                            Console.WriteLine($"Not a derived message class: \"{m.ToString()}\"");
                            Console.WriteLine(e);
                            break;
                    }
                }
            } catch (IOException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            _reader.Dispose();
            _stream.Dispose();
            _clientState.client.Close();

            Console.WriteLine("Client message handler ended...");
        }
    }
}
