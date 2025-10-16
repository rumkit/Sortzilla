using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Sortzilla.Core.Sorter;

internal class FileSplitConsumer(ChannelReader<FileSplitDto> channelReader, SortContext context)
{
    private readonly List<Task> _workerTasks = new();
    private static readonly IComparer<string> LinesComparer = new OptimizedLinesComparer();

    public void Run()
    {
        for (int i = 0; i < context.Settings.MaxWorkersCount; i++)
        {
            _workerTasks.Add(Task.Run(WorkerPayload));
        }
    }

    private async Task WorkerPayload()
    {
        while (await channelReader.WaitToReadAsync())
        {
            if (channelReader.TryRead(out var fileChunk))
            {
                var sortedLines = fileChunk.Lines.OrderBy(x => x, LinesComparer);
                var partName = $"{Guid.NewGuid():N}.part";
                await File.WriteAllLinesAsync(Path.Combine(context.WorkingDirectory, partName), sortedLines);
            }                    
        }
    }

    public TaskAwaiter GetAwaiter() => Task.WhenAll(_workerTasks).GetAwaiter();
}
