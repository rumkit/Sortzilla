using System.Threading.Channels;

namespace Sortzilla.Core.Sorter;

internal class FileSplitConsumer(ChannelReader<FileSplitDto> channelReader, SortContext context) : ParallelConsumerBase(context)
{ 
    private static readonly IComparer<string> LinesComparer = new OptimizedLinesComparer();
    private readonly SortContext _context = context;

    protected override async Task WorkerPayload()
    {
        while (await channelReader.WaitToReadAsync())
        {
            if (channelReader.TryRead(out var fileChunk))
            {
                var sortedLines = fileChunk.Lines.OrderBy(x => x, LinesComparer);
                var partName = $"{Guid.NewGuid():N}.part";
                await File.WriteAllLinesAsync(Path.Combine(_context.WorkingDirectory, partName), sortedLines);
            }                    
        }
    }    
}
