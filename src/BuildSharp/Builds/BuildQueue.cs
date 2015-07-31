using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildSharp.Builds
{
    /// <summary>
    /// Represents a build scheduler which is responsible for keeping
    /// track of the pending and running builds.
    /// </summary>
    public class BuildQueue
    {
        /// <summary>
        /// Queue with builds.
        /// </summary>
        private readonly List<Build> builds;

        public BuildQueue()
        {
            builds = new List<Build>();
        }

        /// <summary>
        /// Event that is triggered when builds are added.
        /// </summary>
        public Action<Build> BuildAdded;

        /// <summary>
        /// Event that is triggered when builds are removed.
        /// </summary>
        public Action<Build> BuildRemoved;

        /// <summary>
        /// Adds a new build to the scheduler.
        /// </summary>
        public void AddBuild(Build build)
        {
            builds.Add(build);

            if (BuildAdded != null)
                BuildAdded(build);
        }

        /// <summary>
        /// Removes an existing build from the scheduler.
        /// </summary>
        public void RemoveBuild(Build build)
        {
            builds.Remove(build);

            if (BuildRemoved != null)
                BuildRemoved(build);
        }

        /// <summary>
        /// Gets the next queued build.
        /// </summary>
        /// <returns></returns>
        public Build GetNextBuild()
        {
            return builds.FirstOrDefault();
        }
    }
}
