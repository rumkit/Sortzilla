using Sortzilla.Core.Generator;

namespace Sortzilla.Tests.IntegrationTests;


[NotInParallel]
internal class FileGenerationTest : IDisposable
{
    private const string TestFilePath = "testfile.txt";
    private const int SizeToGenerate = 100_000_000; // ~100 MB
    private const int BufferSize = 1_000_000; // ~1 MB
    private static readonly SimpleLinesGenerator _simpleGenerator = new(
        new RandomNumberSource(),
        new RandomStringSource()
    );
    private static readonly OptimizedLinesGenerator _optimizedGenerator = new(
        new RandomWordsGenerator()
,
        new RandomNumberSource());

    public void Dispose()
    {
        File.Delete(TestFilePath);
    }

    [Test]
    public async Task Simple_GenerateFileOfSufficientSize()
    {
        using (var fStream = File.Create(TestFilePath))
        using (var writer = new StreamWriter(fStream, bufferSize: BufferSize))
        {
            foreach (var line in _simpleGenerator.GenerateLines(SizeToGenerate))
            {
                await writer.WriteAsync(line);
            }
        }

        var fileSize = new FileInfo(TestFilePath).Length;
        await Assert.That(fileSize).IsGreaterThanOrEqualTo(SizeToGenerate);
    }

    [Test]
    public async Task Optimized_GenerateFileOfSufficientSize()
    {
        using (var fStream = File.Create(TestFilePath))
        using (var writer = new StreamWriter(fStream, bufferSize: BufferSize))
        {
            _optimizedGenerator.GenerateLines(SizeToGenerate, writer.Write);
        }

        var fileSize = new FileInfo(TestFilePath).Length;
        await Assert.That(fileSize).IsGreaterThanOrEqualTo(SizeToGenerate);
    }
}
