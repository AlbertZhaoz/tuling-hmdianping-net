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