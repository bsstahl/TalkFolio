namespace TalkFolio;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

/// <summary>
/// Provides a repository that loads the TalkFolio catalog from YAML files on disk.
/// </summary>
public interface ITalkCatalogRepository
{
    /// <summary>
    /// Loads the canonical catalog representation from the configured data source.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the load operation.</param>
    /// <returns>The loaded catalog.</returns>
    Task<TalkCatalog> LoadAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Configures the file-backed repository used to read TalkFolio data.
/// </summary>
public sealed class TalkCatalogRepositoryOptions
{
    /// <summary>
    /// Gets or sets the root directory that contains the repository data.
    /// </summary>
    public string DataRoot { get; set; } = string.Empty;
}

/// <summary>
/// Reads the TalkFolio catalog from a file-based YAML data source.
/// </summary>
public sealed class FileSystemTalkCatalogRepository(
    IOptions<TalkCatalogRepositoryOptions> options,
    ILogger<FileSystemTalkCatalogRepository>? logger = null) : ITalkCatalogRepository
{
    private readonly ILogger<FileSystemTalkCatalogRepository> _logger = logger ?? NullLogger<FileSystemTalkCatalogRepository>.Instance;
    private readonly IOptions<TalkCatalogRepositoryOptions> _options = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc/>
    public async Task<TalkCatalog> LoadAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Loading TalkFolio catalog.");
        var dataRoot = _options.Value.DataRoot;
        if (string.IsNullOrWhiteSpace(dataRoot))
        {
            _logger.LogError("TalkFolio catalog load failed because DataRoot is not configured.");
            throw new InvalidOperationException("The repository data root is not configured.");
        }

        if (!Directory.Exists(dataRoot))
        {
            _logger.LogError("TalkFolio catalog load failed because DataRoot does not exist: {DataRoot}", dataRoot);
            throw new DirectoryNotFoundException($"The TalkFolio data root '{dataRoot}' does not exist.");
        }

        var talksDirectory = Path.Combine(dataRoot, "talks");

        _logger.LogInformation("Loading talks from {Directory}.", talksDirectory);
        var talks = await LoadTalksAsync(talksDirectory, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Loaded TalkFolio catalog with {TalkCount} talks.", talks.Count);

        return new TalkCatalog(talks);
    }

    private async Task<IReadOnlyList<TalkRecord>> LoadTalksAsync(string talksDirectory, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(talksDirectory))
        {
            _logger.LogWarning("Talks directory does not exist: {Directory}", talksDirectory);
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
            _logger.LogTrace("Reading talk file {FilePath}.", file);
            var yaml = await File.ReadAllTextAsync(file, cancellationToken).ConfigureAwait(false);
            var payload = DeserializeTalk(yaml, file);

            _logger.LogTrace(
                "Deserialized talk payload {TalkId} ({TalkTitle}) from {FilePath}.",
                payload.Id,
                payload.Title,
                file);

            if (seenTalkIds.TryGetValue(payload.Id, out var firstTalkIdFilePath))
            {
                var duplicateIdException = new DuplicateTalkIdException(payload.Id, firstTalkIdFilePath, file);
                _logger.LogError(
                    duplicateIdException,
                    "Catalog load failed because duplicate TalkId {TalkId} was found in {DuplicateFilePath}. First seen in {FirstFilePath}.",
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
                _logger.LogError(
                    duplicateTitleVariantException,
                    "Catalog load failed because duplicate talk title and variant were found for Title '{Title}' and Variant '{Variant}' in {DuplicateFilePath}. First seen in {FirstFilePath}.",
                    payload.Title,
                    variant,
                    file,
                    firstTitleVariantFilePath);
                throw duplicateTitleVariantException;
            }

            seenTalkIds.Add(payload.Id, file);
            seenTitleVariants.Add(titleVariantKey, file);
            talks.Add(MapTalk(payload));
            _logger.LogTrace("Mapped talk record {TalkId} from {FilePath}.", payload.Id, file);
        }

        return talks.AsReadOnly();
    }

    private TalkRecord MapTalk(YamlTalkRecord source)
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
        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();

        try
        {
            return deserializer.Deserialize<YamlTalkRecord>(yaml)
                ?? throw new InvalidOperationException($"Talk YAML file '{filePath}' did not produce a talk record.");
        }
        catch (YamlException ex)
        {
            var malformedTalkYamlException = new MalformedTalkYamlException(filePath, ex);
            _logger.LogError(
                malformedTalkYamlException,
                "Catalog load failed because talk file {FilePath} contains malformed YAML.",
                filePath);
            throw malformedTalkYamlException;
        }
        catch (InvalidOperationException ex)
        {
            var malformedTalkYamlException = new MalformedTalkYamlException(filePath, ex);
            _logger.LogError(
                malformedTalkYamlException,
                "Catalog load failed because talk file {FilePath} could not be deserialized.",
                filePath);
            throw malformedTalkYamlException;
        }
    }

    private readonly record struct TalkTitleVariantKey(string Title, string Variant);
}

/// <summary>
/// Represents the complete TalkFolio catalog returned by the repository.
/// </summary>
/// <param name="Talks">The talks managed by the catalog.</param>
public sealed record TalkCatalog(IReadOnlyList<TalkRecord> Talks);

/// <summary>
/// Represents a canonical Talk record in the TalkFolio read model.
/// </summary>
/// <param name="Id">The unique identifier for the talk.</param>
/// <param name="Title">The title of the talk.</param>
/// <param name="AlternateTitles">Alternate titles used for the talk.</param>
/// <param name="Category">The talk category.</param>
/// <param name="Tags">The tags associated with the talk.</param>
/// <param name="LifecycleStatus">The lifecycle status of the talk.</param>
/// <param name="TargetAudience">The target audience for the talk.</param>
/// <param name="PresentationFamily">The presentation family relationship for the talk.</param>
/// <param name="SlideDeckIds">The slide deck identifiers for the talk.</param>
/// <param name="ProposalCopyItems">The typed proposal copy items for the talk.</param>
/// <param name="PublicPresentationReferences">The public presentation references for the talk.</param>
/// <param name="RelatedContent">The lightweight companion material references for the talk.</param>
/// <param name="Flags">Optional metadata flags attached to the talk.</param>
/// <param name="IdeationNotes">Optional ideation notes for the talk.</param>
/// <param name="CreatedAt">The date the talk record was created.</param>
/// <param name="UpdatedAt">The date the talk record was last updated.</param>
public sealed record TalkRecord(
    Guid Id,
    string Title,
    IReadOnlyList<string> AlternateTitles,
    string Category,
    IReadOnlyList<string> Tags,
    string LifecycleStatus,
    IReadOnlyList<string> TargetAudience,
    PresentationFamilyReference? PresentationFamily,
    IReadOnlyList<Guid> SlideDeckIds,
    IReadOnlyList<ProposalCopyItem> ProposalCopyItems,
    IReadOnlyList<PublicPresentationReference> PublicPresentationReferences,
    IReadOnlyList<RelatedContentItem> RelatedContent,
    IReadOnlyDictionary<string, bool>? Flags,
    string? IdeationNotes,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt);

/// <summary>
/// Represents the family relationship for a talk within the canonical model.
/// </summary>
/// <param name="Name">The stable family name the talk belongs to.</param>
/// <param name="Variant">The talk's variant within the family.</param>
public sealed record PresentationFamilyReference(string Name, string Variant);

/// <summary>
/// Represents typed proposal copy attached to a talk.
/// </summary>
/// <param name="Type">The type of the proposal copy item.</param>
/// <param name="Copy">The proposal copy contents.</param>
public sealed record ProposalCopyItem(string Type, string Copy);

/// <summary>
/// Represents a public presentation reference for a talk.
/// </summary>
/// <param name="Source">The public source or platform.</param>
/// <param name="Url">The public URL to the presentation.</param>
/// <param name="PublicId">The public identifier used by the source.</param>
public sealed record PublicPresentationReference(string Source, string? Url, string? PublicId);

/// <summary>
/// Represents lightweight companion material related to a talk.
/// </summary>
/// <param name="Type">The type of companion material.</param>
/// <param name="Title">The title of the companion material.</param>
/// <param name="Url">An optional URL to the companion material.</param>
/// <param name="Notes">Optional notes about the companion material.</param>
public sealed record RelatedContentItem(string Type, string Title, string? Url, string? Notes);

internal sealed class YamlTalkRecord
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public List<string>? AlternateTitles { get; set; }

    public string? Category { get; set; }

    public List<string>? Tags { get; set; }

    public YamlPresentationFamilyReference? PresentationFamily { get; set; }

    public string? LifecycleStatus { get; set; }

    public List<string>? TargetAudience { get; set; }

    public Dictionary<string, bool>? Flags { get; set; }

    public List<Guid>? SlideDeckIds { get; set; }

    public List<YamlProposalCopyItem>? ProposalCopyItems { get; set; }

    public List<YamlPublicPresentationReference>? PublicPresentationReferences { get; set; }

    public List<YamlRelatedContentItem>? RelatedContent { get; set; }

    public string? IdeationNotes { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}

internal sealed class YamlPresentationFamilyReference
{
    public string? Name { get; set; }

    public string? Variant { get; set; }
}

internal sealed class YamlProposalCopyItem
{
    public string? Type { get; set; }

    public string? Copy { get; set; }
}

internal sealed class YamlPublicPresentationReference
{
    public string? Source { get; set; }

    public string? Url { get; set; }

    public string? PublicId { get; set; }
}

internal sealed class YamlRelatedContentItem
{
    public string? Type { get; set; }

    public string? Title { get; set; }

    public string? Url { get; set; }

    public string? Notes { get; set; }
}
