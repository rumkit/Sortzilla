using Sortzilla.Core.Sorter;

namespace Sortzilla.Tests;

internal class SortContextTests
{
    public static IEnumerable<Func<SortSettings?>> SortSettingsSource()
    {
        yield return () => new SortSettings();
        yield return() => null;
    }

    [Test]
    [MethodDataSource(nameof(SortSettingsSource))]
    public async Task MapSettingsToInternal_WhenSettingsNotProvided_ShouldReturnDefault(SortSettings settings)
    {
        const int fileLength = 2_000_000;
        var result = SortContext.MapSettingsToInternal(settings, fileLength);
        
        var expectedWorkerCount = Environment.ProcessorCount;
        await Assert.That(result).IsNotNull();
        await Assert.That(result.TempPath).IsEqualTo(Path.GetTempPath());
        await Assert.That(result.MaxWorkersCount).IsEqualTo(expectedWorkerCount);
        await Assert.That(result.ChunkSizeBytes).IsEqualTo(fileLength / expectedWorkerCount);
    }

    [Test]
    public async Task MapSettingsToInternal_WhenSmallFile_ShouldReturn1KBChunkSize()
    {
        const int fileLength = 2500;
        var result = SortContext.MapSettingsToInternal(settings: null, inputFileLength: fileLength);

        await Assert.That(result.ChunkSizeBytes).IsEqualTo(1024);
    }

    [Test]
    public async Task MapSettingsToInternal_WhenSuperSmallFile_ShouldReturnFileSizedChunks()
    {
        const int fileLength = 123;
        var result = SortContext.MapSettingsToInternal(settings: null, inputFileLength: fileLength);

        await Assert.That(result.ChunkSizeBytes).IsEqualTo(123);
    }
}

