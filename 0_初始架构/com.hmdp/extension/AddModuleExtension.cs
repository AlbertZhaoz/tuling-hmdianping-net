using AngleSharp.Text;
using com.hmdp.utils;
using SqlSugar.IOC;
using StackExchange.Redis;

namespace com.hmdp.extension;

public static class AddModuleExtension
{
    public static void AddModule(this IServiceCollection service, IConfiguration configuration)
    {
        if(string.IsNullOrEmpty(AppSetting.SqlConfig.ConnectionString))
        {
            return;
        }
        else
        {
            SugarIocServices.AddSqlSugar(new IocConfig()
            {
                ConnectionString = AppSetting.SqlConfig.ConnectionString,
                DbType = AppSetting.SqlConfig.DbType.ToEnum<IocDbType>(IocDbType.MySql),
                IsAutoCloseConnection = AppSetting.SqlConfig.IsAutoCloseConnection,
            }); 
        }
        
        
        if(string.IsNullOrEmpty(AppSetting.RedisConfig.ConnectStr))
        {
            return;
        }
        else
        {
            service.AddSingleton<IDatabase>( c =>
            {
                var redis = ConnectionMultiplexer
                    .ConnectAsync(AppSetting.RedisConfig.ConnectStr).Result;

                return redis.GetDatabase();
            });
        }
    }
}