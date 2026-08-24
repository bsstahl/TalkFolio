namespace TalkFolio.Entities;

/// <summary>
/// Represents a canonical Talk in the TalkFolio read model.
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
public sealed record Talk(
    Guid Id,
    string Title,
    IReadOnlyList<string> AlternateTitles,
    string Category,
    IReadOnlyList<string> Tags,
    string LifecycleStatus,
    IReadOnlyList<string> TargetAudience,
    PresentationFamily? PresentationFamily,
    IReadOnlyList<Guid> SlideDeckIds,
    IReadOnlyList<ProposalCopyItem> ProposalCopyItems,
    IReadOnlyList<PublicPresentationReference> PublicPresentationReferences,
    IReadOnlyList<RelatedContentItem> RelatedContent,
    IReadOnlyDictionary<string, bool>? Flags,
    string? IdeationNotes,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt);