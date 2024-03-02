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