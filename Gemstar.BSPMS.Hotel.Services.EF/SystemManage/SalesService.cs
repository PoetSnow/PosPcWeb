using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class SalesService : CRUDService<Sales>, ISalesService
    {
        public SalesService(DbHotelPmsContext db) : base(db, db.Sales)
        {
            _pmsContext = db;
        }
        protected override Sales GetTById(string id)
        {
            return new Sales { Id = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 模糊查询指定酒店内的业务员资料
        /// </summary>
        /// <param name="hid">指定酒店ID</param>
        /// <param name="nameOrMobile">业务员姓名或手机号</param>
        /// <param name="notName">不包括的业务员姓名</param>
        /// <returns></returns>
        public List<Sales> List(string hid, string nameOrMobile, string notName = null)
        {
            if (!string.IsNullOrWhiteSpace(notName))
            {
                return _pmsContext.Sales.Where(c => c.Hid == hid && (c.Name.Contains(nameOrMobile) || c.Mobile.Contains(nameOrMobile)) && c.Name != notName).ToList();
            }
            else
            {
                return _pmsContext.Sales.Where(c => c.Hid == hid && (c.Name.Contains(nameOrMobile) || c.Mobile.Contains(nameOrMobile))).ToList();
            }
        }

        /// <summary>
        /// 模糊查询指定酒店内的业务员资料
        /// </summary>
        /// <param name="hid">指定酒店ID</param>
        /// <param name="nameOrMobile">业务员姓名或手机号</param>
        /// <param name="notName">不包括的业务员姓名</param>
        /// <returns></returns>
        public List<Sales> List(string hid)
        {
            return _pmsContext.Sales.Where(c => c.Hid == hid).ToList();
        }


        /// <summary>
        /// 批量更改状态（启用，禁用）
        /// </summary>
        /// <param name="ids">要更改的id，多项之间以逗号分隔</param>
        /// <param name="status">更新为当前状态</param>
        /// <returns>更改结果</returns>
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
                    Sales update = _pmsContext.Sales.Find(Guid.Parse(id));
                    if (update.Status != status)
                    {
                        update.Status = status;
                        _pmsContext.Entry(update).State = EntityState.Modified;
                    }
                }
                _pmsContext.AddDataChangeLogs(OpLogType.业务员启用禁用);
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 验证是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="salesName">业务员</param>
        /// <returns></returns>
        public bool Exists(string hid, string salesName, Guid? notId = null)
        {
            if (notId == null)
            {
                return _pmsContext.Sales.Where(c => c.Hid == hid && c.Name == salesName).Any();
            }
            else
            {
                var id = notId.Value;
                return _pmsContext.Sales.Where(c => c.Hid == hid && c.Name == salesName && c.Id != id).Any();
            }
        }
        public bool GroupControlAdd(Sales model, string grpid)
        {
            //集团的业务员增加
            Sales entity = new Sales
            {
                Id = Guid.NewGuid(),
                Hid = grpid,
                Grpid = grpid,
                Name = model.Name,
                Mobile = model.Mobile,
                Email = model.Email,
                Status = model.Status,
                Belonghotel = model.Belonghotel
            };
            _pmsContext.Sales.Add(entity);
            _pmsContext.SaveChanges();
            //var hotel = _pmsContext.PmsHotels.Where(w => w.Grpid == grpid && w.Hid.Contains(model.Belonghotel));
            if (string.IsNullOrEmpty(model.Belonghotel)) { return true; }
            string[] hotelids = model.Belonghotel.Split(',');
            foreach (var h in hotelids)
            {
                if (_pmsContext.Sales.Where(w => w.Hid == h && w.Name == model.Name).Count() <= 0)
                {
                    entity = new Sales
                    {
                        Id = Guid.NewGuid(),
                        Hid = h,
                        Grpid = grpid,
                        Name = model.Name,
                        Mobile = model.Mobile,
                        Email = model.Email,
                        Status = model.Status
                    };
                    _pmsContext.Sales.Add(entity);
                }
            }
            _pmsContext.SaveChanges();
            return true;
        }
        public bool GroupControlEdit(Sales model, Sales orimodel, string grpid)
        {
            string[] hotel = (model.Belonghotel + "," + grpid).Split(',');
            foreach (var h in hotel)
            {
                Sales sale = _pmsContext.Sales.Where(w => w.Hid == h && w.Name == orimodel.Name).FirstOrDefault();
                if (sale == null)
                {
                    Sales entity = new Sales
                    {
                        Id = Guid.NewGuid(),
                        Hid = h,
                        Grpid = grpid,
                        Name = model.Name,
                        Mobile = model.Mobile,
                        Email = model.Email,
                        Status = model.Status
                    };
                    if (grpid == h)
                    {
                        entity.Belonghotel = model.Belonghotel;
                    }
                    _pmsContext.Sales.Add(entity); _pmsContext.SaveChanges();
                }
                else
                {
                    sale.Name = model.Name;
                    sale.Mobile = model.Mobile;
                    sale.Email = model.Email;
                    if (grpid == h)
                    {
                        sale.Belonghotel = model.Belonghotel;
                    }
                    _pmsContext.Entry(sale).State = EntityState.Modified; _pmsContext.SaveChanges();
                }
            }

            return true;
        }

        public string getGrouphotelid(string id)    
        {
            string[] arrid = id.Split(',');
            string str = "";
            for (int j = 0; j < arrid.Length; j++)
            {
                Guid ids = Guid.Parse(arrid[j]);
                Sales s = _pmsContext.Sales.AsNoTracking().Where(w => w.Id == ids).FirstOrDefault();
                Guid[] sale = _pmsContext.Sales.AsNoTracking().Where(w => w.Name == s.Name && w.Grpid == s.Hid).Select(w => w.Id).ToArray(); 
                for (int i = 0; i < sale.Count(); i++)
                {
                    str += sale[i].ToString() + ",";
                }
            } 
            return str.Trim(',');
        }
    }
}
