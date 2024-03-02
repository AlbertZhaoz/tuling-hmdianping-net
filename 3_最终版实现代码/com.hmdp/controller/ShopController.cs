using System.Linq.Expressions;
using com.hmdp.attribute;
using com.hmdp.Const;
using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.service;
using Microsoft.AspNetCore.Mvc;

namespace com.hmdp.controller;

[ApiController]
[Route("[controller]")]
public class ShopController:BaseController
{
    [AutoWire]
    public IShopService ShopService { get; set; }
    
    /// <summary>
    /// 根据id查询商铺信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
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
    [HttpPut("saveShop")]
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
    [HttpPut("updateShop")]
    public async Task<Result> UpdateShop(tb_shop shop)
    {
        var result = await ShopService.Update(shop);
        
        return Result.Success(result);
    }
   
   /// <summary>
   /// 这个接口测试之前需要运行 InitDbController 中的 InitShop 接口
   /// 将商铺信息初始化到 redis 中
   /// </summary>
   /// <param name="type"></param>
   /// <param name="current"></param>
   /// <param name="x"></param>
   /// <param name="y"></param>
   /// <returns></returns>
   [HttpGet("of/type")]
   public async Task<Result> QueryByType([FromQuery(Name = "typeId")]int type,int current,double? x = null,double? y =null)
   {
       return await ShopService.QueryShopByType(type,current,x,y);
   }
   
   [HttpGet("of/name/{name}/{current}")]
   public async Task<Result> QueryShopByName(int current,string name = "") {
       // 根据类型分页查询
       Expression<Func<tb_shop, bool>> where = a => a.name == name;
       var result = await ShopService.QueryPage(where, current, SystemConst.MAX_PAGE_SIZE);
       
       return Result.Success(result);
   }
}