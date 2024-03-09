using Microsoft.Extensions.Options;

namespace com.hmdp.utils;

public class AppSetting
{
    public static IConfiguration Configuration { get; private set; }
    
    public static void Init(IServiceCollection services,IConfiguration configuration)
    {
        Configuration = configuration;
        var provider = services.BuildServiceProvider();
        RedisConfig = provider.GetService<IOptions<RedisConfig>>().Value;
        SqlConfig = provider.GetService<IOptions<SqlSugarConfig>>().Value;
    }
    public static RedisConfig RedisConfig { get; set; }
    public static SqlSugarConfig SqlConfig { get; set; }
    
    public static string GetSettingString(string key)
    {
        return Configuration[key];
    }
    // 多个节点,通过.GetSection("key")["key1"]获取
    public static IConfigurationSection GetSection(string key)
    {
        return Configuration.GetSection(key);
    }
}

public record RedisConfig
{
    public bool Enable { get; set; }
    public string ConnectStr { get; set; }
}

public record SqlSugarConfig
{
    public string ConnectionString { get; set; }
    public string DbType { get; set; }
    public bool IsAutoCloseConnection { get; set; }
}