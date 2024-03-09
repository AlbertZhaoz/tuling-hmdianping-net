using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace com.hmdp.entity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("tb_blog")]
    public partial class tb_blog
    {
           public tb_blog(){


           }
           /// <summary>
           /// Desc:主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public ulong id {get;set;}

           /// <summary>
           /// Desc:商户id
           /// Default:
           /// Nullable:False
           /// </summary>
           [SugarColumn(ColumnName = "shop_id")] 
           public long shopId {get;set;}

           /// <summary>
           /// Desc:用户id
           /// Default:
           /// Nullable:False
           /// </summary>
           [SugarColumn(ColumnName = "user_id")]  
           public ulong userId {get;set;}

           /// <summary>
           /// Desc:标题
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string title {get;set;}

           /// <summary>
           /// Desc:探店的照片，最多9张，多张以","隔开
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string images {get;set;}

           /// <summary>
           /// Desc:探店的文字描述
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string content {get;set;}

           /// <summary>
           /// Desc:点赞数量
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public uint liked {get;set;}

           /// <summary>
           /// Desc:评论数量
           /// Default:
           /// Nullable:True
           /// </summary>           
           public uint comments {get;set;}

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
           
           /// <summary>
           /// 用户图标
           /// </summary>
           [SugarColumn(IsIgnore = true)]
           public string icon { get; set; }
           /// <summary>
           /// 用户姓名
           /// </summary>
           [SugarColumn(IsIgnore = true)]
           public string name { get; set; }
           /// <summary>
           /// 是否点赞过了
           /// </summary>
           [SugarColumn(IsIgnore = true)]
           public bool isLike { get; set; }
    }
}
