using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace com.hmdp.entity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("tb_user")]
    public partial class tb_user
    {
           public tb_user(){


           }
           /// <summary>
           /// Desc:主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public ulong id {get;set;}

           /// <summary>
           /// Desc:手机号码
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string phone {get;set;}

           /// <summary>
           /// Desc:密码，加密存储
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? password {get;set;}

           /// <summary>
           /// Desc:昵称，默认是用户id
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? nick_name {get;set;}

           /// <summary>
           /// Desc:人物头像
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? icon {get;set;}

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
