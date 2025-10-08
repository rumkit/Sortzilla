using Sortzilla.Core.Generator;
using Sortzilla.Core.Validator;

namespace Sortzilla.Tests.IntegrationTests;


[NotInParallel]
internal class FileGenerationTest
{
    private const string TestFilePath = GlobalHooks.TestFileName;
    private const int SizeToGenerate = 100_000_000; // ~100 MB
    private const int BufferSize = 1_000_000; // ~1 MB
    private static readonly SimpleLinesGenerator _simpleGenerator = new(
        new RandomPositiveNumberSource(),
        new RandomStringSource()
    );
    private static readonly OptimizedLinesGenerator _optimizedGenerator = new(
        new RandomStringPartWriter()
,
        new RandomPositiveNumberSource());

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

    [Test]
    public async Task Generate_And_Validate()
    {
        using (var fStream = File.Create(TestFilePath))
        using (var writer = new StreamWriter(fStream, bufferSize: 10_000))
        {
            _optimizedGenerator.GenerateLines(100_000, writer.Write);
        }


        using var readStream = File.OpenRead(TestFilePath);        
        var result = FormatValidator.ValidateLines(readStream);

        await Assert.That(result.HasValidFormat).IsTrue();
        await Assert.That(result.IsSorted).IsFalse();
        await Assert.That(result.HasRepetitions).IsTrue();
    }
}
