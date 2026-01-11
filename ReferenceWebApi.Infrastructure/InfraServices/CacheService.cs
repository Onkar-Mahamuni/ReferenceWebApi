using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ReferenceWebApi.Application.Interfaces;

namespace ReferenceWebApi.Infrastructure.InfraServices
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;
        private readonly HashSet<string> _cacheKeys = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_cache.TryGetValue(key, out T? value))
                {
                    _logger.LogDebug("Cache hit for key: {CacheKey}", key);
                    return value;
                }

                _logger.LogDebug("Cache miss for key: {CacheKey}", key);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cache for key: {CacheKey}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration, CancellationToken cancellationToken)
        {
            try
            {
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };

                cacheOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    _cacheKeys.Remove(key.ToString()!);
                    _logger.LogDebug("Cache evicted for key: {CacheKey}, Reason: {Reason}", key, reason);
                });

                _cache.Set(key, value, cacheOptions);

                await _semaphore.WaitAsync(cancellationToken);
                try
                {
                    _cacheKeys.Add(key);
                }
                finally
                {
                    _semaphore.Release();
                }

                _logger.LogDebug("Cache set for key: {CacheKey}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {CacheKey}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                _cache.Remove(key);

                await _semaphore.WaitAsync(cancellationToken);
                try
                {
                    _cacheKeys.Remove(key);
                }
                finally
                {
                    _semaphore.Release();
                }

                _logger.LogDebug("Cache removed for key: {CacheKey}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key: {CacheKey}", key);
            }
        }

        public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphore.WaitAsync(cancellationToken);
                try
                {
                    var keysToRemove = _cacheKeys.Where(k => k.StartsWith(prefix)).ToList();

                    foreach (var key in keysToRemove)
                    {
                        _cache.Remove(key);
                        _cacheKeys.Remove(key);
                    }

                    _logger.LogDebug("Cache removed for prefix: {Prefix}, Count: {Count}", prefix, keysToRemove.Count);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache by prefix: {Prefix}", prefix);
            }
        }
    }
}
