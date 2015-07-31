using System;
using System.Threading.Tasks;
using BuildSharp.Networking;

namespace BuildSharp.Agent
{
    /// <summary>
    /// Represents a build agent.
    /// </summary>
    public class BuildAgent : Driver
    {
        private NetworkClient networkClient;
        private Task networkClientTask;

        public void RunAgent()
        {
            var socket = ConnectToServer();
            if (socket == null)
            {
                Log.Error("Could not establish a connection to the server, aborting...");
                return;
            }

            networkClient = new NetworkClient(socket);
            networkClient.MessageReceived += AgentOnMessageReceived;

            var agentInfo = new AgentInfoMessage
            {
                MachineName = Environment.MachineName
            };
            networkClient.Send(agentInfo);

            networkClientTask = networkClient.Process();
        }

        private void AgentOnMessageReceived(NetworkClient client, NetworkMessage message)
        {
            Log.Message("Received: {0}", message);
        }

        private TcpClient ConnectToServer()
        {
            const int numTries = 10;
            for (var i = 0; i < numTries; i++)
            {
                Console.Write("Trying to connect to server... ");

                TcpClient client;
                try
                {
                    client = new TcpClient();
                    client.Connect(Options.ServerAddress, Options.ServerPort);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("failed.");
                    continue;
                }

                Console.WriteLine("OK.");
                return client;
            }

            return null;
        }
    }
}
