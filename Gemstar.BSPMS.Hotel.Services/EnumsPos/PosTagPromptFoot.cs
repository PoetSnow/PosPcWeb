using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 买单后埋脚方式
    /// </summary>
    public enum PosTagPromptFoot : byte
    {
        //0：不提示；1：提示；2：必须
        [Description("不提示")]
        不提示 = 0,
        [Description("提示")]
        提示 = 1,
        [Description("必须")]
        必须 = 2
    }
}
