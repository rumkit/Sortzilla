using Sortzilla.Core.Generator;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Text.RegularExpressions;

public class GenerateCommand : AsyncCommand<GenerateCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<FileName>")]
        public required string FileName { get; set; }

        [CommandOption("-s|--size <SIZE>", isRequired: true)]
        public required string Size { get; set; }

        [CommandOption("-d|--dictionary <FileName>")]
        public string? DictionaryFileName { get; set; }
    }


    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        Regex sizeRegex = new(@"^(\d+)([KMG]?)$", RegexOptions.IgnoreCase);
        var match = sizeRegex.Match(settings.Size);
        if (!match.Success)
            throw new ApplicationException("Size must be in format <number>[K|M|G], e.g. 10K, 500M, 2G");

        long size = long.Parse(match.Groups[1].Value);
        string sizeSuffix = match.Groups[2].Value.ToUpperInvariant();
        size = sizeSuffix switch
        {
            "K" => size * 1_024L,
            "M" => size * 1_024L * 1_024L,
            "G" => size * 1_024L * 1_024L * 1_024L,
            _ => size
        };

        using var fileStream = File.Create(settings.FileName);
        using var streamWriter = new StreamWriter(fileStream, bufferSize: 10_000_000);


        await AnsiConsole.Progress()
        .AutoClear(false)   // Do not remove the task list when done
        .HideCompleted(false)   // Hide tasks as they are completed
        .Columns(
        [
                new TaskDescriptionColumn(),    // Task description
                new ProgressBarColumn(),        // Progress bar
                new PercentageColumn(),         // Percentage
                new ElapsedTimeColumn(),        // Elapsed time
                new SpinnerColumn(),            // Spinner
        ])
        .StartAsync(async ctx =>
        {
            var fileTask = ctx.AddTask("Writing file");

            ISequenceSource<string> wordsSource = settings.DictionaryFileName == null
                ? new RandomCachedDictionaryStringSource()
                : new StaticDictionaryStringSource(File.ReadAllLines(settings.DictionaryFileName));
            var generator = new OptimizedLinesGenerator(wordsSource);
            long bytesWrittern = 0;

            var generatorTask = Task.Run(() => generator.GenerateLines(size, lineSpan =>
            {
                streamWriter.Write(lineSpan);
                bytesWrittern += lineSpan.Length;
            }));

            while (!generatorTask.IsCompleted)
            {
                fileTask.Value = bytesWrittern * 100 / size;
            }
        });

        AnsiConsole.MarkupLine($"[green]Done![/]");

        return 0;
    }
}