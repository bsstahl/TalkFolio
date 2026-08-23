namespace TalkFolio.Tests;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

public sealed class FileSystemTalkCatalogRepository_LoadAsync_Should : IDisposable
{
    private readonly string _dataRoot;

    public FileSystemTalkCatalogRepository_LoadAsync_Should()
    {
        _dataRoot = Path.Combine(Path.GetTempPath(), $"talkfolio-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_dataRoot);
    }

    [Fact]
    public async Task ReturnCanonicalCatalog_WhenYamlFilesExist()
    {
        // Arrange
        var repositoryRoot = CreateRepositoryRoot();
        var target = new FileSystemTalkCatalogRepository(
            Options.Create(new TalkCatalogRepositoryOptions
            {
                DataRoot = repositoryRoot,
            }));

        // Act
        var actual = await target.LoadAsync(CancellationToken.None);

        // Assert
        var talk = Assert.Single(actual.Talks);
        Assert.Equal(Guid.Parse("6c8d4d27-9cc7-4c41-9bf8-19e55758e7cc"), talk.Id);
        Assert.Equal("Finding TP for Your People's Bungholes", talk.Title);
        Assert.Equal(["My People Need TP"], talk.AlternateTitles);
        Assert.Equal("Leadership & Community", talk.Category);
        Assert.Equal(["tp", "bungholes"], talk.Tags);
        Assert.Equal("Active", talk.LifecycleStatus);
        Assert.Equal(["conference-organizers", "technical-leaders"], talk.TargetAudience);
        Assert.Equal(Guid.Parse("70b739b6-8dcb-43da-adc8-7392abf9a6ef"), Assert.Single(talk.SlideDeckIds));

        var proposalCopy = Assert.Single(talk.ProposalCopyItems);
        Assert.Equal("Abstract", proposalCopy.Type);
        Assert.Equal("A practical guide to securing TP for bungholes in high-pressure conference environments.", proposalCopy.Copy);

        var relatedContent = Assert.Single(talk.RelatedContent);
        Assert.Equal("BlogPost", relatedContent.Type);
        Assert.Equal("I Am the Great TalkFolio", relatedContent.Title);
        Assert.Equal("https://example.com/blog/i-am-the-great-talkfolio", relatedContent.Url);
        Assert.Equal("Companion article for Cornholio's TP sourcing strategy.", relatedContent.Notes);

        Assert.NotNull(talk.PresentationFamily);
        Assert.Equal(Guid.Parse("8ccdf8b8-fd2c-4d41-9fe0-32fade0f41dc"), talk.PresentationFamily!.Id);
        Assert.Equal("Canonical", talk.PresentationFamily.Variant);

        var publicPresentation = Assert.Single(talk.PublicPresentationReferences);
        Assert.Equal("SlideFed", publicPresentation.Source);
        Assert.Equal("https://example.com/presentation/great-cornholio-tp", publicPresentation.Url);
        Assert.Equal("great-cornholio-tp", publicPresentation.PublicId);

        var family = Assert.Single(actual.PresentationFamilies);
        Assert.Equal(Guid.Parse("8ccdf8b8-fd2c-4d41-9fe0-32fade0f41dc"), family.Id);
        Assert.Equal("The Great Cornholio Speaker Kit", family.Name);
        Assert.Equal("Canonical Cornholio speaking family.", family.Notes);
    }

    [Fact]
    public async Task EmitBoundaryLogs_WhenLoadingCatalog()
    {
        // Arrange
        var repositoryRoot = CreateRepositoryRoot();
        var logger = Substitute.For<ILogger<FileSystemTalkCatalogRepository>>();
        var target = new FileSystemTalkCatalogRepository(
            Options.Create(new TalkCatalogRepositoryOptions
            {
                DataRoot = repositoryRoot,
            }),
            logger);

        // Act
        _ = await target.LoadAsync(CancellationToken.None);

        // Assert
        var calls = logger.ReceivedCalls()
            .Select(static call => call.GetArguments())
            .ToList();

        Assert.Contains(
            calls,
            static arguments => arguments[0] is LogLevel level
                && level == LogLevel.Information
                && arguments[2]?.ToString()?.Contains("Loading TalkFolio catalog.", StringComparison.Ordinal) == true);
        Assert.Contains(
            calls,
            static arguments => arguments[0] is LogLevel level
                && level == LogLevel.Information
                && arguments[2]?.ToString()?.Contains("Loading talks from", StringComparison.Ordinal) == true);
        Assert.Contains(
            calls,
            static arguments => arguments[0] is LogLevel level
                && level == LogLevel.Information
                && arguments[2]?.ToString()?.Contains("Loaded TalkFolio catalog with", StringComparison.Ordinal) == true);
        Assert.Contains(
            calls,
            static arguments => arguments[0] is LogLevel level
                && level == LogLevel.Trace
                && arguments[2]?.ToString()?.Contains("Deserialized talk payload", StringComparison.Ordinal) == true
                && arguments[2]?.ToString()?.Contains("Finding TP for Your People's Bungholes", StringComparison.Ordinal) == true);
        Assert.Contains(
            calls,
            static arguments => arguments[0] is LogLevel level
                && level == LogLevel.Trace
                && arguments[2]?.ToString()?.Contains("Deserialized presentation family payload", StringComparison.Ordinal) == true);
    }

    public void Dispose()
    {
        if (Directory.Exists(_dataRoot))
        {
            Directory.Delete(_dataRoot, recursive: true);
        }
    }

    private string CreateRepositoryRoot()
    {
        var repositoryRoot = Path.Combine(_dataRoot, "catalog");
        var presentationFamiliesDirectory = Directory.CreateDirectory(Path.Combine(repositoryRoot, "presentation-families"));
        var talksDirectory = Directory.CreateDirectory(Path.Combine(repositoryRoot, "talks"));

        File.WriteAllText(
            Path.Combine(presentationFamiliesDirectory.FullName, "great-cornholio-speaker-kit.yaml"),
            """
            Id: 8ccdf8b8-fd2c-4d41-9fe0-32fade0f41dc
            Name: The Great Cornholio Speaker Kit
            Notes: |-
              Canonical Cornholio speaking family.
            """);

        File.WriteAllText(
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
              Id: 8ccdf8b8-fd2c-4d41-9fe0-32fade0f41dc
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
