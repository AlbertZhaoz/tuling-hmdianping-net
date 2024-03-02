using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace com.hmdp.entity
{
    ///<summary>
    ///秒杀优惠券表，与优惠券是一对一关系
    ///</summary>
    [SugarTable("tb_seckill_voucher")]
    public partial class tb_seckill_voucher
    {
           public tb_seckill_voucher(){


           }
           /// <summary>
           /// Desc:关联的优惠券的id
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity = false,ColumnName = "voucher_id")]
           public long voucherId {get;set;}

           /// <summary>
           /// Desc:库存
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int stock {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           public DateTime create_time {get;set;}

           /// <summary>
           /// Desc:生效时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           public DateTime begin_time {get;set;}

           /// <summary>
           /// Desc:失效时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           public DateTime end_time {get;set;}

           /// <summary>
           /// Desc:更新时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           public DateTime update_time {get;set;}

    }
}
