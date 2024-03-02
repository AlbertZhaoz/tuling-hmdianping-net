using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.repo;

namespace com.hmdp.service;

public interface IVoucherService:IBaseService<tb_voucher>
{
    Task<Result> QueryVoucherOfShop(ulong shopId);
    Task<Result> AddSeckillVoucher(tb_voucher voucher);
}