using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.repo;

namespace com.hmdp.service;

public interface IFollowService:IBaseService<tb_follow>
{
    Task<Result> Follow(ulong followUserId, bool isFollow);
    Task<Result> IsFollow(ulong followUserId);
    Task<Result> FollowCommons(long id);
}