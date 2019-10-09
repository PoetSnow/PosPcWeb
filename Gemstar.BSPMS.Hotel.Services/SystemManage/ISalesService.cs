using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface ISalesService : ICRUDService<Sales>
    {
        /// <summary>
        /// 模糊查询指定酒店内的业务员资料
        /// </summary>
        /// <param name="hid">指定酒店ID</param>
        /// <param name="nameOrMobile">业务员姓名或手机号</param>
        /// <param name="notName">不包括的业务员姓名</param>
        /// <returns></returns>
        List<Sales> List(string hid, string nameOrMobile, string notName = null);

        /// <summary>
        /// 模糊查询指定酒店内的业务员资料
        /// </summary>
        /// <param name="hid">指定酒店ID</param>
        /// <param name="nameOrMobile">业务员姓名或手机号</param>
        /// <param name="notName">不包括的业务员姓名</param>
        /// <returns></returns>
        List<Sales> List(string hid);

        /// <summary>
        /// 批量更改状态（启用，禁用）
        /// </summary>
        /// <param name="ids">要更改的id，多项之间以逗号分隔</param>
        /// <param name="status">更新为当前状态</param>
        /// <returns>更改结果</returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);

        /// <summary>
        /// 验证是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="salesName">业务员</param>
        /// <returns></returns>
        bool Exists(string hid, string salesName, System.Guid? notId = null);
        /// <summary>
        /// 集团增加下发分店数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="grpid"></param>
        /// <returns></returns>
        bool GroupControlAdd(Sales model, string grpid);
        /// <summary>
        /// 集团修改下发分店数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="grpid"></param>
        /// <returns></returns>
        bool GroupControlEdit(Sales model, Sales orimodel, string grpid);
        /// <summary>
        /// 获取和当前id同业务员的集团各分店对应的业务员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string getGrouphotelid(string id);
    }
}
