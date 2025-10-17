using System.Text;
using Sortzilla.Core.Validator;

namespace Sortzilla.Tests;

internal class StreamSpanReaderTests
{
    private static readonly string TestData =
        "1. Apple" + Environment.NewLine + 
        "2. Banana" + Environment.NewLine +
        "3. Cherry" + Environment.NewLine +
        "4. Date" + Environment.NewLine +
        "5. Elderberry";

    [Test]
    public async Task Read()
    {
        using MemoryStream stream = new(Encoding.UTF8.GetBytes(TestData));
        using var reader = new StreamSpanReader(stream, bufferSize: 100);
        Span<char> lineBuffer = stackalloc char[100];
        List<string> lines = new();

        int charsCount = 0;
        do
        {
            charsCount = reader.ReadLine(lineBuffer);
            lines.Add(lineBuffer[..charsCount].ToString());
        } while (charsCount > 0);

        await Assert.That(lines).IsEquivalentTo(new[]
        {
            "1. Apple",
            "2. Banana",
            "3. Cherry",
            "4. Date",
            "5. Elderberry",
            ""
        });
    }
}
