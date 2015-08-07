using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BuildSharp.Networking
{
   /// <summary>
   /// Delegate representing messages received.
   /// </summary>
    public delegate void MessageReceivedDelegate(NetworkClient client,
        NetworkMessage message);

    /// <summary>
    /// Represents a client in a network.
    /// </summary>
    public class NetworkClient
    {
        private readonly TcpClient _socket;
        private readonly NetworkStream _networkStream;
        private readonly int _id;

        public bool IsActive { get; set; }
        public int Id { get { return _id; } }
        public TcpClient Socket { get { return _socket; } }

        public event MessageReceivedDelegate MessageReceived;
        public event NetworkClientDisconnectedDelegate ClientDisconnected;

        public NetworkClient(TcpClient socket, int id = -1)
        {
            _socket = socket;
            _id = id;

            IsActive = true;
            _networkStream = _socket.GetStream();
        }

        private void Disconnect()
        {
            IsActive = false;
            if (ClientDisconnected != null)
                ClientDisconnected(this);
        }

        public async Task<NetworkMessage> GetMessage()
        {
            try
            {
                if (!_networkStream.CanRead)
                {
                    Disconnect();
                    return null;
                }

                var message = new NetworkMessage();

                using (var reader = new BinaryReader(_networkStream,
                    Encoding.Default, leaveOpen: true))
                {
                    var kind = (NetworkMessageKind) reader.ReadInt32();

                    // If content is null, that means the connection has been
                    // gracefully disconnected.
                    if (kind == NetworkMessageKind.Disconnect)
                    {
                        Disconnect();
                        return null;
                    }

                    var size = reader.ReadInt32();
                    var buffer = new byte[size];

                    await _networkStream.ReadAsync(buffer, 0, size);
                    message.Data = Encoding.UTF8.GetString(buffer);
                }

                if (MessageReceived != null)
                    MessageReceived(this, message);

                return message;
            }

            // If the TCP connection is ungracefully disconnected, it will
            // throw an exception.
            catch (IOException)
            {
                Disconnect();
                return null;
            }
        }

        public async void SendMessage(NetworkMessageKind kind,
            string data)
        {
            if (!IsActive)
                return;

            try
            {
                // Don't use a using statement as we do not want the stream closed
                // after the write is completed.
                using (var writer = new BinaryWriter(_networkStream,
                    Encoding.Default, leaveOpen: true))
                {
                    writer.Write((int)kind);

                    var buffer = Encoding.UTF8.GetBytes(data);
                    writer.Write(buffer.Length);
                    writer.Write(buffer);
                    writer.Flush();
                }
            }
            catch (IOException)
            {
                // Socket closed.
                Disconnect();
            }
        }
    }
}
