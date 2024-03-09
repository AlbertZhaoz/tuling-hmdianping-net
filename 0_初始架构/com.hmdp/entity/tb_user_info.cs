using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace com.hmdp.entity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("tb_user_info")]
    public partial class tb_user_info
    {
           public tb_user_info(){


           }
           /// <summary>
           /// Desc:主键，用户id
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public ulong user_id {get;set;}

           /// <summary>
           /// Desc:城市名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? city {get;set;}

           /// <summary>
           /// Desc:个人介绍，不要超过128个字符
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? introduce {get;set;}

           /// <summary>
           /// Desc:粉丝数量
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public uint fans {get;set;}

           /// <summary>
           /// Desc:关注的人的数量
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public uint followee {get;set;}

           /// <summary>
           /// Desc:性别，0：男，1：女
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public short gender {get;set;}

           /// <summary>
           /// Desc:生日
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? birthday {get;set;}

           /// <summary>
           /// Desc:积分
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public uint credits {get;set;}

           /// <summary>
           /// Desc:会员级别，0~9级,0代表未开通会员
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public short level {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           public DateTime create_time {get;set;}

           /// <summary>
           /// Desc:更新时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           public DateTime update_time {get;set;}

    }
}
