using System.Text;

namespace Sortzilla.Core.Validator;

public class StreamSpanReader : IDisposable
{
    private readonly bool _leaveOpen;
    private readonly Stream _stream;
    private readonly int _bufferSize;
    private readonly byte[] newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
    private int _bytesCount = 0;
    private byte[] _buffer;

    private bool _eosReached;

    public StreamSpanReader(Stream stream, bool leaveOpen = false, int bufferSize = 100)
    {
        _stream = stream;
        _leaveOpen = leaveOpen;
        _bufferSize = bufferSize;
        _buffer = new byte[bufferSize];
    }

    public int ReadLine(Span<char> lineBuffer)
    {
        if(!_eosReached)
            _bytesCount += _stream.Read(_buffer, _bytesCount, _bufferSize - _bytesCount);

        if(!_eosReached && _bytesCount < _bufferSize)
            _eosReached = true;

        if (_eosReached && _bytesCount == 0)
            return 0;

        var spanBuffer = _buffer.AsSpan();
        var newLineIndex = spanBuffer[.. _bytesCount].IndexOf(newLineBytes);
        if(newLineIndex < 0)
        {
            if(_eosReached || _stream.Position == _stream.Length)
            {
                Encoding.UTF8.GetChars(spanBuffer[.._bytesCount], lineBuffer);
                var result = _bytesCount;
                _bytesCount = 0;
                return result;
            }
            else
            {
                throw new InvalidOperationException("Internal buffer overflow");
            }
        }

        Encoding.UTF8.GetChars(spanBuffer[..newLineIndex], lineBuffer);
        var leftoverSize = _bytesCount - (newLineIndex + newLineBytes.Length);
        if(leftoverSize > 0)
        {
            spanBuffer[(newLineIndex + newLineBytes.Length) .. _bytesCount].CopyTo(spanBuffer);            
        }

        _bytesCount = leftoverSize;
        return newLineIndex;
    }

    public void Dispose()
    {
        if(!_leaveOpen)
            _stream.Dispose();
    }
}
