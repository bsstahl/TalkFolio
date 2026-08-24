namespace TalkFolio.Data.YamlFile;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using DomainPresentationFamily = TalkFolio.Entities.PresentationFamily;
using DomainProposalCopyItem = TalkFolio.Entities.ProposalCopyItem;
using DomainPublicPresentationReference = TalkFolio.Entities.PublicPresentationReference;
using DomainRelatedContentItem = TalkFolio.Entities.RelatedContentItem;
using DomainTalk = TalkFolio.Entities.Talk;
using DomainTalkCatalog = TalkFolio.Entities.TalkCatalog;
using TalkFolio.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

/// <summary>
/// Reads the TalkFolio catalog from a file-based data source.
/// </summary>
public sealed class TalkCatalogRepository(
    IOptions<TalkCatalogOptions> options,
    ILogger<TalkCatalogRepository>? logger = null) : ITalkCatalogRepository
{
    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .IgnoreUnmatchedProperties()
        .Build();
    private readonly ILogger<TalkCatalogRepository> _logger = logger ?? NullLogger<TalkCatalogRepository>.Instance;
    private readonly IOptions<TalkCatalogOptions> _options = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc/>
    public async Task<DomainTalkCatalog> LoadAsync(CancellationToken cancellationToken = default)
    {
        TalkCatalogRepositoryLog.LoadingCatalog(_logger);

        var dataRoot = _options.Value.DataRoot;
        if (string.IsNullOrWhiteSpace(dataRoot))
        {
            TalkCatalogRepositoryLog.LoadingCatalogFailedBecauseDataRootNotConfigured(_logger);
            throw new InvalidOperationException("The repository data root is not configured.");
        }

        if (!Directory.Exists(dataRoot))
        {
            TalkCatalogRepositoryLog.LoadingCatalogFailedBecauseDataRootDoesNotExist(_logger, dataRoot);
            throw new DirectoryNotFoundException($"The TalkFolio data root '{dataRoot}' does not exist.");
        }

        var talksDirectory = Path.Combine(dataRoot, "talks");

        TalkCatalogRepositoryLog.LoadingTalksFrom(_logger, talksDirectory);
        var talks = await LoadTalksAsync(talksDirectory, cancellationToken).ConfigureAwait(false);
        TalkCatalogRepositoryLog.LoadedCatalog(_logger, talks.Count);

        return new DomainTalkCatalog(talks);
    }

    private async Task<IReadOnlyList<DomainTalk>> LoadTalksAsync(string talksDirectory, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(talksDirectory))
        {
            TalkCatalogRepositoryLog.TalksDirectoryDoesNotExist(_logger, talksDirectory);
            return [];
        }

        var talks = new List<DomainTalk>();
        var seenTalkIds = new Dictionary<Guid, string>();
        var seenTitleVariants = new Dictionary<TalkTitleVariantKey, string>();
        var files = Directory.EnumerateFiles(talksDirectory, "*.*", SearchOption.TopDirectoryOnly)
            .Where(static file => file.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".yml", StringComparison.OrdinalIgnoreCase))
            .OrderBy(static file => file, StringComparer.OrdinalIgnoreCase);

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            TalkCatalogRepositoryLog.ReadingTalkFile(_logger, file);
            var yaml = await File.ReadAllTextAsync(file, cancellationToken).ConfigureAwait(false);
            var payload = DeserializeTalk(yaml, file);

            TalkCatalogRepositoryLog.DeserializedTalkPayload(_logger, payload.Id, payload.Title, file);

            if (seenTalkIds.TryGetValue(payload.Id, out var firstTalkIdFilePath))
            {
                var duplicateIdException = new DuplicateTalkIdException(payload.Id, firstTalkIdFilePath, file);
                TalkCatalogRepositoryLog.DuplicateTalkId(
                    _logger,
                    duplicateIdException,
                    payload.Id,
                    file,
                    firstTalkIdFilePath);
                throw duplicateIdException;
            }

            var variant = payload.PresentationFamily?.Variant ?? string.Empty;
            var titleVariantKey = new TalkTitleVariantKey(payload.Title, variant);
            if (seenTitleVariants.TryGetValue(titleVariantKey, out var firstTitleVariantFilePath))
            {
                var duplicateTitleVariantException = new DuplicateTalkTitleVariantException(
                    payload.Title,
                    variant,
                    firstTitleVariantFilePath,
                    file);
                TalkCatalogRepositoryLog.DuplicateTalkTitleVariant(
                    _logger,
                    duplicateTitleVariantException,
                    payload.Title,
                    variant,
                    file,
                    firstTitleVariantFilePath);
                throw duplicateTitleVariantException;
            }

            seenTalkIds.Add(payload.Id, file);
            seenTitleVariants.Add(titleVariantKey, file);
            talks.Add(MapTalk(payload));
            TalkCatalogRepositoryLog.MappedTalk(_logger, payload.Id, file);
        }

        return talks.AsReadOnly();
    }

    private static DomainTalk MapTalk(TalkRecord source)
    {
        return new DomainTalk(
            Id: source.Id,
            Title: source.Title,
            AlternateTitles: source.AlternateTitles ?? [],
            Category: source.Category ?? string.Empty,
            Tags: source.Tags ?? [],
            LifecycleStatus: source.LifecycleStatus ?? string.Empty,
            TargetAudience: source.TargetAudience ?? [],
            PresentationFamily: source.PresentationFamily is null ? null : new DomainPresentationFamily(
                source.PresentationFamily.Name ?? string.Empty,
                source.PresentationFamily.Variant ?? string.Empty),
            SlideDeckIds: source.SlideDeckIds ?? [],
            ProposalCopyItems: source.ProposalCopyItems is null
                ? []
                : source.ProposalCopyItems
                    .Select(static item => new DomainProposalCopyItem(item.Type ?? string.Empty, item.Copy ?? string.Empty))
                    .ToList()
                    .AsReadOnly(),
            PublicPresentationReferences: source.PublicPresentationReferences is null
                ? []
                : source.PublicPresentationReferences
                    .Select(static item => new DomainPublicPresentationReference(
                        item.Source ?? string.Empty,
                        item.Url,
                        item.PublicId))
                    .ToList()
                    .AsReadOnly(),
            RelatedContent: source.RelatedContent is null
                ? []
                : source.RelatedContent
                    .Select(static item => new DomainRelatedContentItem(
                        item.Type ?? string.Empty,
                        item.Title ?? string.Empty,
                        item.Url,
                        item.Notes))
                    .ToList()
                    .AsReadOnly(),
            Flags: source.Flags is null ? null : new Dictionary<string, bool>(source.Flags, StringComparer.Ordinal),
            IdeationNotes: source.IdeationNotes,
            CreatedAt: source.CreatedAt,
            UpdatedAt: source.UpdatedAt);
    }

    private TalkRecord DeserializeTalk(string yaml, string filePath)
    {
        try
        {
            return Deserializer.Deserialize<TalkRecord>(yaml)
                ?? throw new InvalidOperationException($"Talk YAML file '{filePath}' did not produce a talk record.");
        }
        catch (YamlException ex)
        {
            var malformedTalkYamlException = MalformedTalkYamlException.ForFilePath(filePath, ex);
            TalkCatalogRepositoryLog.TalkFileMalformed(_logger, malformedTalkYamlException, filePath);
            throw malformedTalkYamlException;
        }
        catch (InvalidOperationException ex)
        {
            var malformedTalkYamlException = MalformedTalkYamlException.ForFilePath(filePath, ex);
            TalkCatalogRepositoryLog.TalkFileCouldNotBeDeserialized(_logger, malformedTalkYamlException, filePath);
            throw malformedTalkYamlException;
        }
    }

    private readonly record struct TalkTitleVariantKey(string Title, string Variant);
}