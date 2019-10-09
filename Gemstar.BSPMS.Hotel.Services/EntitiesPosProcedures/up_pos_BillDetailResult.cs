using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{

    public class up_pos_BillDetailResult
    {

        [Display(Name = "id")]
        public Int64 Id { get; set; }


        [Display(Name = "酒店代码")]
        public string Hid { get; set; }


        [Display(Name = "账单id")]
        public string MBillid { get; set; }


        [Display(Name = "业务单id")]
        public string Billid { get; set; }


        [Display(Name = "项目id")]
        public string Itemid { get; set; }


        [Display(Name = "项目代码")]
        public string ItemCode { get; set; }


        [Display(Name = "项目名称")]
        public string ItemName { get; set; }


        [Display(Name = "单位id")]
        public string Unitid { get; set; }


        [Display(Name = "单位代码")]
        public string UnitCode { get; set; }


        [Display(Name = "单位名称")]
        public string UnitName { get; set; }

        [Display(Name = "数量")]
        public decimal? Quantity { get; set; }

        [Display(Name = "称重条只")]
        public decimal? Piece { get; set; }

        [Display(Name = "扣钝倍数")]
        public decimal? Multiple { get; set; }

        [Display(Name = "单价")]
        public decimal? Price { get; set; }

        [Display(Name = "作法加价")]
        public decimal? AddPrice { get; set; }

        [Display(Name = "折前金额")]
        public decimal? Dueamount { get; set; }

        [Display(Name = "金额")]
        public decimal? Amount { get; set; }

        [Display(Name = "服务费")]
        public decimal? Service { get; set; }

        [Display(Name = "税额")]
        public decimal? Tax { get; set; }


        [Display(Name = "客位")]
        public string Place { get; set; }


        [Display(Name = "作法")]
        public string Action { get; set; }


        [Display(Name = "要求")]
        public string Request { get; set; }


        [Display(Name = "消费还是付款")]
        public string DcFlag { get; set; }

        [Display(Name = "结账状态")]
        public bool? IsCheck { get; set; }

        [Display(Name = "自动标志")]
        public byte? Isauto { get; set; }

        [Display(Name = "计费状态")]
        public byte? Status { get; set; }

        [Display(Name = "出品状态")]
        public byte? IsProduce { get; set; }

        [Display(Name = "求和套餐")]
        public bool? SP { get; set; }
        
        [Display(Name = "求和套餐明细")]
        public bool? SD { get; set; }
        [Display(Name = "所属套餐")]
        public Guid? Upid { get; set; }

        [Display(Name = "套餐分摊金额")]
        public decimal? OverSuite { get; set; }


        [Display(Name = "餐台id")]
        public string Tabid { get; set; }


        [Display(Name = "锁牌号")]
        public string Keyid { get; set; }

        [Display(Name = "单道折扣")]
        public decimal? Discount { get; set; }

        [Display(Name = "是否单道折扣")]
        public bool? IsDishDisc { get; set; }

        [Display(Name = "单道金额折")]
        public decimal? DiscAmount { get; set; }


        [Display(Name = "批次")]
        public string BatchTime { get; set; }


        [Display(Name = "业务员")]
        public string Sale { get; set; }

        [Display(Name = "消费营业日")]
        public DateTime? TransBsnsDate { get; set; }


        [Display(Name = "消费市别")]
        public string TransShuffleid { get; set; }


        [Display(Name = "消费班次")]
        public string TransShiftid { get; set; }


        [Display(Name = "入账操作员")]
        public string TransUser { get; set; }

        [Display(Name = "消费时间")]
        public DateTime? TransDate { get; set; }


        [Display(Name = "修改操作员")]
        public string ModiUser { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? ModiDate { get; set; }

        [Display(Name = "结账id")]
        public Guid? Settleid { get; set; }


        [Display(Name = "结账交易号")]
        public string SettleTransno { get; set; }


        [Display(Name = "结账交易名称")]
        public string SettleTransName { get; set; }

        [Display(Name = "结账营业日")]
        public DateTime? SettleBsnsDate { get; set; }


        [Display(Name = "结账市别")]
        public string SettleShuffleid { get; set; }


        [Display(Name = "结账班次")]
        public string SettleShiftId { get; set; }


        [Display(Name = "结账操作员")]
        public string SettleUser { get; set; }

        [Display(Name = "结账时间")]
        public DateTime? SettleDate { get; set; }

        [Display(Name = "原价")]
        public decimal? PriceOri { get; set; }

        [Display(Name = "会员价")]
        public decimal? PriceClub { get; set; }

        [Display(Name = "会员折扣")]
        public decimal? DiscountClub { get; set; }


        [Display(Name = "批准人")]
        public string Approver { get; set; }


        [Display(Name = "取消原因")]
        public string CanReason { get; set; }


        [Display(Name = "备注")]
        public string Memo { get; set; }


        [Display(Name = "手工单号")]
        public string Acbillno { get; set; }

        [Display(Name = "打单次数")]
        public byte? iPrint { get; set; }

        [Display(Name = "点菜单次数")]
        public byte? iOrderPrint { get; set; }
    }
}
