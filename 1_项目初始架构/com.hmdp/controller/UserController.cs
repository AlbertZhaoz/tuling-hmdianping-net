using com.hmdp.dto;
using com.hmdp.utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace com.hmdp.controller;

[ApiController]
[Route("[controller]/[action]")]
public class UserController:BaseController
{
    [HttpGet("GetUserHolder")]
    public async Task<Result> GetUserHolder()
    {
        return Result.Success(AppSetting.RedisConfig.ConnectStr);
    }
}