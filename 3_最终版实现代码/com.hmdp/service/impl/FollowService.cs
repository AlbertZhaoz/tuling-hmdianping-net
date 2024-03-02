using System.Collections;
using AutoMapper;
using com.hmdp.attribute;
using com.hmdp.Const;
using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.repo;
using com.hmdp.utils;
using StackExchange.Redis;

namespace com.hmdp.service.impl;

public class FollowService:BaseService<tb_follow>,IFollowService
{
    [AutoWire] public IDatabase redisDb { get; set; }
    [AutoWire] public IUserService userService { get; set; }
    [AutoWire] public IMapper mapper { get; set; }
    
    public async Task<Result> Follow(ulong followUserId, bool isFollow)
    {
        // 1.获取登录用户
        var userId = UserHolder.GetUser().id;
        var key = RedisConst.FOLLOWS_KEY + userId;
        // 1.判断到底是关注还是取关
        try
        {
            if (isFollow) 
            {
                // 2.关注，新增数据
                var follow = new tb_follow();
                follow.userId = userId;
                follow.followUserId = followUserId;
                follow.create_time = DateTime.Now;
                var count = await base.AddAndIgnoreColumns(follow, x => x.id);
            
                if (count > 0) {
                    // 把关注用户的id，放入redis的set集合 sadd userId followerUserId
                    await redisDb.SetAddAsync(key, followUserId.ToString());
                }
            } 
            else 
            {
                // 3.取关，删除 delete from tb_follow where user_id = ? and follow_user_id = ?
                var isSuccess = await base.DeleteNoEntity(x => x.userId == userId && x.followUserId == followUserId);
               
                if (isSuccess) {
                    // 把关注用户的id从Redis集合中移除
                    await redisDb.SetRemoveAsync(key, followUserId.ToString());
                }
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> IsFollow(ulong followUserId)
    {
        // 1.获取登录用户
        var userId = UserHolder.GetUser().id;
        // 2.查询是否关注 select count(*) from tb_follow where user_id = ? and follow_user_id = ?
        var count = await base.Count(x => x.userId == userId && x.followUserId == followUserId);
        // 3.判断
        return Result.Success(count > 0);
    }

    public async Task<Result> FollowCommons(long id)
    {
        // 1.获取当前用户
        var userId = UserHolder.GetUser().id;
        string key = RedisConst.FOLLOWS_KEY+ userId;
        // 2.求交集
        string key2 = RedisConst.FOLLOWS_KEY + id;
        var intersect = await redisDb.SetCombineAsync(SetOperation.Intersect, key, key2);
        
        if (intersect == null || intersect.Length == 0) {
            // 无交集
            return Result.Success(Enumerable.Empty<tb_user_dto>());
        }
        // 3.解析id集合并查询用户
        var users = await userService.QueryByIDs(intersect.Select(x => (object)x).ToArray());
        var userDtos = mapper.Map<List<tb_user_dto>>(users);
        return Result.Success(userDtos);
    }
}