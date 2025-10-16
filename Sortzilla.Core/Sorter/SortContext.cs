
namespace Sortzilla.Core.Sorter;

internal class SortContext
{
    public required SortSettingsInternal Settings { get; init; }
    public required string InputFileName { get; init; }
    public required string OutputFileName { get; init; }
    public required string WorkingDirectory { get; init; }
    public required long FileSize { get; set; }

    public static SortContext GetContext(string inputFileName, string? outputFileName, SortSettings? settings)
    {
        var fileSize = new FileInfo(inputFileName).Length;
        var settingsInternal = MapSettingsToInternal(settings, fileSize);

        return new SortContext
        {
            InputFileName = inputFileName,
            OutputFileName = outputFileName ?? $"{Path.GetFileNameWithoutExtension(inputFileName)}-sorted.{Path.GetExtension(inputFileName)}", // input.txt => input-sorted.txt
            Settings = settingsInternal,
            WorkingDirectory = Path.Combine(settingsInternal.TempPath, "SortZilla", inputFileName),
            FileSize = fileSize
        };
    }

    internal static SortSettingsInternal MapSettingsToInternal(SortSettings? settings, long inputFileLength)
    {
        var workersCount = settings?.MaxWorkersCount ?? Environment.ProcessorCount;
        var chunkSizeBytes = settings?.ChunkSizeBytes ?? (int)Math.Min(inputFileLength / workersCount, 1024 * 1024 * 128); // each worker gets a chunk, but chunks are less than 128MB by default
        
        // for extra small files chunks are either 1KB or fileSize whatever is smaller
        chunkSizeBytes = Math.Max(chunkSizeBytes, 1024);
        chunkSizeBytes = Math.Min(chunkSizeBytes, (int)inputFileLength);
            

        return new SortSettingsInternal
        {
            TempPath = settings?.TempPath ?? Path.GetTempPath(),
            MaxWorkersCount = workersCount,
            ChunkSizeBytes = chunkSizeBytes
        };            
    }
}