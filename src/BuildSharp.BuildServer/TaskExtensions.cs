using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildSharp.BuildServer
{
    public static class TaskExtensions
    {
        public static async Task PeriodicRun(TimeSpan interval,
                                             CancellationToken token,
                                             Action action)
        {
            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {
                action();

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }
    }
}
