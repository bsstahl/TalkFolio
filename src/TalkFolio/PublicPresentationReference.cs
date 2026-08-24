namespace TalkFolio;

/// <summary>
/// Represents a public presentation reference for a talk.
/// </summary>
/// <param name="Source">The public source or platform.</param>
/// <param name="Url">The public URL to the presentation.</param>
/// <param name="PublicId">The public identifier used by the source.</param>
#pragma warning disable CA1054, CA1056
public sealed record PublicPresentationReference(string Source, string? Url, string? PublicId);
#pragma warning restore CA1054, CA1056
