using System;
using System.Threading;
using BuildSharp.Builds;

namespace BuildSharp.GitLab
{
    public class Runner
    {
        /// <summary>
        /// Build process
        /// </summary>
        private static Build build = null;

        /// <summary>
        /// Start the configured runner
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("* Gitlab CI Runner started");
            Console.WriteLine("* Waiting for builds");

            WaitForBuilds();
        }

        public static bool Register()
        {
            var response = GitLabAPI.RegisterRunner(SSHKey.GetPublicKey(),
                Config.RegisterToken);

            if (response == null)
                return false;

            Config.AuthToken = response;
            return true;
        }

        /// <summary>
        /// Build completed?
        /// </summary>
        public static bool Completed
        {
            get { return Running && build.Completed; }
        }

        /// <summary>
        /// Build running?
        /// </summary>
        public static bool Running
        {
            get { return build != null; }
        }

        /// <summary>
        /// Wait for an incoming build or update current Build
        /// </summary>
        private static void WaitForBuilds()
        {
            while (true)
            {
                if (Completed || Running)
                {
                    // Build is running or completed
                    // Update build
                    UpdateBuild();
                }
                else
                {
                    // Get new build
                    GetBuild();
                }

                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// Update the current running build progress
        /// </summary>
        private static void UpdateBuild()
        {
            if (build.Completed)
            {
                // Build finished
                if (PushBuild())
                {
                    //Console.WriteLine("[" + DateTime.Now.ToString() + "] Completed build " + build.buildInfo.id);
                    build = null;
                }
            }
            else
            {
                // Build is currently running
                PushBuild();
            }
        }

        /// <summary>
        /// PUSH Build Status to Gitlab CI
        /// </summary>
        /// <returns>true on success, false on fail</returns>
        private static bool PushBuild()
        {
            var buildInfo = (GitLabAPI.BuildInfo) build.Info;
            return GitLabAPI.PushBuild(buildInfo.id, build.State, build.Output.ToString());
        }

        /// <summary>
        /// Get a new build job
        /// </summary>
        private static void GetBuild()
        {
            var buildInfo = GitLabAPI.GetBuild();
            if (buildInfo != null)
            {
                var conf = new BuildConfiguration();

                // Create Build Job
                build = new Build() {Info = buildInfo};
                Console.WriteLine("[" + DateTime.Now + "] Build " + buildInfo.Value.id
                    + " started...");

                var thread = new Thread(build.Start);
                thread.Start();
            }
        }
    }
}
