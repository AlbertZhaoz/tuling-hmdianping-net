using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace com.hmdp.entity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("tb_voucher")]
    public partial class tb_voucher
    {
           public tb_voucher(){


           }
           /// <summary>
           /// Desc:主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public ulong id {get;set;}

           /// <summary>
           /// Desc:商铺id
           /// Default:
           /// Nullable:True
           /// </summary>
           [SugarColumn(ColumnName = "shop_id")] 
           public ulong shopId {get;set;}

           /// <summary>
           /// Desc:代金券标题
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string title {get;set;}

           /// <summary>
           /// Desc:副标题
           /// Default:
           /// Nullable:True
           /// </summary>
           [SugarColumn(ColumnName = "sub_title")]  
           public string? subTitle {get;set;}

           /// <summary>
           /// Desc:使用规则
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? rules {get;set;}

           /// <summary>
           /// Desc:支付金额，单位是分。例如200代表2元
           /// Default:
           /// Nullable:False
           /// </summary>
           [SugarColumn(ColumnName = "pay_value")]  
           public ulong payValue {get;set;}

           /// <summary>
           /// Desc:抵扣金额，单位是分。例如200代表2元
           /// Default:
           /// Nullable:False
           /// </summary>
           [SugarColumn(ColumnName = "actual_value")] 
           public long actualValue {get;set;}

           /// <summary>
           /// Desc:0,普通券；1,秒杀券
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public short type {get;set;}

           /// <summary>
           /// Desc:1,上架; 2,下架; 3,过期
           /// Default:1
           /// Nullable:False
           /// </summary>           
           public short status {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>
           [SugarColumn(ColumnName = "create_time")]  
           public DateTime createTime {get;set;}

           /// <summary>
           /// Desc:更新时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>
           [SugarColumn(ColumnName = "update_time")]   
           public DateTime updateTime {get;set;}

           /// <summary>
           /// 库存
           /// </summary>
           [SugarColumn(IsIgnore = true)]
           public int stock { get; set; }
           
           /// <summary>
           /// 生效时间
           /// </summary>
           [SugarColumn(IsIgnore = true)]
           public DateTime beginTime { get; set; }
           
           /// <summary>
           /// 失效时间
           /// </summary>
           [SugarColumn(IsIgnore = true)]
           public DateTime endTime { get; set; }
    }
}
