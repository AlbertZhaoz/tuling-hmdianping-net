using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.repo;

namespace com.hmdp.service;

public interface IShopService:IBaseService<tb_shop>
{
    Task<Result>  QueryById(long id);

    Task<Result>  Update(tb_shop shop);

    Task<Result>  QueryShopByType(int typeId, int current, double x, double y);
}