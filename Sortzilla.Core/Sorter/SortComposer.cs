using System.Dynamic;
using System.Threading.Channels;

namespace Sortzilla.Core.Sorter;

public static class SortComposer
{
    public static async Task SortFileAsync(string fileName)
    {
        var settings = new SortSettings()
        {
            //todo: replace with CLI parameters or computed optimal ones
            MaxWorkersCount = 8,
            ChunkSizeBytes = 1024 * 1024 * 10
        };
        
        using var inputFileStream = File.OpenRead(fileName);
        var fileSize = inputFileStream.Length;

        var tempFilesDirectory = Path.Combine(settings.TempPath, fileName);
        if(!Directory.Exists(tempFilesDirectory))
            Directory.CreateDirectory(tempFilesDirectory);

        // Prepare producer and consumers for splitting the input file in parts
        var channelBound = settings.MaxWorkersCount > 3 ? settings.MaxWorkersCount / 2 : settings.MaxWorkersCount;
        var splitChannel = Channel.CreateBounded<FileSplitDto>(new BoundedChannelOptions(channelBound)
        {
            SingleReader = false,
            SingleWriter = true,
            FullMode = BoundedChannelFullMode.Wait
        });

        var splitProducer = new FileSplitProducer(splitChannel.Writer, settings);
        var splitConsumer = new FileSplitConsumer(splitChannel.Reader, settings, fileName);

        splitConsumer.Run();
        await splitProducer.SplitAsync(inputFileStream);

        // Wait for all consumers to finish processing
        await splitConsumer;

        // and start merging temp files
        var mergeChannel = Channel.CreateUnbounded<FileMergeDto>();
        var mergeProducer = new FileMergeProducer(mergeChannel.Writer, settings, fileName, fileSize);
        var mergeConsumer = new FileMergeConsumer(mergeChannel.Reader, settings, fileName, mergeProducer.OnNewFileReadyAsync);

        mergeConsumer.Run();
        await mergeProducer.MergeAsync();

        await mergeConsumer;

        File.Move(mergeProducer.ResultFileName, fileName + "-sorted");
    }
}
