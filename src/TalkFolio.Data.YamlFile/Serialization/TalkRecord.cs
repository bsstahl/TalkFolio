namespace TalkFolio.Data.YamlFile.Serialization;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Raw projection for a talk record.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by YamlDotNet reflection deserialization.")]
internal sealed class TalkRecord
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public List<string>? AlternateTitles { get; set; }

    public string? Category { get; set; }

    public List<string>? Tags { get; set; }

    public PresentationFamilyReference? PresentationFamily { get; set; }

    public string? LifecycleStatus { get; set; }

    public List<string>? TargetAudience { get; set; }

    public Dictionary<string, bool>? Flags { get; set; }

    public List<Guid>? SlideDeckIds { get; set; }

    public List<ProposalCopyItem>? ProposalCopyItems { get; set; }

    public List<PublicPresentationReference>? PublicPresentationReferences { get; set; }

    public List<RelatedContentItem>? RelatedContent { get; set; }

    public string? IdeationNotes { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
