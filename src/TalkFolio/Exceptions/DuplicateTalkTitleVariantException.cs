namespace TalkFolio;

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
