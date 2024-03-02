using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace com.hmdp.entity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("tb_shop")]
    public partial class tb_shop
    {
           public tb_shop(){


           }
           /// <summary>
           /// Desc:主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public ulong id {get;set;}

           /// <summary>
           /// Desc:商铺名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string name {get;set;}

           /// <summary>
           /// Desc:商铺类型的id
           /// Default:
           /// Nullable:False
           /// </summary>
           [SugarColumn(ColumnName = "type_id")] 
           public ulong typeId {get;set;}

           /// <summary>
           /// Desc:商铺图片，多个图片以','隔开
           /// Default:
           /// Nullable:False
           /// </summary>
           public string images {get;set;}

           /// <summary>
           /// Desc:商圈，例如陆家嘴
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? area {get;set;}

           /// <summary>
           /// Desc:地址
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string address {get;set;}

           /// <summary>
           /// Desc:经度
           /// Default:
           /// Nullable:False
           /// </summary>           
           public double x {get;set;}

           /// <summary>
           /// Desc:维度
           /// Default:
           /// Nullable:False
           /// </summary>           
           public double y {get;set;}

           /// <summary>
           /// Desc:均价，取整数
           /// Default:
           /// Nullable:True
           /// </summary>
           [SugarColumn(ColumnName = "avg_price")]  
           public ulong avgPrice {get;set;}

           /// <summary>
           /// Desc:销量
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int sold {get;set;}

           /// <summary>
           /// Desc:评论数量
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int comments {get;set;}

           /// <summary>
           /// Desc:评分，1~5分，乘10保存，避免小数
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int score {get;set;}

           /// <summary>
           /// Desc:营业时间，例如 10:00-22:00
           /// Default:
           /// Nullable:True
           /// </summary>
           [SugarColumn(ColumnName = "open_hours")] 
           public string? openHours {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>
           [SugarColumn(ColumnName = "create_time")]  
           public DateTime? createTime {get;set;}

           /// <summary>
           /// Desc:更新时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>
           [SugarColumn(ColumnName = "update_time")]  
           public DateTime? updateTime {get;set;}
           
           /// <summary>
           /// 距离
           /// </summary>
           [SugarColumn(IsIgnore = true)]
           public double distance {get;set;}

    }
}
