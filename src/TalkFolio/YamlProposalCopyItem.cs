namespace TalkFolio;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Raw YAML projection of proposal copy item data.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by YamlDotNet reflection deserialization.")]
internal sealed class YamlProposalCopyItem
{
    public string? Type { get; set; }

    public string? Copy { get; set; }
}
