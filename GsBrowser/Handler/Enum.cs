using System.ComponentModel;

namespace GsBrowser
{
    public enum CateringServicesType
    {
        [Description("餐饮")]
        A,
        [Description("快餐")]
        B,
        [Description("零售")]
        C,
        [Description("海鲜池")]
        A4
    }

    public enum StartPage
    {
        默认,
        楼面,
        入单,
        收银,
    }
}
