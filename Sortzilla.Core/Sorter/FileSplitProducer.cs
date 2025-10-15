using System.Threading.Channels;

namespace Sortzilla.Core.Sorter;

internal class FileSplitProducer(ChannelWriter<FileSplitDto> channelWriter, SortSettings settings)
{
    public async Task SplitAsync(Stream inputFile)
    {
        using var reader = new StreamReader(inputFile);
        var chunkSizeTarget = settings.ChunkSizeBytes;

        // Split file to chunks and send to consumers
        while (!reader.EndOfStream)
        {
            var chunkLines = new List<string>(chunkSizeTarget / SortSettings.MaxLineLength);
            var currentChunkSize = 0;
            while (!reader.EndOfStream && currentChunkSize < chunkSizeTarget)
            {
                var nextLine = await reader.ReadLineAsync();
                if(nextLine is not null)
                {
                    chunkLines.Add(nextLine);
                    currentChunkSize += nextLine.Length + Environment.NewLine.Length;
                }
            }

            await channelWriter.WriteAsync(new FileSplitDto
            {
                Lines = [.. chunkLines]
            });
        }

        channelWriter.Complete();        
    }
}
