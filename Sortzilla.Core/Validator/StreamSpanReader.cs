using System.Text;

namespace Sortzilla.Core.Validator;

internal class StreamSpanReader(Stream stream, bool leaveOpen = false, int bufferSize = 100)
    : IDisposable
{
    private readonly byte[] _newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
    private readonly byte[] _buffer = new byte[bufferSize];
    private int _bytesCount;

    private bool _eosReached;

    public int ReadLine(Span<char> lineBuffer)
    {
        if(!_eosReached)
            _bytesCount += stream.Read(_buffer, _bytesCount, bufferSize - _bytesCount);

        if(!_eosReached && _bytesCount < bufferSize)
            _eosReached = true;

        if (_eosReached && _bytesCount == 0)
            return 0;

        var spanBuffer = _buffer.AsSpan();
        var newLineIndex = spanBuffer[.. _bytesCount].IndexOf(_newLineBytes);
        if(newLineIndex < 0)
        {
            if (!_eosReached && stream.Position != stream.Length)
                throw new InvalidOperationException("Internal buffer overflow");
            
            Encoding.UTF8.GetChars(spanBuffer[.._bytesCount], lineBuffer);
            var result = _bytesCount;
            _bytesCount = 0;
            return result;
        }

        Encoding.UTF8.GetChars(spanBuffer[..newLineIndex], lineBuffer);
        var leftoverSize = _bytesCount - (newLineIndex + _newLineBytes.Length);
        if(leftoverSize > 0)
        {
            spanBuffer[(newLineIndex + _newLineBytes.Length) .. _bytesCount].CopyTo(spanBuffer);            
        }

        _bytesCount = leftoverSize;
        return newLineIndex;
    }

    public void Dispose()
    {
        if(!leaveOpen)
            stream.Dispose();
    }
}
