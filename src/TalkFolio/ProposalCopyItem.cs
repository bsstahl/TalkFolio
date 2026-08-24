namespace TalkFolio;

/// <summary>
/// Represents typed proposal copy attached to a talk.
/// </summary>
/// <param name="Type">The type of the proposal copy item.</param>
/// <param name="Copy">The proposal copy contents.</param>
public sealed record ProposalCopyItem(string Type, string Copy);
