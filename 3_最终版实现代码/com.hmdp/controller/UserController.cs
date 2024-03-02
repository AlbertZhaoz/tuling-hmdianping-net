using AutoMapper;
using com.hmdp.attribute;
using com.hmdp.dto;
using com.hmdp.service;
using com.hmdp.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.hmdp.controller;

[ApiController]
[Route("[controller]")]
public class UserController:BaseController
{
    [AutoWire]
    public IUserService userService { get; set; }
    [AutoWire]
    public IUserInfoService userInfoService { get; set; }
    [AutoWire]
    public IMapper mapper { get; set; }
    
    /// <summary>
    /// 发送手机验证码
    /// </summary>
    /// <param name="phone"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("code")]
    public async Task<Result> SendCode(string phone)
    {
        return await userService.SendCode(phone);
    }
    
    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<Result> Login(login_form_dto loginForm)
    {
        return await userService.Login(loginForm);
    }
    
    [HttpGet("[action]")]
    public async Task<Result> Me()
    {
        var user = UserHolder.GetUser();
        
        return Result.Success(user);
    }

    [HttpGet("info/{userId}")]
    public async Task<Result> Info(long userId)
    {
        var info = await userInfoService.QueryById(userId);
        
        if (info == null)
        {
            // 没有详情，应该是第一次查看详情
            return Result.Success();
        }

        info.create_time = null;
        info.update_time = null;

        return Result.Success(info);
    }
    
    [HttpGet("{userId}")]
    public async Task<Result> QueryUserById(long userId)
    {
        var user = await userService.QueryById(userId);

        if (user == null)
        {
            return Result.Success();
        }

        var userDto = mapper.Map<tb_user_dto>(user);
        
        return Result.Success(userDto);
    }

    [HttpPost("[action]")]
    public async Task<Result> Sign()
    {
        return await userService.Sign();
    }
    
    [HttpPost("sign/count")]
    public async Task<Result> SignCount()
    {
        return await userService.SignCount();
    }
}