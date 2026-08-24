namespace TalkFolio.Services;

using TalkFolio.Entities;
using TalkFolio.Interfaces;

/// <summary>
/// Provides catalog operations for use within the TalkFolio domain.
/// </summary>
public sealed class TalkCatalogService(ITalkCatalogRepository repository)
{
    /// <summary>
    /// Loads the canonical TalkFolio catalog.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the load operation.</param>
    /// <returns>The loaded catalog.</returns>
    public Task<TalkCatalog> LoadAsync(CancellationToken cancellationToken = default)
        => repository.LoadAsync(cancellationToken);
}