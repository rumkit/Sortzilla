namespace Sortzilla.Core;

internal class OptimizedLinesComparer : IComparer<string>
{
    public int Compare(string? left, string? right)
    {
        if(left == null)
            throw new ArgumentException(nameof(left), "Cannot be null");
        if (right == null)
            throw new ArgumentException(nameof(right), "Cannot be null");

        var leftSpan = left.AsSpan();
        var rightSpan = right.AsSpan();

        var leftDotIndex = left.IndexOf('.');
        if (leftDotIndex <= 0 )
            throw new ArgumentException(nameof(left), "Line format is invalid");
        
        var rightDotIndex = rightSpan.IndexOf('.');
        if (rightDotIndex <= 0)
            throw new ArgumentException(nameof(right), "Line format is invalid");       

        var stringPartComparasionResult = leftSpan[leftDotIndex ..].CompareTo(rightSpan[rightDotIndex ..], StringComparison.Ordinal);

        if(stringPartComparasionResult != 0)
            return stringPartComparasionResult;

        if (int.TryParse(leftSpan[.. leftDotIndex], out var leftNumber)
            && int.TryParse(rightSpan[.. rightDotIndex], out var rightNumber))
        {
            return leftNumber.CompareTo(rightNumber);
        }

        throw new ArgumentException("numbers", "Line format is invalid");
    }
}
