using System;

namespace RaspiFanController.Logic
{
    public interface IWrappedStopwatch
    {
        bool IsRunning { get; }

        TimeSpan Elapsed { get; }

        void Restart();
    }
}