using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 零价项目处理方式
    /// </summary>
    public enum PosTagZeroBill:byte
    {
        //0：不处理；1：提示；9：不能通过
        [Description("不处理")]
        不处理 =0,
        [Description("提示")]
        提示 =1,
        [Description("不能通过")]
        不能通过 =9
    }
}
