namespace TalkFolio;

/// <summary>
/// Represents lightweight companion material related to a talk.
/// </summary>
/// <param name="Type">The type of companion material.</param>
/// <param name="Title">The title of the companion material.</param>
/// <param name="Url">An optional URL to the companion material.</param>
/// <param name="Notes">Optional notes about the companion material.</param>
#pragma warning disable CA1054, CA1056
public sealed record RelatedContentItem(string Type, string Title, string? Url, string? Notes);
#pragma warning restore CA1054, CA1056
