using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Data.SqlClient;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Common.Services;
using System.Data;
using System.Data.SqlClient;

namespace Gemstar.BSPMS.Hotel.Services.EF.SystemManage
{
    public class ImportExcelService : IImportExcelService
    {
        public ImportExcelService(DbHotelPmsContext db)
        {
            _pmsContext = db;
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 获取所有表
        /// </summary>
        /// <returns>key表名,value存储过程名</returns>
        public List<KeyValuePairModel<string,string>> GetList()
        {
            string sql = @"select tableName as [Key],procName as [Value] from 
                                (
                                select
                                id as tableId,
                                name as tableName, 
                                REPLACE(name,'导入','') as tableType 
                                from [sysobjects] 
                                where 
                                xtype='U' 
                                and name like '导入%'
                                ) a
                                ,
                                (
                                select 
                                id as procId,
                                name as procName, 
                                REPLACE(name,'up_pos_import导入','') as procType 
                                from [sysobjects] 
                                where 
                                xtype='P' 
                                and 
                                name like 'up_pos_import导入%'
                                )b
                                where a.tableType = b.procType";

            return _pmsContext.Database.SqlQuery<KeyValuePairModel<string, string>>(sql).Distinct().ToList();
        }

        /// <summary>
        /// 获取表中所有列
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>key列名,value列类型</returns>
        public List<KeyValuePairModel<string, string>> GetTableColumns(string tableName)
        {
            string sql = string.Format(@"select 
                             c.name as [Key]
                            ,t.name as [Value]
                            --,c.length
                            --,(case t.name when 'nvarchar' then c.length/2 when 'nchar' then c.length/2 else c.length end) as reallength
                            from [syscolumns] c join [systypes] t
                            on c.xtype=t.xtype
                            where t.name <> 'sysname' and c.id = OBJECT_ID('{0}')
                            order by colorder asc", tableName);

            return _pmsContext.Database.SqlQuery<KeyValuePairModel<string, string>>(sql).Distinct().ToList();
        }

        /// <summary>
        /// 执行存储过程，使导入的资料执行到业务表中
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="batch">批次ID</param>
        /// <param name="isDelete">是否删除业务表中的旧数据</param>
        public upJsonResultData<string> Save(string procName, ICurrentInfo currentInfo, Guid batch, bool isDelete = false)
        {
            string sql = string.Format("exec {0} @hid='{1}',@batch='{2}',@isdelete={3},@shiftId='{4}',@userName='{5}'",
                procName, 
                currentInfo.HotelId, 
                batch.ToString().ToUpper(), 
                (isDelete ? "1" : "0"),
                currentInfo.ShiftId,
                currentInfo.UserName
                );
            _pmsContext.Database.CommandTimeout = 180;
            var result = _pmsContext.Database.SqlQuery<upJsonResultData<string>>(sql).FirstOrDefault();
            _pmsContext.Database.CommandTimeout = null;
            return result;
        }

        /// <summary>
        /// 执行存储过程，删除当天导入的数据
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="currentInfo">当前登录用户信息</param>
        public upJsonResultData<string> Delete(string procName, ICurrentInfo currentInfo)
        {
            string sql = string.Format("exec {0} @hid='{1}',@batch='',@isdelete=1,@shiftId='{2}',@userName='{3}'",
                procName,
                currentInfo.HotelId,
                currentInfo.ShiftId,
                currentInfo.UserName
                );
            _pmsContext.Database.CommandTimeout = 180;
            var result = _pmsContext.Database.SqlQuery<upJsonResultData<string>>(sql).FirstOrDefault();
            _pmsContext.Database.CommandTimeout = null;
            return result;
        }

    }
}
