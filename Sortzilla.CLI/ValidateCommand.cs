using Sortzilla.Core.Validator;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Sortzilla.CLI;

public class ValidateCommand : AsyncCommand<ValidateCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<FileName>")]
        public required string FileName { get; set; }
    }


    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var fileLength = new FileInfo(settings.FileName).Length;
        await using var fileStream = File.OpenRead(settings.FileName);

        bool hasValidFormat = false;
        bool isSorted = false;
        bool hasRepetitions = false;

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
                var validateTask = ctx.AddTask("Checking file", maxValue: 100);
                long bytesProcessed = 0;

                var workerTask = Task.Run(() =>
                    (hasValidFormat, isSorted, hasRepetitions) = FormatSpanValidator.ValidateLines(fileStream, (x) => bytesProcessed = x));            

                while (!workerTask.IsCompleted)
                {
                    validateTask.Value = bytesProcessed * 100 / fileLength;
                    await Task.Delay(250);
                }

                validateTask.Value = 100;
            });

        var rows = new List<Text>()
        {
            new ($"Has valid format: {hasValidFormat}", GetStyle(hasValidFormat)),
            new ($"Is sorted: {isSorted}", GetStyle(isSorted)),
            new ($"Has repetitions: {hasRepetitions}", GetStyle(hasRepetitions))
        };
        AnsiConsole.Write(new Rows(rows));

        return 0;
    }

    private static Style GetStyle(bool condition) => condition 
        ? new Style(foreground: Color.Green) 
        : new Style(foreground: Color.Red);
}