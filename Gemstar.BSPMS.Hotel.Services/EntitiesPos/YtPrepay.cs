using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("YtPrepay")]
    public class YtPrepay
    {
        [LogIgnore]
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("酒店hid")]
        public string Hid { get; set; }

        [LogIgnore]
        [LogCName("会员id")]
        public Guid? Cardid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("会员卡号")]
        public string CardNo { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("姓名")]
        public string VGuest { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("原单号")]
        public string OriBillNo { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("押金单号")]
        public string BillNo { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("营业点")]
        public string PosNo { get; set; }

        [LogIgnore]
        [LogCName("押金类型")]
        public byte? IType { get; set; } 

        [LogIgnore]
        [LogCName("是否结清")]
        public byte? IsClear { get; set; }       

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("付款方式")]
        public string PayModeNo { get; set; }

        [LogIgnore]
        [LogCName("原币金额")]
        public decimal? Amount { get; set; }  

        [LogIgnore]
        [LogCName("本位币金额")]
        public decimal? Amountbwb { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("批准人")]
        public string Approver { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("付款单号")]
        public string PaidNo { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("创建人")]
        public string Creator { get; set; }

        [LogIgnore]
        [LogCName("创建时间")]
        public DateTime? CreateDate { get; set; }     

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("修改人")]
        public string ModifiCator { get; set; }

        [LogIgnore]
        [LogCName("修改时间")]
        public DateTime? ModifiedDate { get; set; }

        [LogIgnore]
        [LogCName("押金状态")]
        public byte? IPrepay { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }

        [LogIgnore]
        [LogCName("营业日")]
        public DateTime? DBusiness { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("班次")]
        public string Shiftid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("联系电话")]
        public string Mobile { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("收据号码")]
        public string HandBillno { get; set; }

        [LogCName("使用时间")]
        public DateTime? UsedDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("使用说明")]
        public string UsedDesc { get; set; }

        [LogCName("是否短信通知")]
        public bool? IsMsg { get; set; }


        [LogCName("微信，支付宝支付交易号")]
        public string PayTransno { get; set; }
    }
}
