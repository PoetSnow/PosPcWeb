using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// 操作员折扣设置 服务接口
    /// </summary>
    public interface IPosOperDiscountService : ICRUDService<PosOperDiscount>
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="grpId"></param>
        /// <returns></returns>
        List<PmsUser> GetPmsUserList(string grpId);

        /// <summary>
        /// 判断数是否存在
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="userId">用户Id</param>
        /// <param name="module">模块</param>
        /// <returns></returns>
        bool IsExists(string hid, string userId, string module);

        /// <summary>
        /// 落单判断赠送，点菜限额
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="billId">账单Id</param>
        /// <param name="module">模块</param>
        /// <param name="refeId">营业点Id</param>
        void cmpOperDiscount(string hid, string billId, string module, string refeId);


        /// <summary>
        /// 根据操作员Id,用户Id,营业点id,模块 获取数据
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="refeId">营业点Id</param>
        /// <param name="module">模块</param>
        /// <returns></returns>
        PosOperDiscount GetOperDiscountByUserId(string hid, string userId, string refeId, string module);
    }
}
