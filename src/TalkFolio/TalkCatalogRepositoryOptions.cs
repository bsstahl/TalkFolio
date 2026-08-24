namespace TalkFolio;

/// <summary>
/// Configures the file-backed repository used to read TalkFolio data.
/// </summary>
public sealed class TalkCatalogRepositoryOptions
{
    /// <summary>
    /// Gets or sets the root directory that contains the repository data.
    /// </summary>
    public string DataRoot { get; set; } = string.Empty;
}
