namespace TalkFolio.Api;

public partial class Program
{
    public static WebApplication BuildApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddOptions<TalkCatalogRepositoryOptions>()
            .BindConfiguration("TalkCatalogRepository");

        builder.Services.AddSingleton<ITalkCatalogRepository, FileSystemTalkCatalogRepository>();

        var app = builder.Build();

        app.MapGet("/", () => Results.Ok());

        app.MapGet(
            "/talks",
            async Task<IResult> (
                ITalkCatalogRepository repository,
                ILoggerFactory loggerFactory,
                CancellationToken cancellationToken) =>
            {
                var logger = loggerFactory.CreateLogger("TalkFolio.Api.TalksEndpoint");
                logger.LogInformation("Handling GET /talks request.");
                var catalog = await repository.LoadAsync(cancellationToken).ConfigureAwait(false);
                logger.LogInformation("Returning {TalkCount} talks from GET /talks.", catalog.Talks.Count);
                logger.LogTrace(
                    "Returning talk payload for GET /talks with talk IDs {TalkIds}.",
                    catalog.Talks.Select(static talk => talk.Id).ToArray());
                return Results.Ok(catalog.Talks);
            });

        return app;
    }

    public static void Main(string[] args)
    {
        BuildApp(args).Run();
    }
}
