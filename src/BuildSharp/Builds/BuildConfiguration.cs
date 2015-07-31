using System.Collections.Generic;

namespace BuildSharp.Builds
{
    /// <summary>
    /// Represents a build configuration.
    /// </summary>
    public class BuildConfiguration
    {
        /// <summary>
        /// Keeps the enabled status of the build configuration.
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// List of build steps part of the build configuration.
        /// </summary>
        public IReadOnlyList<BuildStep> Steps;

        /// <summary>
        /// Directory where to build.
        /// </summary>
        public string Directory { get; set; }

        public BuildConfiguration()
        {
            Steps = new List<BuildStep>();
            Enabled = true;
        }

        /// <summary>
        /// Adds a build step to the build configuration.
        /// </summary>
        public void AddStep(BuildStep step)
        {
            var list = Steps as List<BuildStep>;
            list.Add(step);
        }

        /// <summary>
        /// Removes a build step from the build configuration.
        /// </summary>
        public void RemoveStep(BuildStep step)
        {
            var list = Steps as List<BuildStep>;
            list.Remove(step);
        }
    }
}
