using Sortzilla.Core.Generator;

namespace Sortzilla.Tests.Integration;


[NotInParallel]
internal class FileGenerationTest : IDisposable
{
    private const string TestFilePath = "testfile.txt";
    private static readonly SimpleLinesGenerator _generator = new(
        new RandomNumberSource(),
        new RandomStringSource()
    );

    public void Dispose()
    {
        File.Delete(TestFilePath);
    }

    [Test]
    public async Task GenerateFileOfSufficientSize()
    {
        const long targetSize = 10_000_000_000; // ~1 GB
        using var fStream = File.Create(TestFilePath);
        using var writer = new StreamWriter(fStream, bufferSize: 10_000_000);
        {
            foreach (var line in _generator.GenerateLines(targetSize))
            {
                await writer.WriteAsync(line);
            }

            await writer.FlushAsync();
        }

        var fileSize = new FileInfo(TestFilePath).Length;

        await Assert.That(fileSize).IsGreaterThanOrEqualTo(targetSize);
    }
}
