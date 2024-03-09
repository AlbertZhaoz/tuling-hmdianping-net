using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.repo;

namespace com.hmdp.service;

public interface IBlogService:IBaseService<tb_blog>
{
    Task<Result> QueryHotBlog(int current);
    Task<Result> QueryBlogById(long id);
    Task<Result> LikeBlog(long id);
    Task<Result> QueryBlogLikes(long id);
    Task<Result> SaveBlog(tb_blog blog);
    Task<Result> QueryBlogOfFollow(long max,int offset);
}