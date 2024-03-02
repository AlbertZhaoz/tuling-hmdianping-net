using com.hmdp.attribute;
using com.hmdp.dto;
using com.hmdp.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.hmdp.controller;

[ApiController]
[Route("shop-type/[action]")]
public class ShopTypeController:BaseController
{
    [AutoWire]
    public IShopTypeService shopTypeService { get; set; }
    
    [AllowAnonymous]
    [HttpGet]
    [ActionName("list")]
    public async Task<Result> QueryTypeList()
    {
        var list = (await shopTypeService.Query()).OrderBy(x => x.sort);

        return Result.Success(list);
    }
}