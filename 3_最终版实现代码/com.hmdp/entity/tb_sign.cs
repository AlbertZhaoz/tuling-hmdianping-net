using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace com.hmdp.entity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("tb_sign")]
    public partial class tb_sign
    {
           public tb_sign(){


           }
           /// <summary>
           /// Desc:主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public ulong id {get;set;}

           /// <summary>
           /// Desc:用户id
           /// Default:
           /// Nullable:False
           /// </summary>           
           public ulong user_id {get;set;}

           /// <summary>
           /// Desc:签到的年
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int year {get;set;}

           /// <summary>
           /// Desc:签到的月
           /// Default:
           /// Nullable:False
           /// </summary>           
           public byte month {get;set;}

           /// <summary>
           /// Desc:签到的日期
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime date {get;set;}

           /// <summary>
           /// Desc:是否补签
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short is_backup {get;set;}

    }
}
