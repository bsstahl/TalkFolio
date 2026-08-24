namespace TalkFolio;

/// <summary>
/// Represents a talk record that did not supply a required identifier.
/// </summary>
public sealed class MissingTalkIdException : TalkCatalogLoadException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingTalkIdException"/> class.
    /// </summary>
    public MissingTalkIdException()
        : this("Talk file is missing a required Id value.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingTalkIdException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public MissingTalkIdException(string message)
        : this(message, new InvalidOperationException(message))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingTalkIdException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public MissingTalkIdException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates a missing talk identifier exception for a specific file path.
    /// </summary>
    /// <param name="filePath">The talk file path that is missing an identifier.</param>
    /// <returns>The created exception.</returns>
    public static MissingTalkIdException ForFilePath(string filePath)
        => new($"Talk file '{filePath}' is missing a required Id value.")
    {
        FilePath = filePath,
    };

    /// <summary>
    /// Gets the talk file path that is missing the required identifier.
    /// </summary>
    public string FilePath { get; private set; } = string.Empty;
}
