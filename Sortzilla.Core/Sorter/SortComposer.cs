using System.Threading.Channels;

namespace Sortzilla.Core.Sorter;

public static class SortComposer
{
    public static async Task SortFileAsync(string fileName, string? outputFileName, SortSettings? settings = null)
    {
        var sortContext = SortContext.GetContext(fileName, outputFileName, settings);

        await using var inputFileStream = File.OpenRead(fileName);

        // Prepare a working directory for temp files
        if (Directory.Exists(sortContext.WorkingDirectory))
            Directory.Delete(sortContext.WorkingDirectory, true);

        Directory.CreateDirectory(sortContext.WorkingDirectory);

        // Prepare producer and consumers for splitting the input file in parts
        var channelBound = sortContext.Settings.MaxWorkersCount > 3 
            ? sortContext.Settings.MaxWorkersCount / 2 
            : sortContext.Settings.MaxWorkersCount;

        var splitChannel = Channel.CreateBounded<FileSplitDto>(new BoundedChannelOptions(channelBound)
        {
            SingleWriter = true
        });

        var splitProducer = new FileSplitProducer(splitChannel.Writer, sortContext);
        var splitConsumer = new FileSplitConsumer(splitChannel.Reader, sortContext);

        splitConsumer.Run();
        await splitProducer.SplitAsync(inputFileStream);

        // Wait for all consumers to finish processing
        await splitConsumer;

        // and start merging temp files
        var mergeChannel = Channel.CreateUnbounded<FileMergeDto>(new UnboundedChannelOptions()
        {
            SingleWriter = true,
        });
        var mergeProducer = new FileMergeProducer(mergeChannel.Writer, sortContext);
        var mergeConsumer = new FileMergeConsumer(mergeChannel.Reader, sortContext, mergeProducer.OnNewFileReadyAsync);

        mergeConsumer.Run();
        await mergeProducer.MergeAsync();        

        // Wait for all merge workers to complete
        await mergeConsumer;

        if (string.IsNullOrEmpty(mergeProducer.ResultFileName))
            throw new ApplicationException("Cannot locate result file");

        // and move the resulting file to the output destination
        File.Move(mergeProducer.ResultFileName, sortContext.OutputFileName);
    }
}
