using fastJSON;

namespace BuildSharp.Networking
{
    public enum NetworkMessageKind
    {
        Disconnect,
        ProtocolVersion,
        BuildAgentInfo,
    }

    public class NetworkMessage
    {
        public NetworkMessageKind Kind;
        public string Data;
    }

    public static class NetworkMessageExtensions
    {
        public static void Send<T>(this NetworkClient client,
            NetworkMessageKind kind, T @object)
        {
            var data = JSON.ToJSON(@object);
            client.SendMessage(kind, data);
        }
    }

    public struct ProtocolVersionMessage
    {
        public int Version;
    }

    public struct BuildAgentInfoMessage
    {
        public string MachineName { get; set; }
    }
}
