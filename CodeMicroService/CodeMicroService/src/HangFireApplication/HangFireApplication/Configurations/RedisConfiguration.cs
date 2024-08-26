using HangFireApplication.Services;
using StackExchange.Redis;
namespace HangFireApplication.Configurations;

public static class RedisConfiguration
{
    public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfig = configuration
            .GetSection("Redis")
            .Get<RedisConfig>();

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect
            ($"{redisConfig.Host}:{redisConfig.Port}"));
        services.AddScoped(typeof(IRedisService<>), typeof(RedisService<>));
    }
}