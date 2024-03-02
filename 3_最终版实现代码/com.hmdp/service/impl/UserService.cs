using com.hmdp.attribute;
using com.hmdp.Const;
using com.hmdp.dto;
using com.hmdp.entity;
using com.hmdp.repo;
using com.hmdp.utils;
using Masuit.Tools;
using Masuit.Tools.Strings;
using StackExchange.Redis;

namespace com.hmdp.service.impl;

public class UserService:BaseService<tb_user>,IUserService
{
    [AutoWire]
    public IDatabase RedisDb { get; set; }
    
    // 如果要设置到 Session 中，注入 IHttpContextAccessor _httpContextAccessor;
    // var currentSession = _httpContextAccessor.HttpContext.Session;
    // currentSession.SetString("Phone", phone);

    public async Task<Result> SendCode(string phone)
    {
        // 1.校验手机号
        if (!phone.MatchPhoneNumber())
        {
            // 2.如果不符合，返回错误信息
            return Result.Fail("手机号格式不正确");
        }
        
        // 3.符合，生成验证码
        var code = ValidateCode.CreateValidateCode(6);
        
        // 4.保存验证码到 redis 中
        RedisDb.StringSet(RedisConst.LOGIN_CODE_KEY + phone, 
            code, 
            TimeSpan.FromMinutes(RedisConst.LOGIN_CODE_TTL));
        
        // 5.发送验证码到手机
        // 6.返回成功信息
        return Result.Success("发送成功");
    }

    public async Task<Result> Login(login_form_dto loginForm)
    {
        // 1.校验手机号
        if (!loginForm.phone.MatchPhoneNumber())
        {
            // 2.如果不符合，返回错误信息
            return Result.Fail("手机号格式不正确");
        }
        // 3.1 从 redis 获取验证码并校验
        var code = RedisDb.StringGet(RedisConst.LOGIN_CODE_KEY + loginForm.phone);
        
        if (string.IsNullOrEmpty(code) || code != loginForm.code)
        {
            // 3.2 如果不符合，返回错误信息
            return Result.Fail("验证码错误");
        }
        
        // 4.一致，根据手机号查询用户 select * from tb_user where phone = ?
        var user = (await base.Query(t => t.phone == loginForm.phone)).FirstOrDefault();

        // 5.判断用户是否存在
        if (user == null)
        {
            // 6.不存在，创建新用户并保存
            user = await CreateUserWithPhone(loginForm.phone);
        }
        
        // 7.保存用户信息到 redis 中
        // 7.1.随机生成token，作为登录令牌，不带横杠
        var token = Guid.NewGuid().ToString("N");
        // 7.2.将User对象转为HashMap存储到redis中
        var tokenKey = RedisConst.LOGIN_USER_KEY + token;
        RedisDb.HashSet(tokenKey, user.ToHashEntries());
        // 7.3.设置过期时间 30 分钟
        RedisDb.KeyExpire(tokenKey, TimeSpan.FromMinutes(RedisConst.LOGIN_USER_TTL));
        
        // 8.返回token
        return Result.Success(token);
    }
    
    public async Task<Result> Sign()
    {
        var (key,day) = GetSignKeyAndDay();
        // 5.签到：写入Redis SETBIT key offset 1
        RedisDb.StringSetBit(key,day-1,true);
        
        return Result.Success();
    }

    public async Task<Result> SignCount()
    {
        var (key,day) = GetSignKeyAndDay();

        return Result.Success();
    }

    private (string,int) GetSignKeyAndDay()
    {
        // 1.获取当前登录用户
        var userId = UserHolder.GetUser().id;
        // 2.获取日期
        var now = DateTime.Now.ToString(":yyyyMM");
        // 3.拼接 key
        var key  = RedisConst.USER_SIGN_KEY+ userId + now;
        // 4.获取今天是本月的第几天
        var day = DateTime.Now.Day;
        // 5.获取本月截止今天为止的所有的签到记录，返回的是一个十进制的数字 BITFIELD sign:5:202203 GET u14 0
        return (key,day);
    }

    private async Task<tb_user> CreateUserWithPhone(string phone) {
        // 1.创建用户
        var user = new tb_user();
        user.phone = phone;
        user.nick_name = SystemConst.SER_NICK_NAME_PREFIX + ValidateCode.CreateValidateCode(10);
        // 2.保存用户
        await base.Add(user);
        return user;
    }
}