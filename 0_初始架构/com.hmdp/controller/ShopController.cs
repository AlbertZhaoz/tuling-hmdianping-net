using com.hmdp.attribute;
using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.service;
using Microsoft.AspNetCore.Mvc;

namespace com.hmdp.controller;

[ApiController]
[Route("[controller]/[action]")]
public class ShopController:BaseController
{
    [AutoWire]
    public IShopService ShopService { get; set; }
    
    /// <summary>
    /// 根据id查询商铺信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<Result> QueryById(long id)
    {
        var result = await ShopService.QueryById(id);
        return result;
    }
    
    /// <summary>
    /// 新增商铺信息
    /// </summary>
    /// <param name="shop"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<Result> SaveShop(tb_shop shop)
    {
        var result = await ShopService.Add(shop);
        return Result.Success(result);
    }
    
   /// <summary>
   /// 更新商铺信息
   /// </summary>
   /// <param name="shop"></param>
   /// <returns></returns>
    [HttpPut]
    public async Task<Result> UpdateShop(tb_shop shop)
    {
        var result = await ShopService.Update(shop);
        return Result.Success(result);
    }
}