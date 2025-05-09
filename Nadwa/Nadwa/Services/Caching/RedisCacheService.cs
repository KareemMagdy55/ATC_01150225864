using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Nadwa.Services.Caching;

public class RedisCacheService(IDistributedCache? cache) : IRedisCacheService {
    public T? GetData<T>(string key) {
        var data = cache?.GetString(key);

        return data is null ? default : JsonSerializer.Deserialize<T>(data);
    }

    public void SetData<T>(string key, T data) {
        var options = new DistributedCacheEntryOptions() {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };
        cache?.SetString(key, JsonSerializer.Serialize(data), options);
    }
}