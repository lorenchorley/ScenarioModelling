using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<ManualRunCommand>("run")
          .WithDescription("");

});

return await app.RunAsync(args).ConfigureAwait(false);