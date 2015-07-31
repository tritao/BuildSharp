using System.IO;
using BuildSharp.Builds;

namespace BuildSharp
{
    public class Driver
    {
        public Options Options { get; private set; }
        public ILog Log { get; private set; }

        /// <summary>
        /// Queue with pending builds.
        /// </summary>
        public BuildQueue Queue { get; private set; }

        public Driver()
        {
            Options = new Options();
            Log = new ConsoleLog();
            Setup();
        }

        public Driver(Options options, ILog log = null)
        {
            Options = options;
            Log = log;
            Setup();
        }

        public void Setup()
        {
            if (Log == null)
                Log = new ConsoleLog();

            if (Options.OutputDir == null)
                Options.OutputDir = Directory.GetCurrentDirectory();

            Queue = new BuildQueue();
            Queue.BuildAdded += BuildAdded;
            Queue.BuildRemoved += BuildRemoved;
        }

        private void BuildAdded(Build build)
        {
            Log.Message("Build added to queue: {0}", build);
        }

        private void BuildRemoved(Build build)
        {
            Log.Message("Build removed from queue: {0}", build);
        }
    }
}