using com.hmdp.attribute;
using com.hmdp.dto;
using com.hmdp.service;
using Microsoft.AspNetCore.Mvc;

namespace com.hmdp.controller;

[ApiController]
[Route("[controller]")]
public class FollowController:BaseController
{
    [AutoWire] 
    public IFollowService followService { get; set; }
    
    [HttpPut("{id}/{isFollow}")]
    public async Task<Result> Follow(ulong id,bool isFollow)
    {
        return await followService.Follow(id, isFollow);
    }
    
    [HttpGet("or/not/{followUserId}")]
    public async Task<Result> IsFollow(ulong followUserId)
    {
        return await followService.IsFollow(followUserId);
    }
    
    [HttpGet("common/{followUserId}")]
    public async Task<Result> FollowCommons(long followUserId)
    {
        return await followService.FollowCommons(followUserId);
    }
}