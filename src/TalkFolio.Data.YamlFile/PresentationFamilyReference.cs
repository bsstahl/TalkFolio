namespace TalkFolio.Data.YamlFile;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Raw projection of presentation family data.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by YamlDotNet reflection deserialization.")]
internal sealed class PresentationFamilyReference
{
    public string? Name { get; set; }

    public string? Variant { get; set; }
}
