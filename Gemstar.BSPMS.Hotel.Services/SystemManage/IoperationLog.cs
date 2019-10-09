using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Services.SystemManage
{
    public interface IoperationLog
    {
        List<UpQueryOperationLog> GetOperationLog(ResLogQueryPara para);
        /// <summary>
        /// 获取单个客单日志
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="type">类型（1=主单ID，2=子单ID）</param>
        /// <param name="resid">主单ID 或 子单ID</param>
        /// <param name="keywords">内容关键字</param>
        /// <returns></returns>
        List<OpLog> GetCustomerOrderLog(string hid, int type, string id, string keywords);
        List<PmsUser> GetPmsUser(string hid);
        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="type">操作类型</param>
        /// <param name="text">操作内容</param>
        /// <param name="user">操作员</param>
        /// <param name="ip">操作IP</param>
        /// <param name="keys">关键字</param>
        void AddOperationLog(string hid, OpLogType type, string text, string user, string ip, string keys = "");

        /// <summary>
        /// 添加订单操作日志
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="user">操作员</param>
        /// <param name="ip">操作IP</param>
        /// <param name="regids">账号,多个之间用逗号隔开</param>
        /// <param name="type">类型（0换房，1调价，2迟付）</param>
        /// <param name="value1">旧值</param>
        /// <param name="value2">新值</param>
        /// <param name="other1">其他内容 旧值</param>
        /// <param name="other2">其他内容 新值</param>
        /// <param name="remark">手动备注</param>
        /// <param name="describle">描述</param>
        Guid AddResLog(string hid, string user, string ip, string regids, int type, string value1, string value2, string other1, string other2, string remark, string describle);
    }
}
