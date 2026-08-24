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
