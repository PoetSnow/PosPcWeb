using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System.Collections.Generic;
using System.Linq;
using System;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System.Data.SqlClient;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos出品打印机资料服务实现
    public class PosProdPrinterService : CRUDService<PosProdPrinter>, IPosProdPrinterService
    {
        private DbHotelPmsContext _db;

        public PosProdPrinterService(DbHotelPmsContext db) : base(db, db.PosProdPrinters)
        {
            _db = db;
        }

        protected override PosProdPrinter GetTById(string id)
        {
            return new PosProdPrinter { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的出品打印机是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">出品打印机代码</param>
        /// <param name="name">出品打印机名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的出品打印机了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosProdPrinters.Any(w => w.Hid == hid && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 判断指定的代码或者名称的出品打印机是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">出品打印机代码</param>
        /// <param name="name">出品打印机名称</param>
        /// <param name="exceptId">要排队的出品打印机id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的出品打印机了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosProdPrinters.Any(w => w.Hid == hid && w.Id != exceptId && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 获取指定酒店下的出品打印机列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店和模块下的出品打印机列表</returns>
        public List<PosProdPrinter> GetPosProdPrinter(string hid)
        {
            return _db.PosProdPrinters.Where(w => w.Hid == hid && (w.IStatus < (byte)EntityStatus.禁用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块下的出品打印机列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的出品打印机列表</returns>
        public List<PosProdPrinter> GetPosProdPrinterByModule(string hid, string moduleCode)
        {
            return _db.PosProdPrinters.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus < (byte)EntityStatus.禁用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块下的故障打印机信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        public List<PosProdPrinter> GetPosProdPrinterByFault(string hid, string moduleCode)
        {
            return _db.PosProdPrinters.Where(w => w.Hid == hid && w.Module == moduleCode && w.IStatus == (byte)PosPordPrinterStatus.故障 && (w.IStatus < (byte)EntityStatus.禁用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 获取指定酒店下的出品打印机列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店和模块下的出品打印机列表</returns>
        public List<up_Pos_list_ProducelistResult> GetPosProdPrinterResult(string hid)
        {
            return _db.Database.SqlQuery<up_Pos_list_ProducelistResult>("exec up_Pos_list_Producelist @hid=@hid", new SqlParameter("@hid", hid)).ToList();
        }

        /// <summary>
        /// 根据指定酒店和代码获取出品打印机
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">代码</param>
        /// <returns>指定酒店和模块下的出品打印机列表</returns>
        public PosProdPrinter GetPosProdPrinterByCode(string hid, string code)
        {
            return _db.PosProdPrinters.Where(w => w.Hid == hid && w.Code == code).FirstOrDefault();
        }

        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public JsonResultData BatchUpdateStatus(string ids, EntityStatus status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ids))
                {
                    return JsonResultData.Failure("请指定要修改的记录id，多项之间以逗号分隔");
                }
                var idArray = ids.Split(',');
                foreach (var id in idArray)
                {
                    var update = _db.PosProdPrinters.Find(id);

                    if (update.IStatus != (byte)status)
                    {
                        update.IStatus = (byte)status;
                        _db.Entry(update).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(OpLogType.Pos部门类别启用禁用);
                _db.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
    }
}