using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    public class ResDetailRegInfo
    {
        /// <summary>
        /// 主键ID 流水号
        /// </summary>
        [Display(Name = "主键ID 流水号")]
        public string Id { get; set; }

        /// <summary>
        /// 登记单id
        /// </summary>
        [Display(Name = "登记单id")]
        public string RegId { get; set; }

        /// <summary>
        /// 客人名
        /// </summary>
        [Display(Name = "客人名")]
        public string GuestName { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        [Display(Name = "证件类型")]
        public string CerType { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        [Display(Name = "证件号")]
        public string CerId { get; set; }

        /// <summary>
        /// 性别（M：男F：女）
        /// </summary>
        [Display(Name = "性别")]
        public string Gender { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        [Display(Name = "籍贯")]
        public string City { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Display(Name = "地址")]
        public string Address { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [Display(Name = "生日")]
        public string Birthday { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        public string Mobile { get; set; }

        /// <summary>
        /// qq号
        /// </summary>
        [Display(Name = "qq号")]
        public string Qq { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 喜好
        /// </summary>
        [Display(Name = "喜好")]
        public string Interest { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        [Display(Name = "车牌号")]
        public string CarNo { get; set; }

        /// <summary>
        /// 国籍
        /// </summary>
        [Display(Name = "国籍")]
        public string Nation { get; set; }

        /// <summary>
        /// 是否随行人，Accompanying的缩写。1:主客，0:随行人 内部使用
        /// </summary>
        [Display(Name = "车牌号")]
        public string IsMast { get; set; }

        /// <summary>
        /// 身份证照片
        /// </summary>
        [Display(Name = "身份证照片")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 原始随行人信息的json字符串
        /// </summary>
        public string OriginRegInfoJsonData { get; set; }
    }
}
