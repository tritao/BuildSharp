using System;
using System.Collections.Generic;

namespace BuildSharp.VersionControl
{
    /// <summary>
    /// Represents a branch in a version control system.
    /// </summary>
    public class Branch
    {
        public Branch()
        {
            Commits = new Dictionary<string, Commit>();
        }

        /// <summary>
        /// Name of the branch.
        /// </summary>
        public string Name;

        /// <summary>
        /// Maps commit references to the respective commits.
        /// </summary>
        public Dictionary<string, Commit> Commits;

        /// <summary>
        /// Repository that this branch belongs to.
        /// </summary>
        public WeakReference<Repository> Repository;

        /// <summary>
        /// Gets the given commit specified by the reference.
        /// </summary>
        public Commit GetCommitByReference(string reference)
        {
            Commit commit;
            Commits.TryGetValue(reference, out commit);

            return commit;
        }

        /// <summary>
        /// Adds the commit to this branch.
        /// </summary>
        public void AddCommit(Commit commit)
        {
            Commits[commit.Revision] = commit;
        }
    }
}
