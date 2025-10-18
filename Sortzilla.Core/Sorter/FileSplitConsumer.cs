using System.Threading.Channels;

namespace Sortzilla.Core.Sorter;

internal class FileSplitConsumer(ChannelReader<FileSplitDto> channelReader, SortContext context)
    : ParallelConsumerBase(context.Settings.MaxWorkersCount)
{ 
    private static readonly IComparer<string> LinesComparer = new OptimizedLinesComparer();

    protected override async Task WorkerPayload()
    {
        while (await channelReader.WaitToReadAsync())
        {
            while (channelReader.TryRead(out var fileChunk))
            {
                var sortedLines = fileChunk.Lines.OrderBy(x => x, LinesComparer);
                var partName = $"{Guid.NewGuid():N}.part";
                await File.WriteAllLinesAsync(Path.Combine(context.WorkingDirectory, partName), sortedLines);
            }                    
        }
    }    
}
