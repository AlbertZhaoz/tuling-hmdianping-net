using AutoMapper;
using com.hmdp.Const;
using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Redis;

namespace com.hmdp.config;

public class RefreshTokenFilterAttribute:Attribute,IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // 1.1获取依赖注入的服务
        var _redis = context.HttpContext.RequestServices.GetService(typeof(IDatabase)) as IDatabase;
        var _mapper = context.HttpContext.RequestServices.GetService(typeof(IMapper)) as IMapper; 
        // 1.2获取请求头中的token
        var token = context.HttpContext.Request.Headers.Authorization;
        
        if (string.IsNullOrEmpty(token))
        {
            return;
        }
        
        // 2.基于TOKEN获取redis中的用户信息
        var key  = RedisConst.LOGIN_USER_KEY + token;
        var user = _redis.HashGetAll(key).ConvertFromRedis<tb_user>();
        
        // 3.判断用户是否存在
        if (user == null)
        {
            return;
        }
        
        // 4.将查询到的hash数据转为UserDTO
        var userDTO = _mapper.Map<tb_user_dto>(user);
        // 6.存在，保存用户信息到 AsyncLocal 中
        UserHolder.SaveUser(userDTO);
        // 7.刷新token有效期
        _redis.KeyExpire(key, TimeSpan.FromMinutes(RedisConst.LOGIN_USER_TTL));
    }
}