using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<GenerateCommand>("generate")
        .WithAlias("gen")
        .WithDescription("Generates a new file")
        .WithExample("generate", "output.txt", "--size", "10G")
        .WithExample("gen", "output.txt", "-s", "10K", "-d", "dictionary.txt");

    config.AddCommand<ValidateCommand>("validate")
        .WithAlias("valid")
        .WithDescription("Validates existing file and checks whether it is sorted")
        .WithExample("valid", "file.txt");

    config.AddCommand<SortCommand>("sort")
        .WithDescription("Sorts the provided file")
        .WithExample("sort", "file.txt");

});

return app.Run(args);
