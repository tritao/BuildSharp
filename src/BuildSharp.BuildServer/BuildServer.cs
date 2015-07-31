using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using BuildSharp.Builds;
using BuildSharp.BuildServer;
using BuildSharp.Networking;
using BuildSharp.VersionControl;
using Database = BuildSharp.BuildServer.Database.Database;

namespace BuildSharp.Server
{
    /// <summary>
    /// Represents a master build server.
    /// </summary>
    public class BuildServer : IDisposable
    {
        public Options Options;

        /// <summary>
        /// Database for persisting build data.
        /// </summary>
        public Database Database;

        /// <summary>
        /// List of projects.
        /// </summary>
        public List<Project> Projects;

        /// <summary>
        /// List of connected build agents.
        /// </summary>
        public List<BuildAgent> BuildAgents;

        /// <summary>
        /// Build queue with pending builds.
        /// </summary>
        public BuildQueue BuildQueue;

        /// <summary>
        /// Github polling service.
        /// </summary>
        public GithubPoller GithubPoller;

        /// <summary>
        /// HTTP server listening for webhooks.
        /// </summary>
        public GithubWebhookListener HttpServer;

        /// <summary>
        /// TCP server for connection to build agents.
        /// </summary>
        public NetworkServer NetworkServer;

        /// <summary>
        /// This is set to false in case server is shutting down.
        /// </summary>
        public bool IsExiting;

        public BuildServer(Options options)
        {
            Options = options;
            Projects = new List<Project>();
            BuildAgents = new List<BuildAgent>();
            BuildQueue = new BuildQueue();
            GithubPoller = new GithubPoller(Projects, Options.Username,
                Options.Token);
            GithubPoller.RepositoryChanged += OnRepositoryChange;

            Database = new Database(options.Database);
            Database.LoadProjects(Projects);
            Database.LoadBuilds();
        }

        public void Dispose()
        {
            Database.Dispose();
        }

        public void RunServer()
        {
            StartHttpServer();
            StartTcpServer();

            TaskExtensions.PeriodicRun(TimeSpan.FromSeconds(30),
                CancellationToken.None, () => GithubPoller.Poll());

            while (!IsExiting)
            {
                CheckAgents();
                CheckBuilds();
            }
        }

        void StartHttpServer()
        {
            Log.Message("Listening for HTTP requests...");

            HttpServer = new GithubWebhookListener();
            HttpServer.Start();
            HttpServer.ProcessAsync();
        }

        void StartTcpServer()
        {
            Log.Message("Listening for agents...");

            NetworkServer = new NetworkServer(IPAddress.Any, Options.ServerPort);
            NetworkServer.ClientConnected += HandleBuildAgentConnection;
            NetworkServer.ClientDisconnected += HandleBuildAgentDisconnect;
            NetworkServer.Run();
        }

        #region Build agent management

        private void HandleBuildAgentConnection(NetworkClient client)
        {
            var agent = new BuildAgent();
            lock (BuildAgents)
            {
                BuildAgents.Add(agent);
            }

            //agent.Client.MessageReceived += HandleBuildAgentMessage;
            Log.Message("Build agent connected: {0}", agent);
        }

        private void HandleBuildAgentDisconnect(NetworkClient client)
        {
            lock (BuildAgents)
            {
                //foreach (var agent in BuildAgents.Where(
                //    agent => agent.Client == client) .ToArray())
                //{
                //    BuildAgents.Remove(agent);
                //    Log.Message("Build agent disconnected: {0}", agent);
                //}
            }
        }

        private void HandleBuildAgentMessage(NetworkClient client,
            NetworkMessage message)
        {
            var data = message.Data;
            Log.Message("Message: {0}", data);
        }

        public void CheckAgents()
        {
            lock (BuildAgents)
            {
                foreach (var agent in BuildAgents)
                    agent.Update();
            }
        }

        #endregion

        #region Builds management

        public void CheckBuilds()
        {
            // Retrieve the next queued build from the scheduler.
            var nextBuild = BuildQueue.GetNextBuild();
            if (nextBuild == null)
                return;

            // Check for build agent compatible with the requirements.
            var agent = GetCompatibleBuildAgent(nextBuild);
            if (agent == null)
                return;

            // Remove the build from the queue.
            BuildQueue.RemoveBuild(nextBuild);

            // Send the build to the agent.
            agent.PendingBuilds.Add(nextBuild);
        }

        BuildAgent GetCompatibleBuildAgent(Build build)
        {
            foreach (var agent in BuildAgents)
            {
                if (agent.Status != BuildAgentStatus.Idle)
                    continue;

                Log.Message("Assigned build {0} to agent {1}.", build, agent);
                return agent;
            }

            return null;
        }

        void OnRepositoryChange(GithubRepository repository, List<Commit> commits)
        {
            foreach (var commit in commits)
            {
                if (commit.BuildSet != null)
                    continue;

                // Spawn a set of builds for this commit.
                var buildSet = new BuildSet { Commit = commit };

                commit.BuildSet = buildSet;

                Branch branch;
                commit.Branch.TryGetTarget(out branch);

                Repository repo;
                branch.Repository.TryGetTarget(out repo);

                var buildConfiguration = repo.Project.DefaultBuildConfiguration;
                buildConfiguration.Directory = Options.OutputDir;

                Log.Message("Spawning new build for {0}/{1}", repo.Project.Name,
                    commit.ToString());

                var build = new Build(repo.Project, commit, buildConfiguration);
                buildSet.Builds.Add(build);

                Database.AddBuild(build);

                BuildQueue.AddBuild(build);
            }
        }

        #endregion

        static void Main(string[] args)
        {
            var project = new Project { Name = "CppSharp" };
            var repo = new GithubRepository(project)
            {
                Owner = "mono",
                Name = "CppSharp",
                URL = @"https://github.com/mono/CppSharp.git",
                MinRevision = new Commit("cd3e729d3873a845eacee4260480e4c3dfe14579")
            };
            project.Repositories.Add(repo);

            var config = new BuildConfiguration();
            project.Configurations.Add(config);

            var options = new Options
            {
                OutputDir = @"C:\builds\",
                Username = "",
                Token = ""
            };

            var agent = new BuildAgent();

            using (var server = new BuildServer(options))
            {
                ConsoleUtils.SetupExitHandler(sig =>
                {
                    server.IsExiting = true;
                    return true;
                });

                server.Projects.Add(project);
                server.BuildAgents.Add(agent);
                server.RunServer();
            }
        }
    }
}
