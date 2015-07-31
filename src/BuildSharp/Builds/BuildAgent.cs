using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using BuildSharp.Networking;
using BuildSharp.Steps;

namespace BuildSharp.Builds
{
    /// <summary>
    /// Represents the status of the build agent.
    /// </summary>
    public enum BuildAgentStatus
    {
        Idle,
        RunningBuild
    }

    /// <summary>
    /// Event data for build agent status changes.
    /// </summary>
    public class BuildAgentStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// New status of the build agent.
        /// </summary>
        public BuildAgentStatus Status;
    }

    /// <summary>
    /// Represents a remote build agent.
    /// </summary>
    public class BuildAgent
    {
        public BuildAgent()
        {
            PendingBuilds = new List<Build>();
        }

        /// <summary>
        /// Name of the build agent.
        /// </summary>
        public string Name;

        private BuildAgentStatus status;

        /// <summary>
        /// Status of the build agent.
        /// </summary>
        public BuildAgentStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                StatusChanged(this,
                    new BuildAgentStatusChangedEventArgs { Status = value });
            }
        }

        /// <summary>
        /// List of pending builds for this agent.
        /// </summary>
        public List<Build> PendingBuilds;

        /// <summary>
        /// Event triggered when the build agent status is changed.
        /// </summary>
        public EventHandler<BuildAgentStatusChangedEventArgs> StatusChanged
            = delegate { };

        /// <summary>
        /// Last connected time for the agent.
        /// </summary>
        public DateTime LastConnected;

        /// <summary>
        /// Updates a build agent.
        /// </summary>
        public void Update()
        {
            if (PendingBuilds.Count == 0)
                return;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var build = PendingBuilds.First();
            PendingBuilds.Remove(build);

            RunBuild(build);
            //try
            //{
            //    RunBuild(build);
            //}
            //catch (Exception ex)
            //{
            //    Log.Error("Build error: {0}", ex.ToString());
            //}

            stopwatch.Stop();
            Log.Message(string.Format(CultureInfo.InvariantCulture,
                "Executed build in {0:0.##}s", stopwatch.Elapsed.TotalSeconds));
        }

        private void RunBuild(Build build)
        {
            // Change the working directory to the project.
            var repoPath = Path.Combine(build.Configuration.Directory,
                build.Project.Name);

            if (!Directory.Exists(repoPath))
                Directory.CreateDirectory(repoPath);

            Directory.SetCurrentDirectory(repoPath);

            List<BuildStep> steps;
            if (MakeBuildSteps(build, out steps))
                return;

            build.Steps = steps;

            foreach (var step in steps)
            {
                step.Run();

                if (Log.Logger.Verbose)
                    Log.Message(step.Output.ToString());
            }

            var @out = build.Output;
        }

        private bool MakeBuildSteps(Build build, out List<BuildStep> steps)
        {
            steps = new List<BuildStep>
            {
                new GitBuildStep(this, build),
                new LuaBuildStep(this, build)
            };

            foreach (var step in build.Configuration.Steps)
            {
                try
                {
                    steps.Add(step);
                }
                catch
                {
                    Log.Error("Error running build step, aborting...");
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
