using MessagePack;
using System.Runtime.CompilerServices;

namespace Common
{
    // MessagePack Classes
    [MessagePackObject]
    public class Envelope
    {
        [Key(0)]
        public Guid guid { get; }
        [Key(1)]
        public Message message { get; }

        public Envelope(Guid g, Message m)
        {
            this.guid = g;
            this.message = m;
        }

        public override string ToString() {
            return $"Env {{guid: {this.guid}, message: {this.message}}}";
        }
    }

    [MessagePackObject]
    [Union(0, typeof(MouseMove))]
    [Union(1, typeof(Test))]
    [Union(2, typeof(SetGuid))]
    [Union(3, typeof(UserConnection))]
    public abstract class Message {}

    [MessagePackObject]
    public class SetGuid : Message
    {
        [Key(0)]
        public Guid guid { get; }

        [SerializationConstructor]
        public SetGuid(Guid guid)
        {
            this.guid = guid;
        }

        public override string ToString()
        {
            return $"Guid {{guid: {this.guid.ToString()}}}";
        }
    }

    [MessagePackObject]
    public class MouseMove : Message {
        [Key(0)]
        public int x { get; set; }
        [Key(1)]
        public int y { get; set; }

        [SerializationConstructor]
        public MouseMove(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"MouseMove {{x: {this.x}, y: {this.y}}}";
        }
    }

    [MessagePackObject]
    public class Test : Message
    {
        [Key(0)]
        public string x { get; set; }

        [SerializationConstructor]
        public Test(string x)
        {
            this.x = x;
        }

        public override string ToString()
        {
            return $"Test {{x: {this.x}}}";
        }
    }

    [MessagePackObject]
    public class UserConnection : Message
    {
        public enum ConnectionType {
            CONNECT, DISCONNECT
        }
        [Key(0)]
        public ConnectionType type { get; set; }
        
        [SerializationConstructor]
        public UserConnection(ConnectionType type)
        {
            this.type = type;
        }

        public override string ToString() {
            return $"UserConnection {{type: {this.type.ToString()}}}";
        }
    }
}
