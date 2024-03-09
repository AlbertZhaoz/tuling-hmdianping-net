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