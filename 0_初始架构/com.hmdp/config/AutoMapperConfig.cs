using AutoMapper;

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
        // CreateMap<BlogArticle, BlogViewModels>();
        // CreateMap<BlogViewModels, BlogArticle>();
        // CreateMap<SysUserInfo, SysUserInfoDto>()
        // .ForMember(a => a.uID, o => o.MapFrom(d => d.Id))
        // .ForMember(a => a.RIDs, o => o.MapFrom(d => d.RIDs))
        // .ForMember(a => a.addr, o => o.MapFrom(d => d.Address))
        // .ForMember(a => a.age, o => o.MapFrom(d => d.Age))
        // .ForMember(a => a.birth, o => o.MapFrom(d => d.Birth))
        // .ForMember(a => a.uStatus, o => o.MapFrom(d => d.Status))
        // .ForMember(a => a.uUpdateTime, o => o.MapFrom(d => d.UpdateTime))
        // .ForMember(a => a.uCreateTime, o => o.MapFrom(d => d.CreateTime))
        // .ForMember(a => a.uErrorCount, o => o.MapFrom(d => d.ErrorCount))
        // .ForMember(a => a.uLastErrTime, o => o.MapFrom(d => d.LastErrorTime))
        // .ForMember(a => a.uLoginName, o => o.MapFrom(d => d.LoginName))
        // .ForMember(a => a.uLoginPWD, o => o.MapFrom(d => d.LoginPWD))
        // .ForMember(a => a.uRemark, o => o.MapFrom(d => d.Remark))
        // .ForMember(a => a.uRealName, o => o.MapFrom(d => d.RealName))
        // .ForMember(a => a.name, o => o.MapFrom(d => d.Name))
        // .ForMember(a => a.tdIsDelete, o => o.MapFrom(d => d.IsDeleted))
        // .ForMember(a => a.RoleNames, o => o.MapFrom(d => d.RoleNames));
    }
}