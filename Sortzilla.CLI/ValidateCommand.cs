using Sortzilla.Core.Validator;
using Spectre.Console;
using Spectre.Console.Cli;

public class ValidateCommand : Command<ValidateCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<FileName>")]
        public required string FileName { get; set; }
    }


    public override int Execute(CommandContext context, Settings settings)
    {  
        using var fileStream = File.OpenRead(settings.FileName);

        bool hasValidFormat = false;
        bool isSorted = false;
        bool hasRepetitions = false;

        AnsiConsole.Progress()
        .AutoClear(false)   // Do not remove the task list when done
        .HideCompleted(false)   // Hide tasks as they are completed
        .Columns(
        [
                new TaskDescriptionColumn(),    // Task description
                new ProgressBarColumn(),        // Progress bar
                new ElapsedTimeColumn(),        // Elapsed time
                new SpinnerColumn(),            // Spinner
        ])
        .Start(ctx =>
        {
            var validateTask = ctx.AddTask("Checking file", maxValue: 1);            
            validateTask.IsIndeterminate = true;
            (hasValidFormat, isSorted, hasRepetitions) = FormatSpanValidator.ValidateLines(fileStream);
            validateTask.Increment(1);
        });

        var rows = new List<Text>()
        {
            new Text($"Has valid format: {hasValidFormat}", GetStyle(hasValidFormat)),
            new Text($"Is sorted: {isSorted}", GetStyle(isSorted)),
            new Text($"Has repetitions: {hasRepetitions}", GetStyle(hasRepetitions))
        };
        AnsiConsole.Write(new Rows(rows));

        return 0;
    }

    private Style GetStyle(bool condition) => condition ? new Style(foreground: Color.Green) : new Style(foreground: Color.Red);
}