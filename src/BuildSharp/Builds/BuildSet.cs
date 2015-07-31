using System.Collections.Generic;
using BuildSharp.VersionControl;

namespace BuildSharp.Builds
{
    /// <summary>
    /// A build set represents a set of builds spawned by the same commit.
    /// </summary>
    public sealed class BuildSet
    {
        public BuildSet()
        {
            Builds = new List<Build>();
        }

        /// <summary>
        /// Commit that spawned this change.
        /// </summary>
        public Commit Commit;

        /// <summary>
        /// List of builds in the set.
        /// </summary>
        public List<Build> Builds;
    }
}
