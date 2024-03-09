using com.hmdp.attribute;
using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.service;
using Microsoft.AspNetCore.Mvc;

namespace com.hmdp.controller;

[ApiController]
[Route("voucher-order")]
public class VoucherOrderController:BaseController
{
    [AutoWire] public IVoucherOrderService voucherOrderService { get; set; }

    /// <summary>
    /// 秒杀优惠券
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("seckill/{id}")]
    public async Task<Result> SeckillVoucher(ulong id)
    {
        return await voucherOrderService.SeckillVoucher(id);
    }
}