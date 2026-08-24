namespace TalkFolio.Api;

using TalkFolio.Data.YamlFile;
using TalkFolio.Interfaces;
using TalkFolio.Services;

#pragma warning disable CA1052, CA1515
public partial class Program
{
    public static WebApplication BuildApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddOptions<TalkCatalogOptions>()
            .BindConfiguration("TalkCatalog");

        builder.Services.AddSingleton<ITalkCatalogRepository, TalkCatalogRepository>();
        builder.Services.AddSingleton<TalkCatalogService>();

        var app = builder.Build();

        app.MapGet("/", () => Results.Ok());

        app.MapGet(
            "/talks",
            async Task<IResult> (
                TalkCatalogService service,
                ILoggerFactory loggerFactory,
                CancellationToken cancellationToken) =>
            {
                var logger = loggerFactory.CreateLogger("TalkFolio.Api.TalksEndpoint");
                ProgramLog.HandlingGetTalksRequest(logger);
                var catalog = await service.LoadAsync(cancellationToken).ConfigureAwait(false);
                ProgramLog.ReturningTalksFromGetTalks(logger, catalog.Talks.Count);

                if (logger.IsEnabled(LogLevel.Trace))
                {
                    var talkIds = catalog.Talks.Select(static talk => talk.Id).ToArray();
                    ProgramLog.ReturningTalkPayloadForGetTalks(logger, talkIds);
                }

                return Results.Ok(catalog.Talks);
            });

        return app;
    }

    public static void Main(string[] args)
    {
        BuildApp(args).Run();
    }
}
#pragma warning restore CA1052, CA1515