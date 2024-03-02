using com.hmdp.utils;

namespace com.hmdp.extension;

public static class AppSettingExtension
{
    public static void AddAppSettingSetup(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions()
            .Configure<RedisConfig>(redis=> configuration.GetSection("AppSetting:RedisConfig").Bind(redis))
            .Configure<SqlSugarConfig>(sql=> configuration.GetSection("AppSetting:SqlSugarConfig").Bind(sql));
    }
}