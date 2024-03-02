using System.Globalization;
using com.hmdp.dto;
using com.hmdp.utils;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using StackExchange.Redis;

namespace com.hmdp.controller;

[ApiController]
[Route("[controller]/[action]")]
public class InitDbController:BaseController
{
    private readonly ISqlSugarClient _sugarClient;
    private readonly IDatabase _redisDb;
    
    public InitDbController(ISqlSugarClient sugarClient, IDatabase redisDb)
    {
        _sugarClient = sugarClient;
        _redisDb = redisDb;
    }

    [HttpGet("GenerateEntityClassFromDb")]
    public Result GenerateEntityClassFromDb()
    {
        _sugarClient.DbFirst
            .IsCreateAttribute()
            .StringNullable()
            .CreateClassFile("E:\\00_Repo\\hm-dianping-net\\Solution1\\com.hmdp\\entity"
                , "com.hmdp.entity");

        return Result.Success();
    }
    
    [HttpGet("GenerateStrToRedis")]
    public async Task<Result> GenerateStrToRedis()
    {
        var result = await _redisDb.StringSetAsync("test", "testValue");
        
        return Result.Success();
    }
}