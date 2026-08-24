namespace TalkFolio;

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
