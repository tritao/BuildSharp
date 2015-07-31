using System;
using System.Collections.Generic;
using BuildSharp.Builds;

namespace BuildSharp.VersionControl
{
    /// <summary>
    /// Represents a project in the build system.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// The id of the build.
        /// </summary>
        public int Id;

        /// <summary>
        /// Name of the project.
        /// </summary>
        public string Name;

        /// <summary>
        /// Repositories associated to the project.
        /// </summary>
        public List<Repository> Repositories;

        /// <summary>
        /// Build configurations for the project.
        /// </summary>
        public List<BuildConfiguration> Configurations;

        /// <summary>
        /// Default build configuration for this project.
        /// </summary>
        public BuildConfiguration DefaultBuildConfiguration;

        /// <summary>
        /// Last time that we checked repository for changes.
        /// </summary>
        public TimeSpan LastCheckedForChanges;

        public Project()
        {
            Repositories = new List<Repository>();
            Configurations = new List<BuildConfiguration>();
            DefaultBuildConfiguration = new BuildConfiguration();
        }
    }
}