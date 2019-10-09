using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    /// <summary>
    /// 订单主表信息
    /// </summary>
    public class ResMainInfo
    {
        public ResMainInfo()
        {
            ResDetailInfos = new List<ResDetailInfo>();
            SaveContinue = "0";
        }

        /// <summary>
        /// 是否继续保存，默认为0
        /// 在保存时根据情况，提示后，只有在操作员选择为继续保存时，才更改为1并且重新提交数据
        /// </summary>
        public string SaveContinue { get; set; }

        [Display(Name = "订单Id")]
        public string Resid { get; set; }

        [Display(Name = "订单号")]
        public string Resno { get; set; }

        [Display(Name = "订单名/团体名")]
        public string Name { get; set; }

        [Display(Name = "外部预订号")]
        public string ResNoExt { get; set; }

        [Display(Name = "预订人/入住人")]
        public string ResCustName { get; set; }

        [Display(Name = "手机")]
        public string ResCustMobile { get; set; }

        [Display(Name = "预订时间")]
        public string ResTime { get; set; }

        [Display(Name = "合约单位")]
        public Guid? Cttid { get; set; }

        /// <summary>
        /// 团体散客标志 0:散客，非团体 1：团体
        /// </summary>
        [Display(Name = "团体散客标志")]
        public byte? IsGroup { get; set; }

        /// <summary>
        /// 是否客情保密（0否，1是）
        /// </summary>
        [Display(Name = "客情保密")]
        public byte? IsCustemSecret { get; set; }

        /// <summary>
        /// 是否隐藏房价（0否，1是）
        /// </summary>
        [Display(Name = "隐藏房价")]
        public byte? IsHidePrice { get; set; }

        /// <summary>
        /// 原始订单信息的json字符串
        /// </summary>
        public string OriginResMainInfoJsonData { get; set; }

        /// <summary>
        /// 订单发票信息
        /// </summary>
        public ResInvoiceInfo ResInvoiceInfo { get; set; }

        /// <summary>
        /// 订单明细信息列表中的一项Regid选中
        /// </summary>
        public string SelectRegId { get; set; }

        /// <summary>
        /// 选中的Regid子单是否是新创建的入住订单
        /// </summary>
        public bool SelectRegIdIsNewCheckIn { get; set; }

        /// <summary>
        /// 订单明细信息
        /// </summary>
        public List<ResDetailInfo> ResDetailInfos { get; set; }

        [Display(Name = "外部订单类型")]
        public string ExtType { get; set; }

        /// <summary>
        /// 扩展 额外的其他信息
        /// </summary>
        public string OtherMessage { get; set; }
        /// <summary>
        /// 扩展 关联更新主单内所有roomType的价格
        /// </summary>
        public bool IsRelationUpdateAllRoonTypeRatePlan { get; set; }
        /// <summary>
        /// 扩展 是否更新备注到主单内的所有子单
        /// </summary>
        public bool IsRelationUpdateAllRemark { get; set; }

        /// <summary>
        /// 授权完成后继续保存订单（授权主键ID+授权时间）
        /// </summary>
        public string AuthorizationSaveContinue { get; set; }

        /// <summary>
        /// 积分换房 继续保存订单 1：继续保存，其他：返回提示信息
        /// </summary>
        public string UseScoreSaveContinue { get; set; }
    }
}
