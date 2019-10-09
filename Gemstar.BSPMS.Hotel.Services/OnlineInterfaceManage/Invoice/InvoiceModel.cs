using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.OnlineInterfaceManage.Invoice
{
    public class InvoiceModel : OnlineInterfaceModel
    {
        /// <summary>
        /// 商户的接入标识号
        /// </summary>
        [Display(Name = "发票授权ID")]
        [Required(ErrorMessage = "请输入发票授权ID")]
        public string InvoiceAppId { get; set; }

        /// <summary>
        /// 商户的接入密钥
        /// </summary>
        [Display(Name = "发票授权秘钥")]
        [Required(ErrorMessage = "请输入发票授权秘钥")]
        public string InvoiceAppSecret { get; set; }
    }
    /// <summary>
    /// 接口交互类
    /// </summary>
    [DataContract]
    public class InvoiceParameter
    {
        /// <summary>
        /// 获取凭证
        /// </summary>
        public class AccessToken
        {
            /// <summary>
            /// 请求类
            /// </summary>
            public class Request
            {
                /// <summary>
                /// 发票授权id
                /// </summary>
                public string invoiceappid { get; set; }
                /// <summary>
                /// 发票授权秘钥
                /// </summary>
                public string invoiceappsecret { get; set; }
            }

            /// <summary>
            /// 返回类
            /// </summary>
            public class Response
            {
                /// <summary>
                /// 提示信息
                /// </summary>
                [DataMember]
                public string msg { get; set; }
                /// <summary>
                /// 秘钥
                /// </summary>
                [DataMember]
                public string key { get; set; }
                /// <summary>
                /// 返回状态代码
                /// </summary>
                [DataMember]
                public int code { get; set; }
                /// <summary>
                /// 过期时间
                /// </summary>
                [DataMember]
                public string expire { get; set; }
            }
        }

        /// <summary>
        /// 获取商品代码列表
        /// </summary>
        public class FindAllSP
        {
            /// <summary>
            /// 请求类
            /// </summary>
            public class Request
            {
                /// <summary>
                /// 秘钥
                /// </summary>
                public string key { get; set; }
            }

            /// <summary>
            /// 返回类
            /// </summary>
            public class Response
            {
                /// <summary>
                /// 分类id
                /// </summary>
                [DataMember]
                public string classify { get; set; }
                /// <summary>
                /// 商品TCIS_ID
                /// </summary>
                [DataMember]
                public string code { get; set; }
                /// <summary>
                /// 商品名称
                /// </summary>
                [DataMember]
                public string name { get; set; }
                /// <summary>
                /// 单价
                /// </summary>
                [DataMember]
                public string price { get; set; }
                /// <summary>
                /// 税率
                /// </summary>
                [DataMember]
                public string rate { get; set; }
                /// <summary>
                /// 规格
                /// </summary>
                [DataMember]
                public string spec { get; set; }
                /// <summary>
                /// 单位
                /// </summary>
                [DataMember]
                public string unit { get; set; }
                /// <summary>
                /// 排序
                /// </summary>
                [DataMember]
                public string seqid { get; set; }
                /// <summary>
                /// ID
                /// </summary>
                [DataMember]
                public string id { get; set; }
                /// <summary>
                /// 零税率标识  零税率标识 默认0 1、免税 税率填写0  2、不征收 税率填写0
                /// </summary>
                [DataMember]
                public string zerotaxratestate { get; set; }
                /// <summary>
                /// 优待政策标识 优惠政策标识 0无优惠 1有优惠
                /// </summary>
                [DataMember]
                public string preferentialstate { get; set; }
                /// <summary>
                /// 优待政策说明
                /// </summary>
                [DataMember]
                public string preferentialexplain { get; set; }
            }
        }

        /// <summary>
        /// 获取商品代码
        /// </summary>
        public class FindSP
        {
            /// <summary>
            /// 请求类
            /// </summary>
            public class Request
            {
                /// <summary>
                /// 秘钥
                /// </summary>
                public string key { get; set; }

                /// <summary>
                /// 商品的名字，税务系统的名字，不是消费商品的名字，不支持模糊查询
                /// </summary>
                public string name { get; set; }
            }

            /// <summary>
            /// 返回类
            /// </summary>
            public class Response
            {
                /// <summary>
                /// 分类id
                /// </summary>
                [DataMember]
                public string classify { get; set; }
                /// <summary>
                /// 商品TCIS_ID
                /// </summary>
                [DataMember]
                public string code { get; set; }
                /// <summary>
                /// 商品名称
                /// </summary>
                [DataMember]
                public string name { get; set; }
                /// <summary>
                /// 单价
                /// </summary>
                [DataMember]
                public string price { get; set; }
                /// <summary>
                /// 税率
                /// </summary>
                [DataMember]
                public string rate { get; set; }
                /// <summary>
                /// 规格
                /// </summary>
                [DataMember]
                public string spec { get; set; }
                /// <summary>
                /// 单位
                /// </summary>
                [DataMember]
                public string unit { get; set; }
                /// <summary>
                /// 排序
                /// </summary>
                [DataMember]
                public string seqid { get; set; }
                /// <summary>
                /// ID
                /// </summary>
                [DataMember]
                public string id { get; set; }
                /// <summary>
                /// 零税率标识  零税率标识 默认0 1、免税 税率填写0  2、不征收 税率填写0
                /// </summary>
                [DataMember]
                public string zerotaxratestate { get; set; }
                /// <summary>
                /// 优待政策标识 优惠政策标识 0无优惠 1有优惠
                /// </summary>
                [DataMember]
                public string preferentialstate { get; set; }
                /// <summary>
                /// 优待政策说明
                /// </summary>
                [DataMember]
                public string preferentialexplain { get; set; }
            }
        }

        /// <summary>
        /// 增加消费记录
        /// </summary>
        public class ConsumInfo
        {
            /// <summary>
            /// 请求类
            /// </summary>
            public class Request
            {
                /// <summary>
                /// 秘钥
                /// </summary>
                public string key { get; set; }
                /// <summary>
                /// 金额合计
                /// </summary>
                public decimal amount { get; set; }
                /// <summary>
                /// 单号
                /// </summary>
                public string billno { get; set; }
                /// <summary>
                /// 操作员
                /// </summary>
                public string cuser { get; set; }
                /// <summary>
                /// 客人名
                /// </summary>
                public string guest { get; set; }
                /// <summary>
                /// 锁牌号码
                /// </summary>
                public string keyno { get; set; }
                /// <summary>
                /// 营业点代码
                /// </summary>
                public string outletcode { get; set; }
                /// <summary>
                /// 营业点名称
                /// </summary>
                public string outletname { get; set; }
                /// <summary>
                /// 备注
                /// </summary>
                public string remark { get; set; }
                /// <summary>
                /// 房号
                /// </summary>
                public string roomno { get; set; }
                /// <summary>
                /// 查询字段
                /// </summary>
                public string searchtext { get; set; }
                /// <summary>
                /// 班次
                /// </summary>
                public string shift { get; set; }
                /// <summary>
                /// 营业日
                /// </summary>
                public string transbsnsdate { get; set; }
                /// <summary>
                /// 消费时间
                /// </summary>
                public string transdate { get; set; }
                /// <summary>
                /// 消费详情
                /// </summary>
                public string transdetail { get; set; }
            }

            public class Transdetail
            {
                /// <summary>
                /// 商品代码
                /// </summary>
                public string Itemcode { get; set; }
                /// <summary>
                /// 商品名称
                /// </summary>
                public string Itemname { get; set; }
                /// <summary>
                /// 商品数量
                /// </summary>
                public string Quantity { get; set; }
                /// <summary>
                /// 商品总价
                /// </summary>
                public string Amount { get; set; }
                /// <summary>
                /// 商品单位
                /// </summary>
                public string Unit { get; set; }
            }

            /// <summary>
            /// 返回类
            /// </summary>
            public class Response
            {
                /// <summary>
                /// 错误码
                /// </summary>
                [DataMember]
                public string msg { get; set; }
                /// <summary>
                /// 原因描述
                /// </summary>
                [DataMember]
                public int code { get; set; }
                /// <summary>
                /// 回调id
                /// </summary>
                [DataMember]
                public string obj { get; set; }
            }
        }

        /// <summary>
        /// 撤销消费记录
        /// </summary>
        public class ConsumInfoRepeal
        {
            /// <summary>
            /// 请求类
            /// </summary>
            public class Request
            {
                /// <summary>
                /// 秘钥
                /// </summary>
                public string key { get; set; }

                /// <summary>
                /// 消费记录标识
                /// </summary>
                public string xf { get; set; }
            }

            /// <summary>
            /// 返回类
            /// </summary>
            public class Response
            {
                /// <summary>
                /// 提示信息
                /// </summary>
                [DataMember]
                public string msg { get; set; }
                /// <summary>
                /// 返回状态代码
                /// </summary>
                [DataMember]
                public int code { get; set; }
            }
        }

        /// <summary>
        /// 基础返回类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class BaseResponse<T>
        {
            /// <summary>
            /// 错误码
            /// </summary>
            [DataMember]
            public string msg { get; set; }
            /// <summary>
            /// 原因描述
            /// </summary>
            [DataMember]
            public int code { get; set; }
            /// <summary>
            /// 回调id
            /// </summary>
            [DataMember]
            public T obj { get; set; }
        }


    }
}
