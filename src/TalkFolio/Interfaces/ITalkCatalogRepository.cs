namespace TalkFolio.Interfaces;

using TalkFolio.Entities;

/// <summary>
/// Provides a repository that loads the TalkFolio catalog from a data source.
/// </summary>
public interface ITalkCatalogRepository
{
    /// <summary>
    /// Loads the canonical catalog representation from the configured data source.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the load operation.</param>
    /// <returns>The loaded catalog.</returns>
    Task<TalkCatalog> LoadAsync(CancellationToken cancellationToken = default);
}