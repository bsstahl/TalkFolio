namespace TalkFolio.Entities;

/// <summary>
/// Represents the presentation family for a talk within the canonical model.
/// </summary>
/// <param name="Name">The stable family name the talk belongs to.</param>
/// <param name="Variant">The talk's variant within the family.</param>
public sealed record PresentationFamily(string Name, string Variant);
