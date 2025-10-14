using System.Text.RegularExpressions;

namespace Sortzilla.Core.Validator;

public static class FormatSpanValidator
{
    private readonly static Regex LineRegex = LineTools.LineRegex();

    public static (bool HasValidFormat, bool IsSorted, bool HasRepetitions) ValidateLines(Stream stream)
    {
        var isSorted = true;
        var hasRepetitions = false;
        Span<char> firstLine = stackalloc char[100];
        Span<char> secondLine = stackalloc char[100];

        var comparer = new LinesComparer();
        using var reader = new StreamSpanReader(stream);

        if (!ReadNextLine(reader, firstLine, out int firstLineLength) || !LineRegex.IsMatch(firstLine[..firstLineLength]))
            return (false, false, false);
        int firstDotIndex = firstLine[.. firstLineLength].IndexOf('.');
        int firstNumber = int.Parse(firstLine[..firstDotIndex]);


        while (ReadNextLine(reader, secondLine, out int secondLineLength))
        {
            var isMatch = LineRegex.IsMatch(secondLine[..secondLineLength]);
            var secondDotIndex = secondLine.IndexOf('.');
            var secondNumber = -1;
            if (isMatch && secondDotIndex > 0)
                secondNumber = int.Parse(secondLine[..secondDotIndex]);

            if (!isMatch || secondDotIndex < 0 || secondNumber < 0)
                return (HasValidFormat: false, false, hasRepetitions);

            if(isSorted)
            {
                var compareResult =
                firstLine[firstDotIndex..firstLineLength].SequenceCompareTo(secondLine[secondDotIndex..secondLineLength]);

                if (compareResult == 0)
                    compareResult = firstNumber.CompareTo(secondNumber);

                if (compareResult > 0)
                    isSorted = false;
            }

            if (!hasRepetitions)
            {
                // works only if adjacent or already sorted
                hasRepetitions = firstLine[firstDotIndex..firstLineLength].SequenceEqual(secondLine[secondDotIndex .. secondLineLength]);
            }

            secondLine[.. secondLineLength].CopyTo(firstLine);
            firstLineLength = secondLineLength;
            firstDotIndex = secondDotIndex;
            firstNumber = secondNumber;
        }

        return (HasValidFormat: true, isSorted, hasRepetitions);
    }

    private static bool ReadNextLine(StreamSpanReader reader, Span<char> buffer, out int length)
    {
        length = reader.ReadLine(buffer);
        return length > 0;
    }
}
