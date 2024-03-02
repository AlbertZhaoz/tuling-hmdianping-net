using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace com.hmdp.entity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("tb_follow")]
    public partial class tb_follow
    {
           public tb_follow(){


           }
           /// <summary>
           /// Desc:主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long id {get;set;}

           /// <summary>
           /// Desc:用户id
           /// Default:
           /// Nullable:False
           /// </summary>
           [SugarColumn(ColumnName = "user_id")] 
           public ulong userId {get;set;}

           /// <summary>
           /// Desc:关联的用户id
           /// Default:
           /// Nullable:False
           /// </summary>
           [SugarColumn(ColumnName = "follow_user_id")]  
           public ulong followUserId {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           public DateTime create_time {get;set;}

    }
}
