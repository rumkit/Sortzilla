using Sortzilla.Core.Sorter;
using Sortzilla.Tests.TestUtils;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Channels;

namespace Sortzilla.Tests;

internal class FileSplitProducerTests
{
    private const string Input = """
                                 1. first line
                                 2. second line
                                 3. third line
                                 """;


    [Test]
    public async Task SplitAsync_WhenCalled_ShouldSplitStreamIntoChunks()
    {
        var channel = Channel.CreateUnbounded<FileSplitDto>();
        var producer = new FileSplitProducer(channel.Writer, SortContextFactory.GetContext(Input.Length, 16));

        using var inputStream = new MemoryStream(Encoding.ASCII.GetBytes(Input));
        await producer.SplitAsync(inputStream);

        var messages = channel.Reader.ReadAllAsync()
            .ToBlockingEnumerable()
            .ToArray();

        await Assert.That(channel.Writer.TryComplete()).IsFalse();
        await Assert.That(messages).HasCount(2);
    }
}

