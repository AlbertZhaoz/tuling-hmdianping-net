using com.hmdp.attribute;
using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.service;
using com.hmdp.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.hmdp.controller;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class BlogController:BaseController
{
    [AutoWire]
    public IBlogService blogService { get; set; }
    
    [HttpPost]
    public async Task<Result> SaveBlog(tb_blog blog)
    {
        return await blogService.SaveBlog(blog);
    }

    [HttpPut("like/{id}")]
    public async Task<Result> LikeBlog(long id)
    {
        return await blogService.LikeBlog(id);
    }

    [HttpGet("of/me")]
    public async Task<Result> QueryMyBlog(int current = 1)
    {
        // 1. 获取登录用户
        var user = UserHolder.GetUser();
        // 2. 根据用户id查询博客
        var result = await blogService.QueryPage(x=>x.userId == user.id, current, 10);

        return Result.Success(result.data);
    }
    
    [HttpGet("hot")]
    public async Task<Result> QueryHotBlog(int current = 1)
    {
        return await blogService.QueryHotBlog(current);
    }
    
    [HttpGet("{id}")]
    public async Task<Result> QueryBlogById(long id)
    {
        return await blogService.QueryBlogById(id);
    }
    
    [HttpGet("likes/{id}")]
    public async Task<Result> QueryBlogLikes(long id)
    {
        return await blogService.QueryBlogLikes(id);
    }
    
    [HttpGet("of/user")]
    public async Task<Result> QueryBlogByUserId(ulong id, int current = 1)
    {
        var result = await blogService.QueryPage(x=>x.userId == id, current, 10);

        return Result.Success(result);
    }
    
    [HttpGet("of/follow")]
    public async Task<Result> QueryBlogOfFollow([FromQuery(Name = "lastId")]long max, int offset = 0)
    {
        return await blogService.QueryBlogOfFollow(max, offset);
    }
}