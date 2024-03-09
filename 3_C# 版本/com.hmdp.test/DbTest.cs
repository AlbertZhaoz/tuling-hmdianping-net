using Autofac;
using com.hmdp.Const;
using com.hmdp.entity;
using com.hmdp.test.DependencyInjectionForTest;
using com.hmdp.utils;
using SqlSugar;
using SqlSugar.IOC;
using StackExchange.Redis;

namespace com.hmdp.test;

public class DbTest
{
    private DITest diMock = new DITest();
    
    private IDatabase redisDb;
    private ISqlSugarClient sqlDb;

    public DbTest()
    {
        var diCollections = diMock.DICollections();
        
        redisDb = diCollections.Resolve<IDatabase>();
        sqlDb = DbScoped.SugarScope;
    }

    [Fact]
    public async Task TestGetSystemCurrentUnixTime()
    {
        // 打印当前系统时间戳-Linux
        var unixTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        
        Console.WriteLine(unixTime);
    }
    
    [Fact]
    public async Task TestRedisFunctionIsOk()
    {
        redisDb.StringSet("test", "test");
        var result = redisDb.StringGet("test");
        
        Assert.Equal("test", result);
    }
    
    /// <summary>
    /// 用于迁移店铺数据，从数据库到redis
    /// </summary>
    [Fact]
    public async Task TestMigrationShopFromDb2Redis()
    {
        long result = 0;
        
        // 1.查询所有店铺
        var shops = sqlDb.Queryable<tb_shop>().ToList();
        // 2.把店铺分组，按照typeId分组，typeId一致的放到一个集合
        var maps = shops.GroupBy(x => x.typeId);

        foreach (var entry in maps)
        {
            var key = RedisConst.SHOP_GEO_KEY + entry.Key;
            GeoEntry[] geoEntrys = new GeoEntry[entry.Count()];
            var index = 0;
            
            foreach (var shop in entry)
            {
                var geoEntry = new GeoEntry(shop.x, shop.y, shop.id.ToString());
                geoEntrys[index] = geoEntry;
                index++;
            }
            
            result = await redisDb.GeoAddAsync(key,geoEntrys);
        }
        
        
        
        Assert.True(result > 0);
    }
    /// <summary>
    /// 用于生成实体类，此项目已经生成好了，不需要再生成
    /// </summary>
    // [Fact]
    // public async Task TestGenerateEntityClassFromDb()
    // {
    //     sqlDb.DbFirst
    //         .IsCreateAttribute()
    //         .StringNullable()
    //         .CreateClassFile("E:\\00_Repo\\hm-dianping-net\\Solution1\\com.hmdp\\entity-backup"
    //             , "com.hmdp.entity");
    // }
}