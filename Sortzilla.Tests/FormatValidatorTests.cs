using System.Text;

namespace Sortzilla.Tests;

public class FormatValidatorTests
{
    private const string EmptySource = "";

    private const string InvalidFormatSource = """
        1. Valid line
        Invalid line
        3. Another valid line
        """;

    private const string NumberIsLessThanOneSource = """
        1. Valid line
        -2. Invalid line
        3. Another valid line
        """;

    private const string ValidFormatSource = """
        1. First line
        2. And Second line
        3. Third line
        """;

    private const string ValidFormatSortedSource = """
        1. First line
        2. Second line
        3. Third line
        """;

    private const string ValidFormatDuplicateSortedSource = """
        1. First line
        2. First line
        3. Third line
        """;



    [Test]
    [Arguments(EmptySource, false, false, false)]
    [Arguments(InvalidFormatSource, false, false, false)]
    [Arguments(NumberIsLessThanOneSource, false, false, false)]

    [Arguments(ValidFormatSource, true, false, false)]
    [Arguments(ValidFormatSortedSource, true, true, false)]
    [Arguments(ValidFormatDuplicateSortedSource, true, true, true)]
    public async Task ValidateLines_WhenCalled_ShouldReturnExpectedResult(string source, bool expectedValid, bool expectedSorted, bool expectedRepetitions)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(source));
        var result = Core.Validator.FormatValidator.ValidateLines(stream);
        await Assert.That(result.HasValidFormat).IsEqualTo(expectedValid);
        await Assert.That(result.IsSorted).IsEqualTo(expectedSorted);
        await Assert.That(result.HasRepetitions).IsEqualTo(expectedRepetitions);
    }
}
