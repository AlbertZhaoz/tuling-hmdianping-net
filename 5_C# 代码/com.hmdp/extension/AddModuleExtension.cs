using AngleSharp.Text;
using com.hmdp.utils;
using SqlSugar;
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
            // 注入sqlsugar
            SugarIocServices.AddSqlSugar(new IocConfig()
            {
                ConnectionString = AppSetting.SqlConfig.ConnectionString,
                DbType = AppSetting.SqlConfig.DbType.ToEnum<IocDbType>(IocDbType.MySql),
                IsAutoCloseConnection = AppSetting.SqlConfig.IsAutoCloseConnection,
            }); 
            
            //配置参数
            SugarIocServices.ConfigurationSugar(db =>
            {
                db.Aop.OnLogExecuting = (sql, p) =>
                {
                    Console.WriteLine(UtilMethods.GetNativeSql(sql,p));
                };
                //设置更多连接参数
                //db.CurrentConnectionConfig.XXXX=XXXX
                //db.CurrentConnectionConfig.MoreSettings=new ConnMoreSettings(){}
                //二级缓存设置
                //db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices()
                //{
                // DataInfoCacheService = myCache //配置我们创建的缓存类
                //}
                //读写分离设置
                //laveConnectionConfigs = new List<SlaveConnectionConfig>(){...}
      
                /*多租户注意*/
                //单库是db.CurrentConnectionConfig 
                //多租户需要db.GetConnection(configId).CurrentConnectionConfig 
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