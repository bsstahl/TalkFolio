namespace TalkFolio.Data.YamlFile;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Raw YAML projection for a talk record.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by YamlDotNet reflection deserialization.")]
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
