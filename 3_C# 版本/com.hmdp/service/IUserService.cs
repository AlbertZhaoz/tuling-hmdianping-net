using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.repo;

namespace com.hmdp.service;

public interface IUserService:IBaseService<tb_user>
{
    Task<Result>  SendCode(string phone);
    Task<Result>  Login(login_form_dto loginForm);
    Task<Result>  Sign();
    Task<Result>  SignCount();
}