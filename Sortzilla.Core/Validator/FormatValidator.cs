using System.Text.RegularExpressions;

namespace Sortzilla.Core.Validator;

public static class FormatValidator
{
    private static readonly Regex LineRegex = LineTools.LineRegex();

    public static (bool HasValidFormat, bool IsSorted, bool HasRepetitions) ValidateLines(Stream stream)
    {
        var isSorted = true;
        var hasRepetitions = false;

        var comparer = new LinesComparer();
        using var reader = new StreamReader(stream);

        if (!GetNextLine(reader, out string previousLine) || !LineRegex.IsMatch(previousLine))
            return (false, false, false);

        while(GetNextLine(reader, out string currentLine))
        {
            var match = LineRegex.Match(currentLine);
            if(!match.Success || int.Parse(match.Groups["number"].Value) < 1)
                return (HasValidFormat: false, false, hasRepetitions);

            if (isSorted && comparer.Compare(previousLine, currentLine) > 0)
                isSorted = false;

            if (!hasRepetitions)
            {
                // works only if adjacent or already sorted
                hasRepetitions = match.Groups["string"].Value == LineRegex.Match(previousLine).Groups["string"].Value;
            }

            previousLine = currentLine;
        }

        return (HasValidFormat: true, isSorted, hasRepetitions);
    }

    private static bool GetNextLine(StreamReader reader, out string line)
    {
        line = string.Empty;
        
        var input = reader.ReadLine();
        if (string.IsNullOrEmpty(input))
            return false;

        line = input;

        return true;
    }


}
