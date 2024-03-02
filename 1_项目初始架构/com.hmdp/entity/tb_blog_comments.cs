using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace com.hmdp.entity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("tb_blog_comments")]
    public partial class tb_blog_comments
    {
           public tb_blog_comments(){


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
           /// Desc:探店id
           /// Default:
           /// Nullable:False
           /// </summary>           
           public ulong blog_id {get;set;}

           /// <summary>
           /// Desc:关联的1级评论id，如果是一级评论，则值为0
           /// Default:
           /// Nullable:False
           /// </summary>           
           public ulong parent_id {get;set;}

           /// <summary>
           /// Desc:回复的评论id
           /// Default:
           /// Nullable:False
           /// </summary>           
           public ulong answer_id {get;set;}

           /// <summary>
           /// Desc:回复的内容
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string content {get;set;}

           /// <summary>
           /// Desc:点赞数
           /// Default:
           /// Nullable:True
           /// </summary>           
           public uint liked {get;set;}

           /// <summary>
           /// Desc:状态，0：正常，1：被举报，2：禁止查看
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short status {get;set;}

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
