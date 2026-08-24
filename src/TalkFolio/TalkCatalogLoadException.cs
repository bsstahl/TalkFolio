namespace TalkFolio;

/// <summary>
/// Represents a catalog load failure in the TalkFolio domain.
/// </summary>
public abstract class TalkCatalogLoadException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TalkCatalogLoadException"/> class.
    /// </summary>
    protected TalkCatalogLoadException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TalkCatalogLoadException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    protected TalkCatalogLoadException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TalkCatalogLoadException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    protected TalkCatalogLoadException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Represents a malformed YAML talk file that cannot be parsed.
/// </summary>
/// <param name="filePath">The YAML file path that failed to parse.</param>
/// <param name="innerException">The parser exception.</param>
public sealed class MalformedTalkYamlException(string filePath, Exception innerException)
    : TalkCatalogLoadException(
        $"Talk file '{filePath}' contains malformed YAML. Scalar values containing ':' must be quoted, for example: - \"Workshop Edition: TP for Teams\".",
        innerException)
{
    /// <summary>
    /// Gets the YAML file path that failed to parse.
    /// </summary>
    public string FilePath { get; } = filePath;
}

/// <summary>
/// Represents a duplicate talk identifier found during catalog load.
/// </summary>
/// <param name="talkId">The duplicate talk identifier.</param>
/// <param name="firstFilePath">The first file where the identifier was found.</param>
/// <param name="duplicateFilePath">The file that introduced the duplicate identifier.</param>
public sealed class DuplicateTalkIdException(Guid talkId, string firstFilePath, string duplicateFilePath)
    : TalkCatalogLoadException(
        $"Talk ID '{talkId}' is defined more than once. First file: '{firstFilePath}'. Duplicate file: '{duplicateFilePath}'.")
{
    /// <summary>
    /// Gets the duplicate talk identifier.
    /// </summary>
    public Guid TalkId { get; } = talkId;

    /// <summary>
    /// Gets the first file where this identifier was observed.
    /// </summary>
    public string FirstFilePath { get; } = firstFilePath;

    /// <summary>
    /// Gets the file path that introduced the duplicate identifier.
    /// </summary>
    public string DuplicateFilePath { get; } = duplicateFilePath;
}

/// <summary>
/// Represents a duplicate talk title and presentation-family variant combination.
/// </summary>
/// <param name="title">The duplicate talk title.</param>
/// <param name="variant">The duplicate presentation-family variant.</param>
/// <param name="firstFilePath">The first file where the key was found.</param>
/// <param name="duplicateFilePath">The file that introduced the duplicate key.</param>
public sealed class DuplicateTalkTitleVariantException(string title, string variant, string firstFilePath, string duplicateFilePath)
    : TalkCatalogLoadException(
        $"Talk title '{title}' with PresentationFamily.Variant '{variant}' is defined more than once. First file: '{firstFilePath}'. Duplicate file: '{duplicateFilePath}'.")
{
    /// <summary>
    /// Gets the duplicate talk title.
    /// </summary>
    public string Title { get; } = title;

    /// <summary>
    /// Gets the duplicate presentation-family variant.
    /// </summary>
    public string Variant { get; } = variant;

    /// <summary>
    /// Gets the first file where this key was observed.
    /// </summary>
    public string FirstFilePath { get; } = firstFilePath;

    /// <summary>
    /// Gets the file path that introduced the duplicate key.
    /// </summary>
    public string DuplicateFilePath { get; } = duplicateFilePath;
}
