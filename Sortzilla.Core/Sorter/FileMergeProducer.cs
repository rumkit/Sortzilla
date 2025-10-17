using System.Threading.Channels;

namespace Sortzilla.Core.Sorter;

internal class FileMergeProducer(ChannelWriter<FileMergeDto> writer, SortContext context)
{
    private readonly SemaphoreSlim _semaphore = new (1, 1);
    private string _file1 = string.Empty;

    public string? ResultFileName { get; private set; }

    public async ValueTask MergeAsync()
    {
        // block adding new files until initial processing is over
        await _semaphore.WaitAsync();
        try
        {
            foreach (var fileName in Directory.GetFiles(context.WorkingDirectory))
            {
                await OnNewFileReadyInternalAsync(fileName, new FileInfo(fileName).Length);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask OnNewFileReadyAsync(string fileName, long fileSize)
    {
        await _semaphore.WaitAsync();

        try
        {
            await OnNewFileReadyInternalAsync(fileName, fileSize);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    internal async ValueTask OnNewFileReadyInternalAsync(string fileName, long fileSize)
    {
        // The file is the final merged file
        if (fileSize >= context.FileSize)
        {
            writer.Complete();
            ResultFileName = fileName;
            return;
        }

        if (string.IsNullOrEmpty(_file1))
        {
            _file1 = fileName;
            return;
        }

        await writer.WriteAsync(new FileMergeDto
        {
            File1 = _file1,
            File2 = fileName
        });

        _file1 = string.Empty;
    }
}