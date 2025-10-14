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

    const string TestSample = """
        1119782830. ARoger mostly searches cheap reveals kernel
        772399000. BFreeware estonia journal platinum parliamentary promised quest renewal this
        1720963445. CGuitar crucial exchange destruction amount annie collectors disciplines
        2125588986. DEconomic highlighted ladder albert
        2109749920. EHeadline vic column ut antigua warren pricing bin advert durham older sanyo
        185719021. FLocations marcus retain cir physiology struct camps gilbert
        906576720. GReflection smithsonian maintain ph dicke joint
        1840748938. GReflection smithsonian maintain ph dicke joint
        1967042264. IMontreal activity automobiles peace gbp triangle randy toronto boats bulletin
        1002880194. JLiterally von officer raises learned via instrumentation transcripts
        1930581224. KArgument fatal records interval literally
        1719978313. LDimensions attach prediction factor brake selective ref info buyer feb spiritual
        1508421561. Mambda exotic sharp asia lcd consent beneath athens
        1032554530. NEnables furniture single bloomberg involved qty limit get column cowboy moment
        
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

    [Test]
    [Arguments(EmptySource, false, false, false)]
    [Arguments(InvalidFormatSource, false, false, false)]
    [Arguments(NumberIsLessThanOneSource, false, false, false)]

    [Arguments(ValidFormatSource, true, false, false)]
    [Arguments(ValidFormatSortedSource, true, true, false)]
    [Arguments(ValidFormatDuplicateSortedSource, true, true, true)]
    [Arguments(TestSample, true, true, true)]
    public async Task ValidateLinesSpan_WhenCalled_ShouldReturnExpectedResult(string source, bool expectedValid, bool expectedSorted, bool expectedRepetitions)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(source));
        var result = Core.Validator.FormatSpanValidator.ValidateLines(stream);
        await Assert.That(result.HasValidFormat).IsEqualTo(expectedValid);
        await Assert.That(result.IsSorted).IsEqualTo(expectedSorted);
        await Assert.That(result.HasRepetitions).IsEqualTo(expectedRepetitions);
    }
}
