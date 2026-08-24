namespace TalkFolio;

/// <summary>
/// Represents a malformed YAML talk file that cannot be parsed.
/// </summary>
public sealed class MalformedTalkYamlException : TalkCatalogLoadException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MalformedTalkYamlException"/> class.
    /// </summary>
    public MalformedTalkYamlException()
        : this("Talk file content could not be parsed.", new InvalidOperationException("Talk file content could not be parsed."))
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
        => new(BuildMessage(filePath), innerException)
        {
            FilePath = filePath,
        };

    /// <summary>
    /// Gets the YAML file path that failed to parse.
    /// </summary>
    public string FilePath { get; private set; } = string.Empty;

    private static string BuildMessage(string filePath)
    {
        return $"Talk file '{filePath}' contains malformed YAML. Scalar values containing ':' must be quoted, for example: - \"Workshop Edition: TP for Teams\".";
    }
}
