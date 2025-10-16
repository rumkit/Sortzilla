using Sortzilla.Core.Generator;
using Sortzilla.Core.Sorter;
using Sortzilla.Core.Validator;
using Sortzilla.Tests.TestUtils;

namespace Sortzilla.Tests.IntegrationTests;

[NotInParallel]
[Explicit]
internal class FileGenerationTest
{
    private const string TestFilePath = GlobalHooks.TestFileName;
    private const string OutputFilePath = GlobalHooks.OutputFileName;
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
    public async Task Generate_And_ValidateFormat()
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

    [Test]
    public async Task Generate_And_SelfValidateContents()
    {
        using (var fStream = File.Create(TestFilePath))
        using (var writer = new StreamWriter(fStream, bufferSize: 10_000))
        {
            _optimizedGenerator.GenerateLines(100_000, writer.Write);
        }

        var validator = new FileContentsValidator();
        validator.CollectData(TestFilePath);

        var result = validator.Validate(TestFilePath);
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Generate_Sort_And_Validate()
    {
        using (var fStream = File.Create(TestFilePath))
        using (var writer = new StreamWriter(fStream, bufferSize: 10_000))
        {
            _optimizedGenerator.GenerateLines(100_000, writer.Write);
        }
        var validator = new FileContentsValidator();
        validator.CollectData(TestFilePath);

        await SortComposer.SortFileAsync(TestFilePath, OutputFilePath);

        using var readStream = File.OpenRead(OutputFilePath);
        var formatValidationResult = FormatValidator.ValidateLines(readStream);
        var contentValidationResult = validator.Validate(TestFilePath);

        await Assert.That(contentValidationResult).IsTrue();
        await Assert.That(formatValidationResult.HasValidFormat).IsTrue();
        await Assert.That(formatValidationResult.IsSorted).IsTrue();
        await Assert.That(formatValidationResult.HasRepetitions).IsTrue();
    }
}
