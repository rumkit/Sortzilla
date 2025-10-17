using System.Runtime.CompilerServices;

namespace Sortzilla.Core.Sorter;

internal abstract class ParallelConsumerBase(SortContext context)
{
    private readonly List<Task> _workerTasks = new();

    public void Run()
    {
        for (int i = 0; i < context.Settings.MaxWorkersCount; i++)
        {
            _workerTasks.Add(WorkerPayload());
        }
    }

    protected abstract Task WorkerPayload();

    public TaskAwaiter GetAwaiter() => Task.WhenAll(_workerTasks).GetAwaiter();
}