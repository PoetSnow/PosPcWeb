using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 消费余额是否收服务费
    /// </summary>
    public enum PosTagLimitSrv:byte
    {
        //消费余额是否收服务费：0：不收；1：收服务费；2：服务费计最低消费

        [Description("不收")]
        不收 =0,
        [Description("收服务费")]
        收服务费 =1,
        [Description("服务费计最低消费")]
        服务费计最低消费 =2
    }
}
