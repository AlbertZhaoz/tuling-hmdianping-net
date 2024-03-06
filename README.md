# hm-dianping-net
Redis 黑马点评项目 C# 版本实现。  

项目如何启动：
1. 打开【2_前端及数据库】在数据库中执行 hmdp.sql（用 MySQL 数据库）
2. 打开【2_前端及数据库】进入到 nginx-1.18.0 文件夹下用命令行执行 `start nginx.exe`
3. 安装 Redis，并在 Redis 命令行中执行`xgroup create streamOrders g1 0-0 MKSTREAM`创建消息队列，**注意如果你是 Windows 上安装 Redis，请确保 Redis 版本高于 5.0**(5.0 才支持消费者组消息队列和其他一些需要用到的高级功能)
4. 打开【3_最终实现代码】修改 appsettings.json 中数据库和 Redis 的连接字符串为个人的
5. 启动项目执行单元测试`TestMigrationShopFromDb2Redis()`将店铺数据预热到 Redis 中。
## 1 项目结构分析
此后端项目是基于 .NET8 WebApi 实现的，代码在

- [https://gitee.com/AlbertZhaoz/hm-dianping-net](https://gitee.com/AlbertZhaoz/hm-dianping-net)
- [https://github.com/AlbertZhaoz/hm-dianping-net](https://github.com/AlbertZhaoz/hm-dianping-net)
### 1.1 依赖包使用

- Autofac.Extensions.DependencyInjection：Autofac 批量依赖注入、属性注入等功能
- Autofac.Extras.DynamicProxy：Autofac 批量依赖注入、属性注入等功能
- AutoMapper：映射功能
- Mapster：配合 AutoMapper 做映射功能
- DistributedLock.Redis：各类分布式锁（此项目只用了 Redis 相关的分布式锁），同时这个包底层用了 StackExchange.Redis 客户端
- Masuit.Tools.Core：全能工具库，类似于 Java  中的 HuTool
- Microsoft.Extensions.Configuration：配置基础套件
- Microsoft.Extensions.Configuration.Json：配置 Json 基础套件
- Microsoft.Extensions.DependencyInjection：依赖注入基础套件
- Microsoft.Extensions.DependencyInjection.Abstractions：依赖注入基础套件
- SqlSugar.IOC：SqlSugar IOC 注入
- SqlSugarCore：SqlSugar ORM 框架
### 1.2 项目目录结构分析
![image.png](https://cdn.nlark.com/yuque/0/2024/png/957395/1709362314531-dcff1ced-2486-4d28-901b-ba1f2e459bc6.png#averageHue=%231d282b&clientId=u19bc1a3a-c834-4&from=paste&height=580&id=udc9bc84a&originHeight=725&originWidth=525&originalType=binary&ratio=1&rotation=0&showTitle=false&size=84682&status=done&style=none&taskId=ue373c6b8-c431-4dfb-afaf-b79f1289157&title=&width=420)
## 2 核心代码
### 2.1 Autofac 依赖注入
```csharp
// 1、配置host与容器
builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new AutofacModuleRegister());
    });
// 2. 替换默认的控制器由 Autofac 来创建
builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());


using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using com.hmdp.attribute;
using com.hmdp.bgservices;
using com.hmdp.controller;
using com.hmdp.service.impl;

namespace com.hmdp.config;

public class AutofacModuleRegister: Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 1. 批量注册所有服务层服务
        builder.RegisterAssemblyTypes(typeof(AutofacModuleRegister).Assembly)
            .Where(t => t.Name.EndsWith("Service"))
            .AsImplementedInterfaces()
            .PropertiesAutowired(new PropertySelector());
        
        // 2. 注册每一个控制器和抽象之间的关系
        var controllerBaseType = typeof(BaseController);
        builder.RegisterAssemblyTypes(typeof(AutofacModuleRegister).Assembly)
            .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
            // 支持属性注入
            .PropertiesAutowired(new PropertySelector());
        
        #region 没有接口层的服务层注入
        //因为没有接口层，所以不能实现解耦，只能用 Load 方法。
        //注意如果使用没有接口的服务，并想对其使用 AOP 拦截，就必须设置为虚方法
        //var assemblysServicesNoInterfaces = Assembly.Load("Blog.Core.Services");
        //builder.RegisterAssemblyTypes(assemblysServicesNoInterfaces);
        // // 1. 注册所有服务层服务
        // builder.RegisterGeneric(typeof(BaseService<>))
        //     .As(typeof(IBaseService<>))
        //     .InstancePerDependency(); 
        //
        // builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(AutofacModuleRegister)))
        //     .AsImplementedInterfaces()
        //     .InstancePerDependency()
        //     .PropertiesAutowired(); //允许将拦截器服务的列表分配给注册。
        // builder.RegisterType<ShopService>().As<IShopService>()
        //     .AsImplementedInterfaces()
        //     .InstancePerDependency();

        // 2. 注册 ShopService 类作为 IShopService 接口的实现
        // 获取 Service.dll 程序集服务，并注册
        // var assemblysServices = Assembly.LoadFrom(servicesDllFile);
        // builder.RegisterAssemblyTypes(assemblysServices)
        //     .AsImplementedInterfaces()
        //     .InstancePerDependency()
        //     .PropertiesAutowired()
        //     .EnableInterfaceInterceptors()       //引用Autofac.Extras.DynamicProxy;
        //     .InterceptedBy(cacheType.ToArray()); //允许将拦截器服务的列表分配给注册。
        #endregion

        #region 没有接口的单独类，启用class代理拦截
        //只能注入该类中的虚方法，且必须是public
        //这里仅仅是一个单独类无接口测试，不用过多追问
        // builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Love)))
        //     .EnableClassInterceptors()
        //     .InterceptedBy(cacheType.ToArray());
        #endregion

        #region 单独注册一个含有接口的类，启用interface代理拦截
        //不用虚方法
        //builder.RegisterType<AopService>().As<IAopService>()
        //   .AsImplementedInterfaces()
        //   .EnableInterfaceInterceptors()
        //   .InterceptedBy(typeof(BlogCacheAOP));
        #endregion
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class AutoWireAttribute:Attribute
{
    
}

public class PropertySelector:Autofac.Core.IPropertySelector
{
    public bool InjectProperty(PropertyInfo propertyInfo, object instance)
    {
        return propertyInfo.GetCustomAttribute<AutoWireAttribute>() != null;
    }
}
```
### 2.2 AutoMapper 映射
```csharp
// AutoMapper Config
builder.Services.AddutoMapperSetup();

public static class AutoMapperExtension
{
    public static void AddutoMapperSetup(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMapperConfig));
    }
}

/// <summary>
/// 静态全局 AutoMapper 配置文件
/// </summary>
public class AutoMapperConfig:Profile
{
    /// <summary>
    /// 配置构造函数，用来创建关系映射
    /// </summary>
    public AutoMapperConfig()
    {
        // 如果想要使用直接注入 IMapper.Mapper 即可
        CreateMap<tb_user, tb_user_dto>();
        // CreateMap<SysUserInfo, SysUserInfoDto>()
        // .ForMember(a => a.uID, o => o.MapFrom(d => d.Id))
        // .ForMember(a => a.RIDs, o => o.MapFrom(d => d.RIDs))
    }
}

// 调用 IMapper 注入即可
```
### 2.3 一人一单+超卖问题实现
```csharp
using Castle.Components.DictionaryAdapter.Xml;
using com.hmdp.attribute;
using com.hmdp.Const;
using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.repo;
using com.hmdp.utils;
using StackExchange.Redis;

namespace com.hmdp.service.impl;

/// <summary>
/// 这个服务有很复杂的业务逻辑，里面会涉及到分布式锁，事务，缓存，消息队列等等
/// </summary>
public class VoucherOrderService:BaseService<tb_voucher_order>,IVoucherOrderService
{
    [AutoWire] public ISeckillVoucherService seckillVoucherService { get; set; }
    [AutoWire] public IDatabase redisDb { get; set; }
    
    private string luaScript = @"
            -- 0.判断消息队列是否存在，不存在则创建
            if(not redis.call('exists', KEYS[1])) then
                redis.call('XGROUP', 'CREATE', KEYS[1], 'g1', '0-0', 'MKSTREAM')
            end

            -- 1.参数列表
            -- 1.1.优惠券id
            local voucherId = ARGV[1]
            -- 1.2.用户id
            local userId = ARGV[2]
            -- 1.3.订单id
            local orderId = ARGV[3]
            
            -- 2.数据key
            -- 2.1.库存key
            -- 'seckill:stock:'
            local stockKey = KEYS[2] .. voucherId
            -- 2.2.订单key
            -- 'seckill:order:'
            local orderKey = KEYS[3] .. voucherId
            
            -- 3.脚本业务
            -- 3.1.判断库存是否充足 get stockKey
            if(tonumber(redis.call('get', stockKey)) <= 0) then
                -- 3.2.库存不足，返回1
                return 1
            end
            -- 3.2.判断用户是否下单 SISMEMBER orderKey userId
            if(redis.call('sismember', orderKey, userId) == 1) then
                -- 3.3.存在，说明是重复下单，返回2
                return 2
            end
            -- 3.4.扣库存 incrby stockKey -1
            redis.call('incrby', stockKey, -1)
            -- 3.5.下单（保存用户）sadd orderKey userId
            redis.call('sadd', orderKey, userId)
            -- 3.6.发送消息到队列中， XADD stream.orders * k1 v1 k2 v2 ...
            redis.call('xadd', KEYS[1], '*', 'userId', userId, 'voucherId', voucherId, 'id', orderId)
            return 0
    ";
    
    public async Task<Result> SeckillVoucher(ulong voucherId)
    {
        var userId = UserHolder.GetUser().id;
        var redisIdWorker = new RedisIdWorker(redisDb);
        var orderId = redisIdWorker.NextId(RedisConst.ORDER);
        
        // 1.执行 Lua 脚本：这里面有一个消息队列，这段脚本中包含消息队列不存在则创建
        var result = await redisDb.ScriptEvaluateAsync(luaScript,
            new RedisKey[] {
                RedisConst.MESSAGE_STREAM_KEY,
                RedisConst.SECKILL_STOCK_KEY, 
                RedisConst.SECKILL_ORDER_KEY}, 
            new RedisValue[] {voucherId, userId, orderId});
        
        // 2.解析结果
        if (result.ToString() != "0")
        {
            // 2.1.不为0 ，代表没有购买资格
            return Result.Fail(result.ToString() == "1" ? "库存不足" : "不能重复下单");
        }

        return Result.Success(orderId);
    }
}
```
```csharp
using System.Linq.Expressions;
using com.hmdp.attribute;
using com.hmdp.Const;
using com.hmdp.entity;
using com.hmdp.service;
using com.hmdp.utils;
using SqlSugar.IOC;
using StackExchange.Redis;

namespace com.hmdp.bgservices;

public class SekillOrderBgService:BackgroundService
{
    public IDatabase _redisDb { get; set; }
    public IVoucherOrderService _voucherOrderService { get; set; }
    public ISeckillVoucherService _seckillVoucherService { get; set; }
    public ILogger<SekillOrderBgService> _log { get; set; }
    
    public SekillOrderBgService(IDatabase redisDb, IVoucherOrderService voucherOrderService, ISeckillVoucherService seckillVoucherService, ILogger<SekillOrderBgService> log)
    {
        _redisDb = redisDb;
        _voucherOrderService = voucherOrderService;
        _seckillVoucherService = seckillVoucherService;
        _log = log;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                // 1.获取消息队列中的订单信息 XREADGROUP GROUP g1 c1 COUNT 1 [BLOCK 2000] STREAMS MESSAGE_STREAM_KEY >
                // https://github.com/StackExchange/StackExchange.Redis/issues/1109
                var entrys = _redisDb.StreamReadGroup(
                    RedisConst.MESSAGE_STREAM_KEY, "g1", "c1", ">", 1, false);
                    
                // 2.判断订单信息是否为空
                if (entrys == null || entrys.Length == 0)
                {
                    await Task.Delay(2000);
                    continue;
                }
                    
                // 3.解析出订单 id 处理订单信息
                var entry = entrys[0];
                var order = new tb_voucher_order();
                
                if (entry.Values.Length > 2)
                {
                    order.user_id = entry.Values[0].Value.ObjToLong();
                    order.voucher_id = entry.Values[1].Value.ObjToLong();
                    order.id = entry.Values[2].Value.ObjToLong();
                    order.create_time = DateTime.Now;
                    order.update_time = DateTime.Now;
                }
                
                // 3.创建订单会用到分布式锁
                await CreateVoucherOrder(order);

                await Task.Delay(2000);
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
            }
        }
    }

    private async Task CreateVoucherOrder(tb_voucher_order order)
    {
        var userId = order.user_id;
        var voucherId = order.voucher_id;
        // 创建锁对象
        var isLock = await _redisDb.LockTakeAsync(RedisConst.LOCK_ORDER+ userId, voucherId, TimeSpan.FromSeconds(50));
        // 判断
        if (!isLock) {
            // 获取锁失败，直接返回失败或者重试
            _log.LogError("获取锁失败，直接返回失败或者重试");
            return;
        }

        try
        {
            // 5.1.查询订单
            var count = await _voucherOrderService.Count(a =>
                a.user_id == userId &&
                a.voucher_id == voucherId);

            // 5.2.判断是否存在
            if (count > 0)
            {
                // 用户已经购买过了
                _log.LogError("不允许重复下单！");
                return;
            }

            await DbScoped.SugarScope.Ado.BeginTranAsync();
            // 6.扣减库存
            var updateLine = await _seckillVoucherService.Db
                .Updateable<tb_seckill_voucher>()
                // set stock = stock - 1
                .SetColumns(x => new tb_seckill_voucher()
                {
                    stock = x.stock - 1
                })
                // where id = ? and stock > 0
                .Where(x => x.voucherId == voucherId && x.stock > 0)
                .ExecuteCommandAsync();

            if (updateLine < 1)
            {
                // 扣减失败
                _log.LogError("库存不足！");
                return;
            }

            // 7.创建订单
            await _voucherOrderService.Add(order);
            
            // 8.提交事务
            await DbScoped.SugarScope.Ado.CommitTranAsync();
        }
        catch (Exception e)
        {
            // 回滚事务
            await DbScoped.SugarScope.Ado.RollbackTranAsync();
            _log.LogError(e.Message);
        }
        finally {
            // 释放锁
            await _redisDb.LockReleaseAsync(RedisConst.LOCK_ORDER+ userId, voucherId);
        }
    }
}
```
### 2.4 缓存穿透解决方案+GEO 类型使用
```csharp
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using com.hmdp.attribute;
using com.hmdp.Const;
using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.repo;
using com.hmdp.utils;
using Masuit.Tools;
using StackExchange.Redis;

namespace com.hmdp.service.impl;

public class ShopService:BaseService<tb_shop>,IShopService
{
    [AutoWire]
    public IDatabase RedisDb { get; set; }
    
    public async Task<Result> QueryById(long id)
    {
        var result = await QueryWithPassThrough(RedisConst.CACHE_SHOP_KEY+id,id);
        
        return result;
    }

    public async Task<Result> Update(tb_shop shop)
    {
        var id = shop.id;

        if (id < 0)
        {
            return Result.Fail("id 不能小于 0");
        }
        
        // 这边要保证事务
        try
        {
            await base.Db.Ado.BeginTranAsync();
            // 1. 更新数据库
            var result = await base.Update(shop);
            // 2. 删除缓存
            RedisDb.KeyDelete(RedisConst.CACHE_SHOP_KEY+id);
            await base.Db.Ado.CommitTranAsync();
        }
        catch (Exception e)
        {
            await base.Db.Ado.RollbackTranAsync();
            return Result.Fail(e.Message);
        }

        return Result.Success();
    }

    public async Task<Result>  QueryShopByType(int typeId, int current, double? x, double? y)
    {
         // 1.判断是否需要根据坐标查询
        if (x == null || y == null) {
            // 不需要坐标查询，按数据库查询
            var page = await base.QueryPage(a => 
                a.typeId == (ulong)typeId, current, SystemConst.DEFAULT_PAGE_SIZE);
            
            return Result.Success(page.data);
        }

        // 2.计算分页参数
        int from = (current - 1) * SystemConst.DEFAULT_PAGE_SIZE;
        int end = current * SystemConst.DEFAULT_PAGE_SIZE;

        // 3.查询redis、按照距离排序、分页。结果：shopId、distance
        string key = RedisConst.SHOP_GEO_KEY + typeId;
        
        // GEOSEARCH key BYLONLAT x y BYRADIUS 10 WITHDISTANCE
        var results = RedisDb.GeoRadius(
            key, (double)x, (double)y,5000L, GeoUnit.Meters, end, 
            Order.Ascending,
            GeoRadiusOptions.WithDistance);
        
        // 4.没有下一页了
        if (results == null && results?.Length <= from){
            return Result.Success(Enumerable.Empty<tb_shop>());
        }

        // 4.1.截取 from ~ end的部分
        var list = results.Skip(from).Take(end - from).ToList();
        // 4.2.获取店铺id
        var ids = list.Select(a => ulong.Parse(a.Member)).ToList();
        // 4.3.获取距离
        var distanceMap = list.ToDictionary(a => a.Member, a => a.Distance);
        // 5.根据id查询Shop
        Expression<Func<tb_shop,bool>> where = a => ids.Contains(a.id);
        var shops = await base.Query(where);
        
        // 6.设置距离
        foreach (var shop in shops) {
            shop.distance = distanceMap[shop.id.ToString()].Value;
        }
        
        // 6.返回
        return Result.Success(shops);
    }

    /// <summary>
    /// 缓存穿透解决方案
    /// </summary>
    /// <returns></returns>
    private async Task<Result> QueryWithPassThrough(string key,long id)
    {
        // 1.从redis查询商铺缓存
        var shopCache  = RedisDb.StringGet(key);

        tb_shop? shop = null;
        
        // 2. 判断 shopCache 是否存在，存在则反序列化给前端
        if (!string.IsNullOrEmpty(shopCache))
        {
            // 如果查找到是自定义的缓存穿透字符串，说明是我们缓存的空数据
            if (shopCache == RedisConst.CACHE_NULL_VALUE)
            {
                return Result.Fail("查询空值，请检查 id");
            }
            
            // 3. 存在，序列化返回
            try
            {
                shop = JsonSerializer.Deserialize<tb_shop>(shopCache);
                return Result.Success(shop);
            }
            catch (Exception e)
            {
                return Result.Fail(e.Message);
            }
           
        }
        
        // 如果缓存不存在（连空字符串都没有），则查询数据库
        shop = await base.QueryById(id);

        if (shop == null)
        {
            //这里的常量值是 2 分钟，如果从缓存中查询不到则写 ""
            RedisDb.StringSet(key, RedisConst.CACHE_NULL_VALUE, TimeSpan.FromMinutes(2));
            return Result.Fail("店铺不存在！！");
        }
        
        // 如果存在则设置过期时间 30 分钟
        RedisDb.StringSet(key, shop.ToJsonString(), TimeSpan.FromMinutes(30));
        return Result.Success(shop);
    }
}
```
### 2.5 排行榜实现 SortedSet

```csharp
using System.Linq.Expressions;
using AutoMapper;
using com.hmdp.attribute;
using com.hmdp.Const;
using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.repo;
using com.hmdp.utils;
using Masuit.Tools;
using Microsoft.AspNetCore.Authorization;
using NetTaste;
using StackExchange.Redis;

namespace com.hmdp.service.impl;

public class BlogService:BaseService<tb_blog>,IBlogService
{
    [AutoWire]
    public IUserService userService { get; set; }
    [AutoWire]
    public IDatabase redisDb { get; set; }
    [AutoWire]
    public IFollowService followService { get; set; }
    [AutoWire]
    public IMapper mapper { get; set; }
    
    public async Task<Result> QueryHotBlog(int current)
    {
        var queryPage = await base.QueryPage(a => 
            true, current, 10, "liked desc");

        var result = queryPage.data.Select(blog =>
        {
            QueryBlogUser(blog);
            IsBlogLiked(blog);
            
            return blog;
        }).ToList();
        
        return Result.Success(result);
    }

    public async Task<Result> QueryBlogById(long id)
    {
        // 1.查询 blog
        var blog = await base.QueryById(id);
        
        if (blog == null)
        {
            return Result.Fail("博客不存在");
        }
        
        // 2.查询 blog 有关的用户
        await QueryBlogUser(blog);
        // 3.查询 blog 是否被当前登录用户点赞
        await IsBlogLiked(blog);

        return Result.Success(blog);
    }

    public async Task<Result> LikeBlog(long id)
    {
        // 1.获取登录用户
        var userId = UserHolder.GetUser().id;
        // 2.判断当前登录用户是否已经点赞
        var key = RedisConst.USER_BLOG_LIKE + id;
        var sortedSetScore = redisDb.SortedSetScore(key,userId);
        
        if (sortedSetScore == null) {
            // 3.如果未点赞，可以点赞
            // 3.1.数据库点赞数 + 1
            var blog = await base.QueryById(id);
            blog.liked += 1;
            bool isSuccess = await base.Update(blog);
            
            // 3.2.保存用户到Redis的set集合  zadd key value score
            if (isSuccess)
            {
                redisDb.SortedSetAdd(key, userId, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            }
        } else {
            // 4.如果已点赞，取消点赞
            // 4.1.数据库点赞数 -1
            var blog = await base.QueryById(id);
            blog.liked -= 1;
            bool isSuccess = await base.Update(blog);
            // 4.2.把用户从Redis的set集合移除
            if (isSuccess) {
                redisDb.SortedSetRemove(key, userId);
            }
        }

        return Result.Success();
    }

    public async Task<Result> QueryBlogLikes(long id)
    {
        string key = RedisConst.USER_BLOG_LIKE + id;
        // 1.查询top5的点赞用户 zrange key 0 4
        var top5 = redisDb.SortedSetRangeByRank(key, 0, 4);
        
        if (top5 == null && top5.IsNullOrEmpty()) {
            return Result.Success(Enumerable.Empty<string>());
        }
        // 2.解析出其中的用户id
        var ids = top5.Select(x => x.ToString()).Select(ulong.Parse).ToList();
        String idStr = String.Join(",", ids);
        // 3.根据用户id查询用户 WHERE id IN ( 5 , 1 ) ORDER BY FIELD(id, 5, 1)
        Expression<Func<tb_user, bool>> expression = a => ids.Contains(a.id);

        var userList = await userService.QueryByIDsWithExpression(expression);
        var userDtoList = userList.Select(x=>mapper.Map<tb_user_dto>(x)).ToList();
        
        // 4.返回
        return Result.Success(userDtoList);
    }

    public async Task<Result> SaveBlog(tb_blog blog)
    {
        // 1.获取登录用户
        var user = UserHolder.GetUser();
        blog.userId = user.id;
        // 2.保存探店笔记
        var count = await base.Add(blog);
        
        if(count<1){
            return Result.Fail("新增笔记失败!");
        }
        
        // 3.查询笔记作者的所有粉丝 select * from tb_follow where follow_user_id = ?
        var follows = await followService.Query(x=>x.followUserId == user.id);
        
        // 4.推送笔记id给所有粉丝
        foreach (var follow in follows)
        {
            // 4.1.获取粉丝id
            var userId = follow.userId;
            // 4.2.推送
            var key = RedisConst.FEED_KEY + userId;
            redisDb.SortedSetAdd(key, blog.id, DateTimeOffset.Now.ToUnixTimeMilliseconds());
        }
        
        // 5.返回id
        return Result.Success(blog.id);
    }

    public async Task<Result> QueryBlogOfFollow(long max, int offset)
    {
        // 1.获取当前用户
        var userId = UserHolder.GetUser().id;
        // 2.查询收件箱 ZREVRANGEBYSCORE key Max Min LIMIT offset count
        string key = RedisConst.FEED_KEY + userId;
        
        var sortedSetEntries = redisDb.SortedSetRangeByScoreWithScores(
            key, 0, max,Exclude.Start, Order.Descending,offset,2);

        // 3.非空判断
        if (sortedSetEntries.IsNullOrEmpty()) {
            
            return Result.Success();
        }
        
        // 4.解析数据：blogId、minTime（时间戳）、offset
        var ids = new List<ulong>(sortedSetEntries.Length);
        long minTime = 0; // 2
        int os = 1; // 2

        foreach (var sortedSetEntry in sortedSetEntries)
        {
            // 4.1.获取id
            ids.Add(ulong.Parse(sortedSetEntry.Element));
            // 4.2.获取分数(时间戳）
            long time = (long)sortedSetEntry.Score;
            if(time == minTime){
                os++;
            }else{
                minTime = time;
                os = 1;
            }
        }

        // 5.根据id查询blog，能够实现 ORDER BY FIELD(id," + idStr + ")" 按照给的 id 的顺序排序
        Expression<Func<tb_blog, bool>> expression = a => ids.Contains(a.id);
        var blogs = await base.QueryByIDsWithExpression(expression);

        foreach (var blog in blogs)
        {
            // 5.1.查询blog有关的用户
            await QueryBlogUser(blog);
            // 5.2.查询blog是否被点赞
            await IsBlogLiked(blog);
        }

        // 6.封装并返回
        ScrollResult r = new ScrollResult();
        r.list = blogs.Select(a => (object)a).ToList();
        r.offset = os;
        r.minTime = minTime;

        return Result.Success(r);
    }

    private async Task QueryBlogUser(tb_blog blog)
    {
        var userId = blog.userId;
        var user = await userService.QueryById(userId);
        blog.name = user.nick_name;
        blog.icon = user.icon;
    }

    private async Task IsBlogLiked(tb_blog blog)
    {
        // 1. 获取当前登录用户
        var user = UserHolder.GetUser();
        
        if (user == null)
        {
            return;
        }
        
        // 2. 判断当前登录用户是否已经点赞
        var key = RedisConst.USER_BLOG_LIKE + blog.id;
        var sortedSetScore = redisDb.SortedSetScore(key,user.id);
        blog.isLike = sortedSetScore != null;
    }
}
```
### 2.6 用户签到 BitMap 使用
```csharp
    public async Task<Result> Sign()
    {
        var (key,day) = GetSignKeyAndDay();
        // 5.签到：写入Redis SETBIT key offset 1
        RedisDb.StringSetBit(key,day-1,true);
        
        return Result.Success();
    }

    public async Task<Result> SignCount()
    {
        var (key,day) = GetSignKeyAndDay();

        return Result.Success();
    }

    private (string,int) GetSignKeyAndDay()
    {
        // 1.获取当前登录用户
        var userId = UserHolder.GetUser().id;
        // 2.获取日期
        var now = DateTime.Now.ToString(":yyyyMM");
        // 3.拼接 key
        var key  = RedisConst.USER_SIGN_KEY+ userId + now;
        // 4.获取今天是本月的第几天
        var day = DateTime.Now.Day;
        // 5.获取本月截止今天为止的所有的签到记录，返回的是一个十进制的数字 BITFIELD sign:5:202203 GET u14 0
        return (key,day);
    }
```
