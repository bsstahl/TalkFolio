namespace TalkFolio.Data.YamlFile;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Raw projection of proposal copy item data.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by YamlDotNet reflection deserialization.")]
internal sealed class ProposalCopyItem
{
    public string? Type { get; set; }

    public string? Copy { get; set; }
}
