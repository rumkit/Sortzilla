using Sortzilla.Core.Sorter;
using Spectre.Console;
using Spectre.Console.Cli;

public class SortCommand : AsyncCommand<SortCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<FileName>")]
        public required string FileName { get; set; }
    }

    public async override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        await AnsiConsole.Progress()
        .AutoClear(false)   // Do not remove the task list when done
        .HideCompleted(false)   // Hide tasks as they are completed
        .Columns(
        [
                new TaskDescriptionColumn(),    // Task description
                new ProgressBarColumn(),        // Progress bar
                new ElapsedTimeColumn(),        // Elapsed time
                new SpinnerColumn(),            // Spinner
        ])
        .StartAsync(async ctx =>
        {
            var validateTask = ctx.AddTask("Sorting file", maxValue: 1);
            validateTask.IsIndeterminate = true;
            await SortComposer.SortFileAsync(settings.FileName);
            validateTask.Increment(1);
        });

        return 0;
    }
}
