using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{

    [Table("Hotel")]
    public class CenterHotel
    {
        [Column(TypeName = "char")]
        public string Grpid { get; set; }

        [Key]
        [Column(TypeName = "char")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        public string Provinces { get; set; }

        [Column(TypeName = "varchar")]
        public string City { get; set; }

        [Column(TypeName = "varchar")]
        public string Star { get; set; }

        [Column(TypeName = "varchar")]
        public string Email { get; set; }

        [Column(TypeName = "varchar")]
        public string Mobile { get; set; }

        public Guid Serverid { get; set; }

        public Guid Dbid { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public byte? Status { get; set; }

        [Column(TypeName = "varchar")]
        public string LockType { get; set; }

        [Column(TypeName = "varchar")]
        public string IdCardType { get; set; }

        [Column(TypeName = "varchar")]
        public string ItemAction { get; set; }

        [Column(TypeName = "varchar")]
        public string PicLink { get; set; }

        [Column(TypeName = "varchar")]
        public string GsUserId { get; set; }

        [Column(TypeName = "varchar")]
        public string OnlineLockType { get; set; }

        [Column(TypeName = "varchar")]
        public string MbrType { get; set; }

        public bool isOpenAnalysis { get; set; }
         
        public string Remark { get; set; }

        public string Hotelshortname { get; set; }

        public string manageType { get; set; }

        public string Sales { get; set; }

        public string Contactsname { get; set; }

        public string address { get; set; }

        public string oriSystem { get; set; }

        /// <summary>
        /// 餐饮属性
        /// </summary>
        public string CateringServicesType { get; set; }

        /// <summary>
        /// 餐饮模块
        /// </summary>
        public string CateringServicesModule { get; set; }
        
        /// <summary>
        /// 收银点个数
        /// </summary>
        public byte? PosSettlePointCount { get; set; }
        /// <summary>
        /// 如果后台有设置餐饮属性，则返回设置的餐饮属性，否则返回固定的A
        /// </summary>
        public string CateringServicesTypeOrDefault {
            get {
                if (string.IsNullOrWhiteSpace(CateringServicesType)) {
                    return "A";
                }
                return CateringServicesType;
            }
        }
        /// <summary>
        /// 如果后台有设置餐饮模块，则返回餐饮模块，否则返回固定的A3，收银，以便可以进入设置基础资料
        /// </summary>
        public string CateringServicesModuleOrDefault {
            get {
                if (string.IsNullOrWhiteSpace(CateringServicesModule)) {
                    return "A3";
                }
                return CateringServicesModule;
            }
        }

    }
}
