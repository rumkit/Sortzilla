using System.Threading.Channels;

namespace Sortzilla.Core.Sorter;

internal class FileMergeProducer(ChannelWriter<FileMergeDto> writer, SortContext context)
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private string _file1 = string.Empty;

    public string? ResultFileName { get; private set; }

    public async Task MergeAsync()
    {
        foreach(var fileName in Directory.EnumerateFiles(context.WorkingDirectory))
        {
            await OnNewFileReadyAsync(fileName, new FileInfo(fileName).Length);
        }
    }

    public async Task OnNewFileReadyAsync(string fileName, long fileSize)
    {
        // The file is the final merged file
        if(fileSize >= context.FileSize)
        {
            writer.Complete();
            ResultFileName = fileName;
            return;
        }

        await _semaphore.WaitAsync();

        try
        {
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
        finally
        {
            _semaphore.Release();
        }        
    }
}