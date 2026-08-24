namespace TalkFolio.Tests;

using NSubstitute;
using TalkFolio.Entities;
using TalkFolio.Interfaces;
using TalkFolio.Services;

public sealed class TalkCatalogService_LoadAsync_Should
{
    [Fact]
    public async Task ReturnRepositoryCatalog_WhenLoading()
    {
        var repository = Substitute.For<ITalkCatalogRepository>();
        var expected = new TalkCatalog([]);
        repository.LoadAsync(Arg.Any<CancellationToken>()).Returns(expected);
        var target = new TalkCatalogService(repository);

        var actual = await target.LoadAsync(CancellationToken.None);

        Assert.Same(expected, actual);
    }
}
