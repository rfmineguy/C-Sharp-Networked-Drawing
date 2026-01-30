using System.Net.Sockets;
using Common;
using MessagePack;

namespace Networked_Drawing
{
    internal class ClientHandler
    {
        private TcpClient _client;
        private ServerState _serverState;
        private NetworkStream _stream;
        private MessagePackStreamReader _reader;
        private CancellationTokenSource _cancelTokenSource;
        public Guid _uuid;
        
        public ClientHandler(TcpClient client, ServerState serverState) {
            _client = client;
            _serverState = serverState;
            _stream = _client.GetStream();
            _reader = new MessagePackStreamReader(_stream);
            _cancelTokenSource = new CancellationTokenSource();
            _uuid = Guid.NewGuid();
        }

        public void SendMessage(Message message)
        {
            Envelope env = new Envelope(_uuid, message);
            MessagePack.MessagePackSerializer.Serialize<Envelope>(this._stream, env);
            this._stream.Flush();
        }

        public async Task Run()
        {
            Console.WriteLine("Client listener started...");
            SendMessage(new SetGuid(_uuid));
            Console.WriteLine($"Sending guid: {_uuid}");
            bool running = true;
            while (running)
            {
                if (_reader == null) continue;
                if (_cancelTokenSource.IsCancellationRequested) break;

                var readResult = await _reader.ReadAsync(_cancelTokenSource.Token);
                if (readResult == null) break;

                // read messages
                Envelope e = MessagePackSerializer.Deserialize<Envelope>(readResult.Value);
                Message m = e.message;
                Console.WriteLine($"Received: {e}");
            }

            // _serverState.Broadcast(this, new UserConnection(UserConnection.ConnectionType.DISCONNECT));
            Console.WriteLine($"Client listener ended... {this._uuid}");
            _client.Close();
            _serverState.clients.Remove(this);
        }
    }
}
