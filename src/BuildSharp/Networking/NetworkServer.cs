using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BuildSharp.Networking
{
    /// <summary>
    /// Delegate representing client connections.
    /// </summary>
    public delegate void NetworkClientConnectedDelegate(NetworkClient client);

    /// <summary>
    /// Delegate representing client disconnections.
    /// </summary>
    public delegate void NetworkClientDisconnectedDelegate(NetworkClient client);

    /// <summary>
    /// Represents a server in a network.
    /// </summary>
    public class NetworkServer
    {
        private readonly TcpListener _listener;
        private readonly List<NetworkClient> _clients;
        private readonly List<KeyValuePair<Task, NetworkClient>> _clientReceiveInputTasks;
        private Task _clientListenTask;

        public bool IsRunning { get; private set; }

        public Exception ClientListenTaskException
        {
            get { return _clientListenTask.Exception; }
        }

        public event NetworkClientConnectedDelegate ClientConnected = delegate { };
        public event NetworkClientDisconnectedDelegate ClientDisconnected = delegate { };

        public NetworkServer(IPAddress ip, int port)
        {
            _listener = new TcpListener(ip, port);
            _clients = new List<NetworkClient>();
            _clientReceiveInputTasks = new List<KeyValuePair<Task, NetworkClient>>();
        }

        private void OnClientConnected(TcpClient client, int clientNumber)
        {
            var netClient = new NetworkClient(client, clientNumber);
            netClient.ClientDisconnected += OnClientDisconnected;

            ClientConnected(netClient);

            _clients.Add(netClient);
            _clientReceiveInputTasks.Add(
                new KeyValuePair<Task, NetworkClient>(netClient.GetMessage(), netClient));
        }

        private void OnClientDisconnected(NetworkClient client)
        {
            client.IsActive = false;
            client.Socket.Close();

            if (_clients.Contains(client))
                _clients.Remove(client);

            ClientDisconnected(client);
        }

        private async Task ListenForClients()
        {
            var numClients = 0;
            while (IsRunning)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();
                OnClientConnected(tcpClient, numClients);
                numClients++;
            }

            _listener.Stop();
        }

        public void Run()
        {
            _listener.Start();
            IsRunning = true;

            _clientListenTask = ListenForClients();
        }
    }
}
