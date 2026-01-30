using Common;
using System.Net.Sockets;

namespace Client
{
    internal class ClientState
    {
        public Guid? guid;
        public TcpClient? client;
        public ClientData data;
        public Dictionary<Guid, ClientData> clients;

        public ClientState(ref TcpClient client) {
            guid = null;
            this.client = client;
            this.data = new ClientData();
            this.clients = new Dictionary<Guid, ClientData>(10);
        }

        internal void Disconnect()
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(guid);
            this.SendMessage(new UserConnection(UserConnection.ConnectionType.DISCONNECT));
            client.Close();
            client = null;
            guid = null;
        }

        internal void SendMessage(Message m)
        {
            if (client == null) return;
            if (guid == null) return;
            Envelope env = new Envelope(guid.Value, m);
            MessagePack.MessagePackSerializer.Serialize<Envelope>(client.GetStream(), env);
            client.GetStream().Flush();
        }
    }
}
