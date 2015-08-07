using System;
using BuildSharp.GitLab;

namespace BuildSharp.Agent
{
    /// <summary>
    /// Represents a GitLab build agent.
    /// </summary>
    public class GitLabAgent
    {
        static void Main(string[] args)
        {
            Setup();

            ListRunners();

            Console.WriteLine("Registering runner in CI...");
            if (!Runner.Register())
            {
                Console.WriteLine("Could not register as an agent, aborting...");
                return;
            }

            Console.WriteLine("Auth token: " + Config.AuthToken);
            Runner.Run();
        }

        private static void Setup()
        {
            Config.GitLabEndpoint = "http://192.168.1.112/";
            Config.GitLabPrivateToken = "r8pHJ3khRNsNY5yCZ9zK";

            Config.GitLabCIEndpoint = "http://192.168.1.112/gitlabci";
            Config.RegisterToken = "4d2b9e8f8119d52e3e33";

            SSHKey.GenerateSSHKeyPair();
        }

        private static void ListRunners()
        {
            var runners = GitLabAPI.GetRunners();
            if (runners == null)
            {
                Console.WriteLine("Could not get list of build runners");
                return;
            }

            Console.WriteLine("Registered runners:");
            foreach (var runner in runners)
                Console.WriteLine("  " + runner);
        }
    }
}
