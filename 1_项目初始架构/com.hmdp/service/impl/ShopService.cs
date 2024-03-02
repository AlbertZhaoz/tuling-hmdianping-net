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

    public Task<Result>  Update(tb_shop shop)
    {
        return null;
    }

    public Task<Result>  QueryShopByType(int typeId, int current, double x, double y)
    {
        return null;
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