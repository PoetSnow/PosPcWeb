using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class MbrCardInfoModel
    {
        public string MbrCardType  {get; set;}
        public string MbrCardTypeName  {get; set;}
        public string MbrCardNo  {get; set;}
        public string GuestCName {get; set;}
        public string DiscountMode  {get; set;}
        public decimal DiscountRate  {get; set;}
        public string DisCountModeName {get; set;}

        public bool IsHasDiscount { get; set; }
        public string ProFileID { get; set; }  //会员ID

        public decimal? Balance { get; set; }   //储值余额 + 增值余额

        public decimal? BaseAmtBalance { get; set; }  //本金余额（储值余额）

        public decimal? Incamount { get; set; }  //增值余额




    }
}
