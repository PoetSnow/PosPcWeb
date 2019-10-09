using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    /// <summary>
    /// 订单明细表及相关联的所有信息
    /// </summary>
    public class ResDetailAllInfo
    {
        /// <summary>
        /// 主单信息
        /// </summary>
        public Entities.Res ResEntity { get; set; }

        /// <summary>
        /// 子单信息
        /// </summary>
        public Entities.ResDetail ResDetailEntity { get; set; }

        /// <summary>
        /// 价格信息
        /// </summary>
        public List<Entities.ResDetailPlan> ResDetailPlans { get; set; }

        /// <summary>
        /// 客人信息
        /// </summary>
        public List<Entities.RegInfo> RegInfos { get; set; }
    }
}
