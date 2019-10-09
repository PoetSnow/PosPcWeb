using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosCashier
{
    public class OtherSysPayInfo
    {
        //卡号
        public string MbrCardNo { get; set; }
        //会员名称
        public string GuestCName { get; set; }
        //当前余额，注意要减掉支付金额
        public decimal lblBalance { get; set; }
        //合约单位名称
        public string CorpAutoComplete { get; set; }
        //签单人
        public string CorpSignPerson { get; set; }
        //房号
        public string roomNo { get; set; }
        //客人名称
        public string labelRoom { get; set; }
    }
}