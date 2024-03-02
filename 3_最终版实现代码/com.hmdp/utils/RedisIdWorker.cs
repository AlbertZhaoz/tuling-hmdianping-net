using com.hmdp.dto;
using StackExchange.Redis;

namespace com.hmdp.utils;

public class RedisIdWorker
{
    private const long BEGIN_TIMESTAMP = 1709178108L;
    
    private const int COUNT_BITS = 32;
    
    private IDatabase _redisDb;

    public RedisIdWorker(IDatabase redisDb)
    {
        _redisDb = redisDb;
    }

    public long NextId(string keyPrefix)
    {
        // 1.生成时间戳
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - BEGIN_TIMESTAMP;
        
        // 2.生成序列号
        // 2.1.获取当前日期，精确到天
        var date = DateTime.Now.ToString("yyyy:MM:dd");
        // 2.2.自增长
        var sequence = _redisDb.StringIncrement($"icr:{keyPrefix}:{date}", 1);
        
        // 3.生成ID
        var result = (timestamp << COUNT_BITS) | sequence;

        return result;
    }
}