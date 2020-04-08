using System;
using System.Diagnostics;

namespace RaspiFanController.Logic
{
    public class WrappedStopwatch : IWrappedStopwatch
    {
        public WrappedStopwatch()
        {
            Stopwatch = new Stopwatch();
        }

        /// <inheritdoc />
        public bool IsRunning => Stopwatch.IsRunning;

        /// <inheritdoc />
        public TimeSpan Elapsed => Stopwatch.Elapsed;

        private Stopwatch Stopwatch { get; }

        /// <inheritdoc />
        public void Restart()
        {
            Stopwatch.Restart();
        }
    }
}