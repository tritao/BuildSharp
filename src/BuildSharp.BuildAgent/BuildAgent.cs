using System;
using System.Net.Sockets;
using BuildSharp.Networking;

namespace BuildSharp.Agent
{
    /// <summary>
    /// Represents a build agent.
    /// </summary>
    public class BuildAgent
    {
        public BuildAgent(Options options)
        {
            this.options = options;
        }

        private readonly Options options;
        private NetworkClient networkClient;

        public void RunAgent()
        {
            var socket = ConnectToServer();
            if (socket == null)
            {
                Log.Error("Could not establish a connection to the server, aborting...");
                return;
            }

            networkClient = new NetworkClient(socket);
            //networkClient.MessageReceived += AgentOnMessageReceived;

            var agentInfo = new BuildAgentInfoMessage
            {
                MachineName = Environment.MachineName
            };

            networkClient.Send(NetworkMessageKind.BuildAgentInfo, agentInfo);

            while (networkClient.IsActive)
            {
                var task = networkClient.GetMessage();
                task.Wait();

                var message = task.Result;
                OnServerMessage(networkClient, message);
            }
        }

        private void OnServerMessage(NetworkClient client, NetworkMessage message)
        {
            Log.Message("Received: {0}", message);

            switch (message.Kind)
            {
                case NetworkMessageKind.Disconnect:
                    break;
                case NetworkMessageKind.ProtocolVersion:
                    break;
                case NetworkMessageKind.BuildAgentInfo:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private TcpClient ConnectToServer()
        {
            const int numTries = 10;
            for (var i = 0; i < numTries; i++)
            {
                Log.Message("Trying to connect to server... ");

                TcpClient client;
                try
                {
                    client = new TcpClient();
                    client.Connect(options.ServerAddress, options.ServerPort);
                }
                catch (SocketException ex)
                {
                    Log.Message("Connection failed.");
                    continue;
                }

                Log.Message("Connected.");
                return client;
            }

            return null;
        }

        static void Main(string[] args)
        {
            var options = new Options
            {
                ServerAddress = string.Empty,
                ServerPort = 0
            };

            var agent = new BuildAgent(options);
            agent.RunAgent();
        }
    }
}
