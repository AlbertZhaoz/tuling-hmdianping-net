using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.repo;

namespace com.hmdp.service;

public interface IVoucherOrderService:IBaseService<tb_voucher_order>
{
    Task<Result> SeckillVoucher(ulong voucherId);
}