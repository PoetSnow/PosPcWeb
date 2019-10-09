using Gemstar.BSPMS.Common.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.AuthManages {
    /// <summary>
    /// 产品类型，其数值表示在掩码中的位置
    /// </summary>
    public enum ProductType : byte
    {
        /// <summary>
        /// 捷云pms
        /// </summary>
        Pms = 1,
        /// <summary>
        /// 会员
        /// </summary>
        Member = 2,
        /// <summary>
        /// 合约单位
        /// </summary>
        Corp = 3,
        /// <summary>
        /// 长租公寓
        /// </summary>
        Permanent = 4,
        /// <summary>
        /// pos收银
        /// </summary>
        Pos = 5,
        /// <summary>
        /// GS康养系统
        /// </summary>
        Pension = 6,
        /// <summary>
        /// 云工程
        /// </summary>
        Engineering = 7,
        /// <summary>
        /// 汇颐康养系统
        /// </summary>
        HYHIS = 8,
        /// <summary>
        /// 云游艇系统
        /// </summary>
        Yacht = 9,
        /// <summary>
        /// 集团系统
        /// </summary>
        Group = 10,
        /// <summary>
        /// 云温泉系统
        /// </summary>
        WQ = 11,
        /// <summary>
        /// 发票系统
        /// </summary>
        INVOICE = 12,
    }
    public static class ProductTypeHelper {
        /// <summary>
        /// 根据产品实例获取对应的产品类型，默认为pms捷云
        /// </summary>
        /// <param name="product">产品实例</param>
        /// <returns>产品类型</returns>
        public static ProductType GetProductType(M_v_products product) {

            if (product != null)
            {
                //1
                if (product.Code.Equals("pms", System.StringComparison.OrdinalIgnoreCase))
                {
                    return ProductType.Pms;
                }
                //2
                if (product.Code.Equals("member", System.StringComparison.OrdinalIgnoreCase))
                {
                    return ProductType.Member;
                }
                //3
                if (product.Code.Equals("corp", System.StringComparison.OrdinalIgnoreCase))
                {
                    return ProductType.Corp;
                }
                //4
                if (product.Code.Equals("Permanent", System.StringComparison.OrdinalIgnoreCase))
                {
                    return ProductType.Permanent;
                }
                //5
                if (product.Code.Equals("pos", System.StringComparison.OrdinalIgnoreCase))
                {
                    return ProductType.Pos;
                }
                //6
                if (product.Code.Equals("pension", System.StringComparison.OrdinalIgnoreCase))
                {
                    return ProductType.Pension;
                }
                //7
                if (product.Code.Equals("engineering", System.StringComparison.OrdinalIgnoreCase))
                {
                    return ProductType.Engineering;
                }
                //8
                if (product.Code.Equals("hyhis", System.StringComparison.OrdinalIgnoreCase))
                {
                    return ProductType.HYHIS;
                }
                //9
                if (product.Code.Equals("yacht", System.StringComparison.OrdinalIgnoreCase))
                {
                    return ProductType.Yacht;
                }
                //10
                if (product.Code.Equals("group", System.StringComparison.OrdinalIgnoreCase))
                {
                    return ProductType.Group;
                }
                //11
                if (product.Code.Equals("wq", System.StringComparison.OrdinalIgnoreCase))
                {
                    return ProductType.WQ;
                }
                //invoice 12
                if (product.Code.Equals("invoice", System.StringComparison.OrdinalIgnoreCase))
                {
                    return ProductType.INVOICE;
                }
            }
            return ProductType.Pos;
        }

        public static string GetProductName(ProductType type)
        {
            if (type == ProductType.Member)
            {
                return "会员系统";
            }
            if (type == ProductType.Corp)
            {
                return "合约单位系统";
            }
            if (type == ProductType.Permanent)
            {
                return "长租公寓系统";
            }
            if (type == ProductType.Pos)
            {
                return "云pos系统";
            }
            if (type == ProductType.Pension)
            {
                return "养老公寓系统";
            }
            if (type == ProductType.Engineering)
            {
                return "云工程系统";
            }
            if (type == ProductType.Group)
            {
                return "集团系统";
            }
            if (type == ProductType.INVOICE)
            {
                return "发票系统";
            }
            return "捷云系统";
        }
    }
}
