using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace com.hmdp.entity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("tb_voucher_order")]
    public partial class tb_voucher_order
    {
           public tb_voucher_order(){


           }
           /// <summary>
           /// Desc:主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public long id {get;set;}

           /// <summary>
           /// Desc:下单的用户id
           /// Default:
           /// Nullable:False
           /// </summary>           
           public ulong user_id {get;set;}

           /// <summary>
           /// Desc:购买的代金券id
           /// Default:
           /// Nullable:False
           /// </summary>           
           public ulong voucher_id {get;set;}

           /// <summary>
           /// Desc:支付方式 1：余额支付；2：支付宝；3：微信
           /// Default:1
           /// Nullable:False
           /// </summary>           
           public short pay_type {get;set;}

           /// <summary>
           /// Desc:订单状态，1：未支付；2：已支付；3：已核销；4：已取消；5：退款中；6：已退款
           /// Default:1
           /// Nullable:False
           /// </summary>           
           public short status {get;set;}

           /// <summary>
           /// Desc:下单时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           public DateTime create_time {get;set;}

           /// <summary>
           /// Desc:支付时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? pay_time {get;set;}

           /// <summary>
           /// Desc:核销时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? use_time {get;set;}

           /// <summary>
           /// Desc:退款时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? refund_time {get;set;}

           /// <summary>
           /// Desc:更新时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           public DateTime update_time {get;set;}

    }
}
