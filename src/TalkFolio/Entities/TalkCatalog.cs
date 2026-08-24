namespace TalkFolio.Entities;

/// <summary>
/// Represents the complete TalkFolio catalog returned by the repository.
/// </summary>
/// <param name="Talks">The talks managed by the catalog.</param>
public sealed record TalkCatalog(IReadOnlyList<Talk> Talks);