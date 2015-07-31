using fastJSON;

namespace BuildSharp.Networking
{
    public enum NetworkMessageKind
    {
        Disconnect,
        JSON,
    }

    public class NetworkMessage
    {
        public NetworkMessageKind Kind;
        public string Data;
    }

    public static class NetworkMessageExtensions
    {
        public static void Send<T>(this NetworkClient client, T @object)
        {
            var data = JSON.ToJSON(@object);
            client.SendMessage(NetworkMessageKind.JSON, data);
        }
    }

    public struct AgentInfoMessage
    {
        public string MachineName { get; set; }
    }
}
