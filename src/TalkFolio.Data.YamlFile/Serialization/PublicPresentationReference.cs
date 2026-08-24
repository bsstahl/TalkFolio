namespace TalkFolio.Data.YamlFile.Serialization;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Raw projection of a public presentation reference.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by YamlDotNet reflection deserialization.")]
internal sealed class PublicPresentationReference
{
    public string? Source { get; set; }

    public string? Url { get; set; }

    public string? PublicId { get; set; }
}
