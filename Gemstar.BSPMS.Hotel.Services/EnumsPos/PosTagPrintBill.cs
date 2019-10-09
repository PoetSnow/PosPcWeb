using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 买单未打单
    /// </summary>
    public enum PosTagPrintBill : byte
    {
        //0：不提示；1：提示是否买单；2：立即打印账单；3：提示是否打印账单；4：立即打印账单和埋脚；5：账单预览；6：提示打印账单和埋脚；9：必须打单才买单

        [Description("不提示")]
        不提示 = 0,
        [Description("提示是否买单")]
        提示是否买单 = 1,
        [Description("立即打印账单")]
        立即打印账单 = 2,
        [Description("提示是否打印账单")]
        提示是否打印账单 = 3,
        [Description("立即打印账单和埋脚")]
        立即打印账单和埋脚 = 4,
        [Description("账单预览")]
        账单预览 = 5,
        [Description("提示打印账单和埋脚")]
        提示打印账单和埋脚 = 6,
        [Description("必须打单才买单")]
        必须打单才买单 = 9
    }
}
