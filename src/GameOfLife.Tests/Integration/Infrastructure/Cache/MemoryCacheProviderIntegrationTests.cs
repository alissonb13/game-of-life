using GameOfLife.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace GameOfLife.Tests.Integration.Infrastructure.Cache;

public class MemoryCacheProviderIntegrationTests
{
    private readonly MemoryCacheProvider _cacheProvider;

    public MemoryCacheProviderIntegrationTests()
    {
        IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
        _cacheProvider = new MemoryCacheProvider(memoryCache);
    }

    [Fact]
    public void Set_ShouldStoreAndRetrieveValueSuccessfully()
    {
        const string key = "testKey";
        const string expectedValue = "testValue";

        _cacheProvider.Set(key, expectedValue);
        var result = _cacheProvider.Get<string>(key);

        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Get_ShouldReturnNull_WhenKeyDoesNotExist()
    {
        const string key = "nonExistingKey";

        var result = _cacheProvider.Get<string>(key);

        Assert.Null(result);
    }

    [Fact]
    public void Set_ShouldOverwriteExistingValue()
    {
        const string key = "testKey";
        const string firstValue = "firstValue";
        const string secondValue = "secondValue";

        _cacheProvider.Set(key, firstValue);
        var firstResult = _cacheProvider.Get<string>(key);

        _cacheProvider.Set(key, secondValue);
        var secondResult = _cacheProvider.Get<string>(key);

        Assert.Equal(firstValue, firstResult);
        Assert.Equal(secondValue, secondResult);
    }
}