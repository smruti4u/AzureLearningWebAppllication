using System;
using System.Threading.Tasks;

namespace CacheService.Redis
{
    public interface ICacheService
    {
        Task<T> GetData<T>(string key);
        Task SetData<T>(string key, T t, TimeSpan timeSpan);
    }
}
