namespace TalkFolio.Data.YamlFile.Tests;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TalkFolio.Data.YamlFile;
using NSubstitute;

public sealed class TalkCatalogRepository_LoadAsync_Should : IDisposable
{
    private readonly string _dataRoot;

    public TalkCatalogRepository_LoadAsync_Should()
    {
        _dataRoot = Path.Combine(Path.GetTempPath(), $"talkfolio-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_dataRoot);
    }

    [Fact]
    public async Task ReturnCanonicalCatalog_WhenYamlFilesExist()
    {
        // Arrange
        var repositoryRoot = await CreateRepositoryRoot();
        var target = new TalkCatalogRepository(
            Options.Create(new TalkCatalogOptions
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
        Assert.Equal("The Great Cornholio Speaker Kit", talk.PresentationFamily!.Name);
        Assert.Equal("Canonical", talk.PresentationFamily.Variant);

        var publicPresentation = Assert.Single(talk.PublicPresentationReferences);
        Assert.Equal("SlideFed", publicPresentation.Source);
        Assert.Equal("https://example.com/presentation/great-cornholio-tp", publicPresentation.Url);
        Assert.Equal("great-cornholio-tp", publicPresentation.PublicId);
    }

    [Fact]
    public async Task ThrowInvalidOperationException_WhenDataRootIsNotConfigured()
    {
        var target = new TalkCatalogRepository(Options.Create(new TalkCatalogOptions()));

        var actual = await Assert.ThrowsAsync<InvalidOperationException>(
            () => target.LoadAsync(CancellationToken.None));

        Assert.Equal("The repository data root is not configured.", actual.Message);
    }

    [Fact]
    public async Task ReturnEmptyCatalog_WhenTalksDirectoryIsMissing()
    {
        var repositoryRoot = Path.Combine(_dataRoot, "no-talks-directory");
        Directory.CreateDirectory(repositoryRoot);
        var target = new TalkCatalogRepository(
            Options.Create(new TalkCatalogOptions
            {
                DataRoot = repositoryRoot,
            }));

        var actual = await target.LoadAsync(CancellationToken.None);

        Assert.Empty(actual.Talks);
    }

    [Fact]
    public async Task ThrowDirectoryNotFoundException_WhenDataRootDoesNotExist()
    {
        var repositoryRoot = Path.Combine(_dataRoot, "missing-root");
        var target = new TalkCatalogRepository(
            Options.Create(new TalkCatalogOptions
            {
                DataRoot = repositoryRoot,
            }));

        var actual = await Assert.ThrowsAsync<DirectoryNotFoundException>(
            () => target.LoadAsync(CancellationToken.None));

        Assert.Contains(repositoryRoot, actual.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ThrowDuplicateTalkIdException_WhenLaterFilesResolveToSameId()
    {
        // Arrange
        var repositoryRoot = Path.Combine(Path.GetTempPath(), $"talkfolio-duplicate-ids-{Guid.NewGuid():N}");
        Directory.CreateDirectory(repositoryRoot);
        var talksDirectory = Directory.CreateDirectory(Path.Combine(repositoryRoot, "talks"));
        var duplicateId = Guid.Parse("8f9eb83f-05c4-4e30-8e98-f00976f01ca0");
        var firstFilePath = Path.Combine(talksDirectory.FullName, "a-first-talk.yaml");
        var duplicateFilePath = Path.Combine(talksDirectory.FullName, "b-second-talk.yaml");

        await File.WriteAllTextAsync(
            firstFilePath,
            $$"""
            Id: {{duplicateId}}
            Title: First Talk
            Category: Leadership & Community
            Tags:
              - first
            PresentationFamily:
              Name: Duplicate Family
              Variant: Canonical
            LifecycleStatus: Active
            """);

        await File.WriteAllTextAsync(
            duplicateFilePath,
            $$"""
            Id: {{duplicateId}}
            Title: Second Talk
            Category: Leadership & Community
            Tags:
              - second
            PresentationFamily:
              Name: Duplicate Family
              Variant: Workshop
            LifecycleStatus: Active
            """);

        try
        {
            var logger = Substitute.For<ILogger<TalkCatalogRepository>>();
            var target = new TalkCatalogRepository(
                Options.Create(new TalkCatalogOptions
                {
                    DataRoot = repositoryRoot,
                }),
                logger);

            // Act
            var actual = await Assert.ThrowsAsync<DuplicateTalkIdException>(
                () => target.LoadAsync(CancellationToken.None));

            // Assert
            Assert.Equal(duplicateId, actual.TalkId);
            Assert.Equal(firstFilePath, actual.FirstFilePath);
            Assert.Equal(duplicateFilePath, actual.DuplicateFilePath);
        }
        finally
        {
            if (Directory.Exists(repositoryRoot))
            {
                Directory.Delete(repositoryRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ThrowDuplicateTalkTitleVariantException_WhenLaterFilesResolveToSameTitleAndVariant()
    {
        // Arrange
        var repositoryRoot = Path.Combine(Path.GetTempPath(), $"talkfolio-duplicate-title-variant-{Guid.NewGuid():N}");
        Directory.CreateDirectory(repositoryRoot);
        var talksDirectory = Directory.CreateDirectory(Path.Combine(repositoryRoot, "talks"));
        var firstFilePath = Path.Combine(talksDirectory.FullName, "a-first-talk.yaml");
        var duplicateFilePath = Path.Combine(talksDirectory.FullName, "b-second-talk.yaml");

        await File.WriteAllTextAsync(
            firstFilePath,
            """
            Id: 11111111-1111-1111-1111-111111111111
            Title: LLMs Under the Hood
            Category: Leadership & Community
            Tags:
              - first
            PresentationFamily:
              Name: LLMs Family
              Variant: Workshop
            LifecycleStatus: Active
            """);

        await File.WriteAllTextAsync(
            duplicateFilePath,
            """
            Id: 22222222-2222-2222-2222-222222222222
            Title: LLMs Under the Hood
            Category: Leadership & Community
            Tags:
              - second
            PresentationFamily:
              Name: LLMs Family
              Variant: Workshop
            LifecycleStatus: Active
            """);

        try
        {
            var target = new TalkCatalogRepository(
                Options.Create(new TalkCatalogOptions
                {
                    DataRoot = repositoryRoot,
                }));

            // Act
            var actual = await Assert.ThrowsAsync<DuplicateTalkTitleVariantException>(
                () => target.LoadAsync(CancellationToken.None));

            // Assert
            Assert.Equal("LLMs Under the Hood", actual.Title);
            Assert.Equal("Workshop", actual.Variant);
            Assert.Equal(firstFilePath, actual.FirstFilePath);
            Assert.Equal(duplicateFilePath, actual.DuplicateFilePath);
        }
        finally
        {
            if (Directory.Exists(repositoryRoot))
            {
                Directory.Delete(repositoryRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ThrowMalformedTalkYamlException_WhenTalkRecordCannotBeDeserialized()
    {
        // Arrange
        var repositoryRoot = Path.Combine(Path.GetTempPath(), $"talkfolio-malformed-yaml-{Guid.NewGuid():N}");
        Directory.CreateDirectory(repositoryRoot);
        var talksDirectory = Directory.CreateDirectory(Path.Combine(repositoryRoot, "talks"));
        var malformedFilePath = Path.Combine(talksDirectory.FullName, "broken-talk.yaml");

        await File.WriteAllTextAsync(
            malformedFilePath,
            """
            Id: 87654321-4321-4321-4321-cba987654321
            Title: Broken Talk
            AlternateTitles:
              - Workshop Edition: TP for Teams
            Category: Leadership & Community
            Tags:
              - broken
            PresentationFamily:
              Name: Broken Family
              Variant: Canonical
            LifecycleStatus: Active
            """);

        try
        {
            var target = new TalkCatalogRepository(
                Options.Create(new TalkCatalogOptions
                {
                    DataRoot = repositoryRoot,
                }));

            // Act
            var actual = await Assert.ThrowsAsync<MalformedTalkYamlException>(
                () => target.LoadAsync(CancellationToken.None));

            // Assert
            Assert.Equal(malformedFilePath, actual.FilePath);
            Assert.Contains("must be quoted", actual.Message, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(repositoryRoot))
            {
                Directory.Delete(repositoryRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task ThrowMissingTalkIdException_WhenTalkRecordDoesNotSupplyId()
    {
        // Arrange
        var repositoryRoot = Path.Combine(Path.GetTempPath(), $"talkfolio-missing-id-{Guid.NewGuid():N}");
        Directory.CreateDirectory(repositoryRoot);
        var talksDirectory = Directory.CreateDirectory(Path.Combine(repositoryRoot, "talks"));
        var talkFilePath = Path.Combine(talksDirectory.FullName, "missing-id.yaml");

        await File.WriteAllTextAsync(
            talkFilePath,
            """
            Title: Missing Identifier Talk
            Category: Leadership & Community
            Tags:
              - missing-id
            PresentationFamily:
              Name: Missing Identifier Family
              Variant: Canonical
            LifecycleStatus: Active
            """);

        try
        {
            var target = new TalkCatalogRepository(
                Options.Create(new TalkCatalogOptions
                {
                    DataRoot = repositoryRoot,
                }));

            // Act
            var actual = await Assert.ThrowsAsync<MissingTalkIdException>(
                () => target.LoadAsync(CancellationToken.None));

            // Assert
            Assert.Equal(talkFilePath, actual.FilePath);
        }
        finally
        {
            if (Directory.Exists(repositoryRoot))
            {
                Directory.Delete(repositoryRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task EmitBoundaryLogs_WhenLoadingCatalog()
    {
        // Arrange
        var repositoryRoot = await CreateRepositoryRoot();
        var logger = new CollectingLogger<TalkCatalogRepository>();
        var target = new TalkCatalogRepository(
            Options.Create(new TalkCatalogOptions
            {
                DataRoot = repositoryRoot,
            }),
            logger);

        // Act
        _ = await target.LoadAsync(CancellationToken.None);

        // Assert
        var levels = logger.Levels;

        Assert.Contains(LogLevel.Information, levels);
        Assert.Contains(LogLevel.Trace, levels);
        Assert.DoesNotContain(LogLevel.Warning, levels);
        Assert.DoesNotContain(LogLevel.Error, levels);
        Assert.True(levels.Count(level => level == LogLevel.Information) >= 2);
        Assert.True(levels.Count(level => level == LogLevel.Trace) >= 3);
    }

    private sealed class CollectingLogger<T> : ILogger<T>
    {
        public List<LogLevel> Levels { get; } = [];

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Levels.Add(logLevel);
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();

        public void Dispose()
        {
        }
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
