using BuildSharp.Builds;
using BuildSharp.Networking;

namespace BuildSharp.BuildServer
{
    /// <summary>
    /// Represents a remote build agent.
    /// </summary>
    public class RemoteBuildAgent
    {
        /// <summary>
        /// Network client representing remote build agent.
        /// </summary>
        public NetworkClient Client;

        /// <summary>
        /// Status of the remote build agent.
        /// </summary>
        public BuildAgentStatus Status;

        public RemoteBuildAgent(NetworkClient client)
        {
            Client = client;
            Client.MessageReceived += HandleBuildAgentMessage;
        }

        void HandleBuildAgentMessage(NetworkClient client,
            NetworkMessage message)
        {
            var data = message.Data;
            Log.Message("Message: {0}", data);
        }

        public void Update()
        {
            
        }

        public void SendBuild(Build build)
        {
            
        }
    }
}
