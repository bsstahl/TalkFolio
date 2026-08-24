namespace TalkFolio.Data.YamlFile;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Raw YAML projection of presentation family data.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by YamlDotNet reflection deserialization.")]
internal sealed class YamlPresentationFamilyReference
{
    public string? Name { get; set; }

    public string? Variant { get; set; }
}
