namespace TalkFolio.Data.YamlFile;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Raw projection of related companion content.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by YamlDotNet reflection deserialization.")]
internal sealed class RelatedContentItem
{
    public string? Type { get; set; }

    public string? Title { get; set; }

    public string? Url { get; set; }

    public string? Notes { get; set; }
}
