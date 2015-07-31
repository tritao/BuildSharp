using System;
using BuildSharp.Builds;

namespace BuildSharp.VersionControl
{
    /// <summary>
    /// Represents a commit from a version control system.
    /// </summary>
    public sealed class Commit
    {
        public Commit()
        {
        }

        public Commit(string revision)
        {
            Revision = revision;
        }

        /// <summary>
        /// Commit author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Commit message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Commit revision or hash.
        /// </summary>
        public string Revision { get; set; }

        /// <summary>
        /// Branch that this commit belongs to.
        /// </summary>
        public WeakReference<Branch> Branch;

        /// <summary>
        /// Build set associated to this commit.
        /// </summary>
        public BuildSet BuildSet;

        public override string ToString()
        {
            return Revision;
        }
    }
}
