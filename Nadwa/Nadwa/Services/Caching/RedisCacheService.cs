using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Nadwa.Services.Caching;

public class RedisCacheService(IDistributedCache? cache) : IRedisCacheService {
    public T? GetData<T>(string key)
    {
        try
        {
            var task = Task.Run(() => cache?.GetString(key));
            if (task.Wait(TimeSpan.FromMilliseconds(300)))
            {
                var data = task.Result;
                Console.WriteLine("Redis GetData Success");
                return data is null ? default : JsonSerializer.Deserialize<T>(data);
            }
            else
            {
                Console.WriteLine($"Redis GetData timeout");
                return default;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Redis failed");
            return default;
        }
    }

    public void SetData<T>(string key, T data)
    {
        try
        {
            var jsonData = JsonSerializer.Serialize(data);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5),
                AbsoluteExpiration = DateTimeOffset.Now.AddMilliseconds(5000),
            };

            var task = Task.Run(() => cache?.SetString(key, jsonData, options));
            if (!task.Wait(TimeSpan.FromMilliseconds(250)))
            {
                Console.WriteLine($"Redis SetData timeout");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Redis failed");
        }
    }


}