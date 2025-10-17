using System.Threading.Channels;

namespace Sortzilla.Core.Sorter;

internal class FileMergeConsumer(ChannelReader<FileMergeDto> channelReader, SortContext context, Func<string, long, ValueTask> fileMergedCallback)
    : ParallelConsumerBase(context)
{
    private readonly IComparer<string> _comparer = new OptimizedLinesComparer();
    private readonly SortContext _context = context;

    protected override async Task WorkerPayload()
    {
        while (await channelReader.WaitToReadAsync())
        {
            if(channelReader.TryRead(out var filesPair))
            {
                var (outputFileName, outputFileSize) = await MergeFiles(filesPair.File1, filesPair.File2);
                await fileMergedCallback(outputFileName, outputFileSize);

                File.Delete(filesPair.File1);
                File.Delete(filesPair.File2);
            }
        }
    }

    internal async Task<(string, long)> MergeFiles(string file1, string file2)
    {
        var outputFileName = Path.Combine(_context.WorkingDirectory, $"{Guid.NewGuid():N}.part");

        await using var outputFile = File.Create(outputFileName);
        await using var outputWriter = new StreamWriter(outputFile, bufferSize: SortSettingsInternal.DefaultWriteBuffer );

        using (var inputReader1 = File.OpenText(file1))
        using (var inputReader2 = File.OpenText(file2))
        {
            string? line1 = await inputReader1.ReadLineAsync();
            string? line2 = await inputReader2.ReadLineAsync();

            while (line1 != null && line2 != null)
            {
                var result = _comparer.Compare(line1, line2);
                if (result < 0)
                {
                    await outputWriter.WriteLineAsync(line1);
                    line1 = await inputReader1.ReadLineAsync();
                }
                else
                {
                    await outputWriter.WriteLineAsync(line2);
                    line2 = await inputReader2.ReadLineAsync();
                }
            }

            while (line1 != null)
            {            
                 await outputWriter.WriteLineAsync(line1);
                 line1 = await inputReader1.ReadLineAsync();
            }

            while (line2 != null)
            {
                await outputWriter.WriteLineAsync(line2);
                line2 = await inputReader2.ReadLineAsync();
            }
        }

        // flushing writer before getting file size for the callback
        await outputWriter.FlushAsync();
        return (outputFileName, outputWriter.BaseStream.Length);
    }
}
