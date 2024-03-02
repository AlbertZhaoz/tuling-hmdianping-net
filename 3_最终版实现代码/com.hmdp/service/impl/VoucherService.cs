using com.hmdp.attribute;
using com.hmdp.Const;
using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.repo;
using SqlSugar;
using SqlSugar.IOC;
using StackExchange.Redis;

namespace com.hmdp.service.impl;

public class VoucherService:BaseService<tb_voucher>,IVoucherService
{
    [AutoWire] public ISeckillVoucherService seckillVoucherService { get; set; }
    [AutoWire] public IDatabase redisDb { get; set; }
    
    public async Task<Result> QueryVoucherOfShop(ulong shopId)
    {
        // 1.查询优惠券信息
        // SELECT
        // v.`id`, v.`shop_id`, v.`title`, v.`sub_title`, v.`rules`, v.`pay_value`,
        // v.`actual_value`, v.`type`, sv.`stock` , sv.begin_time , sv.end_time
        //     FROM tb_voucher v
        // LEFT JOIN  tb_seckill_voucher sv ON v.id = sv.voucher_id
        // WHERE v.shop_id = #{shopId} AND v.status = 1
        var vouchers = await DbScoped.SugarScope.Queryable<tb_voucher, tb_seckill_voucher>((v, sv) => new object[]
            {
                JoinType.Left, v.id == (ulong)sv.voucherId
            })
            .Where((v, sv) => v.shopId == shopId && v.status == 1)
            .Select((v, sv) => new tb_voucher
            {
                id = v.id,
                shopId = v.shopId,
                title = v.title,
                subTitle = v.subTitle,
                rules = v.rules,
                payValue = v.payValue,
                actualValue = v.actualValue,
                type = v.type,
                stock = sv.stock,
                beginTime = sv.begin_time,
                endTime = sv.end_time,
                createTime = DateTime.Now,
                updateTime = DateTime.Now
            })
            .ToListAsync();
        
        // 2.返回结果
        return Result.Success(vouchers);
    }
    
    public async Task<Result> AddSeckillVoucher(tb_voucher voucher)
    {
        try
        {
            await base.Db.Ado.BeginTranAsync();
            // 1.保存优惠券
            var voucherId = await base.AddAndIgnoreColumns(voucher, a => a.id);
        
            // 2.保存秒杀优惠券
            var seckillVoucher = new tb_seckill_voucher
            {
                voucherId = voucherId, 
                begin_time = voucher.beginTime,
                end_time = voucher.endTime,
                stock = voucher.stock,
                create_time = DateTime.Now,
                update_time = DateTime.Now
            };
            await seckillVoucherService.AddNoSnow(seckillVoucher);
            
            // 3.保存秒杀库存到Redis中
            var key = RedisConst.SECKILL_STOCK_KEY + voucherId;
            await redisDb.StringSetAsync(key, voucher.stock);
            
            // 4.提交事务
            await base.Db.Ado.CommitTranAsync();

            return Result.Success(voucherId);

        }
        catch (Exception e)
        { 
            base.Db.Ado.RollbackTran();

            return Result.Fail("添加秒杀优惠券失败");
        }
    }
}