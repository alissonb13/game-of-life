namespace GameOfLife.Business.Domain.Interfaces;

public interface ICacheProvider
{
    TResult? Get<TResult>(string key);
    void Set<TResult>(string key, TResult value);
}