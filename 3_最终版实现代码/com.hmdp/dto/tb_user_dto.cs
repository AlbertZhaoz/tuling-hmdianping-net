using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace com.hmdp.dto
{
    public class tb_user_dto
    {
           /// <summary>
           /// Desc:主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           public ulong id {get;set;}
           
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
    }
}
