using System;

namespace Gemstar.BSPMS.Common.PayManage.WxProviderPay
{
    /// <summary>
    /// 微信支付商家数据
    /// 主要用于传递一些在回调时必须的参数
    /// </summary>
    public class WxPayAttach
    {
        /// <summary>
        /// 内部数据分隔符
        /// </summary>
        private const char SplitChar = '|';
        /// <summary>
        /// 酒店id
        /// </summary>
        public string HotelId { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public PayProductType ProductType { get; set; }
        /// <summary>
        /// 获取微信支付商家数据
        /// </summary>
        /// <returns>商家数据字符串表示</returns>
        public string GetAttachStr()
        {
            return string.Format("{0}{1}{2}", HotelId, SplitChar, ProductType.ToString());
        }
        /// <summary>
        /// 解析微信支付商家数据
        /// </summary>
        /// <param name="attachStr">商家数据字符串</param>
        /// <param name="hid">酒店id</param>
        /// <returns>商家数据对象实例</returns>
        public static WxPayAttach ParseAttrachStr(string attachStr,string hid)
        {
            if (string.IsNullOrWhiteSpace(attachStr))
            {
                return new WxPayAttach
                {
                    HotelId = hid,
                    ProductType = PayProductType.ResFolio
                };
            }
            var infos = attachStr.Split(SplitChar);
            if(infos.Length < 2)
            {
                return new WxPayAttach
                {
                    HotelId = hid,
                    ProductType = PayProductType.ResFolio
                };
            }
            return new WxPayAttach
            {
                HotelId = infos[0],
                ProductType = (PayProductType)Enum.Parse(typeof(PayProductType), infos[1])
            };
        }
    }
}
