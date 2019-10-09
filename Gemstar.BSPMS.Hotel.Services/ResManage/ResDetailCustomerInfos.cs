using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    /// <summary>
    /// 订单客人信息
    /// </summary>
    public class ResDetailCustomerInfos
    {
        [Display(Name = "客人Id")]
        public Guid Id { get; set; }

        [Display(Name = "账号")]
        public string Regid { get; set; }

        [Display(Name = "房号")]
        public string RoomNo { get; set; }

        [Display(Name = "客人姓名")]
        public string GuestName { get; set; }

        [Display(Name = "性别")]
        public string Gender { get; set; }

        [Display(Name = "生日")]
        public DateTime? Birthday { get; set; }

        [Display(Name = "证件类型")]
        public string CerType { get; set; }

        [Display(Name = "证件号")]
        public string Cerid { get; set; }

        [Display(Name = "国籍")]
        public string Nation { get; set; }

        [Display(Name = "籍贯")]
        public string City { get; set; }

        [Display(Name = "地址")]
        public string Address { get; set; }

        [Display(Name = "熟客Id")]
        public Guid? GuestId { get; set; }

        [Display(Name = "身份证照片")]
        public string PhotoUrl { get; set; }
    }
}
