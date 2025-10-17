using System.Threading.Channels;
using Sortzilla.Core.Sorter;
using Sortzilla.Tests.TestUtils;

namespace Sortzilla.Tests;

internal class FileMergeProducerTests
{
    private readonly Channel<FileMergeDto> _channel = Channel.CreateUnbounded<FileMergeDto>();


    [Test]
    public async Task OnNewFileReadyAsync_WhenFileSizeMatchesInitialFile_ShouldComplete()
    {
        const int fileSize = 1024;
        const string fileName = nameof(fileName);
        var producer = new FileMergeProducer(_channel.Writer, SortContextFactory.GetContext(fileSize, 16));

        await producer.OnNewFileReadyAsync(fileName, fileSize);

        await Assert.That(_channel.Writer.TryComplete()).IsFalse();
        await Assert.That(producer.ResultFileName).IsEqualTo(fileName);
    }

    [Test]
    public async Task OnNewFileReadyAsync_WhenFilesReady_ShouldPairTogetherAndSend()
    {
        const int fileSize = 1024;
        const string fileName1 = nameof(fileName1);
        const string fileName2 = nameof(fileName2);
        var producer = new FileMergeProducer(_channel.Writer, SortContextFactory.GetContext(fileSize, 16));

        await producer.OnNewFileReadyAsync(fileName1, 100);
        await producer.OnNewFileReadyAsync(fileName2, 100);

        var dto = await _channel.Reader.ReadAsync();

        await Assert.That(_channel.Writer.TryComplete()).IsTrue();
        await Assert.That(dto.File1).IsEqualTo(fileName1);
        await Assert.That(dto.File2).IsEqualTo(fileName2);
    }
}

