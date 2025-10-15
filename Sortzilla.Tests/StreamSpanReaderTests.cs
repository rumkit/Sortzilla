using System.Text;
using Sortzilla.Core.Validator;

namespace Sortzilla.Tests;

internal class StreamSpanReaderTests
{
    private const string TestData =
        "1. Apple\r\n" +
        "2. Banana\r\n" +
        "3. Cherry\r\n" +
        "4. Date\r\n" +
        "5. Elderberry\r\n";

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
