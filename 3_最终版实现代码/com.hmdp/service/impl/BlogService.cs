using System.Linq.Expressions;
using AutoMapper;
using com.hmdp.attribute;
using com.hmdp.Const;
using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.repo;
using com.hmdp.utils;
using Masuit.Tools;
using Microsoft.AspNetCore.Authorization;
using NetTaste;
using StackExchange.Redis;

namespace com.hmdp.service.impl;

public class BlogService:BaseService<tb_blog>,IBlogService
{
    [AutoWire]
    public IUserService userService { get; set; }
    [AutoWire]
    public IDatabase redisDb { get; set; }
    [AutoWire]
    public IFollowService followService { get; set; }
    [AutoWire]
    public IMapper mapper { get; set; }
    
    public async Task<Result> QueryHotBlog(int current)
    {
        var queryPage = await base.QueryPage(a => 
            true, current, 10, "liked desc");

        var result = queryPage.data.Select(blog =>
        {
            QueryBlogUser(blog);
            IsBlogLiked(blog);
            
            return blog;
        }).ToList();
        
        return Result.Success(result);
    }

    public async Task<Result> QueryBlogById(long id)
    {
        // 1.查询 blog
        var blog = await base.QueryById(id);
        
        if (blog == null)
        {
            return Result.Fail("博客不存在");
        }
        
        // 2.查询 blog 有关的用户
        await QueryBlogUser(blog);
        // 3.查询 blog 是否被当前登录用户点赞
        await IsBlogLiked(blog);

        return Result.Success(blog);
    }

    public async Task<Result> LikeBlog(long id)
    {
        // 1.获取登录用户
        var userId = UserHolder.GetUser().id;
        // 2.判断当前登录用户是否已经点赞
        var key = RedisConst.USER_BLOG_LIKE + id;
        var sortedSetScore = redisDb.SortedSetScore(key,userId);
        
        if (sortedSetScore == null) {
            // 3.如果未点赞，可以点赞
            // 3.1.数据库点赞数 + 1
            var blog = await base.QueryById(id);
            blog.liked += 1;
            bool isSuccess = await base.Update(blog);
            
            // 3.2.保存用户到Redis的set集合  zadd key value score
            if (isSuccess)
            {
                redisDb.SortedSetAdd(key, userId, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            }
        } else {
            // 4.如果已点赞，取消点赞
            // 4.1.数据库点赞数 -1
            var blog = await base.QueryById(id);
            blog.liked -= 1;
            bool isSuccess = await base.Update(blog);
            // 4.2.把用户从Redis的set集合移除
            if (isSuccess) {
                redisDb.SortedSetRemove(key, userId);
            }
        }

        return Result.Success();
    }

    public async Task<Result> QueryBlogLikes(long id)
    {
        string key = RedisConst.USER_BLOG_LIKE + id;
        // 1.查询top5的点赞用户 zrange key 0 4
        var top5 = redisDb.SortedSetRangeByRank(key, 0, 4);
        
        if (top5 == null && top5.IsNullOrEmpty()) {
            return Result.Success(Enumerable.Empty<string>());
        }
        // 2.解析出其中的用户id
        var ids = top5.Select(x => x.ToString()).Select(ulong.Parse).ToList();
        String idStr = String.Join(",", ids);
        // 3.根据用户id查询用户 WHERE id IN ( 5 , 1 ) ORDER BY FIELD(id, 5, 1)
        Expression<Func<tb_user, bool>> expression = a => ids.Contains(a.id);

        var userList = await userService.QueryByIDsWithExpression(expression);
        var userDtoList = userList.Select(x=>mapper.Map<tb_user_dto>(x)).ToList();
        
        // 4.返回
        return Result.Success(userDtoList);
    }

    public async Task<Result> SaveBlog(tb_blog blog)
    {
        // 1.获取登录用户
        var user = UserHolder.GetUser();
        blog.userId = user.id;
        // 2.保存探店笔记
        var count = await base.Add(blog);
        
        if(count<1){
            return Result.Fail("新增笔记失败!");
        }
        
        // 3.查询笔记作者的所有粉丝 select * from tb_follow where follow_user_id = ?
        var follows = await followService.Query(x=>x.followUserId == user.id);
        
        // 4.推送笔记id给所有粉丝
        foreach (var follow in follows)
        {
            // 4.1.获取粉丝id
            var userId = follow.userId;
            // 4.2.推送
            var key = RedisConst.FEED_KEY + userId;
            redisDb.SortedSetAdd(key, blog.id, DateTimeOffset.Now.ToUnixTimeMilliseconds());
        }
        
        // 5.返回id
        return Result.Success(blog.id);
    }

    public async Task<Result> QueryBlogOfFollow(long max, int offset)
    {
        // 1.获取当前用户
        var userId = UserHolder.GetUser().id;
        // 2.查询收件箱 ZREVRANGEBYSCORE key Max Min LIMIT offset count
        string key = RedisConst.FEED_KEY + userId;
        
        var sortedSetEntries = redisDb.SortedSetRangeByScoreWithScores(
            key, 0, max,Exclude.Start, Order.Descending,offset,2);

        // 3.非空判断
        if (sortedSetEntries.IsNullOrEmpty()) {
            
            return Result.Success();
        }
        
        // 4.解析数据：blogId、minTime（时间戳）、offset
        var ids = new List<ulong>(sortedSetEntries.Length);
        long minTime = 0; // 2
        int os = 1; // 2

        foreach (var sortedSetEntry in sortedSetEntries)
        {
            // 4.1.获取id
            ids.Add(ulong.Parse(sortedSetEntry.Element));
            // 4.2.获取分数(时间戳）
            long time = (long)sortedSetEntry.Score;
            if(time == minTime){
                os++;
            }else{
                minTime = time;
                os = 1;
            }
        }

        // 5.根据id查询blog，能够实现 ORDER BY FIELD(id," + idStr + ")" 按照给的 id 的顺序排序
        Expression<Func<tb_blog, bool>> expression = a => ids.Contains(a.id);
        var blogs = await base.QueryByIDsWithExpression(expression);

        foreach (var blog in blogs)
        {
            // 5.1.查询blog有关的用户
            await QueryBlogUser(blog);
            // 5.2.查询blog是否被点赞
            await IsBlogLiked(blog);
        }

        // 6.封装并返回
        ScrollResult r = new ScrollResult();
        r.list = blogs.Select(a => (object)a).ToList();
        r.offset = os;
        r.minTime = minTime;

        return Result.Success(r);
    }

    private async Task QueryBlogUser(tb_blog blog)
    {
        var userId = blog.userId;
        var user = await userService.QueryById(userId);
        blog.name = user.nick_name;
        blog.icon = user.icon;
    }

    private async Task IsBlogLiked(tb_blog blog)
    {
        // 1. 获取当前登录用户
        var user = UserHolder.GetUser();
        
        if (user == null)
        {
            return;
        }
        
        // 2. 判断当前登录用户是否已经点赞
        var key = RedisConst.USER_BLOG_LIKE + blog.id;
        var sortedSetScore = redisDb.SortedSetScore(key,user.id);
        blog.isLike = sortedSetScore != null;
    }
}