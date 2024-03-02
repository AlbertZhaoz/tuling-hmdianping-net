using AutoMapper;
using com.hmdp.dto;
using com.hmdp.entity;

namespace com.hmdp.config;

/// <summary>
/// 静态全局 AutoMapper 配置文件
/// </summary>
public class AutoMapperConfig:Profile
{
    /// <summary>
    /// 配置构造函数，用来创建关系映射
    /// </summary>
    public AutoMapperConfig()
    {
        // 如果想要使用直接注入 IMapper.Mapper 即可
        CreateMap<tb_user, tb_user_dto>();
        // CreateMap<SysUserInfo, SysUserInfoDto>()
        // .ForMember(a => a.uID, o => o.MapFrom(d => d.Id))
        // .ForMember(a => a.RIDs, o => o.MapFrom(d => d.RIDs))
    }
}