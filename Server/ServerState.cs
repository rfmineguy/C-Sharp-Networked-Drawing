using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Networked_Drawing
{
    internal class ServerState
    {
        public List<ClientHandler> clients;

        public ServerState() { 
            clients = new List<ClientHandler>();
        }

        public void Broadcast(ClientHandler from, Message message)
        {
            foreach (ClientHandler clientHandler in clients)
            {
                if (clientHandler == from) continue;
                clientHandler.SendMessage(message);
            }

        }
    }
}
