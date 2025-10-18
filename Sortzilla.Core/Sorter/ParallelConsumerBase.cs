using System.Runtime.CompilerServices;

namespace Sortzilla.Core.Sorter;

internal abstract class ParallelConsumerBase(int maxConcurrentWorkers)
{
    private readonly List<Task> _workerTasks = new();

    public void Run()
    {
        for (int i = 0; i < maxConcurrentWorkers; i++)
        {
            _workerTasks.Add(WorkerPayload());
        }
    }

    protected abstract Task WorkerPayload();

    public TaskAwaiter GetAwaiter() => Task.WhenAll(_workerTasks).GetAwaiter();
}