using System;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Tools;


namespace Gemstar.BSPMS.Hotel.Services
{
    public interface IImportExcelService
    {
        /// <summary>
        /// 获取所有表
        /// </summary>
        /// <returns>key表名,value存储过程名</returns>
        List<KeyValuePairModel<string, string>> GetList();

        /// <summary>
        /// 获取表中所有列
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>key列名,value列类型</returns>
        List<KeyValuePairModel<string, string>> GetTableColumns(string tableName);

        /// <summary>
        /// 执行存储过程，使导入的资料执行到业务表中
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="batch">批次ID</param>
        /// <param name="isDelete">是否删除业务表中的旧数据</param>
        upJsonResultData<string> Save(string procName, ICurrentInfo currentInfo, Guid batch, bool isDelete = false);

        /// <summary>
        /// 执行存储过程，删除当天导入的数据
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="currentInfo">当前登录用户信息</param>
        upJsonResultData<string> Delete(string procName, ICurrentInfo currentInfo);
    }
}
