using com.hmdp.attribute;
using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.hmdp.controller;

[ApiController]
[Route("[controller]")]
public class VoucherController:BaseController
{
    [AutoWire] public IVoucherService voucherService { get; set; }

    /// <summary>
    /// 新增秒杀券
    /// </summary>
    /// <param name="voucher"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("seckill")]
    public async Task<Result> AddSeckillVoucher(tb_voucher voucher)
    {
        return await voucherService.AddSeckillVoucher(voucher);
    }
    
    /// <summary>
    /// 新增普通券
    /// </summary>
    /// <param name="voucher"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<Result> AddVoucher(tb_voucher voucher)
    {
        var id = await voucherService.AddAndIgnoreColumns(voucher, a => a.id);

        return Result.Success(id);
    }

    /// <summary>
    /// 查询店铺的优惠券列表
    /// </summary>
    /// <param name="shopId"></param>
    /// <returns></returns>
    [HttpGet("list/{shopId}")]
    public async Task<Result> QueryVoucherOfShop(ulong shopId)
    {
        return await voucherService.QueryVoucherOfShop(shopId);
    }
}