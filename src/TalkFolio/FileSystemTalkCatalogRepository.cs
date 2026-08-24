namespace TalkFolio;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

/// <summary>
/// Reads the TalkFolio catalog from a file-based YAML data source.
/// </summary>
public sealed class FileSystemTalkCatalogRepository(
    IOptions<TalkCatalogRepositoryOptions> options,
    ILogger<FileSystemTalkCatalogRepository>? logger = null) : ITalkCatalogRepository
{
    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .IgnoreUnmatchedProperties()
        .Build();
    private readonly ILogger<FileSystemTalkCatalogRepository> _logger = logger ?? NullLogger<FileSystemTalkCatalogRepository>.Instance;
    private readonly IOptions<TalkCatalogRepositoryOptions> _options = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc/>
    public async Task<TalkCatalog> LoadAsync(CancellationToken cancellationToken = default)
    {
        FileSystemTalkCatalogRepositoryLog.LoadingCatalog(_logger);

        var dataRoot = _options.Value.DataRoot;
        if (string.IsNullOrWhiteSpace(dataRoot))
        {
            FileSystemTalkCatalogRepositoryLog.LoadingCatalogFailedBecauseDataRootNotConfigured(_logger);
            throw new InvalidOperationException("The repository data root is not configured.");
        }

        if (!Directory.Exists(dataRoot))
        {
            FileSystemTalkCatalogRepositoryLog.LoadingCatalogFailedBecauseDataRootDoesNotExist(_logger, dataRoot);
            throw new DirectoryNotFoundException($"The TalkFolio data root '{dataRoot}' does not exist.");
        }

        var talksDirectory = Path.Combine(dataRoot, "talks");

        FileSystemTalkCatalogRepositoryLog.LoadingTalksFrom(_logger, talksDirectory);
        var talks = await LoadTalksAsync(talksDirectory, cancellationToken).ConfigureAwait(false);
        FileSystemTalkCatalogRepositoryLog.LoadedCatalog(_logger, talks.Count);

        return new TalkCatalog(talks);
    }

    private async Task<IReadOnlyList<TalkRecord>> LoadTalksAsync(string talksDirectory, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(talksDirectory))
        {
            FileSystemTalkCatalogRepositoryLog.TalksDirectoryDoesNotExist(_logger, talksDirectory);
            return [];
        }

        var talks = new List<TalkRecord>();
        var seenTalkIds = new Dictionary<Guid, string>();
        var seenTitleVariants = new Dictionary<TalkTitleVariantKey, string>();
        var files = Directory.EnumerateFiles(talksDirectory, "*.*", SearchOption.TopDirectoryOnly)
            .Where(static file => file.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".yml", StringComparison.OrdinalIgnoreCase))
            .OrderBy(static file => file, StringComparer.OrdinalIgnoreCase);

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FileSystemTalkCatalogRepositoryLog.ReadingTalkFile(_logger, file);
            var yaml = await File.ReadAllTextAsync(file, cancellationToken).ConfigureAwait(false);
            var payload = DeserializeTalk(yaml, file);

            FileSystemTalkCatalogRepositoryLog.DeserializedTalkPayload(_logger, payload.Id, payload.Title, file);

            if (seenTalkIds.TryGetValue(payload.Id, out var firstTalkIdFilePath))
            {
                var duplicateIdException = new DuplicateTalkIdException(payload.Id, firstTalkIdFilePath, file);
                FileSystemTalkCatalogRepositoryLog.DuplicateTalkId(
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
                FileSystemTalkCatalogRepositoryLog.DuplicateTalkTitleVariant(
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
            FileSystemTalkCatalogRepositoryLog.MappedTalkRecord(_logger, payload.Id, file);
        }

        return talks.AsReadOnly();
    }

    private static TalkRecord MapTalk(YamlTalkRecord source)
    {
        return new TalkRecord(
            Id: source.Id,
            Title: source.Title,
            AlternateTitles: source.AlternateTitles ?? [],
            Category: source.Category ?? string.Empty,
            Tags: source.Tags ?? [],
            LifecycleStatus: source.LifecycleStatus ?? string.Empty,
            TargetAudience: source.TargetAudience ?? [],
            PresentationFamily: source.PresentationFamily is null ? null : new PresentationFamilyReference(
                source.PresentationFamily.Name ?? string.Empty,
                source.PresentationFamily.Variant ?? string.Empty),
            SlideDeckIds: source.SlideDeckIds ?? [],
            ProposalCopyItems: source.ProposalCopyItems is null
                ? []
                : source.ProposalCopyItems
                    .Select(static item => new ProposalCopyItem(item.Type ?? string.Empty, item.Copy ?? string.Empty))
                    .ToList()
                    .AsReadOnly(),
            PublicPresentationReferences: source.PublicPresentationReferences is null
                ? []
                : source.PublicPresentationReferences
                    .Select(static item => new PublicPresentationReference(
                        item.Source ?? string.Empty,
                        item.Url,
                        item.PublicId))
                    .ToList()
                    .AsReadOnly(),
            RelatedContent: source.RelatedContent is null
                ? []
                : source.RelatedContent
                    .Select(static item => new RelatedContentItem(
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

    private YamlTalkRecord DeserializeTalk(string yaml, string filePath)
    {
        try
        {
            return Deserializer.Deserialize<YamlTalkRecord>(yaml)
                ?? throw new InvalidOperationException($"Talk YAML file '{filePath}' did not produce a talk record.");
        }
        catch (YamlException ex)
        {
            var malformedTalkYamlException = MalformedTalkYamlException.ForFilePath(filePath, ex);
            FileSystemTalkCatalogRepositoryLog.TalkFileMalformed(_logger, malformedTalkYamlException, filePath);
            throw malformedTalkYamlException;
        }
        catch (InvalidOperationException ex)
        {
            var malformedTalkYamlException = MalformedTalkYamlException.ForFilePath(filePath, ex);
            FileSystemTalkCatalogRepositoryLog.TalkFileCouldNotBeDeserialized(_logger, malformedTalkYamlException, filePath);
            throw malformedTalkYamlException;
        }
    }

    private readonly record struct TalkTitleVariantKey(string Title, string Variant);
}
