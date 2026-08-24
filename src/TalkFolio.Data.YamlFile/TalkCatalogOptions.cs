namespace TalkFolio.Data.YamlFile;

/// <summary>
/// Configures the repository used to read TalkFolio data.
/// </summary>
public sealed class TalkCatalogOptions
{
    /// <summary>
    /// Gets or sets the root directory that contains the repository data.
    /// </summary>
    public string DataRoot { get; set; } = string.Empty;
}
