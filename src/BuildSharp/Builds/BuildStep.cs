using System;

namespace BuildSharp.Builds
{
    /// <summary>
    /// Represents a step in a build configuration.
    /// </summary>
    public abstract class BuildStep
    {
        /// <summary>
        /// Reference to the driver.
        /// </summary>
        public Driver Driver;

        /// <summary>
        /// Build agent associated with the build step.
        /// </summary>
        public BuildAgent BuildAgent { get; private set; }

        /// <summary>
        /// Build associated with the build step.
        /// </summary>
        public Build Build { get; private set; }

        /// <summary>
        /// Output associated with the build.
        /// </summary>
        public BuildOutput Output { get { return Build.Output; } }

        /// <summary>
        /// Configuration associated with the build step.
        /// </summary>
        public BuildConfiguration Configuration
        {
            get { return Build.Configuration; }
        }

        public Options Options;

        protected BuildStep(BuildAgent agent, Build build)
        {
            if (agent == null)
                throw new ArgumentNullException("agent");

            if (build == null)
                throw new ArgumentNullException("build");

            BuildAgent = agent;
            Build = build;
        }

        public abstract void Run();
    }
}
