using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    /// <summary>
    /// 预订客账的主单信息
    /// 其中包含客账界面需要的所有信息
    /// </summary>
    public class ResFolioMainInfo
    {
        public List<ResFolioDetailInfo> FolioDetails { get; set; }
        public string CurrentUserName { get; set; }
    }
}
