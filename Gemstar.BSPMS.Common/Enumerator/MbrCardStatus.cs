using System.ComponentModel;

namespace Gemstar.BSPMS.Common.Enumerator
{
    public enum MbrCardStatus
    {
        [Description("正常")]
        Nomal = 1,
        [Description("禁用")]
        Disabled = 51,
        [Description("作废")]
        Abandon = 52,
        [Description("过期")]
        Expired = 53,
        [Description("挂失")]
        Lose = 54,
    }
}
