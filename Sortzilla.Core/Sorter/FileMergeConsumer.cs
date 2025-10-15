using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Sortzilla.Core.Sorter;

internal class FileMergeConsumer(ChannelReader<FileMergeDto> channelReader, SortSettings settings, string inputFileName, Func<string, long, Task> fileMergedCallback)
{
    private readonly List<Task> _workerTasks = new();
    private readonly string WorkingDir = Path.Combine(settings.TempPath, inputFileName);
    private readonly IComparer<string> _comparer = new LinesComparer();

    public void Run()
    {
        for (int i = 0; i < settings.MaxWorkersCount; i++)
        {
            _workerTasks.Add(Task.Run(WorkerPayload));
        }
    }

    private async Task WorkerPayload()
    {
        while (await channelReader.WaitToReadAsync())
        {
            if(channelReader.TryRead(out var filesPair))
            {
                if (string.IsNullOrEmpty(filesPair.File1) && string.IsNullOrEmpty(filesPair.File2))
                    continue;

                var (outputFileName, outputFileSize) = await MergeFiles(filesPair.File1, filesPair.File2);
                await fileMergedCallback(outputFileName, outputFileSize);
            }            
        }
    }

    internal async Task<(string, long)> MergeFiles(string file1, string file2)
    {
        var outputFileName = Path.Combine(WorkingDir, $"{Guid.NewGuid():N}.part");
        using var outputWriter = File.CreateText(outputFileName);

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

            if (line1 != null)
                await outputWriter.WriteLineAsync(line1);
            if (line2 != null)
                await outputWriter.WriteLineAsync(line2);

            while (!inputReader1.EndOfStream)
            {            
                await outputWriter.WriteLineAsync(await inputReader1.ReadLineAsync());
            }

            while (!inputReader2.EndOfStream)
            {
                await outputWriter.WriteLineAsync(await inputReader2.ReadLineAsync());
            }
        }        

        File.Delete(file1);
        File.Delete(file2);

        // flushing writer before getting file size for the callback
        await outputWriter.FlushAsync();
        return (outputFileName, outputWriter.BaseStream.Length);
    }

    public TaskAwaiter GetAwaiter() => Task.WhenAll(_workerTasks).GetAwaiter();
}
