using Sortzilla.Core.Sorter;
using Spectre.Console;
using Spectre.Console.Cli;

public class SortCommand : AsyncCommand<SortCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<FileName>")]
        public required string FileName { get; set; }

        [CommandOption("-o|--output <OuputFileName>")]
        public string? OutputFileName { get; set; }

        [CommandOption("-w|--workers <WorkersCount>")]
        public int? WorkersCount { get; set; }

        [CommandOption("-m|--memoryLimit <MemoryLimitPerChunk>")]
        public int? MemoryLimit { get; set; }

        [CommandOption("-t|--tempPath <TemporaryFolderPath>")]
        public string? TempDirectory { get; set; }
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

            var sortSettings = new SortSettings
            {
                MaxWorkersCount = settings.WorkersCount,
                ChunkSizeBytes = settings.MemoryLimit,
                TempPath = settings.TempDirectory
            };
            await SortComposer.SortFileAsync(settings.FileName, settings.OutputFileName, sortSettings);

            validateTask.Increment(1);
        });

        return 0;
    }
}
