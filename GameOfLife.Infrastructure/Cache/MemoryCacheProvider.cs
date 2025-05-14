using GameOfLife.Business.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace GameOfLife.Infrastructure.Cache;

public class MemoryCacheProvider(IMemoryCache cache) : ICacheProvider
{
    public TResult? Get<TResult>(string key)
    {
        cache.TryGetValue(key, out TResult? value);
        return value;
    }

    public void Set<TResult>(string key, TResult value)
    {
        cache.Set(key, value);
    }
}