namespace TalkFolio.Tests;

using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using TalkFolio.Entities;

public sealed class TalksEndpoint_GetTalks_Should : IDisposable
{
    private readonly string _dataRoot;

    public TalksEndpoint_GetTalks_Should()
    {
        _dataRoot = Path.Combine(Path.GetTempPath(), $"talkfolio-api-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_dataRoot);
    }

    [Fact]
    public async Task ReturnCanonicalTalks_WhenRepositoryContainsTalkData()
    {
        // Arrange
        var repositoryRoot = await CreateRepositoryRoot();
        using var factoryRoot = new WebApplicationFactory<TalkFolio.Api.Program>();
        using var factory = factoryRoot.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["TalkCatalog:DataRoot"] = repositoryRoot,
                });
            });
        });
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync(new Uri("/talks", UriKind.Relative), CancellationToken.None);

        // Assert
        response.EnsureSuccessStatusCode();
        var talks = await response.Content.ReadFromJsonAsync<List<Talk>>(CancellationToken.None);
        var talk = Assert.Single(talks!);
        Assert.Equal(Guid.Parse("6c8d4d27-9cc7-4c41-9bf8-19e55758e7cc"), talk.Id);
        Assert.Equal("Finding TP for Your People's Bungholes", talk.Title);
        Assert.NotNull(talk.PresentationFamily);
        Assert.Equal("The Great Cornholio Speaker Kit", talk.PresentationFamily!.Name);
        Assert.Equal("Canonical", talk.PresentationFamily.Variant);
    }

    public void Dispose()
    {
        if (Directory.Exists(_dataRoot))
        {
            Directory.Delete(_dataRoot, recursive: true);
        }
    }

    private async Task<string> CreateRepositoryRoot()
    {
        var repositoryRoot = Path.Combine(_dataRoot, "catalog");
        var talksDirectory = Directory.CreateDirectory(Path.Combine(repositoryRoot, "talks"));

        await File.WriteAllTextAsync(
            Path.Combine(talksDirectory.FullName, "finding-tp-for-your-peoples-bungholes.yaml"),
            """
            Id: 6c8d4d27-9cc7-4c41-9bf8-19e55758e7cc
            Title: Finding TP for Your People's Bungholes
            AlternateTitles:
              - My People Need TP
            Category: Leadership & Community
            Tags:
              - tp
              - bungholes
            PresentationFamily:
              Name: The Great Cornholio Speaker Kit
              Variant: Canonical
            LifecycleStatus: Active
            ProposalCopyItems:
              - Type: Abstract
                Copy: |-
                  A practical guide to securing TP for bungholes in high-pressure conference environments.
            TargetAudience:
              - conference-organizers
              - technical-leaders
            Flags:
              HandsOn: true
            SlideDeckIds:
              - 70b739b6-8dcb-43da-adc8-7392abf9a6ef
            PublicPresentationReferences:
              - Source: SlideFed
                Url: https://example.com/presentation/great-cornholio-tp
                PublicId: great-cornholio-tp
            RelatedContent:
              - Type: BlogPost
                Title: I Am the Great TalkFolio
                Url: https://example.com/blog/i-am-the-great-talkfolio
                Notes: |-
                  Companion article for Cornholio's TP sourcing strategy.
            IdeationNotes: |-
              Consider a sequel on identifying sources of caffeine.
            CreatedAt: 2026-08-20T00:00:00Z
            UpdatedAt: 2026-08-21T00:00:00Z
            """);

        return repositoryRoot;
    }
}
