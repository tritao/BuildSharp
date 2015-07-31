using System.Collections.Generic;

namespace BuildSharp.VersionControl
{
    /// <summary>
    /// Represents a version-control repository.
    /// </summary>
    public class Repository
    {
        public Repository(Project project)
        {
            Branches = new List<Branch>();
            Project = project;
        }

        /// <summary>
        /// URL to the repository.
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Min revision to build.
        /// </summary>
        public Commit MinRevision { get; set; }

        /// <summary>
        /// Max revision to build.
        /// </summary>
        public Commit MaxRevision { get; set; }

        /// <summary>
        /// List of branches in the repository.
        /// </summary>
        public List<Branch> Branches;

        /// <summary>
        /// Project associated with this repository.
        /// </summary>
        public Project Project;
    }
}
