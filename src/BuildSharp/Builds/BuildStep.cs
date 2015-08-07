using System;

namespace BuildSharp.Builds
{
    /// <summary>
    /// Represents a step in a build configuration.
    /// </summary>
    public abstract class BuildStep
    {
        /// <summary>
        /// Build executor associated with the build step.
        /// </summary>
        public BuildExecutor BuildExecutor { get; private set; }

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

        protected BuildStep(BuildExecutor executor, Build build)
        {
            if (executor == null)
                throw new ArgumentNullException("executor");

            if (build == null)
                throw new ArgumentNullException("build");

            BuildExecutor = executor;
            Build = build;
        }

        public abstract void Run();
    }
}
