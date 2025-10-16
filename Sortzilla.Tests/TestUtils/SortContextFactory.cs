using Sortzilla.Core.Sorter;

namespace Sortzilla.Tests.TestUtils;

internal class SortContextFactory
{
    public static SortContext GetContext(int fileSize, int chunkSize) => new()
    {
        FileSize = fileSize,
        InputFileName = "input",
        OutputFileName = "output",
        WorkingDirectory = string.Empty,
        Settings = new SortSettingsInternal
        {
            ChunkSizeBytes = chunkSize,
            MaxWorkersCount = 1,
            TempPath = string.Empty
        }
    };
}
