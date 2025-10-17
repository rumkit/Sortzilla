using System.Threading.Channels;

namespace Sortzilla.Core.Sorter;

internal class FileSplitProducer(ChannelWriter<FileSplitDto> channelWriter, SortContext context)
{
    public async Task SplitAsync(Stream inputFile)
    {
        
        var chunkSizeTarget = context.Settings.ChunkSizeBytes;
        using var reader = new StreamReader(inputFile);

        var nextLine = await reader.ReadLineAsync();
        // Split file into chunks and send to consumers
        while (nextLine != null)
        {
            var chunkLines = new List<string>(chunkSizeTarget / SortSettingsInternal.MaxLineLength);
            var currentChunkSize = 0;
            
            while (nextLine != null && currentChunkSize < chunkSizeTarget)
            {
                chunkLines.Add(nextLine);
                currentChunkSize += nextLine.Length + Environment.NewLine.Length;
                
                nextLine = await reader.ReadLineAsync();
            }

            await channelWriter.WriteAsync(new FileSplitDto
            {
                Lines = [.. chunkLines]
            });
        }

        channelWriter.Complete();        
    }
}
