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
        : base("Talk catalog load failed.")
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
public sealed class MalformedTalkYamlException : TalkCatalogLoadException
{
    private MalformedTalkYamlException(string filePath, Exception innerException, bool unused)
        : base(
            $"Talk file '{filePath}' contains malformed YAML. Scalar values containing ':' must be quoted, for example: - \"Workshop Edition: TP for Teams\".",
            innerException)
    {
        FilePath = filePath;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MalformedTalkYamlException"/> class.
    /// </summary>
    public MalformedTalkYamlException()
        : this(string.Empty, new InvalidOperationException("Talk file content could not be parsed."))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MalformedTalkYamlException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public MalformedTalkYamlException(string message)
        : this(message, new InvalidOperationException(message))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MalformedTalkYamlException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The parser exception.</param>
    public MalformedTalkYamlException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates a malformed YAML exception for a specific file path.
    /// </summary>
    /// <param name="filePath">The YAML file path that failed to parse.</param>
    /// <param name="innerException">The parser exception.</param>
    /// <returns>The created exception.</returns>
    public static MalformedTalkYamlException ForFilePath(string filePath, Exception innerException)
        => new(filePath, innerException, true);

    /// <summary>
    /// Gets the YAML file path that failed to parse.
    /// </summary>
    public string FilePath { get; } = string.Empty;
}

/// <summary>
/// Represents a duplicate talk identifier found during catalog load.
/// </summary>
public sealed class DuplicateTalkIdException : TalkCatalogLoadException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateTalkIdException"/> class.
    /// </summary>
    public DuplicateTalkIdException()
        : this(Guid.Empty, string.Empty, string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateTalkIdException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public DuplicateTalkIdException(string message)
        : this(message, new InvalidOperationException(message))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateTalkIdException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public DuplicateTalkIdException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateTalkIdException"/> class.
    /// </summary>
    /// <param name="talkId">The duplicate talk identifier.</param>
    /// <param name="firstFilePath">The first file where the identifier was found.</param>
    /// <param name="duplicateFilePath">The file that introduced the duplicate identifier.</param>
    public DuplicateTalkIdException(Guid talkId, string firstFilePath, string duplicateFilePath)
        : base(
            $"Talk ID '{talkId}' is defined more than once. First file: '{firstFilePath}'. Duplicate file: '{duplicateFilePath}'.")
    {
        TalkId = talkId;
        FirstFilePath = firstFilePath;
        DuplicateFilePath = duplicateFilePath;
    }

    /// <summary>
    /// Gets the duplicate talk identifier.
    /// </summary>
    public Guid TalkId { get; } = Guid.Empty;

    /// <summary>
    /// Gets the first file where this identifier was observed.
    /// </summary>
    public string FirstFilePath { get; } = string.Empty;

    /// <summary>
    /// Gets the file path that introduced the duplicate identifier.
    /// </summary>
    public string DuplicateFilePath { get; } = string.Empty;
}

/// <summary>
/// Represents a duplicate talk title and presentation-family variant combination.
/// </summary>
public sealed class DuplicateTalkTitleVariantException : TalkCatalogLoadException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateTalkTitleVariantException"/> class.
    /// </summary>
    public DuplicateTalkTitleVariantException()
        : this(string.Empty, string.Empty, string.Empty, string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateTalkTitleVariantException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public DuplicateTalkTitleVariantException(string message)
        : this(message, new InvalidOperationException(message))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateTalkTitleVariantException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public DuplicateTalkTitleVariantException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateTalkTitleVariantException"/> class.
    /// </summary>
    /// <param name="title">The duplicate talk title.</param>
    /// <param name="variant">The duplicate presentation-family variant.</param>
    /// <param name="firstFilePath">The first file where the key was found.</param>
    /// <param name="duplicateFilePath">The file that introduced the duplicate key.</param>
    public DuplicateTalkTitleVariantException(string title, string variant, string firstFilePath, string duplicateFilePath)
        : base(
            $"Talk title '{title}' with PresentationFamily.Variant '{variant}' is defined more than once. First file: '{firstFilePath}'. Duplicate file: '{duplicateFilePath}'.")
    {
        Title = title;
        Variant = variant;
        FirstFilePath = firstFilePath;
        DuplicateFilePath = duplicateFilePath;
    }

    /// <summary>
    /// Gets the duplicate talk title.
    /// </summary>
    public string Title { get; } = string.Empty;

    /// <summary>
    /// Gets the duplicate presentation-family variant.
    /// </summary>
    public string Variant { get; } = string.Empty;

    /// <summary>
    /// Gets the first file where this key was observed.
    /// </summary>
    public string FirstFilePath { get; } = string.Empty;

    /// <summary>
    /// Gets the file path that introduced the duplicate key.
    /// </summary>
    public string DuplicateFilePath { get; } = string.Empty;
}
