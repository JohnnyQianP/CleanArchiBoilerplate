using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace CleanArchi.Boilerplate.Infrastructure.Db;

public class SqlSugarRedisService : ICacheService
{
    public void Add<V>(string key, V value)
    {
        throw new NotImplementedException();
    }

    public void Add<V>(string key, V value, int cacheDurationInSeconds)
    {
        throw new NotImplementedException();
    }

    public bool ContainsKey<V>(string key)
    {
        throw new NotImplementedException();
    }

    public V Get<V>(string key)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> GetAllKey<V>()
    {
        throw new NotImplementedException();
    }

    public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
    {
        throw new NotImplementedException();
    }

    public void Remove<V>(string key)
    {
        throw new NotImplementedException();
    }
}
